
using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class GiftCardUsageHistoryMap : NopEntityTypeConfiguration<GiftCardUsageHistory>
    {
        public GiftCardUsageHistoryMap()
        {
            this.ToTable("GiftCardUsageHistory");
            this.HasKey(gcuh => gcuh.Id);
            this.Property(gcuh => gcuh.UsedValue).HasPrecision(18, 4);
            //this.Property(gcuh => gcuh.UsedValueInCustomerCurrency).HasPrecision(18, 4);

            this.HasRequired(gcuh => gcuh.GiftCard)
                .WithMany(gc => gc.GiftCardUsageHistory)
                .HasForeignKey(gcuh => gcuh.GiftCardId);

            this.HasRequired(gcuh => gcuh.UsedWithSubscriptionOrder)
                .WithMany(o => o.GiftCardUsageHistory)
                .HasForeignKey(gcuh => gcuh.UsedWithSubscriptionOrderId);
        }
    }
}