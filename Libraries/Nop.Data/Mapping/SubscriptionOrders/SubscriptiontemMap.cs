using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class SubscriptionOrderItemMap : NopEntityTypeConfiguration<SubscriptionOrderItem>
    {
        public SubscriptionOrderItemMap()
        {
            this.ToTable("SubscriptionOrderItem");
            this.HasKey(orderItem => orderItem.Id);

            this.Property(orderItem => orderItem.UnitPriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.UnitPriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.OriginalPlanCost).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.ItemWeight).HasPrecision(18, 4);


            this.HasRequired(orderItem => orderItem.SubscriptionOrder)
                .WithMany(o => o.SubscriptionOrderItems)
                .HasForeignKey(orderItem => orderItem.SubscriptionOrderId);

            this.HasRequired(orderItem => orderItem.Plan)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.PlanId);
        }
    }
}