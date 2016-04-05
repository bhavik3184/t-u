using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class CheckoutAttributeMap : NopEntityTypeConfiguration<CheckoutAttribute>
    {
        public CheckoutAttributeMap()
        {
            this.ToTable("CheckoutAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}