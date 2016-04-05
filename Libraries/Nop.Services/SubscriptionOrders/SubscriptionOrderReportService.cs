using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Helpers;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder report service
    /// </summary>
    public partial class SubscriptionOrderReportService : ISubscriptionOrderReportService
    {
        #region Fields

        private readonly IRepository<SubscriptionOrder> _orderRepository;
        private readonly IRepository<SubscriptionOrderItem> _orderItemRepository;
        private readonly IRepository<Plan> _productRepository;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">SubscriptionOrder repository</param>
        /// <param name="orderItemRepository">SubscriptionOrder item repository</param>
        /// <param name="productRepository">Plan repository</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public SubscriptionOrderReportService(IRepository<SubscriptionOrder> orderRepository,
            IRepository<SubscriptionOrderItem> orderItemRepository,
            IRepository<Plan> productRepository,
            IDateTimeHelper dateTimeHelper)
        {
            this._orderRepository = orderRepository;
            this._orderItemRepository = orderItemRepository;
            this._productRepository = productRepository;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get "order by country" report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">SubscriptionOrder status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        public virtual IList<SubscriptionOrderByCountryReportLine> GetCountryReport(int storeId, SubscriptionOrderStatus? os,
            PaymentStatus? ps, ShippingStatus? ss, DateTime? startTimeUtc, DateTime? endTimeUtc)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderStatusId.HasValue)
                query = query.Where(o => o.SubscriptionOrderStatusId == orderStatusId.Value);
            if (paymentStatusId.HasValue)
                query = query.Where(o => o.PaymentStatusId == paymentStatusId.Value);
            if (shippingStatusId.HasValue)
                query = query.Where(o => o.ShippingStatusId == shippingStatusId.Value);
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            
            var report = (from oq in query
                        group oq by oq.BillingAddress.CountryId into result
                        select new
                        {
                            CountryId = result.Key,
                            TotalSubscriptionOrders = result.Count(),
                            SumSubscriptionOrders = result.Sum(o => o.SubscriptionOrderTotal)
                        }
                       )
                       .OrderByDescending(x => x.SumSubscriptionOrders)
                       .Select(r => new SubscriptionOrderByCountryReportLine
                       {
                           CountryId = r.CountryId,
                           TotalSubscriptionOrders = r.TotalSubscriptionOrders,
                           SumSubscriptionOrders = r.SumSubscriptionOrders
                       })

                       .ToList();

            return report;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="orderId">SubscriptionOrder identifier; pass 0 to ignore this parameter</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="os">SubscriptionOrder status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="ignoreCancelledSubscriptionOrders">A value indicating whether to ignore cancelled orders</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual SubscriptionOrderAverageReportLine GetSubscriptionOrderAverageReportLine(int storeId = 0,
            int vendorId = 0, int billingCountryId = 0, 
            int orderId = 0, string paymentMethodSystemName = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", 
            bool ignoreCancelledSubscriptionOrders = false, string orderNotes = null)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderId > 0)
                query = query.Where(o => o.Id == orderId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.SubscriptionOrderItems
                    .Any(orderItem => orderItem.Plan.VendorId == vendorId));
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (ignoreCancelledSubscriptionOrders)
            {
                var cancelledSubscriptionOrderStatusId = (int)SubscriptionOrderStatus.Cancelled;
                query = query.Where(o => o.SubscriptionOrderStatusId != cancelledSubscriptionOrderStatusId);
            }
            if (!String.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (orderStatusId.HasValue)
                query = query.Where(o => o.SubscriptionOrderStatusId == orderStatusId.Value);
            if (paymentStatusId.HasValue)
                query = query.Where(o => o.PaymentStatusId == paymentStatusId.Value);
            if (shippingStatusId.HasValue)
                query = query.Where(o => o.ShippingStatusId == shippingStatusId.Value);
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!String.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!String.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.SubscriptionOrderNotes.Any(on => on.Note.Contains(orderNotes)));
            
			var item = (from oq in query
						group oq by 1 into result
						select new
						           {
                                       SubscriptionOrderCount = result.Count(),
                                       SubscriptionOrderShippingExclTaxSum = result.Sum(o => o.SubscriptionOrderShippingExclTax),
                                       SubscriptionOrderTaxSum = result.Sum(o => o.SubscriptionOrderTax), 
                                       SubscriptionOrderTotalSum = result.Sum(o => o.SubscriptionOrderTotal)
						           }
					   ).Select(r => new SubscriptionOrderAverageReportLine
                       {
                           CountSubscriptionOrders = r.SubscriptionOrderCount,
                           SumShippingExclTax = r.SubscriptionOrderShippingExclTaxSum, 
                           SumTax = r.SubscriptionOrderTaxSum, 
                           SumSubscriptionOrders = r.SubscriptionOrderTotalSum
                       })
                       .FirstOrDefault();

			item = item ?? new SubscriptionOrderAverageReportLine
			                   {
                                   CountSubscriptionOrders = 0,
                                   SumShippingExclTax = decimal.Zero,
                                   SumTax = decimal.Zero,
                                   SumSubscriptionOrders = decimal.Zero, 
			                   };
            return item;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">SubscriptionOrder status</param>
        /// <returns>Result</returns>
        public virtual SubscriptionOrderAverageReportLineSummary SubscriptionOrderAverageReport(int storeId, SubscriptionOrderStatus os)
        {
            var item = new SubscriptionOrderAverageReportLineSummary();
            item.SubscriptionOrderStatus = os;

            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            var t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                var todayResult = GetSubscriptionOrderAverageReportLine(storeId: storeId,
                    os: os, 
                    startTimeUtc: startTime1);
                item.SumTodaySubscriptionOrders = todayResult.SumSubscriptionOrders;
                item.CountTodaySubscriptionOrders = todayResult.CountSubscriptionOrders;
            }
            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                var weekResult = GetSubscriptionOrderAverageReportLine(storeId: storeId,
                    os: os,
                    startTimeUtc: startTime2);
                item.SumThisWeekSubscriptionOrders = weekResult.SumSubscriptionOrders;
                item.CountThisWeekSubscriptionOrders = weekResult.CountSubscriptionOrders;
            }
            //month
            var t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                var monthResult = GetSubscriptionOrderAverageReportLine(storeId: storeId,
                    os: os,
                    startTimeUtc: startTime3);
                item.SumThisMonthSubscriptionOrders = monthResult.SumSubscriptionOrders;
                item.CountThisMonthSubscriptionOrders = monthResult.CountSubscriptionOrders;
            }
            //year
            var t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                var yearResult = GetSubscriptionOrderAverageReportLine(storeId: storeId,
                    os: os,
                    startTimeUtc: startTime4);
                item.SumThisYearSubscriptionOrders = yearResult.SumSubscriptionOrders;
                item.CountThisYearSubscriptionOrders = yearResult.CountSubscriptionOrders;
            }
            //all time
            var allTimeResult = GetSubscriptionOrderAverageReportLine(storeId: storeId, os: os);
            item.SumAllTimeSubscriptionOrders = allTimeResult.SumSubscriptionOrders;
            item.CountAllTimeSubscriptionOrders = allTimeResult.CountSubscriptionOrders;

            return item;
        }

        /// <summary>
        /// Get best sellers report
        /// </summary>
        /// <param name="storeId">Store identifier (orders placed in a specific store); 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">SubscriptionOrder created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">SubscriptionOrder created date to (UTC); null to load all records</param>
        /// <param name="os">SubscriptionOrder status; null to load all records</param>
        /// <param name="ps">SubscriptionOrder payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="orderBy">1 - order by quantity, 2 - order by total amount</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Result</returns>
        public virtual IPagedList<BestsellersReportLine> BestSellersReport(
            int categoryId = 0, int manufacturerId = 0,
            int storeId = 0, int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            int billingCountryId = 0,
            int orderBy = 1,
            int pageIndex = 0, int pageSize = int.MaxValue, 
            bool showHidden = false)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            var query1 = from orderItem in _orderItemRepository.Table
                         join o in _orderRepository.Table on orderItem.SubscriptionOrderId equals o.Id
                         join p in _productRepository.Table on orderItem.PlanId equals p.Id
                         //join pc in _productCategoryRepository.Table on p.Id equals pc.PlanId into p_pc from pc in p_pc.DefaultIfEmpty()
                         //join pm in _productManufacturerRepository.Table on p.Id equals pm.PlanId into p_pm from pm in p_pm.DefaultIfEmpty()
                         where (storeId == 0 || storeId == o.StoreId) &&
                         (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                         (!orderStatusId.HasValue || orderStatusId == o.SubscriptionOrderStatusId) &&
                         (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                         (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                         (!o.Deleted) &&
                         (!p.Deleted) &&
                         (vendorId == 0 || p.VendorId == vendorId) &&
                         //(categoryId == 0 || pc.CategoryId == categoryId) &&
                         //(manufacturerId == 0 || pm.ManufacturerId == manufacturerId) &&
                         (categoryId == 0 || p.PlanMembershipCategories.Count(pc => pc.MembershipCategoryId == categoryId) > 0) &&
                         (billingCountryId == 0 || o.BillingAddress.CountryId == billingCountryId) &&
                         (showHidden || p.Published)
                         select orderItem;

            IQueryable<BestsellersReportLine> query2 = 
                //group by products
                from orderItem in query1
                group orderItem by orderItem.PlanId into g
                select new BestsellersReportLine
                {
                    PlanId = g.Key,
                    TotalAmount = g.Sum(x => x.PriceExclTax),
                    TotalQuantity = g.Sum(x => x.Quantity),
                }
                ;

            switch (orderBy)
            {
                case 1:
                    {
                        query2 = query2.OrderByDescending(x => x.TotalQuantity);
                    }
                    break;
                case 2:
                    {
                        query2 = query2.OrderByDescending(x => x.TotalAmount);
                    }
                    break;
                default:
                    throw new ArgumentException("Wrong orderBy parameter", "orderBy");
            }

            var result = new PagedList<BestsellersReportLine>(query2, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets a list of products (identifiers) purchased by other customers who purchased a specified product
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="productId">Plan identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Plans</returns>
        public virtual int[] GetAlsoPurchasedPlansIds(int storeId, int productId,
            int recordsToReturn = 5, bool showHidden = false)
        {
            if (productId == 0)
                throw new ArgumentException("Plan ID is not specified");

            //this inner query should retrieve all orders that contains a specified product ID
            var query1 = from orderItem in _orderItemRepository.Table
                          where orderItem.PlanId == productId
                          select orderItem.SubscriptionOrderId;

            var query2 = from orderItem in _orderItemRepository.Table
                         join p in _productRepository.Table on orderItem.PlanId equals p.Id
                         where (query1.Contains(orderItem.SubscriptionOrderId)) &&
                         (p.Id != productId) &&
                         (showHidden || p.Published) &&
                         (!orderItem.SubscriptionOrder.Deleted) &&
                         (storeId == 0 || orderItem.SubscriptionOrder.StoreId == storeId) &&
                         (!p.Deleted) &&
                         (showHidden || p.Published)
                         select new { orderItem, p };

            var query3 = from orderItem_p in query2
                         group orderItem_p by orderItem_p.p.Id into g
                         select new
                         {
                             PlanId = g.Key,
                             PlansPurchased = g.Sum(x => x.orderItem.Quantity),
                         };
            query3 = query3.OrderByDescending(x => x.PlansPurchased);

            if (recordsToReturn > 0)
                query3 = query3.Take(recordsToReturn);

            var report = query3.ToList();
            
            var ids = new List<int>();
            foreach (var reportLine in report)
                ids.Add(reportLine.PlanId);

            return ids.ToArray();
        }

        /// <summary>
        /// Gets a list of products that were never sold
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="createdFromUtc">SubscriptionOrder created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">SubscriptionOrder created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Plans</returns>
        public virtual IPagedList<Plan> PlansNeverSold(int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            //this inner query should retrieve all purchased product identifiers
            var query1 = (from orderItem in _orderItemRepository.Table
                          join o in _orderRepository.Table on orderItem.SubscriptionOrderId equals o.Id
                          where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                                (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                                (!o.Deleted)
                          select orderItem.PlanId).Distinct();

            var simplePlanTypeId = (int)PlanType.SimplePlan;

            var query2 = from p in _productRepository.Table
                         orderby p.Name
                         where (!query1.Contains(p.Id)) &&
                             //include only simple products
                               (p.PlanTypeId == simplePlanTypeId) &&
                               (!p.Deleted) &&
                               (vendorId == 0 || p.VendorId == vendorId) &&
                               (showHidden || p.Published)
                         select p;

            var products = new PagedList<Plan>(query2, pageIndex, pageSize);
            return products;
        }

        /// <summary>
        /// Get profit report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="orderId">SubscriptionOrder identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="os">SubscriptionOrder status; null to load all records</param>
        /// <param name="ps">SubscriptionOrder payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual decimal ProfitReport(int storeId = 0, int vendorId = 0,
            int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchEmail = String.IsNullOrEmpty(billingEmail);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchLastName = String.IsNullOrEmpty(billingLastName);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchSubscriptionOrderNotes = String.IsNullOrEmpty(orderNotes);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchPaymentMethods = String.IsNullOrEmpty(paymentMethodSystemName);
            var query = from orderItem in _orderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.SubscriptionOrderId equals o.Id
                        where (storeId == 0 || storeId == o.StoreId) &&
                              (orderId == 0 || orderId == o.Id) &&
                              (billingCountryId ==0 || (o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId)) &&
                              (dontSearchPaymentMethods || paymentMethodSystemName == o.PaymentMethodSystemName) &&
                              (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreatedOnUtc) &&
                              (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreatedOnUtc) &&
                              (!orderStatusId.HasValue || orderStatusId == o.SubscriptionOrderStatusId) &&
                              (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                              (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                              (!o.Deleted) &&
                              (vendorId == 0 || orderItem.Plan.VendorId == vendorId) &&
                              //we do not ignore deleted products when calculating order reports
                              //(!p.Deleted)
                              (dontSearchEmail || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail))) &&
                              (dontSearchLastName || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName))) &&
                              (dontSearchSubscriptionOrderNotes || o.SubscriptionOrderNotes.Any(oNote => oNote.Note.Contains(orderNotes)))
                        select orderItem;

            var productCost = Convert.ToDecimal(query.Sum(orderItem => (decimal?)orderItem.OriginalPlanCost * orderItem.Quantity));

            var reportSummary = GetSubscriptionOrderAverageReportLine(
                storeId: storeId,
                vendorId: vendorId,
                billingCountryId: billingCountryId,
                orderId: orderId,
                paymentMethodSystemName: paymentMethodSystemName,
                os: os, 
                ps: ps, 
                ss: ss,
                startTimeUtc: startTimeUtc,
                endTimeUtc: endTimeUtc,
                billingEmail: billingEmail,
                billingLastName: billingLastName,
                orderNotes: orderNotes);
            var profit = reportSummary.SumSubscriptionOrders - reportSummary.SumShippingExclTax - reportSummary.SumTax - productCost;
            return profit;
        }

        #endregion
    }
}
