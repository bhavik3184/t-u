using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.Orders
{
    public partial class BorrowCartItemMap : NopEntityTypeConfiguration<BorrowCartItem>
    {
        public BorrowCartItemMap()
        {
            this.ToTable("BorrowCartItem");
            this.HasKey(sci => sci.Id);

            this.Property(sci => sci.CustomerEnteredPrice).HasPrecision(18, 4);

            this.Ignore(sci => sci.BorrowCartType);
            this.Ignore(sci => sci.IsFreeShipping);
            this.Ignore(sci => sci.IsShipEnabled);
            this.Ignore(sci => sci.AdditionalShippingCharge);
            this.Ignore(sci => sci.IsTaxExempt);

            this.HasRequired(sci => sci.Customer)
                .WithMany(c => c.BorrowCartItems)
                .HasForeignKey(sci => sci.CustomerId);

            this.HasRequired(sci => sci.Product)
                .WithMany()
                .HasForeignKey(sci => sci.ProductId);
        }
    }
}
