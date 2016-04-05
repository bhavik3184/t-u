using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.SubscriptionOrder
{
    public partial class SubmitReturnRequestModel : BaseNopModel
    {
        public SubmitReturnRequestModel()
        {
            Items = new List<SubscriptionOrderItemModel>();
            AvailableReturnReasons = new List<ReturnRequestReasonModel>();
            AvailableReturnActions= new List<ReturnRequestActionModel>();
        }

        public int SubscriptionOrderId { get; set; }
        
        public IList<SubscriptionOrderItemModel> Items { get; set; }
        
        [AllowHtml]
        [NopResourceDisplayName("ReturnRequests.ReturnReason")]
        public int ReturnRequestReasonId { get; set; }
        public IList<ReturnRequestReasonModel> AvailableReturnReasons { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ReturnRequests.ReturnAction")]
        public int ReturnRequestActionId { get; set; }
        public IList<ReturnRequestActionModel> AvailableReturnActions { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ReturnRequests.Comments")]
        public string Comments { get; set; }

        public string Result { get; set; }
        
        #region Nested classes

        public partial class SubscriptionOrderItemModel : BaseNopEntityModel
        {
            public int PlanId { get; set; }

            public string PlanName { get; set; }

            public string PlanSeName { get; set; }

            public string AttributeInfo { get; set; }

            public string UnitPrice { get; set; }

            public int Quantity { get; set; }
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