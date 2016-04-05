using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a VoidPaymentResult
    /// </summary>
    public partial class VoidPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; set; }
    }
}
