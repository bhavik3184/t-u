using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class SubscriptionCartPlansModel : BaseNopModel
    {
        public SubscriptionCartPlansModel()
        {
            this.MembershipCategories = new List<MembershipCategoryModel>();
        }

        public IList<MembershipCategoryModel> MembershipCategories { get; set; }

    }
}