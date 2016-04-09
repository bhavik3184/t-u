using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class ReturnTransactionListModel : BaseNopModel
    {
        public ReturnTransactionListModel()
        {
        }
        public int SubscriptionOrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.List.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [NopResourceDisplayName("Admin.SubscriptionOrders.List.ShippingStatus")]
        public int ShippingStatusId { get; set; }


        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.SubscriptionOrders.Shipments.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

    }
}