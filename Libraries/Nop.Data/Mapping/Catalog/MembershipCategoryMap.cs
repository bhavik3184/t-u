using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class MembershipCategoryMap : NopEntityTypeConfiguration<MembershipCategory>
    {
        public MembershipCategoryMap()
        {
            this.ToTable("MembershipCategory");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.Property(c => c.MetaKeywords).HasMaxLength(400);
            this.Property(c => c.MetaTitle).HasMaxLength(400);
            this.Property(c => c.PriceRanges).HasMaxLength(400);
            this.Property(c => c.PageSizeOptions).HasMaxLength(200);
        }
    }
}