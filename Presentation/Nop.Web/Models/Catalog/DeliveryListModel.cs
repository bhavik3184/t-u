using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Web.Models.Catalog
{
    public partial class DeliveryListModel : BaseNopModel
    {
        public DeliveryListModel()
        {
            Deliveries = new List<DeliveryModel>();
            Shipments = new List<ShipmentModel>();
        }
        public int BatchId { get; set; }
        public int OrderItemId { get; set; }
        public int QuantityToAdd { get; set; }
        public int QuantityOrdered { get; set; }

        public bool IsPendingDeliveryList { get; set; }
        public int SubscriptionOrderId { get; set; }
        public string ItemWeight { get; set; }
        public string ItemDimensions { get; set; }
        public string TotalWeight { get; set; }

        public bool IsPendingReturn { get; set; }
        public IList<ShipmentModel> Shipments { get; set; }
        
        public IList<DeliveryModel> Deliveries { get; set; }
         
    }
}