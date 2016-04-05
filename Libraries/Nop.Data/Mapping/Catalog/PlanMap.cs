using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PlanMap : NopEntityTypeConfiguration<Plan>
    {
        public PlanMap()
        {
            this.ToTable("Plan");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.MetaKeywords).HasMaxLength(400);
            this.Property(p => p.MetaTitle).HasMaxLength(400);
            this.Property(p => p.Sku).HasMaxLength(400);
            this.Property(p => p.ManufacturerPartNumber).HasMaxLength(400);
            this.Property(p => p.Gtin).HasMaxLength(400);
            this.Property(p => p.AdditionalShippingCharge).HasPrecision(18, 4);
            this.Property(p => p.Price).HasPrecision(18, 4);
            this.Property(p => p.OldPrice).HasPrecision(18, 4);
            this.Property(p => p.PlanCost).HasPrecision(18, 4);
            this.Property(p => p.SpecialPrice).HasPrecision(18, 4);
            this.Property(p => p.MinimumCustomerEnteredPrice).HasPrecision(18, 4);
            this.Property(p => p.MaximumCustomerEnteredPrice).HasPrecision(18, 4);
            this.Property(p => p.Weight).HasPrecision(18, 4);
            this.Property(p => p.Length).HasPrecision(18, 4);
            this.Property(p => p.Width).HasPrecision(18, 4);
            this.Property(p => p.Height).HasPrecision(18, 4);
            this.Property(p => p.AllowedQuantities).HasMaxLength(1000);
            this.Property(p => p.BasepriceAmount).HasPrecision(18, 4);
            this.Property(p => p.BasepriceBaseAmount).HasPrecision(18, 4);

            this.Ignore(p => p.PlanType);
            this.Ignore(p => p.BackorderMode);
            this.Ignore(p => p.DownloadActivationType);
            this.Ignore(p => p.GiftCardType);
            this.Ignore(p => p.LowStockActivity);
            this.Ignore(p => p.ManageInventoryMethod);
            this.Ignore(p => p.RecurringCyclePeriod);
            this.Ignore(p => p.RentalPricePeriod);

            
        }
    }
}