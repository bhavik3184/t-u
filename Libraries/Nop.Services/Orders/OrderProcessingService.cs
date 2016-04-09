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
    /// Order processing service
    /// </summary>
    public partial class OrderProcessingService : IOrderProcessingService
    {
        #region Fields
        
        private readonly ISubscriptionOrderService _subscriptionOrderService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IBorrowCartService _borrowCartService;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ITaxService _taxService;
        private readonly ICustomerService _customerService;
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
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionOrderService">Order service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="productService">Product service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="orderTotalCalculationService">Order total calculationservice</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="productAttributeFormatter">Product attribute formatter</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="borrowCartService">Shopping cart service</param>
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
        /// <param name="orderSettings">Order settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="currencySettings">Currency settings</param>
        public OrderProcessingService(ISubscriptionOrderService subscriptionOrderService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IProductService productService,
            IPaymentService paymentService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter,
            IBorrowCartService borrowCartService,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ITaxService taxService,
            ICustomerService customerService,
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
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings)
        {
            this._subscriptionOrderService = subscriptionOrderService;
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
            this._borrowCartService = borrowCartService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._taxService = taxService;
            this._customerService = customerService;
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

        protected class PlaceOrderContainter
        {
            public PlaceOrderContainter()
            {
                this.Cart = new List<BorrowCartItem>();
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

            public bool IsRecurringBorrowCart { get; set; }
            //initial order (used with recurring payments)
            public SubscriptionOrder InitialOrder { get; set; }

            public string CheckoutAttributeDescription { get; set; }
            public string CheckoutAttributesXml { get; set; }

            public IList<BorrowCartItem> Cart { get; set; }
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
        protected virtual PlaceOrderContainter PreparePlaceOrderDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceOrderContainter();

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
                details.CustomerCurrencyCode = details.InitialOrder.CustomerCurrencyCode;
                details.CustomerCurrencyRate = details.InitialOrder.CurrencyRate;
            }

            //customer language
            if (!processPaymentRequest.IsRecurringPayment)
            {
                details.CustomerLanguage = _languageService.GetLanguageById(details.Customer.GetAttribute<int>(
                    SystemCustomerAttributeNames.LanguageId, processPaymentRequest.StoreId));
            }
            else
            {
                details.CustomerLanguage = _languageService.GetLanguageById(details.InitialOrder.CustomerLanguageId);
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
                if (details.InitialOrder.BillingAddress == null)
                    throw new NopException("Billing address is not available");

                //clone billing address
                details.BillingAddress = (Address)details.InitialOrder.BillingAddress.Clone();
                if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                    throw new NopException(string.Format("Country '{0}' is not allowed for billing", details.BillingAddress.Country.Name));
            }

           

            //load and validate customer shopping cart
            if (!processPaymentRequest.IsRecurringPayment)
            {
                //load shopping cart
                details.Cart = details.Customer.BorrowCartItems
                    .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                    .LimitPerStore(processPaymentRequest.StoreId)
                    .ToList();

                if (details.Cart.Count == 0)
                    throw new NopException("Cart is empty");

                //validate the entire shopping cart
                var warnings = _borrowCartService.GetBorrowCartWarnings(details.Cart,
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
                    var sciWarnings = _borrowCartService.GetBorrowCartItemWarnings(details.Customer, sci.BorrowCartType,
                        sci.Product, processPaymentRequest.StoreId, sci.AttributesXml,
                        sci.CustomerEnteredPrice ,
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
                bool minOrderSubtotalAmountOk = ValidateMinOrderSubtotalAmount(details.Cart);
                if (!minOrderSubtotalAmountOk)
                {
                    decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                    throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false)));
                }
                bool minOrderTotalAmountOk = ValidateMinOrderTotalAmount(details.Cart);
                if (!minOrderTotalAmountOk)
                {
                    decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                    throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false)));
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
                details.CustomerTaxDisplayType = details.InitialOrder.CustomerTaxDisplayType;
            }
            
            //sub total
            if (!processPaymentRequest.IsRecurringPayment)
            {
                //sub total (incl tax)
                decimal orderSubTotalDiscountAmount1;
                Discount orderSubTotalAppliedDiscount1;
                decimal subTotalWithoutDiscountBase1;
                decimal subTotalWithDiscountBase1;
                _orderTotalCalculationService.GetBorrowCartSubTotal(details.Cart,
                    true, out orderSubTotalDiscountAmount1, out orderSubTotalAppliedDiscount1,
                    out subTotalWithoutDiscountBase1, out subTotalWithDiscountBase1);
                details.SubscriptionOrderSubTotalInclTax = subTotalWithoutDiscountBase1;
                details.SubscriptionOrderSubTotalDiscountInclTax = orderSubTotalDiscountAmount1;


                //sub total (excl tax)
                decimal orderSubTotalDiscountAmount2;
                Discount orderSubTotalAppliedDiscount2;
                decimal subTotalWithoutDiscountBase2;
                decimal subTotalWithDiscountBase2;
                _orderTotalCalculationService.GetBorrowCartSubTotal(details.Cart,
                    false, out orderSubTotalDiscountAmount2, out orderSubTotalAppliedDiscount2,
                    out subTotalWithoutDiscountBase2, out subTotalWithDiscountBase2);
                details.SubscriptionOrderSubTotalExclTax = subTotalWithoutDiscountBase2;
                details.SubscriptionOrderSubTotalDiscountExclTax = orderSubTotalDiscountAmount2;
            }
            else
            {
                details.SubscriptionOrderSubTotalInclTax = details.InitialOrder.SubscriptionOrderSubtotalInclTax;
                details.SubscriptionOrderSubTotalExclTax = details.InitialOrder.SubscriptionOrderSubtotalExclTax;
            }


            //shipping info
            bool borrowCartRequiresShipping;
            if (!processPaymentRequest.IsRecurringPayment)
            {
                borrowCartRequiresShipping = details.Cart.RequiresShipping();
            }
            else
            {
                borrowCartRequiresShipping = details.InitialOrder.ShippingStatus != ShippingStatus.ShippingNotRequired;
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
                    details.PickUpInStore = details.InitialOrder.PickUpInStore;
                    if (!details.PickUpInStore)
                    {
                        if (details.InitialOrder.ShippingAddress == null)
                            throw new NopException("Shipping address is not available");

                        //clone shipping address
                        details.ShippingAddress = (Address)details.InitialOrder.ShippingAddress.Clone();
                        if (details.ShippingAddress.Country != null && !details.ShippingAddress.Country.AllowsShipping)
                        {
                            throw new NopException(string.Format("Country '{0}' is not allowed for shipping", details.ShippingAddress.Country.Name));
                        }
                    }

                    details.ShippingMethodName = details.InitialOrder.ShippingMethod;
                    details.ShippingRateComputationMethodSystemName = details.InitialOrder.ShippingRateComputationMethodSystemName;
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
                decimal? orderShippingTotalInclTax = _orderTotalCalculationService.GetBorrowCartShippingTotal(details.Cart, true, out taxRate, out shippingTotalDiscount);
                decimal? orderShippingTotalExclTax = _orderTotalCalculationService.GetBorrowCartShippingTotal(details.Cart, false);
                if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
                    throw new NopException("Shipping total couldn't be calculated");
                details.SubscriptionOrderShippingTotalInclTax = orderShippingTotalInclTax.Value;
                details.SubscriptionOrderShippingTotalExclTax = orderShippingTotalExclTax.Value;

               
            }
            else
            {
                details.SubscriptionOrderShippingTotalInclTax = details.InitialOrder.SubscriptionOrderShippingInclTax;
                details.SubscriptionOrderShippingTotalExclTax = details.InitialOrder.SubscriptionOrderShippingExclTax;
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
                details.SubscriptionOrderTaxTotal = details.InitialOrder.SubscriptionOrderTax;
                //VAT number
                details.VatNumber = details.InitialOrder.VatNumber;
            }


            
                details.SubscriptionOrderDiscountAmount = details.InitialOrder.SubscriptionOrderDiscount;
                details.SubscriptionOrderTotal = details.InitialOrder.SubscriptionOrderTotal;
             
            
            //recurring or standard shopping cart?
            if (!processPaymentRequest.IsRecurringPayment)
            {
                details.IsRecurringBorrowCart = details.Cart.IsRecurring();
                if (details.IsRecurringBorrowCart)
                {
                    int recurringCycleLength;
                    RecurringProductCyclePeriod recurringCyclePeriod;
                    int recurringTotalCycles;
                    string recurringCyclesError = details.Cart.GetRecurringCycleInfo(_localizationService,
                        out recurringCycleLength, out recurringCyclePeriod, out recurringTotalCycles);
                    if (!string.IsNullOrEmpty(recurringCyclesError))
                        throw new NopException(recurringCyclesError);

                    processPaymentRequest.RecurringCycleLength = recurringCycleLength;
                    processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
                }
            }
            else
            {
                details.IsRecurringBorrowCart = true;
            }

            return details;
        }

        /// <summary>
        /// Award (earn) reward points (for placing a new order)
        /// </summary>
        /// <param name="order">Order</param>
        
         
        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="order">Order</param>
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
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //order notes, notifications
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Order status has been changed to {0}", os.ToString()),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);


            if (prevSubscriptionOrderStatus != SubscriptionOrderStatus.Complete &&
                os == SubscriptionOrderStatus.Complete
                && notifyCustomer)
            {
                //notification
                var orderCompletedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                    _pdfService.PrintOrderToPdf(order, 0) : null;
                var orderCompletedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                    "order.pdf" : null;
                int orderCompletedCustomerNotificationQueuedEmailId = _workflowMessageService
                    .SendOrderCompletedCustomerNotification(order, order.CustomerLanguageId, orderCompletedAttachmentFilePath,
                    orderCompletedAttachmentFileName);
                if (orderCompletedCustomerNotificationQueuedEmailId > 0)
                {
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("\"Order completed\" email (to customer) has been queued. Queued email identifier: {0}.", orderCompletedCustomerNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionOrderService.UpdateSubscriptionOrder(order);
                }
            }

            if (prevSubscriptionOrderStatus != SubscriptionOrderStatus.Cancelled &&
                os == SubscriptionOrderStatus.Cancelled
                && notifyCustomer)
            {
                //notification
                int orderCancelledCustomerNotificationQueuedEmailId = _workflowMessageService.SendOrderCancelledCustomerNotification(order, order.CustomerLanguageId);
                if (orderCancelledCustomerNotificationQueuedEmailId > 0)
                {
                    order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                    {
                        Note = string.Format("\"Order cancelled\" email (to customer) has been queued. Queued email identifier: {0}.", orderCancelledCustomerNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionOrderService.UpdateSubscriptionOrder(order);
                }
            }

            
        }

        /// <summary>
        /// Process order paid status
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ProcessOrderPaid(SubscriptionOrder order)
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

                var orderPaidAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                    _pdfService.PrintOrderToPdf(order, 0) : null;
                var orderPaidAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                    "order.pdf" : null;
                _workflowMessageService.SendOrderPaidCustomerNotification(order, order.CustomerLanguageId,
                    orderPaidAttachmentFilePath, orderPaidAttachmentFileName);

                _workflowMessageService.SendOrderPaidStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
                var vendors = GetVendorsInOrder(order);
                foreach (var vendor in vendors)
                {
                    _workflowMessageService.SendOrderPaidVendorNotification(order, vendor, _localizationSettings.DefaultAdminLanguageId);
                }
                //TODO add "order paid email sent" order note
            }

            //customer roles with "purchased with product" specified
            ProcessCustomerRolesWithPurchasedProductSpecified(order, true);
        }

        /// <summary>
        /// Process customer roles with "Purchased with Product" property configured
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="add">A value indicating whether to add configured customer role; true - add, false - remove</param>
        protected virtual void ProcessCustomerRolesWithPurchasedProductSpecified(SubscriptionOrder order, bool add)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //purchased product identifiers
            var purchasedProductIds = new List<int>();
            foreach (var orderItem in order.OrderItems)
            {
                foreach (var itemDetail in orderItem.ItemDetails)
                {
                    //standard items
                    purchasedProductIds.Add(itemDetail.ProductId);

                    //bundled (associated) products
                    var attributeValues = _productAttributeParser.ParseProductAttributeValues(itemDetail.AttributesXml);
                    foreach (var attributeValue in attributeValues)
                    {
                        if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                        {
                            purchasedProductIds.Add(attributeValue.AssociatedProductId);
                        }
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
        /// <param name="order">Order</param>
        /// <returns>Vendors</returns>
        protected virtual IList<Vendor> GetVendorsInOrder(SubscriptionOrder order)
        {
            var vendors = new List<Vendor>();
            

            return vendors;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Validated order</returns>
        public virtual void CheckSubscriptionOrderStatus(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.PaymentStatus == PaymentStatus.Paid && !order.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                order.PaidDateUtc = DateTime.UtcNow;
                _subscriptionOrderService.UpdateSubscriptionOrder(order);
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
                    if (order.ShippingStatus == ShippingStatus.ShippingNotRequired || order.ShippingStatus == ShippingStatus.Delivered)
                    {
                        SetSubscriptionOrderStatus(order, SubscriptionOrderStatus.Complete, true);
                    }
                }
            }
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        public virtual PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest)
        {
            //think about moving functionality of processing recurring orders (after the initial order was placed) to ProcessNextRecurringPayment() method
            if (processPaymentRequest == null)
                throw new ArgumentNullException("processPaymentRequest");

            var result = new PlaceOrderResult();
            try
            {
                 

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);

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
                        if (details.IsRecurringBorrowCart)
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
                        if (details.IsRecurringBorrowCart)
                        {
                            //Old credit card info
                            processPaymentRequest.CreditCardType = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardType) : "";
                            processPaymentRequest.CreditCardName = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardName) : "";
                            processPaymentRequest.CreditCardNumber = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardNumber) : "";
                            processPaymentRequest.CreditCardCvv2 = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardCvv2) : "";
                            try
                            {
                                processPaymentRequest.CreditCardExpireMonth = details.InitialOrder.AllowStoringCreditCardNumber ? Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationMonth)) : 0;
                                processPaymentRequest.CreditCardExpireYear = details.InitialOrder.AllowStoringCreditCardNumber ? Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationYear)) : 0;
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
                        //SubscriptionOrderGuid = processPaymentRequest.SecurityDeposit,
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
                    _subscriptionOrderService.InsertSubscriptionOrder(order);

                    result.PlacedOrder = order;

                    if (!processPaymentRequest.IsRecurringPayment)
                    {
                        //move shopping cart items to order items
                        foreach (var sc in details.Cart)
                        {
                            //prices
                            decimal taxRate;
                            //Discount scDiscount;
                            //decimal discountAmount;
                            decimal scUnitPrice = decimal.Zero;
                            decimal scSubTotal = decimal.Zero;
                            decimal scUnitPriceInclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.Customer, out taxRate);
                            decimal scUnitPriceExclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.Customer, out taxRate);
                            decimal scSubTotalInclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.Customer, out taxRate);
                            decimal scSubTotalExclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.Customer, out taxRate);

                            decimal discountAmountInclTax = _taxService.GetProductPrice(sc.Product,   decimal.Zero, out taxRate);
                            decimal discountAmountExclTax = _taxService.GetProductPrice(sc.Product, decimal.Zero, out taxRate);
                          

                            //attributes
                            string attributeDescription = _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.Customer);

                            var itemWeight = _shippingService.GetBorrowCartItemWeight(sc);

                            //save order item
                            var orderItem = new OrderItem
                            {
                                OrderItemGuid = Guid.NewGuid(),
                                SubscriptionOrder = order,
                                //ProductId = sc.ProductId,
                                //UnitPriceInclTax = scUnitPriceInclTax,
                                //UnitPriceExclTax = scUnitPriceExclTax,
                                //PriceInclTax = scSubTotalInclTax,
                                //PriceExclTax = scSubTotalExclTax,
                                //OriginalProductCost = _priceCalculationService.GetProductCost(sc.Product, sc.AttributesXml),
                                //AttributeDescription = attributeDescription,
                                //AttributesXml = sc.AttributesXml,
                                //Quantity = sc.Quantity,
                                //DiscountAmountInclTax = discountAmountInclTax,
                                //DiscountAmountExclTax = discountAmountExclTax,
                                //DownloadCount = 0,
                                //IsDownloadActivated = false,
                                //LicenseDownloadId = 0,
                                //ItemWeight = itemWeight,
                                //RentalStartDateUtc = sc.RentalStartDateUtc,
                                //RentalEndDateUtc = sc.RentalEndDateUtc
                            };
                            order.OrderItems.Add(orderItem);
                            _subscriptionOrderService.UpdateSubscriptionOrder(order);

                          

                            //inventory
                            _productService.AdjustInventory(sc.Product, -sc.Quantity, sc.AttributesXml);
                        }

                        //clear shopping cart
                        details.Cart.ToList().ForEach(sci => _borrowCartService.DeleteBorrowCartItem(sci, false));
                    }
                    else
                    {
                        //recurring payment
                        var initialOrderItems = details.InitialOrder.OrderItems;
                        foreach (var orderItem in initialOrderItems)
                        {
                            //save item
                            var newOrderItem = new OrderItem
                            {
                                OrderItemGuid = Guid.NewGuid(),
                                SubscriptionOrder = order,
                                //UnitPriceInclTax = orderItem.UnitPriceInclTax,
                                //UnitPriceExclTax = orderItem.UnitPriceExclTax,
                                //PriceInclTax = orderItem.PriceInclTax,
                                //PriceExclTax = orderItem.PriceExclTax,
                                //OriginalProductCost = orderItem.OriginalProductCost,
                                //AttributeDescription = orderItem.AttributeDescription,
                                //AttributesXml = orderItem.AttributesXml,
                                //Quantity = orderItem.Quantity,
                                //DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
                                //DiscountAmountExclTax = orderItem.DiscountAmountExclTax,
                                
                                //ItemWeight = orderItem.ItemWeight,
                                //RentalStartDateUtc = orderItem.RentalStartDateUtc,
                                //RentalEndDateUtc = orderItem.RentalEndDateUtc
                            };
                            order.OrderItems.Add(newOrderItem);
                            _subscriptionOrderService.UpdateSubscriptionOrder(order);

                            

                            //inventory
                            //_productService.AdjustInventory(orderItem.Product, -orderItem.Quantity, orderItem.AttributesXml);
                        }
                    }

                   

                    //reward points history
                    if (details.RedeemedRewardPointsAmount > decimal.Zero)
                    {
                        _rewardPointService.AddRewardPointsHistoryEntry(details.Customer,
                            -details.RedeemedRewardPoints, order.StoreId,
                            string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.Id),
                            order, details.RedeemedRewardPointsAmount);
                        _customerService.UpdateCustomer(details.Customer);
                    }

                    //recurring orders
                     

                    #endregion

                    #region Notifications & notes

                    //notes, messages
                    if (_workContext.OriginalCustomerIfImpersonated != null)
                    {
                        //this order is placed by a store administrator impersonating a customer
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("Order placed by a store owner ('{0}'. ID = {1}) impersonating the customer.",
                                _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionOrderService.UpdateSubscriptionOrder(order);
                    }
                    else
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = "Order placed",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionOrderService.UpdateSubscriptionOrder(order);
                    }


                    //send email notifications
                    int orderPlacedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderPlacedStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
                    if (orderPlacedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"Order placed\" email (to store owner) has been queued. Queued email identifier: {0}.", orderPlacedStoreOwnerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionOrderService.UpdateSubscriptionOrder(order);
                    }

                    var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                        _pdfService.PrintOrderToPdf(order, order.CustomerLanguageId) : null;
                    var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                        "order.pdf" : null;
                    int orderPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
                        .SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
                    if (orderPlacedCustomerNotificationQueuedEmailId > 0)
                    {
                        order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                        {
                            Note = string.Format("\"Order placed\" email (to customer) has been queued. Queued email identifier: {0}.", orderPlacedCustomerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionOrderService.UpdateSubscriptionOrder(order);
                    }

                    var vendors = GetVendorsInOrder(order);
                    foreach (var vendor in vendors)
                    {
                        int orderPlacedVendorNotificationQueuedEmailId = _workflowMessageService.SendOrderPlacedVendorNotification(order, vendor, order.CustomerLanguageId);
                        if (orderPlacedVendorNotificationQueuedEmailId > 0)
                        {
                            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                            {
                                Note = string.Format("\"Order placed\" email (to vendor) has been queued. Queued email identifier: {0}.", orderPlacedVendorNotificationQueuedEmailId),
                                DisplayToCustomer = false,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                            _subscriptionOrderService.UpdateSubscriptionOrder(order);
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
                            "PublicStore.PlaceOrder",
                            _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"),
                            order.Id);
                    }
                    
                    //raise event       
                    _eventPublisher.Publish(new SubscriptionOrderPlacedEvent(order));

                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        ProcessOrderPaid(order);
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


        public virtual int PlaceBorrowItem(List<BorrowCartItem> scList)
        {
            //think about moving functionality of processing recurring orders (after the initial order was placed) to ProcessNextRecurringPayment() method
            int success = 0;
            if (scList.Count() == 0)
            {
                success = 0; 
            }
            else { 
                var result = new PlaceOrderResult();
                var order = _subscriptionOrderService.GetCurrentSubscribedOrder(scList.FirstOrDefault().CustomerId);
                //int batchId = _subscriptionOrderService.GetOrderItemBatchId(order.Id);
                //if (batchId == 0)
                //    batchId = 1;
              
                    try
                    {  
                        #region Save order details
                        if (order != null) { 
                            //prices
                            //decimal taxRate;
                            //Discount scDiscount;
                            //decimal discountAmount;
                            //decimal scUnitPrice = decimal.Zero;
                            //decimal scSubTotal = decimal.Zero;
                            decimal scUnitPriceInclTax = decimal.Zero;
                            decimal scUnitPriceExclTax = decimal.Zero;
                            decimal scSubTotalInclTax = decimal.Zero;
                            decimal scSubTotalExclTax = decimal.Zero;
                            decimal discountAmountInclTax = decimal.Zero;
                            decimal discountAmountExclTax = decimal.Zero;

                            //save order item
                            var orderItem = new OrderItem
                            {
                                OrderItemGuid = Guid.NewGuid(),
                                SubscriptionOrder = order,
                                //ProductId = sc.ProductId,
                                //UnitPriceInclTax = scUnitPriceInclTax,
                                //UnitPriceExclTax = scUnitPriceExclTax,
                                //PriceInclTax = scSubTotalInclTax,
                                //PriceExclTax = scSubTotalExclTax,
                                //OriginalProductCost = decimal.Zero,
                                //AttributeDescription = attributeDescription,
                                //BatchId = batchId,
                                ////Quantity = sc.Quantity,
                                //DiscountAmountInclTax = discountAmountInclTax,
                                //DiscountAmountExclTax = discountAmountExclTax,
                                //DownloadCount = 0,
                                //IsDownloadActivated = false,
                                //LicenseDownloadId = 0,
                                //ItemWeight = itemWeight,
                                //RentalStartDateUtc = sc.RentalStartDateUtc,
                                //RentalEndDateUtc = sc.RentalEndDateUtc
                                UpdatedOnUtc = DateTime.UtcNow,
                                CreatedOnUtc = DateTime.UtcNow,
                            };
                           
                        
                              foreach (BorrowCartItem sc in scList) {
                                  string attributeDescription = _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, sc.Customer);

                                  var itemWeight = _shippingService.GetBorrowCartItemWeight(sc);
                                  var itemDetail = new ItemDetail
                                  {   //attributes
                                      //OrderItemGuid = Guid.NewGuid(),
                                      OrderItem = orderItem,
                                      ProductId = sc.ProductId,
                                      UnitPriceInclTax = scUnitPriceInclTax,
                                      UnitPriceExclTax = scUnitPriceExclTax,
                                      PriceInclTax = scSubTotalInclTax,
                                      PriceExclTax = scSubTotalExclTax,
                                      OriginalProductCost = decimal.Zero,
                                      AttributeDescription = attributeDescription,
                                     // BatchId = batchId,
                                      //Quantity = sc.Quantity,
                                      DiscountAmountInclTax = discountAmountInclTax,
                                      DiscountAmountExclTax = discountAmountExclTax,
                                      AttributesXml = sc.AttributesXml,
                                     // BatchId = batchId,
                                      Quantity = sc.Quantity,
                                      //DiscountAmountInclTax = discountAmountInclTax,
                                      //DiscountAmountExclTax = discountAmountExclTax,
                                      ItemWeight = itemWeight,
                                      //RentalStartDateUtc = sc.RentalStartDateUtc,
                                      //RentalEndDateUtc = sc.RentalEndDateUtc
                                      UpdatedOnUtc = DateTime.UtcNow,
                                      CreatedOnUtc = DateTime.UtcNow,
                                  };
                                  orderItem.ItemDetails.Add(itemDetail);

                                  #region Process errors

                                  string error = "";
                                  for (int i = 0; i < result.Errors.Count; i++)
                                  {
                                      error += string.Format("Error {0}: {1}", i + 1, result.Errors[i]);
                                      if (i != result.Errors.Count - 1)
                                          error += ". ";
                                      success = 0;
                                  }
                                  if (!String.IsNullOrEmpty(error))
                                  {
                                      //log it
                                      string logError = string.Format("Error while Borrwoing Item. {0}", error);
                                      var customer = _customerService.GetCustomerById(sc.CustomerId);
                                      _logger.Error(logError, customer: customer);
                                      success = 0;
                                  }

                                  #endregion
                              }
                              order.OrderItems.Add(orderItem);

                            _subscriptionOrderService.UpdateSubscriptionOrder(order);
                            success = 1;

                            
                            #endregion
                        }
                        else
                        {
                            success = 99;
                        }
                    }
            
                    catch (Exception exc)
                    {
                        _logger.Error(exc.Message, exc);
                        result.AddError(exc.Message);
                        success = 0;
                    }

                if (success == 1) { 
                    order.NoOfDeliveries = order.NoOfDeliveries + 1;
                    _subscriptionOrderService.UpdateSubscriptionOrder(order);
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteOrder(SubscriptionOrder order)
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

                //cancel recurring payments
              
                //Adjust inventory for already shipped shipments
                //only products with "use multiple warehouses"
                var orderItems = order.OrderItems;
                foreach (var orderItem in orderItems)
                {
                    foreach (var itemDetail in orderItem.ItemDetails)
                    {
                        foreach (var shipment in orderItem.Shipments)
                        {
                            foreach (var shipmentItem in shipment.ShipmentItems)
                            {
                                var orderItem1 = _subscriptionOrderService.GetOrderItemById(shipmentItem.ItemDetailId);
                                if (orderItem == null)
                                    continue;

                                _productService.ReverseBookedInventory(itemDetail.Product, shipmentItem);
                            }
                        }
                        _productService.AdjustInventory(itemDetail.Product, itemDetail.Quantity, itemDetail.AttributesXml);
                    }
                }
            }

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Order has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);
            
            //now delete an order
            _subscriptionOrderService.DeleteSubscriptionOrder(order);
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

            var order = _subscriptionOrderService.GetOrderById(shipment.OrderItem.SubscriptionOrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is already shipped");

            shipment.ShippedDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            //process products with "Multiple warehouse" support enabled
            foreach (var item in shipment.ShipmentItems)
            {
                var orderItem = _subscriptionOrderService.GetOrderItemById(item.ItemDetailId);
                foreach (var itemDetail in orderItem.ItemDetails)
                {
                    _productService.BookReservedInventory(itemDetail.Product, item.WarehouseId, -item.Quantity);
                }
            }

            //check whether we have more items to ship
            if (order.HasItemsToAddToShipment() || order.HasItemsToShip())
                order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
            else
                order.ShippingStatusId = (int)ShippingStatus.Shipped;
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
                {
                    Note = string.Format("Shipment# {0} has been sent", shipment.Id),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

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
                    _subscriptionOrderService.UpdateSubscriptionOrder(order);
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
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = string.Format("Shipment# {0} has been delivered", shipment.Id),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

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
                    _subscriptionOrderService.UpdateSubscriptionOrder(order);
                }
            }

            //event
            _eventPublisher.PublishShipmentDelivered(shipment);

            //check order status
            CheckSubscriptionOrderStatus(order);
        }



        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public virtual bool CanCancelOrder(SubscriptionOrder order)
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
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void CancelOrder(SubscriptionOrder order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanCancelOrder(order))
                throw new NopException("Cannot do cancel for order.");

            //Cancel order
            SetSubscriptionOrderStatus(order, SubscriptionOrderStatus.Cancelled, notifyCustomer);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Order has been cancelled",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

          

            //Adjust inventory for already shipped shipments
            //only products with "use multiple warehouses"
            foreach (var orderItem in order.OrderItems)
            {
                foreach (var shipment in orderItem.Shipments)
                {
                    foreach (var shipmentItem in shipment.ShipmentItems)
                    {
                        var orderItem1 = _subscriptionOrderService.GetOrderItemById(shipmentItem.ItemDetailId);
                        if (orderItem1 == null)
                            continue;
                        foreach (var itemDetail in orderItem.ItemDetails)
                        {
                            _productService.ReverseBookedInventory(itemDetail.Product, shipmentItem);
                            _productService.AdjustInventory(itemDetail.Product, itemDetail.Quantity, itemDetail.AttributesXml);
                        }
                    }
                   
                }
            //Adjust inventory
           
                
            }

            _eventPublisher.Publish(new SubscriptionOrderCancelledEvent(order));

        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public virtual bool CanMarkOrderAsAuthorized(SubscriptionOrder order)
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
        /// <param name="order">Order</param>
        public virtual void MarkAsAuthorized(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.PaymentStatusId = (int)PaymentStatus.Authorized;
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Order has been marked as authorized",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //check order status
            CheckSubscriptionOrderStatus(order);
        }



        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
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
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public virtual bool CanMarkOrderAsPaid(SubscriptionOrder order)
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
        /// <param name="order">Order</param>
        public virtual void MarkOrderAsPaid(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanMarkOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            order.PaymentStatusId = (int)PaymentStatus.Paid;
            order.PaidDateUtc = DateTime.UtcNow;
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Order has been marked as paid",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            CheckSubscriptionOrderStatus(order);
   
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                ProcessOrderPaid(order);
            }
        }



        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
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
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
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
        /// <param name="order">Order</param>
        public virtual void VoidOffline(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order.PaymentStatusId = (int)PaymentStatus.Voided;
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //add a note
            order.SubscriptionOrderNotes.Add(new SubscriptionOrderNote
            {
                Note = "Order has been marked as voided",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionOrderService.UpdateSubscriptionOrder(order);

            //check orer status
            CheckSubscriptionOrderStatus(order);
        }



        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void ReOrder(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //move shopping cart items (if possible)
            foreach (var orderItem in order.OrderItems)
            {
                foreach (var itemDetail in orderItem.ItemDetails)
                {
                    _borrowCartService.AddToCart(order.Customer, itemDetail.Product,
                        BorrowCartType.BorrowCart, order.StoreId,
                        itemDetail.AttributesXml, itemDetail.UnitPriceExclTax,
                        itemDetail.Quantity, false);
                }
            }
            //set checkout attributes
            //comment the code below if you want to disable this functionality
            _genericAttributeService.SaveAttribute(order.Customer, SystemCustomerAttributeNames.CheckoutAttributes, order.CheckoutAttributesXml, order.StoreId);
        }
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public virtual bool IsReturnRequestAllowed(SubscriptionOrder order)
        {
            if (!_orderSettings.ReturnRequestsEnabled)
                return false;

            if (order == null || order.Deleted)
                return false;

            //if (order.SubscriptionOrderStatus != SubscriptionOrderStatus.Cancelled)
            //    return false;

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
        public virtual bool ValidateMinOrderSubtotalAmount(IList<BorrowCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            //min order amount sub-total validation
            if (cart.Count > 0 && _orderSettings.MinOrderSubtotalAmount > decimal.Zero)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                Discount orderSubTotalAppliedDiscount;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                _orderTotalCalculationService.GetBorrowCartSubTotal(cart, _orderSettings.MinOrderSubtotalAmountIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

                if (subTotalWithoutDiscountBase < _orderSettings.MinOrderSubtotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Valdiate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public virtual bool ValidateMinOrderTotalAmount(IList<BorrowCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (cart.Count > 0 && _orderSettings.MinOrderTotalAmount > decimal.Zero)
            {
                decimal? borrowCartTotalBase = _orderTotalCalculationService.GetBorrowCartTotal(cart,false);
                if (borrowCartTotalBase.HasValue && borrowCartTotalBase.Value < _orderSettings.MinOrderTotalAmount)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
