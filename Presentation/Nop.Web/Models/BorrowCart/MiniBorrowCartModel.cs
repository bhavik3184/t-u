using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.BorrowCart
{
    public partial class MiniBorrowCartModel : BaseNopModel
    {
        public MiniBorrowCartModel()
        {
            Items = new List<BorrowCartItemModel>();
        }

        public IList<BorrowCartItemModel> Items { get; set; }
        public int TotalProducts { get; set; }
        public string SubTotal { get; set; }
        public bool DisplayBorrowCartButton { get; set; }
        public bool DisplayCheckoutButton { get; set; }
        public bool CurrentCustomerIsGuest { get; set; }
        public bool AnonymousCheckoutAllowed { get; set; }
        public bool ShowProductImages { get; set; }


        #region Nested Classes

        public partial class BorrowCartItemModel : BaseNopEntityModel
        {
            public BorrowCartItemModel()
            {
                Picture = new PictureModel();
            }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public int Quantity { get; set; }

            public string UnitPrice { get; set; }

            public string AttributeInfo { get; set; }

            public PictureModel Picture { get; set; }
        }

        #endregion
    }
}