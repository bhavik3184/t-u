using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Order
{
    public partial class SubmitReturnRequestModel : BaseNopModel
    {
        public SubmitReturnRequestModel()
        {
            Items = new List<OrderItemModel>();
            AvailableReturnReasons = new List<ReturnRequestReasonModel>();
            AvailableReturnActions= new List<ReturnRequestActionModel>();
        }

        public int SubscriptionOrderId { get; set; }

        public int TransactionId { get; set; }
        
        public IList<OrderItemModel> Items { get; set; }
        
        [AllowHtml]
        [NopResourceDisplayName("ReturnRequests.ReturnReason")]
        public int ReturnRequestReasonId { get; set; }
        public IList<ReturnRequestReasonModel> AvailableReturnReasons { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ReturnRequests.ReturnAction")]
        public int ReturnRequestActionId { get; set; }
        public IList<ReturnRequestActionModel> AvailableReturnActions { get; set; }
        public DateTime AvailableDate { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ReturnRequests.Comments")]
        public string Comments { get; set; }

        public string Result { get; set; }
        
        #region Nested classes

        public partial class OrderItemModel : BaseNopEntityModel
        {
            public OrderItemModel()
            {
                ItemDetails = new List<ItemDetailModel>();
            }
            public int OrderItemId { get; set; }

            public IList<ItemDetailModel> ItemDetails { get; set; }

            public partial class ItemDetailModel : BaseNopEntityModel
            {
                public ItemDetailModel()
                {
                    DefaultPictureModel = new PictureModel();
                }
                public int ProductId { get; set; }

                public string ProductName { get; set; }

                public string ProductSeName { get; set; }

                public string AttributeInfo { get; set; }

                public string UnitPrice { get; set; }

                public int Quantity { get; set; }

                public bool IsPendingReturn { get; set; }
                public PictureModel DefaultPictureModel { get; set; }
            }
        }

        public partial class ReturnRequestReasonModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }
        public partial class ReturnRequestActionModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }

        #endregion
    }

}