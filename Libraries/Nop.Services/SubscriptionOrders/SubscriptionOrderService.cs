using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder service
    /// </summary>
    public partial class SubscriptionOrderService : ISubscriptionOrderService
    {
        #region Fields

        private readonly IRepository<SubscriptionOrder> _orderRepository;
        private readonly IRepository<SubscriptionOrderItem> _subscriptionOrderItemRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<ItemDetail> _itemDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<SubscriptionOrderNote> _orderNoteRepository;
        private readonly IRepository<Plan> _planRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionRepository">SubscriptionOrder repository</param>
        /// <param name="subscriptionOrderItemRepository">SubscriptionOrder item repository</param>
        /// <param name="orderNoteRepository">SubscriptionOrder note repository</param>
        /// <param name="planRepository">Plan repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SubscriptionOrderService(IRepository<SubscriptionOrder> subscriptionRepository,
            IRepository<SubscriptionOrderItem> subscriptionOrderItemRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<ItemDetail> itemDetailRepository,
            IRepository<SubscriptionOrderNote> orderNoteRepository,
            IRepository<Plan> planRepository,
            IRepository<Product> productRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<Customer> customerRepository, 
            IEventPublisher eventPublisher)
        {
            this._orderRepository = subscriptionRepository;
            this._subscriptionOrderItemRepository = subscriptionOrderItemRepository;
            this._orderItemRepository = orderItemRepository;
            this._itemDetailRepository =itemDetailRepository;
            this._orderNoteRepository = orderNoteRepository;
            this._planRepository = planRepository;
            this._productRepository = productRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region SubscriptionOrders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>SubscriptionOrder</returns>
        public virtual SubscriptionOrder GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderRepository.GetById(orderId);
        }

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">SubscriptionOrder identifiers</param>
        /// <returns>SubscriptionOrder</returns>
        public virtual IList<SubscriptionOrder> GetSubscriptionOrdersByIds(int[] orderIds)
        {
            if (orderIds == null || orderIds.Length == 0)
                return new List<SubscriptionOrder>();

            var query = from o in _orderRepository.Table
                        where orderIds.Contains(o.Id)
                        select o;
            var orders = query.ToList();
            //sort by passed identifiers
            var sortedSubscriptionOrders = new List<SubscriptionOrder>();
            foreach (int id in orderIds)
            {
                var order = orders.Find(x => x.Id == id);
                if (order != null)
                    sortedSubscriptionOrders.Add(order);
            }
            return sortedSubscriptionOrders;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>SubscriptionOrder</returns>
        public virtual SubscriptionOrder GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var query = from o in _orderRepository.Table
                        where o.SubscriptionOrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }


        public virtual SubscriptionOrder GetCurrentSubscribedOrder(int CustomerId)
        {
            var query = from o in _orderRepository.Table
                        where  o.CustomerId == CustomerId
                        && (o.SubscriptionOrderStatusId == 10
                        || o.SubscriptionOrderStatusId == 20
                        || o.SubscriptionOrderStatusId == 30)
                        select o;
            try {
                var order = query.ToList();
                if (order != null)
                    return order.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception e)
            {
                
            }
            return null;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteSubscriptionOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.Deleted = true;
            UpdateSubscriptionOrder(order);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Plan identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="os">SubscriptionOrder status; null to load all orders</param>
        /// <param name="ps">SubscriptionOrder payment status; null to load all orders</param>
        /// <param name="ss">SubscriptionOrder shipment status; null to load all orders</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="orderGuid">Search by order GUID (Global unique identifier) or part of GUID. Leave empty to load all orders.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>SubscriptionOrders</returns>
        public virtual IPagedList<SubscriptionOrder> SearchSubscriptionOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            string billingEmail = null, string billingLastName = "",
            string orderNotes = null, string orderGuid = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
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
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.SubscriptionOrderItems
                    .Any(orderItem => orderItem.Plan.VendorId == vendorId));
            }
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (productId > 0)
            {
                query = query
                    .Where(o => o.SubscriptionOrderItems
                    .Any(orderItem => orderItem.Plan.Id == productId));
            }
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query
                    .Where(o => o.SubscriptionOrderItems
                    .Any(orderItem =>
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (orderItem.Plan.ManageInventoryMethodId == manageStockInventoryMethodId && 
                        orderItem.Plan.UseMultipleWarehouses )
                        ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((orderItem.Plan.ManageInventoryMethodId != manageStockInventoryMethodId ||
                        !orderItem.Plan.UseMultipleWarehouses) &&
                        orderItem.Plan.WarehouseId == warehouseId))
                        );
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!String.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (orderStatusId.HasValue)
                query = query.Where(o => orderStatusId.Value == o.SubscriptionOrderStatusId);
            if (paymentStatusId.HasValue)
                query = query.Where(o => paymentStatusId.Value == o.PaymentStatusId);
            if (shippingStatusId.HasValue)
                query = query.Where(o => shippingStatusId.Value == o.ShippingStatusId);
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!String.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!String.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.SubscriptionOrderNotes.Any(on => on.Note.Contains(orderNotes)));
            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            
           
            if (!String.IsNullOrEmpty(orderGuid))
            {
                //filter by GUID. Filter in BLL because EF doesn't support casting of GUID to string
                var orders = query.ToList();
                orders = orders.FindAll(o => o.SubscriptionOrderGuid.ToString().ToLowerInvariant().Contains(orderGuid.ToLowerInvariant()));
                return new PagedList<SubscriptionOrder>(orders, pageIndex, pageSize);
            }
            
            //database layer paging
            return new PagedList<SubscriptionOrder>(query, pageIndex, pageSize);
        }


        public virtual IList<SubscriptionOrder> GetSubscriptionOrdersForCart(int customerId = 0,
          int planId = 0, DateTime? rentalFromUtc = null, DateTime? rentalToUtc = null,
          SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null)
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
             
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (rentalFromUtc.HasValue)
                query = query.Where(o => rentalFromUtc.Value <= o.CreatedOnUtc);
            if (rentalToUtc.HasValue)
                query = query.Where(o => rentalToUtc.Value >= o.CreatedOnUtc);
            if (orderStatusId.HasValue)
                query = query.Where(o => orderStatusId.Value == o.SubscriptionOrderStatusId);
            if (paymentStatusId.HasValue)
                query = query.Where(o => paymentStatusId.Value == o.PaymentStatusId);
            if (shippingStatusId.HasValue)
                query = query.Where(o => shippingStatusId.Value == o.ShippingStatusId);
          
            query = query.Where(o => !o.Deleted);
            query = query.Where(o => o.SubscriptionOrderStatusId != 40);
            query = query.OrderByDescending(o => o.CreatedOnUtc);
           

             
            //filter by GUID. Filter in BLL because EF doesn't support casting of GUID to string
            var orders = query.ToList();
                
            return orders;
            

            //database layer paging
           
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        public virtual void InsertSubscriptionOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            _orderRepository.Insert(order);

            //event notification
            _eventPublisher.EntityInserted(order);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void UpdateSubscriptionOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            _orderRepository.Update(order);

            //event notification
            _eventPublisher.EntityUpdated(order);
        }

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>SubscriptionOrder</returns>
        public virtual SubscriptionOrder GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, 
            string paymentMethodSystemName)
        { 
            var query = _orderRepository.Table;
            if (!String.IsNullOrWhiteSpace(authorizationTransactionId))
                query = query.Where(o => o.AuthorizationTransactionId == authorizationTransactionId);
            
            if (!String.IsNullOrWhiteSpace(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            
            query = query.OrderByDescending(o => o.CreatedOnUtc);
            var order = query.FirstOrDefault();
            return order;
        }
        
        #endregion
        
        #region SubscriptionOrders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">SubscriptionOrder item identifier</param>
        /// <returns>SubscriptionOrder item</returns>
        public virtual SubscriptionOrderItem GetSubscriptionOrderItemById(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return _subscriptionOrderItemRepository.GetById(orderItemId);
        }

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="orderItemGuid">SubscriptionOrder identifier</param>
        /// <returns>SubscriptionOrder item</returns>
        public virtual SubscriptionOrderItem GetSubscriptionOrderItemByGuid(Guid orderItemGuid)
        {
            if (orderItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _subscriptionOrderItemRepository.Table
                        where orderItem.SubscriptionOrderItemGuid == orderItemGuid
                        select orderItem;
            var item = query.FirstOrDefault();
            return item;
        }
        
        /// <summary>
        /// Gets all order items
        /// </summary>
        /// <param name="orderId">SubscriptionOrder identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="createdFromUtc">SubscriptionOrder created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">SubscriptionOrder created date to (UTC); null to load all records</param>
        /// <param name="os">SubscriptionOrder status; null to load all records</param>
        /// <param name="ps">SubscriptionOrder payment status; null to load all records</param>
        /// <param name="ss">SubscriptionOrder shipment status; null to load all records</param>
        /// <param name="loadDownloablePlansOnly">Value indicating whether to load downloadable products only</param>
        /// <returns>SubscriptionOrders</returns>
        public virtual IList<SubscriptionOrderItem> GetAllSubscriptionOrderItems(int? orderId,
            int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc, 
            SubscriptionOrderStatus? os, PaymentStatus? ps, ShippingStatus? ss,
            bool loadDownloablePlansOnly)
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


            var query = from orderItem in _subscriptionOrderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.SubscriptionOrderId equals o.Id
                        join p in _planRepository.Table on orderItem.PlanId equals p.Id
                        where (!orderId.HasValue || orderId.Value == 0 || orderId == o.Id) &&
                        (!customerId.HasValue || customerId.Value == 0 || customerId == o.CustomerId) &&
                        (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                        (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                        (!orderStatusId.HasValue || orderStatusId == o.SubscriptionOrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        (!loadDownloablePlansOnly || p.IsDownload) &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        public virtual void DeleteSubscriptionOrderItem(SubscriptionOrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            _subscriptionOrderItemRepository.Delete(orderItem);

            //event notification
            _eventPublisher.EntityDeleted(orderItem);
        }

        #endregion

        #region Orders items

        public virtual ItemDetail GetItemDetailById(int itemDetailId)
        {
            if (itemDetailId == 0)
                return null;

            return _itemDetailRepository.GetById(itemDetailId);
        }

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemById(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return _orderItemRepository.GetById(orderItemId);
        }

        //public virtual int GetOrderItemBatchId(int orderid)
        //{
        //    if (orderid == 0)
        //        return 1;
        //    var query = from orderItem in _orderItemRepository.Table
        //                where orderItem.SubscriptionOrderId == orderid
        //                orderby orderItem.Id descending
        //                select orderItem;
        //    var item = query.FirstOrDefault();

        //    if (item != null)
        //        return item.Id + 1;

        //    return 1;
        //}

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="orderItemGuid">Order identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemByGuid(Guid orderItemGuid)
        {
            if (orderItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _orderItemRepository.Table
                        where orderItem.OrderItemGuid == orderItemGuid
                        select orderItem;
            var item = query.FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Gets all order items
        /// </summary>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shipment status; null to load all records</param>
        /// <param name="loadDownloableProductsOnly">Value indicating whether to load downloadable products only</param>
        /// <returns>Orders</returns>
        public virtual IList<OrderItem> GetAllOrderItems(int? orderId,
            int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc,
            SubscriptionOrderStatus? os, PaymentStatus? ps, ShippingStatus? ss,
            bool loadDownloableProductsOnly)
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


            var query = from itemDetail in _itemDetailRepository.Table
                        join orderItem in _orderItemRepository.Table on itemDetail.OrderItemId equals orderItem.Id
                        join o in _orderRepository.Table on orderItem.SubscriptionOrderId equals o.Id
                        join p in _productRepository.Table on itemDetail.ProductId equals p.Id
                        where (!orderId.HasValue || orderId.Value == 0 || orderId == o.Id) &&
                        (!customerId.HasValue || customerId.Value == 0 || customerId == o.CustomerId) &&
                        (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                        (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                        (!orderStatusId.HasValue || orderStatusId == o.SubscriptionOrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        (!loadDownloableProductsOnly || p.IsDownload) &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }

        public virtual IList<OrderItem> GetAllOrderItemsCount(int? orderId,
           int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc,
           SubscriptionOrderStatus? os, PaymentStatus? ps, ShippingStatus? ss
           )
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


            var query = from orderItem in _orderItemRepository.Table 
                        //join on itemDetail.OrderItemId equals orderItem.Id
                        join o in _orderRepository.Table on orderItem.SubscriptionOrderId equals o.Id
                        //join p in _productRepository.Table on itemDetail.ProductId equals p.Id
                        where (!orderId.HasValue || orderId.Value == 0 || orderId == o.Id) &&
                        (!customerId.HasValue || customerId.Value == 0 || customerId == o.CustomerId) &&
                        (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                        (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                        (!orderStatusId.HasValue || orderStatusId == o.SubscriptionOrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        //(!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        !o.Deleted
                        orderby orderItem.Id descending, o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        public virtual void DeleteOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            _orderItemRepository.Delete(orderItem);

            //event notification
            _eventPublisher.EntityDeleted(orderItem);
        }

        #endregion

        #region SubscriptionOrders notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>SubscriptionOrder note</returns>
        public virtual SubscriptionOrderNote GetSubscriptionOrderNoteById(int orderNoteId)
        {
            if (orderNoteId == 0)
                return null;

            return _orderNoteRepository.GetById(orderNoteId);
        }

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        public virtual void DeleteSubscriptionOrderNote(SubscriptionOrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");

            _orderNoteRepository.Delete(orderNote);

            //event notification
            _eventPublisher.EntityDeleted(orderNote);
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void DeleteRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            recurringPayment.Deleted = true;
            UpdateRecurringPayment(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public virtual RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

           return _recurringPaymentRepository.GetById(recurringPaymentId);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Insert(recurringPayment);

            //event notification
            _eventPublisher.EntityInserted(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Update(recurringPayment);

            //event notification
            _eventPublisher.EntityUpdated(recurringPayment);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialSubscriptionOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialSubscriptionOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payments</returns>
        public virtual IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialSubscriptionOrderId = 0, SubscriptionOrderStatus? initialSubscriptionOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            int? initialSubscriptionOrderStatusId = null;
            if (initialSubscriptionOrderStatus.HasValue)
                initialSubscriptionOrderStatusId = (int)initialSubscriptionOrderStatus.Value;

            var query1 = from rp in _recurringPaymentRepository.Table
                         join c in _customerRepository.Table on rp.InitialSubscriptionOrder.CustomerId equals c.Id
                         where
                         (!rp.Deleted) &&
                         (showHidden || !rp.InitialSubscriptionOrder.Deleted) &&
                         (showHidden || !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || rp.InitialSubscriptionOrder.CustomerId == customerId) &&
                         (storeId == 0 || rp.InitialSubscriptionOrder.StoreId == storeId) &&
                         (initialSubscriptionOrderId == 0 || rp.InitialSubscriptionOrder.Id == initialSubscriptionOrderId) &&
                         (!initialSubscriptionOrderStatusId.HasValue || initialSubscriptionOrderStatusId.Value == 0 || rp.InitialSubscriptionOrder.SubscriptionOrderStatusId == initialSubscriptionOrderStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;

            var recurringPayments = new PagedList<RecurringPayment>(query2, pageIndex, pageSize);
            return recurringPayments;
        }

        #endregion

        #endregion
    }
}
