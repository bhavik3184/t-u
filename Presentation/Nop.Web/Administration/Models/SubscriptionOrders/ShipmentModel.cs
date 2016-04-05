using System;
using System.Collections.Generic;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class ShipmentModel : BaseNopEntityModel
    {
        public ShipmentModel()
        {
            this.ShipmentStatusEvents = new List<ShipmentStatusEventModel>();
            this.Items = new List<ShipmentItemModel>();
        }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.ID")]
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.SubscriptionOrderID")]
        public int SubscriptionOrderId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.TotalWeight")]
        public string TotalWeight { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.TrackingNumber")]
        public string TrackingNumber { get; set; }
        public string TrackingNumberUrl { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.ShippedDate")]
        public string ShippedDate { get; set; }
        public bool CanShip { get; set; }
        public DateTime? ShippedDateUtc { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.DeliveryDate")]
        public string DeliveryDate { get; set; }
        public bool CanDeliver { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.AdminComment")]
        public string AdminComment { get; set; }

        public List<ShipmentItemModel> Items { get; set; }

        public IList<ShipmentStatusEventModel> ShipmentStatusEvents { get; set; }

        #region Nested classes

        public partial class ShipmentItemModel : BaseNopEntityModel
        {
            public ShipmentItemModel()
            {
                AvailableWarehouses = new List<WarehouseInfo>();
            }

            public int ItemDetailId { get; set; }

            public int OrderItemId { get; set; }
            public int ProductId { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.Products.ProductName")]
            public string ProductName { get; set; }
            public string Sku { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public bool ShipSeparately { get; set; }

            //weight of one item (product)
            [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.Products.ItemWeight")]
            public string ItemWeight { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.Products.ItemDimensions")]
            public string ItemDimensions { get; set; }

            public int QuantityToAdd { get; set; }
            public int QuantityOrdered { get; set; }
            [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.Products.QtyShipped")]
            public int QuantityInThisShipment { get; set; }
            public int QuantityInAllShipments { get; set; }

            public string ShippedFromWarehouse { get; set; }
            public bool AllowToChooseWarehouse { get; set; }
            //used before a shipment is created
            public List<WarehouseInfo> AvailableWarehouses { get; set; }

            #region Nested Classes
            public class WarehouseInfo : BaseNopModel
            {
                public int WarehouseId { get; set; }
                public string WarehouseName { get; set; }
                public int StockQuantity { get; set; }
                public int ReservedQuantity { get; set; }
                public int PlannedQuantity { get; set; }
            }
            #endregion
        }

        public partial class ShipmentStatusEventModel : BaseNopModel
        {
            public string EventName { get; set; }
            public string Location { get; set; }
            public string Country { get; set; }
            public DateTime? Date { get; set; }
        }

        #endregion
    }
}