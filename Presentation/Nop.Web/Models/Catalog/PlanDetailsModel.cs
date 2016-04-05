using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class PlanDetailsModel : BaseNopEntityModel
    {
        public PlanDetailsModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            GiftCard = new GiftCardModel();
            PlanPrice = new PlanPriceModel();
            AddToBag = new AddToBagModel();
            PlanAttributes = new List<PlanAttributeModel>();
            AssociatedPlans = new List<PlanDetailsModel>();
            VendorModel = new VendorBriefInfoModel();
            Breadcrumb = new PlanBreadcrumbModel();
            PlanManufacturers = new List<ManufacturerModel>();
            TierPrices = new List<TierPriceModel>();
        }

        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string PlanTemplateViewPath { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public bool ShowSku { get; set; }
        public string Sku { get; set; }

        public bool ShowManufacturerPartNumber { get; set; }
        public string ManufacturerPartNumber { get; set; }

        public bool ShowGtin { get; set; }
        public string Gtin { get; set; }

        public bool ShowVendor { get; set; }
        public VendorBriefInfoModel VendorModel { get; set; }

        public bool HasSampleDownload { get; set; }

        public GiftCardModel GiftCard { get; set; }

        public bool IsShipEnabled { get; set; }
        public bool IsFreeShipping { get; set; }
        public bool FreeShippingNotificationEnabled { get; set; }
        public string DeliveryDate { get; set; }


        public bool IsRental { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }

        public string StockAvailability { get; set; }

        public bool DisplayBackInStockSubscriptionOrder { get; set; }

        public bool EmailAFriendEnabled { get; set; }
        public bool ComparePlansEnabled { get; set; }

        public string PageShareCode { get; set; }

        public PlanPriceModel PlanPrice { get; set; }

        public AddToBagModel AddToBag { get; set; }

        public PlanBreadcrumbModel Breadcrumb { get; set; }

        public IList<PlanAttributeModel> PlanAttributes { get; set; }

        public IList<ManufacturerModel> PlanManufacturers { get; set; }

        public IList<TierPriceModel> TierPrices { get; set; }

        //a list of associated products. For example, "Grouped" products could have several child "simple" products
        public IList<PlanDetailsModel> AssociatedPlans { get; set; }

        public bool DisplayDiscontinuedMessage { get; set; }

        #region Nested Classes

        public partial class PlanBreadcrumbModel : BaseNopModel
        {
            public PlanBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategorySimpleModel>();
            }

            public bool Enabled { get; set; }
            public int PlanId { get; set; }
            public string PlanName { get; set; }
            public string PlanSeName { get; set; }
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        public partial class AddToBagModel : BaseNopModel
        {
            public AddToBagModel()
            {
                this.AllowedQuantities = new List<SelectListItem>();
            }
            public int PlanId { get; set; }

            //qty
            [NopResourceDisplayName("Plans.Qty")]
            public int EnteredQuantity { get; set; }
            public string MinimumQuantityNotification { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            //price entered by customers
            [NopResourceDisplayName("Plans.EnterPlanPrice")]
            public bool CustomerEntersPrice { get; set; }
            [NopResourceDisplayName("Plans.EnterPlanPrice")]
            public decimal CustomerEnteredPrice { get; set; }
            public String CustomerEnteredPriceRange { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableMyToyBoxButton { get; set; }

            //rental
            public bool IsRental { get; set; }

            //pre-order
            public bool AvailableForPreSubscription { get; set; }
            public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }

            //updating existing shopping cart item?
            public int UpdatedSelectionBagItemId { get; set; }
        }

        public partial class PlanPriceModel : BaseNopModel
        {
            /// <summary>
            /// The currency (in 3-letter ISO 4217 format) of the offer price 
            /// </summary>
            public string CurrencyCode { get; set; }

            public string OldPrice { get; set; }

            public string Price { get; set; }
            public string PriceWithDiscount { get; set; }
            public decimal PriceValue { get; set; }

            public bool CustomerEntersPrice { get; set; }

            public bool CallForPrice { get; set; }

            public int PlanId { get; set; }

            public bool HidePrices { get; set; }

            //rental
            public bool IsRental { get; set; }
            public string RentalPrice { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }
        }

        public partial class GiftCardModel : BaseNopModel
        {
            public bool IsGiftCard { get; set; }

            [NopResourceDisplayName("Plans.GiftCard.RecipientName")]
            [AllowHtml]
            public string RecipientName { get; set; }
            [NopResourceDisplayName("Plans.GiftCard.RecipientEmail")]
            [AllowHtml]
            public string RecipientEmail { get; set; }
            [NopResourceDisplayName("Plans.GiftCard.SenderName")]
            [AllowHtml]
            public string SenderName { get; set; }
            [NopResourceDisplayName("Plans.GiftCard.SenderEmail")]
            [AllowHtml]
            public string SenderEmail { get; set; }
            [NopResourceDisplayName("Plans.GiftCard.Message")]
            [AllowHtml]
            public string Message { get; set; }

            public GiftCardType GiftCardType { get; set; }
        }

        public partial class TierPriceModel : BaseNopModel
        {
            public string Price { get; set; }

            public int Quantity { get; set; }
        }

        public partial class PlanAttributeModel : BaseNopEntityModel
        {
            public PlanAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<PlanAttributeValueModel>();
            }

            public int PlanId { get; set; }

            public int PlanAttributeId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }
            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// A value indicating whether this attribute depends on some other attribute
            /// </summary>
            public bool HasCondition { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<PlanAttributeValueModel> Values { get; set; }

        }

        public partial class PlanAttributeValueModel : BaseNopEntityModel
        {
            public PlanAttributeValueModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            public string PriceAdjustment { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            //picture model is used when we want to override a default product picture when some attribute is selected
            public PictureModel PictureModel { get; set; }
        }

		#endregion
    }
}