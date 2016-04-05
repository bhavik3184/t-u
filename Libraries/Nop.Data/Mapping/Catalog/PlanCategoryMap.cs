using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PlanCategoryMap : NopEntityTypeConfiguration<PlanCategory>
    {
        public PlanCategoryMap()
        {
            this.ToTable("Plan_Category_Mapping");
            this.HasKey(pc => pc.Id);
            
            this.HasRequired(pc => pc.Category)
                .WithMany()
                .HasForeignKey(pc => pc.CategoryId);


            this.HasRequired(pc => pc.Plan)
                .WithMany(p => p.PlanCategories)
                .HasForeignKey(pc => pc.PlanId);
        }
    }
}