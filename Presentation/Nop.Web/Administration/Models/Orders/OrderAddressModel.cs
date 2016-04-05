using Nop.Admin.Models.Common;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderAddressModel : BaseNopModel
    {
        public int SubscriptionOrderId { get; set; }

        public AddressModel Address { get; set; }
    }
}