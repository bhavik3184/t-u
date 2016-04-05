using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class PlanCategoryModel : BaseNopEntityModel
    {
        public PlanCategoryModel()
        {
            
        }

        public string CategoryName { get; set; }
        public string PlanName { get; set; }
        public int PlanId { get; set; }
        public int CategoryId { get; set; }
        public int MyToyBoxQuantity { get; set; }
        public int UsedMyToyBoxQuantity { get; set; }
        public int UsedQuantity { get; set; }
        public int Quantity { get; set; }
        
    }
}