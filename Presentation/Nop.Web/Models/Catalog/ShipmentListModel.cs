using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class ShipmentListModel : BaseNopModel
    {
        public ShipmentListModel()
        {
            Shipments = new List<ShipmentModel>();
        }

        public int SubscriptionOrderId { get; set; }
        public IList<ShipmentModel> Shipments { get; set; }
         
    }
}