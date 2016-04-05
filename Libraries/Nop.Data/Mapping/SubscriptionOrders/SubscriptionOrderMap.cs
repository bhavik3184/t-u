using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Data.Mapping.SubscriptionOrders
{
    public partial class SubscriptionOrderMap : NopEntityTypeConfiguration<SubscriptionOrder>
    {
        public SubscriptionOrderMap()
        {
            this.ToTable("SubscriptionOrder");
            this.HasKey(o => o.Id);
            this.Property(o => o.CurrencyRate).HasPrecision(18, 8);
            this.Property(o => o.SubscriptionOrderSubtotalInclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderSubtotalExclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderSubTotalDiscountInclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderSubTotalDiscountExclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderShippingInclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderShippingExclTax).HasPrecision(18, 4);
            this.Property(o => o.PaymentMethodAdditionalFeeInclTax).HasPrecision(18, 4);
            this.Property(o => o.PaymentMethodAdditionalFeeExclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderDiscount).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionOrderTotal).HasPrecision(18, 4);
            this.Property(o => o.RefundedAmount).HasPrecision(18, 4);

            this.Ignore(o => o.SubscriptionOrderStatus);
            this.Ignore(o => o.PaymentStatus);
            this.Ignore(o => o.ShippingStatus);
            this.Ignore(o => o.CustomerTaxDisplayType);
            this.Ignore(o => o.TaxRatesDictionary);
            
            this.HasRequired(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId);

        
            
            //code below is commented because it causes some issues on big databases - http://www.nopcommerce.com/boards/t/11126/bug-version-20-command-confirm-takes-several-minutes-using-big-databases.aspx
            //this.HasRequired(o => o.BillingAddress).WithOptional().Map(x => x.MapKey("BillingAddressId")).WillCascadeOnDelete(false);
            //this.HasOptional(o => o.ShippingAddress).WithOptionalDependent().Map(x => x.MapKey("ShippingAddressId")).WillCascadeOnDelete(false);
            this.HasRequired(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .WillCascadeOnDelete(false);
            this.HasOptional(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .WillCascadeOnDelete(false);
        }
    }
}