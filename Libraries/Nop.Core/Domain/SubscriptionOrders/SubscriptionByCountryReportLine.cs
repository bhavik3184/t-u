namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// Represents an "order by country" report line
    /// </summary>
    public partial class SubscriptionOrderByCountryReportLine
    {
        /// <summary>
        /// Country identifier; null for unknow country
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the number of orders
        /// </summary>
        public int TotalSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the order total summary
        /// </summary>
        public decimal SumSubscriptionOrders { get; set; }
    }
}
