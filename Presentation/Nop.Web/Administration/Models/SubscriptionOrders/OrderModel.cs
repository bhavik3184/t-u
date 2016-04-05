using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class SubscriptionOrderModel : BaseNopEntityModel
    {
        public SubscriptionOrderModel()
        {
            CustomValues = new Dictionary<string, object>();
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<SubscriptionOrderItemModel>();
            OrderItems = new List<OrderItemModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
        }

        public bool IsLoggedInAsVendor { get; set; }

        //identifiers
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.ID")]
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderGuid")]
        public Guid SubscriptionOrderGuid { get; set; }

        //store
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Store")]
        public string StoreName { get; set; }

        //customer info
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Customer")]
        public string CustomerInfo { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }
        public string CustomerFullName { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CustomerIP")]
        public string CustomerIp { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CustomValues")]
        public Dictionary<string, object> CustomValues { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Affiliate")]
        public string AffiliateName { get; set; }

        //Used discounts
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.UsedDiscounts")]
        public IList<UsedDiscountModel> UsedDiscounts { get; set; }

        //totals
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        public TaxDisplayType TaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderSubtotalInclTax")]
        public string SubscriptionOrderSubtotalInclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderSubtotalExclTax")]
        public string SubscriptionOrderSubtotalExclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderSubTotalDiscountInclTax")]
        public string SubscriptionOrderSubTotalDiscountInclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderSubTotalDiscountExclTax")]
        public string SubscriptionOrderSubTotalDiscountExclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderShippingInclTax")]
        public string SubscriptionOrderShippingInclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderShippingExclTax")]
        public string SubscriptionOrderShippingExclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.PaymentMethodAdditionalFeeInclTax")]
        public string PaymentMethodAdditionalFeeInclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.PaymentMethodAdditionalFeeExclTax")]
        public string PaymentMethodAdditionalFeeExclTax { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Tax")]
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderTotalDiscount")]
        public string SubscriptionOrderTotalDiscount { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.RedeemedRewardPoints")]
        public int RedeemedRewardPoints { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.RedeemedRewardPoints")]
        public string RedeemedRewardPointsAmount { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderTotal")]
        public string SubscriptionOrderTotal { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.RefundedAmount")]
        public string RefundedAmount { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Profit")]
        public string Profit { get; set; }


        public string RegistrationCharge { get; set; }
        public string RegistrationChargeDiscount { get; set; }

        public string SecurityDeposit { get; set; }

        //edit totals
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderSubtotal")]
        public decimal SubscriptionOrderSubtotalInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderSubtotal")]
        public decimal SubscriptionOrderSubtotalExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderSubTotalDiscount")]
        public decimal SubscriptionOrderSubTotalDiscountInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderSubTotalDiscount")]
        public decimal SubscriptionOrderSubTotalDiscountExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderShipping")]
        public decimal SubscriptionOrderShippingInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderShipping")]
        public decimal SubscriptionOrderShippingExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.Tax")]
        public decimal TaxValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.TaxRates")]
        public string TaxRatesValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderTotalDiscount")]
        public decimal SubscriptionOrderTotalDiscountValue { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.Edit.SubscriptionOrderTotal")]
        public decimal SubscriptionOrderTotalValue { get; set; }

        //associated recurring payment id
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.RecurringPayment")]
        public int RecurringPaymentId { get; set; }

        //order status
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderStatus")]
        public string SubscriptionOrderStatus { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderStatus")]
        public int SubscriptionOrderStatusId { get; set; }

        //payment info
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        //credit card info
        public bool AllowStoringCreditCardNumber { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CardType")]
        [AllowHtml]
        public string CardType { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CardName")]
        [AllowHtml]
        public string CardName { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CardNumber")]
        [AllowHtml]
        public string CardNumber { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CardCVV2")]
        [AllowHtml]
        public string CardCvv2 { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CardExpirationMonth")]
        [AllowHtml]
        public string CardExpirationMonth { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CardExpirationYear")]
        [AllowHtml]
        public string CardExpirationYear { get; set; }

        //misc payment info
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.AuthorizationTransactionID")]
        public string AuthorizationTransactionId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CaptureTransactionID")]
        public string CaptureTransactionId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.SubscriptionOrderTransactionID")]
        public string SubscriptionTransactionId { get; set; }

        //shipping info
        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.ShippingAddress")]
        public AddressModel ShippingAddress { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }
        public string ShippingAddressGoogleMapsUrl { get; set; }
        public bool CanAddNewShipments { get; set; }

        //billing info
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.BillingAddress")]
        public AddressModel BillingAddress { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.VatNumber")]
        public string VatNumber { get; set; }
        
        //gift cards
        public IList<GiftCard> GiftCards { get; set; }

        //items
        public bool HasDownloadablePlans { get; set; }
        public IList<SubscriptionOrderItemModel> Items { get; set; }

        public IList<OrderItemModel> OrderItems { get; set; }

        //creation date
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //checkout attributes
        public string CheckoutAttributeInfo { get; set; }


        //order notes
        [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.DisplayToCustomer")]
        public bool AddSubscriptionOrderNoteDisplayToCustomer { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.Note")]
        [AllowHtml]
        public string AddSubscriptionOrderNoteMessage { get; set; }
        public bool AddSubscriptionOrderNoteHasDownload { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.Download")]
        [UIHint("Download")]
        public int AddSubscriptionOrderNoteDownloadId { get; set; }

        //refund info
        [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.PartialRefund.AmountToRefund")]
        public decimal AmountToRefund { get; set; }
        public decimal MaxAmountToRefund { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        //workflow info
        public bool CanCancelSubscriptionOrder { get; set; }
        public bool CanCapture { get; set; }
        public bool CanMarkSubscriptionOrderAsPaid { get; set; }
        public bool CanRefund { get; set; }
        public bool CanRefundOffline { get; set; }
        public bool CanPartiallyRefund { get; set; }
        public bool CanPartiallyRefundOffline { get; set; }
        public bool CanVoid { get; set; }
        public bool CanVoidOffline { get; set; }

        #region Nested Classes

        public partial class SubscriptionOrderItemModel : BaseNopEntityModel
        {
            public SubscriptionOrderItemModel()
            {
                ReturnRequestIds = new List<int>();
                PurchasedGiftCardIds = new List<int>();
            }
            public int PlanId { get; set; }
            public string PlanName { get; set; }
            public string VendorName { get; set; }
            public string Sku { get; set; }

            public string PictureThumbnailUrl { get; set; }

            public string UnitPriceInclTax { get; set; }
            public string UnitPriceExclTax { get; set; }
            public decimal UnitPriceInclTaxValue { get; set; }
            public decimal UnitPriceExclTaxValue { get; set; }

            public int Quantity { get; set; }

            public string DiscountInclTax { get; set; }
            public string DiscountExclTax { get; set; }
            public decimal DiscountInclTaxValue { get; set; }
            public decimal DiscountExclTaxValue { get; set; }

            public string SubTotalInclTax { get; set; }
            public string SubTotalExclTax { get; set; }
            public decimal SubTotalInclTaxValue { get; set; }
            public decimal SubTotalExclTaxValue { get; set; }

            public string AttributeInfo { get; set; }
            public string RecurringInfo { get; set; }
            public string RentalInfo { get; set; }
            public IList<int> ReturnRequestIds { get; set; }
            public IList<int> PurchasedGiftCardIds { get; set; }

            public bool IsDownload { get; set; }
            public int DownloadCount { get; set; }
            public DownloadActivationType DownloadActivationType { get; set; }
            public bool IsDownloadActivated { get; set; }
            public Guid LicenseDownloadGuid { get; set; }
        }

        public partial class OrderItemModel : BaseNopEntityModel
        {
            public OrderItemModel()
            {
                ReturnRequestIds = new List<int>();
                PurchasedGiftCardIds = new List<int>();
                ItemDetails = new List<ItemDetailModel>();
            }
       
            public IList<int> ReturnRequestIds { get; set; }
            public IList<int> PurchasedGiftCardIds { get; set; }
            public IList<ItemDetailModel> ItemDetails { get; set; }
            public partial class ItemDetailModel : BaseNopEntityModel
            {
                public ItemDetailModel()
                {
                    ReturnRequestIds = new List<int>();
                    PurchasedGiftCardIds = new List<int>();
                }
                public int ProductId { get; set; }
                public string ProductName { get; set; }
                public string VendorName { get; set; }
                public string Sku { get; set; }

                public string PictureThumbnailUrl { get; set; }

                public string UnitPriceInclTax { get; set; }
                public string UnitPriceExclTax { get; set; }
                public decimal UnitPriceInclTaxValue { get; set; }
                public decimal UnitPriceExclTaxValue { get; set; }

                public int Quantity { get; set; }

                public string DiscountInclTax { get; set; }
                public string DiscountExclTax { get; set; }
                public decimal DiscountInclTaxValue { get; set; }
                public decimal DiscountExclTaxValue { get; set; }

                public string SubTotalInclTax { get; set; }
                public string SubTotalExclTax { get; set; }
                public decimal SubTotalInclTaxValue { get; set; }
                public decimal SubTotalExclTaxValue { get; set; }

                public string AttributeInfo { get; set; }
                public string RecurringInfo { get; set; }
                public string RentalInfo { get; set; }
                public IList<int> ReturnRequestIds { get; set; }
                public IList<int> PurchasedGiftCardIds { get; set; }

               
            }

        }


        public partial class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopModel
        {
            [NopResourceDisplayName("Admin.SubscriptionOrders.Fields.GiftCardInfo")]
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class SubscriptionOrderNote : BaseNopEntityModel
        {
            public int SubscriptionOrderId { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.DisplayToCustomer")]
            public bool DisplayToCustomer { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.Note")]
            public string Note { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.Download")]
            public int DownloadId { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.Download")]
            public Guid DownloadGuid { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.SubscriptionOrderNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class UploadLicenseModel : BaseNopModel
        {
            public int SubscriptionOrderId { get; set; }

            public int SubscriptionOrderItemId { get; set; }

            [UIHint("Download")]
            public int LicenseDownloadId { get; set; }

        }

        public partial class AddSubscriptionOrderModel : BaseNopModel
        {
            public AddSubscriptionOrderModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailablePlanTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanName")]
            [AllowHtml]
            public string SearchPlanName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanType")]
            public int SearchPlanTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailablePlanTypes { get; set; }

            public int SubscriptionOrderId { get; set; }

            #region Nested classes
            
            public partial class PlanModel : BaseNopEntityModel
            {
                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.Name")]
                [AllowHtml]
                public string Name { get; set; }

                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.SKU")]
                [AllowHtml]
                public string Sku { get; set; }
            }

            public partial class PlanDetailsModel : BaseNopModel
            {
                public PlanDetailsModel()
                {
                    PlanAttributes = new List<PlanAttributeModel>();
                    GiftCard = new GiftCardModel();
                    Warnings = new List<string>();
                }

                public int PlanId { get; set; }

                public int SubscriptionOrderId { get; set; }

                public PlanType PlanType { get; set; }

                public string Name { get; set; }

                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.UnitPriceInclTax")]
                public decimal UnitPriceInclTax { get; set; }
                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.UnitPriceExclTax")]
                public decimal UnitPriceExclTax { get; set; }

                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.Quantity")]
                public int Quantity { get; set; }

                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.SubTotalInclTax")]
                public decimal SubTotalInclTax { get; set; }
                [NopResourceDisplayName("Admin.SubscriptionOrders.Plans.AddNew.SubTotalExclTax")]
                public decimal SubTotalExclTax { get; set; }

                //plan attributes
                public IList<PlanAttributeModel> PlanAttributes { get; set; }
                //gift card info
                public GiftCardModel GiftCard { get; set; }
                //rental
                public bool IsRental { get; set; }

                public List<string> Warnings { get; set; }

            }

            public partial class PlanAttributeModel : BaseNopEntityModel
            {
                public PlanAttributeModel()
                {
                    Values = new List<PlanAttributeValueModel>();
                }

                public int PlanAttributeId { get; set; }

                public string Name { get; set; }

                public string TextPrompt { get; set; }

                public bool IsRequired { get; set; }

                public AttributeControlType AttributeControlType { get; set; }

                public IList<PlanAttributeValueModel> Values { get; set; }
            }

            public partial class PlanAttributeValueModel : BaseNopEntityModel
            {
                public string Name { get; set; }

                public bool IsPreSelected { get; set; }
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
            #endregion
        }

        public partial class UsedDiscountModel:BaseNopModel
        {
            public int DiscountId { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }


    public partial class SubscriptionOrderAggreratorModel : BaseNopModel
    {
        //aggergator properties
        public string aggregatorprofit { get; set; }
        public string aggregatorshipping { get; set; }
        public string aggregatortax { get; set; }
        public string aggregatortotal { get; set; }
    }
}