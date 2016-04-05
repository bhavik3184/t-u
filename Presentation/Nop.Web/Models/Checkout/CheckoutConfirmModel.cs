using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutConfirmModel : BaseNopModel
    {
        public CheckoutConfirmModel()
        {
            Warnings = new List<string>();
        }

        public bool TermsOfServiceOnSubscriptionOrderConfirmPage { get; set; }
        public string MinSubscriptionOrderTotalWarning { get; set; }

        public IList<string> Warnings { get; set; }
    }
}