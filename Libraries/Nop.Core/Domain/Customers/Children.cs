using System;
namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public partial class Children : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }
        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual Customer Customer { get; set; }


    }
}
