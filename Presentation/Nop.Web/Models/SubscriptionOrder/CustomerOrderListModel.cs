using System;
using System.Collections.Generic;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.SubscriptionOrder
{
    public partial class CustomerSubscriptionOrderListModel : BaseNopModel
    {
        public CustomerSubscriptionOrderListModel()
        {
            SubscriptionOrders = new List<SubscriptionOrderDetailsModel>();
            RecurringSubscriptionOrders = new List<RecurringSubscriptionOrderModel>();
            CancelRecurringPaymentErrors = new List<string>();
        }

        public IList<SubscriptionOrderDetailsModel> SubscriptionOrders { get; set; }
        public IList<RecurringSubscriptionOrderModel> RecurringSubscriptionOrders { get; set; }
        public IList<string> CancelRecurringPaymentErrors { get; set; }


        #region Nested classes

        public partial class SubscriptionOrderDetailsModel : BaseNopEntityModel
        {
            public string SubscriptionOrderTotal { get; set; }
            public bool IsReturnRequestAllowed { get; set; }
            public SubscriptionOrderStatus SubscriptionOrderStatusEnum { get; set; }
            public string SubscriptionOrderStatus { get; set; }
            public string PaymentStatus { get; set; }
            public string ShippingStatus { get; set; }
            public DateTime CreatedOn { get; set; }
            public string PlanName { get; set; }
            public DateTime RentalStartDate { get; set; }
            public DateTime RentalEndDate { get; set; }
        }

        public partial class RecurringSubscriptionOrderModel : BaseNopEntityModel
        {
            public string StartDate { get; set; }
            public string CycleInfo { get; set; }
            public string NextPayment { get; set; }
            public int TotalCycles { get; set; }
            public int CyclesRemaining { get; set; }
            public int InitialSubscriptionOrderId { get; set; }
            public bool CanCancel { get; set; }
        }

        #endregion
    }
}