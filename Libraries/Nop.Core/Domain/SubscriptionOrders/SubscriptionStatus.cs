namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// Represents an order status enumeration
    /// </summary>
    public enum SubscriptionOrderStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,
        /// <summary>
        /// Processing
        /// </summary>
        Processing = 20,
        /// <summary>
        /// Complete
        /// </summary>
        Complete = 30,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40,

        Closed = 50
    }
}
