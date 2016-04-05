using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder service interface
    /// </summary>
    public partial interface ISubscriptionOrderService
    {
        #region SubscriptionOrders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>SubscriptionOrder</returns>
        SubscriptionOrder GetOrderById(int orderId);


        SubscriptionOrder GetCurrentSubscribedOrder(int CustomerId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">SubscriptionOrder identifiers</param>
        /// <returns>SubscriptionOrder</returns>
        IList<SubscriptionOrder> GetSubscriptionOrdersByIds(int[] orderIds);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>SubscriptionOrder</returns>
        SubscriptionOrder GetOrderByGuid(Guid orderGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        void DeleteSubscriptionOrder(SubscriptionOrder order);

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; null to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; null to load all orders</param>
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
        /// <param name="orderGuid">Search by order GUID (Global unique identifier) or part of GUID. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>SubscriptionOrders</returns>
        IPagedList<SubscriptionOrder> SearchSubscriptionOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int planId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            string billingEmail = null, string billingLastName = "", 
            string orderNotes = null, string orderGuid = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        IList<SubscriptionOrder> GetSubscriptionOrdersForCart(int customerId = 0,
          int planId = 0, DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
          SubscriptionOrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null);
        
        
        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        void InsertSubscriptionOrder(SubscriptionOrder order);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        void UpdateSubscriptionOrder(SubscriptionOrder order);

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>SubscriptionOrder</returns>
        SubscriptionOrder GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, string paymentMethodSystemName);
        
        #endregion

        #region SubscriptionOrders items
        
        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">SubscriptionOrder item identifier</param>
        /// <returns>SubscriptionOrder item</returns>
        SubscriptionOrderItem GetSubscriptionOrderItemById(int orderItemId);

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemGuid">SubscriptionOrder item identifier</param>
        /// <returns>SubscriptionOrder item</returns>
        SubscriptionOrderItem GetSubscriptionOrderItemByGuid(Guid orderItemGuid);

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
        /// <returns>SubscriptionOrder items</returns>
        /// 
        IList<SubscriptionOrderItem> GetAllSubscriptionOrderItems(int? orderId,
           int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc, 
           SubscriptionOrderStatus? os, PaymentStatus? ps, ShippingStatus? ss,
           bool loadDownloablePlansOnly = false);

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        void DeleteSubscriptionOrderItem(SubscriptionOrderItem orderItem);

        #endregion

        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>Order item</returns>
        OrderItem GetOrderItemById(int orderItemId);
        ItemDetail GetItemDetailById(int itemDetailId);
      //  int GetOrderItemBatchId(int orderid);
        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemGuid">Order item identifier</param>
        /// <returns>Order item</returns>
        OrderItem GetOrderItemByGuid(Guid orderItemGuid);

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
        /// <returns>Order items</returns>
        IList<OrderItem> GetAllOrderItems(int? orderId,
           int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc,
           SubscriptionOrderStatus? os, PaymentStatus? ps, ShippingStatus? ss,
           bool loadDownloableProductsOnly = false);

        IList<OrderItem> GetAllOrderItemsCount(int? orderId,
           int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc,
           SubscriptionOrderStatus? os, PaymentStatus? ps, ShippingStatus? ss
           );

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        void DeleteOrderItem(OrderItem orderItem);

        #endregion

        #region SubscriptionOrder notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>SubscriptionOrder note</returns>
        SubscriptionOrderNote GetSubscriptionOrderNoteById(int orderNoteId);

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        void DeleteSubscriptionOrderNote(SubscriptionOrderNote orderNote);

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void DeleteRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        RecurringPayment GetRecurringPaymentById(int recurringPaymentId);

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void InsertRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void UpdateRecurringPayment(RecurringPayment recurringPayment);

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
        IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialSubscriptionOrderId = 0, SubscriptionOrderStatus? initialSubscriptionOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        #endregion
    }
}
