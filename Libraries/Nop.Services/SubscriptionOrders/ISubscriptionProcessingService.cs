using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Payments;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder processing service interface
    /// </summary>
    public partial interface ISubscriptionOrderProcessingService
    {
        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Validated order</returns>
        void CheckSubscriptionOrderStatus(SubscriptionOrder order);

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        PlaceSubscriptionOrderResult PlaceSubscriptionOrder(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        void DeleteSubscriptionOrder(SubscriptionOrder order);


        /// <summary>
        /// Process next recurring psayment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void ProcessNextRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        IList<string> CancelRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment);

        void Ship(Shipment shipment, bool notifyCustomer);
         
        void Deliver(Shipment shipment, bool notifyCustomer);

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        bool CanCancelSubscriptionOrder(SubscriptionOrder order);

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void CancelSubscriptionOrder(SubscriptionOrder order, bool notifyCustomer);



        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        bool CanMarkSubscriptionOrderAsAuthorized(SubscriptionOrder order);

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        void MarkAsAuthorized(SubscriptionOrder order);



        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        bool CanCapture(SubscriptionOrder order);

        /// <summary>
        /// Capture an order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> Capture(SubscriptionOrder order);

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        bool CanMarkSubscriptionOrderAsPaid(SubscriptionOrder order);

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        void MarkSubscriptionOrderAsPaid(SubscriptionOrder order);



        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        bool CanRefund(SubscriptionOrder order);

        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> Refund(SubscriptionOrder order);

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        bool CanRefundOffline(SubscriptionOrder order);

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        void RefundOffline(SubscriptionOrder order);

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        bool CanPartiallyRefund(SubscriptionOrder order, decimal amountToRefund);

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> PartiallyRefund(SubscriptionOrder order, decimal amountToRefund);

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        bool CanPartiallyRefundOffline(SubscriptionOrder order, decimal amountToRefund);

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        void PartiallyRefundOffline(SubscriptionOrder order, decimal amountToRefund);



        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        bool CanVoid(SubscriptionOrder order);

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Voided order</returns>
        IList<string> Void(SubscriptionOrder order);

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        bool CanVoidOffline(SubscriptionOrder order);

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        void VoidOffline(SubscriptionOrder order);




        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        void ReSubscriptionOrder(SubscriptionOrder order);
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Result</returns>
        bool IsReturnRequestAllowed(SubscriptionOrder order);



        /// <summary>
        /// Valdiate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        bool ValidateMinSubscriptionOrderSubtotalAmount(IList<SubscriptionCartItem> cart);

        /// <summary>
        /// Valdiate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        bool ValidateMinSubscriptionOrderTotalAmount(IList<SubscriptionCartItem> cart);
    }
}
