using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Models.BorrowCart
{
    public partial class MyToyBoxModel : BaseNopModel
    {
        public MyToyBoxModel()
        {
            Items = new List<BorrowCartItemModel>();
            Warnings = new List<string>();
            PlanCategoryModels = new List<PlanCategoryModel>();
        }

        public Guid CustomerGuid { get; set; }
        public string CustomerFullname { get; set; }
      
        public bool EmailMyToyBoxEnabled { get; set; }

        public bool ShowSku { get; set; }

        public bool ShowProductImages { get; set; }
        public string AllowedTransaction { get; set; }

        public string UsedTransaction { get; set; }
        public bool IsEditable { get; set; }

        public bool DisplayAddToCart { get; set; }

        public bool DisplayTaxShippingInfo { get; set; }

        public IList<BorrowCartItemModel> Items { get; set; }
        public IList<PlanCategoryModel> PlanCategoryModels { get; set; }
        public IList<string> Warnings { get; set; }
        
		#region Nested Classes

        public partial class BorrowCartItemModel : BaseNopEntityModel
        {
            public BorrowCartItemModel()
            {
                Picture = new PictureModel();
                AllowedQuantities = new List<SelectListItem>();
                Warnings = new List<string>();
            }
            public string Sku { get; set; }

            public PictureModel Picture {get;set;}

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public string Discount { get; set; }
            public bool IsStockAvailable { get; set; }

            public int Quantity { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }
            
            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public IList<string> Warnings { get; set; }

        }

		#endregion
    }
}