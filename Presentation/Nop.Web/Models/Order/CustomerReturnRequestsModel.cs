using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Order
{
    public partial class CustomerReturnRequestsModel : BaseNopModel
    {
         public CustomerReturnRequestsModel()
        {
            TransactionItems = new List<TransactionItemModel>();
        }

           public IList<TransactionItemModel> TransactionItems { get; set; }


        public partial class TransactionItemModel : BaseNopEntityModel
        {
            public TransactionItemModel()
            {
                Items = new List<ReturnRequestModel>();
            }

            public IList<ReturnRequestModel> Items { get; set; }
            
            public partial class ReturnRequestModel : BaseNopEntityModel
            {
                public ReturnRequestModel()
                {
                    DefaultPictureModel = new PictureModel();
                }
                public string ReturnRequestStatus { get; set; }
                public int ProductId { get; set; }
                public int OrderItemId { get; set; }
                public int ItemDetailId { get; set; }
                public string ProductName { get; set; }
                public string ProductSeName { get; set; }
                public int Quantity { get; set; }

                public string ReturnReason { get; set; }
                public string ReturnAction { get; set; }
                public string Comments { get; set; }

                public DateTime CreatedOn { get; set; }

                public PictureModel DefaultPictureModel { get; set; }
            }
        }
      
    }
}