using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PlanPictureMap : NopEntityTypeConfiguration<PlanPicture>
    {
        public PlanPictureMap()
        {
            this.ToTable("Plan_Picture_Mapping");
            this.HasKey(pp => pp.Id);
            
            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.Plan)
                .WithMany(p => p.PlanPictures)
                .HasForeignKey(pp => pp.PlanId);
        }
    }
}