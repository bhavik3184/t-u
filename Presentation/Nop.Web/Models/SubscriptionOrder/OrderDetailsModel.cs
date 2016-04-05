using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.SubscriptionOrder
{
    public partial class SubscriptionOrderDetailsModel : BaseNopEntityModel
    {
        public SubscriptionOrderDetailsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<SubscriptionOrderItemModel>();
            SubscriptionOrderNotes = new List<SubscriptionOrderNote>();
            Shipments = new List<ShipmentBriefModel>();
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
            CustomValues = new Dictionary<string, object>();
        }

        public bool PrintMode { get; set; }
        public bool PdfInvoiceDisabled { get; set; }

        public DateTime CreatedOn { get; set; }

        public string SubscriptionOrderStatus { get; set; }

        public bool IsReSubscriptionOrderAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }
        
        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        public string ShippingStatus { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public IList<ShipmentBriefModel> Shipments { get; set; }

        public AddressModel BillingAddress { get; set; }

        public string VatNumber { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentMethodStatus { get; set; }
        public bool CanRePostProcessPayment { get; set; }
        public Dictionary<string, object> CustomValues { get; set; }

        public string SubscriptionOrderSubtotal { get; set; }
        public string SubscriptionOrderSubTotalDiscount { get; set; }
        public string SubscriptionOrderShipping { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }
        public string CheckoutAttributeInfo { get; set; }

        public bool PricesIncludeTax { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public string SecurityDeposit { get; set; }
        public string RegistrationCharge { get; set; }
        public string SubscriptionOrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }
        public string SubscriptionOrderTotal { get; set; }
        
        public IList<GiftCard> GiftCards { get; set; }

        public bool ShowSku { get; set; }
        public IList<SubscriptionOrderItemModel> Items { get; set; }
        
        public IList<SubscriptionOrderNote> SubscriptionOrderNotes { get; set; }

		#region Nested Classes

        public partial class SubscriptionOrderItemModel : BaseNopEntityModel
        {
            public Guid SubscriptionOrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string UnitPrice { get; set; }
            public string SubTotal { get; set; }
            public int Quantity { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public int MaxNoOfDeliveries { get; set; }
            public int NoOfItemsToBorrow { get; set; }
            public string Duration { get; set; }
            //downloadable product properties
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
        }

        public partial class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class SubscriptionOrderNote : BaseNopEntityModel
        {
            public bool HasDownload { get; set; }
            public string Note { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public partial class ShipmentBriefModel : BaseNopEntityModel
        {
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }
		#endregion
    }
}