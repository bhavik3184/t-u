using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class SubscriptionCartItemMap : NopEntityTypeConfiguration<SubscriptionCartItem>
    {
        public SubscriptionCartItemMap()
        {
            this.ToTable("SubscriptionCartItem");
            this.HasKey(sci => sci.Id);

            this.Property(sci => sci.CustomerEnteredPrice).HasPrecision(18, 4);

            this.Ignore(sci => sci.SubscriptionCartType);
            this.Ignore(sci => sci.IsFreeShipping);
            this.Ignore(sci => sci.IsShipEnabled);
            this.Ignore(sci => sci.AdditionalShippingCharge);
            this.Ignore(sci => sci.IsTaxExempt);

            this.HasRequired(sci => sci.Customer)
                .WithMany(c => c.SubscriptionCartItems)
                .HasForeignKey(sci => sci.CustomerId);

            this.HasRequired(sci => sci.Plan)
                .WithMany()
                .HasForeignKey(sci => sci.PlanId);
        }
    }
}
