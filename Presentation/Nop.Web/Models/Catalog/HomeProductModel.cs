using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using System.Web.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class HomeProductModel : BaseNopEntityModel
    {
        public HomeProductModel()
        {
            FeaturedProducts = new List<ProductOverviewModel>();
            NewProducts = new List<ProductOverviewModel>();
        }

        public IList<ProductOverviewModel> FeaturedProducts { get; set; }
        public IList<ProductOverviewModel> NewProducts { get; set; }
 
    }
}