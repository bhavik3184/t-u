
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PlanMembershipCategoryMap : NopEntityTypeConfiguration<PlanMembershipCategory>
    {
        public PlanMembershipCategoryMap()
        {
            this.ToTable("Plan_MembershipCategory_Mapping");
            this.HasKey(pc => pc.Id);
            
            this.HasRequired(pc => pc.MembershipCategory)
                .WithMany()
                .HasForeignKey(pc => pc.MembershipCategoryId);


            this.HasRequired(pc => pc.Plan)
                .WithMany(p => p.PlanMembershipCategories)
                .HasForeignKey(pc => pc.PlanId);
        }
    }
}