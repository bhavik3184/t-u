namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder paid event
    /// </summary>
    public class SubscriptionOrderPaidEvent
    {
        public SubscriptionOrderPaidEvent(SubscriptionOrder order)
        {
            this.SubscriptionOrder = order;
        }

        /// <summary>
        /// SubscriptionOrder
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; private set; }
    }

    /// <summary>
    /// SubscriptionOrder placed event
    /// </summary>
    public class SubscriptionOrderPlacedEvent
    {
        public SubscriptionOrderPlacedEvent(SubscriptionOrder order)
        {
            this.SubscriptionOrder = order;
        }

        /// <summary>
        /// SubscriptionOrder
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; private set; }
    }

    /// <summary>
    /// SubscriptionOrder cancelled event
    /// </summary>
    public class SubscriptionOrderCancelledEvent
    {
        public SubscriptionOrderCancelledEvent(SubscriptionOrder order)
        {
            this.SubscriptionOrder = order;
        }

        /// <summary>
        /// SubscriptionOrder
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; private set; }
    }

    /// <summary>
    /// SubscriptionOrder refunded event
    /// </summary>
    public class SubscriptionOrderRefundedEvent
    {
        public SubscriptionOrderRefundedEvent(SubscriptionOrder order, decimal amount)
        {
            this.SubscriptionOrder = order;
            this.Amount = amount;
        }

        /// <summary>
        /// SubscriptionOrder
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }
    }

}