using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    public partial class LocalityMap : NopEntityTypeConfiguration<Locality>
    {
        public LocalityMap()
        {
            this.ToTable("Locality");
            this.HasKey(sp => sp.Id);
            this.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            this.Property(sp => sp.Abbreviation).HasMaxLength(100);


            this.HasRequired(sp => sp.City)
                .WithMany(c => c.Localities)
                .HasForeignKey(sp => sp.CityId);
        }
    }
}