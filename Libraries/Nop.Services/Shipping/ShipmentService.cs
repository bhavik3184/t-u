using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentService : IShipmentService
    {
        #region Fields

        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentItem> _siRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<ItemDetail> _itemDetailRepository;
        private readonly IEventPublisher _eventPublisher;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipmentRepository">Shipment repository</param>
        /// <param name="siRepository">Shipment item repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ShipmentService(IRepository<Shipment> shipmentRepository,
            IRepository<ShipmentItem> siRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<ItemDetail> itemDetailRepository,
            IEventPublisher eventPublisher)
        {
            this._shipmentRepository = shipmentRepository;
            this._siRepository = siRepository;
            this._orderItemRepository = orderItemRepository;
            this._itemDetailRepository = itemDetailRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void DeleteShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Delete(shipment);

            //event notification
            _eventPublisher.EntityDeleted(shipment);
        }
        
        /// <summary>
        /// Search shipments
        /// </summary>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier, only shipments with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
        /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
        /// <param name="shippingCity">Shipping city; null to load all records</param>
        /// <param name="trackingNumber">Search by tracking number</param>
        /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Shipments</returns>
        public virtual IPagedList<Shipment> GetAllShipments(int vendorId = 0, int customerId = 0, int warehouseId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _shipmentRepository.Table;
            if (!String.IsNullOrEmpty(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));
            if (shippingCountryId > 0)
                query = query.Where(s => s.OrderItem.SubscriptionOrder.ShippingAddress.CountryId == shippingCountryId);
            if (shippingStateId > 0)
                query = query.Where(s => s.OrderItem.SubscriptionOrder.ShippingAddress.StateProvinceId == shippingStateId);
            if (!String.IsNullOrWhiteSpace(shippingCity))
                query = query.Where(s => s.OrderItem.SubscriptionOrder.ShippingAddress.City.Contains(shippingCity));
            if (loadNotShipped)
                query = query.Where(s => !s.ShippedDateUtc.HasValue);
            if (createdFromUtc.HasValue)
                query = query.Where(s => createdFromUtc.Value <= s.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(s => createdToUtc.Value >= s.CreatedOnUtc);
            query = query.Where(s => s.OrderItem.SubscriptionOrder != null && !s.OrderItem.SubscriptionOrder.Deleted);
            if (vendorId > 0)
            {
                var queryVendorOrderItems = from itemDetail in _itemDetailRepository.Table
                                            join orderItem in _orderItemRepository.Table on itemDetail.OrderItemId equals orderItem.Id
                                            where itemDetail.Product.VendorId == vendorId
                                            select itemDetail.Id;

                query = from s in query
                        where queryVendorOrderItems.Intersect(s.ShipmentItems.Select(si => si.ItemDetailId)).Any()
                        select s;
            }

            if (customerId > 0)
            {
                var querycustomerOrderItems = from orderItem in _orderItemRepository.Table
                                              where orderItem.SubscriptionOrder.CustomerId == customerId
                                            select orderItem.Id;

                query = from s in query
                        where querycustomerOrderItems.Intersect(s.ShipmentItems.Select(si => si.ItemDetailId)).Any()
                        select s;
            }

            if (warehouseId > 0)
            {
                query = from s in query
                        where s.ShipmentItems.Any(si => si.WarehouseId == warehouseId)
                        select s;
            }
            query = query.OrderByDescending(s => s.CreatedOnUtc);

            var shipments = new PagedList<Shipment>(query, pageIndex, pageSize);
            return shipments;
        }


        public virtual IPagedList<ShipmentItem> GetAllShipmentItems(int OrderId, int ItemDetailId, int customerId = 0, 
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _siRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(s => createdFromUtc.Value <= s.Shipment.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(s => createdToUtc.Value >= s.Shipment.CreatedOnUtc);



            query = query.Where(s => s.Shipment.OrderItem.SubscriptionOrder != null && !s.Shipment.OrderItem.SubscriptionOrder.Deleted);

            if (customerId > 0)
            {
                var querycustomerOrderItems = from orderItem in _orderItemRepository.Table
                                              where orderItem.SubscriptionOrder.CustomerId == customerId
                                              select orderItem.Id;
                query = from s in query
                        where querycustomerOrderItems.Intersect(s.Shipment.ShipmentItems.Select(si => si.ItemDetailId)).Any()
                        select s;
            }

            if (OrderId > 0 && ItemDetailId > 0)
            {
                query = query.Where(s => OrderId == s.Shipment.OrderItem.SubscriptionOrderId);
                query = query.Where(s => ItemDetailId == s.ItemDetailId);
            }
            query = query.OrderByDescending(s => s.Shipment.CreatedOnUtc);

            var shipments = new PagedList<ShipmentItem>(query, pageIndex, pageSize);
            return shipments;
        }


        public virtual IPagedList<ShipmentItem> GetAllShipmentItemsByStatus(int OrderId, int ItemDetailId, int customerId = 0,
        Boolean deliveryStatus = false,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _siRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(s => createdFromUtc.Value <= s.Shipment.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(s => createdToUtc.Value >= s.Shipment.CreatedOnUtc);



            query = query.Where(s => s.Shipment.OrderItem.SubscriptionOrder != null && !s.Shipment.OrderItem.SubscriptionOrder.Deleted);

            if (customerId > 0)
            {
                var querycustomerOrderItems = from orderItem in _orderItemRepository.Table
                                              where orderItem.SubscriptionOrder.CustomerId == customerId
                                              select orderItem.Id;
                query = from s in query
                        where querycustomerOrderItems.Intersect(s.Shipment.ShipmentItems.Select(si => si.ItemDetailId)).Any()
                        select s;
            }

            if(deliveryStatus ==true)
                query = query.Where(s => s.Shipment.DeliveryDateUtc != null);
            else
                query = query.Where(s => s.Shipment.DeliveryDateUtc == null);

            if (OrderId > 0 && ItemDetailId > 0)
            {
                query = query.Where(s => OrderId == s.Shipment.OrderItem.SubscriptionOrderId);

                query = query.Where(s => ItemDetailId == s.ItemDetailId);
            }
            query = query.OrderByDescending(s => s.Shipment.CreatedOnUtc);

            var shipments = new PagedList<ShipmentItem>(query, pageIndex, pageSize);
            return shipments;
        }


        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentIds">Shipment identifiers</param>
        /// <returns>Shipments</returns>
        public virtual IList<Shipment> GetShipmentsByIds(int[] shipmentIds)
        {
            if (shipmentIds == null || shipmentIds.Length == 0)
                return new List<Shipment>();

            var query = from o in _shipmentRepository.Table
                        where shipmentIds.Contains(o.Id)
                        select o;
            var shipments = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Shipment>();
            foreach (int id in shipmentIds)
            {
                var shipment = shipments.Find(x => x.Id == id);
                if (shipment != null)
                    sortedOrders.Add(shipment);
            }
            return sortedOrders;
        }

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>Shipment</returns>
        public virtual Shipment GetShipmentById(int shipmentId)
        {
            if (shipmentId == 0)
                return null;

            return _shipmentRepository.GetById(shipmentId);
        }

        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void InsertShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Insert(shipment);

            //event notification
            _eventPublisher.EntityInserted(shipment);
        }

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void UpdateShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Update(shipment);

            //event notification
            _eventPublisher.EntityUpdated(shipment);
        }


        
        /// <summary>
        /// Deletes a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        public virtual void DeleteShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");

            _siRepository.Delete(shipmentItem);

            //event notification
            _eventPublisher.EntityDeleted(shipmentItem);
        }

        /// <summary>
        /// Gets a shipment item
        /// </summary>
        /// <param name="shipmentItemId">Shipment item identifier</param>
        /// <returns>Shipment item</returns>
        public virtual ShipmentItem GetShipmentItemById(int shipmentItemId)
        {
            if (shipmentItemId == 0)
                return null;

            return _siRepository.GetById(shipmentItemId);
        }
        
        /// <summary>
        /// Inserts a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        public virtual void InsertShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");

            _siRepository.Insert(shipmentItem);

            //event notification
            _eventPublisher.EntityInserted(shipmentItem);
        }

        /// <summary>
        /// Updates the shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        public virtual void UpdateShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");

            _siRepository.Update(shipmentItem);

            //event notification
            _eventPublisher.EntityUpdated(shipmentItem);
        }




        /// <summary>
        /// Get quantity in shipments. For example, get planned quantity to be shipped
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="ignoreShipped">Ignore already shipped shipments</param>
        /// <param name="ignoreDelivered">Ignore already delivered shipments</param>
        /// <returns>Quantity</returns>
        public virtual int GetQuantityInShipments(Product product, int warehouseId,
            bool ignoreShipped, bool ignoreDelivered)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                return 0;
            if (!product.UseMultipleWarehouses)
                return 0;

            const int cancelledSubscriptionOrderStatusId = (int)SubscriptionOrderStatus.Cancelled;


            var query = _siRepository.Table;
            query = query.Where(si => !si.Shipment.OrderItem.SubscriptionOrder.Deleted);
            query = query.Where(si => si.Shipment.OrderItem.SubscriptionOrder.SubscriptionOrderStatusId != cancelledSubscriptionOrderStatusId);
            if (warehouseId > 0)
                query = query.Where(si => si.WarehouseId == warehouseId);
            if (ignoreShipped)
                query = query.Where(si => !si.Shipment.ShippedDateUtc.HasValue);
            if (ignoreDelivered)
                query = query.Where(si => !si.Shipment.DeliveryDateUtc.HasValue);

            var queryProductOrderItems = from itemDetail in _itemDetailRepository.Table
                                         join orderItem in _orderItemRepository.Table on itemDetail.OrderItemId equals orderItem.Id
                                         where itemDetail.ProductId == product.Id
                                         select itemDetail.Id;
            query = from si in query
                    where queryProductOrderItems.Any(orderItemId => orderItemId == si.ItemDetailId)
                    select si;

            //some null validation
            var result = Convert.ToInt32(query.Sum(si => (int?)si.Quantity));
            return result;
        }


        #endregion
    }
}
