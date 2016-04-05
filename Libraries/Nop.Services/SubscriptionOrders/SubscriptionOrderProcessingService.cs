using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// SubscriptionOrder processing service
    /// </summary>
    public partial class SubscriptionOrderProcessingService : ISubscriptionOrderProcessingService
    {
        #region Fields
        
        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly ISubscriptionOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly ISubscriptionCartService _subscriptionCartService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ITaxService _taxService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IEncryptionService _encryptionService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly IAffiliateService _affiliateService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPdfService _pdfService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IGenericAttributeService _genericAttributeService;

        private readonly ShippingSettings _shippingSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly SubscriptionOrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionService">SubscriptionOrder service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="productService">Product service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="orderTotalCalculationService">SubscriptionOrder total calculationservice</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="productAttributeFormatter">Product attribute formatter</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="subscriptionCartService">Shopping cart service</param>
        /// <param name="checkoutAttributeFormatter">Checkout attribute service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shipmentService">Shipment service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="vendorService">Vendor service</param>
        /// <param name="customerActivityService">Customer activity service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="affiliateService">Affiliate service</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="pdfService">PDF service</param>
        /// <param name="rewardPointService">Reward point service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="orderSettings">SubscriptionOrder settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="currencySettings">Currency settings</param>
        public SubscriptionOrderProcessingService(ISubscriptionOrderService subscriptionService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IProductService productService,
            IPaymentService paymentService,
            ILogger logger,
            ISubscriptionOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter,
            IGiftCardService giftCardService,
            ISubscriptionCartService subscriptionCartService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ITaxService taxService,
            ICustomerService customerService,
            IDiscountService discountService,
            IEncryptionService encryptionService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            ICurrencyService currencyService,
            IAffiliateService affiliateService,
            IEventPublisher eventPublisher,
            IPdfService pdfService,
            IRewardPointService rewardPointService,
            IGenericAttributeService genericAttributeService,
            ShippingSettings shippingSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            SubscriptionOrderSettings orderSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings)
        {
            this._subscriptionService = subscriptionService;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._productService = productService;
            this._paymentService = paymentService;
            this._logger = logger;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._giftCardService = giftCardService;
            this._subscriptionCartService = subscriptionCartService;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._taxService = taxService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._encryptionService = encryptionService;
            this._customerActivityService = customerActivityService;
            this._currencyService = currencyService;
            this._affiliateService = affiliateService;
            this._eventPublisher = eventPublisher;
            this._pdfService = pdfService;
            this._rewardPointService = rewardPointService;
            this._genericAttributeService = genericAttributeService;

            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._currencySettings = currencySettings;
        }

        #endregion

        #region Nested classes

        protected class PlaceSubscriptionOrderContainter
        {
            public PlaceSubscriptionOrderContainter()
            {
                this.Cart = new List<SubscriptionCartItem>();
                this.AppliedDiscounts = new List<Discount>();
                this.AppliedGiftCards = new List<AppliedGiftCard>();
            }

            public Customer Customer { get; set; }
            public Language CustomerLanguage { get; set; }
            public int AffiliateId { get; set; }
            public TaxDisplayType CustomerTaxDisplayType {get; set; }
            public string CustomerCurrencyCode { get; set; }
            public decimal CustomerCurrencyRate { get; set; }

            public Address BillingAddress { get; set; }
            public Address ShippingAddress {get; set; }
            public ShippingStatus ShippingStatus { get; set; }
            public string ShippingMethodName { get; set; }
            public string ShippingRateComputationMethodSystemName { get; set; }
            public bool PickUpInStore { get; set; }

            public bool IsRecurringSubscriptionCart { get; set; }
            //initial order (used with recurring payments)
            public SubscriptionOrder InitialSubscriptionOrder { get; set; }

            public string CheckoutAttributeDescription { get; set; }
            public string CheckoutAttributesXml { get; set; }

            public IList<SubscriptionCartItem> Cart { get; set; }
            public List<Discount> AppliedDiscounts { get; set; }
            public List<AppliedGiftCard> AppliedGiftCards { get; set; }

            public decimal RegistrationCharge { get; set; }

            public decimal RegistrationChargeDiscount { get; set; }
            public decimal SecurityDeposit { get; set; }

            public decimal SubscriptionOrderSubTotalInclTax { get; set; }
            public decimal SubscriptionOrderSubTotalExclTax { get; set; }
            public decimal SubscriptionOrderSubTotalDiscountInclTax { get; set; }
            public decimal SubscriptionOrderSubTotalDiscountExclTax { get; set; }
            public decimal SubscriptionOrderShippingTotalInclTax { get; set; }
            public decimal SubscriptionOrderShippingTotalExclTax { get; set; }
            public decimal PaymentAdditionalFeeInclTax {get; set; }
            public decimal PaymentAdditionalFeeExclTax { get; set; }
            public decimal SubscriptionOrderTaxTotal  {get; set; }
            public string VatNumber {get; set; }
            public string TaxRates {get; set; }
            public decimal SubscriptionOrderDiscountAmount { get; set; }
            public int RedeemedRewardPoints { get; set; }
            public decimal RedeemedRewardPointsAmount { get; set; }
            public decimal SubscriptionOrderTotal { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare details to place an order. It also sets some properties to "processPaymentRequest"
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceSubscriptionOrderContainter PreparePlaceSubscriptionOrderDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceSubscriptionOrderContainter();

            //Recurring orders. Load initial order
            if (processPaymentRequest.IsRecurringPayment)
            {
                details.InitialSubscriptionOrder = _subscriptionService.GetOrderById(processPaymentRequest.InitialSubscriptionOrderId);
                if (details.InitialSubscriptionOrder == null)
                    throw new ArgumentException("Initial order is not set for recurring payment");

                processPaymentRequest.PaymentMethodSystemName = details.InitialSubscriptionOrder.PaymentMethodSystemName;
            }

            //customer
            details.Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //customer currency
            if (!processPaymentRequest.IsRecurringPayment)
            {
                var currencyTmp = _currencyService.GetCurrencyById(details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId, processPaymentRequest.StoreId));
                var customerCurrency = (currencyTmp != null && currencyTmp.Published) ? currencyTmp : _workContext.WorkingCurrency;
                details.CustomerCurrencyCode = customerCurrency.CurrencyCode;
                var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                details.CustomerCurrencyRate = customerCurrency.Rate / primaryStoreCurrency.Rate;
            }
            else
            {
                details.CustomerCurrencyCode = details.InitialSubscriptionOrder.CustomerCurrencyCode;
                details.CustomerCurrencyRate = details.InitialSubscriptionOrder.CurrencyRate;
            }

            //customer language
            if (!processPaymentRequest.IsRecurringPayment)
            {
                details.CustomerLanguage = _languageService.GetLanguageById(details.Customer.GetAttribute<int>(
                    SystemCustomerAttributeNames.LanguageId, processPaymentRequest.StoreId));
            }
            else
            {
                details.CustomerLanguage = _languageService.GetLanguageById(details.InitialSubscriptionOrder.CustomerLanguageId);
            }
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            //check whether customer is guest
            if (details.Customer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                throw new NopException("Anonymous checkout is not allowed");

            //billing address
            if (!processPaymentRequest.IsRecurringPayment)
            {
                if (details.Customer.BillingAddress == null)
                    throw new NopException("Billing address is not provided");

                if (!CommonHelper.IsValidEmail(details.Customer.BillingAddress.Email))
                    throw new NopException("Email is not valid");

                //clone billing address
                details.BillingAddress = (Address)details.Customer.BillingAddress.Clone();
                if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                    throw new NopException(string.Format("Country '{0}' is not allowed for billing", details.BillingAddress.Country.Name));
            }
            else
            {
                if (details.InitialSubscriptionOrder.BillingAddress == null)
                    throw new NopException("Billing address is not available");

                //clone billing address
                details.BillingAddress = (Address)details.InitialSubscriptionOrder.BillingAddress.Clone();
                if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                    throw new NopException(string.Format("Country '{0}' is not allowed for billing", details.BillingAddress.Country.Name));
            }

            //checkout attributes
            if (!processPaymentRequest.IsRecurringPayment)
            {
                details.CheckoutAttributesXml = details.Customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, processPaymentRequest.StoreId);
                details.CheckoutAttributeDescription = _checkoutAttributeFormatter.FormatAttributes(details.CheckoutAttributesXml, details.Customer);
            }
            else
            {
                details.CheckoutAttributesXml = details.InitialSubscriptionOrder.CheckoutAttributesXml;
                details.CheckoutAttributeDescription = details.InitialSubscriptionOrder.CheckoutAttributeDescription;
            }

            //load and validate customer shopping cart
            if (!processPaymentRequest.IsRecurringPayment)
            {
                //load shopping cart
                details.Cart = details.Customer.SubscriptionCartItems
                    .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                    .LimitPerStore(processPaymentRequest.StoreId)
                    .ToList();

                if (details.Cart.Count == 0)
                    throw new NopException("Cart is empty");

                //validate the entire shopping cart
                var warnings = _subscriptionCartService.GetSubscriptionCartWarnings(details.Cart,
                    details.CheckoutAttributesXml,
                    true);
                if (warnings.Count > 0)
                {
                    var warningsSb = new StringBuilder();
                    foreach (string warning in warnings)
                    {
                        warningsSb.Append(warning);
                        warningsSb.Append(";");
                    }
                    throw new NopException(warningsSb.ToString());
                }

                //validate individual cart items
                foreach (var sci in details.Cart)
                {
                    var sciWarnings = _subscriptionCartService.GetSubscriptionCartItemWarnings(details.Customer, sci.SubscriptionCartType,
                        sci.Plan, processPaymentRequest.StoreId, sci.AttributesXml,
                        sci.CustomerEnteredPrice, sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                        sci.Quantity, false);
                    if (sciWarnings.Count > 0)
                    {
                        var warningsSb = new StringBuilder();
                        foreach (string warning in sciWarnings)
                        {
                            warningsSb.Append(warning);
                            warningsSb.Append(";");
                        }
                        throw new NopException(warningsSb.ToString());
                    }
                }
            }

            //min totals validation
            if (!processPaymentRequest.IsRecurringPayment)
            {
                bool minSubscriptionOrderSubtotalAmountOk = ValidateMinSubscriptionOrderSubtotalAmount(details.Cart);
                if (!minSubscriptionOrderSubtotalAmountOk)
                {
                    decimal minSubscriptionOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinSubscriptionOrderSubtotalAmount, _workContext.WorkingCurrency);
                    throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinSubscriptionOrderSubtotalAmount"), _priceFormatter.FormatPrice(minSubscriptionOrderSubtotalAmount, true, false)));
                }
                bool minSubscriptionOrderTotalAmountOk = ValidateMinSubscriptionOrderTotalAmount(details.Cart);
                if (!minSubscriptionOrderTotalAmountOk)
                {
                    decimal minSubscriptionOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinSubscriptionOrderTotalAmount, _workContext.WorkingCurrency);
                    throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinSubscriptionOrderTotalAmount"), _priceFormatter.FormatPrice(minSubscriptionOrderTotalAmount, true, false)));
                }
            }

            //tax display type
            if (!processPaymentRequest.IsRecurringPayment)
            {
                if (_taxSettings.AllowCustomersToSelectTaxDisplayType)
                    details.CustomerTaxDisplayType = (TaxDisplayType)details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, processPaymentRequest.StoreId);
                else
                    details.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;
            }
            else
            {
                details.CustomerTaxDisplayType = details.InitialSubscriptionOrder.CustomerTaxDisplayType;
            }
            
            //sub total
            if (!processPaymentRequest.IsRecurringPayment)
            {
                //sub total (incl tax)
                decimal orderSubTotalDiscountAmount1;
                Discount orderSubTotalAppliedDiscount1;
                decimal subTotalWithoutDiscountBase1;
                decimal subTotalWithDiscountBase1;
                decimal subTotalRegistrationAmount1;
                decimal subTotalDepositAmount1;
                _orderTotalCalculationService.GetSubscriptionCartSubTotal(details.Cart,
                    true, out subTotalRegistrationAmount1, out subTotalDepositAmount1, 
                    out orderSubTotalDiscountAmount1, out orderSubTotalAppliedDiscount1,
                    out subTotalWithoutDiscountBase1, out subTotalWithDiscountBase1);
                details.SubscriptionOrderSubTotalInclTax = subTotalWithoutDiscountBase1;
                details.SubscriptionOrderSubTotalDiscountInclTax = orderSubTotalDiscountAmount1;

                //discount history
                if (orderSubTotalAppliedDiscount1 != null && !details.AppliedDiscounts.ContainsDiscount(orderSubTotalAppliedDiscount1))
                    details.AppliedDiscounts.Add(orderSubTotalAppliedDiscount1);

                //sub total (excl tax)
                decimal orderSubTotalDiscountAmount2;
                Discount orderSubTotalAppliedDiscount2;
                decimal subTotalWithoutDiscountBase2;
                decimal subTotalWithDiscountBase2;
                decimal subTotalRegistrationAmount2;
                decimal subTotalDepositAmount2;
                _orderTotalCalculationService.GetSubscriptionCartSubTotal(details.Cart,
                    false,
                     out subTotalRegistrationAmount2, out subTotalDepositAmount2, 
                     out orderSubTotalDiscountAmount2, out orderSubTotalAppliedDiscount2,
                    out subTotalWithoutDiscountBase2, out subTotalWithDiscountBase2);
                details.SubscriptionOrderSubTotalExclTax = subTotalWithoutDiscountBase2;
                details.SubscriptionOrderSubTotalDiscountExclTax = orderSubTotalDiscountAmount2;
            }
            else
            {
                details.SubscriptionOrderSubTotalInclTax = details.InitialSubscriptionOrder.SubscriptionOrderSubtotalInclTax;
                details.SubscriptionOrderSubTotalExclTax = details.InitialSubscriptionOrder.SubscriptionOrderSubtotalExclTax;
            }


            //shipping info
            bool borrowCartRequiresShipping;
            if (!processPaymentRequest.IsRecurringPayment)
            {
                borrowCartRequiresShipping = details.Cart.RequiresShipping();
            }
            else
            {
                borrowCartRequiresShipping = details.InitialSubscriptionOrder.ShippingStatus != ShippingStatus.ShippingNotRequired;
            }
            if (borrowCartRequiresShipping)
            {
                if (!processPaymentRequest.IsRecurringPayment)
                {
                    details.PickUpInStore = _shippingSettings.AllowPickUpInStore &&
                        details.Customer.GetAttribute<bool>(SystemCustomerAttributeNames.SelectedPickUpInStore, processPaymentRequest.StoreId);

                    if (!details.PickUpInStore)
                    {
                        if (details.Customer.ShippingAddress == null)
                            throw new NopException("Shipping address is not provided");

                        if (!CommonHelper.IsValidEmail(details.Customer.ShippingAddress.Email))
                            throw new NopException("Email is not valid");

                        //clone shipping address
                        details.ShippingAddress = (Address)details.Customer.ShippingAddress.Clone();
                        if (details.ShippingAddress.Country != null && !details.ShippingAddress.Country.AllowsShipping)
                        {
                            throw new NopException(string.Format("Country '{0}' is not allowed for shipping", details.ShippingAddress.Country.Name));
                        }
                    }

                    var shippingOption = details.Customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, processPaymentRequest.StoreId);
                    if (shippingOption != null)
                    {
                        details.ShippingMethodName = shippingOption.Name;
                        details.ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName;
                    }
                }
                else
                {
                    details.PickUpInStore = details.InitialSubscriptionOrder.PickUpInStore;
                    if (!details.PickUpInStore)
                    {
                        if (details.InitialSubscriptionOrder.ShippingAddress == null)
                            throw new NopException("Shipping address is not available");

                        //clone shipping address
                        details.ShippingAddress = (Address)details.InitialSubscriptionOrder.ShippingAddress.Clone();
                        if (details.ShippingAddress.Country != null && !details.ShippingAddress.Country.AllowsShipping)
                        {
                            throw new NopException(string.Format("Country '{0}' is not allowed for shipping", details.ShippingAddress.Country.Name));
                        }
                    }

                    details.ShippingMethodName = details.InitialSubscriptionOrder.ShippingMethod;
                    details.ShippingRateComputationMethodSystemName = details.InitialSubscriptionOrder.ShippingRateComputationMethodSystemName;
                }
            }
            details.ShippingStatus = borrowCartRequiresShipping
                ? ShippingStatus.NotYetShipped
                : ShippingStatus.ShippingNotRequired;

            //shipping total
            if (!processPaymentRequest.IsRecurringPayment)
            {
                decimal taxRate;
                Discount shippingTotalDiscount;
                decimal? orderShippingTotalInclTax = decimal.Zero; ;
                decimal? orderShippingTotalExclTax = decimal.Zero;
                if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
                    throw new NopException("Shipping total couldn't be calculated");
                details.SubscriptionOrderShippingTotalInclTax = orderShippingTotalInclTax.Value;
                details.SubscriptionOrderShippingTotalExclTax = orderShippingTotalExclTax.Value;

                
            }
            else
            {
                details.SubscriptionOrderShippingTotalInclTax = details.InitialSubscriptionOrder.SubscriptionOrderShippingInclTax;
                details.SubscriptionOrderShippingTotalExclTax = details.InitialSubscriptionOrder.SubscriptionOrderShippingExclTax;
            }


            //payment total
            if (!processPaymentRequest.IsRecurringPayment)
            {
                decimal paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(details.Cart, processPaymentRequest.PaymentMethodSystemName);
                details.PaymentAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, details.Customer);
                details.PaymentAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, details.Customer);
            }
            else
            {
                details.PaymentAdditionalFeeInclTax = details.InitialSubscriptionOrder.PaymentMethodAdditionalFeeInclTax;
                details.PaymentAdditionalFeeExclTax = details.InitialSubscriptionOrder.PaymentMethodAdditionalFeeExclTax;
            }


            //tax total
            if (!processPaymentRequest.IsRecurringPayment)
            {
                //tax amount
                SortedDictionary<decimal, decimal> taxRatesDictionary;
                details.SubscriptionOrderTaxTotal = _orderTotalCalculationService.GetTaxTotal(details.Cart, out taxRatesDictionary);

                //VAT number
                var customerVatStatus = (VatNumberStatus)details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId);
                if (_taxSettings.EuVatEnabled && customerVatStatus == VatNumberStatus.Valid)
                    details.VatNumber = details.Customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

                //tax rates
                foreach (var kvp in taxRatesDictionary)
                {
                    var taxRate = kvp.Key;
                    var taxValue = kvp.Value;
                    details.TaxRates += string.Format("{0}:{1};   ", taxRate.ToString(CultureInfo.InvariantCulture), taxValue.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                details.SubscriptionOrderTaxTotal = details.InitialSubscriptionOrder.SubscriptionOrderTax;
                //VAT number
                details.VatNumber = details.InitialSubscriptionOrder.VatNumber;
            }


            //order total (and applied discounts, gift cards, reward points)
            if (!processPaymentRequest.IsRecurringPayment)
            {
                List<AppliedGiftCard> appliedGiftCards;
                Discount orderAppliedDiscount;
                decimal orderDiscountAmount;
                decimal orderRegistrationAmount;
                decimal orderDepositAmount;
                int redeemedRewardPoints ;
                decimal redeemedRewardPointsAmount;

                var orderTotal = _orderTotalCalculationService.GetSubscriptionCartTotal(details.Cart,
                     out orderRegistrationAmount, out orderDepositAmount,
                    out orderDiscountAmount, out orderAppliedDiscount, out appliedGiftCards,
                    out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (!orderTotal.HasValue)
                    throw new NopException("SubscriptionOrder total couldn't be calculated");

                details.SubscriptionOrderDiscountAmount = orderDiscountAmount;
                details.RedeemedRewardPoints = redeemedRewardPoints;
                details.RedeemedRewardPointsAmount = redeemedRewardPointsAmount;
                details.AppliedGiftCards = appliedGiftCards;
                details.SecurityDeposit = orderDepositAmount;
                details.RegistrationCharge = orderRegistrationAmount;
                details.SubscriptionOrderTotal = orderTotal.Value + details.RegistrationCharge + details.SecurityDeposit;

                //discount history
                if (orderAppliedDiscount != null && !details.AppliedDiscounts.ContainsDiscount(orderAppliedDiscount))
                    details.AppliedDiscounts.Add(orderAppliedDiscount);
            }
            else
            {
                details.SecurityDeposit = details.Cart.FirstOrDefault().SecurityDeposit;
                details.RegistrationCharge = details.Cart.FirstOrDefault().RegistrationCharge;
                details.SubscriptionOrderDiscountAmount = details.InitialSubscriptionOrder.SubscriptionOrderDiscount;
                details.SubscriptionOrderTotal = details.InitialSubscriptionOrder.SubscriptionOrderTotal + details.RegistrationCharge + details.SecurityDeposit;
            } 
            processPaymentRequest.SubscriptionOrderTotal = details.SubscriptionOrderTotal;

            //recurring or standard shopping cart?
            if (!processPaymentRequest.IsRecurringPayment)
            {
                details.IsRecurringSubscriptionCart = details.Cart.IsRecurring();
                if (details.IsRecurringSubscriptionCart)
                {
                    int recurringCycleLength;
                    RecurringPlanCyclePeriod recurringCyclePeriod;
                    int recurringTotalCycles;
                    string recurringCyclesError = details.Cart.GetRecurringCycleInfo(_localizationService,
                        out recurringCycleLength, out recurringCyclePeriod, out recurringTotalCycles);
                    if (!string.IsNullOrEmpty(recurringCyclesError))
                        throw new NopException(recurringCyclesError);

                    processPaymentRequest.RecurringCycleLength = recurringCycleLength;
                    processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
                    processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
                }
            }
            else
            {
                details.IsRecurringSubscriptionCart = true;
            }

            return details;
        }

        /// <summary>
        /// Award (earn) reward points (for placing a new order)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        protected virtual void AwardRewardPoints(SubscriptionOrder order)
        {
            int points = _orderTotalCalculationService.CalculateRewardPoints(order.Customer, order.SubscriptionOrderTotal);
            if (points == 0)
                return;

            //Ensure that reward points were not added (earned) before. We should not add reward points if they were already earned for this order
            if (order.RewardPointsWereAdded)
                return;

            //add reward points
            _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.EarnedForSubscriptionOrder"), order.Id));
            order.RewardPointsWereAdded = true;
            _subscriptionService.UpdateSubscriptionOrder(order);
        }

        /// <summary>
        /// Reduce (cancel) reward points (previously awarded for placing an order)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        protected virtual void ReduceRewardPoints(SubscriptionOrder order)
        {
            int points = _orderTotalCalculationService.CalculateRewardPoints(order.Customer, order.SubscriptionOrderTotal);
            if (points == 0)
                return;

            //ensure that reward points were already earned for this order before
            if (!order.RewardPointsWereAdded)
                return;

            //reduce reward points
            _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReducedForSubscriptionOrder"), order.Id));
            _subscriptionService.UpdateSubscriptionOrder(order);
        }

        /// <summary>
        /// Return back redeemded reward points to a customer (spent when placing an order)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        protected virtual void ReturnBackRedeemedRewardPoints(SubscriptionOrder order)
        {
            //were some points redeemed when placing an order?
            if (order.RedeemedRewardPointsEntry == null)
                return;

            //return back
            _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -order.RedeemedRewardPointsEntry.Points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForSubscriptionOrder"), order.Id));
            _subscriptionService.UpdateSubscriptionOrder(order);
        }


        /// <summary>
        /// Set IsActivated value for purchase gift cards for particular order
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="activate">A value indicating whether to activate gift cards; true - activate, false - deactivate</param>
        protected virtual void SetActivatedValueForPurchasedGiftCards(SubscriptionOrder order, bool activate)
        {
            var giftCards = _giftCardService.GetAllGiftCards(purchasedWithSubscriptionOrderId: order.Id, 
                isGiftCardActivated: !activate);
            foreach (var gc in giftCards)
            {
                if (activate)
                {
                    //activate
                    bool isRecipientNotified = gc.IsRecipientNotified;
                    if (gc.GiftCardType == GiftCardType.Virtual)
                    {
                        //send email for virtual gift card
                        if (!String.IsNullOrEmpty(gc.RecipientEmail) &&
                            !String.IsNullOrEmpty(gc.SenderEmail))
                        {
                            var customerLang = _languageService.GetLanguageById(order.CustomerLanguageId);
                            if (customerLang == null)
                                customerLang = _languageService.GetAllLanguages().FirstOrDefault();
                            if (customerLang == null)
                                throw new Exception("No languages could be loaded");
                            int queuedEmailId = _workflowMessageService.SendGiftCardNotification(gc, customerLang.Id);
                            if (queuedEmailId > 0)
                                isRecipientNotified = true;
                        }
                    }
                    gc.IsGiftCardActivated = true;
                    gc.IsRecipientNotified = isRecipientNotified;
                    _giftCardService.UpdateGiftCard(gc);
                }
                else
                {
                    //deactivate
                    gc.IsGiftCardActivated = false;
                    _giftCardService.UpdateGiftCard(gc);
                }
            }
        }

        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="os">New order status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        protected virtual void SetSubscriptionOrderStatus(SubscriptionOrder order, SubscriptionOrderStatus os, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            SubscriptionOrderStatus prevSubscriptionOrderStatus = order.SubscriptionOrderStatus;
            if (prevSubscriptionOrderStatus == os)
                return;

            //set and save new order status
            order.SubscriptionOrderStatusId = (int)os;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //order notes, notifications
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("SubscriptionOrder status has been changed to {0}", os.ToString()),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            _subscriptionService.UpdateSubscriptionOrder(order);


            if (prevSubscriptionOrderStatus != SubscriptionOrderStatus.Complete &&
                os == SubscriptionOrderStatus.Complete
                && notifyCustomer)
            {
                //notification
                var orderCompletedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToSubscriptionOrderCompletedEmail ?
                    _pdfService.PrintSubscriptionOrderToPdf(order, 0) : null;
                var orderCompletedAttachmentFileName = _orderSettings.AttachPdfInvoiceToSubscriptionOrderCompletedEmail ?
                    "order.pdf" : null;
                int orderCompletedCustomerNotificationQueuedEmailId = _workflowMessageService
                    .SendSubscriptionOrderCompletedCustomerNotification(order, order.CustomerLanguageId, orderCompletedAttachmentFilePath,
                    orderCompletedAttachmentFileName);
                if (orderCompletedCustomerNotificationQueuedEmailId > 0)
                {
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("\"SubscriptionOrder completed\" email (to customer) has been queued. Queued email identifier: {0}.", orderCompletedCustomerNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);
                }
            }

            if (prevSubscriptionOrderStatus != SubscriptionOrderStatus.Cancelled &&
                os == SubscriptionOrderStatus.Cancelled
                && notifyCustomer)
            {
                //notification
                int orderCancelledCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderCancelledCustomerNotification(order, order.CustomerLanguageId);
                if (orderCancelledCustomerNotificationQueuedEmailId > 0)
                {
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("\"SubscriptionOrder cancelled\" email (to customer) has been queued. Queued email identifier: {0}.", orderCancelledCustomerNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);
                }
            }

           

            //gift cards activation
            if (_orderSettings.GiftCards_Activated_SubscriptionOrderStatusId > 0 &&
               _orderSettings.GiftCards_Activated_SubscriptionOrderStatusId == (int)order.SubscriptionOrderStatus)
            {
                SetActivatedValueForPurchasedGiftCards(order, true);
            }

            //gift cards deactivation
            if (_orderSettings.GiftCards_Deactivated_SubscriptionOrderStatusId > 0 &&
               _orderSettings.GiftCards_Deactivated_SubscriptionOrderStatusId == (int)order.SubscriptionOrderStatus)
            {
                SetActivatedValueForPurchasedGiftCards(order, false);
            }
        }

        /// <summary>
        /// Process order paid status
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        protected virtual void ProcessSubscriptionOrderPaid(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //raise event
            _eventPublisher.Publish(new SubscriptionOrderPaidEvent(order));

            //order paid email notification
            if (order.SubscriptionOrderTotal != decimal.Zero)
            {
                //we should not send it for free ($0 total) orders?
                //remove this "if" statement if you want to send it in this case

                var orderPaidAttachmentFilePath = _orderSettings.AttachPdfInvoiceToSubscriptionOrderPaidEmail ?
                    _pdfService.PrintSubscriptionOrderToPdf(order, 0) : null;
                var orderPaidAttachmentFileName = _orderSettings.AttachPdfInvoiceToSubscriptionOrderPaidEmail ?
                    "order.pdf" : null;
                _workflowMessageService.SendSubscriptionOrderPaidCustomerNotification(order, order.CustomerLanguageId,
                    orderPaidAttachmentFilePath, orderPaidAttachmentFileName);

                _workflowMessageService.SendSubscriptionOrderPaidStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
                var vendors = GetVendorsInSubscriptionOrder(order);
                foreach (var vendor in vendors)
                {
                    _workflowMessageService.SendSubscriptionOrderPaidVendorNotification(order, vendor, _localizationSettings.DefaultAdminLanguageId);
                }
                //TODO add "order paid email sent" order note
            }

            //customer roles with "purchased with product" specified
            ProcessCustomerRolesWithPurchasedProductSpecified(order, true);
        }

        /// <summary>
        /// Process customer roles with "Purchased with Product" property configured
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="add">A value indicating whether to add configured customer role; true - add, false - remove</param>
        protected virtual void ProcessCustomerRolesWithPurchasedProductSpecified(SubscriptionOrder order, bool add)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //purchased product identifiers
            var purchasedProductIds = new List<int>();
            foreach (var orderItem in order.SubscriptionOrderItems)
            {
                //standard items
                purchasedProductIds.Add(orderItem.PlanId);

                //bundled (associated) products
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(orderItem.AttributesXml);
                foreach (var attributeValue in attributeValues)
                {
                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    {
                       purchasedProductIds.Add(attributeValue.AssociatedProductId);
                    }
                }
            }

            //list of customer roles
            var customerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Where(cr => purchasedProductIds.Contains(cr.PurchasedWithProductId))
                .ToList();

            if (customerRoles.Count > 0)
            {
                var customer = order.Customer;
                foreach (var customerRole in customerRoles)
                {
                    if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) == 0)
                    {
                        //not in the list yet
                        if (add)
                        {
                            //add
                            customer.CustomerRoles.Add(customerRole);
                        }
                    }
                    else
                    {
                        //already in the list
                        if (!add)
                        {
                            //remove
                            customer.CustomerRoles.Remove(customerRole);
                        }
                    }
                }
                _customerService.UpdateCustomer(customer);
            }
        }

        /// <summary>
        /// Get a list of vendors in order (order items)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Vendors</returns>
        protected virtual IList<Vendor> GetVendorsInSubscriptionOrder(SubscriptionOrder order)
        {
            var vendors = new List<Vendor>();
            foreach (var orderItem in order.SubscriptionOrderItems)
            {
                var vendorId = orderItem.Plan.VendorId;
                //find existing
                var vendor = vendors.FirstOrDefault(v => v.Id == vendorId);
                if (vendor == null)
                {
                    //not found. load by Id
                    vendor = _vendorService.GetVendorById(vendorId);
                    if (vendor != null && !vendor.Deleted && vendor.Active)
                    {
                        vendors.Add(vendor);
                    }
                }
            }

            return vendors;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Validated order</returns>
        public virtual void CheckSubscriptionOrderStatus(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.PaymentStatus == PaymentStatus.Paid && !order.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                order.PaidDateUtc = DateTime.UtcNow;
                _subscriptionService.UpdateSubscriptionOrder(order);
            }

            if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Pending)
            {
                if (order.PaymentStatus == PaymentStatus.Authorized ||
                    order.PaymentStatus == PaymentStatus.Paid)
                {
                    SetSubscriptionOrderStatus(order, SubscriptionOrderStatus.Processing, false);
                }
            }

            if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Pending)
            {
                if (order.ShippingStatus == ShippingStatus.PartiallyShipped ||
                    order.ShippingStatus == ShippingStatus.Shipped ||
                    order.ShippingStatus == ShippingStatus.Delivered)
                {
                    SetSubscriptionOrderStatus(order, SubscriptionOrderStatus.Processing, false);
                }
            }

            if (order.SubscriptionOrderStatus != SubscriptionOrderStatus.Cancelled &&
                order.SubscriptionOrderStatus != SubscriptionOrderStatus.Complete)
            {
                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    //if (order.ShippingStatus == ShippingStatus.ShippingNotRequired || order.ShippingStatus == ShippingStatus.Delivered)
                    //{
                        SetSubscriptionOrderStatus(order, SubscriptionOrderStatus.Complete, true);
                  //  }
                }
            }
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        public virtual PlaceSubscriptionOrderResult PlaceSubscriptionOrder(ProcessPaymentRequest processPaymentRequest)
        {
            //think about moving functionality of processing recurring orders (after the initial order was placed) to ProcessNextRecurringPayment() method
            if (processPaymentRequest == null)
                throw new ArgumentNullException("processPaymentRequest");

            var result = new PlaceSubscriptionOrderResult();
            try
            {
                if (processPaymentRequest.SubscriptionOrderGuid == Guid.Empty)
                    processPaymentRequest.SubscriptionOrderGuid = Guid.NewGuid();

                //prepare order details
                var details = PreparePlaceSubscriptionOrderDetails(processPaymentRequest);

                #region Payment workflow


                //process payment
                ProcessPaymentResult processPaymentResult = null;
                //skip payment workflow if order total equals zero
                var skipPaymentWorkflow = details.SubscriptionOrderTotal == decimal.Zero;
                if (!skipPaymentWorkflow)
                {
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        throw new NopException("Payment method couldn't be loaded");

                    //ensure that payment method is active
                    if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
                        throw new NopException("Payment method is not active");

                    if (!processPaymentRequest.IsRecurringPayment)
                    {
                        if (details.IsRecurringSubscriptionCart)
                        {
                            //recurring cart
                            var recurringPaymentType = _paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName);
                            switch (recurringPaymentType)
                            {
                                case RecurringPaymentType.NotSupported:
                                    throw new NopException("Recurring payments are not supported by selected payment method");
                                case RecurringPaymentType.Manual:
                                case RecurringPaymentType.Automatic:
                                    processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                                    break;
                                default:
                                    throw new NopException("Not supported recurring payment type");
                            }
                        }
                        else
                        {
                            //standard cart
                            processPaymentResult = _paymentService.ProcessPayment(processPaymentRequest);
                        }
                    }
                    else
                    {
                        if (details.IsRecurringSubscriptionCart)
                        {
                            //Old credit card info
                            processPaymentRequest.CreditCardType = details.InitialSubscriptionOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialSubscriptionOrder.CardType) : "";
                            processPaymentRequest.CreditCardName = details.InitialSubscriptionOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialSubscriptionOrder.CardName) : "";
                            processPaymentRequest.CreditCardNumber = details.InitialSubscriptionOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialSubscriptionOrder.CardNumber) : "";
                            processPaymentRequest.CreditCardCvv2 = details.InitialSubscriptionOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialSubscriptionOrder.CardCvv2) : "";
                            try
                            {
                                processPaymentRequest.CreditCardExpireMonth = details.InitialSubscriptionOrder.AllowStoringCreditCardNumber ? Convert.ToInt32(_encryptionService.DecryptText(details.InitialSubscriptionOrder.CardExpirationMonth)) : 0;
                                processPaymentRequest.CreditCardExpireYear = details.InitialSubscriptionOrder.AllowStoringCreditCardNumber ? Convert.ToInt32(_encryptionService.DecryptText(details.InitialSubscriptionOrder.CardExpirationYear)) : 0;
                            }
                            catch {}

                            var recurringPaymentType = _paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName);
                            switch (recurringPaymentType)
                            {
                                case RecurringPaymentType.NotSupported:
                                    throw new NopException("Recurring payments are not supported by selected payment method");
                                case RecurringPaymentType.Manual:
                                    processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                                    break;
                                case RecurringPaymentType.Automatic:
                                    //payment is processed on payment gateway site
                                    processPaymentResult = new ProcessPaymentResult();
                                    break;
                                default:
                                    throw new NopException("Not supported recurring payment type");
                            }
                        }
                        else
                        {
                            throw new NopException("No recurring products");
                        }
                    }
                }
                else
                {
                    //payment is not required
                    if (processPaymentResult == null)
                        processPaymentResult = new ProcessPaymentResult();
                    processPaymentResult.NewPaymentStatus = PaymentStatus.Paid;
                }

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                #endregion

                if (processPaymentResult.Success)
                {
                    #region Save order details

                    var order = new SubscriptionOrder
                    {
                        StoreId = processPaymentRequest.StoreId,
                        SubscriptionOrderGuid = processPaymentRequest.SubscriptionOrderGuid,
                        CustomerId = details.Customer.Id,
                        CustomerLanguageId = details.CustomerLanguage.Id,
                        CustomerTaxDisplayType = details.CustomerTaxDisplayType,
                        CustomerIp = _webHelper.GetCurrentIpAddress(),
                        SubscriptionOrderSubtotalInclTax = details.SubscriptionOrderSubTotalInclTax,
                        SubscriptionOrderSubtotalExclTax = details.SubscriptionOrderSubTotalExclTax,
                        SubscriptionOrderSubTotalDiscountInclTax = details.SubscriptionOrderSubTotalDiscountInclTax,
                        SubscriptionOrderSubTotalDiscountExclTax = details.SubscriptionOrderSubTotalDiscountExclTax,
                        SubscriptionOrderShippingInclTax = details.SubscriptionOrderShippingTotalInclTax,
                        SubscriptionOrderShippingExclTax = details.SubscriptionOrderShippingTotalExclTax,
                        PaymentMethodAdditionalFeeInclTax = details.PaymentAdditionalFeeInclTax,
                        PaymentMethodAdditionalFeeExclTax = details.PaymentAdditionalFeeExclTax,
                        TaxRates = details.TaxRates,
                        SubscriptionOrderTax = details.SubscriptionOrderTaxTotal,
                        SubscriptionOrderTotal = details.SubscriptionOrderTotal,
                        RegistrationCharge = details.RegistrationCharge,
                        SecurityDeposit = details.SecurityDeposit,
                        RegistrationChargeDiscount = details.RegistrationChargeDiscount,
                        RefundedAmount = decimal.Zero,
                        SubscriptionOrderDiscount = details.SubscriptionOrderDiscountAmount,
                        CheckoutAttributeDescription = details.CheckoutAttributeDescription,
                        CheckoutAttributesXml = details.CheckoutAttributesXml,
                        CustomerCurrencyCode = details.CustomerCurrencyCode,
                        CurrencyRate = details.CustomerCurrencyRate,
                        AffiliateId = details.AffiliateId,
                        SubscriptionOrderStatus = SubscriptionOrderStatus.Pending,
                        AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
                        CardType = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardType) : string.Empty,
                        CardName = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardName) : string.Empty,
                        CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardNumber) : string.Empty,
                        MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(processPaymentRequest.CreditCardNumber)),
                        CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardCvv2) : string.Empty,
                        CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireMonth.ToString()) : string.Empty,
                        CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireYear.ToString()) : string.Empty,
                        PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                        AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                        AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                        AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                        CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                        CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                        SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                        PaymentStatus = processPaymentResult.NewPaymentStatus,
                        PaidDateUtc = null,
                        BillingAddress = details.BillingAddress,
                        ShippingAddress = details.ShippingAddress,
                        ShippingStatus = details.ShippingStatus,
                        ShippingMethod = details.ShippingMethodName,
                        PickUpInStore = details.PickUpInStore,
                        ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
                        CustomValuesXml = processPaymentRequest.SerializeCustomValues(),
                        VatNumber = details.VatNumber,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _subscriptionService.InsertSubscriptionOrder(order);

                    result.PlacedSubscriptionOrder = order;

                    if (!processPaymentRequest.IsRecurringPayment)
                    {
                        //move shopping cart items to order items
                        foreach (var sc in details.Cart)
                        {
                            //prices
                            decimal taxRate;
                            Discount scDiscount;
                            decimal discountAmount;
                            decimal scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                            decimal scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out discountAmount, out scDiscount);
                            decimal scUnitPriceInclTax = _taxService.GetPlanPrice(sc.Plan, scUnitPrice, true, details.Customer, out taxRate);
                            decimal scUnitPriceExclTax = _taxService.GetPlanPrice(sc.Plan, scUnitPrice, false, details.Customer, out taxRate);
                            decimal scSubTotalInclTax = _taxService.GetPlanPrice(sc.Plan, scSubTotal, true, details.Customer, out taxRate);
                            decimal scSubTotalExclTax = _taxService.GetPlanPrice(sc.Plan, scSubTotal, false, details.Customer, out taxRate);

                            decimal discountAmountInclTax = _taxService.GetPlanPrice(sc.Plan, discountAmount, true, details.Customer, out taxRate);
                            decimal discountAmountExclTax = _taxService.GetPlanPrice(sc.Plan, discountAmount, false, details.Customer, out taxRate);
                            if (scDiscount != null && !details.AppliedDiscounts.ContainsDiscount(scDiscount))
                                details.AppliedDiscounts.Add(scDiscount);

                            //attributes
                           // string attributeDescription = _productAttributeFormatter.FormatAttributes(sc.Plan, sc.AttributesXml, details.Customer);
                             

                            //save order item
                            var orderItem = new SubscriptionOrderItem
                            {
                                SubscriptionOrderItemGuid = Guid.NewGuid(),
                                SubscriptionOrder = order,
                                PlanId = sc.PlanId,
                                PriceInclTax = scSubTotalInclTax,
                                PriceExclTax = scSubTotalExclTax,
                                OriginalPlanCost = _priceCalculationService.GetProductCost(sc.Plan, sc.AttributesXml),
                                AttributeDescription = "",
                                AttributesXml = sc.AttributesXml,
                                Quantity = sc.Quantity,
                                DiscountAmountInclTax = discountAmountInclTax,
                                DiscountAmountExclTax = discountAmountExclTax,
                                DownloadCount = 0,
                                IsDownloadActivated = false,
                                IsRental = sc.Plan.IsRental,
                                RentalPriceLength = sc.Plan.RentalPriceLength,
                                RentalPricePeriodId = sc.Plan.RentalPricePeriodId,
                                LicenseDownloadId = 0,
                                ItemWeight = Decimal.Zero,
                                RentalStartDateUtc = sc.RentalStartDateUtc,
                                RentalEndDateUtc = sc.RentalEndDateUtc
                            };

                            if (sc.Plan.RentalPricePeriodId == 20)
                            {
                                orderItem.UnitPriceExclTax = scSubTotalExclTax / sc.Plan.RentalPriceLength;
                                orderItem.UnitPriceInclTax = scSubTotalInclTax / sc.Plan.RentalPriceLength;
                            }else if (sc.Plan.RentalPricePeriodId == 30)
                            {
                                orderItem.UnitPriceExclTax = scSubTotalExclTax / (sc.Plan.RentalPriceLength * 12);
                                orderItem.UnitPriceInclTax = scSubTotalInclTax / (sc.Plan.RentalPriceLength * 12);
                            }
                            else
                            {
                                orderItem.UnitPriceExclTax = scSubTotalExclTax;
                                orderItem.UnitPriceInclTax = scSubTotalInclTax;
                            }

                            order.SubscriptionOrderItems.Add(orderItem);
                            _subscriptionService.UpdateSubscriptionOrder(order);

                            //gift cards
                            if (sc.Plan.IsGiftCard)
                            {
                                string giftCardRecipientName, giftCardRecipientEmail,
                                    giftCardSenderName, giftCardSenderEmail, giftCardMessage;
                                _productAttributeParser.GetGiftCardAttribute(sc.AttributesXml,
                                    out giftCardRecipientName, out giftCardRecipientEmail,
                                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                                for (int i = 0; i < sc.Quantity; i++)
                                {
                                    var gc = new GiftCard
                                    {
                                        GiftCardType = sc.Plan.GiftCardType,
                                        PurchasedWithSubscriptionOrderItem = orderItem,
                                        Amount = sc.Plan.OverriddenGiftCardAmount.HasValue ? sc.Plan.OverriddenGiftCardAmount.Value : scUnitPriceExclTax,
                                        IsGiftCardActivated = false,
                                        GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                                        RecipientName = giftCardRecipientName,
                                        RecipientEmail = giftCardRecipientEmail,
                                        SenderName = giftCardSenderName,
                                        SenderEmail = giftCardSenderEmail,
                                        Message = giftCardMessage,
                                        IsRecipientNotified = false,
                                        CreatedOnUtc = DateTime.UtcNow
                                    };
                                    _giftCardService.InsertGiftCard(gc);
                                }
                            }

                            //inventory
                           // _productService.AdjustInventory(sc.Plan, -sc.Quantity, sc.AttributesXml);
                        }

                        //clear shopping cart
                        details.Cart.ToList().ForEach(sci => _subscriptionCartService.DeleteSubscriptionCartItem(sci, false));
                    }
                    else
                    {
                        //recurring payment
                        var initialSubscriptionOrderItems = details.InitialSubscriptionOrder.SubscriptionOrderItems;
                        foreach (var orderItem in initialSubscriptionOrderItems)
                        {
                            //save item
                            var newSubscriptionOrderItem = new SubscriptionOrderItem
                            {
                                SubscriptionOrderItemGuid = Guid.NewGuid(),
                                SubscriptionOrder = order,
                                PlanId = orderItem.PlanId,
                                UnitPriceInclTax = orderItem.UnitPriceInclTax,
                                UnitPriceExclTax = orderItem.UnitPriceExclTax,
                                PriceInclTax = orderItem.PriceInclTax,
                                PriceExclTax = orderItem.PriceExclTax,
                                OriginalPlanCost = orderItem.OriginalPlanCost,
                                AttributeDescription = orderItem.AttributeDescription,
                                AttributesXml = orderItem.AttributesXml,
                                Quantity = orderItem.Quantity,
                                DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
                                DiscountAmountExclTax = orderItem.DiscountAmountExclTax,
                                DownloadCount = 0,
                                IsDownloadActivated = false,
                                LicenseDownloadId = 0,
                                ItemWeight = orderItem.ItemWeight,
                                RentalStartDateUtc = orderItem.RentalStartDateUtc,
                                RentalEndDateUtc = orderItem.RentalEndDateUtc
                            };
                            order.SubscriptionOrderItems.Add(newSubscriptionOrderItem);
                            _subscriptionService.UpdateSubscriptionOrder(order);

                            //gift cards
                            if (orderItem.Plan.IsGiftCard)
                            {
                                string giftCardRecipientName, giftCardRecipientEmail,
                                    giftCardSenderName, giftCardSenderEmail, giftCardMessage;
                                _productAttributeParser.GetGiftCardAttribute(orderItem.AttributesXml,
                                    out giftCardRecipientName, out giftCardRecipientEmail,
                                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                                for (int i = 0; i < orderItem.Quantity; i++)
                                {
                                    var gc = new GiftCard
                                    {
                                        GiftCardType = orderItem.Plan.GiftCardType,
                                        PurchasedWithSubscriptionOrderItem = newSubscriptionOrderItem,
                                        Amount = orderItem.UnitPriceExclTax,
                                        IsGiftCardActivated = false,
                                        GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                                        RecipientName = giftCardRecipientName,
                                        RecipientEmail = giftCardRecipientEmail,
                                        SenderName = giftCardSenderName,
                                        SenderEmail = giftCardSenderEmail,
                                        Message = giftCardMessage,
                                        IsRecipientNotified = false,
                                        CreatedOnUtc = DateTime.UtcNow
                                    };
                                    _giftCardService.InsertGiftCard(gc);
                                }
                            }

                            //inventory
                           ///_productService.AdjustInventory(orderItem.Plan, -orderItem.Quantity, orderItem.AttributesXml);
                        }
                    }

                    //discount usage history
                    if (!processPaymentRequest.IsRecurringPayment)
                        foreach (var discount in details.AppliedDiscounts)
                        {
                            var duh = new DiscountUsageHistory
                            {
                                Discount = discount,
                                SubscriptionOrder = order,
                                CreatedOnUtc = DateTime.UtcNow
                            };
                            _discountService.InsertDiscountUsageHistory(duh);
                        }

                    //gift card usage history
                    if (!processPaymentRequest.IsRecurringPayment)
                        if (details.AppliedGiftCards != null)
                            foreach (var agc in details.AppliedGiftCards)
                            {
                                decimal amountUsed = agc.AmountCanBeUsed;
                                var gcuh = new GiftCardUsageHistory
                                {
                                    GiftCard = agc.GiftCard,
                                    UsedWithSubscriptionOrder = order,
                                    UsedValue = amountUsed,
                                    CreatedOnUtc = DateTime.UtcNow
                                };
                                agc.GiftCard.GiftCardUsageHistory.Add(gcuh);
                                _giftCardService.UpdateGiftCard(agc.GiftCard);
                            }

                    //reward points history
                    if (details.RedeemedRewardPointsAmount > decimal.Zero)
                    {
                        _rewardPointService.AddRewardPointsHistoryEntry(details.Customer,
                            -details.RedeemedRewardPoints, order.StoreId,
                            string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForSubscriptionOrder", order.CustomerLanguageId), order.Id),
                            order, details.RedeemedRewardPointsAmount);
                        _customerService.UpdateCustomer(details.Customer);
                    }

                    //recurring orders
                    if (!processPaymentRequest.IsRecurringPayment && details.IsRecurringSubscriptionCart)
                    {
                        //create recurring payment (the first payment)
                        var rp = new RecurringPayment
                        {
                            CycleLength = processPaymentRequest.RecurringCycleLength,
                            CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
                            TotalCycles = processPaymentRequest.RecurringTotalCycles,
                            StartDateUtc = DateTime.UtcNow,
                            IsActive = true,
                            CreatedOnUtc = DateTime.UtcNow,
                            InitialSubscriptionOrder = order,
                        };
                        _subscriptionService.InsertRecurringPayment(rp);


                        var recurringPaymentType = _paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName);
                        switch (recurringPaymentType)
                        {
                            case RecurringPaymentType.NotSupported:
                                {
                                    //not supported
                                }
                                break;
                            case RecurringPaymentType.Manual:
                                {
                                    //first payment
                                    var rph = new RecurringPaymentHistory
                                    {
                                        RecurringPayment = rp,
                                        CreatedOnUtc = DateTime.UtcNow,
                                        SubscriptionOrderId = order.Id,
                                    };
                                    rp.RecurringPaymentHistory.Add(rph);
                                    _subscriptionService.UpdateRecurringPayment(rp);
                                }
                                break;
                            case RecurringPaymentType.Automatic:
                                {
                                    //will be created later (process is automated)
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    #endregion

                    #region Notifications & notes

                    //notes, messages
                    if (_workContext.OriginalCustomerIfImpersonated != null)
                    {
                        //this order is placed by a store administrator impersonating a customer
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("SubscriptionOrder placed by a store owner ('{0}'. ID = {1}) impersonating the customer.",
                                _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }
                    else
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = "SubscriptionOrder placed",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }


                    //send email notifications
                    int orderPlacedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderPlacedStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
                    if (orderPlacedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"SubscriptionOrder placed\" email (to store owner) has been queued. Queued email identifier: {0}.", orderPlacedStoreOwnerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }

                    var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToSubscriptionOrderPlacedEmail ?
                        _pdfService.PrintSubscriptionOrderToPdf(order, order.CustomerLanguageId) : null;
                    var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToSubscriptionOrderPlacedEmail ?
                        "order.pdf" : null;
                    int orderPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
                        .SendSubscriptionOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
                    if (orderPlacedCustomerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"SubscriptionOrder placed\" email (to customer) has been queued. Queued email identifier: {0}.", orderPlacedCustomerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }

                    var vendors = GetVendorsInSubscriptionOrder(order);
                    foreach (var vendor in vendors)
                    {
                        int orderPlacedVendorNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderPlacedVendorNotification(order, vendor, order.CustomerLanguageId);
                        if (orderPlacedVendorNotificationQueuedEmailId > 0)
                        {
                            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                            {
                                Note = string.Format("\"SubscriptionOrder placed\" email (to vendor) has been queued. Queued email identifier: {0}.", orderPlacedVendorNotificationQueuedEmailId),
                                DisplayToCustomer = false,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                            _subscriptionService.UpdateSubscriptionOrder(order);
                        }
                    }

                    //check order status
                    CheckSubscriptionOrderStatus(order);

                    //reset checkout data
                    if (!processPaymentRequest.IsRecurringPayment)
                        _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);

                    if (!processPaymentRequest.IsRecurringPayment)
                    {
                        _customerActivityService.InsertActivity(
                            "PublicStore.PlaceSubscriptionOrder",
                            _localizationService.GetResource("ActivityLog.PublicStore.PlaceSubscriptionOrder"),
                            order.Id);
                    }
                    
                    //raise event       
                    _eventPublisher.Publish(new SubscriptionOrderPlacedEvent(order));

                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        ProcessSubscriptionOrderPaid(order);
                    }
                    #endregion
                }
                else
                {
                    //payment errors
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            #region Process errors

            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i + 1, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //log it
                string logError = string.Format("Error while placing order. {0}", error);
                var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                _logger.Error(logError, customer: customer);
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteSubscriptionOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //check whether the order wasn't cancelled before
            //if it already was cancelled, then there's no need to make the following adjustments
            //(such as reward points, inventory, recurring payments)
            //they already was done when cancelling the order
            if (order.SubscriptionOrderStatus != SubscriptionOrderStatus.Cancelled)
            {
                //return (add) back redeemded reward points
                ReturnBackRedeemedRewardPoints(order);
                //reduce (cancel) back reward points (previously awarded for this order)
                ReduceRewardPoints(order);

                //cancel recurring payments
                var recurringPayments = _subscriptionService.SearchRecurringPayments(initialSubscriptionOrderId: order.Id);
                foreach (var rp in recurringPayments)
                {
                    var errors = CancelRecurringPayment(rp);
                    //use "errors" variable?
                }
 
                //Adjust inventory
                foreach (var orderItem in order.SubscriptionOrderItems)
                {
                  //  _productService.AdjustInventory(orderItem.Plan, orderItem.Quantity, orderItem.AttributesXml);
                }

            }

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "SubscriptionOrder has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);
            
            //now delete an order
            _subscriptionService.DeleteSubscriptionOrder(order);
        }

        /// <summary>
        /// Process next recurring psayment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void ProcessNextRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");
            try
            {
                if (!recurringPayment.IsActive)
                    throw new NopException("Recurring payment is not active");

                var initialSubscriptionOrder = recurringPayment.InitialSubscriptionOrder;
                if (initialSubscriptionOrder == null)
                    throw new NopException("Initial order could not be loaded");

                var customer = initialSubscriptionOrder.Customer;
                if (customer == null)
                    throw new NopException("Customer could not be loaded");

                var nextPaymentDate = recurringPayment.NextPaymentDate;
                if (!nextPaymentDate.HasValue)
                    throw new NopException("Next payment date could not be calculated");

                //payment info
                var paymentInfo = new ProcessPaymentRequest
                {
                    StoreId = initialSubscriptionOrder.StoreId,
                    CustomerId = customer.Id,
                    SubscriptionOrderGuid = Guid.NewGuid(),
                    IsRecurringPayment = true,
                    InitialSubscriptionOrderId = initialSubscriptionOrder.Id,
                    RecurringCycleLength = recurringPayment.CycleLength,
                    RecurringCyclePeriod = recurringPayment.CyclePeriod,
                    RecurringTotalCycles = recurringPayment.TotalCycles,
                };

                //place a new order
                var result = this.PlaceSubscriptionOrder(paymentInfo);
                if (result.Success)
                {
                    if (result.PlacedSubscriptionOrder == null)
                        throw new NopException("Placed order could not be loaded");

                    var rph = new RecurringPaymentHistory
                    {
                        RecurringPayment = recurringPayment,
                        CreatedOnUtc = DateTime.UtcNow,
                        SubscriptionOrderId = result.PlacedSubscriptionOrder.Id,
                    };
                    recurringPayment.RecurringPaymentHistory.Add(rph);
                    _subscriptionService.UpdateRecurringPayment(recurringPayment);
                }
                else
                {
                    string error = "";
                    for (int i = 0; i < result.Errors.Count; i++)
                    {
                        error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                        if (i != result.Errors.Count - 1)
                            error += ". ";
                    }
                    throw new NopException(error);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error while processing recurring order. {0}", exc.Message), exc);
                throw;
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual IList<string> CancelRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var initialSubscriptionOrder = recurringPayment.InitialSubscriptionOrder;
            if (initialSubscriptionOrder == null)
                return new List<string> { "Initial order could not be loaded" };


            var request = new CancelRecurringPaymentRequest();
            CancelRecurringPaymentResult result = null;
            try
            {
                request.SubscriptionOrder = initialSubscriptionOrder;
                result = _paymentService.CancelRecurringPayment(request);
                if (result.Success)
                {
                    //update recurring payment
                    recurringPayment.IsActive = false;
                    _subscriptionService.UpdateRecurringPayment(recurringPayment);


                    //add a note
                    initialSubscriptionOrder.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = "Recurring payment has been cancelled",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(initialSubscriptionOrder);

                    //notify a store owner
                    _workflowMessageService
                        .SendRecurringPaymentCancelledStoreOwnerNotification(recurringPayment, 
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CancelRecurringPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }


            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                initialSubscriptionOrder.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Unable to cancel recurring payment. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(initialSubscriptionOrder);

                //log it
                string logError = string.Format("Error cancelling recurring payment. SubscriptionOrder #{0}. Error: {1}", initialSubscriptionOrder.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public virtual bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return false;

            if (customerToValidate == null)
                return false;

            var initialSubscriptionOrder = recurringPayment.InitialSubscriptionOrder;
            if (initialSubscriptionOrder == null)
                return false;

            var customer = recurringPayment.InitialSubscriptionOrder.Customer;
            if (customer == null)
                return false;

            if (initialSubscriptionOrder.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
                return false;

            if (!customerToValidate.IsAdmin())
            {
                if (customer.Id != customerToValidate.Id)
                    return false;
            }

            if (!recurringPayment.NextPaymentDate.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Send a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void Ship(Shipment shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = _subscriptionService.GetOrderById(shipment.OrderItem.SubscriptionOrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is already shipped");

            shipment.ShippedDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            //process products with "Multiple warehouse" support enabled
            foreach (var item in shipment.ShipmentItems)
            {
                var itemDetail = _subscriptionService.GetItemDetailById(item.ItemDetailId);
               _productService.BookReservedInventory(itemDetail.Product, item.WarehouseId, -item.Quantity);
            }

            foreach (var orderitem in order.OrderItems) {
                if (orderitem.HasItemsToAddToShipment() || orderitem.HasItemsToShip())
                    orderitem.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
                else
                    orderitem.ShippingStatusId = (int)ShippingStatus.Shipped;
                _subscriptionService.UpdateSubscriptionOrder(order);
            }


            //check whether we have more items to ship
            if (order.HasItemsToAddToShipment() || order.HasItemsToShip())
                order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
            else
                order.ShippingStatusId = (int)ShippingStatus.Shipped;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = string.Format("Shipment# {0} has been sent", shipment.Id),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            if (notifyCustomer)
            {
                //notify customer
                int queuedEmailId = _workflowMessageService.SendShipmentSentCustomerNotification(shipment, order.CustomerLanguageId);
                if (queuedEmailId > 0)
                {
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("\"Shipped\" email (to customer) has been queued. Queued email identifier: {0}.", queuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);
                }
            }

            //event
            _eventPublisher.PublishShipmentSent(shipment);

            //check order status
            CheckSubscriptionOrderStatus(order);
        }

        /// <summary>
        /// Marks a shipment as delivered
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void Deliver(Shipment shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.OrderItem.SubscriptionOrder;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (!shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is not shipped yet");

            if (shipment.DeliveryDateUtc.HasValue)
                throw new Exception("This shipment is already delivered");

            shipment.DeliveryDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            if (!order.HasItemsToAddToShipment() && !order.HasItemsToShip() && !order.HasItemsToDeliver())
                order.ShippingStatusId = (int)ShippingStatus.Delivered;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = string.Format("Shipment# {0} has been delivered", shipment.Id),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            if (notifyCustomer)
            {
                //send email notification
                int queuedEmailId = _workflowMessageService.SendShipmentDeliveredCustomerNotification(shipment, order.CustomerLanguageId);
                if (queuedEmailId > 0)
                {
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("\"Delivered\" email (to customer) has been queued. Queued email identifier: {0}.", queuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);
                }
            }

            //event
            _eventPublisher.PublishShipmentDelivered(shipment);

            //check order status
            CheckSubscriptionOrderStatus(order);
        }

 

        /// <summary>
        /// Marks a shipment as delivered
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
       


        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public virtual bool CanCancelSubscriptionOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
                return false;

            return true;
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void CancelSubscriptionOrder(SubscriptionOrder order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanCancelSubscriptionOrder(order))
                throw new NopException("Cannot do cancel for order.");

            //Cancel order
            SetSubscriptionOrderStatus(order, SubscriptionOrderStatus.Cancelled, notifyCustomer);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "SubscriptionOrder has been cancelled",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            //return (add) back redeemded reward points
            ReturnBackRedeemedRewardPoints(order);

            //cancel recurring payments
            var recurringPayments = _subscriptionService.SearchRecurringPayments(initialSubscriptionOrderId: order.Id);
            foreach (var rp in recurringPayments)
            {
                var errors = CancelRecurringPayment(rp);
                //use "errors" variable?
            }

            
            //Adjust inventory
            foreach (var orderItem in order.SubscriptionOrderItems)
            {
               // _productService.AdjustInventory(orderItem.Plan, orderItem.Quantity, orderItem.AttributesXml);
            }

            _eventPublisher.Publish(new SubscriptionOrderCancelledEvent(order));

        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public virtual bool CanMarkSubscriptionOrderAsAuthorized(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Pending)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        public virtual void MarkAsAuthorized(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.PaymentStatusId = (int)PaymentStatus.Authorized;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "SubscriptionOrder has been marked as authorized",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            //check order status
            CheckSubscriptionOrderStatus(order);
        }



        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public virtual bool CanCapture(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled ||
                order.SubscriptionOrderStatus == SubscriptionOrderStatus.Pending)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportCapture(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Capture an order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Capture(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanCapture(order))
                throw new NopException("Cannot do capture for order.");

            var request = new CapturePaymentRequest();
            CapturePaymentResult result = null;
            try
            {
                //old info from placing order
                request.SubscriptionOrder = order;
                result = _paymentService.Capture(request);

                if (result.Success)
                {
                    var paidDate = order.PaidDateUtc;
                    if (result.NewPaymentStatus == PaymentStatus.Paid)
                        paidDate = DateTime.UtcNow;

                    order.CaptureTransactionId = result.CaptureTransactionId;
                    order.CaptureTransactionResult = result.CaptureTransactionResult;
                    order.PaymentStatus = result.NewPaymentStatus;
                    order.PaidDateUtc = paidDate;
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    //add a note
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = "SubscriptionOrder has been captured",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    CheckSubscriptionOrderStatus(order);
     
                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        ProcessSubscriptionOrderPaid(order);
                    }
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CapturePaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }


            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Unable to capture order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                //log it
                string logError = string.Format("Error capturing order #{0}. Error: {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public virtual bool CanMarkSubscriptionOrderAsPaid(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.Refunded ||
                order.PaymentStatus == PaymentStatus.Voided)
                return false;

            return true;
        }

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        public virtual void MarkSubscriptionOrderAsPaid(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanMarkSubscriptionOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            var orders = _subscriptionService.SearchSubscriptionOrders(customerId: order.CustomerId);
            foreach (SubscriptionOrder so in orders)
            {
                if (so.Id != order.Id) { 
                    if (so.SubscriptionOrderStatus == SubscriptionOrderStatus.Complete)
                    {
                        so.SubscriptionOrderStatus = SubscriptionOrderStatus.Closed;
                    }
                if (so.SubscriptionOrderStatus == SubscriptionOrderStatus.Processing
                        || so.SubscriptionOrderStatus == SubscriptionOrderStatus.Pending)
                {
                    so.SubscriptionOrderStatus = SubscriptionOrderStatus.Cancelled;
                }
                }
            }
            

            order.PaymentStatusId = (int)PaymentStatus.Paid;
            order.PaidDateUtc = DateTime.UtcNow;
            _subscriptionService.UpdateSubscriptionOrder(order);

            var cust = order.Customer;

            cust.RegistrationChargeBalance = cust.RegistrationChargeBalance + (order.RegistrationCharge - order.RegistrationChargeDiscount);
            cust.SecurityDepositBalance = cust.SecurityDepositBalance + (order.SecurityDeposit);
            _customerService.UpdateCustomer(cust);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "SubscriptionOrder has been marked as paid",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            CheckSubscriptionOrderStatus(order);
   
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                ProcessSubscriptionOrderPaid(order);
            }
        }



        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanRefund(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Paid &&
                _paymentService.SupportRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }
        
        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Refund(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanRefund(order))
                throw new NopException("Cannot do refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.SubscriptionOrder = order;
                request.AmountToRefund = order.SubscriptionOrderTotal;
                request.IsPartialRefund = false;
                result = _paymentService.Refund(request);
                if (result.Success)
                {
                    //total amount refunded
                    decimal totalAmountRefunded = order.RefundedAmount + request.AmountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.PaymentStatus = result.NewPaymentStatus;
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    //add a note
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("SubscriptionOrder has been refunded. Amount = {0}", request.AmountToRefund),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    //check order status
                    CheckSubscriptionOrderStatus(order);

                    //notifications
                    var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedStoreOwnerNotification(order, request.AmountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"SubscriptionOrder refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", orderRefundedStoreOwnerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }
                    var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedCustomerNotification(order, request.AmountToRefund, order.CustomerLanguageId);
                    if (orderRefundedCustomerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"SubscriptionOrder refunded\" email (to customer) has been queued. Queued email identifier: {0}.", orderRefundedCustomerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }

                    //raise event       
                    _eventPublisher.Publish(new SubscriptionOrderRefundedEvent(order, request.AmountToRefund));
                }

            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Unable to refund order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                //log it
                string logError = string.Format("Error refunding order #{0}. Error: {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        public virtual bool CanRefundOffline(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
            //     return false;

            if (order.PaymentStatus == PaymentStatus.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        public virtual void RefundOffline(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanRefundOffline(order))
                throw new NopException("You can't refund this order");

            //amout to refund
            decimal amountToRefund = order.SubscriptionOrderTotal;

            //total amount refunded
            decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatus = PaymentStatus.Refunded;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = string.Format("SubscriptionOrder has been marked as refunded. Amount = {0}", amountToRefund),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            //check order status
            CheckSubscriptionOrderStatus(order);

            //notifications
            var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
            {
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("\"SubscriptionOrder refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", orderRefundedStoreOwnerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);
            }
            var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
            if (orderRefundedCustomerNotificationQueuedEmailId > 0)
            {
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("\"SubscriptionOrder refunded\" email (to customer) has been queued. Queued email identifier: {0}.", orderRefundedCustomerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);
            }

            //raise event       
            _eventPublisher.Publish(new SubscriptionOrderRefundedEvent(order, amountToRefund));
        }

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanPartiallyRefund(SubscriptionOrder order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
            //    return false;

            decimal canBeRefunded = order.SubscriptionOrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded) &&
                _paymentService.SupportPartiallyRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> PartiallyRefund(SubscriptionOrder order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanPartiallyRefund(order, amountToRefund))
                throw new NopException("Cannot do partial refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.SubscriptionOrder = order;
                request.AmountToRefund = amountToRefund;
                request.IsPartialRefund = true;

                result = _paymentService.Refund(request);

                if (result.Success)
                {
                    //total amount refunded
                    decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.PaymentStatus = result.NewPaymentStatus;
                    _subscriptionService.UpdateSubscriptionOrder(order);


                    //add a note
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("SubscriptionOrder has been partially refunded. Amount = {0}", amountToRefund),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    //check order status
                    CheckSubscriptionOrderStatus(order);

                    //notifications
                    var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"SubscriptionOrder refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", orderRefundedStoreOwnerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }
                    var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
                    if (orderRefundedCustomerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"SubscriptionOrder refunded\" email (to customer) has been queued. Queued email identifier: {0}.", orderRefundedCustomerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscriptionOrder(order);
                    }

                    //raise event       
                    _eventPublisher.Publish(new SubscriptionOrderRefundedEvent(order, amountToRefund));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Unable to partially refund order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                //log it
                string logError = string.Format("Error refunding order #{0}. Error: {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        public virtual bool CanPartiallyRefundOffline(SubscriptionOrder order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
            //    return false;

            decimal canBeRefunded = order.SubscriptionOrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public virtual void PartiallyRefundOffline(SubscriptionOrder order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            
            if (!CanPartiallyRefundOffline(order, amountToRefund))
                throw new NopException("You can't partially refund (offline) this order");

            //total amount refunded
            decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            //if (order.SubscriptionOrderTotal == totalAmountRefunded), then set order.PaymentStatus = PaymentStatus.Refunded;
            order.PaymentStatus = PaymentStatus.PartiallyRefunded;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = string.Format("SubscriptionOrder has been marked as partially refunded. Amount = {0}", amountToRefund),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            //check order status
            CheckSubscriptionOrderStatus(order);

            //notifications
            var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
            {
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("\"SubscriptionOrder refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", orderRefundedStoreOwnerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);
            }
            var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
            if (orderRefundedCustomerNotificationQueuedEmailId > 0)
            {
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("\"SubscriptionOrder refunded\" email (to customer) has been queued. Queued email identifier: {0}.", orderRefundedCustomerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);
            }
            //raise event       
            _eventPublisher.Publish(new SubscriptionOrderRefundedEvent(order, amountToRefund));
        }



        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public virtual bool CanVoid(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportVoid(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Voided order</returns>
        public virtual IList<string> Void(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanVoid(order))
                throw new NopException("Cannot do void for order.");

            var request = new VoidPaymentRequest();
            VoidPaymentResult result = null;
            try
            {
                request.SubscriptionOrder = order;
                result = _paymentService.Void(request);

                if (result.Success)
                {
                    //update order info
                    order.PaymentStatus = result.NewPaymentStatus;
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    //add a note
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = "SubscriptionOrder has been voided",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscriptionOrder(order);

                    //check order status
                    CheckSubscriptionOrderStatus(order);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new VoidPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Unable to voiding order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscriptionOrder(order);

                //log it
                string logError = string.Format("Error voiding order #{0}. Error: {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        public virtual bool CanVoidOffline(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.SubscriptionOrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.SubscriptionOrderStatus == SubscriptionOrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        public virtual void VoidOffline(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order.PaymentStatusId = (int)PaymentStatus.Voided;
            _subscriptionService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "SubscriptionOrder has been marked as voided",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscriptionOrder(order);

            //check orer status
            CheckSubscriptionOrderStatus(order);
        }



        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void ReSubscriptionOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //move shopping cart items (if possible)
            foreach (var orderItem in order.SubscriptionOrderItems)
            {
                _subscriptionCartService.AddToCart(order.Customer, orderItem.Plan,
                    SubscriptionCartType.SubscriptionCart, order.StoreId, 
                    orderItem.AttributesXml, orderItem.UnitPriceExclTax,
                    orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                    orderItem.Quantity, false);
            }

            //set checkout attributes
            //comment the code below if you want to disable this functionality
            _genericAttributeService.SaveAttribute(order.Customer, SystemCustomerAttributeNames.CheckoutAttributes, order.CheckoutAttributesXml, order.StoreId);
        }
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <returns>Result</returns>
        public virtual bool IsReturnRequestAllowed(SubscriptionOrder order)
        {
            if (!_orderSettings.ReturnRequestsEnabled)
                return false;

            if (order == null || order.Deleted)
                return false;

            if (order.SubscriptionOrderStatus != SubscriptionOrderStatus.Complete)
                return false;

            if (_orderSettings.NumberOfDaysReturnRequestAvailable == 0)
                return true;

            var daysPassed = (DateTime.UtcNow - order.CreatedOnUtc).TotalDays;
            return (daysPassed - _orderSettings.NumberOfDaysReturnRequestAvailable) < 0;
        }
        


        /// <summary>
        /// Valdiate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        public virtual bool ValidateMinSubscriptionOrderSubtotalAmount(IList<SubscriptionCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            //min order amount sub-total validation
            if (cart.Count > 0 && _orderSettings.MinSubscriptionOrderSubtotalAmount > decimal.Zero)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                Discount orderSubTotalAppliedDiscount;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                decimal subTotalRegistrationBase;
                decimal subTotalDepositBase;
                _orderTotalCalculationService.GetSubscriptionCartSubTotal(cart, _orderSettings.MinSubscriptionOrderSubtotalAmountIncludingTax,
                    out subTotalRegistrationBase, out subTotalDepositBase,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

                if (subTotalWithoutDiscountBase < _orderSettings.MinSubscriptionOrderSubtotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Valdiate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public virtual bool ValidateMinSubscriptionOrderTotalAmount(IList<SubscriptionCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (cart.Count > 0 && _orderSettings.MinSubscriptionOrderTotalAmount > decimal.Zero)
            {
                decimal? borrowCartTotalBase = _orderTotalCalculationService.GetSubscriptionCartTotal(cart);
                if (borrowCartTotalBase.HasValue && borrowCartTotalBase.Value < _orderSettings.MinSubscriptionOrderTotalAmount)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
