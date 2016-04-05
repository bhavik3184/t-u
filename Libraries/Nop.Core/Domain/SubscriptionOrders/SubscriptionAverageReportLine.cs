namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// Represents an order average report line
    /// </summary>
    public partial class SubscriptionOrderAverageReportLine
    {
        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int CountSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the shipping summary (excluding tax)
        /// </summary>
        public decimal SumShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the tax summary
        /// </summary>
        public decimal SumTax { get; set; }

        /// <summary>
        /// Gets or sets the order total summary
        /// </summary>
        public decimal SumSubscriptionOrders { get; set; }
    }
}
