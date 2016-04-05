using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class SubscriptionOrderListModel : BaseNopModel
    {
        public SubscriptionOrderListModel()
        {
            AvailableSubscriptionOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailablePaymentMethods = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.List.SubscriptionOrderStatus")]
        public int SubscriptionOrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.List.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.List.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.PaymentMethod")]
        public string PaymentMethodSystemName { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.Store")]
        public int StoreId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.Warehouse")]
        public int WarehouseId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.Product")]
        public int PlanId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.BillingEmail")]
        [AllowHtml]
        public string BillingEmail { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.BillingLastName")]
        [AllowHtml]
        public string BillingLastName { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.BillingCountry")]
        public int BillingCountryId { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.SubscriptionOrderNotes")]
        [AllowHtml]
        public string SubscriptionOrderNotes { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.SubscriptionOrderGuid")]
        [AllowHtml]
        public string SubscriptionOrderGuid { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.List.GoDirectlyToNumber")]
        [AllowHtml]
        public int GoDirectlyToNumber { get; set; }

        public bool IsLoggedInAsVendor { get; set; }


        public IList<SelectListItem> AvailableSubscriptionOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<SelectListItem> AvailablePaymentMethods { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
    }
}