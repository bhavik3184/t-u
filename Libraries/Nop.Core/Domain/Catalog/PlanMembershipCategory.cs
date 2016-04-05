namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public partial class PlanMembershipCategory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int MembershipCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is featured
        /// </summary>
        public bool IsFeaturedPlan { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual MembershipCategory MembershipCategory { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Plan Plan { get; set; }

    }

}
