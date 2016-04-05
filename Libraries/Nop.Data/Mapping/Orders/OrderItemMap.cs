using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.Orders
{
    public partial class OrderItemMap : NopEntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            this.ToTable("OrderItem");
            this.HasKey(orderItem => orderItem.Id);

         

            this.Ignore(o => o.ShippingStatus);

            this.HasRequired(orderItem => orderItem.SubscriptionOrder)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(orderItem => orderItem.SubscriptionOrderId);

            
        }
    }
}