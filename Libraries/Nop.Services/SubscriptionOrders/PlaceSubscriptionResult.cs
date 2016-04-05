using System.Collections.Generic;
using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// Place order result
    /// </summary>
    public partial class PlaceSubscriptionOrderResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PlaceSubscriptionOrderResult() 
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }
        
        /// <summary>
        /// Gets or sets the placed order
        /// </summary>
        public SubscriptionOrder PlacedSubscriptionOrder { get; set; }
    }
}
