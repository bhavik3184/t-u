using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class GiftCardMap : NopEntityTypeConfiguration<GiftCard>
    {
        public GiftCardMap()
        {
            this.ToTable("GiftCard");
            this.HasKey(gc => gc.Id);

            this.Property(gc => gc.Amount).HasPrecision(18, 4);

            this.Ignore(gc => gc.GiftCardType);

            this.HasOptional(gc => gc.PurchasedWithSubscriptionOrderItem)
                .WithMany(orderItem => orderItem.AssociatedGiftCards)
                .HasForeignKey(gc => gc.PurchasedWithSubscriptionOrderItemId);
        }
    }
}