using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.Orders
{
    public partial class ItemDetailMap : NopEntityTypeConfiguration<ItemDetail>
    {
        public ItemDetailMap()
        {
            this.ToTable("ItemDetail");
            this.HasKey(orderItem => orderItem.Id);
            
            this.Property(orderItem => orderItem.UnitPriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.UnitPriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.OriginalProductCost).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.ItemWeight).HasPrecision(18, 4);
 
            this.HasRequired(orderItem => orderItem.Product)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.ProductId);

            this.HasRequired(orderItem => orderItem.OrderItem)
               .WithMany(o => o.ItemDetails)
               .HasForeignKey(orderItem => orderItem.OrderItemId);
        }
    }
}