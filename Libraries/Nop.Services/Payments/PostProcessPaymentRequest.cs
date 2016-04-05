using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public SubscriptionOrder SubscriptionOrder { get; set; }
    }
}
