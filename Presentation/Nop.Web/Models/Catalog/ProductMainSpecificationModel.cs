using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductMainSpecificationModel : BaseNopModel
    {
        public ProductMainSpecificationModel()
        {

             PSModel = new List<ProductSpecificationModel>();
        }
       
        public string SpecificationAttributeName { get; set; }

        //this value is already HTML encoded
        public string ValueRaw { get; set; }

        public IList<ProductSpecificationModel> PSModel { get; set; }
    }
}