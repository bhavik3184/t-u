using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class CountryReportLineModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.SalesReport.Country.Fields.CountryName")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Country.Fields.TotalSubscriptionOrders")]
        public int TotalSubscriptionOrders { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Country.Fields.SumSubscriptionOrders")]
        public string SumSubscriptionOrders { get; set; }
    }
}