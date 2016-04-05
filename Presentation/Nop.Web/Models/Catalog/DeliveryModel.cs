using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using System;

namespace Nop.Web.Models.Catalog
{
    public partial class DeliveryModel : BaseNopEntityModel
    {
        public DeliveryModel()
        {
            DefaultPictureModel = new PictureModel();
             
        }
        public override int Id { get; set; }
        public int SubscriptionOrderId { get; set; }
        public string TotalWeight { get; set; }
        public bool IsPendingDelivery { get; set; }
        public string AdminComment { get; set; }
        public bool IsShipmentAvailable { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public string Sku { get; set; }
        public string AttributeInfo { get; set; }
        public string RentalInfo { get; set; }
        public string TransactionDate { get; set; }
        //weight of one item (product)
        public string ItemWeight { get; set; }
        public string ItemDimensions { get; set; }

        public string ReturnStatus { get; set; }
        //used before a shipment is created
        public PictureModel DefaultPictureModel { get; set; }
      
        
       
    }
}