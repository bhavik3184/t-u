using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class BestsellersReportLineModel : BaseNopModel
    {
        public int PlanId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.Name")]
        public string PlanName { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.TotalAmount")]
        public string TotalAmount { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.TotalQuantity")]
        public decimal TotalQuantity { get; set; }
    }
}