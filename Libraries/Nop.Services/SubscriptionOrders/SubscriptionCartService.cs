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
using Nop.Core.Domain.Payments;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class SubscriptionCartService : ISubscriptionCartService
    {
        #region Fields

        private readonly IRepository<SubscriptionCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IPlanService _planService;
        private readonly ILocalizationService _localizationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerService _customerService;
        private readonly SubscriptionCartSettings _borrowCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductAttributeParser _planAttributeParser;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="planService">Plan settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="planAttributeParser">Plan attribute parser</param>
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
        /// <param name="planAttributeService">Plan attribute service</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public SubscriptionCartService(IRepository<SubscriptionCartItem> sciRepository,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService,
            ISubscriptionOrderService subscriptionService,
            IPlanService planService,
            ILocalizationService localizationService,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IPriceFormatter priceFormatter,
            ICustomerService customerService,
            SubscriptionCartSettings borrowCartSettings,
            IEventPublisher eventPublisher,
            IPermissionService permissionService, 
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IGenericAttributeService genericAttributeService,
            IDateTimeHelper dateTimeHelper,
            IProductAttributeParser planAttributeParser)
        {
            this._sciRepository = sciRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._subscriptionService = subscriptionService;
            this._planService = planService;
            this._localizationService = localizationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._priceFormatter = priceFormatter;
            this._customerService = customerService;
            this._borrowCartSettings = borrowCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._genericAttributeService = genericAttributeService;
            this._dateTimeHelper = dateTimeHelper;
            this._planAttributeParser = planAttributeParser;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="borrowCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public virtual void DeleteSubscriptionCartItem(SubscriptionCartItem borrowCartItem, bool resetCheckoutData = true, 
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

            //reset "HasSubscriptionCartItems" property used for performance optimization
            customer.HasSubscriptionCartItems = customer.SubscriptionCartItems.Count > 0;
            _customerService.UpdateCustomer(customer);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore mytoybox)
                borrowCartItem.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
            {
                var cart = customer.SubscriptionCartItems
                    .Where(x => x.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                    .LimitPerStore(storeId)
                    .ToList();

                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, storeId);
                checkoutAttributesXml = _checkoutAttributeParser.EnsureOnlyActiveAttributes(checkoutAttributesXml, cart);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CheckoutAttributes, checkoutAttributesXml, storeId);
            }

            //event notification
            _eventPublisher.EntityDeleted(borrowCartItem);
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredSubscriptionCartItems(DateTime olderThanUtc)
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
        /// Validates required plans (plans which require some other plans to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="automaticallyAddRequiredPlansIfEnabled">Automatically add required plans if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredPlanWarnings(Customer customer,
            SubscriptionCartType borrowCartType, Plan plan,
            int storeId, bool automaticallyAddRequiredPlansIfEnabled)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (plan == null)
                throw new ArgumentNullException("plan");

            var cart = customer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == borrowCartType)
                .LimitPerStore(storeId)
                .ToList();

            var warnings = new List<string>();

            if (plan.RequireOtherPlans)
            {
                var requiredPlans = new List<Plan>();
                foreach (var id in plan.ParseRequiredPlanIds())
                {
                    var rp = _planService.GetPlanById(id);
                    if (rp != null)
                        requiredPlans.Add(rp);
                }
                
                foreach (var rp in requiredPlans)
                {
                    //ensure that plan is in the cart
                    bool alreadyInTheCart = false;
                    foreach (var sci in cart)
                    {
                        if (sci.PlanId == rp.Id)
                        {
                            alreadyInTheCart = true;
                            break;
                        }
                    }
                    //not in the cart
                    if (!alreadyInTheCart)
                    {

                        if (plan.AutomaticallyAddRequiredPlans)
                        {
                            //add to cart (if possible)
                            if (automaticallyAddRequiredPlansIfEnabled)
                            {
                                //pass 'false' for 'automaticallyAddRequiredPlansIfEnabled' to prevent circular references
                                var addToCartWarnings = AddToCart(customer: customer,
                                    plan: rp, 
                                    borrowCartType: borrowCartType,
                                    storeId: storeId,
                                    automaticallyAddRequiredPlansIfEnabled: false);
                                if (addToCartWarnings.Count > 0)
                                {
                                    //a plan wasn't atomatically added for some reasons

                                    //don't display specific errors from 'addToCartWarnings' variable
                                    //display only generic error
                                    warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.RequiredPlanWarning"), rp.GetLocalized(x => x.Name)));
                                }
                            }
                            else
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.RequiredPlanWarning"), rp.GetLocalized(x => x.Name)));
                            }
                        }
                        else
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.RequiredPlanWarning"), rp.GetLocalized(x => x.Name)));
                        }
                    }
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Validates a plan for standard properties
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(Customer customer, SubscriptionCartType borrowCartType,
            Plan plan, string attributesXml, decimal customerEnteredPrice,
            int quantity)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (plan == null)
                throw new ArgumentNullException("plan");

            var warnings = new List<string>();

            //deleted
            if (plan.Deleted)
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.PlanDeleted"));
                return warnings;
            }

            //published
            if (!plan.Published)
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.PlanUnpublished"));
            }

            //we can add only simple plans
            //if (plan.PlanType != PlanType.SimplePlan)
            //{
            //    warnings.Add("This is not simple plan");
            //}
            
            //ACL
            if (!_aclService.Authorize(plan, customer))
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.PlanUnpublished"));
            }

            //Store mapping
            if (!_storeMappingService.Authorize(plan, _storeContext.CurrentStore.Id))
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.PlanUnpublished"));
            }

            //disabled "add to cart" button
            if (borrowCartType == SubscriptionCartType.SubscriptionCart && plan.DisableBuyButton)
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.BuyingDisabled"));
            }

            //disabled "add to mytoybox" button
            if (borrowCartType == SubscriptionCartType.MyToyBox && plan.DisableMyToyBoxButton)
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.MyToyBoxDisabled"));
            }

            //call for price
            if (borrowCartType == SubscriptionCartType.SubscriptionCart && plan.CallForPrice)
            {
                warnings.Add(_localizationService.GetResource("Plans.CallForPrice"));
            }

            //customer entered price
            if (plan.CustomerEntersPrice)
            {
                if (customerEnteredPrice < plan.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > plan.MaximumCustomerEnteredPrice)
                {
                    decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(plan.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(plan.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.CustomerEnteredPrice.RangeError"),
                        _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                        _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false)));
                }
            }

            //quantity validation
            var hasQtyWarnings = false;
            if (quantity < plan.SubscriptionMinimumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.MinimumQuantity"), plan.SubscriptionMinimumQuantity));
                hasQtyWarnings = true;
            }
            //if (quantity > plan.SubscriptionOrderMaximumQuantity)
            //{
            //    warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.MaximumQuantity"), plan.SubscriptionOrderMaximumQuantity));
            //    hasQtyWarnings = true;
            //}
            var allowedQuantities = plan.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

            var validateOutOfStock = borrowCartType == SubscriptionCartType.SubscriptionCart || !_borrowCartSettings.AllowOutOfStockItemsToBeAddedToMyToyBox;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (plan.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        {
                            //do nothing
                        }
                        break;
                    case ManageInventoryMethod.ManageStock:
                        {
                            if (plan.BackorderMode == BackorderMode.NoBackorders)
                            {
                                //int maximumQuantityCanBeAdded = plan.GetTotalStockQuantity();
                                //if (maximumQuantityCanBeAdded < quantity)
                                //{
                                //    if (maximumQuantityCanBeAdded <= 0)
                                //        warnings.Add(_localizationService.GetResource("SubscriptionCart.OutOfStock"));
                                //    else
                                //        warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                //}
                            }
                        }
                        break;
                    
                    default:
                        break;
                }
            }

            //availability dates
            bool availableStartDateError = false;
            if (plan.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableStartDateTime = DateTime.SpecifyKind(plan.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(now) > 0)
                {
                    warnings.Add(_localizationService.GetResource("SubscriptionCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }
            if (plan.AvailableEndDateTimeUtc.HasValue && !availableStartDateError)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableEndDateTime = DateTime.SpecifyKind(plan.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableEndDateTime.CompareTo(now) < 0)
                {
                    warnings.Add(_localizationService.GetResource("SubscriptionCart.NotAvailable"));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetSubscriptionCartItemAttributeWarnings(Customer customer, 
            SubscriptionCartType borrowCartType,
            Plan plan, 
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            var warnings = new List<string>();

            //ensure it's our attributes
               
             

            if (warnings.Count > 0)
                return warnings;

            

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetSubscriptionCartItemGiftCardWarnings(SubscriptionCartType borrowCartType,
            Plan plan, string attributesXml)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            var warnings = new List<string>();

            //gift cards
            if (plan.IsGiftCard)
            {
                string giftCardRecipientName;
                string giftCardRecipientEmail;
                string giftCardSenderName;
                string giftCardSenderEmail;
                string giftCardMessage;
                _planAttributeParser.GetGiftCardAttribute(attributesXml,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(_localizationService.GetResource("SubscriptionCart.RecipientNameError"));

                if (plan.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                        warnings.Add(_localizationService.GetResource("SubscriptionCart.RecipientEmailError"));
                }

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(_localizationService.GetResource("SubscriptionCart.SenderNameError"));

                if (plan.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
                        warnings.Add(_localizationService.GetResource("SubscriptionCart.SenderEmailError"));
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item for rental plans
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRentalPlanWarnings(Plan plan,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");
            
            var warnings = new List<string>();

            if (!plan.IsRental)
                return warnings;

            if (!rentalStartDate.HasValue)
            {
               // warnings.Add(_localizationService.GetResource("SubscriptionCart.Rental.EnterStartDate"));
                return warnings;
            }
            if (!rentalEndDate.HasValue)
            {
             //   warnings.Add(_localizationService.GetResource("SubscriptionCart.Rental.EnterEndDate"));
                return warnings;
            }
            if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
            {
               // warnings.Add(_localizationService.GetResource("SubscriptionCart.Rental.StartDateLessEndDate"));
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
                warnings.Add(_localizationService.GetResource("SubscriptionCart.Rental.StartDateShouldBeFuture"));
                return warnings;
            }

            return warnings;
        }


        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredPlansIfEnabled">Automatically add required plans if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a plan for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate plan attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredPlanWarnings">A value indicating whether we should validate required plans (plans which require other plans to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetSubscriptionCartItemWarnings(Customer customer, SubscriptionCartType borrowCartType,
            Plan plan, int storeId,
            string attributesXml, decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredPlansIfEnabled = true,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredPlanWarnings = true,
            bool getRentalWarnings = true)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            var warnings = new List<string>();
            
            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(customer, borrowCartType, plan, attributesXml, customerEnteredPrice, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetSubscriptionCartItemAttributeWarnings(customer, borrowCartType, plan, quantity, attributesXml));

            //gift cards
            if (getGiftCardWarnings)
                warnings.AddRange(GetSubscriptionCartItemGiftCardWarnings(borrowCartType, plan, attributesXml));

            //required plans
            if (getRequiredPlanWarnings)
                warnings.AddRange(GetRequiredPlanWarnings(customer, borrowCartType, plan, storeId, automaticallyAddRequiredPlansIfEnabled));

            //rental plans
            if (getRentalWarnings)
                warnings.AddRange(GetRentalPlanWarnings(plan, rentalStartDate, rentalEndDate));
            
            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetSubscriptionCartWarnings(IList<SubscriptionCartItem> borrowCart, 
            string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartPlans = false;
            bool hasRecurringPlans = false;

            foreach (var sci in borrowCart)
            {
                var plan = sci.Plan;
                if (plan == null)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.CannotLoadPlan"), sci.PlanId));
                    return warnings;
                }

                if (plan.IsRecurring)
                    hasRecurringPlans = true;
                else
                    hasStandartPlans = true;
            }

            //don't mix standard and recurring plans
            if (hasStandartPlans && hasRecurringPlans)
                warnings.Add(_localizationService.GetResource("SubscriptionCart.CannotMixStandardAndAutoshipPlans"));

            //recurring cart validation
            if (hasRecurringPlans)
            {
                int cycleLength;
                RecurringPlanCyclePeriod cyclePeriod;
                int totalCycles;
                string cyclesError = borrowCart.GetRecurringCycleInfo(_localizationService,
                    out cycleLength, out cyclePeriod, out totalCycles);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (validateCheckoutAttributes)
            {
                //selected attributes
                var attributes1 = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttributesXml);

                //existing checkout attributes
                var attributes2 = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !borrowCart.RequiresShipping());
                foreach (var a2 in attributes2)
                {
                    if (a2.IsRequired)
                    {
                        bool found = false;
                        //selected checkout attributes
                        foreach (var a1 in attributes1)
                        {
                            if (a1.Id == a2.Id)
                            {
                                var attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                                foreach (string str1 in attributeValuesStr)
                                    if (!String.IsNullOrEmpty(str1.Trim()))
                                    {
                                        found = true;
                                        break;
                                    }
                            }
                        }

                        //if not found
                        if (!found)
                        {
                            if (!string.IsNullOrEmpty(a2.GetLocalized(a => a.TextPrompt)))
                                warnings.Add(a2.GetLocalized(a => a.TextPrompt));
                            else
                                warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.SelectAttribute"), a2.GetLocalized(a => a.Name)));
                        }
                    }
                }

                //now validation rules

                //minimum length
                foreach (var ca in attributes2)
                {
                    if (ca.ValidationMinLength.HasValue)
                    {
                        if (ca.AttributeControlType == AttributeControlType.TextBox ||
                            ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            var valuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id);
                            var enteredText = valuesStr.FirstOrDefault();
                            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                            if (ca.ValidationMinLength.Value > enteredTextLength)
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.TextboxMinimumLength"), ca.GetLocalized(a => a.Name), ca.ValidationMinLength.Value));
                            }
                        }
                    }

                    //maximum length
                    if (ca.ValidationMaxLength.HasValue)
                    {
                        if (ca.AttributeControlType == AttributeControlType.TextBox ||
                            ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            var valuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id);
                            var enteredText = valuesStr.FirstOrDefault();
                            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                            if (ca.ValidationMaxLength.Value < enteredTextLength)
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.TextboxMaximumLength"), ca.GetLocalized(a => a.Name), ca.ValidationMaxLength.Value));
                            }
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Found shopping cart item</returns>
        public virtual SubscriptionCartItem FindSubscriptionCartItemInTheCart(IList<SubscriptionCartItem> borrowCart,
            SubscriptionCartType borrowCartType,
            Plan plan,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, 
            DateTime? rentalEndDate = null)
        {
            if (borrowCart == null)
                throw new ArgumentNullException("borrowCart");

            if (plan == null)
                throw new ArgumentNullException("plan");

            foreach (var sci in borrowCart.Where(a => a.SubscriptionCartType == borrowCartType))
            {
                if (sci.PlanId == plan.Id)
                {
                    //attributes
                    bool attributesEqual = true;

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (sci.Plan.IsGiftCard)
                    {
                        string giftCardRecipientName1;
                        string giftCardRecipientEmail1;
                        string giftCardSenderName1;
                        string giftCardSenderEmail1;
                        string giftCardMessage1;
                        _planAttributeParser.GetGiftCardAttribute(attributesXml,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2;
                        string giftCardRecipientEmail2;
                        string giftCardSenderName2;
                        string giftCardSenderEmail2;
                        string giftCardMessage2;
                        _planAttributeParser.GetGiftCardAttribute(sci.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for plans which require customers to enter a price)
                    bool customerEnteredPricesEqual = true;
                    if (sci.Plan.CustomerEntersPrice)
                        //TODO should we use RoundingHelper.RoundPrice here?
                        customerEnteredPricesEqual = Math.Round(sci.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);

                    //rental plans
                    bool rentalInfoEqual = true;
                    if (sci.Plan.IsRental)
                    {
                        rentalInfoEqual = sci.RentalStartDateUtc == rentalStartDate && sci.RentalEndDateUtc == rentalEndDate;
                    }

                    //found?
                    if (attributesEqual && giftCardInfoSame && customerEnteredPricesEqual && rentalInfoEqual)
                        return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a plan to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="plan">Plan</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredPlansIfEnabled">Automatically add required plans if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> AddToCart(Customer customer, Plan plan,
            SubscriptionCartType borrowCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredPlansIfEnabled = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (plan == null)
                throw new ArgumentNullException("plan");

            var warnings = new List<string>();
            if (borrowCartType == SubscriptionCartType.SubscriptionCart && !_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart, customer))
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }
            if (borrowCartType == SubscriptionCartType.MyToyBox && !_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox, customer))
            {
                warnings.Add("MyToyBox is disabled");
                return warnings;
            }
            if (customer.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add(_localizationService.GetResource("SubscriptionCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            _customerService.ResetCheckoutData(customer, storeId);

            var cart = customer.SubscriptionCartItems
                .Where(sci => sci.SubscriptionCartType == borrowCartType)
                .LimitPerStore(storeId)
                .ToList();

            var borrowCartItem = FindSubscriptionCartItemInTheCart(cart,
                borrowCartType, plan, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (borrowCartItem != null)
            {
                //update existing shopping cart item
             //   int newQuantity = borrowCartItem.Quantity + quantity;
                int newQuantity = borrowCartItem.Quantity ;
                warnings.AddRange(GetSubscriptionCartItemWarnings(customer, borrowCartType, plan,
                    storeId, attributesXml, 
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, automaticallyAddRequiredPlansIfEnabled));

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
                warnings.AddRange(GetSubscriptionCartItemWarnings(customer, borrowCartType, plan,
                    storeId, attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate, 
                    quantity, automaticallyAddRequiredPlansIfEnabled));
                if (warnings.Count == 0)
                {
                    //maximum items validation
                    switch (borrowCartType)
                    {
                        case SubscriptionCartType.SubscriptionCart:
                            {
                                 
                            }
                            break;
                        case SubscriptionCartType.MyToyBox:
                            {
                                if (cart.Count >= _borrowCartSettings.MaximumMyToyBoxItems)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("SubscriptionCart.MaximumMyToyBoxItems"), _borrowCartSettings.MaximumMyToyBoxItems));
                                    return warnings;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    Decimal SecurityDeposit = 0;
                    Decimal SecurityDepositPaid = 0;
                    Decimal SecurityDepositBalance = 0;
                    Decimal RegistrationCharge = 0;
                    Decimal RegistrationChargePaid = 0;
                    Decimal RegistrationChargeBalance = 0;
                    Decimal PreviousRentalBalance = 0;
                    String  PreviousRentalBalanceDesc = "";

                    var currentorder = _subscriptionService.GetCurrentSubscribedOrder(customer.Id);
                    if (currentorder != null)
                    {
                        if (currentorder.SubscriptionOrderStatus == SubscriptionOrderStatus.Complete) {

                            if (currentorder.PaymentStatus == PaymentStatus.Paid) {

                                SubscriptionOrderItem soi = currentorder.SubscriptionOrderItems.FirstOrDefault();
                                if (soi != null)
                                {
                                    Decimal currentplanprice = soi.UnitPriceExclTax;
                                    DateTime rentalenddate  = soi.RentalEndDateUtc??  DateTime.Now;

                                    DateTime from = rentalenddate.Date;
                                    DateTime to = DateTime.Now.Date;
                                    //var dateSpan = DateTimeSpan.CompareDates(rentaldateonly, currentdate);
                                    //long mo = (long)(rentaldateonly - currentdate).TotalMilliseconds;


                                    int remainingmonths = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

                                    if (from.AddMonths(remainingmonths) > to || to.Day < from.Day)
                                    {
                                       remainingmonths=  remainingmonths - 1;
                                    }
                                     
                                  //  int remainingmonths =  (rentalenddate.Year * 12 + rentalenddate.Month) - (DateTime.Now.Year * 12 + DateTime.Now.Month);
                                    //int remainingdays = (int)Math.Ceiling((rentalenddate - DateTime.Now).TotalDays);
                                    if (remainingmonths > 0)
                                    {
                                        PreviousRentalBalance = Math.Round(soi.UnitPriceExclTax * remainingmonths, 2);
                                        PreviousRentalBalanceDesc = remainingmonths + " months pending";
                                    }
                                }
                            }

                            SecurityDeposit = plan.SecurityDeposit;
                            SecurityDepositPaid = customer.SecurityDepositBalance;
                            RegistrationCharge = plan.RegistrationCharge;
                            SecurityDepositBalance = plan.SecurityDeposit - customer.SecurityDepositBalance;
                            if (SecurityDepositBalance < 0)
                                SecurityDepositBalance = 0;

                            RegistrationChargeBalance = plan.RegistrationCharge - customer.RegistrationChargeBalance;
                        }
                        else
                        {
                            SecurityDeposit = plan.SecurityDeposit;
                            SecurityDepositPaid = 0;
                            SecurityDepositBalance = plan.SecurityDeposit;

                            RegistrationCharge = plan.RegistrationCharge;
                            RegistrationChargePaid = 0;
                            RegistrationChargeBalance = plan.RegistrationCharge;
                        }
                    }
                    else
                    {
                        SecurityDeposit = plan.SecurityDeposit;
                        SecurityDepositPaid = customer.SecurityDepositBalance;
                        SecurityDepositBalance = plan.SecurityDeposit - customer.SecurityDepositBalance;
                        
                        RegistrationCharge = plan.RegistrationCharge;
                        RegistrationChargePaid = customer.RegistrationChargeBalance;
                        RegistrationChargeBalance = plan.RegistrationCharge - customer.RegistrationChargeBalance;
                    }

                   
                    int daysvalue = 0;
                    if (plan.RentalPricePeriodId == 10)
                    {
                        daysvalue = plan.RentalPriceLength * 7;
                    }

                    if (plan.RentalPricePeriodId == 20)
                    {
                      //  DateTime.Now.Month
                        daysvalue = (int)Math.Ceiling((DateTime.Now.AddMonths(plan.RentalPriceLength) - DateTime.Now).TotalDays);
                    }

                    if (plan.RentalPricePeriodId == 30)
                    {
                        daysvalue = (int)Math.Ceiling((DateTime.Now.AddYears(plan.RentalPriceLength) - DateTime.Now).TotalDays);
                    }

                    rentalStartDate = DateTime.Now.AddDays(1);
                    rentalEndDate = DateTime.Now.AddDays(daysvalue+1);

                    DateTime now = DateTime.UtcNow;
                    borrowCartItem = new SubscriptionCartItem
                    {
                        SubscriptionCartType = borrowCartType,
                        StoreId = storeId,
                        Plan = plan,
                        AttributesXml = attributesXml,
                        CustomerEnteredPrice = customerEnteredPrice,
                        Quantity = quantity,
                        RentalStartDateUtc = rentalStartDate,
                        RentalEndDateUtc = rentalEndDate,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now,
                    };

                    
                    borrowCartItem.SecurityDeposit = SecurityDeposit;
                    borrowCartItem.RegistrationCharge = RegistrationCharge;
                    borrowCartItem.PreviousRentalBalance = PreviousRentalBalance;
                    borrowCartItem.PreviousRentalBalanceDesc = PreviousRentalBalanceDesc;

                    customer.SubscriptionCartItems.Add(borrowCartItem);
                    _customerService.UpdateCustomer(customer);


                    //updated "HasSubscriptionCartItems" property used for performance optimization
                    customer.HasSubscriptionCartItems = customer.SubscriptionCartItems.Count > 0;
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
        public virtual IList<string> UpdateSubscriptionOrderCartItem(Customer customer,
            int borrowCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, 
            int quantity = 1, bool resetCheckoutData = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var warnings = new List<string>();

            var borrowCartItem = customer.SubscriptionCartItems.FirstOrDefault(sci => sci.Id == borrowCartItemId);
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
                    warnings.AddRange(GetSubscriptionCartItemWarnings(customer, borrowCartItem.SubscriptionCartType,
                        borrowCartItem.Plan, borrowCartItem.StoreId,
                        attributesXml, customerEnteredPrice, 
                        rentalStartDate, rentalEndDate, quantity, false));
                    if (warnings.Count == 0)
                    {
                        //if everything is OK, then update a shopping cart item
                        borrowCartItem.Quantity = quantity;
                        borrowCartItem.AttributesXml = attributesXml;
                        borrowCartItem.CustomerEnteredPrice = customerEnteredPrice;
                        borrowCartItem.RentalStartDateUtc = rentalStartDate;
                        borrowCartItem.RentalEndDateUtc = rentalEndDate;
                        borrowCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _customerService.UpdateCustomer(customer);

                        //event notification
                        _eventPublisher.EntityUpdated(borrowCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    DeleteSubscriptionCartItem(borrowCartItem, resetCheckoutData, true);
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
        public virtual void MigrateSubscriptionCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            if (fromCustomer == null)
                throw new ArgumentNullException("fromCustomer");
            if (toCustomer == null)
                throw new ArgumentNullException("toCustomer");

            if (fromCustomer.Id == toCustomer.Id)
                return; //the same customer

            //shopping cart items
            var fromCart = fromCustomer.SubscriptionCartItems.ToList();
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                AddToCart(toCustomer, sci.Plan, sci.SubscriptionCartType, sci.StoreId, 
                    sci.AttributesXml, sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
            }
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                DeleteSubscriptionCartItem(sci);
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
