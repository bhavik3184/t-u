namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// Represents an order average report line summary
    /// </summary>
    public partial class SubscriptionOrderAverageReportLineSummary
    {
        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public SubscriptionOrderStatus SubscriptionOrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the sum today total
        /// </summary>
        public decimal SumTodaySubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the today count
        /// </summary>
        public int CountTodaySubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum this week total
        /// </summary>
        public decimal SumThisWeekSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the this week count
        /// </summary>
        public int CountThisWeekSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum this month total
        /// </summary>
        public decimal SumThisMonthSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the this month count
        /// </summary>
        public int CountThisMonthSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum this year total
        /// </summary>
        public decimal SumThisYearSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the this year count
        /// </summary>
        public int CountThisYearSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum all time total
        /// </summary>
        public decimal SumAllTimeSubscriptionOrders { get; set; }

        /// <summary>
        /// Gets or sets the all time count
        /// </summary>
        public int CountAllTimeSubscriptionOrders { get; set; }
    }
}
