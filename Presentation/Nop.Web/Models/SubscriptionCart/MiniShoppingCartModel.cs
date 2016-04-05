using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.SubscriptionCart
{
    public partial class MiniSubscriptionCartModel : BaseNopModel
    {
        public MiniSubscriptionCartModel()
        {
            Items = new List<SubscriptionCartItemModel>();
        }

        public IList<SubscriptionCartItemModel> Items { get; set; }
        public int TotalPlans { get; set; }
        public string SubTotal { get; set; }
        public bool DisplaySubscriptionCartButton { get; set; }
        public bool DisplayCheckoutButton { get; set; }
        public bool CurrentCustomerIsGuest { get; set; }
        public bool AnonymousCheckoutAllowed { get; set; }
        public bool ShowPlanImages { get; set; }


        #region Nested Classes

        public partial class SubscriptionCartItemModel : BaseNopEntityModel
        {
            public SubscriptionCartItemModel()
            {
                Picture = new PictureModel();
            }

            public int PlanId { get; set; }

            public string PlanName { get; set; }

            public string PlanSeName { get; set; }

            public int Quantity { get; set; }

            public string UnitPrice { get; set; }

            public string AttributeInfo { get; set; }

            public PictureModel Picture { get; set; }
        }

        #endregion
    }
}