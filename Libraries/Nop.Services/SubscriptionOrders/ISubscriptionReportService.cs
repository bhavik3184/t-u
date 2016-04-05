using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder report service interface
    /// </summary>
    public partial interface ISubscriptionOrderReportService
    {
        /// <summary>
        /// Get "order by country" report
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="os">SubscriptionOrder status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        IList<SubscriptionOrderByCountryReportLine> GetCountryReport(int storeId = 0, SubscriptionOrderStatus? os = null,
            PaymentStatus? ps = null, ShippingStatus? ss = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null);

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
        SubscriptionOrderAverageReportLine GetSubscriptionOrderAverageReportLine(int storeId = 0, int vendorId = 0,
            int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", 
            bool ignoreCancelledSubscriptionOrders = false, string orderNotes = null);
        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">SubscriptionOrder status</param>
        /// <returns>Result</returns>
        SubscriptionOrderAverageReportLineSummary SubscriptionOrderAverageReport(int storeId, SubscriptionOrderStatus os);

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
        IPagedList<BestsellersReportLine> BestSellersReport(
            int categoryId = 0, int manufacturerId = 0, 
            int storeId = 0, int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            int billingCountryId = 0,
            int orderBy = 1,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool showHidden = false);
        
        /// <summary>
        /// Gets a list of products (identifiers) purchased by other customers who purchased a specified product
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="productId">Plan identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Plans</returns>
        int[] GetAlsoPurchasedPlansIds(int storeId, int productId,
            int recordsToReturn = 5, bool showHidden = false);

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
        IPagedList<Plan> PlansNeverSold(int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

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
        decimal ProfitReport(int storeId = 0, int vendorId = 0,
            int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null);
    }
}
