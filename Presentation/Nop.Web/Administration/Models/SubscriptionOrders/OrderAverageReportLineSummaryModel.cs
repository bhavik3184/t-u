using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SubscriptionOrders
{
    public partial class SubscriptionOrderAverageReportLineSummaryModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.SalesReport.Average.SubscriptionOrderStatus")]
        public string SubscriptionOrderStatus { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Average.SumTodaySubscriptionOrders")]
        public string SumTodaySubscriptionOrders { get; set; }
        
        [NopResourceDisplayName("Admin.SalesReport.Average.SumThisWeekSubscriptionOrders")]
        public string SumThisWeekSubscriptionOrders { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Average.SumThisMonthSubscriptionOrders")]
        public string SumThisMonthSubscriptionOrders { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Average.SumThisYearSubscriptionOrders")]
        public string SumThisYearSubscriptionOrders { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Average.SumAllTimeSubscriptionOrders")]
        public string SumAllTimeSubscriptionOrders { get; set; }
    }
}
