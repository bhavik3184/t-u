using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class ShipmentListModel : BaseNopModel
    {
        public ShipmentListModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.TrackingNumber")]
        [AllowHtml]
        public string TrackingNumber { get; set; }
        
        public IList<SelectListItem> AvailableCountries { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.StateProvince")]
        public int StateProvinceId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.City")]
        [AllowHtml]
        public string City { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.LoadNotShipped")]
        public bool LoadNotShipped { get; set; }


        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
    }
}