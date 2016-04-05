using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Payments;
using System;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// Order processing service interface
    /// </summary>
    public partial interface IOrderProcessingService
    {
        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Validated order</returns>
        void CheckSubscriptionOrderStatus(SubscriptionOrder order);

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        void DeleteOrder(SubscriptionOrder order);


        int PlaceBorrowItem(List<BorrowCartItem> sc);

        /// <summary>
        /// Send a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void Ship(Shipment shipment, bool notifyCustomer);
        
        /// <summary>
        /// Marks a shipment as delivered
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void Deliver(Shipment shipment, bool notifyCustomer);



        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        bool CanCancelOrder(SubscriptionOrder order);

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void CancelOrder(SubscriptionOrder order, bool notifyCustomer);



        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        bool CanMarkOrderAsAuthorized(SubscriptionOrder order);

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">Order</param>
        void MarkAsAuthorized(SubscriptionOrder order);


 

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        bool CanVoidOffline(SubscriptionOrder order);

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        void VoidOffline(SubscriptionOrder order);




        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        void ReOrder(SubscriptionOrder order);
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        bool IsReturnRequestAllowed(SubscriptionOrder order);



        /// <summary>
        /// Valdiate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        bool ValidateMinOrderSubtotalAmount(IList<BorrowCartItem> cart);

        /// <summary>
        /// Valdiate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        bool ValidateMinOrderTotalAmount(IList<BorrowCartItem> cart);
    }
}
