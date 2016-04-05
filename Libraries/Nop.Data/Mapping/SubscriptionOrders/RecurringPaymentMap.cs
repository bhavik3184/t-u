using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class RecurringPaymentMap : NopEntityTypeConfiguration<RecurringPayment>
    {
        public RecurringPaymentMap()
        {
            this.ToTable("RecurringPayment");
            this.HasKey(rp => rp.Id);

            this.Ignore(rp => rp.NextPaymentDate);
            this.Ignore(rp => rp.CyclesRemaining);
            this.Ignore(rp => rp.CyclePeriod);



            //this.HasRequired(rp => rp.InitialSubscriptionOrder).WithOptional().Map(x => x.MapKey("InitialSubscriptionOrderId")).WillCascadeOnDelete(false);
            this.HasRequired(rp => rp.InitialSubscriptionOrder)
                .WithMany()
                .HasForeignKey(o => o.InitialSubscriptionOrderId)
                .WillCascadeOnDelete(false);
        }
    }
}