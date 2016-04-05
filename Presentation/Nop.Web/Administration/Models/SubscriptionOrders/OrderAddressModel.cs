using Nop.Admin.Models.Common;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class SubscriptionOrderAddressModel : BaseNopModel
    {
        public int SubscriptionOrderId { get; set; }

        public AddressModel Address { get; set; }
    }
}