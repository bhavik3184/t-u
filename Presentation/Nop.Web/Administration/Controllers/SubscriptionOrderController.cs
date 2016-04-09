using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.SubscriptionOrders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
 
namespace Nop.Admin.Controllers
{
	public partial class SubscriptionOrderController : BaseAdminController
    {
        #region Fields

        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly ISubscriptionOrderReportService _orderReportService;
        private readonly ISubscriptionOrderProcessingService _orderProcessingService;
        private readonly IReturnRequestService _returnRequestService;
	    private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IEncryptionService _encryptionService;
        private readonly IPaymentService _paymentService;
        private readonly IMeasureService _measureService;
        private readonly IPdfService _pdfService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPlanService _planService;
        private readonly IExportManager _exportManager;
        private readonly IPermissionService _permissionService;
	    private readonly IWorkflowMessageService _workflowMessageService;
	    private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
	    private readonly IPlanAttributeFormatter _planAttributeFormatter;
        private readonly ISubscriptionCartService _subscriptionCartService;
        private readonly IGiftCardService _giftCardService;
        private readonly IDownloadService _downloadService;
        private readonly IShipmentService _shipmentService;
	    private readonly IShippingService _shippingService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
	    private readonly IAddressAttributeFormatter _addressAttributeFormatter;
	    private readonly IAffiliateService _affiliateService;
	    private readonly IPictureService _pictureService;

        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly MeasureSettings _measureSettings;
        private readonly AddressSettings _addressSettings;
	    private readonly ShippingSettings _shippingSettings;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IBorrowCartService _borrowCartService;
        

        #endregion

        #region Ctor

        public SubscriptionOrderController(ISubscriptionOrderService subscriptionService,
            ISubscriptionOrderReportService orderReportService,
            ISubscriptionOrderProcessingService orderProcessingService,
            IReturnRequestService returnRequestService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IEncryptionService encryptionService,
            IPaymentService paymentService,
            IMeasureService measureService,
            IPdfService pdfService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IPlanService planService,
            IExportManager exportManager,
            IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService,
            IPlanAttributeFormatter planAttributeFormatter, 
            ISubscriptionCartService subscriptionCartService,
            IGiftCardService giftCardService, 
            IDownloadService downloadService,
            IShipmentService shipmentService, 
            IShippingService shippingService,
            IStoreService storeService,
            IVendorService vendorService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAffiliateService affiliateService,
            IPictureService pictureService,
            CurrencySettings currencySettings, 
            TaxSettings taxSettings,
            MeasureSettings measureSettings,
            AddressSettings addressSettings,
            ShippingSettings shippingSettings,
            IProductAttributeService productAttributeService, 
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter, 
            IBorrowCartService borrowCartService)
		{
            this._subscriptionService = subscriptionService;
            this._orderReportService = orderReportService;
            this._orderProcessingService = orderProcessingService;
            this._returnRequestService = returnRequestService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._encryptionService = encryptionService;
            this._paymentService = paymentService;
            this._measureService = measureService;
            this._pdfService = pdfService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._planService = planService;
            this._exportManager = exportManager;
            this._permissionService = permissionService;
            this._workflowMessageService = workflowMessageService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._planAttributeFormatter = planAttributeFormatter;
            this._subscriptionCartService = subscriptionCartService;
            this._giftCardService = giftCardService;
            this._downloadService = downloadService;
            this._shipmentService = shipmentService;
            this._shippingService = shippingService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._affiliateService = affiliateService;
            this._pictureService = pictureService;

            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._measureSettings = measureSettings;
            this._addressSettings = addressSettings;
            this._shippingSettings = shippingSettings;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._borrowCartService = borrowCartService;
		}
        
        #endregion

        #region Utilities

        [NonAction]
        protected virtual bool HasAccessToSubscription(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorPlans = order.SubscriptionOrderItems.Any(subscriptionOrderItem => subscriptionOrderItem.Plan.VendorId == vendorId);
            return hasVendorPlans;
        }

        [NonAction]
        protected virtual bool HasAccessToSubscriptionItem(SubscriptionOrderItem subscriptionOrderItem)
        {
            if (subscriptionOrderItem == null)
                throw new ArgumentNullException("subscriptionOrderItem");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return subscriptionOrderItem.Plan.VendorId == vendorId;
        }

        [NonAction]
        protected virtual bool HasAccessToPlan(Plan plan)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return plan.VendorId == vendorId;
        }

        [NonAction]
        protected virtual bool HasAccessToShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var hasVendorPlans = false;
            var vendorId = _workContext.CurrentVendor.Id;
            
            return hasVendorPlans;
        }

        [NonAction]
        protected virtual void PrepareSubscriptionDetailsModel(SubscriptionOrderModel model, SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = order.Id;
            model.SubscriptionOrderStatus = order.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.SubscriptionOrderStatusId = order.SubscriptionOrderStatusId;
            model.SubscriptionOrderGuid = order.SubscriptionOrderGuid;
            var store = _storeService.GetStoreById(order.StoreId);
            model.StoreName = store != null ? store.Name : "Unknown";
            model.CustomerId = order.CustomerId;
            var customer = order.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.CustomerIp = order.CustomerIp;
            model.VatNumber = order.VatNumber;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;
            

            var affiliate = _affiliateService.GetAffiliateById(order.AffiliateId);
            if (affiliate != null)
            {
                model.AffiliateId = affiliate.Id;
                model.AffiliateName = affiliate.GetFullName();;
            }

            //a vendor should have access only to his plans
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            //custom values
            model.CustomValues = order.DeserializeCustomValues();
            
            #region Subscription totals

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");
            model.RegistrationCharge = _priceFormatter.FormatPrice(order.RegistrationCharge, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.RegistrationChargeDiscount = _priceFormatter.FormatPrice(order.RegistrationChargeDiscount, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.SecurityDeposit = _priceFormatter.FormatPrice(order.SecurityDeposit, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            //subtotal
            model.SubscriptionOrderSubtotalInclTax = _priceFormatter.FormatPrice(order.SubscriptionOrderSubtotalInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.SubscriptionOrderSubtotalExclTax = _priceFormatter.FormatPrice(order.SubscriptionOrderSubtotalExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.SubscriptionOrderSubtotalInclTaxValue = order.SubscriptionOrderSubtotalInclTax;
            model.SubscriptionOrderSubtotalExclTaxValue = order.SubscriptionOrderSubtotalExclTax;
            //discount (applied to order subtotal)
            string orderSubtotalDiscountInclTaxStr = _priceFormatter.FormatPrice(order.SubscriptionOrderSubTotalDiscountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            string orderSubtotalDiscountExclTaxStr = _priceFormatter.FormatPrice(order.SubscriptionOrderSubTotalDiscountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            if (order.SubscriptionOrderSubTotalDiscountInclTax > decimal.Zero)
                model.SubscriptionOrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.SubscriptionOrderSubTotalDiscountExclTax > decimal.Zero)
                model.SubscriptionOrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
            model.SubscriptionOrderSubTotalDiscountInclTaxValue = order.SubscriptionOrderSubTotalDiscountInclTax;
            model.SubscriptionOrderSubTotalDiscountExclTaxValue = order.SubscriptionOrderSubTotalDiscountExclTax;

            //shipping
            model.SubscriptionOrderShippingInclTax = _priceFormatter.FormatShippingPrice(order.SubscriptionOrderShippingInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.SubscriptionOrderShippingExclTax = _priceFormatter.FormatShippingPrice(order.SubscriptionOrderShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.SubscriptionOrderShippingInclTaxValue = order.SubscriptionOrderShippingInclTax;
            model.SubscriptionOrderShippingExclTaxValue = order.SubscriptionOrderShippingExclTax;

            //payment method additional fee
            if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
                model.PaymentMethodAdditionalFeeExclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            }
            model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
            model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;


            //tax
            model.Tax = _priceFormatter.FormatPrice(order.SubscriptionOrderTax, true, false);
            SortedDictionary<decimal, decimal> taxRates = order.TaxRatesDictionary;
            bool displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
            bool displayTax = !displayTaxRates;
            foreach (var tr in order.TaxRatesDictionary)
            {
                model.TaxRates.Add(new SubscriptionOrderModel.TaxRate
                {
                    Rate = _priceFormatter.FormatTaxRate(tr.Key),
                    Value = _priceFormatter.FormatPrice(tr.Value, true, false),
                });
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.TaxValue = order.SubscriptionOrderTax;
            model.TaxRatesValue = order.TaxRates;

            //discount
            if (order.SubscriptionOrderDiscount > 0)
                model.SubscriptionOrderTotalDiscount = _priceFormatter.FormatPrice(-order.SubscriptionOrderDiscount, true, false);
            model.SubscriptionOrderTotalDiscountValue = order.SubscriptionOrderDiscount;

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new SubscriptionOrderModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-gcuh.UsedValue, true, false),
                });
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-order.RedeemedRewardPointsEntry.UsedAmount, true, false);
            }

            //total
            model.SubscriptionOrderTotal = _priceFormatter.FormatPrice(order.SubscriptionOrderTotal, true, false);
            model.SubscriptionOrderTotalValue = order.SubscriptionOrderTotal;

            //refunded amount
            if (order.RefundedAmount > decimal.Zero)
                model.RefundedAmount = _priceFormatter.FormatPrice(order.RefundedAmount, true, false);

            //used discounts
            var duh = _discountService.GetAllDiscountUsageHistory(orderId: order.Id);
            foreach (var d in duh)
            {
                model.UsedDiscounts.Add(new SubscriptionOrderModel.UsedDiscountModel
                {
                    DiscountId = d.DiscountId,
                    DiscountName = d.Discount.Name
                });
            }

            //profit (hide for vendors)
            if (_workContext.CurrentVendor == null)
            {
                var profit = _orderReportService.ProfitReport(orderId: order.Id);
                model.Profit = _priceFormatter.FormatPrice(profit, true, false);
            }

            #endregion

            #region Payment info

            if (order.AllowStoringCreditCardNumber)
            {
                //card type
                model.CardType = _encryptionService.DecryptText(order.CardType);
                //cardholder name
                model.CardName = _encryptionService.DecryptText(order.CardName);
                //card number
                model.CardNumber = _encryptionService.DecryptText(order.CardNumber);
                //cvv
                model.CardCvv2 = _encryptionService.DecryptText(order.CardCvv2);
                //expiry date
                string cardExpirationMonthDecrypted = _encryptionService.DecryptText(order.CardExpirationMonth);
                if (!String.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                    model.CardExpirationMonth = cardExpirationMonthDecrypted;
                string cardExpirationYearDecrypted = _encryptionService.DecryptText(order.CardExpirationYear);
                if (!String.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                    model.CardExpirationYear = cardExpirationYearDecrypted;

                model.AllowStoringCreditCardNumber = true;
            }
            else
            {
                string maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(order.MaskedCreditCardNumber);
                if (!String.IsNullOrEmpty(maskedCreditCardNumberDecrypted))
                    model.CardNumber = maskedCreditCardNumberDecrypted;
            }


            //payment transaction info
            model.AuthorizationTransactionId = order.AuthorizationTransactionId;
            model.CaptureTransactionId = order.CaptureTransactionId;
            model.SubscriptionTransactionId = order.SubscriptionTransactionId;

            //payment method info
            var pm = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
            model.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);

            //payment method buttons
            model.CanCancelSubscriptionOrder = _orderProcessingService.CanCancelSubscriptionOrder(order);
            model.CanCapture = _orderProcessingService.CanCapture(order);
            model.CanMarkSubscriptionOrderAsPaid = _orderProcessingService.CanMarkSubscriptionOrderAsPaid(order);
            model.CanRefund = _orderProcessingService.CanRefund(order);
            model.CanRefundOffline = _orderProcessingService.CanRefundOffline(order);
            model.CanPartiallyRefund = _orderProcessingService.CanPartiallyRefund(order, decimal.Zero);
            model.CanPartiallyRefundOffline = _orderProcessingService.CanPartiallyRefundOffline(order, decimal.Zero);
            model.CanVoid = _orderProcessingService.CanVoid(order);
            model.CanVoidOffline = _orderProcessingService.CanVoidOffline(order);
            
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.MaxAmountToRefund = order.SubscriptionOrderTotal - order.RefundedAmount;

            //recurring payment record
            var recurringPayment = _subscriptionService.SearchRecurringPayments(initialSubscriptionOrderId: order.Id, showHidden: true).FirstOrDefault();
            if (recurringPayment != null)
            {
                model.RecurringPaymentId = recurringPayment.Id;
            }
            #endregion

            #region Billing & shipping info

            model.BillingAddress = order.BillingAddress.ToModel();
            model.BillingAddress.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(order.BillingAddress.CustomAttributes);
            model.BillingAddress.FirstNameEnabled = true;
            model.BillingAddress.FirstNameRequired = true;
            model.BillingAddress.LastNameEnabled = true;
            model.BillingAddress.LastNameRequired = true;
            model.BillingAddress.EmailEnabled = true;
            model.BillingAddress.EmailRequired = true;
            model.BillingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.BillingAddress.CompanyRequired = _addressSettings.CompanyRequired;
            model.BillingAddress.CountryEnabled = _addressSettings.CountryEnabled;
            model.BillingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.BillingAddress.CityEnabled = _addressSettings.CityEnabled;
            model.BillingAddress.CityRequired = _addressSettings.CityRequired;
            model.BillingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.BillingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.BillingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.BillingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.BillingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.BillingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.BillingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.BillingAddress.PhoneRequired = _addressSettings.PhoneRequired;
            model.BillingAddress.FaxEnabled = _addressSettings.FaxEnabled;
            model.BillingAddress.FaxRequired = _addressSettings.FaxRequired;

            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext); ;
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;

                model.PickUpInStore = order.PickUpInStore;
                if (!order.PickUpInStore)
                {
                    model.ShippingAddress = order.ShippingAddress.ToModel();
                    model.ShippingAddress.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(order.ShippingAddress.CustomAttributes);
                    model.ShippingAddress.FirstNameEnabled = true;
                    model.ShippingAddress.FirstNameRequired = true;
                    model.ShippingAddress.LastNameEnabled = true;
                    model.ShippingAddress.LastNameRequired = true;
                    model.ShippingAddress.EmailEnabled = true;
                    model.ShippingAddress.EmailRequired = true;
                    model.ShippingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
                    model.ShippingAddress.CompanyRequired = _addressSettings.CompanyRequired;
                    model.ShippingAddress.CountryEnabled = _addressSettings.CountryEnabled;
                    model.ShippingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
                    model.ShippingAddress.CityEnabled = _addressSettings.CityEnabled;
                    model.ShippingAddress.CityRequired = _addressSettings.CityRequired;
                    model.ShippingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
                    model.ShippingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
                    model.ShippingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
                    model.ShippingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
                    model.ShippingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
                    model.ShippingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
                    model.ShippingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
                    model.ShippingAddress.PhoneRequired = _addressSettings.PhoneRequired;
                    model.ShippingAddress.FaxEnabled = _addressSettings.FaxEnabled;
                    model.ShippingAddress.FaxRequired = _addressSettings.FaxRequired;
                    
                    model.ShippingAddressGoogleMapsUrl = string.Format("http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={0}", Server.UrlEncode(order.ShippingAddress.Address1 + " " + order.ShippingAddress.ZipPostalCode + " " + order.ShippingAddress.City + " " + (order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "")));
                }
                model.ShippingMethod = order.ShippingMethod;

              //  model.CanAddNewShipments = order.HasItemsToAddToShipment();
            }

            #endregion

            #region Plans

            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
            bool hasDownloadableItems = false;
            var plans = order.SubscriptionOrderItems;
            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                plans = plans
                    .Where(subscriptionOrderItem => subscriptionOrderItem.Plan.VendorId == _workContext.CurrentVendor.Id)
                    .ToList();
            }
            foreach (var subscriptionOrderItem in plans)
            {
                if (subscriptionOrderItem.Plan.IsDownload)
                    hasDownloadableItems = true;

                var subscriptionOrderItemModel = new SubscriptionOrderModel.SubscriptionOrderItemModel
                {
                    Id = subscriptionOrderItem.Id,
                    PlanId = subscriptionOrderItem.PlanId,
                    PlanName = subscriptionOrderItem.Plan.Name,
                    Sku = subscriptionOrderItem.Plan.Sku,
                    Quantity = subscriptionOrderItem.Quantity,
                    IsDownload = subscriptionOrderItem.Plan.IsDownload,
                    DownloadCount = subscriptionOrderItem.DownloadCount,
                    DownloadActivationType = subscriptionOrderItem.Plan.DownloadActivationType,
                    IsDownloadActivated = subscriptionOrderItem.IsDownloadActivated
                };
                //picture
                //var subscriptionOrderItemPicture = subscriptionOrderItem.Plan.GetPlanPicture(subscriptionOrderItem.AttributesXml, _pictureService, _planAttributeParser);
                //subscriptionOrderItemModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(subscriptionOrderItemPicture, 75, true);

                //license file
                if (subscriptionOrderItem.LicenseDownloadId.HasValue)
                {
                    var licenseDownload = _downloadService.GetDownloadById(subscriptionOrderItem.LicenseDownloadId.Value);
                    if (licenseDownload != null)
                    {
                        subscriptionOrderItemModel.LicenseDownloadGuid = licenseDownload.DownloadGuid;
                    }
                }
                //vendor
                var vendor = _vendorService.GetVendorById(subscriptionOrderItem.Plan.VendorId);
                subscriptionOrderItemModel.VendorName = vendor != null ? vendor.Name : "";

                //unit price
                subscriptionOrderItemModel.UnitPriceInclTaxValue = subscriptionOrderItem.UnitPriceInclTax;
                subscriptionOrderItemModel.UnitPriceExclTaxValue = subscriptionOrderItem.UnitPriceExclTax;
                subscriptionOrderItemModel.UnitPriceInclTax = _priceFormatter.FormatPrice(subscriptionOrderItem.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                subscriptionOrderItemModel.UnitPriceExclTax = _priceFormatter.FormatPrice(subscriptionOrderItem.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //discounts
                subscriptionOrderItemModel.DiscountInclTaxValue = subscriptionOrderItem.DiscountAmountInclTax;
                subscriptionOrderItemModel.DiscountExclTaxValue = subscriptionOrderItem.DiscountAmountExclTax;
                subscriptionOrderItemModel.DiscountInclTax = _priceFormatter.FormatPrice(subscriptionOrderItem.DiscountAmountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                subscriptionOrderItemModel.DiscountExclTax = _priceFormatter.FormatPrice(subscriptionOrderItem.DiscountAmountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //subtotal
                subscriptionOrderItemModel.SubTotalInclTaxValue = subscriptionOrderItem.PriceInclTax;
                subscriptionOrderItemModel.SubTotalExclTaxValue = subscriptionOrderItem.PriceExclTax;
                subscriptionOrderItemModel.SubTotalInclTax = _priceFormatter.FormatPrice(subscriptionOrderItem.PriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                subscriptionOrderItemModel.SubTotalExclTax = _priceFormatter.FormatPrice(subscriptionOrderItem.PriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                subscriptionOrderItemModel.AttributeInfo = subscriptionOrderItem.AttributeDescription;
                if (subscriptionOrderItem.Plan.IsRecurring)
                    subscriptionOrderItemModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.SubscriptionOrders.Plans.RecurringPeriod"), subscriptionOrderItem.Plan.RecurringCycleLength, subscriptionOrderItem.Plan.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));
                //rental info
                if (subscriptionOrderItem.Plan.IsRental)
                {
                    var rentalStartDate = subscriptionOrderItem.RentalStartDateUtc.HasValue ? subscriptionOrderItem.Plan.FormatRentalDate(subscriptionOrderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = subscriptionOrderItem.RentalEndDateUtc.HasValue ? subscriptionOrderItem.Plan.FormatRentalDate(subscriptionOrderItem.RentalEndDateUtc.Value) : "";
                    subscriptionOrderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Subscription.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //return requests
                
                //gift cards
                subscriptionOrderItemModel.PurchasedGiftCardIds = _giftCardService.GetGiftCardsByPurchasedWithSubscriptionOrderItemId(subscriptionOrderItem.Id)
                    .Select(gc => gc.Id).ToList();

                model.Items.Add(subscriptionOrderItemModel);
            }
            model.HasDownloadablePlans = hasDownloadableItems;
            #endregion

            #region Products

            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
            var products = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                //products = products
                //    .Where(orderItem => orderItem.Product.VendorId == _workContext.CurrentVendor.Id)
                //    .ToList();
            }
            foreach (var orderItem in products)
            {
                var orderItemModel = new SubscriptionOrderModel.OrderItemModel
                {
                    Id = orderItem.Id,
                };

                foreach (var itemDetail in orderItem.ItemDetails)
                {
                    if (itemDetail.Product.IsDownload)
                        hasDownloadableItems = true;

                    var itemDetailModel = new SubscriptionOrderModel.OrderItemModel.ItemDetailModel
                    {
                        Id = itemDetail.Id,
                        ProductId = itemDetail.ProductId,
                        ProductName = itemDetail.Product.Name,
                        Sku = itemDetail.Product.Sku,
                        Quantity = itemDetail.Quantity,
                    };
                   
                    //picture
                    var orderItemPicture = itemDetail.Product.GetProductPicture(itemDetail.AttributesXml, _pictureService, _productAttributeParser);
                    itemDetailModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(orderItemPicture, 75, true);
                  
                    //vendor
                    var vendor = _vendorService.GetVendorById(itemDetail.Product.VendorId);
                    itemDetailModel.VendorName = vendor != null ? vendor.Name : "";

                    //unit price
                    itemDetailModel.UnitPriceInclTaxValue = itemDetail.UnitPriceInclTax;
                    itemDetailModel.UnitPriceExclTaxValue = itemDetail.UnitPriceExclTax;
                    itemDetailModel.UnitPriceInclTax = _priceFormatter.FormatPrice(itemDetail.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                    itemDetailModel.UnitPriceExclTax = _priceFormatter.FormatPrice(itemDetail.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                    //discounts
                    itemDetailModel.DiscountInclTaxValue = itemDetail.DiscountAmountInclTax;
                    itemDetailModel.DiscountExclTaxValue = itemDetail.DiscountAmountExclTax;
                    itemDetailModel.DiscountInclTax = _priceFormatter.FormatPrice(itemDetail.DiscountAmountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                    itemDetailModel.DiscountExclTax = _priceFormatter.FormatPrice(itemDetail.DiscountAmountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                    //subtotal
                    itemDetailModel.SubTotalInclTaxValue = itemDetail.PriceInclTax;
                    itemDetailModel.SubTotalExclTaxValue = itemDetail.PriceExclTax;
                    itemDetailModel.SubTotalInclTax = _priceFormatter.FormatPrice(itemDetail.PriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                    itemDetailModel.SubTotalExclTax = _priceFormatter.FormatPrice(itemDetail.PriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                    itemDetailModel.AttributeInfo = itemDetail.AttributeDescription;
                    if (itemDetail.Product.IsRecurring)
                        itemDetailModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.Orders.Products.RecurringPeriod"), itemDetail.Product.RecurringCycleLength, itemDetail.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));
                    //rental info

                    //return requests
                    orderItemModel.ReturnRequestIds = _returnRequestService.SearchReturnRequests(itemDetailId: itemDetail.Id)
                        .Select(rr => rr.Id).ToList();
                    //gift cards

                    orderItemModel.ItemDetails.Add(itemDetailModel);
                    
                }

                model.OrderItems.Add(orderItemModel);
            }
            
            #endregion
        }

        [NonAction]
        protected virtual SubscriptionOrderModel.AddSubscriptionOrderModel.PlanDetailsModel PrepareAddPlanToSubscriptionOrderModel(int orderId, int planId)
        {
            var plan = _planService.GetPlanById(planId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            var presetQty = 1;
            var presetPrice = _priceCalculationService.GetFinalPrice(plan, order.Customer, decimal.Zero, true, presetQty);
            decimal taxRate;
            decimal presetPriceInclTax = _taxService.GetPlanPrice(plan, presetPrice, true, order.Customer, out taxRate);
            decimal presetPriceExclTax = _taxService.GetPlanPrice(plan, presetPrice, false, order.Customer, out taxRate);

            var model = new SubscriptionOrderModel.AddSubscriptionOrderModel.PlanDetailsModel
            {
                PlanId = planId,
                SubscriptionOrderId = orderId,
                Name = plan.Name,
                PlanType = plan.PlanType,
                UnitPriceExclTax = presetPriceExclTax,
                UnitPriceInclTax = presetPriceInclTax,
                Quantity = presetQty,
                SubTotalExclTax = presetPriceExclTax,
                SubTotalInclTax = presetPriceInclTax
            };

            //attributes
            //var attributes = _planAttributeService.GetPlanAttributeMappingsByPlanId(plan.Id);
            //foreach (var attribute in attributes)
            //{
            //    var attributeModel = new SubscriptionOrderModel.AddSubscriptionOrderModel.PlanAttributeModel
            //    {
            //        Id = attribute.Id,
            //        PlanAttributeId = attribute.PlanAttributeId,
            //        Name = attribute.PlanAttribute.Name,
            //        TextPrompt = attribute.TextPrompt,
            //        IsRequired = attribute.IsRequired,
            //        AttributeControlType = attribute.AttributeControlType
            //    };

            //    if (attribute.ShouldHaveValues())
            //    {
            //        //values
            //        var attributeValues = _planAttributeService.GetPlanAttributeValues(attribute.Id);
            //        foreach (var attributeValue in attributeValues)
            //        {
            //            var attributeValueModel = new SubscriptionOrderModel.AddSubscriptionOrderModel.PlanAttributeValueModel
            //            {
            //                Id = attributeValue.Id,
            //                Name = attributeValue.Name,
            //                IsPreSelected = attributeValue.IsPreSelected
            //            };
            //            attributeModel.Values.Add(attributeValueModel);
            //        }
            //    }

            //    model.PlanAttributes.Add(attributeModel);
            //}
            //gift card
            model.GiftCard.IsGiftCard = plan.IsGiftCard;
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = plan.GiftCardType;
            }
            //rental
            model.IsRental = plan.IsRental;
            return model;
        }

        [NonAction]
        protected virtual ShipmentModel PrepareShipmentModel(Shipment shipment, bool preparePlans, bool prepareShipmentEvent = false)
        {
            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var model = new ShipmentModel
            {
                Id = shipment.Id,
                TrackingNumber = shipment.TrackingNumber,
                TotalWeight = shipment.TotalWeight.HasValue ? string.Format("{0:F2} [{1}]", shipment.TotalWeight, baseWeightIn) : "",
                ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.ShippedDate.NotYet"),
                ShippedDateUtc = shipment.ShippedDateUtc,
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.DeliveryDate.NotYet"),
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                AdminComment = shipment.AdminComment,
                SubscriptionOrderId = shipment.OrderItem.SubscriptionOrderId,
            };

            if (preparePlans)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {

                    var itemDetail = _subscriptionService.GetItemDetailById(shipmentItem.ItemDetailId);
                    if (itemDetail == null)
                        continue;
                     

                        //quantities
                        var qtyInThisShipment = shipmentItem.Quantity;
                        var maxQtyToAdd = itemDetail.GetTotalNumberOfItemsCanBeAddedToShipment();
                        var qtyOrdered = itemDetail.Quantity;
                        var qtyInAllShipments = itemDetail.GetTotalNumberOfItemsInAllShipment();

                        var warehouse = _shippingService.GetWarehouseById(shipmentItem.WarehouseId);
                        var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                        {
                            Id = shipmentItem.Id,
                            ItemDetailId = itemDetail.OrderItemId,
                            ProductId = itemDetail.ProductId,
                            ProductName = itemDetail.Product.Name,
                            Sku = itemDetail.Product.FormatSku(itemDetail.AttributesXml, _productAttributeParser),
                            AttributeInfo = itemDetail.AttributeDescription,
                            ShippedFromWarehouse = warehouse != null ? warehouse.Name : null,
                            ShipSeparately = itemDetail.Product.ShipSeparately,
                            ItemWeight = itemDetail.ItemWeight.HasValue ? string.Format("{0:F2} [{1}]", itemDetail.ItemWeight, baseWeightIn) : "",
                            ItemDimensions = string.Format("{0:F2} x {1:F2} x {2:F2} [{3}]", itemDetail.Product.Length, itemDetail.Product.Width, itemDetail.Product.Height, baseDimensionIn),
                            QuantityOrdered = qtyOrdered,
                            QuantityInThisShipment = qtyInThisShipment,
                            QuantityInAllShipments = qtyInAllShipments,
                            QuantityToAdd = maxQtyToAdd,
                        };
                       
                        model.Items.Add(shipmentItemModel);
                }
            }

            if (prepareShipmentEvent && !String.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var order = shipment.OrderItem.SubscriptionOrder;
                var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(order.ShippingRateComputationMethodSystemName);
                if (srcm != null &&
                    srcm.PluginDescriptor.Installed &&
                    srcm.IsShippingRateComputationMethodActive(_shippingSettings))
                {
                    var shipmentTracker = srcm.ShipmentTracker;
                    if (shipmentTracker != null)
                    {
                        model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
                        if (_shippingSettings.DisplayShipmentEventsToStoreOwner)
                        {
                            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
                            if (shipmentEvents != null)
                            {
                                foreach (var shipmentEvent in shipmentEvents)
                                {
                                    var shipmentStatusEventModel = new ShipmentModel.ShipmentStatusEventModel();
                                    var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                                    shipmentStatusEventModel.Country = shipmentEventCountry != null
                                        ? shipmentEventCountry.GetLocalized(x => x.Name)
                                        : shipmentEvent.CountryCode;
                                    shipmentStatusEventModel.Date = shipmentEvent.Date;
                                    shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                                    shipmentStatusEventModel.Location = shipmentEvent.Location;
                                    model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                                }
                            }
                        }
                    }
                }
            }

             

            return model;
        }

        #endregion

        #region Subscription list

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? orderStatusId = null,
            int? paymentStatusId = null, int? shippingStatusId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //order statuses
            var model = new SubscriptionOrderListModel();
            model.AvailableSubscriptionOrderStatuses = SubscriptionOrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableSubscriptionOrderStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            if (orderStatusId.HasValue)
            {
                //pre-select value?
                var item = model.AvailableSubscriptionOrderStatuses.FirstOrDefault(x => x.Value == orderStatusId.Value.ToString());
                if (item != null)
                    item.Selected = true;
            }

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            if (paymentStatusId.HasValue)
            {
                //pre-select value?
                var item = model.AvailablePaymentStatuses.FirstOrDefault(x => x.Value == paymentStatusId.Value.ToString());
                if (item != null)
                    item.Selected = true;
            }

            //shipping statuses
            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            if (shippingStatusId.HasValue)
            {
                //pre-select value?
                var item = model.AvailableShippingStatuses.FirstOrDefault(x => x.Value == shippingStatusId.Value.ToString());
                if (item != null)
                    item.Selected = true;
            }

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var w in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            //payment methods
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var pm in _paymentService.LoadAllPaymentMethods())
                model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
            {
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //a vendor should have access only to orders with his plans
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            return View(model);
		}

		[HttpPost]
		public ActionResult SubscriptionList(DataSourceRequest command, SubscriptionOrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;

		    var filterByPlanId = 0;
		    var plan = _planService.GetPlanById(model.PlanId);
		    if (plan != null && HasAccessToPlan(plan))
                filterByPlanId = model.PlanId;

            //load orders
            var orders = _subscriptionService.SearchSubscriptionOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                planId: filterByPlanId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue, 
                createdToUtc: endDateValue,
                os: orderStatus, 
                ps: paymentStatus, 
                ss: shippingStatus, 
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.SubscriptionOrderNotes,
                orderGuid: model.SubscriptionOrderGuid,
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new SubscriptionOrderModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        SubscriptionOrderTotal = _priceFormatter.FormatPrice(x.SubscriptionOrderTotal, true, false),
                        SubscriptionOrderStatus = x.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        CustomerEmail = x.BillingAddress.Email,
                        CustomerFullName = string.Format("{0} {1}", x.BillingAddress.FirstName, x.BillingAddress.LastName),
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = orders.TotalCount
            };

            //summary report
            //currently we do not support planId and warehouseId parameters for this report
            var reportSummary = _orderReportService.GetSubscriptionOrderAverageReportLine(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                orderId: 0,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.SubscriptionOrderNotes);
            var profit = _orderReportService.ProfitReport(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                os: orderStatus,
                ps: paymentStatus, 
                ss: shippingStatus, 
                startTimeUtc: startDateValue, 
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.SubscriptionOrderNotes);
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            gridModel.ExtraData = new SubscriptionOrderAggreratorModel
            {
                aggregatorprofit = _priceFormatter.FormatPrice(profit, true, false),
                aggregatorshipping = _priceFormatter.FormatShippingPrice(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false),
                aggregatortax = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false),
                aggregatortotal = _priceFormatter.FormatPrice(reportSummary.SumSubscriptionOrders, true, false)
            };
			return new JsonResult
			{
				Data = gridModel
			};
		}
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public ActionResult GoToSubscriptionId(SubscriptionOrderListModel model)
        {
            var order = _subscriptionService.GetOrderById(model.GoDirectlyToNumber);
            if (order == null)
                return List();
            
            return RedirectToAction("Edit", "Subscription", new { id = order.Id });
        }

        public ActionResult PlanSearchAutoComplete(string term)
        {
            const int searchTermMinimumLength = 3;
            if (String.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //a vendor should have access only to his plans
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            //plans
            const int planNumber = 15;
            var plans = _planService.SearchPlans(
                vendorId: vendorId,
                keywords: term,
                pageSize: planNumber,
                showHidden: true);

            var result = (from p in plans
                          select new
                          {
                              label = p.Name,
                              planid = p.Id
                          })
                          .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public ActionResult ExportXmlAll(SubscriptionOrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;

            var filterByPlanId = 0;
            var plan = _planService.GetPlanById(model.PlanId);
            if (plan != null && HasAccessToPlan(plan))
                filterByPlanId = model.PlanId;

            //load orders
            var orders = _subscriptionService.SearchSubscriptionOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                planId: filterByPlanId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.SubscriptionOrderNotes,
                orderGuid: model.SubscriptionOrderGuid);

            try
            {
              //  var xml = _exportManager.ExportSubscriptionOrdersToXml(orders);
               return new XmlDownloadResult("", "orders.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            var orders = new List<SubscriptionOrder>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_subscriptionService.GetSubscriptionOrdersByIds(ids));
            }

           // var xml = _exportManager.ExportSubscriptionOrdersToXml(orders);
            var xml = "";
            return new XmlDownloadResult(xml, "orders.xml");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public ActionResult ExportExcelAll(SubscriptionOrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;

            var filterByPlanId = 0;
            var plan = _planService.GetPlanById(model.PlanId);
            if (plan != null && HasAccessToPlan(plan))
                filterByPlanId = model.PlanId;

            //load orders
            var orders = _subscriptionService.SearchSubscriptionOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                planId: filterByPlanId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.SubscriptionOrderNotes,
                orderGuid: model.SubscriptionOrderGuid);

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                   // _exportManager.ExportSubscriptionOrdersToXlsx(stream, orders);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "orders.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            var orders = new List<SubscriptionOrder>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_subscriptionService.GetSubscriptionOrdersByIds(ids));
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
              //  _exportManager.ExportSubscriptionOrdersToXlsx(stream, orders);
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "orders.xlsx");
        }

        #endregion

        #region Subscription details

        #region Payments and other order workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public ActionResult CancelSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _orderProcessingService.CancelSubscriptionOrder(order, true);
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("captureorder")]
        public ActionResult CaptureSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                var errors = _orderProcessingService.Capture(order);
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }

        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public ActionResult MarkSubscriptionAsPaid(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                
                _orderProcessingService.MarkSubscriptionOrderAsPaid(order);
               
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorder")]
        public ActionResult RefundSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                var errors = _orderProcessingService.Refund(order);
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorderoffline")]
        public ActionResult RefundSubscriptionOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _orderProcessingService.RefundOffline(order);
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorder")]
        public ActionResult VoidSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                var errors = _orderProcessingService.Void(order);
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorderoffline")]
        public ActionResult VoidSubscriptionOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _orderProcessingService.VoidOffline(order);
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }
        
        public ActionResult PartiallyRefundSubscriptionPopup(int id, bool online)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            var model = new SubscriptionOrderModel();
            PrepareSubscriptionDetailsModel(model, order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public ActionResult PartiallyRefundSubscriptionPopup(string btnId, string formId, int id, bool online, SubscriptionOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                decimal amountToRefund = model.AmountToRefund;
                if (amountToRefund <= decimal.Zero)
                    throw new NopException("Enter amount to refund");

                decimal maxAmountToRefund = order.SubscriptionOrderTotal - order.RefundedAmount;
                if (amountToRefund > maxAmountToRefund)
                    amountToRefund = maxAmountToRefund;

                var errors = new List<string>();
                if (online)
                    errors = _orderProcessingService.PartiallyRefund(order, amountToRefund).ToList();
                else
                    _orderProcessingService.PartiallyRefundOffline(order, amountToRefund);

                if (errors.Count == 0)
                {
                    //success
                    ViewBag.RefreshPage = true;
                    ViewBag.btnId = btnId;
                    ViewBag.formId = formId;

                    PrepareSubscriptionDetailsModel(model, order);
                    return View(model);
                }
                
                //error
                PrepareSubscriptionDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveSubscriptionOrderStatus")]
        public ActionResult ChangeSubscriptionOrderStatus(int id, SubscriptionOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                order.SubscriptionOrderStatusId = model.SubscriptionOrderStatusId;
                _subscriptionService.UpdateSubscriptionOrder(order);

                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Subscription status has been edited. New status: {0}", order.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext)),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        #endregion

        #region Edit, delete

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null || order.Deleted)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null && !HasAccessToSubscription(order))
                return RedirectToAction("List");

            var model = new SubscriptionOrderModel();
            PrepareSubscriptionDetailsModel(model, order);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            _orderProcessingService.DeleteSubscriptionOrder(order);
            return RedirectToAction("List");
        }

        public ActionResult PdfInvoice(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = orderId });

            var order = _subscriptionService.GetOrderById(orderId);
            var orders = new List<SubscriptionOrder>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", string.Format("order_{0}.pdf", order.Id));
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("pdf-invoice-all")]
        public ActionResult PdfInvoiceAll(SubscriptionOrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;

            var filterByPlanId = 0;
            var plan = _planService.GetPlanById(model.PlanId);
            if (plan != null && HasAccessToPlan(plan))
                filterByPlanId = model.PlanId;

            //load orders
            var orders = _subscriptionService.SearchSubscriptionOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                planId: filterByPlanId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.SubscriptionOrderNotes,
                orderGuid: model.SubscriptionOrderGuid);

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "orders.pdf");
        }

        [HttpPost]
        public ActionResult PdfInvoiceSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var orders = new List<SubscriptionOrder>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_subscriptionService.GetSubscriptionOrdersByIds(ids));
            }

            
            //ensure that we at least one order selected
            if (orders.Count == 0)
            {
                ErrorNotification(_localizationService.GetResource("Admin.SubscriptionOrders.PdfInvoice.NoSubscriptions"));
                return RedirectToAction("List");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "orders.pdf");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public ActionResult EditCreditCardInfo(int id, SubscriptionOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            if (order.AllowStoringCreditCardNumber)
            {
                string cardType = model.CardType;
                string cardName = model.CardName;
                string cardNumber = model.CardNumber;
                string cardCvv2 = model.CardCvv2;
                string cardExpirationMonth = model.CardExpirationMonth;
                string cardExpirationYear = model.CardExpirationYear;

                order.CardType = _encryptionService.EncryptText(cardType);
                order.CardName = _encryptionService.EncryptText(cardName);
                order.CardNumber = _encryptionService.EncryptText(cardNumber);
                order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
                order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
                order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
                order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
                _subscriptionService.UpdateSubscriptionOrder(order);
            }

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Credit card info has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            PrepareSubscriptionDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveSubscriptionOrderTotals")]
        public ActionResult EditSubscriptionOrderTotals(int id, SubscriptionOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            order.SubscriptionOrderSubtotalInclTax = model.SubscriptionOrderSubtotalInclTaxValue;
            order.SubscriptionOrderSubtotalExclTax = model.SubscriptionOrderSubtotalExclTaxValue;
            order.SubscriptionOrderSubTotalDiscountInclTax = model.SubscriptionOrderSubTotalDiscountInclTaxValue;
            order.SubscriptionOrderSubTotalDiscountExclTax = model.SubscriptionOrderSubTotalDiscountExclTaxValue;
            order.SubscriptionOrderShippingInclTax = model.SubscriptionOrderShippingInclTaxValue;
            order.SubscriptionOrderShippingExclTax = model.SubscriptionOrderShippingExclTaxValue;
            order.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
            order.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
            order.TaxRates = model.TaxRatesValue;
            order.SubscriptionOrderTax = model.TaxValue;
            order.SubscriptionOrderDiscount = model.SubscriptionOrderTotalDiscountValue;
            order.SubscriptionOrderTotal = model.SubscriptionOrderTotalValue;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Subscription totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            PrepareSubscriptionDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("save-shipping-method")]
        public ActionResult EditShippingMethod(int id, SubscriptionOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            order.ShippingMethod = model.ShippingMethod;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Shipping method has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            PrepareSubscriptionDetailsModel(model, order);

            //selected tab
            SaveSelectedTabIndex(persistForTheNextRequest: false);

            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveSubscriptionItem")]
        [ValidateInput(false)]
        public ActionResult EditSubscriptionItem(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            //get order item identifier
            int subscriptionOrderItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnSaveSubscriptionItem", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionOrderItemId = Convert.ToInt32(formValue.Substring("btnSaveSubscriptionItem".Length));

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == subscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");


            decimal unitPriceInclTax, unitPriceExclTax, discountInclTax, discountExclTax,priceInclTax,priceExclTax;
            int quantity;
            if (!decimal.TryParse(form["pvUnitPriceInclTax" + subscriptionOrderItemId], out unitPriceInclTax))
                unitPriceInclTax = subscriptionOrderItem.UnitPriceInclTax;
            if (!decimal.TryParse(form["pvUnitPriceExclTax" + subscriptionOrderItemId], out unitPriceExclTax))
                unitPriceExclTax = subscriptionOrderItem.UnitPriceExclTax;
            if (!int.TryParse(form["pvQuantity" + subscriptionOrderItemId], out quantity))
                quantity = subscriptionOrderItem.Quantity;
            if (!decimal.TryParse(form["pvDiscountInclTax" + subscriptionOrderItemId], out discountInclTax))
                discountInclTax = subscriptionOrderItem.DiscountAmountInclTax;
            if (!decimal.TryParse(form["pvDiscountExclTax" + subscriptionOrderItemId], out discountExclTax))
                discountExclTax = subscriptionOrderItem.DiscountAmountExclTax;
            if (!decimal.TryParse(form["pvPriceInclTax" + subscriptionOrderItemId], out priceInclTax))
                priceInclTax = subscriptionOrderItem.PriceInclTax;
            if (!decimal.TryParse(form["pvPriceExclTax" + subscriptionOrderItemId], out priceExclTax))
                priceExclTax = subscriptionOrderItem.PriceExclTax;

            if (quantity > 0)
            {
                int qtyDifference = subscriptionOrderItem.Quantity - quantity;

                subscriptionOrderItem.UnitPriceInclTax = unitPriceInclTax;
                subscriptionOrderItem.UnitPriceExclTax = unitPriceExclTax;
                subscriptionOrderItem.Quantity = quantity;
                subscriptionOrderItem.DiscountAmountInclTax = discountInclTax;
                subscriptionOrderItem.DiscountAmountExclTax = discountExclTax;
                subscriptionOrderItem.PriceInclTax = priceInclTax;
                subscriptionOrderItem.PriceExclTax = priceExclTax;
                _subscriptionService.UpdateSubscriptionOrder(order);

                //adjust inventory
                _planService.AdjustInventory(subscriptionOrderItem.Plan, qtyDifference, subscriptionOrderItem.AttributesXml);

            }
            else
            {
                //adjust inventory
                _planService.AdjustInventory(subscriptionOrderItem.Plan, subscriptionOrderItem.Quantity, subscriptionOrderItem.AttributesXml);

                //delete item
                _subscriptionService.DeleteSubscriptionOrderItem(subscriptionOrderItem);
            }

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Subscription item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            var model = new SubscriptionOrderModel();
            PrepareSubscriptionDetailsModel(model, order);

            //selected tab
            SaveSelectedTabIndex(persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteSubscriptionItem")]
        [ValidateInput(false)]
        public ActionResult DeleteSubscriptionItem(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            //get order item identifier
            int subscriptionOrderItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnDeleteSubscriptionItem", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionOrderItemId = Convert.ToInt32(formValue.Substring("btnDeleteSubscriptionItem".Length));

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == subscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            if (_giftCardService.GetGiftCardsByPurchasedWithSubscriptionOrderItemId(subscriptionOrderItem.Id).Count > 0)
            {
                //we cannot delete an order item with associated gift cards
                //a store owner should delete them first

                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);

                ErrorNotification("This order item has an associated gift card record. Please delete it first.", false);

                //selected tab
                SaveSelectedTabIndex(persistForTheNextRequest: false);

                return View(model);

            }
            else
            {
                //adjust inventory
                _planService.AdjustInventory(subscriptionOrderItem.Plan, subscriptionOrderItem.Quantity, subscriptionOrderItem.AttributesXml);

                //delete item
                _subscriptionService.DeleteSubscriptionOrderItem(subscriptionOrderItem);

                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = "Subscription item has been deleted",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);


                var model = new SubscriptionOrderModel();
                PrepareSubscriptionDetailsModel(model, order);

                //selected tab
                SaveSelectedTabIndex(persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        [ValidateInput(false)]
        public ActionResult ResetDownloadCount(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //get order item identifier
            int subscriptionOrderItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionOrderItemId = Convert.ToInt32(formValue.Substring("btnResetDownloadCount".Length));

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == subscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his plans 
            if (_workContext.CurrentVendor != null && !HasAccessToSubscriptionItem(subscriptionOrderItem))
                return RedirectToAction("List");

            subscriptionOrderItem.DownloadCount = 0;
            _subscriptionService.UpdateSubscriptionOrder(order);

            var model = new SubscriptionOrderModel();
            PrepareSubscriptionDetailsModel(model, order);

            //selected tab
            SaveSelectedTabIndex(persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        [ValidateInput(false)]
        public ActionResult ActivateDownloadItem(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //get order item identifier
            int subscriptionOrderItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionOrderItemId = Convert.ToInt32(formValue.Substring("btnPvActivateDownload".Length));

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == subscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his plans 
            if (_workContext.CurrentVendor != null && !HasAccessToSubscriptionItem(subscriptionOrderItem))
                return RedirectToAction("List");

            subscriptionOrderItem.IsDownloadActivated = !subscriptionOrderItem.IsDownloadActivated;
            _subscriptionService.UpdateSubscriptionOrder(order);

            var model = new SubscriptionOrderModel();
            PrepareSubscriptionDetailsModel(model, order);

            //selected tab
            SaveSelectedTabIndex(persistForTheNextRequest: false);

            return View(model);
        }

        public ActionResult UploadLicenseFilePopup(int id, int subscriptionOrderItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == subscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            if (!subscriptionOrderItem.Plan.IsDownload)
                throw new ArgumentException("Plan is not downloadable");

            //ensure a vendor has access only to his plans 
            if (_workContext.CurrentVendor != null && !HasAccessToSubscriptionItem(subscriptionOrderItem))
                return RedirectToAction("List");

            var model = new SubscriptionOrderModel.UploadLicenseModel
            {
                LicenseDownloadId = subscriptionOrderItem.LicenseDownloadId.HasValue ? subscriptionOrderItem.LicenseDownloadId.Value : 0,
                SubscriptionOrderId = order.Id,
                SubscriptionOrderItemId = subscriptionOrderItem.Id
            };

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("uploadlicense")]
        public ActionResult UploadLicenseFilePopup(string btnId, string formId, SubscriptionOrderModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(model.SubscriptionOrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == model.SubscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his plans 
            if (_workContext.CurrentVendor != null && !HasAccessToSubscriptionItem(subscriptionOrderItem))
                return RedirectToAction("List");

            //attach license
            if (model.LicenseDownloadId > 0)
                subscriptionOrderItem.LicenseDownloadId = model.LicenseDownloadId;
            else
                subscriptionOrderItem.LicenseDownloadId = null;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        [HttpPost, ActionName("UploadLicenseFilePopup")]
        [FormValueRequired("deletelicense")]
        public ActionResult DeleteLicenseFilePopup(string btnId, string formId, SubscriptionOrderModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(model.SubscriptionOrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var subscriptionOrderItem = order.SubscriptionOrderItems.FirstOrDefault(x => x.Id == model.SubscriptionOrderItemId);
            if (subscriptionOrderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his plans 
            if (_workContext.CurrentVendor != null && !HasAccessToSubscriptionItem(subscriptionOrderItem))
                return RedirectToAction("List");

            //attach license
            subscriptionOrderItem.LicenseDownloadId = null;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        public ActionResult AddPlanToSubscription(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = orderId });

            var model = new SubscriptionOrderModel.AddSubscriptionOrderModel();
            model.SubscriptionOrderId = orderId;
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //plan types
            model.AvailablePlanTypes = PlanType.SimplePlan.ToSelectList(false).ToList();
            model.AvailablePlanTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPlanToSubscription(DataSourceRequest command, SubscriptionOrderModel.AddSubscriptionOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Content("");

            var gridModel = new DataSourceResult();
            var plans = _planService.SearchPlans(categoryIds: new List<int> {model.SearchCategoryId},
                manufacturerId: model.SearchManufacturerId,
                planType: model.SearchPlanTypeId > 0 ? (PlanType?)model.SearchPlanTypeId : null,
                keywords: model.SearchPlanName, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize,
                showHidden: true);
            gridModel.Data = plans.Select(x =>
            {
                var planModel = new SubscriptionOrderModel.AddSubscriptionOrderModel.PlanModel
                {
                    Id = x.Id,
                    Name =  x.Name,
                    Sku = x.Sku,
                };

                return planModel;
            });
            gridModel.Total = plans.TotalCount;

            return Json(gridModel);
        }

        public ActionResult AddPlanToSubscriptionDetails(int orderId, int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = orderId });

            var model = PrepareAddPlanToSubscriptionOrderModel(orderId, planId);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPlanToSubscriptionDetails(int orderId, int planId, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = orderId });

            var order = _subscriptionService.GetOrderById(orderId);
            var plan = _planService.GetPlanById(planId);
            //save order item

            //basic properties
            decimal unitPriceInclTax;
            decimal.TryParse(form["UnitPriceInclTax"], out unitPriceInclTax);
            decimal unitPriceExclTax;
            decimal.TryParse(form["UnitPriceExclTax"], out unitPriceExclTax);
            int quantity;
            int.TryParse(form["Quantity"], out quantity);
            decimal priceInclTax;
            decimal.TryParse(form["SubTotalInclTax"], out priceInclTax);
            decimal priceExclTax;
            decimal.TryParse(form["SubTotalExclTax"], out priceExclTax);

            //attributes
            //warnings
            var warnings = new List<string>();
            string attributesXml = "";

             

            #region Gift cards

            string recipientName = "";
            string recipientEmail = "";
            string senderName = "";
            string senderEmail = "";
            string giftCardMessage = "";
            if (plan.IsGiftCard)
            {
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals("giftcard.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.SenderName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.Message", StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                
            }

            #endregion

            #region Rental plan

            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (plan.IsRental)
            {
                var ctrlStartDate = form["rental_start_date"];
                var ctrlEndDate = form["rental_end_date"];
                try
                {
                    //currenly we support only this format (as in the \Views\Subscription\_PlanAddRentalInfo.cshtml file)
                    const string datePickerFormat = "MM/dd/yyyy";
                    rentalStartDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                    rentalEndDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
                }
                catch
                {
                }
            }

            #endregion

            //warnings
            warnings.AddRange(_subscriptionCartService.GetSubscriptionCartItemAttributeWarnings(order.Customer, SubscriptionCartType.SubscriptionCart, plan, quantity, attributesXml));
            warnings.AddRange(_subscriptionCartService.GetSubscriptionCartItemGiftCardWarnings(SubscriptionCartType.SubscriptionCart, plan, attributesXml));
            warnings.AddRange(_subscriptionCartService.GetRentalPlanWarnings(plan, rentalStartDate, rentalEndDate));
            if (warnings.Count == 0)
            {
                //no errors

                //attributes
                string attributeDescription = _planAttributeFormatter.FormatAttributes(plan, attributesXml, order.Customer);

                //save item
                var subscriptionOrderItem = new SubscriptionOrderItem
                {
                    SubscriptionOrderItemGuid = Guid.NewGuid(),
                    SubscriptionOrder = order,
                    PlanId = plan.Id,
                    UnitPriceInclTax = unitPriceInclTax,
                    UnitPriceExclTax = unitPriceExclTax,
                    PriceInclTax = priceInclTax,
                    PriceExclTax = priceExclTax,
                    OriginalPlanCost = _priceCalculationService.GetProductCost(plan, attributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = attributesXml,
                    Quantity = quantity,
                    DiscountAmountInclTax = decimal.Zero,
                    DiscountAmountExclTax = decimal.Zero,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate
                };
                order.SubscriptionOrderItems.Add(subscriptionOrderItem);
                _subscriptionService.UpdateSubscriptionOrder(order);

                //adjust inventory
                _planService.AdjustInventory(subscriptionOrderItem.Plan, -subscriptionOrderItem.Quantity, subscriptionOrderItem.AttributesXml);

                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = "A new order item has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                //gift cards
                if (plan.IsGiftCard)
                {
                    for (int i = 0; i < subscriptionOrderItem.Quantity; i++)
                    {
                        var gc = new GiftCard
                        {
                            GiftCardType = plan.GiftCardType,
                            PurchasedWithSubscriptionOrderItem = subscriptionOrderItem,
                            Amount = unitPriceExclTax,
                            IsGiftCardActivated = false,
                            GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                            RecipientName = recipientName,
                            RecipientEmail = recipientEmail,
                            SenderName = senderName,
                            SenderEmail = senderEmail,
                            Message = giftCardMessage,
                            IsRecipientNotified = false,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        _giftCardService.InsertGiftCard(gc);
                    }
                }

                //redirect to order details page
                return RedirectToAction("Edit", "Subscription", new { id = order.Id });
            }
            
            //errors
            var model = PrepareAddPlanToSubscriptionOrderModel(order.Id, plan.Id);
            model.Warnings.AddRange(warnings);
            return View(model);
        }

        #endregion

        #endregion

        #region Addresses

        public ActionResult AddressEdit(int addressId, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = orderId });

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            var model = new SubscriptionOrderAddressModel();
            model.SubscriptionOrderId = orderId;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;

            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddressEdit(SubscriptionOrderAddressModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(model.SubscriptionOrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = order.Id });

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                throw new ArgumentException("No address found with the specified id");

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = "Address has been edited",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.SubscriptionOrderId });
            }

            //If we got this far, something failed, redisplay form
            model.SubscriptionOrderId = order.Id;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);

            return View(model);
        }

        #endregion
        
        #region Shipments

        public ActionResult ShipmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var model = new ShipmentListModel();
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var w in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View(model);
		}

		[HttpPost]
        public ActionResult ShipmentListSelect(DataSourceRequest command, ShipmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his plans
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var shipments = _shipmentService.GetAllShipments(vendorId: vendorId,
                warehouseId: model.WarehouseId, 
                shippingCountryId: model.CountryId, 
                shippingStateId: model.StateProvinceId, 
                shippingCity: model.City,
                trackingNumber: model.TrackingNumber, 
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue, 
                createdToUtc: endDateValue, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = shipments.Select(shipment => PrepareShipmentModel(shipment, false)),
                Total = shipments.TotalCount
            };
			return new JsonResult
			{
				Data = gridModel
			};
		}

        [HttpPost]
        public ActionResult ShipmentsItemsByShipmentId(int shipmentId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                throw new ArgumentException("No shipment found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return Content("");

            var order = _subscriptionService.GetOrderById(shipment.OrderItem.SubscriptionOrderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
           

            //shipments
            var shipmentModel = PrepareShipmentModel(shipment, true);
            var gridModel = new DataSourceResult
            {
                Data = shipmentModel.Items,
                Total = shipmentModel.Items.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ShipmentsBySubscription(int orderId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToSubscription(order))
                return Content("");

            //shipments
            var shipmentModels = new List<ShipmentModel>();
             var orderItems1 = order.OrderItems;
             foreach (var orderItem in orderItems1)
             {
                 var shipments = orderItem.Shipments
                     //a vendor should have access only to his plans
                     .Where(s => _workContext.CurrentVendor == null || HasAccessToShipment(s))
                     .OrderBy(s => s.CreatedOnUtc)
                     .ToList();
                 foreach (var shipment in shipments)
                     shipmentModels.Add(PrepareShipmentModel(shipment, false));
             }
            var gridModel = new DataSourceResult
            {
                Data = shipmentModels,
                Total = shipmentModels.Count
            };


            return Json(gridModel);
        }

        public ActionResult AddShipment(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToSubscription(order))
                return RedirectToAction("List");

            var model = new ShipmentModel
            {
                SubscriptionOrderId = order.Id,
            };

            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var orderItems = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderItems = orderItems.ToList();
            }

            foreach (var orderItem in orderItems)
            {
                foreach (var itemDetail in orderItem.ItemDetails)
                {
                    //we can ship only shippable products
                    //if (!itemDetail.Product.IsShipEnabled)
                    //    continue;

                    //quantities
                    var qtyInThisShipment = 0;
                    var maxQtyToAdd = itemDetail.GetTotalNumberOfItemsCanBeAddedToShipment();
                    var qtyOrdered = itemDetail.Quantity;
                    var qtyInAllShipments = itemDetail.GetTotalNumberOfItemsInAllShipment();

                    //ensure that this product can be added to a shipment
                    //if (maxQtyToAdd <= 0)
                    //    continue;

                    var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                    {
                        ItemDetailId = itemDetail.Id,
                        OrderItemId = orderItem.Id,
                        ProductId = itemDetail.ProductId,
                        ProductName = itemDetail.Product.Name,
                        Sku = itemDetail.Product.FormatSku(itemDetail.AttributesXml, _productAttributeParser),
                        AttributeInfo = itemDetail.AttributeDescription,
                        ShipSeparately = itemDetail.Product.ShipSeparately,
                        ItemWeight = itemDetail.ItemWeight.HasValue ? string.Format("{0:F2} [{1}]", itemDetail.ItemWeight, baseWeightIn) : "",
                        ItemDimensions = string.Format("{0:F2} x {1:F2} x {2:F2} [{3}]", itemDetail.Product.Length, itemDetail.Product.Width, itemDetail.Product.Height, baseDimensionIn),
                        QuantityOrdered = qtyOrdered,
                        QuantityInThisShipment = qtyInThisShipment,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd,
                    };
                    //rental info
                    //if (itemDetail.Product.IsRental)
                    //{
                    //    var rentalStartDate = itemDetail.RentalStartDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(itemDetail.RentalStartDateUtc.Value) : "";
                    //    var rentalEndDate = itemDetail.RentalEndDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(itemDetail.RentalEndDateUtc.Value) : "";
                    //    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                    //        rentalStartDate, rentalEndDate);
                    //}

                    if (itemDetail.Product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                        itemDetail.Product.UseMultipleWarehouses)
                    {
                        //multiple warehouses supported
                        shipmentItemModel.AllowToChooseWarehouse = true;
                        foreach (var pwi in itemDetail.Product.ProductWarehouseInventory
                            .OrderBy(w => w.WarehouseId).ToList())
                        {
                            var warehouse = pwi.Warehouse;
                            if (warehouse != null)
                            {
                                shipmentItemModel.AvailableWarehouses.Add(new ShipmentModel.ShipmentItemModel.WarehouseInfo
                                {
                                    WarehouseId = warehouse.Id,
                                    WarehouseName = warehouse.Name,
                                    StockQuantity = pwi.StockQuantity,
                                    ReservedQuantity = pwi.ReservedQuantity,
                                    PlannedQuantity = _shipmentService.GetQuantityInShipments(itemDetail.Product, warehouse.Id, true, true)
                                });
                            }
                        }
                    }
                    else
                    {
                        //multiple warehouses are not supported
                        var warehouse = _shippingService.GetWarehouseById(itemDetail.Product.WarehouseId);
                        if (warehouse != null)
                        {
                            shipmentItemModel.AvailableWarehouses.Add(new ShipmentModel.ShipmentItemModel.WarehouseInfo
                            {
                                WarehouseId = warehouse.Id,
                                WarehouseName = warehouse.Name,
                                StockQuantity = itemDetail.Product.StockQuantity
                            });
                        }
                    }

                    model.Items.Add(shipmentItemModel);
                }
            }
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult AddShipment(int orderId, FormCollection form, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToSubscription(order))
                return RedirectToAction("List");

            var orderItems = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderItems = orderItems.ToList();
            }

            Shipment shipment = null;
            decimal? totalWeight = null;
            foreach (var orderItem in orderItems)
            {
                foreach (var itemDetail in orderItem.ItemDetails)
                {
                    ////is shippable
                    //if (!itemDetail.Product.IsShipEnabled)
                    //    continue;

                    ////ensure that this product can be shipped (have at least one item to ship)
                    //var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                    //if (maxQtyToAdd <= 0)
                    //    continue;

                    int qtyToAdd = 0; //parse quantity
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("qtyToAdd{0}", itemDetail.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out qtyToAdd);
                            break;
                        }

                    int warehouseId = 0;
                    if (itemDetail.Product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                        itemDetail.Product.UseMultipleWarehouses)
                    {
                        //multiple warehouses supported
                        //warehouse is chosen by a store owner
                        foreach (string formKey in form.AllKeys)
                            if (formKey.Equals(string.Format("warehouse_{0}", orderItem.Id), StringComparison.InvariantCultureIgnoreCase))
                            {
                                int.TryParse(form[formKey], out warehouseId);
                                break;
                            }
                    }
                    else
                    {
                        //multiple warehouses are not supported
                        warehouseId = itemDetail.Product.WarehouseId;
                    }

                    //foreach (string formKey in form.AllKeys)
                    //    if (formKey.Equals(string.Format("qtyToAdd{0}", orderItem.Id), StringComparison.InvariantCultureIgnoreCase))
                    //    {
                    //        int.TryParse(form[formKey], out qtyToAdd);
                    //        break;
                    //    }

                    //validate quantity
                    if (qtyToAdd <= 0)
                        continue;
                    //if (qtyToAdd > maxQtyToAdd)
                    //    qtyToAdd = maxQtyToAdd;

                    //ok. we have at least one item. let's create a shipment (if it does not exist)

                    var orderItemTotalWeight = itemDetail.ItemWeight.HasValue ? itemDetail.ItemWeight * qtyToAdd : null;
                    if (orderItemTotalWeight.HasValue)
                    {
                        if (!totalWeight.HasValue)
                            totalWeight = 0;
                        totalWeight += orderItemTotalWeight.Value;
                    }
                    if (shipment == null)
                    {
                        var trackingNumber = form["TrackingNumber"];
                        var adminComment = form["AdminComment"];
                        shipment = new Shipment
                        {
                            OrderItemId = itemDetail.OrderItemId,
                            TrackingNumber = trackingNumber,
                            TotalWeight = null,
                            ShippedDateUtc = null,
                            DeliveryDateUtc = null,
                            AdminComment = adminComment,
                            CreatedOnUtc = DateTime.UtcNow,
                        };
                    }
                    //create a shipment item
                    var shipmentItem = new ShipmentItem
                    {
                        ItemDetailId = itemDetail.Id,
                        Quantity = qtyToAdd,
                        WarehouseId = warehouseId
                    };
                    shipment.ShipmentItems.Add(shipmentItem);
                }

                //if we have at least one item in the shipment, then save it
                if (shipment != null && shipment.ShipmentItems.Count > 0)
                {
                    shipment.TotalWeight = totalWeight;
                    _shipmentService.InsertShipment(shipment);

                    //add a note
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = "A shipment has been added",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Added"));
                    return continueEditing
                               ? RedirectToAction("ShipmentDetails", new { id = shipment.Id })
                               : RedirectToAction("Edit", new { id = orderId });
                }
            }
            ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoProductsSelected"));
            return RedirectToAction("AddShipment", new { orderId = orderId });
        }
       
        public ActionResult ShipmentDetails(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            var model = PrepareShipmentModel(shipment, true, true);
            return View(model);
        }

     
        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("settrackingnumber")]
        public ActionResult SetTrackingNumber(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            shipment.TrackingNumber = model.TrackingNumber;
            _shipmentService.UpdateShipment(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setadmincomment")]
        public ActionResult SetShipmentAdminComment(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            shipment.AdminComment = model.AdminComment;
            _shipmentService.UpdateShipment(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasshipped")]
        public ActionResult SetAsShipped(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                _orderProcessingService.Ship(shipment, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("saveshippeddate")]
        public ActionResult EditShippedDate(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.ShippedDateUtc.HasValue)
                {
                    throw new Exception("Enter shipped date");
                }
                shipment.ShippedDateUtc = model.ShippedDateUtc;
                _shipmentService.UpdateShipment(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasdelivered")]
        public ActionResult SetAsDelivered(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                _orderProcessingService.Deliver(shipment, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("savedeliverydate")]
        public ActionResult EditDeliveryDate(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.DeliveryDateUtc.HasValue)
                {
                    throw new Exception("Enter delivery date");
                }
                shipment.DeliveryDateUtc = model.DeliveryDateUtc;
                _shipmentService.UpdateShipment(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        public ActionResult PdfPackagingSlip(int shipmentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                //no shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            var shipments = new List<Shipment>();
            shipments.Add(shipment);
            
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", string.Format("packagingslip_{0}.pdf", shipment.Id));
        }

        [HttpPost, ActionName("ShipmentList")]
        [FormValueRequired("exportpackagingslips-all")]
        public ActionResult PdfPackagingSlipAll(ShipmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his plans
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var shipments = _shipmentService.GetAllShipments(vendorId: vendorId,
                warehouseId: model.WarehouseId,
                shippingCountryId: model.CountryId,
                shippingStateId: model.StateProvinceId,
                shippingCity: model.City,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue);

            //ensure that we at least one shipment selected
            if (shipments.Count == 0)
            {
                ErrorNotification(_localizationService.GetResource("Admin.SubscriptionOrders.Shipments.NoShipmentsSelected"));
                return RedirectToAction("ShipmentList");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "packagingslips.pdf");
        }

        [HttpPost]
        public ActionResult PdfPackagingSlipSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                shipments.AddRange(_shipmentService.GetShipmentsByIds(ids));
            }
            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }

            //ensure that we at least one shipment selected
            if (shipments.Count == 0)
            {
                ErrorNotification(_localizationService.GetResource("Admin.SubscriptionOrders.Shipments.NoShipmentsSelected"));
                return RedirectToAction("ShipmentList");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "packagingslips.pdf");
        }

        
        #endregion

        #region Pending Shipment
        public ActionResult BorrowTransactionList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var model = new BorrowTransactionListModel();
            //countries
          
            return View(model);
        }

        [HttpPost]
        public ActionResult BorrowTransactionList(DataSourceRequest command, BorrowTransactionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his plans
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            ////load shipments
            //DateTime? startDateValue = (model.StartDate == null) ? null
            //               : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            //DateTime? endDateValue = (model.EndDate == null) ? null
            //                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;

            
            //load orders
            var orders = _subscriptionService.SearchSubscriptionOrders(
                //storeId: model.StoreId,
                //vendorId: model.VendorId,
                //planId: filterByPlanId,
                //warehouseId: model.WarehouseId,
                //paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                //billingEmail: model.BillingEmail,
                //billingLastName: model.BillingLastName,
                //billingCountryId: model.BillingCountryId,
                //orderNotes: model.SubscriptionOrderNotes,
                //orderGuid: model.SubscriptionOrderGuid,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            List<OrderItem> orderItems = new List<OrderItem>();
            foreach(var order in orders){
                foreach (var orderitem in order.OrderItems)
                {
                    orderItems.Add(orderitem);
                }
            }

            var gridModel = new DataSourceResult
            {
                Data = orderItems.Select(x => new BorrowTransactionModel
                {
                    OrderItemId = x.Id,
                    SubscriptionOrderId = x.SubscriptionOrderId,
                    SubscriptionOrderStatus = x.SubscriptionOrder.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext),

                    PaymentStatus = x.SubscriptionOrder.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                    CustomerEmail =x.SubscriptionOrder.Customer.Email,
                    CreatedOn = x.CreatedOnUtc,
                    TotalItems = x.ItemDetails.ToList().Count()
                }),
                Total = orderItems.Count()
            };


            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult GetBorrowTransactionDetails(int orderItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var orderItem = _subscriptionService.GetOrderItemById(orderItemId);

            var cart = orderItem.ItemDetails.ToList();
           

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    var maxQtyToAdd = sci.GetTotalNumberOfItemsCanBeAddedToShipment();
                    var qtyOrdered = sci.Quantity;
                    var qtyInAllShipments = sci.GetTotalNumberOfItemsInAllShipment();

                    var sciModel = new BorrowTransactionItemModel
                    {
                        Id = sci.Id,
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        QuantityOrdered = qtyOrdered,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd,
                        ProductName = sci.Product.Name,
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }

        #endregion

        #region Subscription notes

        [HttpPost]
        public ActionResult SubscriptionOrderNotesSelect(int orderId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Content("");

            //order notes
            var orderNoteModels = new List<SubscriptionOrderModel.SubscriptionOrderNote>();
            foreach (var orderNote in order.SubscriptionOrderNotes
                .OrderByDescending(on => on.CreatedOnUtc))
            {
                var download = _downloadService.GetDownloadById(orderNote.DownloadId);
                orderNoteModels.Add(new SubscriptionOrderModel.SubscriptionOrderNote
                {
                    Id = orderNote.Id,
                    SubscriptionOrderId = orderNote.SubscriptionOrderId,
                    DownloadId = orderNote.DownloadId,
                    DownloadGuid = download != null ? download.DownloadGuid : Guid.Empty,
                    DisplayToCustomer = orderNote.DisplayToCustomer,
                    Note = orderNote.FormatSubscriptionOrderNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = orderNoteModels,
                Total = orderNoteModels.Count
            };

            return Json(gridModel);
        }
        
        [ValidateInput(false)]
        public ActionResult SubscriptionOrderNoteAdd(int orderId, int downloadId, bool displayToCustomer, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var orderNote = new SubscriptionOrderNote
            {
                DisplayToCustomer = displayToCustomer,
                Note = message,
                DownloadId = downloadId,
                CreatedOnUtc = DateTime.UtcNow,
            };
            order.SubscriptionOrderNotes.Add(orderNote);
            _subscriptionService.UpdateSubscriptionOrder(order);

            //new order notification
            if (displayToCustomer)
            {
                //email
                _workflowMessageService.SendNewSubscriptionOrderNoteAddedCustomerNotification(
                    orderNote, _workContext.WorkingLanguage.Id);

            }

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubscriptionOrderNoteDelete(int id, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Subscription", new { id = orderId });

            var orderNote = order.SubscriptionOrderNotes.FirstOrDefault(on => on.Id == id);
            if (orderNote == null)
                throw new ArgumentException("No order note found with the specified id");
            _subscriptionService.DeleteSubscriptionOrderNote(orderNote);

            return new NullJsonResult();
        }

        #endregion

        #region Reports

        [NonAction]
        protected DataSourceResult GetBestsellersBriefReportModel(int pageIndex,
            int pageSize, int orderBy)
        {
            //a vendor should have access only to his plans
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            var items = _orderReportService.BestSellersReport(
                vendorId : vendorId,
                orderBy: orderBy,
                pageIndex: pageIndex,
                pageSize: pageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel
                    {
                        PlanId = x.PlanId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var plan = _planService.GetPlanById(x.PlanId);
                    if (plan != null)
                        m.PlanName = plan.Name;
                    return m;
                }),
                Total = items.TotalCount
            };
            return gridModel;
        }
        public ActionResult BestsellersBriefReportByQuantity()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public ActionResult BestsellersBriefReportByQuantityList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 1);

            return Json(gridModel);
        }
        public ActionResult BestsellersBriefReportByAmount()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public ActionResult BestsellersBriefReportByAmountList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 2);

            return Json(gridModel);
        }



        public ActionResult BestsellersReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var model = new BestsellersReportModel();
            //vendor
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //order statuses
            model.AvailableSubscriptionOrderStatuses = SubscriptionOrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableSubscriptionOrderStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = _vendorService.GetAllVendors(showHidden: true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            return View(model);
        }
        [HttpPost]
        public ActionResult BestsellersReportList(DataSourceRequest command, BestsellersReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;

            var items = _orderReportService.BestSellersReport(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: model.BillingCountryId,
                orderBy: 2,
                vendorId: model.VendorId,
                categoryId: model.CategoryId,
                manufacturerId: model.ManufacturerId,
                storeId: model.StoreId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel
                    {
                        PlanId = x.PlanId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var plan = _planService.GetPlanById(x.PlanId);
                    if (plan!= null)
                        m.PlanName = plan.Name;
                    return m;
                }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }



        public ActionResult NeverSoldReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            var model = new NeverSoldReportModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult NeverSoldReportList(DataSourceRequest command, NeverSoldReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            //a vendor should have access only to his plans
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            
            var items = _orderReportService.PlansNeverSold(vendorId,
                startDateValue, endDateValue,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                    new NeverSoldReportLineModel
                    {
                        PlanId = x.Id,
                        PlanName = x.Name,
                    }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }


        [ChildActionOnly]
        public ActionResult SubscriptionAverageReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public ActionResult SubscriptionAverageReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            //a vendor does have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");


            var report = new List<SubscriptionOrderAverageReportLineSummary>();
            report.Add(_orderReportService.SubscriptionOrderAverageReport(0, SubscriptionOrderStatus.Pending));
            report.Add(_orderReportService.SubscriptionOrderAverageReport(0, SubscriptionOrderStatus.Processing));
            report.Add(_orderReportService.SubscriptionOrderAverageReport(0, SubscriptionOrderStatus.Complete));
            report.Add(_orderReportService.SubscriptionOrderAverageReport(0, SubscriptionOrderStatus.Cancelled));
            var model = report.Select(x => new SubscriptionOrderAverageReportLineSummaryModel
            {
                SubscriptionOrderStatus = x.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                SumTodaySubscriptionOrders = _priceFormatter.FormatPrice(x.SumTodaySubscriptionOrders, true, false),
                SumThisWeekSubscriptionOrders = _priceFormatter.FormatPrice(x.SumThisWeekSubscriptionOrders, true, false),
                SumThisMonthSubscriptionOrders = _priceFormatter.FormatPrice(x.SumThisMonthSubscriptionOrders, true, false),
                SumThisYearSubscriptionOrders = _priceFormatter.FormatPrice(x.SumThisYearSubscriptionOrders, true, false),
                SumAllTimeSubscriptionOrders = _priceFormatter.FormatPrice(x.SumAllTimeSubscriptionOrders, true, false),
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }

        [ChildActionOnly]
        public ActionResult SubscriptionIncompleteReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public ActionResult SubscriptionIncompleteReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return Content("");
            
            //a vendor does have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = new List<SubscriptionOrderIncompleteReportLineModel>();
            //not paid
            var psPending = _orderReportService.GetSubscriptionOrderAverageReportLine(ps: PaymentStatus.Pending, ignoreCancelledSubscriptionOrders: true);
            model.Add(new SubscriptionOrderIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalUnpaidSubscriptions"),
                Count = psPending.CountSubscriptionOrders,
                Total = _priceFormatter.FormatPrice(psPending.SumSubscriptionOrders, true, false),
                ViewLink = Url.Action("List", "Subscription", new { paymentStatusId = ((int)PaymentStatus.Pending).ToString() })
            });
            //not shipped
            var ssPending = _orderReportService.GetSubscriptionOrderAverageReportLine(ss: ShippingStatus.NotYetShipped, ignoreCancelledSubscriptionOrders: true);
            model.Add(new SubscriptionOrderIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalNotShippedSubscriptions"),
                Count = ssPending.CountSubscriptionOrders,
                Total = _priceFormatter.FormatPrice(ssPending.SumSubscriptionOrders, true, false),
                ViewLink = Url.Action("List", "Subscription", new { shippingStatusId = ((int)ShippingStatus.NotYetShipped).ToString() })
            });
            //pending
            var osPending = _orderReportService.GetSubscriptionOrderAverageReportLine(os: SubscriptionOrderStatus.Pending, ignoreCancelledSubscriptionOrders: true);
            model.Add(new SubscriptionOrderIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalIncompleteSubscriptions"),
                Count = osPending.CountSubscriptionOrders,
                Total = _priceFormatter.FormatPrice(osPending.SumSubscriptionOrders, true, false),
                ViewLink = Url.Action("List", "Subscription", new { orderStatusId = ((int)SubscriptionOrderStatus.Pending).ToString() })
            });

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }


 
       


        #endregion
    }
}
