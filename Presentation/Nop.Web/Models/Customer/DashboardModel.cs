using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    public partial class DashboardModel : BaseNopModel
    {
        public string PlanName { get; set; }

        public string SecurityDeposit { get; set; }

        public string RegistrationFees { get; set; }

        public string PlanSubscriptionDate{ get; set; }

        public string CreationDate { get; set; }

        public string EndDate { get; set; }

        public string SubscriptionFees { get; set; }

        public string MembershipPlanValidity { get; set; }

        public string RemainingDays { get; set; }

        public string PaymentStatus { get; set; }

    }
}