using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a CapturePaymentRequest
    /// </summary>
    public partial class CapturePaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; set; }
    }
}
