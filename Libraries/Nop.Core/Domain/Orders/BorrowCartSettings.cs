
using Nop.Core.Configuration;

namespace Nop.Core.Domain.SubscriptionOrders
{
    public class BorrowCartSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether a custoemr should be redirected to the shopping cart page after adding a product to the cart/mytoybox
        /// </summary>
        public bool DisplayCartAfterAddingProduct { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a custoemr should be redirected to the shopping cart page after adding a product to the cart/mytoybox
        /// </summary>
        public bool DisplayMyToyBoxAfterAddingProduct { get; set; }

        /// <summary>
        /// Gets or sets a value indicating maximum number of items in the shopping cart
        /// </summary>
        public int MaximumBorrowCartItems { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating maximum number of items in the mytoybox
        /// </summary>
        public int MaximumMyToyBoxItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product images in the mini-shopping cart block
        /// </summary>
        public bool AllowOutOfStockItemsToBeAddedToMyToyBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to move items from mytoybox to cart when clicking "Add to cart" button. Otherwise, they are copied.
        /// </summary>
        public bool MoveItemsFromMyToyBoxToCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product image on shopping cart page
        /// </summary>
        public bool ShowProductImagesOnBorrowCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product image on mytoybox page
        /// </summary>
        public bool ShowProductImagesOnWishList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show discount box on shopping cart page
        /// </summary>
        public bool ShowDiscountBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show gift card box on shopping cart page
        /// </summary>
        public bool ShowGiftCardBox { get; set; }

        /// <summary>
        /// Gets or sets a number of "Cross-sells" on shopping cart page
        /// </summary>
        public int CrossSellsNumber { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether "email a mytoybox" feature is enabled
        /// </summary>
        public bool EmailMyToyBoxEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enabled "email a mytoybox" for anonymous users.
        /// </summary>
        public bool AllowAnonymousUsersToEmailMyToyBox { get; set; }
        
        /// <summary>Gets or sets a value indicating whether mini-shopping cart is enabled
        /// </summary>
        public bool MiniBorrowCartEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product images in the mini-shopping cart block
        /// </summary>
        public bool ShowProductImagesInMiniBorrowCart { get; set; }

        /// <summary>Gets or sets a maximum number of products which can be displayed in the mini-shopping cart block
        /// </summary>
        public int MiniBorrowCartProductNumber { get; set; }
        
        //Round is already an issue. 
        //When enabled it can cause one issue: http://www.nopcommerce.com/boards/t/7679/vattax-rounding-error-important-fix.aspx
        //When disable it causes another one: http://www.nopcommerce.com/boards/t/11419/nop-20-order-of-steps-in-checkout.aspx?p=3#46924
        /// <summary>
        /// Gets or sets a value indicating whether to round calculated prices and total during calculation
        /// </summary>
        public bool RoundPricesDuringCalculation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should group shopping cart items for the same products
        /// For example, a customer could have two shopping cart items for the same products (different product attributes)
        /// </summary>
        public bool GroupTierPricesForDistinctBorrowCartItems  { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer will beable to edit products in the cart
        /// </summary>
        public bool AllowCartItemEditing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer will see quantity of attribute values associated to products (when qty > 1)
        /// </summary>
        public bool RenderAssociatedAttributeValueQuantity { get; set; }
    }
}