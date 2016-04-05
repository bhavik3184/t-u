using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class DeliveryBatchListModel : BaseNopModel
    {
        public DeliveryBatchListModel()
        {
            DeliveryList = new List<DeliveryListModel>();
        }

       
        public IList<DeliveryListModel> DeliveryList { get; set; }
         
    }
}