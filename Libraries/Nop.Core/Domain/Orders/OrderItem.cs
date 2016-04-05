using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;

namespace Nop.Core.Domain.SubscriptionOrders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class OrderItem : BaseEntity
    {
        private ICollection<Shipment> _shipments;
        private ICollection<ItemDetail> _itemDetails;
        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public Guid OrderItemGuid { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int SubscriptionOrderId { get; set; }

        /// <summary>
        /// Gets or sets the shipping status identifier
        /// </summary>
        public int ShippingStatusId { get; set; }
 
         
        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual SubscriptionOrder SubscriptionOrder { get; set; }

        /// <summary>
        /// Gets or sets order items
        /// </summary>
        public virtual ICollection<ItemDetail> ItemDetails
        {
            get { return _itemDetails ?? (_itemDetails = new List<ItemDetail>()); }
            protected set { _itemDetails = value; }
        }
 

        /// <summary>
        /// Gets or sets shipments
        /// </summary>
        public virtual ICollection<Shipment> Shipments
        {
            get { return _shipments ?? (_shipments = new List<Shipment>()); }
            protected set { _shipments = value; }
        }


        /// <summary>
        /// Gets or sets the shipping status
        /// </summary>
        public ShippingStatus ShippingStatus
        {
            get
            {
                return (ShippingStatus)this.ShippingStatusId;
            }
            set
            {
                this.ShippingStatusId = (int)value;
            }
        }


    }
}
