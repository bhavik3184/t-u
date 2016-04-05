using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Discounts;
using Nop.Admin.Models.Stores;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(PlanValidator))]
    public partial class PlanModel : BaseNopEntityModel, ILocalizedModel<PlanLocalizedModel>
    {
        public PlanModel()
        {
            Locales = new List<PlanLocalizedModel>();
            PlanPictureModels = new List<PlanPictureModel>();
            CopyPlanModel = new CopyPlanModel();
            AvailableBasepriceUnits = new List<SelectListItem>();
            AvailableBasepriceBaseUnits = new List<SelectListItem>();
            AvailablePlanTemplates = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableTaxCategories = new List<SelectListItem>();
            AvailableDeliveryDates = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailableBaseCategories = new List<SelectListItem>();
            AvailableMembershipCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailablePlanAttributes = new List<SelectListItem>();
            AddPictureModel = new PlanPictureModel();
            AddSpecificationAttributeModel = new AddPlanSpecificationAttributeModel();
            PlanWarehouseInventoryModels = new List<PlanWarehouseInventoryModel>();
        }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ID")]
        public override int Id { get; set; }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PlanType")]
        public int PlanTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PlanType")]
        public string PlanTypeName { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AssociatedToPlanName")]
        public int AssociatedToPlanId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AssociatedToPlanName")]
        public string AssociatedToPlanName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PlanTemplate")]
        public int PlanTemplateId { get; set; }
        public IList<SelectListItem> AvailablePlanTemplates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Vendor")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PlanTags")]
        public string PlanTags { get; set; }

        public decimal RegistrationCharge { get; set; }

        public decimal SecurityDeposit { get; set; }

        public int MaxNoOfDeliveries { get; set; }
        public int NoOfItemsToBorrow { get; set; }
         

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Sku")]
        [AllowHtml]
        public string Sku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ManufacturerPartNumber")]
        [AllowHtml]
        public string ManufacturerPartNumber { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.GTIN")]
        [AllowHtml]
        public virtual string Gtin { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsGiftCard")]
        public bool IsGiftCard { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.GiftCardType")]
        public int GiftCardTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.OverriddenGiftCardAmount")]
        [UIHint("DecimalNullable")]
        public decimal? OverriddenGiftCardAmount { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RequireOtherPlans")]
        public bool RequireOtherPlans { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RequiredPlanIds")]
        public string RequiredPlanIds { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AutomaticallyAddRequiredPlans")]
        public bool AutomaticallyAddRequiredPlans { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsDownload")]
        public bool IsDownload { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Download")]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DownloadExpirationDays")]
        [UIHint("Int32Nullable")]
        public int? DownloadExpirationDays { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DownloadActivationType")]
        public int DownloadActivationTypeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.HasSampleDownload")]
        public bool HasSampleDownload { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SampleDownload")]
        [UIHint("Download")]
        public int SampleDownloadId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.HasUserAgreement")]
        public bool HasUserAgreement { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.UserAgreementText")]
        [AllowHtml]
        public string UserAgreementText { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsRecurring")]
        public bool IsRecurring { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RecurringCycleLength")]
        public int RecurringCycleLength { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RecurringCyclePeriod")]
        public int RecurringCyclePeriodId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RecurringTotalCycles")]
        public int RecurringTotalCycles { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsRental")]
        public bool IsRental { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RentalPriceLength")]
        public int RentalPriceLength { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.RentalPricePeriod")]
        public int RentalPricePeriodId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsShipEnabled")]
        public bool IsShipEnabled { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsFreeShipping")]
        public bool IsFreeShipping { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ShipSeparately")]
        public bool ShipSeparately { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AdditionalShippingCharge")]
        public decimal AdditionalShippingCharge { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DeliveryDate")]
        public int DeliveryDateId { get; set; }
        public IList<SelectListItem> AvailableDeliveryDates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices")]
        public bool IsTelecommunicationsOrBroadcastingOrElectronicServices { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ManageInventoryMethod")]
        public int ManageInventoryMethodId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.UseMultipleWarehouses")]
        public bool UseMultipleWarehouses { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.StockQuantity")]
        public int StockQuantity { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.StockQuantity")]
        public string StockQuantityStr { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DisplayStockAvailability")]
        public bool DisplayStockAvailability { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DisplayStockQuantity")]
        public bool DisplayStockQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MinStockQuantity")]
        public int MinStockQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.LowStockActivity")]
        public int LowStockActivityId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.NotifyAdminForQuantityBelow")]
        public int NotifyAdminForQuantityBelow { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.BackorderMode")]
        public int BackorderModeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AllowBackInStockSubscriptions")]
        public bool AllowBackInStockSubscriptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SubscriptionMinimumQuantity")]
        public int SubscriptionMinimumQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SubscriptionOrderMaximumQuantity")]
        public int SubscriptionOrderMaximumQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AllowedQuantities")]
        public string AllowedQuantities { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AllowAddingOnlyExistingAttributeCombinations")]
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DisableBuyButton")]
        public bool DisableBuyButton { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DisableMyToyBoxButton")]
        public bool DisableMyToyBoxButton { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AvailableForPreSubscription")]
        public bool AvailableForPreSubscription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PreSubscriptionAvailabilityStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.CallForPrice")]
        public bool CallForPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.OldPrice")]
        public decimal OldPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.PlanCost")]
        public decimal PlanCost { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SpecialPrice")]
        [UIHint("DecimalNullable")]
        public decimal? SpecialPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SpecialPriceStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? SpecialPriceStartDateTimeUtc { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SpecialPriceEndDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? SpecialPriceEndDateTimeUtc { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.CustomerEntersPrice")]
        public bool CustomerEntersPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MinimumCustomerEnteredPrice")]
        public decimal MinimumCustomerEnteredPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MaximumCustomerEnteredPrice")]
        public decimal MaximumCustomerEnteredPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.BasepriceEnabled")]
        public bool BasepriceEnabled { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.BasepriceAmount")]
        public decimal BasepriceAmount { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.BasepriceUnit")]
        public int BasepriceUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceUnits { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.BasepriceBaseAmount")]
        public decimal BasepriceBaseAmount { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.BasepriceBaseUnit")]
        public int BasepriceBaseUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceBaseUnits { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MarkAsNew")]
        public bool MarkAsNew { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MarkAsNewStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MarkAsNewEndDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Weight")]
        public decimal Weight { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Length")]
        public decimal Length { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Width")]
        public decimal Width { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Height")]
        public decimal Height { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AvailableStartDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AvailableEndDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.CreatedOn")]
        public DateTime? CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }


        public string PrimaryStoreCurrencyCode { get; set; }
        public string BaseDimensionIn { get; set; }
        public string BaseWeightIn { get; set; }

        public IList<PlanLocalizedModel> Locales { get; set; }


        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public int[] SelectedCustomerRoleIds { get; set; }

        //Store mapping
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }


        //vendor
        public bool IsLoggedInAsVendor { get; set; }



        //categories
        public IList<SelectListItem> AvailableMembershipCategories { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableBaseCategories { get; set; }
        //manufacturers
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        //product attributes
        public IList<SelectListItem> AvailablePlanAttributes { get; set; }
        


        //pictures
        public PlanPictureModel AddPictureModel { get; set; }
        public IList<PlanPictureModel> PlanPictureModels { get; set; }

        //discounts
        public List<DiscountModel> AvailableDiscounts { get; set; }
        public int[] SelectedDiscountIds { get; set; }




        //add specification attribute model
        public AddPlanSpecificationAttributeModel AddSpecificationAttributeModel { get; set; }


        //multiple warehouses
        [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory")]
        public IList<PlanWarehouseInventoryModel> PlanWarehouseInventoryModels { get; set; }

        //copy product
        public CopyPlanModel CopyPlanModel { get; set; }
        
        #region Nested classes

        public partial class AddRequiredPlanModel : BaseNopModel
        {
            public AddRequiredPlanModel()
            {
                AvailableMembershipCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailablePlanTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanName")]
            [AllowHtml]
            public string SearchPlanName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanType")]
            public int SearchPlanTypeId { get; set; }

            public IList<SelectListItem> AvailableMembershipCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailablePlanTypes { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class AddPlanSpecificationAttributeModel : BaseNopModel
        {
            public AddPlanSpecificationAttributeModel()
            {
                AvailableAttributes = new List<SelectListItem>();
                AvailableOptions = new List<SelectListItem>();
            }
            
            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.SpecificationAttribute")]
            public int SpecificationAttributeId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.AttributeType")]
            public int AttributeTypeId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.SpecificationAttributeOption")]
            public int SpecificationAttributeOptionId { get; set; }

            [AllowHtml]
            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.CustomValue")]
            public string CustomValue { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.AllowFiltering")]
            public bool AllowFiltering { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.ShowOnPlanPage")]
            public bool ShowOnPlanPage { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.SpecificationAttributes.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public IList<SelectListItem> AvailableAttributes { get; set; }
            public IList<SelectListItem> AvailableOptions { get; set; }
        }
        
        public partial class PlanPictureModel : BaseNopEntityModel
        {
            public int PlanId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Plans.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Pictures.Fields.OverrideAltAttribute")]
            [AllowHtml]
            public string OverrideAltAttribute { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Pictures.Fields.OverrideTitleAttribute")]
            [AllowHtml]
            public string OverrideTitleAttribute { get; set; }
        }
        
        public partial class PlanMembershipCategoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Plans.Categories.Fields.Category")]
            public string MembershipCategory { get; set; }

            public int PlanId { get; set; }

            public int MembershipCategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Categories.Fields.IsFeaturedPlan")]
            public bool IsFeaturedPlan { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class PlanCategoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Plans.Categories.Fields.Category")]
            public string Category { get; set; }

            public int PlanId { get; set; }

            public int Quantity { get; set; }
            public int MyToyBoxQuantity { get; set; }
            
            public int CategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Categories.Fields.IsFeaturedPlan")]
            public bool IsFeaturedPlan { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class PlanManufacturerModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Plans.Manufacturers.Fields.Manufacturer")]
            public string Manufacturer { get; set; }

            public int PlanId { get; set; }

            public int ManufacturerId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Manufacturers.Fields.IsFeaturedPlan")]
            public bool IsFeaturedPlan { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.Manufacturers.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class RelatedPlanModel : BaseNopEntityModel
        {
            public int PlanId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.RelatedPlans.Fields.Plan")]
            public string Plan2Name { get; set; }
            
            [NopResourceDisplayName("Admin.Catalog.Plans.RelatedPlans.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddRelatedPlanModel : BaseNopModel
        {
            public AddRelatedPlanModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailablePlanTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanName")]
            [AllowHtml]
            public string SearchPlanName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanType")]
            public int SearchPlanTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailablePlanTypes { get; set; }

            public int PlanId { get; set; }

            public int[] SelectedPlanIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class AssociatedPlanModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Plans.AssociatedPlans.Fields.Plan")]
            public string PlanName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.AssociatedPlans.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddAssociatedPlanModel : BaseNopModel
        {
            public AddAssociatedPlanModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailablePlanTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanName")]
            [AllowHtml]
            public string SearchPlanName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanType")]
            public int SearchPlanTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailablePlanTypes { get; set; }

            public int PlanId { get; set; }

            public int[] SelectedPlanIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class CrossSellPlanModel : BaseNopEntityModel
        {
            public int PlanId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.CrossSells.Fields.Plan")]
            public string Plan2Name { get; set; }
        }
        public partial class AddCrossSellPlanModel : BaseNopModel
        {
            public AddCrossSellPlanModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailablePlanTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanName")]
            [AllowHtml]
            public string SearchPlanName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanType")]
            public int SearchPlanTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailablePlanTypes { get; set; }

            public int PlanId { get; set; }

            public int[] SelectedPlanIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class TierPriceModel : BaseNopEntityModel
        {
            public int PlanId { get; set; }

            public int CustomerRoleId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.TierPrices.Fields.CustomerRole")]
            public string CustomerRole { get; set; }


            public int StoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.TierPrices.Fields.Store")]
            public string Store { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.TierPrices.Fields.Quantity")]
            public int Quantity { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.TierPrices.Fields.Price")]
            public decimal Price { get; set; }
        }

        public partial class PlanWarehouseInventoryModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory.Fields.Warehouse")]
            public int WarehouseId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory.Fields.Warehouse")]
            public string WarehouseName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory.Fields.WarehouseUsed")]
            public bool WarehouseUsed { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory.Fields.StockQuantity")]
            public int StockQuantity { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory.Fields.ReservedQuantity")]
            public int ReservedQuantity { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanWarehouseInventory.Fields.PlannedQuantity")]
            public int PlannedQuantity { get; set; }
        }


        public partial class PlanAttributeMappingModel : BaseNopEntityModel
        {
            public int PlanId { get; set; }

            public int PlanAttributeId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.Fields.Attribute")]
            public string PlanAttribute { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.Fields.TextPrompt")]
            [AllowHtml]
            public string TextPrompt { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.Fields.IsRequired")]
            public bool IsRequired { get; set; }

            public int AttributeControlTypeId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.Fields.AttributeControlType")]
            public string AttributeControlType { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public bool ShouldHaveValues { get; set; }
            public int TotalValues { get; set; }

            //validation fields
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.ValidationRules")]
            public bool ValidationRulesAllowed { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.ValidationRules.MinLength")]
            [UIHint("Int32Nullable")]
            public int? ValidationMinLength { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.ValidationRules.MaxLength")]
            [UIHint("Int32Nullable")]
            public int? ValidationMaxLength { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.ValidationRules.FileAllowedExtensions")]
            public string ValidationFileAllowedExtensions { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.ValidationRules.FileMaximumSize")]
            [UIHint("Int32Nullable")]
            public int? ValidationFileMaximumSize { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.ValidationRules.DefaultValue")]
            public string DefaultValue { get; set; }

            //condition
            [NopResourceDisplayName("Admin.Catalog.Plans.PlanAttributes.Attributes.Condition")]
            public bool ConditionAllowed { get; set; }
        }
        public partial class PlanAttributeValueListModel : BaseNopModel
        {
            public int PlanId { get; set; }

            public string PlanName { get; set; }

            public int PlanAttributeMappingId { get; set; }

            public string PlanAttributeName { get; set; }
        }
      
       

        #endregion
    }

    public partial class PlanLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}