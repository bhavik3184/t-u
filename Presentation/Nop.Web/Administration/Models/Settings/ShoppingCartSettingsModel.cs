using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class BorrowCartSettingsModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }



        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.DisplayCartAfterAddingProduct")]
        public bool DisplayCartAfterAddingProduct { get; set; }
        public bool DisplayCartAfterAddingProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.DisplayMyToyBoxAfterAddingProduct")]
        public bool DisplayMyToyBoxAfterAddingProduct { get; set; }
        public bool DisplayMyToyBoxAfterAddingProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.MaximumBorrowCartItems")]
        public int MaximumBorrowCartItems { get; set; }
        public bool MaximumBorrowCartItems_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.MaximumMyToyBoxItems")]
        public int MaximumMyToyBoxItems { get; set; }
        public bool MaximumMyToyBoxItems_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.AllowOutOfStockItemsToBeAddedToMyToyBox")]
        public bool AllowOutOfStockItemsToBeAddedToMyToyBox { get; set; }
        public bool AllowOutOfStockItemsToBeAddedToMyToyBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.MoveItemsFromMyToyBoxToCart")]
        public bool MoveItemsFromMyToyBoxToCart { get; set; }
        public bool MoveItemsFromMyToyBoxToCart_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.ShowProductImagesOnBorrowCart")]
        public bool ShowProductImagesOnBorrowCart { get; set; }
        public bool ShowProductImagesOnBorrowCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.ShowProductImagesOnWishList")]
        public bool ShowProductImagesOnWishList { get; set; }
        public bool ShowProductImagesOnWishList_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.ShowDiscountBox")]
        public bool ShowDiscountBox { get; set; }
        public bool ShowDiscountBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.ShowGiftCardBox")]
        public bool ShowGiftCardBox { get; set; }
        public bool ShowGiftCardBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.CrossSellsNumber")]
        public int CrossSellsNumber { get; set; }
        public bool CrossSellsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.EmailMyToyBoxEnabled")]
        public bool EmailMyToyBoxEnabled { get; set; }
        public bool EmailMyToyBoxEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.AllowAnonymousUsersToEmailMyToyBox")]
        public bool AllowAnonymousUsersToEmailMyToyBox { get; set; }
        public bool AllowAnonymousUsersToEmailMyToyBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.MiniBorrowCartEnabled")]
        public bool MiniBorrowCartEnabled { get; set; }
        public bool MiniBorrowCartEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.ShowProductImagesInMiniBorrowCart")]
        public bool ShowProductImagesInMiniBorrowCart { get; set; }
        public bool ShowProductImagesInMiniBorrowCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.MiniBorrowCartProductNumber")]
        public int MiniBorrowCartProductNumber { get; set; }
        public bool MiniBorrowCartProductNumber_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.BorrowCart.AllowCartItemEditing")]
        public bool AllowCartItemEditing { get; set; }
        public bool AllowCartItemEditing_OverrideForStore { get; set; }
    }
}