using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class SubscriptionOrderNoteMap : NopEntityTypeConfiguration<SubscriptionOrderNote>
    {
        public SubscriptionOrderNoteMap()
        {
            this.ToTable("SubscriptionOrderNote");
            this.HasKey(on => on.Id);
            this.Property(on => on.Note).IsRequired();

            this.HasRequired(on => on.SubscriptionOrder)
                .WithMany(o => o.SubscriptionOrderNotes)
                .HasForeignKey(on => on.SubscriptionOrderId);
        }
    }
}