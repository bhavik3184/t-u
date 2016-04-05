using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial interface ISubscriptionCartService
    {
        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="borrowCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        void DeleteSubscriptionCartItem(SubscriptionCartItem borrowCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false);

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        int DeleteExpiredSubscriptionCartItems(DateTime olderThanUtc);

        /// <summary>
        /// Validates required plans (plans which require some other plans to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="automaticallyAddRequiredPlansIfEnabled">Automatically add required plans if enabled</param>
        /// <returns>Warnings</returns>
        IList<string> GetRequiredPlanWarnings(Customer customer,
            SubscriptionCartType borrowCartType, Plan plan,
            int storeId, bool automaticallyAddRequiredPlansIfEnabled);

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
        IList<string> GetStandardWarnings(Customer customer, SubscriptionCartType borrowCartType,
            Plan plan, string attributesXml,
            decimal customerEnteredPrice, int quantity);

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
        IList<string> GetSubscriptionCartItemAttributeWarnings(Customer customer, 
            SubscriptionCartType borrowCartType,
            Plan plan,
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false);
        
        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="borrowCartType">Shopping cart type</param>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        IList<string> GetSubscriptionCartItemGiftCardWarnings(SubscriptionCartType borrowCartType,
            Plan plan, string attributesXml);

        /// <summary>
        /// Validates shopping cart item for rental plans
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Warnings</returns>
        IList<string> GetRentalPlanWarnings(Plan plan,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null);

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
        IList<string> GetSubscriptionCartItemWarnings(Customer customer, SubscriptionCartType borrowCartType,
            Plan plan, int storeId,
            string attributesXml, decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredPlansIfEnabled = true,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredPlanWarnings = true,
            bool getRentalWarnings = true);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetSubscriptionCartWarnings(IList<SubscriptionCartItem> borrowCart,
            string checkoutAttributesXml, bool validateCheckoutAttributes);

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
        SubscriptionCartItem FindSubscriptionCartItemInTheCart(IList<SubscriptionCartItem> borrowCart,
            SubscriptionCartType borrowCartType,
            Plan plan,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null);


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
        IList<string> AddToCart(Customer customer, Plan plan,
            SubscriptionCartType borrowCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero, 
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredPlansIfEnabled = true);
        
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
        IList<string> UpdateSubscriptionOrderCartItem(Customer customer,
            int borrowCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool resetCheckoutData = true);
        
        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        void MigrateSubscriptionCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes);
    }
}
