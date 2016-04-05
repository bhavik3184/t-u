using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class NeverSoldReportLineModel : BaseNopModel
    {
        public int PlanId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.NeverSold.Fields.Name")]
        public string PlanName { get; set; }
    }
}