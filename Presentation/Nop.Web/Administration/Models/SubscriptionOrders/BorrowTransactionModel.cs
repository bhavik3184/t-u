using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class BorrowTransactionModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public int OrderItemId { get; set; }
        public int SubscriptionOrderId { get; set; }
        public string SubscriptionOrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }

        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.CurrentCarts.TotalItems")]
        public int TotalItems { get; set; }
    }
}
