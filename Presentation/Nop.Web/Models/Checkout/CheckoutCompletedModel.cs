using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutCompletedModel : BaseNopModel
    {
        public int SubscriptionOrderId { get; set; }
        public bool OnePageCheckoutEnabled { get; set; }
    }
}