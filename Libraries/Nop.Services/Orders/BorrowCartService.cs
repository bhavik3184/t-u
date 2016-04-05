using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class BorrowCartService : IBorrowCartService
    {
        #region Fields

        private readonly IRepository<BorrowCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerService _customerService;
        private readonly BorrowCartSettings _borrowCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="productService">Product settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="borrowCartSettings">Shopping cart settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="permissionService">Permission service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="productAttributeService">Product attribute service</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public BorrowCartService(IRepository<BorrowCartItem> sciRepository,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IProductService productService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IPriceFormatter priceFormatter,
            ICustomerService customerService,
            BorrowCartSettings borrowCartSettings,
            IEventPublisher eventPublisher,
            IPermissionService permissionService, 
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IGenericAttributeService genericAttributeService,
            IProductAttributeService productAttributeService,
            IDateTimeHelper dateTimeHelper)
        {
            this._sciRepository = sciRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._priceFormatter = priceFormatter;
            this._customerService = customerService;
            this._borrowCartSettings = borrowCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._genericAttributeService = genericAttributeService;
            this._productAttributeService = productAttributeService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="borrowCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public virtual void DeleteBorrowCartItem(BorrowCartItem borrowCartItem, bool resetCheckoutData = true, 
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            if (borrowCartItem == null)
                throw new ArgumentNullException("borrowCartItem");

            var customer = borrowCartItem.Customer;
            var storeId = borrowCartItem.StoreId;

            //reset checkout data
            if (resetCheckoutData)
            {
                _customerService.ResetCheckoutData(borrowCartItem.Customer, borrowCartItem.StoreId);
            }

            //delete item
            _sciRepository.Delete(borrowCartItem);

            //reset "HasBorrowCartItems" property used for performance optimization
            customer.HasBorrowCartItems = customer.BorrowCartItems.Count > 0;
            _customerService.UpdateCustomer(customer);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore mytoybox)
                borrowCartItem.BorrowCartType == BorrowCartType.BorrowCart)
            {
                var cart = customer.BorrowCartItems
                    .Where(x => x.BorrowCartType == BorrowCartType.BorrowCart)
                    .LimitPerStore(storeId)
                    .ToList();

                
            }

            //event notification
            _eventPublisher.EntityDeleted(borrowCartItem);
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredBorrowCartItems(DateTime olderThanUtc)
        {
            var query = from sci in _sciRepository.Table
                           where sci.UpdatedOnUtc < olderThanUtc
                           select sci;

            var cartItems = query.ToList();
            foreach (var cartItem in cartItems)
                _sciRepository.Delete(cartItem);
            return cartItems.Count;
        }

        /// <summary>
        /// Validates required products (products which require some other products to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredProductWarnings(Customer customer,
            BorrowCartType borrowCartType, Product product,
            int storeId, bool automaticallyAddRequiredProductsIfEnabled)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var cart = customer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == borrowCartType)
                .LimitPerStore(storeId)
                .ToList();

            var warnings = new List<string>();

            if (product.RequireOtherProducts)
            {
                var requiredProducts = new List<Product>();
                foreach (var id in product.ParseRequiredProductIds())
                {
                    var rp = _productService.GetProductById(id);
                    if (rp != null)
                        requiredProducts.Add(rp);
                }
                
                foreach (var rp in requiredProducts)
                {
                    //ensure that product is in the cart
                    bool alreadyInTheCart = false;
                    foreach (var sci in cart)
                    {
                        if (sci.ProductId == rp.Id)
                        {
                            alreadyInTheCart = true;
                            break;
                        }
                    }
                    //not in the cart
                    if (!alreadyInTheCart)
                    {

                        if (product.AutomaticallyAddRequiredProducts)
                        {
                            //add to cart (if possible)
                            if (automaticallyAddRequiredProductsIfEnabled)
                            {
                                //pass 'false' for 'automaticallyAddRequiredProductsIfEnabled' to prevent circular references
                                var addToCartWarnings = AddToCart(customer: customer,
                                    product: rp, 
                                    borrowCartType: borrowCartType,
                                    storeId: storeId,
                                    automaticallyAddRequiredProductsIfEnabled: false);
                                if (addToCartWarnings.Count > 0)
                                {
                                    //a product wasn't atomatically added for some reasons

                                    //don't display specific errors from 'addToCartWarnings' variable
                                    //display only generic error
                                    warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                                }
                            }
                            else
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                            }
                        }
                        else
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                        }
                    }
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Validates a product for standard properties
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(Customer customer, BorrowCartType borrowCartType,
            Product product, string attributesXml, decimal customerEnteredPrice,
            int quantity)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //deleted
            if (product.Deleted)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.ProductDeleted"));
                return warnings;
            }

            //published
            if (!product.Published)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.ProductUnpublished"));
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                warnings.Add("This is not simple product");
            }
            
            //ACL
            if (!_aclService.Authorize(product, customer))
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.ProductUnpublished"));
            }

            //Store mapping
            if (!_storeMappingService.Authorize(product, _storeContext.CurrentStore.Id))
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.ProductUnpublished"));
            }

            //disabled "add to cart" button
            if (borrowCartType == BorrowCartType.BorrowCart && product.DisableBuyButton)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.BuyingDisabled"));
            }

            //disabled "add to mytoybox" button
            if (borrowCartType == BorrowCartType.MyToyBox && product.DisableMyToyBoxButton)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.MyToyBoxDisabled"));
            }

            //call for price
            if (borrowCartType == BorrowCartType.BorrowCart && product.CallForPrice)
            {
                warnings.Add(_localizationService.GetResource("Products.CallForPrice"));
            }

            //customer entered price
            if (product.CustomerEntersPrice)
            {
                if (customerEnteredPrice < product.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > product.MaximumCustomerEnteredPrice)
                {
                    decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.CustomerEnteredPrice.RangeError"),
                        _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                        _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false)));
                }
            }

            //quantity validation
            var hasQtyWarnings = false;
            if (quantity < product.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.MinimumQuantity"), product.OrderMinimumQuantity));
                hasQtyWarnings = true;
            }
            if (quantity > product.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.MaximumQuantity"), product.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }
            var allowedQuantities = product.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

            var validateOutOfStock = borrowCartType == BorrowCartType.BorrowCart || !_borrowCartSettings.AllowOutOfStockItemsToBeAddedToMyToyBox;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (product.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        {
                            //do nothing
                        }
                        break;
                    case ManageInventoryMethod.ManageStock:
                        {
                            if (product.BackorderMode == BackorderMode.NoBackorders)
                            {
                                int maximumQuantityCanBeAdded = product.GetTotalStockQuantity();
                                if (maximumQuantityCanBeAdded < quantity)
                                {
                                    if (maximumQuantityCanBeAdded <= 0)
                                        warnings.Add(_localizationService.GetResource("BorrowCart.OutOfStock"));
                                    else
                                        warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }
                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        {
                            var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                            if (combination != null)
                            {
                                //combination exists
                                //let's check stock level
                                if (!combination.AllowOutOfStockOrders && combination.StockQuantity < quantity)
                                {
                                    int maximumQuantityCanBeAdded = combination.StockQuantity;
                                    if (maximumQuantityCanBeAdded <= 0)
                                    {
                                        warnings.Add(_localizationService.GetResource("BorrowCart.OutOfStock"));
                                    }
                                    else
                                    {
                                        warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                    }
                                }
                            }
                            else
                            {
                                //combination doesn't exist
                                if (product.AllowAddingOnlyExistingAttributeCombinations)
                                {
                                    //maybe, is it better  to display something like "No such product/combination" message?
                                    warnings.Add(_localizationService.GetResource("BorrowCart.OutOfStock"));
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //availability dates
            bool availableStartDateError = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(now) > 0)
                {
                    warnings.Add(_localizationService.GetResource("BorrowCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }
            if (product.AvailableEndDateTimeUtc.HasValue && !availableStartDateError)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableEndDateTime.CompareTo(now) < 0)
                {
                    warnings.Add(_localizationService.GetResource("BorrowCart.NotAvailable"));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetBorrowCartItemAttributeWarnings(Customer customer, 
            BorrowCartType borrowCartType,
            Product product, 
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = _productAttributeParser.ParseProductAttributeMappings(attributesXml);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
            }
            foreach (var attribute in attributes1)
            {
                if (attribute.Product != null)
                {
                    if (attribute.Product.Id != product.Id)
                    {
                        warnings.Add("Attribute error");
                    }
                }
                else
                {
                    warnings.Add("Attribute error");
                    return warnings;
                }
            }

            //validate required product attributes (whether they're chosen/selected/entered)
            var attributes2 = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            if (ignoreNonCombinableAttributes)
            {
                attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
            }
            //validate conditional attributes only (if specified)
            attributes2 = attributes2.Where(x =>
            {
                var conditionMet = _productAttributeParser.IsConditionMet(x, attributesXml);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToList();
            foreach (var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var a1 in attributes1)
                    {
                        if (a1.Id == a2.Id)
                        {
                            var attributeValuesStr = _productAttributeParser.ParseValues(attributesXml, a1.Id);
                            foreach (string str1 in attributeValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        var notFoundWarning = !string.IsNullOrEmpty(a2.TextPrompt) ?
                            a2.TextPrompt : 
                            string.Format(_localizationService.GetResource("BorrowCart.SelectAttribute"), a2.ProductAttribute.GetLocalized(a => a.Name));
                        
                        warnings.Add(notFoundWarning);
                    }
                }

                if (a2.AttributeControlType == AttributeControlType.ReadonlyCheckboxes)
                {
                    //customers cannot edit read-only attributes
                    var allowedReadOnlyValueIds = _productAttributeService.GetProductAttributeValues(a2.Id)
                        .Where(x => x.IsPreSelected)
                        .Select(x => x.Id)
                        .ToArray();

                    var selectedReadOnlyValueIds = _productAttributeParser.ParseProductAttributeValues(attributesXml)
                        .Where(x => x.ProductAttributeMappingId == a2.Id)
                        .Select(x => x.Id)
                        .ToArray();

                    if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
                    {
                        warnings.Add("You cannot change read-only values");
                    }
                }
            }

            //validation rules
            foreach (var pam in attributes2)
            {
                if (!pam.ValidationRulesAllowed())
                    continue;
                
                //minimum length
                if (pam.ValidationMinLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _productAttributeParser.ParseValues(attributesXml, pam.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.TextboxMinimumLength"), pam.ProductAttribute.GetLocalized(a => a.Name), pam.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (pam.ValidationMaxLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _productAttributeParser.ParseValues(attributesXml, pam.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMaxLength.Value < enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.TextboxMaximumLength"), pam.ProductAttribute.GetLocalized(a => a.Name), pam.ValidationMaxLength.Value));
                        }
                    }
                }
            }

            if (warnings.Count > 0)
                return warnings;

            //validate bundled products
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                {
                    if (ignoreNonCombinableAttributes && attributeValue.ProductAttributeMapping.IsNonCombinable())
                        continue;

                    //associated product (bundle)
                    var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                    if (associatedProduct != null)
                    {
                        var totalQty = quantity * attributeValue.Quantity;
                        var associatedProductWarnings = GetBorrowCartItemWarnings(customer,
                            borrowCartType, associatedProduct, _storeContext.CurrentStore.Id,
                            "", decimal.Zero, totalQty, false);
                        foreach (var associatedProductWarning in associatedProductWarnings)
                        {
                            var attributeName = attributeValue.ProductAttributeMapping.ProductAttribute.GetLocalized(a => a.Name);
                            var attributeValueName = attributeValue.GetLocalized(a => a.Name);
                            warnings.Add(string.Format(
                                _localizationService.GetResource("BorrowCart.AssociatedAttributeWarning"), 
                                attributeName, attributeValueName, associatedProductWarning));
                        }
                    }
                    else
                    {
                        warnings.Add(string.Format("Associated product cannot be loaded - {0}", attributeValue.AssociatedProductId));
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetBorrowCartItemGiftCardWarnings(BorrowCartType borrowCartType,
            Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //gift cards
            if (product.IsGiftCard)
            {
                string giftCardRecipientName;
                string giftCardRecipientEmail;
                string giftCardSenderName;
                string giftCardSenderEmail;
                string giftCardMessage;
                _productAttributeParser.GetGiftCardAttribute(attributesXml,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(_localizationService.GetResource("BorrowCart.RecipientNameError"));

                if (product.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                        warnings.Add(_localizationService.GetResource("BorrowCart.RecipientEmailError"));
                }

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(_localizationService.GetResource("BorrowCart.SenderNameError"));

                if (product.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
                        warnings.Add(_localizationService.GetResource("BorrowCart.SenderEmailError"));
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item for rental products
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRentalProductWarnings(Product product,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            
            var warnings = new List<string>();

            if (!product.IsRental)
                return warnings;

            if (!rentalStartDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.Rental.EnterStartDate"));
                return warnings;
            }
            if (!rentalEndDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.Rental.EnterEndDate"));
                return warnings;
            }
            if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.Rental.StartDateLessEndDate"));
                return warnings;
            }

            //allowed start date should be the future date
            //we should compare rental start date with a store local time
            //but we what if a store works in distinct timezones? how we should handle it? skip it for now
            //we also ignore hours (anyway not supported yet)
            //today
            DateTime nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
            var todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
            DateTime todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
            //dates are entered in store timezone (e.g. like in hotels)
            DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
            //but we what if dates should be entered in a customer timezone?
            //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
            if (todayDtUtc.CompareTo(startDateUtc) > 0)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.Rental.StartDateShouldBeFuture"));
                return warnings;
            }

            return warnings;
        }


        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetBorrowCartItemWarnings(Customer customer, BorrowCartType borrowCartType,
            Product product, int storeId,
            string attributesXml, decimal customerEnteredPrice,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
            bool getRentalWarnings = true)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();
            
            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(customer, borrowCartType, product, attributesXml, customerEnteredPrice, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetBorrowCartItemAttributeWarnings(customer, borrowCartType, product, quantity, attributesXml));

            //gift cards
            if (getGiftCardWarnings)
                warnings.AddRange(GetBorrowCartItemGiftCardWarnings(borrowCartType, product, attributesXml));

            //required products
            if (getRequiredProductWarnings)
                warnings.AddRange(GetRequiredProductWarnings(customer, borrowCartType, product, storeId, automaticallyAddRequiredProductsIfEnabled));

           
            
            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetBorrowCartWarnings(IList<BorrowCartItem> borrowCart, 
            string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartProducts = false;
            bool hasRecurringProducts = false;

            foreach (var sci in borrowCart)
            {
                var product = sci.Product;
                if (product == null)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.CannotLoadProduct"), sci.ProductId));
                    return warnings;
                }

                if (product.IsRecurring)
                    hasRecurringProducts = true;
                else
                    hasStandartProducts = true;
            }

            //don't mix standard and recurring products
            if (hasStandartProducts && hasRecurringProducts)
                warnings.Add(_localizationService.GetResource("BorrowCart.CannotMixStandardAndAutoshipProducts"));

            //recurring cart validation
            if (hasRecurringProducts)
            {
                int cycleLength;
                RecurringProductCyclePeriod cyclePeriod;
                int totalCycles;
                string cyclesError = borrowCart.GetRecurringCycleInfo(_localizationService,
                    out cycleLength, out cyclePeriod, out totalCycles);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Found shopping cart item</returns>
        public virtual BorrowCartItem FindBorrowCartItemInTheCart(IList<BorrowCartItem> borrowCart,
            BorrowCartType borrowCartType,
            Product product,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, 
            DateTime? rentalEndDate = null)
        {
            if (borrowCart == null)
                throw new ArgumentNullException("borrowCart");

            if (product == null)
                throw new ArgumentNullException("product");

            foreach (var sci in borrowCart.Where(a => a.BorrowCartType == borrowCartType))
            {
                if (sci.ProductId == product.Id)
                {
                    //attributes
                    bool attributesEqual = _productAttributeParser.AreProductAttributesEqual(sci.AttributesXml, attributesXml, false);

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (sci.Product.IsGiftCard)
                    {
                        string giftCardRecipientName1;
                        string giftCardRecipientEmail1;
                        string giftCardSenderName1;
                        string giftCardSenderEmail1;
                        string giftCardMessage1;
                        _productAttributeParser.GetGiftCardAttribute(attributesXml,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2;
                        string giftCardRecipientEmail2;
                        string giftCardSenderName2;
                        string giftCardSenderEmail2;
                        string giftCardMessage2;
                        _productAttributeParser.GetGiftCardAttribute(sci.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for products which require customers to enter a price)
                    bool customerEnteredPricesEqual = true;
                    if (sci.Product.CustomerEntersPrice)
                        //TODO should we use RoundingHelper.RoundPrice here?
                        customerEnteredPricesEqual = Math.Round(sci.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);

                    //rental products
                    bool rentalInfoEqual = true;
                    //if (sci.Product.IsRental)
                    //{
                    //    rentalInfoEqual = sci.RentalStartDateUtc == rentalStartDate && sci.RentalEndDateUtc == rentalEndDate;
                    //}

                    //found?
                    if (attributesEqual && giftCardInfoSame && customerEnteredPricesEqual && rentalInfoEqual)
                        return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="product">Product</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> AddToCart(Customer customer, Product product,
            BorrowCartType borrowCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();
            //if (borrowCartType == BorrowCartType.BorrowCart && !_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart, customer))
            //{
            //    warnings.Add("Shopping cart is disabled");
            //    return warnings;
            //}
            //if (borrowCartType == BorrowCartType.MyToyBox && !_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox, customer))
            //{
            //    warnings.Add("MyToyBox is disabled");
            //    return warnings;
            //}
            if (customer.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add(_localizationService.GetResource("BorrowCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            _customerService.ResetCheckoutData(customer, storeId);

            var cart = customer.BorrowCartItems
                .Where(sci => sci.BorrowCartType == borrowCartType)
                .LimitPerStore(storeId)
                .ToList();

            var borrowCartItem = FindBorrowCartItemInTheCart(cart,
                borrowCartType, product, attributesXml, customerEnteredPrice);

            if (borrowCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = borrowCartItem.Quantity + quantity;
                warnings.AddRange(GetBorrowCartItemWarnings(customer, borrowCartType, product,
                    storeId, attributesXml, 
                    customerEnteredPrice ,
                    newQuantity, automaticallyAddRequiredProductsIfEnabled));

                if (warnings.Count == 0)
                {
                    borrowCartItem.AttributesXml = attributesXml;
                    borrowCartItem.Quantity = newQuantity;
                    borrowCartItem.UpdatedOnUtc = DateTime.UtcNow;
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityUpdated(borrowCartItem);
                }
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(GetBorrowCartItemWarnings(customer, borrowCartType, product,
                    storeId, attributesXml, customerEnteredPrice,
                    quantity, automaticallyAddRequiredProductsIfEnabled));
                if (warnings.Count == 0)
                {
                    //maximum items validation
                    switch (borrowCartType)
                    {
                        case BorrowCartType.BorrowCart:
                            {
                                if (cart.Count >= _borrowCartSettings.MaximumBorrowCartItems)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.MaximumBorrowCartItems"), _borrowCartSettings.MaximumBorrowCartItems));
                                    return warnings;
                                }
                            }
                            break;
                        case BorrowCartType.MyToyBox:
                            {
                                if (cart.Count >= _borrowCartSettings.MaximumMyToyBoxItems)
                                {
                                  //  warnings.Add(string.Format(_localizationService.GetResource("BorrowCart.MaximumMyToyBoxItems"), _borrowCartSettings.MaximumMyToyBoxItems));
                                    //return warnings;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    DateTime now = DateTime.UtcNow;
                    borrowCartItem = new BorrowCartItem
                    {
                        BorrowCartType = borrowCartType,
                        StoreId = storeId,
                        Product = product,
                        AttributesXml = attributesXml,
                        CustomerEnteredPrice = customerEnteredPrice,
                        Quantity = quantity,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now
                    };
                    customer.BorrowCartItems.Add(borrowCartItem);
                    _customerService.UpdateCustomer(customer);


                    //updated "HasBorrowCartItems" property used for performance optimization
                    customer.HasBorrowCartItems = customer.BorrowCartItems.Count > 0;
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityInserted(borrowCartItem);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartItemId">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">New customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateBorrowCartItem(Customer customer,
            int borrowCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            int quantity = 1, bool resetCheckoutData = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var warnings = new List<string>();

            var borrowCartItem = customer.BorrowCartItems.FirstOrDefault(sci => sci.Id == borrowCartItemId);
            if (borrowCartItem != null)
            {
                if (resetCheckoutData)
                {
                    //reset checkout data
                    _customerService.ResetCheckoutData(customer, borrowCartItem.StoreId);
                }
                if (quantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetBorrowCartItemWarnings(customer, borrowCartItem.BorrowCartType,
                        borrowCartItem.Product, borrowCartItem.StoreId,
                        attributesXml, customerEnteredPrice, 
                        quantity, false));
                    if (warnings.Count == 0)
                    {
                        //if everything is OK, then update a shopping cart item
                        borrowCartItem.Quantity = quantity;
                        borrowCartItem.AttributesXml = attributesXml;
                        borrowCartItem.CustomerEnteredPrice = customerEnteredPrice;
                        borrowCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _customerService.UpdateCustomer(customer);

                        //event notification
                        _eventPublisher.EntityUpdated(borrowCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    DeleteBorrowCartItem(borrowCartItem, resetCheckoutData, true);
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        public virtual void MigrateBorrowCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            if (fromCustomer == null)
                throw new ArgumentNullException("fromCustomer");
            if (toCustomer == null)
                throw new ArgumentNullException("toCustomer");

            if (fromCustomer.Id == toCustomer.Id)
                return; //the same customer

            //shopping cart items
            var fromCart = fromCustomer.BorrowCartItems.ToList();
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                AddToCart(toCustomer, sci.Product, sci.BorrowCartType, sci.StoreId, 
                    sci.AttributesXml, sci.CustomerEnteredPrice,
                     sci.Quantity, false);
            }
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                DeleteBorrowCartItem(sci);
            }
            
            //migrate gift card and discount coupon codes
            if (includeCouponCodes)
            {
                //discount
                var discountCouponCode  = fromCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
                if (!String.IsNullOrEmpty(discountCouponCode))
                    _genericAttributeService.SaveAttribute(toCustomer, SystemCustomerAttributeNames.DiscountCouponCode, discountCouponCode);
                
                //gift card
                foreach (var gcCode in fromCustomer.ParseAppliedGiftCardCouponCodes())
                    toCustomer.ApplyGiftCardCouponCode(gcCode);

                //save customer
                _customerService.UpdateCustomer(toCustomer);
                 
            }
        }

        #endregion
    }
}