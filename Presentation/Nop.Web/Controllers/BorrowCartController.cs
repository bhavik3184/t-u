using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
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
using Nop.Web.Models.BorrowCart;
using Nop.Services.Helpers;
using Nop.Core.Domain.Payments;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    public partial class BorrowCartController : BasePublicController
    {
		#region Fields
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IBorrowCartService _borrowCartService;
        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
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
        private readonly BorrowCartSettings _borrowCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

		#region Constructors

        public BorrowCartController(
            ICategoryService categoryService,
            IProductService productService, 
            IStoreContext storeContext,
            IWorkContext workContext,
            IBorrowCartService borrowCartService, 
            ISubscriptionOrderService subscriptionService,
            IPictureService pictureService,
            ILocalizationService localizationService, 
            IProductAttributeService productAttributeService, 
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService, 
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,
            ICustomerService customerService, 
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService, 
            IOrderTotalCalculationService orderTotalCalculationService,
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
            BorrowCartSettings borrowCartSettings,
            CatalogSettings catalogSettings, 
            OrderSettings orderSettings,
            ShippingSettings shippingSettings, 
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings, 
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            IDateTimeHelper dateTimeHelper)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._borrowCartService = borrowCartService;
            this._subscriptionService = subscriptionService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
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
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        private Category GetParentCategory(int categoryId)
        {
            Category cat = _categoryService.GetCategoryById(categoryId);
            while (cat != null && cat.ParentCategoryId != 0)
            {
                cat = _categoryService.GetCategoryById(cat.ParentCategoryId);
            }

            return cat;
        }


        [NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(BorrowCartItem sci,
            int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(pictureCacheKey, 
                //as we cache per user (shopping cart item identifier)
                //let's cache just for 3 minutes
                3, () =>
            {
                //shopping cart item picture
                var sciPicture = sci.Product.GetProductPicture(sci.AttributesXml, _pictureService, _productAttributeParser);
                return new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                };
            });
            return model;
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
        protected virtual void PrepareBorrowCartModel(BorrowCartModel model, 
            IList<BorrowCartItem> cart, bool isEditable = true, 
            bool validateCheckoutAttributes = false, 
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");
            
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (cart.Count == 0)
                return;
            
            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _borrowCartSettings.ShowProductImagesOnBorrowCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnBorrowCartPage = _orderSettings.TermsOfServiceOnBorrowCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoBorrowCart;

            //gift card and gift card boxes
            model.DiscountBox.Display= _borrowCartSettings.ShowDiscountBox;
            var discountCouponCode = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox.Display = _borrowCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _borrowCartService.GetBorrowCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);
            
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
                var cartItemModel = new BorrowCartModel.BorrowCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(sci.CreatedOnUtc, DateTimeKind.Utc).Day + "/"
                    + _dateTimeHelper.ConvertToUserTime(sci.CreatedOnUtc, DateTimeKind.Utc).Month + "/"
                    + _dateTimeHelper.ConvertToUserTime(sci.CreatedOnUtc, DateTimeKind.Utc).Year ,
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _borrowCartSettings.AllowCartItemEditing && 
                    sci.Product.ProductType == ProductType.SimpleProduct &&
                    (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Product.IsGiftCard) &&
                    sci.Product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
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
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("BorrowCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                //if (sci.Product.IsRental)
                //{
                //    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                //    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                //    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("BorrowCart.Rental.FormattedDate"),
                //        rentalStartDate, rentalEndDate);
                //}

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal borrowCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal borrowCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(borrowCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    Discount scDiscount;
                    decimal borrowCartItemDiscountBase;
                    decimal taxRate;
                    decimal borrowCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out borrowCartItemDiscountBase, out scDiscount), out taxRate);
                    decimal borrowCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(borrowCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscount != null)
                    {
                        borrowCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, borrowCartItemDiscountBase, out taxRate);
                        if (borrowCartItemDiscountBase > decimal.Zero)
                        {
                            decimal borrowCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(borrowCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_borrowCartSettings.ShowProductImagesOnBorrowCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _borrowCartService.GetBorrowCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.BorrowCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    //sci.RentalStartDateUtc,
                    //sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                    model.OrderReviewData.BillingAddress.PrepareModel(
                        address: billingAddress, 
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);
               
                //shipping info
                if (cart.RequiresShipping())
                {
                    model.OrderReviewData.IsShippable = true;

                    if (_shippingSettings.AllowPickUpInStore)
                    {
                        model.OrderReviewData.SelectedPickUpInStore = _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.SelectedPickUpInStore, _storeContext.CurrentStore.Id);
                    }

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            model.OrderReviewData.ShippingAddress.PrepareModel(
                                address: shippingAddress, 
                                excludeProperties: false,
                                addressSettings: _addressSettings,
                                addressAttributeFormatter: _addressAttributeFormatter);
                        }
                    }
                    
                    
                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";

                //custom values
                var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest != null)
                {
                    model.OrderReviewData.CustomValues = processPaymentRequest.CustomValues;
                }
            }
            #endregion
        }

        [NonAction]
        protected virtual void PrepareMyToyBoxModel(MyToyBoxModel model,
            IList<BorrowCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailMyToyBoxEnabled = _borrowCartSettings.EmailMyToyBoxEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoMyToyBox;

            if (cart.Count == 0)
                return;

            #region Simple properties

            var customer = cart.GetCustomer();
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = customer.GetFullName();
            model.ShowProductImages = _borrowCartSettings.ShowProductImagesOnBorrowCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            
            //cart warnings
            var cartWarnings = _borrowCartService.GetBorrowCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion
            
            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new MyToyBoxModel.BorrowCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                };

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
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
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("BorrowCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                //if (sci.Product.IsRental)
                //{
                //    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                //    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                //    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("BorrowCart.Rental.FormattedDate"),
                //        rentalStartDate, rentalEndDate);
                //}

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal borrowCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal borrowCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(borrowCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    Discount scDiscount;
                    decimal borrowCartItemDiscountBase;
                    decimal taxRate;
                    decimal borrowCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out borrowCartItemDiscountBase, out scDiscount), out taxRate);
                    decimal borrowCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(borrowCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscount != null)
                    {
                        borrowCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, borrowCartItemDiscountBase, out taxRate);
                        if (borrowCartItemDiscountBase > decimal.Zero)
                        {
                            decimal borrowCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(borrowCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_borrowCartSettings.ShowProductImagesOnBorrowCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _borrowCartService.GetBorrowCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.BorrowCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    //sci.RentalStartDateUtc,
                    //sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion
        }

        [NonAction]
        protected virtual MiniBorrowCartModel PrepareMiniBorrowCartModel()
        {
            var model = new MiniBorrowCartModel
            {
                ShowProductImages = _borrowCartSettings.ShowProductImagesInMiniBorrowCart,
                //let's always display it
                DisplayBorrowCartButton = true,
                CurrentCustomerIsGuest = _workContext.CurrentCustomer.IsGuest(),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };


            //performance optimization (use "HasBorrowCartItems" property)
            if (_workContext.CurrentCustomer.HasBorrowCartItems)
            {
                var cart = _workContext.CurrentCustomer.BorrowCartItems
                    .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                model.TotalProducts = cart.GetTotalProducts();
                if (cart.Count > 0)
                {
                    //subtotal
                    decimal orderSubTotalDiscountAmountBase;
                    Discount orderSubTotalAppliedDiscount;
                    decimal subTotalWithoutDiscountBase;
                    decimal subTotalWithDiscountBase;
                    var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                    _orderTotalCalculationService.GetBorrowCartSubTotal(cart, subTotalIncludingTax,
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
                    bool checkoutAttributesExist = false;

                    bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
                    model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceOnBorrowCartPage &&
                        minOrderSubtotalAmountOk &&
                        !checkoutAttributesExist;

                    //products. sort descending (recently added products)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(_borrowCartSettings.MiniBorrowCartProductNumber)
                        .ToList())
                    {
                        var cartItemModel = new MiniBorrowCartModel.BorrowCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = sci.Product.Id,
                            ProductName = sci.Product.GetLocalized(x => x.Name),
                            ProductSeName = sci.Product.GetSeName(),
                            Quantity = sci.Quantity,
                            AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml)
                        };

                        //unit prices
                        if (sci.Product.CallForPrice)
                        {
                            cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                        }
                        else
                        {
                            decimal taxRate;
                            decimal borrowCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                            decimal borrowCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.UnitPrice = _priceFormatter.FormatPrice(borrowCartUnitPriceWithDiscount);
                        }

                        //picture
                        if (_borrowCartSettings.ShowProductImagesInMiniBorrowCart)
                        {
                            cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                                _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }
            
            return model;
        }

        [NonAction]
        protected virtual OrderTotalsModel PrepareOrderTotalsModel(IList<BorrowCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;

            if (cart.Count > 0)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                Discount orderSubTotalAppliedDiscount;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetBorrowCartSubTotal(cart, subTotalIncludingTax,
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


                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? borrowCartShippingBase = _orderTotalCalculationService.GetBorrowCartShippingTotal(cart);
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
                decimal paymentMethodAdditionalFee = decimal.Zero;
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
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
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
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
                Discount orderTotalAppliedDiscount;
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? borrowCartTotalBase = _orderTotalCalculationService.GetBorrowCartTotal(cart,false);
                if (borrowCartTotalBase.HasValue)
                {
                    decimal borrowCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(borrowCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(borrowCartTotal, true, false);
                }

                //discount
               

                 

            }

            return model;
        }

        [NonAction]
        protected virtual void ParseAndSaveCheckoutAttributes(List<BorrowCartItem> cart, FormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
             
            

            //save checkout attributes
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Parse product attributes on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <returns>Parsed attributes</returns>
        [NonAction]
        protected virtual string ParseProductAttributes(Product product, FormCollection form)
        {
            string attributesXml = "";

            #region Product attributes

            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}", attribute.Id);
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
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
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
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
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
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [NonAction]
        protected virtual void ParseRentalDates(Product product, FormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", product.Id);
            string endControlId = string.Format("rental_end_date_{0}", product.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        #endregion

        #region Shopping cart
        
        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public ActionResult AddProductToCart_Catalog(int productId, int borrowCartTypeId,
            int quantity, bool forceredirection = false)
        {
            var cartType = (BorrowCartType)borrowCartTypeId;

            var product = _productService.GetProductById(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.IsRental)
            {
                //rental products require start/end dates to be entered
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            var allowedQuantities = product.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.ProductAttributeMappings.Count > 0)
            {
                //product has some attributes. let a customer see them
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == cartType)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            bool successflag = false;
            String errorMessage = "";

            var currentorder = _subscriptionService.GetCurrentSubscribedOrder(_workContext.CurrentCustomer.Id);
            if (currentorder != null)
            {
                var subscriptionOrderItem = currentorder.SubscriptionOrderItems.FirstOrDefault();
                if(subscriptionOrderItem !=null)
                {
                    if(cartType == BorrowCartType.MyToyBox)
                    {
                        var planCategories = subscriptionOrderItem.Plan.PlanCategories.ToList();
                        List<Category> proCategories = new List<Category>();
                        foreach (BorrowCartItem sct in cart)
                        {
                            if (sct.Product.ProductCategories.Count > 0) { 
                                Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                                if (getparent.ParentCategoryId == 0)
                                {
                                    proCategories.Add(getparent);
                                }
                            }
                        }

                        //var productCategories = proCategories.GroupBy(x => x.Id).Select(y => y.First());
                        
                        var productCategories = _categoryService.GetProductCategoriesByProductId(productId);
                        int s = 0;
                        foreach(PlanCategory pl in planCategories){
                            if (pl.Category.ParentCategoryId == 0) {
                                if (productCategories.Count > 0) {
                                    var cat = GetParentCategory(productCategories.FirstOrDefault().CategoryId);
                                    if (pl.CategoryId == cat.Id)
                                    {
                                        int totalcatcount = proCategories.Where(x => x.Id == cat.Id).Count();
                                        
                                        if (pl.MyToyBoxQuantity > totalcatcount)
                                        {
                                            successflag = true;
                                        }
                                        else
                                        {
                                            errorMessage = "Prdouct is not added. Your MyToBox list contain " + (totalcatcount).ToString() + " product from this category. You can add " + pl.MyToyBoxQuantity + " product to MyToBox list from " + cat.Name + " category.";
                                            return Json(new
                                            {
                                                success = false,
                                                message = errorMessage,
                                            });
                                        }
                                    }
                                    s = 1;
                                }
                                else
                                {
                                    successflag = false;
                                    errorMessage = "Product dont have any category assigned..";
                                }
                            }
                            else{

                            }
                            if (productCategories.Count > 0 && successflag == false)
                                errorMessage = "Your current subscription plan dont include product from " + productCategories.FirstOrDefault().Category.Name + " Category.";
                        }
                    }
                    else
                    {
                        var planCategories = subscriptionOrderItem.Plan.PlanCategories.ToList();
                        List<Category> proCategories = new List<Category>();
                        foreach (BorrowCartItem sct in cart)
                        {
                            if (sct.Product.ProductCategories.Count > 0)
                            {
                                Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                                if (getparent.ParentCategoryId == 0)
                                {
                                    proCategories.Add(getparent);
                                }
                            }
                        }

                        //var productCategories = proCategories.GroupBy(x => x.Id).Select(y => y.First());

                        var productCategories = _categoryService.GetProductCategoriesByProductId(productId);
                         
                        foreach (PlanCategory pl in planCategories)
                        {
                            if (pl.Category.ParentCategoryId == 0)
                            {
                                if (productCategories.Count > 0)
                                {
                                    var cat = GetParentCategory(productCategories.FirstOrDefault().CategoryId);
                                    if (pl.CategoryId == cat.Id)
                                    {
                                        int totalcatcount = proCategories.Where(x => x.Id == cat.Id).Count();
                                        
                                        if ((pl.Quantity) > totalcatcount)
                                        {
                                            successflag = true;
                                        }
                                        else
                                        {
                                            errorMessage = "Prdouct is not added. Your Borrow list  contain " + (totalcatcount).ToString() + " product from this category. You can add " + pl.Quantity + " product to Borrow list  from " + cat.Name + " category.";
                                            return Json(new
                                            {
                                                success = false,
                                                message = errorMessage,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    successflag = false;
                                    errorMessage = "Product dont have any category assigned..";
                                }
                                 
                            }
                            else
                            {

                            }
                            if(productCategories.Count > 0 && successflag ==false)
                                errorMessage = "Your current subscription plan dont include product from " + productCategories.FirstOrDefault().Category.Name + " Category.";
                        }
                    }
                }
                else
                {
                    successflag = false;
                    errorMessage = "Subscribed order not found.";
                }
            }
            else
            {
                successflag = false;
                errorMessage = "Subscribed order not found.";
            }

            if (successflag) { 

                var borrowCartItem = _borrowCartService.FindBorrowCartItemInTheCart(cart, cartType, product);
                //if we already have the same product in the cart, then use the total quantity to validate
                var quantityToValidate = borrowCartItem != null ? borrowCartItem.Quantity + quantity : quantity;
                var addToCartWarnings = _borrowCartService
                    .GetBorrowCartItemWarnings(_workContext.CurrentCustomer, cartType,
                    product, _storeContext.CurrentStore.Id, string.Empty,
                    decimal.Zero,   quantityToValidate, false, true, false, false, false);
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

                //now let's try adding product to the cart (now including product attribute validation, etc)
                addToCartWarnings = _borrowCartService.AddToCart(customer: _workContext.CurrentCustomer,
                    product: product,
                    borrowCartType: cartType,
                    storeId: _storeContext.CurrentStore.Id,
                    quantity: quantity);
                if (addToCartWarnings.Count > 0)
                {
                    //cannot be added to the cart
                    //but we do not display attribute and gift card warnings here. let's do it on the product details page
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                    });
                }

                //added to the cart/mytoybox
                switch (cartType)
                {
                    case BorrowCartType.MyToyBox:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToMyToyBox", _localizationService.GetResource("ActivityLog.PublicStore.AddToMyToyBox"), product.Name);

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
                            _workContext.CurrentCustomer.BorrowCartItems
                            .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());
                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheMyToyBox.Link"), Url.RouteUrl("MyToyBox")),
                                updatetopmytoyboxsectionhtml = updatetopmytoyboxsectionhtml,
                            });
                        }
                    case BorrowCartType.BorrowCart:
                    default:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToBorrowCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToBorrowCart"), product.Name);

                            if (_borrowCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                            {
                                //redirect to the shopping cart page
                                return Json(new
                                {
                                    redirect = Url.RouteUrl("BorrowCart"),
                                });
                            }

                            //display notification message and update appropriate blocks
                            var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("BorrowCart.HeaderQuantity"),
                            _workContext.CurrentCustomer.BorrowCartItems
                            .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());

                            var updateflyoutcartsectionhtml = _borrowCartSettings.MiniBorrowCartEnabled
                                ? this.RenderPartialViewToString("FlyoutBorrowCart", PrepareMiniBorrowCartModel())
                                : "";

                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("BorrowCart")),
                                updatetopcartsectionhtml = updatetopcartsectionhtml,
                                updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                            });

                        }
                    }
            }
            return Json(new
            {
                success = false,
                message = errorMessage,
            });

             
        }

        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProductToCart_Details(int productId, int borrowCartTypeId, FormCollection form)
        {
            var product = _productService.GetProductById(productId);
            var cartType = (BorrowCartType)borrowCartTypeId;
            if (product == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple products could be added to the cart"
                });
            }

             //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                         .Where(x => x.BorrowCartType == cartType)
                         .LimitPerStore(_storeContext.CurrentStore.Id)
                         .ToList();

            bool successflag = false;
            String errorMessage = "";

            var currentorder = _subscriptionService.GetCurrentSubscribedOrder(_workContext.CurrentCustomer.Id);
            if (currentorder != null)
            {
                var subscriptionOrderItem = currentorder.SubscriptionOrderItems.FirstOrDefault();
                if(subscriptionOrderItem !=null)
                {
                    if (cartType == BorrowCartType.MyToyBox)
                    {
                        var planCategories = subscriptionOrderItem.Plan.PlanCategories.ToList();
                        List<Category> proCategories = new List<Category>();
                        foreach (BorrowCartItem sct in cart)
                        {
                            if (sct.Product.ProductCategories.Count > 0)
                            {
                                Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                                if (getparent.ParentCategoryId == 0)
                                {
                                    proCategories.Add(getparent);
                                }
                            }
                        }

                        //var productCategories = proCategories.GroupBy(x => x.Id).Select(y => y.First());

                        var productCategories = _categoryService.GetProductCategoriesByProductId(productId);
                        String s = "";
                        foreach (PlanCategory pl in planCategories)
                        {
                            if (pl.Category.ParentCategoryId == 0)
                            {
                                if (productCategories.Count > 0)
                                {
                                    var cat = GetParentCategory(productCategories.FirstOrDefault().CategoryId);
                                    if (pl.CategoryId == cat.Id)
                                    {
                                        int totalcatcount = proCategories.Where(x => x.Id == cat.Id).Count();

                                        if (pl.MyToyBoxQuantity > totalcatcount)
                                        {
                                            successflag = true;
                                        }
                                        else
                                        {
                                            errorMessage = "Prdouct is not added. Your MyToBox list contain " + (totalcatcount).ToString() + " product from this category. You can add " + pl.MyToyBoxQuantity + " product to MyToBox list from " + cat.Name + " category.";
                                            return Json(new
                                            {
                                                success = false,
                                                message = errorMessage,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    successflag = false;
                                    errorMessage = "Product dont have any category assigned..";
                                }
                            }
                            else
                            {

                            }
                            if (productCategories.Count > 0 && successflag == false)
                                errorMessage = "Your current subscription plan dont include product from " + productCategories.FirstOrDefault().Category.Name + " Category.";
                        }


                    }
                    else
                    {
                        var planCategories = subscriptionOrderItem.Plan.PlanCategories.ToList();
                        List<Category> proCategories = new List<Category>();
                        foreach (BorrowCartItem sct in cart)
                        {
                            if (sct.Product.ProductCategories.Count > 0)
                            {
                                Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                                if (getparent.ParentCategoryId == 0)
                                {
                                    proCategories.Add(getparent);
                                }
                            }
                        }

                        //var productCategories = proCategories.GroupBy(x => x.Id).Select(y => y.First());

                        var productCategories = _categoryService.GetProductCategoriesByProductId(productId);
                        String s = "";
                        foreach (PlanCategory pl in planCategories)
                        {
                            if (pl.Category.ParentCategoryId == 0)
                            {
                                if (productCategories.Count > 0)
                                {
                                    var cat = GetParentCategory(productCategories.FirstOrDefault().CategoryId);
                                    if (pl.CategoryId == cat.Id)
                                    {
                                        int totalcatcount = proCategories.Where(x => x.Id == cat.Id).Count();

                                        if (pl.Quantity > totalcatcount)
                                        {
                                            successflag = true;
                                        }
                                        else
                                        {
                                            errorMessage = "Prdouct is not added. Your Borrow list  contain " + (totalcatcount).ToString() + " product from this category. You can add " + pl.Quantity + " product to Borrow list  from " + cat.Name + " category.";
                                            return Json(new
                                            {
                                                success = false,
                                                message = errorMessage,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    successflag = false;
                                    errorMessage = "Product dont have any category assigned..";
                                }

                            }
                            else
                            {

                            }
                            if (productCategories.Count > 0 && successflag == false)
                                errorMessage = "Your current subscription plan dont include product from " + productCategories.FirstOrDefault().Category.Name + " Category.";
                        }
                    }
                }
                else
                {
                    successflag = false;
                    errorMessage = "Subscribed order not found.";
                }
            }
            else
            {
                successflag = false;
                errorMessage = "Subscribed order not found.";
            }


            if (successflag)
            {

                #region Update existing shopping cart item?
                int updatecartitemid = 0;
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("addtocart_{0}.UpdatedBorrowCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out updatecartitemid);
                        break;
                    }
                BorrowCartItem updatecartitem = null;
                if (_borrowCartSettings.AllowCartItemEditing && updatecartitemid > 0)
                {
                     
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
                    //is it this product?
                    if (product.Id != updatecartitem.ProductId)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This product does not match a passed shopping cart item identifier"
                        });
                    }
                }
                #endregion


                int quantity = 1;


                //product and gift card attributes
                string attributes = ParseProductAttributes(product, form);

                //rental attributes
                DateTime? rentalStartDate = null;
                DateTime? rentalEndDate = null;
                if (product.IsRental)
                {
                    ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
                }

                //save item
                var addToCartWarnings = new List<string>();
                if (updatecartitem == null)
                {
                    //add to the cart
                    addToCartWarnings.AddRange(_borrowCartService.AddToCart(_workContext.CurrentCustomer,
                        product, cartType, _storeContext.CurrentStore.Id,
                        attributes, quantity,
                         quantity, true));
                }
                else
                {
                   
                    var otherCartItemWithSameParameters = _borrowCartService.FindBorrowCartItemInTheCart(
                        cart, cartType, product, attributes, 0,
                        rentalStartDate, rentalEndDate);
                    if (otherCartItemWithSameParameters != null &&
                        otherCartItemWithSameParameters.Id == updatecartitem.Id)
                    {
                        //ensure it's other shopping cart cart item
                        otherCartItemWithSameParameters = null;
                    }
                    //update existing item
                    addToCartWarnings.AddRange(_borrowCartService.UpdateBorrowCartItem(_workContext.CurrentCustomer,
                        updatecartitem.Id, attributes, 0,
                       quantity, true));
                    if (otherCartItemWithSameParameters != null && addToCartWarnings.Count == 0)
                    {
                        //delete the same shopping cart item (the other one)
                        _borrowCartService.DeleteBorrowCartItem(otherCartItemWithSameParameters);
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

                //now let's try adding product to the cart (now including product attribute validation, etc)
                //_borrowCartService.AddToCart(customer: _workContext.CurrentCustomer,
                //   product: product,
                //   borrowCartType: cartType,
                //   storeId: _storeContext.CurrentStore.Id,
                //   quantity: quantity);
                //added to the cart/mytoybox
                switch (cartType)
                {
                    case BorrowCartType.MyToyBox:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToMyToyBox", _localizationService.GetResource("ActivityLog.PublicStore.AddToMyToyBox"), product.Name);

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
                            _workContext.CurrentCustomer.BorrowCartItems
                            .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());

                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheMyToyBox.Link"), Url.RouteUrl("MyToyBox")),
                                updatetopmytoyboxsectionhtml = updatetopmytoyboxsectionhtml,
                            });
                        }
                    case BorrowCartType.BorrowCart:
                    default:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToBorrowCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToBorrowCart"), product.Name);

                            if (_borrowCartSettings.DisplayCartAfterAddingProduct)
                            {
                                //redirect to the shopping cart page
                                return Json(new
                                {
                                    redirect = Url.RouteUrl("BorrowCart"),
                                });
                            }

                            //display notification message and update appropriate blocks
                            var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("BorrowCart.HeaderQuantity"),
                            _workContext.CurrentCustomer.BorrowCartItems
                            .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());

                            var updateflyoutcartsectionhtml = _borrowCartSettings.MiniBorrowCartEnabled
                                ? this.RenderPartialViewToString("FlyoutBorrowCart", PrepareMiniBorrowCartModel())
                                : "";

                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("BorrowCart")),
                                updatetopcartsectionhtml = updatetopcartsectionhtml,
                                updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                            });
                        }
                }


                #endregion
            }
            return Json(new
            {
                success = false,
                message = errorMessage,
            });

        }

         
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, FormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return new NullJsonResult();

            string attributeXml = ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            string sku = product.FormatSku(attributeXml, _productAttributeParser);
            string mpn = product.FormatMpn(attributeXml, _productAttributeParser);
            string gtin = product.FormatGtin(attributeXml, _productAttributeParser);

            //price
            string price = "";
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices) && !product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                Discount scDiscount;
                decimal discountAmount;
                decimal finalPrice = _priceCalculationService.GetUnitPrice(product,
                    _workContext.CurrentCustomer,
                    BorrowCartType.BorrowCart,
                    1, attributeXml, 0, null, null,
                    true, out discountAmount, out scDiscount);
                decimal taxRate;
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out taxRate);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
            }

            //stock
            var stockAvailability = product.FormatStockMessage(attributeXml, _localizationService, _productAttributeParser);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            return Json(new
            {
                gtin = gtin,
                mpn = mpn,
                sku = sku,
                price = price,
                stockAvailability = stockAvailability,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray()
            });
        }

        [HttpPost]
        public ActionResult UploadFileProductAttribute(int attributeId)
        {
            var attribute = _productAttributeService.GetProductAttributeMappingById(attributeId);
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
                        message = string.Format(_localizationService.GetResource("BorrowCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
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
                message = _localizationService.GetResource("BorrowCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            }, "text/plain");
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Cart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new BorrowCartModel();

            var currentOrder = _subscriptionService.GetCurrentSubscribedOrder(_workContext.CurrentCustomer.Id);
            if (currentOrder != null)
            {
                if(currentOrder.SubscriptionOrderItems.Count > 0){
                    var orderitems = _subscriptionService.GetAllOrderItemsCount(orderId: currentOrder.Id, customerId: currentOrder.CustomerId, createdFromUtc: DateTime.Now.AddMonths(-1), createdToUtc: DateTime.Now.AddDays(1), os: SubscriptionOrderStatus.Complete, ps: PaymentStatus.Paid, ss: ShippingStatus.Shipped);
                    if (currentOrder.PaymentStatus == PaymentStatus.Paid) {
                        model.AllowedTransaction = (currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.MaxNoOfDeliveries).ToString();
                        model.UsedTransaction = currentOrder.NoOfDeliveries.ToString();
                    }
                    else
                    {
                        model.AllowedTransaction = (currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.MaxNoOfDeliveries).ToString();
                        model.UsedTransaction = currentOrder.NoOfDeliveries.ToString();
                    }

                    var plancategories = currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.PlanCategories;
                    List<Category> proCategories = new List<Category>();

                    foreach (BorrowCartItem sct in cart)
                    {
                        if (sct.Product.ProductCategories.Count > 0)
                        {
                            Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                            if (getparent.ParentCategoryId == 0)
                            {
                                proCategories.Add(getparent);
                            }
                        }
                    }

                    foreach (PlanCategory pl in plancategories)
                    {
                        PlanCategoryModel plm = new PlanCategoryModel();
                        plm.CategoryName = pl.Category.Name;
                        plm.Quantity = pl.Quantity;
                        plm.UsedQuantity = 0;
                        int totalcatcount = proCategories.Where(x => x.Id == pl.CategoryId).Count();
                        plm.UsedQuantity = totalcatcount;
                        model.PlanCategoryModels.Add(plm);
                    }
                }
                else
                {
                    model.AllowedTransaction = "0";
                    model.UsedTransaction = "0";
                }
            }
            else
            {
                model.AllowedTransaction = "0";
                model.UsedTransaction = "0";
            }

            PrepareBorrowCartModel(model, cart);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult OrderSummary(bool? prepareAndDisplayOrderReviewData)
        {
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new BorrowCartModel();
            PrepareBorrowCartModel(model, cart, 
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
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _borrowCartService.DeleteBorrowCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _borrowCartService.UpdateBorrowCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
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
            cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new BorrowCartModel();
            PrepareBorrowCartModel(model, cart);
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

        [HttpPost]
        public ActionResult RemoveProductToCart_Catalog(int ProductId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();



            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                if (sci.Product.Id == ProductId)
                    _borrowCartService.DeleteBorrowCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {

                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new BorrowCartModel();
            PrepareBorrowCartModel(model, cart);
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
            return RedirectToRoute("Cart", model);

        }

        [HttpPost]
        public JsonResult borrowproduct_Catalog(int Id)
        {
            int success =0;
            bool successflag = false;
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                 return Json(new { success = 2 }, JsonRequestBehavior.AllowGet);
            String errorMessage = "";
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var currentOrder = _subscriptionService.GetCurrentSubscribedOrder(_workContext.CurrentCustomer.Id);
            if (currentOrder != null)
            {
                if (currentOrder.SubscriptionOrderItems.Count > 0)
                {
                    if (currentOrder.PaymentStatus == PaymentStatus.Paid)
                    {
                        if (currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.MaxNoOfDeliveries > currentOrder.NoOfDeliveries) { 
                            if (cart.Count <= currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.NoOfItemsToBorrow) { 
                                var orderitems = _subscriptionService.GetAllOrderItemsCount(orderId: currentOrder.Id, customerId: currentOrder.CustomerId, createdFromUtc: DateTime.Now.AddMonths(-1), createdToUtc: DateTime.Now.AddDays(1), os: SubscriptionOrderStatus.Complete, ps: PaymentStatus.Paid, ss: ShippingStatus.Shipped);

                                var planCategories = currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.PlanCategories.ToList();
                                List<Category> proCategories = new List<Category>();

                                foreach (BorrowCartItem sct in cart)
                                {
                                    if (sct.Product.ProductCategories.Count > 0)
                                    {
                                        Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                                        if (getparent.ParentCategoryId == 0)
                                        {
                                            proCategories.Add(getparent);
                                        }
                                    }
                                }
                        
                                String s = "";
                                foreach (PlanCategory pl in planCategories)
                                {
                                    if (pl.Category.ParentCategoryId == 0)
                                    {
                                        int totalcatcount = proCategories.Where(x => x.Id == pl.CategoryId).Count();
                                        if (pl.Quantity  >= totalcatcount)
                                        {
                                            successflag = true;   
                                        }
                                        else
                                        {
                                            errorMessage = "You have already borrowed " + cart.Count + " product.";
                                            return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            else
                            {
                                errorMessage = "Borrow list containts " + cart.Count().ToString() + " Product. No of maximus products allowed are " + currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.NoOfItemsToBorrow.ToString();
                                return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            errorMessage = "Maximum number of allowed transactions reached for this month.";
                            return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        errorMessage = "This product cant be borrowed. No Paid Subscription Order found.";
                        return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    errorMessage = "This product cant be borrowed. No Paid Subscription Order found.";
                    return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                errorMessage = "This product cant be borrowed. No Paid Subscription Order found.";
                return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);

            }
            if (successflag == true) { 
                success = _orderProcessingService.PlaceBorrowItem(cart);
                if (success == 1)
                {
                    foreach (BorrowCartItem sct in cart)
                    {
                        _borrowCartService.DeleteBorrowCartItem(sct, ensureOnlyActiveCheckoutAttributes: true);
                    }
                }
            }
            //var model = new BorrowCartModel();
            //PrepareBorrowCartModel(model, cart);
            ////update current warnings

            return Json(new { success = success,message =errorMessage }, JsonRequestBehavior.AllowGet);

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
        
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public ActionResult StartCheckout(FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            //validate attributes
            var checkoutAttributes = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            var checkoutAttributeWarnings = _borrowCartService.GetBorrowCartWarnings(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Count > 0)
            {
                //something wrong, redisplay the page with warnings
                var model = new BorrowCartModel();
                PrepareBorrowCartModel(model, cart, validateCheckoutAttributes: true);
                return View(model);
            }

            //everything is OK
            if (_workContext.CurrentCustomer.IsGuest())
            {
                if (!_orderSettings.AnonymousCheckoutAllowed)
                    return new HttpUnauthorizedResult();
                
                return RedirectToRoute("LoginCheckoutAsGuest", new {returnUrl = Url.RouteUrl("BorrowCart")});
            }
            
            return RedirectToRoute("Checkout");
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
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new BorrowCartModel();
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
                        model.DiscountBox.Message = _localizationService.GetResource("BorrowCart.DiscountCouponCode.Applied");
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
                            model.DiscountBox.Message = _localizationService.GetResource("BorrowCart.DiscountCouponCode.WrongDiscount");
                            model.DiscountBox.IsApplied = false;
                        }
                    }
                }
                else
                {
                    //discount cannot be found
                    model.DiscountBox.Message = _localizationService.GetResource("BorrowCart.DiscountCouponCode.WrongDiscount");
                    model.DiscountBox.IsApplied = false;
                }
            }
            else
            {
                //empty coupon code
                model.DiscountBox.Message = _localizationService.GetResource("BorrowCart.DiscountCouponCode.WrongDiscount");
                model.DiscountBox.IsApplied = false;
            }

            PrepareBorrowCartModel(model, cart);
            return View(model);
        }

     
        [ValidateInput(false)]
        [PublicAntiForgery]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("estimateshipping")]
        public ActionResult GetEstimateShipping(EstimateShippingModel shippingModel, FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new BorrowCartModel();
            model.EstimateShipping.CountryId = shippingModel.CountryId;
            model.EstimateShipping.StateProvinceId = shippingModel.StateProvinceId;
            model.EstimateShipping.ZipPostalCode = shippingModel.ZipPostalCode;
            PrepareBorrowCartModel(model, cart,setEstimateShippingDefaultAddress: false);

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
        public ActionResult OrderTotals(bool isEditable)
        {
            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = PrepareOrderTotalsModel(cart, isEditable);
            return PartialView(model);
        }
  

        [ChildActionOnly]
        public ActionResult FlyoutBorrowCart()
        {
            if (!_borrowCartSettings.MiniBorrowCartEnabled)
                return Content("");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return Content("");

            var model = PrepareMiniBorrowCartModel();
            return PartialView(model);
        }

        #endregion

        #region MyToyBox

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult MyToyBox(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

            Customer customer = customerGuid.HasValue ? 
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToRoute("HomePage");
            var cart = customer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            
            var model = new MyToyBoxModel();

            var currentOrder = _subscriptionService.GetCurrentSubscribedOrder(_workContext.CurrentCustomer.Id);
            if (currentOrder != null)
            {
                if (currentOrder.SubscriptionOrderItems.Count > 0)
                {
                    var plancategories = currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.PlanCategories;
                    List<Category> proCategories = new List<Category>();

                    foreach (BorrowCartItem sct in cart)
                    {
                        if (sct.Product.ProductCategories.Count > 0)
                        {
                            Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                            if (getparent.ParentCategoryId == 0)
                            {
                                proCategories.Add(getparent);
                            }
                        }
                    }
                    //IList<ProductCategory> productCategories = new List<ProductCategory>();
                    //if (cart.Count > 0) { 
                    //    productCategories = _categoryService.GetProductCategoriesByProductId(cart.FirstOrDefault().ProductId);
                    //}
                    String s = "";
                    foreach (PlanCategory pl in plancategories)
                    {
                        PlanCategoryModel plm = new PlanCategoryModel();
                        plm.CategoryName = pl.Category.Name;
                        plm.MyToyBoxQuantity = pl.MyToyBoxQuantity;
                        plm.UsedMyToyBoxQuantity = 0;
                        //if (productCategories.Count > 0)
                        //{
                        //    var cat = GetParentCategory(productCategories.FirstOrDefault().CategoryId);
                        //    if (pl.CategoryId == cat.Id)
                        //    {
                        int totalcatcount = proCategories.Where(x => x.Id == pl.CategoryId).Count();
                                 
                        plm.UsedMyToyBoxQuantity = totalcatcount;
                            //}
                        //}
                        model.PlanCategoryModels.Add(plm);
                    }
                }
                else
                {
                    model.AllowedTransaction = "0";
                    model.UsedTransaction = "0";
                }
            }
            else
            {
                model.AllowedTransaction = "0";
                model.UsedTransaction = "0";
            }

            PrepareMyToyBoxModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        [HttpPost]
        public ActionResult RemoveProductToBox_Catalog(int ProductId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                if (sci.Product.Id == ProductId)
                    _borrowCartService.DeleteBorrowCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {

                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new BorrowCartModel();
            PrepareBorrowCartModel(model, cart);
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
            return RedirectToRoute("MyToyBox", model);

        }

        [HttpPost]
        public ActionResult borrowmytoyboxproduct_Catalog(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");
            var pageCustomer = _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return RedirectToRoute("HomePage");
            String errorMessage = "";
            int success = 0;

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox && sci.Id == Id)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;
            var currentOrder = _subscriptionService.GetCurrentSubscribedOrder(_workContext.CurrentCustomer.Id);
            if (currentOrder != null)
            {
                if (currentOrder.SubscriptionOrderItems.Count > 0)
                {
                    var cartBorrow = _workContext.CurrentCustomer.BorrowCartItems
                    .Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                    if (currentOrder.PaymentStatus == PaymentStatus.Paid)
                    {
                        var orderitems = _subscriptionService.GetAllOrderItemsCount(orderId: currentOrder.Id, customerId: currentOrder.CustomerId, createdFromUtc: DateTime.Now.AddMonths(-1), createdToUtc: DateTime.Now.AddDays(1), os: SubscriptionOrderStatus.Complete, ps: PaymentStatus.Paid, ss: ShippingStatus.Shipped);

                        var planCategories = currentOrder.SubscriptionOrderItems.FirstOrDefault().Plan.PlanCategories.ToList();
                        List<Category> proCategories = new List<Category>();

                        foreach (BorrowCartItem sct in cartBorrow)
                        {
                            if (sct.Product.ProductCategories.Count > 0)
                            {
                                Category getparent = GetParentCategory(sct.Product.ProductCategories.FirstOrDefault().CategoryId);
                                if (getparent.ParentCategoryId == 0)
                                {
                                    proCategories.Add(getparent);
                                }
                            }
                        }

                        //var productCategories = proCategories.GroupBy(x => x.Id).Select(y => y.First());

                        var productCategories = _categoryService.GetProductCategoriesByProductId(cart.FirstOrDefault().ProductId);
                        String s = "";
                        foreach (PlanCategory pl in planCategories)
                        {
                            if (pl.Category.ParentCategoryId == 0)
                            {
                                if (productCategories.Count > 0)
                                {
                                    var cat = GetParentCategory(productCategories.FirstOrDefault().CategoryId);
                                    if (pl.CategoryId == cat.Id)
                                    {
                                        int totalcatcount = proCategories.Where(x => x.Id == cat.Id).Count();
                                        if (pl.Quantity > totalcatcount)
                                        {
                                            if (cart.FirstOrDefault() != null)
                                            {
                                                var warnings = _borrowCartService.AddToCart(_workContext.CurrentCustomer,
                                                        cart.FirstOrDefault().Product, BorrowCartType.BorrowCart,
                                                        _storeContext.CurrentStore.Id,
                                                        cart.FirstOrDefault().AttributesXml, cart.FirstOrDefault().CustomerEnteredPrice,
                                                       cart.FirstOrDefault().Quantity, true);
                                                if (warnings.Count == 0)
                                                    numberOfAddedItems++;
                                                if (warnings.Count == 0) //no warnings ( already in the cart)
                                                {
                                                    //let's remove the item from mytoybox
                                                    _borrowCartService.DeleteBorrowCartItem(cart.FirstOrDefault());
                                                    success = 1;
                                                }
                                                allWarnings.AddRange(warnings);
                                            }
                                        }
                                        else
                                        {
                                            errorMessage = "This product cant be borrowed. You have already borrowed " + cartBorrow.Count + " product. " + "You can borrow " + pl.Quantity + " product from " + cat.Name + " category."; ;
                                        }
                                    }
                                }
                                else
                                {
                                    success = 0;
                                    errorMessage = "Product dont have any category assigned..";
                                }
                            }
                            else
                            {

                            }
                            s = s + ", " + pl.Category.Name;
                        }
                    }
                    else
                    {
                        errorMessage = "This product cant be borrowed. No Paid Subscription Order found.";
                    }
                }
                else
                {
                    errorMessage = "This product cant be borrowed. No Paid Subscription Order found.";
                }
            }
            else
            {
                errorMessage = "This product cant be borrowed. No Paid Subscription Order found.";
            }

            //no items added. redisplay the mytoybox page
            if (allWarnings.Count > 0)
            {
                ErrorNotification(_localizationService.GetResource("MyToyBox.AddToCart.Error"), false);
            }
            //var cart1 = pageCustomer.BorrowCartItems
            //    .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            //var model = new MyToyBoxModel();
            //PrepareMyToyBoxModel(model, cart1, true);
            return Json(new { success = success, message = errorMessage }, JsonRequestBehavior.AllowGet);
            
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("MyToyBox")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateMyToyBox(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
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
                    _borrowCartService.DeleteBorrowCartItem(sci);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _borrowCartService.UpdateBorrowCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated mytoybox
            cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
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
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart))
                return RedirectToRoute("HomePage");

          
            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return RedirectToRoute("HomePage");

            var pageCart = pageCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
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
                    var warnings = _borrowCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.Product, BorrowCartType.BorrowCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.Quantity, true);
                    if (warnings.Count == 0)
                        numberOfAddedItems++;
                    if (_borrowCartSettings.MoveItemsFromMyToyBoxToCart && //settings enabled
                        !customerGuid.HasValue && //own mytoybox
                        warnings.Count == 0) //no warnings ( already in the cart)
                    {
                        //let's remove the item from mytoybox
                        _borrowCartService.DeleteBorrowCartItem(sci);
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

                return RedirectToRoute("BorrowCart");
            }
            else
            {
                //no items added. redisplay the mytoybox page

                if (allWarnings.Count > 0)
                {
                    ErrorNotification(_localizationService.GetResource("MyToyBox.AddToCart.Error"), false);
                }

                var cart = pageCustomer.BorrowCartItems
                    .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
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
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart) || !_borrowCartSettings.EmailMyToyBoxEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
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
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart) || !_borrowCartSettings.EmailMyToyBoxEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox)
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
