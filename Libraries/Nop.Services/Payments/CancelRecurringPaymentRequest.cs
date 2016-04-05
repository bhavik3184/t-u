using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a CancelRecurringPaymentResult
    /// </summary>
    public partial class CancelRecurringPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; set; }
    }
}
