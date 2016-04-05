using System;

namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// Represents a best sellers report line
    /// </summary>
    [Serializable]
    public partial class BestsellersReportLine
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the total quantity
        /// </summary>
        public int TotalQuantity { get; set; }

    }
}
