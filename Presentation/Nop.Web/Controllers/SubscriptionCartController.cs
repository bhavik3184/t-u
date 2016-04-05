using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.SubscriptionCart;
using Nop.Web.Models.Checkout;
using Nop.Core.Domain.Payments;

namespace Nop.Web.Controllers
{
    public partial class SubscriptionCartController : BasePublicController
    {
		#region Fields

        private readonly IPlanService _planService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISubscriptionCartService _subscriptionCartService;
        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPlanAttributeFormatter _planAttributeFormatter;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly ISubscriptionOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly ISubscriptionOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly HttpContextBase _httpContext;

        private readonly MediaSettings _mediaSettings;
        private readonly SubscriptionCartSettings _borrowCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly SubscriptionOrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ISubscriptionOrderService _subscriptionOrderService;
        private readonly ILogger _logger;
        private readonly IRewardPointService _rewardPointService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPluginFinder _pluginFinder;

        #endregion

		#region Constructors

        public SubscriptionCartController(IPlanService planService, 
            IStoreContext storeContext,
            IWorkContext workContext,
            ISubscriptionCartService subscriptionCartService,
            ISubscriptionOrderService subscriptionService,
            ISubscriptionOrderProcessingService orderProcessingService,
            IPictureService pictureService,
            ILocalizationService localizationService, 
            IPlanAttributeFormatter planAttributeFormatter,
            ITaxService taxService, ICurrencyService currencyService, 
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter, 
            IDiscountService discountService,
            ICustomerService customerService, 
            IGiftCardService giftCardService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService, 
            ISubscriptionOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService, 
            IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService, 
            IDownloadService downloadService,
            ICacheManager cacheManager,
            IWebHelper webHelper, 
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            HttpContextBase httpContext,
            MediaSettings mediaSettings,
            SubscriptionCartSettings borrowCartSettings,
            CatalogSettings catalogSettings, 
            SubscriptionOrderSettings orderSettings,
            ShippingSettings shippingSettings, 
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings, 
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            ISubscriptionOrderService subscriptionOrderService,
            ILogger logger,
            IRewardPointService rewardPointService,
            PaymentSettings paymentSettings,
            IPluginFinder pluginFinder
            )
        {
            this._planService = planService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._subscriptionCartService = subscriptionCartService;
            this._subscriptionOrderService = subscriptionService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._planAttributeFormatter = planAttributeFormatter;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._giftCardService = giftCardService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._paymentService = paymentService;
            this._workflowMessageService = workflowMessageService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._httpContext = httpContext;

            this._mediaSettings = mediaSettings;
            this._borrowCartSettings = borrowCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._rewardPointService = rewardPointService;
            this._subscriptionOrderService =subscriptionOrderService;
            this._logger = logger;
            this._paymentSettings = paymentSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(SubscriptionCartItem sci,
            int pictureSize, bool showDefaultPicture, string planName)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
             
            return null;
        }

        [NonAction]
        protected virtual bool IsPaymentWorkflowRequired(IList<SubscriptionCartItem> cart, bool ignoreRewardPoints = false)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? borrowCartTotalBase = _orderTotalCalculationService.GetSubscriptionCartTotal(cart, ignoreRewardPoints);
            if (borrowCartTotalBase.HasValue && borrowCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }
        
        /// <summary>
        /// Prepare shopping cart model
        /// </summary>
        /// <param name="model">Model instance</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="isEditable">A value indicating whether cart is editable</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether we should validate checkout attributes when preparing the model</param>
        /// <param name="prepareEstimateShippingIfEnabled">A value indicating whether we should prepare "Estimate shipping" model</param>
        /// <param name="setEstimateShippingDefaultAddress">A value indicating whether we should prefill "Estimate shipping" model with the default customer address</param>
        /// <param name="prepareAndDisplayOrderReviewData">A value indicating whether we should prepare review data (such as billing/shipping address, payment or shipping data entered during checkout)</param>
        /// <returns>Model</returns>
        [NonAction]
        protected virtual void PrepareSubscriptionCartModel(SubscriptionCartModel model, 
            IList<SubscriptionCartItem> cart, bool isEditable = true, 
            bool validateCheckoutAttributes = false, 
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");
            
            model.OnePageCheckoutEnabled = true;

            if (cart.Count == 0)
                return;
            
            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowPlanImages = _borrowCartSettings.ShowProductImagesOnSubscriptionCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
            bool minSubscriptionOrderSubtotalAmountOk = _orderProcessingService.ValidateMinSubscriptionOrderSubtotalAmount(cart);
            if (!minSubscriptionOrderSubtotalAmountOk)
            {
                decimal minSubscriptionOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinSubscriptionOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinSubscriptionOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinSubscriptionOrderSubtotalAmount"), _priceFormatter.FormatPrice(minSubscriptionOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnSubscriptionCartPage = _orderSettings.TermsOfServiceOnSubscriptionCartPage;
            model.TermsOfServiceOnSubscriptionOrderConfirmPage = _orderSettings.TermsOfServiceOnSubscriptionOrderConfirmPage;
            model.DisplayTaxShippingInfo = false;

            //gift card and gift card boxes
            model.DiscountBox.Display= true;
            var discountCouponCode = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox.Display = true;

            //cart warnings
            var cartWarnings = _subscriptionCartService.GetSubscriptionCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);
            
            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new SubscriptionCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new SubscriptionCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attributeValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                var selectedCheckoutAttributes = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Count > 0)
                            {
                                DateTime selectedDate;
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }
                            
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var downloadGuidStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                                Guid downloadGuid;
                                Guid.TryParse(downloadGuidStr, out downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                    attributeModel.DefaultValue = download.DownloadGuid.ToString();
                            }
                        }
                        break;
                    default:
                        break;
                }

                model.CheckoutAttributes.Add(attributeModel);
            }

            #endregion 

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                model.EstimateShipping.Enabled = cart.Count > 0 && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
                if (model.EstimateShipping.Enabled)
                {
                    //countries
                    int? defaultEstimateCountryId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                    foreach (var c in _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id))
                        model.EstimateShipping.AvailableCountries.Add(new SelectListItem
                        {
                            Text = c.GetLocalized(x => x.Name),
                            Value = c.Id.ToString(),
                            Selected = c.Id == defaultEstimateCountryId
                        });
                    //states
                    int? defaultEstimateStateId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
                    var states = defaultEstimateCountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId.Value, _workContext.WorkingLanguage.Id).ToList() : new List<StateProvince>();
                    if (states.Count > 0)
                        foreach (var s in states)
                            model.EstimateShipping.AvailableStates.Add(new SelectListItem
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = s.Id == defaultEstimateStateId
                            });
                    else
                        model.EstimateShipping.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                    if (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null)
                        model.EstimateShipping.ZipPostalCode = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode;
                }
            }

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new SubscriptionCartModel.SubscriptionCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Plan.Sku,
                    PlanId = sci.Plan.Id,
                    PlanName = sci.Plan.GetLocalized(x => x.Name),
                    PlanSeName = sci.Plan.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _planAttributeFormatter.FormatAttributes(sci.Plan, sci.AttributesXml),
                    
                };



                //allow editing?
                //1. setting enabled?
                //2. simple plan?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _borrowCartSettings.AllowCartItemEditing && 
                    sci.Plan.PlanType == PlanType.SimplePlan &&
                    (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Plan.IsGiftCard) &&
                    sci.Plan.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = sci.Plan.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }
                
                //recurring info
                if (sci.Plan.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("SubscriptionCart.RecurringPeriod"), sci.Plan.RecurringCycleLength, sci.Plan.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Plan.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Plan.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Plan.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("SubscriptionCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Plan.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Plans.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal borrowCartUnitPriceWithDiscountBase = _taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal borrowCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(borrowCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Plan.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Plans.CallForPrice");
                }
                else
                {
                    //sub total
                    Discount scDiscount;
                    decimal borrowCartItemDiscountBase;
                    decimal taxRate;
                    decimal borrowCartItemSubTotalWithDiscountBase = _taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetSubTotal(sci, true, out borrowCartItemDiscountBase, out scDiscount), out taxRate);
                    decimal borrowCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(borrowCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscount != null)
                    {
                        borrowCartItemDiscountBase = _taxService.GetPlanPrice(sci.Plan, borrowCartItemDiscountBase, out taxRate);
                        if (borrowCartItemDiscountBase > decimal.Zero)
                        {
                            decimal borrowCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(borrowCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_borrowCartSettings.ShowProductImagesInMiniSubscriptionCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.PlanName);
                }

                //item warnings
                var itemWarnings = _subscriptionCartService.GetSubscriptionCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.SubscriptionCartType,
                    sci.Plan,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Button payment methods

            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                pm.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);

                model.ButtonPaymentMethodActionNames.Add(actionName);
                model.ButtonPaymentMethodControllerNames.Add(controllerName);
                model.ButtonPaymentMethodRouteValues.Add(routeValues);
            }

            #endregion

            #region SubscriptionOrder review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.SubscriptionOrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                    model.SubscriptionOrderReviewData.BillingAddress.PrepareModel(
                        address: billingAddress, 
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);
               
                //shipping info
                if (cart.RequiresShipping())
                {
                    model.SubscriptionOrderReviewData.IsShippable = true;

                    if (_shippingSettings.AllowPickUpInStore)
                    {
                        model.SubscriptionOrderReviewData.SelectedPickUpInStore = _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.SelectedPickUpInStore, _storeContext.CurrentStore.Id);
                    }

                    if (!model.SubscriptionOrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            model.SubscriptionOrderReviewData.ShippingAddress.PrepareModel(
                                address: shippingAddress, 
                                excludeProperties: false,
                                addressSettings: _addressSettings,
                                addressAttributeFormatter: _addressAttributeFormatter);
                        }
                    }
                    
                    
                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.SubscriptionOrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.SubscriptionOrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";

                //custom values
                var processPaymentRequest = _httpContext.Session["SubscriptionOrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest != null)
                {
                    model.SubscriptionOrderReviewData.CustomValues = processPaymentRequest.CustomValues;
                }
            }
            #endregion
        }

        [NonAction]
        protected virtual void PrepareMyToyBoxModel(MyToyBoxModel model,
            IList<SubscriptionCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailMyToyBoxEnabled = _borrowCartSettings.EmailMyToyBoxEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = true;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoMyToyBox;

            if (cart.Count == 0)
                return;

            #region Simple properties

            var customer = cart.GetCustomer();
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = customer.GetFullName();
            model.ShowPlanImages = _borrowCartSettings.ShowProductImagesOnSubscriptionCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            
            //cart warnings
            var cartWarnings = _subscriptionCartService.GetSubscriptionCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion
            
            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new MyToyBoxModel.SubscriptionCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Plan.Sku,
                    PlanId = sci.Plan.Id,
                    PlanName = sci.Plan.GetLocalized(x => x.Name),
                    PlanSeName = sci.Plan.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = "",
                };

                //allowed quantities
                var allowedQuantities = sci.Plan.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }
                

                //recurring info
                if (sci.Plan.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("SubscriptionCart.RecurringPeriod"), sci.Plan.RecurringCycleLength, sci.Plan.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Plan.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Plan.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Plan.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("SubscriptionCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Plan.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Plans.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal borrowCartUnitPriceWithDiscountBase = _taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal borrowCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(borrowCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Plan.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Plans.CallForPrice");
                }
                else
                {
                    //sub total
                    Discount scDiscount;
                    decimal borrowCartItemDiscountBase;
                    decimal taxRate;
                    decimal borrowCartItemSubTotalWithDiscountBase = _taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetSubTotal(sci, true, out borrowCartItemDiscountBase, out scDiscount), out taxRate);
                    decimal borrowCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(borrowCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscount != null)
                    {
                        borrowCartItemDiscountBase = _taxService.GetPlanPrice(sci.Plan, borrowCartItemDiscountBase, out taxRate);
                        if (borrowCartItemDiscountBase > decimal.Zero)
                        {
                            decimal borrowCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(borrowCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_borrowCartSettings.ShowProductImagesOnSubscriptionCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.PlanName);
                }

                //item warnings
                var itemWarnings = _subscriptionCartService.GetSubscriptionCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.SubscriptionCartType,
                    sci.Plan,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion
        }

        [NonAction]
        protected virtual MiniSubscriptionCartModel PrepareMiniSubscriptionCartModel()
        {
            var model = new MiniSubscriptionCartModel
            {
                ShowPlanImages = _borrowCartSettings.ShowProductImagesInMiniSubscriptionCart,
                //let's always display it
                DisplaySubscriptionCartButton = true,
                CurrentCustomerIsGuest = _workContext.CurrentCustomer.IsGuest(),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };


            //performance optimization (use "HasSubscriptionCartItems" property)
            if (_workContext.CurrentCustomer.HasSubscriptionCartItems)
            {
                var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                    .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                model.TotalPlans = cart.GetTotalProducts();
                if (cart.Count > 0)
                {
                    //subtotal
                    decimal orderSubTotalDiscountAmountBase;
                    Discount orderSubTotalAppliedDiscount;
                    decimal subTotalWithoutDiscountBase;
                    decimal subTotalWithDiscountBase;
                    decimal subTotalRegistration;
                    decimal subTotalDeposit;
                    var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionOrderSubtotal;
                    _orderTotalCalculationService.GetSubscriptionCartSubTotal(cart, subTotalIncludingTax,
                    out subTotalRegistration, out subTotalDeposit,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                        out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                    decimal subtotalBase = subTotalWithoutDiscountBase;
                    decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                    model.SubTotal = _priceFormatter.FormatPrice(subtotal, false, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                    var requiresShipping = cart.RequiresShipping();
                    //a customer should visit the shopping cart page (hide checkout button) before going to checkout if:
                    //1. "terms of service" are enabled
                    //2. min order sub-total is OK
                    //3. we have at least one checkout attribute
                    var checkoutAttributesExistCacheKey = string.Format(ModelCacheEventConsumer.CHECKOUTATTRIBUTES_EXIST_KEY,
                        _storeContext.CurrentStore.Id, requiresShipping);
                    var checkoutAttributesExist = _cacheManager.Get(checkoutAttributesExistCacheKey,
                        () =>
                        {
                            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !requiresShipping);
                            return checkoutAttributes.Count > 0;
                        });

                    bool minSubscriptionOrderSubtotalAmountOk = _orderProcessingService.ValidateMinSubscriptionOrderSubtotalAmount(cart);
                    model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceOnSubscriptionCartPage &&
                        minSubscriptionOrderSubtotalAmountOk &&
                        !checkoutAttributesExist;

                    //plans. sort descending (recently added plans)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(_borrowCartSettings.MiniSubscriptionCartProductNumber)
                        .ToList())
                    {
                        var cartItemModel = new MiniSubscriptionCartModel.SubscriptionCartItemModel
                        {
                            Id = sci.Id,
                            PlanId = sci.Plan.Id,
                            PlanName = sci.Plan.GetLocalized(x => x.Name),
                            PlanSeName = sci.Plan.GetSeName(),
                            Quantity = sci.Quantity,
                            AttributeInfo = ""
                        };

                        //unit prices
                        if (sci.Plan.CallForPrice)
                        {
                            cartItemModel.UnitPrice = _localizationService.GetResource("Plans.CallForPrice");
                        }
                        else
                        {
                            decimal taxRate;
                            decimal borrowCartUnitPriceWithDiscountBase = _taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                            decimal borrowCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.UnitPrice = _priceFormatter.FormatPrice(borrowCartUnitPriceWithDiscount);
                        }

                        //picture
                        if (_borrowCartSettings.ShowProductImagesInMiniSubscriptionCart)
                        {
                            cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                                _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.PlanName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }
            
            return model;
        }

        [NonAction]
        protected virtual SubscriptionOrderTotalsModel PrepareSubscriptionOrderTotalsModel(IList<SubscriptionCartItem> cart, bool isEditable)
        {
            var model = new SubscriptionOrderTotalsModel();
            model.IsEditable = isEditable;

            if (cart.Count > 0)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                Discount orderSubTotalAppliedDiscount;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                decimal subTotalRegistration;
                decimal subTotalDeposit;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionOrderSubtotal;
                _orderTotalCalculationService.GetSubscriptionCartSubTotal(cart, subTotalIncludingTax,
                    out subTotalRegistration, out subTotalDeposit,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                    model.AllowRemovingSubTotalDiscount = orderSubTotalAppliedDiscount != null &&
                                                          orderSubTotalAppliedDiscount.RequiresCouponCode &&
                                                          !String.IsNullOrEmpty(orderSubTotalAppliedDiscount.CouponCode) &&
                                                          model.IsEditable;
                }
                model.RegistrationCharge =_priceFormatter.FormatPrice(cart.FirstOrDefault().RegistrationCharge);
                model.SecurityDeposit = _priceFormatter.FormatPrice(cart.FirstOrDefault().SecurityDeposit);

                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? borrowCartShippingBase = _orderTotalCalculationService.GetSubscriptionCartShippingTotal(cart);
                    if (borrowCartShippingBase.HasValue)
                    {
                        decimal borrowCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(borrowCartShipping, true);

                        //selected shipping method
                        var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInSubscriptionOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates;
                    decimal borrowCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out taxRates);
                    decimal borrowCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartTaxBase, _workContext.WorkingCurrency);

                    if (borrowCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(borrowCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new SubscriptionOrderTotalsModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                decimal orderTotalDiscountAmountBase;
                decimal orderTotalRegistrationAmount;
                decimal orderTotalDepositAmount;
                Discount orderTotalAppliedDiscount;
                List<AppliedGiftCard> appliedGiftCards;

                
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? borrowCartTotalBase = _orderTotalCalculationService.GetSubscriptionCartTotal(cart,
                    out orderTotalRegistrationAmount, out orderTotalDepositAmount,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscount,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (borrowCartTotalBase.HasValue)
                {
                    decimal borrowCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartTotalBase.Value, _workContext.WorkingCurrency);
                    borrowCartTotal += orderTotalRegistrationAmount;
                    borrowCartTotal += orderTotalDepositAmount;
                    borrowCartTotal -= cart.FirstOrDefault().PreviousRentalBalance;
                    model.SubscriptionOrderTotal = _priceFormatter.FormatPrice(borrowCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubscriptionOrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    model.AllowRemovingSubscriptionOrderTotalDiscount = orderTotalAppliedDiscount != null &&
                        orderTotalAppliedDiscount.RequiresCouponCode &&
                        !String.IsNullOrEmpty(orderTotalAppliedDiscount.CouponCode) &&
                        model.IsEditable;
                }
                if (cart != null) {
                    model.SecurityDepositBalance = _priceFormatter.FormatPrice(orderTotalDepositAmount, true, false);
                    model.SecurityDeposit = _priceFormatter.FormatPrice(cart.FirstOrDefault().SecurityDeposit, true, false);
                    model.SecurityDepositPaid = _priceFormatter.FormatPrice(cart.GetCustomer().SecurityDepositBalance, true, false);

                    model.SecurityDepositPaidAmt = cart.FirstOrDefault().Customer.SecurityDepositBalance;
                    model.SecurityDepositAmt = cart.FirstOrDefault().SecurityDeposit;
                    

                    model.SecurityDepositBalanceAmt = orderTotalRegistrationAmount;

                    model.RegistrationChargeBalance = _priceFormatter.FormatPrice(orderTotalRegistrationAmount, true, false);
                    model.RegistrationCharge = _priceFormatter.FormatPrice(cart.FirstOrDefault().RegistrationCharge, true, false);
                    model.RegistrationChargePaid = _priceFormatter.FormatPrice(cart.GetCustomer().RegistrationChargeBalance, true, false);


                    model.PreviousRentalBalanceAmt = cart.FirstOrDefault().PreviousRentalBalance;
                    model.PreviousRentalBalance = _priceFormatter.FormatPrice(cart.FirstOrDefault().PreviousRentalBalance, true, false);
                    model.PreviousRentalBalanceDesc = cart.FirstOrDefault().PreviousRentalBalanceDesc;
                    
                    model.RegistrationChargeAmt = cart.FirstOrDefault().RegistrationCharge;
                    model.RegistrationChargePaidAmt = cart.GetCustomer().RegistrationChargeBalance;
                    model.RegistrationChargeBalanceAmt = orderTotalRegistrationAmount;

                    if (cart.GetCustomer().SecurityDepositBalance > 0)
                        model.SecurityDepositBalanceLabel = "Balance Security Deposit : ";
                    else
                        model.SecurityDepositBalanceLabel = "Security Deposit : ";

                    if (cart.GetCustomer().RegistrationChargeBalance > 0)
                        model.RegistrationChargeBalanceLabel = "Balance Registration Fee : ";
                    else
                        model.RegistrationChargeBalanceLabel = "Registration Fee : ";


                }
                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new SubscriptionOrderTotalsModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = appliedGiftCard.GiftCard.GetGiftCardRemainingAmount() - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled &&
                    _rewardPointsSettings.DisplayHowMuchWillBeEarned &&
                    borrowCartTotalBase.HasValue)
                {
                    model.WillEarnRewardPoints = _orderTotalCalculationService
                        .CalculateRewardPoints(_workContext.CurrentCustomer, borrowCartTotalBase.Value);
                }

            }

            return model;
        }

        [NonAction]
        protected virtual void ParseAndSaveCheckoutAttributes(List<SubscriptionCartItem> cart, FormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                string controlId = string.Format("checkout_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Parse plan attributes on the plan details page
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="form">Form</param>
        /// <returns>Parsed attributes</returns>
        [NonAction]
        protected virtual string ParsePlanAttributes(Plan plan, FormCollection form)
        {
            string attributesXml = "";

             

            #region Gift cards

            if (plan.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", plan.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", plan.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", plan.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", plan.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", plan.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = "";
            }

            #endregion

            return attributesXml;
        }

        /// <summary>
        /// Parse plan rental dates on the plan details page
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [NonAction]
        protected virtual void ParseRentalDates(Plan plan, FormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", plan.Id);
            string endControlId = string.Format("rental_end_date_{0}", plan.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Plan\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }


        [NonAction]
        protected virtual bool IsMinimumSubscriptionOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumSubscriptionOrderPlacementInterval == 0)
                return true;

            var lastSubscriptionOrder = _subscriptionService.SearchSubscriptionOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastSubscriptionOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastSubscriptionOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumSubscriptionOrderPlacementInterval;
        }

        [NonAction]
        protected virtual CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<SubscriptionCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    _orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }

        [NonAction]
        protected virtual CheckoutConfirmModel PrepareConfirmSubscriptionOrderModel(IList<SubscriptionCartItem> cart)
        {
            var model = new CheckoutConfirmModel();
            //terms of service
            model.TermsOfServiceOnSubscriptionOrderConfirmPage = _orderSettings.TermsOfServiceOnSubscriptionOrderConfirmPage;
            //min order amount validation
            bool minSubscriptionOrderTotalAmountOk = _orderProcessingService.ValidateMinSubscriptionOrderTotalAmount(cart);
            if (!minSubscriptionOrderTotalAmountOk)
            {
                decimal minSubscriptionOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinSubscriptionOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinSubscriptionOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinSubscriptionOrderTotalAmount"), _priceFormatter.FormatPrice(minSubscriptionOrderTotalAmount, true, false));
            }
            return model;
        }

        #endregion

        #region Shopping cart
        
        //add plan to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public ActionResult AddPlanToCart_Catalog(int planId, int borrowCartTypeId,
            int quantity, bool forceredirection = false)
        {
            if (!_workContext.CurrentCustomer.IsRegistered()) { 
                //return RedirectToRoute("login", new { ReturnUrl = this.Request.RawUrl });
                return Json(new
                {
                    redirect = Url.RouteUrl("login", new { ReturnUrl = "subscriptioncart" }),
                });

            }
            else { 
                var cartType = (SubscriptionCartType)borrowCartTypeId;

                var plan = _planService.GetPlanById(planId);
                if (plan == null)
                    //no plan found
                    return Json(new
                    {
                        success = false,
                        message = "No plan found with the specified ID"
                    });

                //we can add only simple plans
                //if (plan.PlanType != PlanType.SimplePlan)
                //{
                //    return Json(new
                //    {
                //        redirect = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }),
                //    });
                //}

                //plans with "minimum order quantity" more than a specified qty
                if (plan.SubscriptionMinimumQuantity > quantity)
                {
                    //we cannot add to the cart such plans from category pages
                    //it can confuse customers. That's why we redirect customers to the plan details page
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }),
                    });
                }

                if (plan.CustomerEntersPrice)
                {
                    //cannot be added to the cart (requires a customer to enter price)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }),
                    });
                }

                //if (plan.IsRental)
                //{
                //    //rental plans require start/end dates to be entered
                //    return Json(new
                //    {
                //        redirect = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }),
                //    });
                //}

                var allowedQuantities = plan.ParseAllowedQuantities();
                if (allowedQuantities.Length > 0)
                {
                    //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }),
                    });
                }


                //get standard warnings without attribute validations
                //first, try to find existing shopping cart item
                var cart1 = _workContext.CurrentCustomer.SubscriptionCartItems
                    .Where(sci => sci.SubscriptionCartType == cartType)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                foreach(SubscriptionCartItem c in cart1){
                      _subscriptionCartService.DeleteSubscriptionCartItem(c, ensureOnlyActiveCheckoutAttributes: true);
                }
                var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                   .Where(sci => sci.SubscriptionCartType == cartType)
                   .LimitPerStore(_storeContext.CurrentStore.Id)
                   .ToList();
                var borrowCartItem = _subscriptionCartService.FindSubscriptionCartItemInTheCart(cart, cartType, plan);
                //if we already have the same plan in the cart, then use the total quantity to validate
                var quantityToValidate = borrowCartItem != null ? borrowCartItem.Quantity  : 1;
                var addToCartWarnings = _subscriptionCartService
                    .GetSubscriptionCartItemWarnings(_workContext.CurrentCustomer, cartType,
                    plan, _storeContext.CurrentStore.Id, string.Empty, 
                    decimal.Zero, null, null, quantityToValidate, false, true, false, false, false);
                if (addToCartWarnings.Count > 0)
                {
                    //cannot be added to the cart
                    //let's display standard warnings
                    return Json(new
                    {
                        success = false,
                        message = addToCartWarnings.ToArray()
                    });
                }

                //now let's try adding plan to the cart (now including plan attribute validation, etc)
                addToCartWarnings = _subscriptionCartService.AddToCart(customer: _workContext.CurrentCustomer,
                    plan: plan,
                    borrowCartType: cartType,
                    storeId: _storeContext.CurrentStore.Id,
                    quantity: quantity);
                if (addToCartWarnings.Count > 0)
                {
                    //cannot be added to the cart
                    //but we do not display attribute and gift card warnings here. let's do it on the plan details page
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }),
                    });
                }

                //added to the cart/mytoybox
                switch (cartType)
                {
                    case SubscriptionCartType.MyToyBox:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToMyToyBox", _localizationService.GetResource("ActivityLog.PublicStore.AddToMyToyBox"), plan.Name);

                            if (_borrowCartSettings.DisplayMyToyBoxAfterAddingProduct || forceredirection)
                            {
                                //redirect to the mytoybox page
                                return Json(new
                                {
                                    redirect = Url.RouteUrl("MyToyBox"),
                                });
                            }
                        
                            //display notification message and update appropriate blocks
                            var updatetopmytoyboxsectionhtml = string.Format(_localizationService.GetResource("MyToyBox.HeaderQuantity"),
                            _workContext.CurrentCustomer.SubscriptionCartItems
                            .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());
                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Plans.PlanHasBeenAddedToTheMyToyBox.Link"), Url.RouteUrl("MyToyBox")),
                                updatetopmytoyboxsectionhtml = updatetopmytoyboxsectionhtml,
                            });
                        }
                    case SubscriptionCartType.SubscriptionCart:
                    default:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToSubscriptionCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToSubscriptionCart"), plan.Name);
                            forceredirection = true;
                            if (_borrowCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                            {
                                //redirect to the shopping cart page
                                return Json(new
                                {
                                    redirect = Url.RouteUrl("SubscriptionCart"),
                                });
                            }
                        
                            //display notification message and update appropriate blocks
                            var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("SubscriptionCart.HeaderQuantity"),
                            _workContext.CurrentCustomer.SubscriptionCartItems
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());
                        
                            var updateflyoutcartsectionhtml = _borrowCartSettings.MiniSubscriptionCartEnabled
                                ? this.RenderPartialViewToString("FlyoutSubscriptionCart", PrepareMiniSubscriptionCartModel())
                                : "";

                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Plans.PlanHasBeenAddedToTheCart.Link"), Url.RouteUrl("SubscriptionCart")),
                                updatetopcartsectionhtml = updatetopcartsectionhtml,
                                updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                            });
                        }
                }
            }
        }

        //add plan to cart using AJAX
        //currently we use this method on the plan details pages
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPlanToCart_Details(int planId, int borrowCartTypeId, FormCollection form)
        {
            var plan = _planService.GetPlanById(planId);
            if (plan == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            //we can add only simple plans
            if (plan.PlanType != PlanType.SimplePlan)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple plans could be added to the cart"
                });
            }

            #region Update existing shopping cart item?
            int updatecartitemid = 0;
            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.UpdatedSubscriptionCartItemId", planId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            SubscriptionCartItem updatecartitem = null;
            if (_borrowCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                    .Where(x => x.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No shopping cart item found to update"
                    });
                }
                //is it this plan?
                if (plan.Id != updatecartitem.PlanId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This plan does not match a passed shopping cart item identifier"
                    });
                }
            }
            #endregion

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (plan.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", planId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;
            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", planId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion

            //plan and gift card attributes
            string attributes = ParsePlanAttributes(plan, form);
            
            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (plan.IsRental)
            {
                ParseRentalDates(plan, form, out rentalStartDate, out rentalEndDate);
            }

            //save item
            var addToCartWarnings = new List<string>();
            var cartType = (SubscriptionCartType)borrowCartTypeId;
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(_subscriptionCartService.AddToCart(_workContext.CurrentCustomer,
                    plan, cartType, _storeContext.CurrentStore.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                    .Where(x => x.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var otherCartItemWithSameParameters = _subscriptionCartService.FindSubscriptionCartItemInTheCart(
                    cart, cartType, plan, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's other shopping cart cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(_subscriptionCartService.UpdateSubscriptionOrderCartItem(_workContext.CurrentCustomer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
                if (otherCartItemWithSameParameters != null && addToCartWarnings.Count == 0)
                {
                    //delete the same shopping cart item (the other one)
                    _subscriptionCartService.DeleteSubscriptionCartItem(otherCartItemWithSameParameters);
                }
            }

            #region Return result

            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart/mytoybox
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/mytoybox
            switch (cartType)
            {
                case SubscriptionCartType.MyToyBox:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToMyToyBox", _localizationService.GetResource("ActivityLog.PublicStore.AddToMyToyBox"), plan.Name);

                        if (_borrowCartSettings.DisplayMyToyBoxAfterAddingProduct)
                        {
                            //redirect to the mytoybox page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("MyToyBox"),
                            });
                        }
                        
                        //display notification message and update appropriate blocks
                        var updatetopmytoyboxsectionhtml = string.Format(_localizationService.GetResource("MyToyBox.HeaderQuantity"),
                        _workContext.CurrentCustomer.SubscriptionCartItems
                        .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts());
                        
                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Plans.PlanHasBeenAddedToTheMyToyBox.Link"), Url.RouteUrl("MyToyBox")),
                            updatetopmytoyboxsectionhtml = updatetopmytoyboxsectionhtml,
                        });
                    }
                case SubscriptionCartType.SubscriptionCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToSubscriptionCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToSubscriptionCart"), plan.Name);

                        if (_borrowCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("SubscriptionCart"),
                            });
                        }
                        
                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("SubscriptionCart.HeaderQuantity"),
                        _workContext.CurrentCustomer.SubscriptionCartItems
                        .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts());
                        
                        var updateflyoutcartsectionhtml = _borrowCartSettings.MiniSubscriptionCartEnabled
                            ? this.RenderPartialViewToString("FlyoutSubscriptionCart", PrepareMiniSubscriptionCartModel())
                            : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Plans.PlanHasBeenAddedToTheCart.Link"), Url.RouteUrl("SubscriptionCart")),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                        });
                    }
            }


            #endregion
        }

        //handle plan attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the plan details pages
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PlanDetails_AttributeChange(int planId, bool validateAttributeConditions, FormCollection form)
        {
            var plan = _planService.GetPlanById(planId);
            if (plan == null)
                return new NullJsonResult();

            string attributeXml = ParsePlanAttributes(plan, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (plan.IsRental)
            {
                ParseRentalDates(plan, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            string sku = plan.Sku;
            string mpn = "";
            string gtin = "";

            //price
            string price = "";
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices) && !plan.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                Discount scDiscount;
                decimal discountAmount;
                decimal finalPrice = _priceCalculationService.GetUnitPrice(plan,
                    _workContext.CurrentCustomer,
                    SubscriptionCartType.SubscriptionCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out discountAmount, out scDiscount);
                decimal taxRate;
                decimal finalPriceWithDiscountBase = _taxService.GetPlanPrice(plan, finalPrice, out taxRate);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
            }

            //stock
             
            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                
            }

            return Json(new
            {
                gtin = gtin,
                mpn = mpn,
                sku = sku,
                price = price,
                stockAvailability = false,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray()
            });
        }

      
        [HttpPost]
        public ActionResult UploadFileCheckoutAttribute(int attributeId)
        {
            var attribute = _checkoutAttributeService.GetCheckoutAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, "text/plain");
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("SubscriptionCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty,
                    }, "text/plain");
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("SubscriptionCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            }, "text/plain");
        }


        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Cart(SubscriptionCartModel model)
        {
           
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            //var model = new SubscriptionCartModel();
            PrepareSubscriptionCartModel(model, cart);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult SubscriptionOrderSummary(bool? prepareAndDisplayOrderReviewData)
        {
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new SubscriptionCartModel();
            PrepareSubscriptionCartModel(model, cart, 
                isEditable: false, 
                prepareEstimateShippingIfEnabled: false,
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
            return PartialView(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateCart(FormCollection form)
        {
          

            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _subscriptionCartService.DeleteSubscriptionCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _subscriptionCartService.UpdateSubscriptionOrderCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            //updated cart
            cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new SubscriptionCartModel();
            PrepareSubscriptionCartModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("continueshopping")]
        public ActionResult ContinueShopping()
        {
            var returnUrl = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LastContinueShoppingPage, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToRoute("HomePage");
            }
        }

        [ChildActionOnly]
        public ActionResult CheckoutProgress(CheckoutProgressStep step)
        {
            var model = new CheckoutProgressModel { CheckoutProgressStep = step };
            return PartialView(model);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateInput(false)]
        public ActionResult StartCheckout(SubscriptionCartModel model,FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                 .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                 .LimitPerStore(_storeContext.CurrentStore.Id)
                 .ToList();

            var model2 = PrepareConfirmSubscriptionOrderModel(cart);
            try
            {
                //validation
             
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                //if (!_orderSettings.OnePageCheckoutEnabled)
                //    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumSubscriptionOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinSubscriptionOrderPlacementInterval"));

                //place order
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart, true);
                if (isPaymentWorkflowRequired)
                {
                    //filter by country
                    int filterByCountryId = 0;
                    if (_addressSettings.CountryEnabled &&
                        _workContext.CurrentCustomer.BillingAddress != null &&
                        _workContext.CurrentCustomer.BillingAddress.Country != null)
                    {
                        filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
                    }

                    //payment is required
                    var paymentMethodModel = PreparePaymentMethodModel(cart, filterByCountryId);

                    if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                        paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                    {
                        //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                        //so customer doesn't have to choose a payment method

                        var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                            SystemCustomerAttributeNames.SelectedPaymentMethod,
                            selectedPaymentMethodSystemName, _storeContext.CurrentStore.Id);

                        var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                        if (paymentMethodInst == null ||
                            !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                            !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                            throw new Exception("Selected payment method can't be parsed");
                       // return OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                    }
                    //customer have to choose a payment method
                }
                // = _httpContext.Session["SubscriptionOrderPaymentInfo"] as ProcessPaymentRequest;
                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumSubscriptionOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinSubscriptionOrderPlacementInterval"));
                var processPaymentRequest = new ProcessPaymentRequest();
                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.RegistrationCharge = cart.FirstOrDefault().RegistrationCharge;
                processPaymentRequest.SecurityDeposit = cart.FirstOrDefault().SecurityDeposit;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var placeSubscriptionOrderResult = _orderProcessingService.PlaceSubscriptionOrder(processPaymentRequest);
                if (placeSubscriptionOrderResult.Success)
                {
                    _httpContext.Session["SubscriptionOrderPaymentInfo"] = null;
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        SubscriptionOrder = placeSubscriptionOrderResult.PlacedSubscriptionOrder
                    };
                    _paymentService.PostProcessPayment(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        //redirection or POST has been done in PostProcessPayment
                        return Content("Redirected");
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeSubscriptionOrderResult.PlacedSubscriptionOrder.Id });
                }

                foreach (var error in placeSubscriptionOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
               // return Json(new { error = 1, message = exc.Message });
            }

          //  PrepareSubscriptionCartModel(model1, cart);

            return RedirectToAction("Cart", model );
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applydiscountcouponcode")]
        public ActionResult ApplyDiscountCoupon(string discountcouponcode, FormCollection form)
        {
            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //cart
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new SubscriptionCartModel();
            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetDiscountByCouponCode(discountcouponcode, true);
                if (discount != null && discount.RequiresCouponCode)
                {
                    var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, discountcouponcode);
                    if (validationResult.IsValid)
                    {
                        //valid
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.DiscountCouponCode, discountcouponcode);
                        model.DiscountBox.Message = _localizationService.GetResource("SubscriptionCart.DiscountCouponCode.Applied");
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(validationResult.UserError))
                        {
                            //some user error
                            model.DiscountBox.Message = validationResult.UserError;
                            model.DiscountBox.IsApplied = false;
                        }
                        else
                        {
                            //general error text
                            model.DiscountBox.Message = _localizationService.GetResource("SubscriptionCart.DiscountCouponCode.WrongDiscount");
                            model.DiscountBox.IsApplied = false;
                        }
                    }
                }
                else
                {
                    //discount cannot be found
                    model.DiscountBox.Message = _localizationService.GetResource("SubscriptionCart.DiscountCouponCode.WrongDiscount");
                    model.DiscountBox.IsApplied = false;
                }
            }
            else
            {
                //empty coupon code
                model.DiscountBox.Message = _localizationService.GetResource("SubscriptionCart.DiscountCouponCode.WrongDiscount");
                model.DiscountBox.IsApplied = false;
            }

            PrepareSubscriptionCartModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public ActionResult ApplyGiftCard(string giftcardcouponcode, FormCollection form)
        {
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            //cart
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new SubscriptionCartModel();
            if (!cart.IsRecurring())
            {
                if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
                    if (isGiftCardValid)
                    {
                        _workContext.CurrentCustomer.ApplyGiftCardCouponCode(giftcardcouponcode);
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                        model.GiftCardBox.IsApplied = true;
                    }
                    else
                    {
                        var discount = _discountService.GetDiscountByCouponCode(giftcardcouponcode, true);
                        if (discount != null && discount.RequiresCouponCode)
                        {
                            var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, giftcardcouponcode);
                            if (validationResult.IsValid)
                            {
                                //valid
                                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.DiscountCouponCode, giftcardcouponcode);
                                model.DiscountBox.Message = _localizationService.GetResource("SubscriptionCart.DiscountCouponCode.Applied");
                                model.DiscountBox.IsApplied = true;
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(validationResult.UserError))
                                {
                                    //some user error
                                    model.DiscountBox.Message = validationResult.UserError;
                                    model.DiscountBox.IsApplied = false;
                                }
                                else
                                {
                                    //general error text
                                    model.DiscountBox.Message = _localizationService.GetResource("SubscriptionCart.DiscountCouponCode.WrongDiscount");
                                    model.DiscountBox.IsApplied = false;
                                }
                            }

                            //   model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                            //  model.GiftCardBox.IsApplied = false;
                        }
                    }
                    
                }
                else
                {
                    model.GiftCardBox.Message = _localizationService.GetResource("SubscriptionCart.GiftCardCouponCode.WrongGiftCard");
                    model.GiftCardBox.IsApplied = false;
                }
            }
            else
            {
                model.GiftCardBox.Message = _localizationService.GetResource("SubscriptionCart.GiftCardCouponCode.DontWorkWithAutoshipPlans");
                model.GiftCardBox.IsApplied = false;
            }

            PrepareSubscriptionCartModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [PublicAntiForgery]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("estimateshipping")]
        public ActionResult GetEstimateShipping(EstimateShippingModel shippingModel, FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new SubscriptionCartModel();
            model.EstimateShipping.CountryId = shippingModel.CountryId;
            model.EstimateShipping.StateProvinceId = shippingModel.StateProvinceId;
            model.EstimateShipping.ZipPostalCode = shippingModel.ZipPostalCode;
            PrepareSubscriptionCartModel(model, cart,setEstimateShippingDefaultAddress: false);

            if (cart.RequiresShipping())
            {
                var address = new Address
                {
                    CountryId = shippingModel.CountryId,
                    Country = shippingModel.CountryId.HasValue ? _countryService.GetCountryById(shippingModel.CountryId.Value) : null,
                    StateProvinceId  = shippingModel.StateProvinceId,
                    StateProvince = shippingModel.StateProvinceId.HasValue ? _stateProvinceService.GetStateProvinceById(shippingModel.StateProvinceId.Value) : null,
                    ZipPostalCode = shippingModel.ZipPostalCode,
                };
                GetShippingOptionResponse getShippingOptionResponse = _shippingService
                    .GetShippingOptions(cart, address, "", _storeContext.CurrentStore.Id);
                if (!getShippingOptionResponse.Success)
                {
                    foreach (var error in getShippingOptionResponse.Errors)
                        model.EstimateShipping.Warnings.Add(error);
                }
                else
                {
                    if (getShippingOptionResponse.ShippingOptions.Count > 0)
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            var soModel = new EstimateShippingModel.ShippingOptionModel
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,

                            };
                            //calculate discounted and taxed rate
                            Discount appliedDiscount = null;
                            decimal shippingTotal = _orderTotalCalculationService.AdjustShippingRate(shippingOption.Rate,
                                cart, out appliedDiscount);

                            decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                            decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                            soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                            model.EstimateShipping.ShippingOptions.Add(soModel);
                        }

                        //pickup in store?
                        if (_shippingSettings.AllowPickUpInStore)
                        {
                            var soModel = new EstimateShippingModel.ShippingOptionModel
                            {
                                Name = _localizationService.GetResource("Checkout.PickUpInStore"),
                                Description = _localizationService.GetResource("Checkout.PickUpInStore.Description"),
                            };
                            decimal shippingTotal = _shippingSettings.PickUpInStoreFee;
                            decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                            decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                            soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                            model.EstimateShipping.ShippingOptions.Add(soModel);
                        }
                    }
                    else
                    {
                       model.EstimateShipping.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                    }
                }
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult SubscriptionOrderTotals(bool isEditable)
        {
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = PrepareSubscriptionOrderTotalsModel(cart, isEditable);
            return PartialView(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("removesubtotaldiscount", "removeordertotaldiscount", "removediscountcouponcode")]
        public ActionResult RemoveDiscountCoupon()
        {
            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new SubscriptionCartModel();

            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.DiscountCouponCode, null);

            PrepareSubscriptionCartModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
        public ActionResult RemoveGiftCardCode(FormCollection form)
        {
            var model = new SubscriptionCartModel();

            //get gift card identifier
            int giftCardId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                    giftCardId = Convert.ToInt32(formValue.Substring("removegiftcard-".Length));
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
            {
                _workContext.CurrentCustomer.RemoveGiftCardCouponCode(gc.GiftCardCouponCode);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }

            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            PrepareSubscriptionCartModel(model, cart);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult FlyoutSubscriptionCart()
        {
            if (!_borrowCartSettings.MiniSubscriptionCartEnabled)
                return Content("");

            

            var model = PrepareMiniSubscriptionCartModel();
            return PartialView(model);
        }

        #endregion

        #region MyToyBox

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult MyToyBox(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox))
                return RedirectToRoute("HomePage");

            Customer customer = customerGuid.HasValue ? 
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToRoute("HomePage");
            var cart = customer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new MyToyBoxModel();
            PrepareMyToyBoxModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("MyToyBox")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateMyToyBox(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null 
                ? form["removefromcart"].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList() 
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _subscriptionCartService.DeleteSubscriptionCartItem(sci);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _subscriptionCartService.UpdateSubscriptionOrderCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated mytoybox
            cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new MyToyBoxModel();
            PrepareMyToyBoxModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("MyToyBox")]
        [FormValueRequired("addtocartbutton")]
        public ActionResult AddItemsToCartFromMyToyBox(Guid? customerGuid, FormCollection form)
        { 

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox))
                return RedirectToRoute("HomePage");

            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return RedirectToRoute("HomePage");

            var pageCart = pageCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;
            var allIdsToAdd = form["addtocart"] != null 
                ? form["addtocart"].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList() 
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _subscriptionCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.Plan, SubscriptionCartType.SubscriptionCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                    if (warnings.Count == 0)
                        numberOfAddedItems++;
                    if (_borrowCartSettings.MoveItemsFromMyToyBoxToCart && //settings enabled
                        !customerGuid.HasValue && //own mytoybox
                        warnings.Count == 0) //no warnings ( already in the cart)
                    {
                        //let's remove the item from mytoybox
                        _subscriptionCartService.DeleteSubscriptionCartItem(sci);
                    }
                    allWarnings.AddRange(warnings);
                }
            }

            if (numberOfAddedItems > 0)
            {
                //redirect to the shopping cart page

                if (allWarnings.Count > 0)
                {
                    ErrorNotification(_localizationService.GetResource("MyToyBox.AddToCart.Error"), true);
                }

                return RedirectToRoute("SubscriptionCart");
            }
            else
            {
                //no items added. redisplay the mytoybox page

                if (allWarnings.Count > 0)
                {
                    ErrorNotification(_localizationService.GetResource("MyToyBox.AddToCart.Error"), false);
                }

                var cart = pageCustomer.SubscriptionCartItems
                    .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var model = new MyToyBoxModel();
                PrepareMyToyBoxModel(model, cart, !customerGuid.HasValue);
                return View(model);
            }
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult EmailMyToyBox()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox) || !_borrowCartSettings.EmailMyToyBoxEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (cart.Count == 0)
                return RedirectToRoute("HomePage");

            var model = new MyToyBoxEmailAFriendModel
            {
                YourEmailAddress = _workContext.CurrentCustomer.Email,
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailMyToyBoxToFriendPage
            };
            return View(model);
        }

        [HttpPost, ActionName("EmailMyToyBox")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        [CaptchaValidator]
        public ActionResult EmailMyToyBoxSend(MyToyBoxEmailAFriendModel model, bool captchaValid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox) || !_borrowCartSettings.EmailMyToyBoxEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == SubscriptionCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (cart.Count == 0)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailMyToyBoxToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            //check whether the current customer is guest and ia allowed to email mytoybox
            if (_workContext.CurrentCustomer.IsGuest() && !_borrowCartSettings.AllowAnonymousUsersToEmailMyToyBox)
            {
                ModelState.AddModelError("", _localizationService.GetResource("MyToyBox.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendMyToyBoxEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, model.YourEmailAddress,
                        model.FriendEmail, Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("MyToyBox.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailMyToyBoxToFriendPage;
            return View(model);
        }

        #endregion
    }
}
