using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class ChildrenMap : NopEntityTypeConfiguration<Children>
    {
        public ChildrenMap()
        {
            this.ToTable("Children");
            this.HasKey(pc => pc.Id);
            
            this.HasRequired(pc => pc.Customer)
                .WithMany()
                .HasForeignKey(pc => pc.CustomerId);
        }
    }
}