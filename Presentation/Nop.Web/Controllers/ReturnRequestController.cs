using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Seo;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Order;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using Nop.Services.Media;
using Nop.Web.Models.Media;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Core.Domain.Payments;

namespace Nop.Web.Controllers
{
    public partial class ReturnRequestController : BasePublicController
    {
		#region Fields

        private readonly IReturnRequestService _returnRequestService;
        private readonly ISubscriptionOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPictureService _pictureService;
        #endregion

        #region Constructors

        public ReturnRequestController(IReturnRequestService returnRequestService,
            ISubscriptionOrderService orderService, 
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService, 
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IDateTimeHelper dateTimeHelper,
            LocalizationSettings localizationSettings,
            ICacheManager cacheManager,
            IShipmentService shipmentService, 
            IShippingService shippingService,
            ShippingSettings shippingSettings,
            IPictureService pictureService
            )
        {
            this._returnRequestService = returnRequestService;
            this._orderService = orderService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._workflowMessageService = workflowMessageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationSettings = localizationSettings;
            this._cacheManager = cacheManager;
            this._shipmentService = shipmentService;
            this._shippingService = shippingService;
            this._shippingSettings = shippingSettings;
            this._pictureService = pictureService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual SubmitReturnRequestModel PrepareReturnRequestModel(SubmitReturnRequestModel model, SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.SubscriptionOrderId = order.Id;

            //return reasons
            model.AvailableReturnReasons = _cacheManager.Get(string.Format(ModelCacheEventConsumer.RETURNREQUESTREASONS_MODEL_KEY, _workContext.WorkingLanguage.Id),
                () =>
                {
                    var reasons = new List<SubmitReturnRequestModel.ReturnRequestReasonModel>();
                    foreach (var rrr in _returnRequestService.GetAllReturnRequestReasons())
                        reasons.Add(new SubmitReturnRequestModel.ReturnRequestReasonModel()
                        {
                            Id = rrr.Id,
                            Name = rrr.GetLocalized(x => x.Name)
                        });
                    return reasons;
                });

            //return actions
            model.AvailableReturnActions = _cacheManager.Get(string.Format(ModelCacheEventConsumer.RETURNREQUESTACTIONS_MODEL_KEY, _workContext.WorkingLanguage.Id),
                () =>
                {
                    var actions = new List<SubmitReturnRequestModel.ReturnRequestActionModel>();
                    foreach (var rra in _returnRequestService.GetAllReturnRequestActions())
                        actions.Add(new SubmitReturnRequestModel.ReturnRequestActionModel()
                        {
                            Id = rra.Id,
                            Name = rra.GetLocalized(x => x.Name)
                        });
                    return actions;
                });

            //products
            var orderItems = _orderService.GetAllOrderItems(order.Id, null, null, null, null, null, null);
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = new SubmitReturnRequestModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    //ProductId = orderItem.Product.Id,
                    //ProductName = orderItem.Product.GetLocalized(x => x.Name),
                    //ProductSeName = orderItem.Product.GetSeName(),
                   // AttributeInfo = orderItem.AttributeDescription,
                    //Quantity = orderItem.Quantity
                };
                model.Items.Add(orderItemModel);

                //unit price
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                  //  var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                  //  orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                   // var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                   // orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }
            }

            return model;
        }


        [NonAction]
        protected virtual SubmitReturnRequestModel PrepareReturnItemsModel(SubmitReturnRequestModel model, OrderItem orderItem)
        {
            if (orderItem ==null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.SubscriptionOrderId = orderItem.SubscriptionOrderId;
            model.TransactionId = orderItem.Id;
            //products
           // var orderItems = _orderService.GetAllOrderItems(order.Id, null, null, null, null, null, null);
            
                var orderItemModel = new SubmitReturnRequestModel.OrderItemModel
                {
                    Id = orderItem.Id,
                };

                foreach (var itemDetail in orderItem.ItemDetails)
                {
                   

                    
                    var itemDetailModel = new SubmitReturnRequestModel.OrderItemModel.ItemDetailModel
                    {
                        Id = itemDetail.Id,
                        ProductId = itemDetail.Product.Id,
                        ProductName = itemDetail.Product.GetLocalized(x => x.Name),
                        ProductSeName = itemDetail.Product.GetSeName(),
                        AttributeInfo = itemDetail.AttributeDescription,
                        Quantity = itemDetail.Quantity
                    };
                    int pictureSize = 150;
                    //prepare picture model

                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, itemDetail.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                    itemDetailModel.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPicturesByProductId(itemDetail.ProductId, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                        };

                        return pictureModel;
                    });

                    var returnRequest = _returnRequestService.SearchReturnRequests(itemDetailId: itemDetail.Id);

                    if (returnRequest.Count > 0)
                    { 
                        if(returnRequest.FirstOrDefault().ReturnRequestStatus == ReturnRequestStatus.Cancelled
                        || returnRequest.FirstOrDefault().ReturnRequestStatus == ReturnRequestStatus.RequestRejected)
                        { 
                            orderItemModel.ItemDetails.Add(itemDetailModel);
                        }
                    }
                    else
                    {
                        orderItemModel.ItemDetails.Add(itemDetailModel);
                    }
                }

                model.Items.Add(orderItemModel);
             
            return model;
        }

        [NonAction]
        protected virtual ShipmentModel PrepareShipmentModel(Shipment shipment, bool preparePlans, bool prepareShipmentEvent = false)
        {
            //measures

            var model = new ShipmentModel
            {
                Id = shipment.Id,
                TrackingNumber = shipment.TrackingNumber,
                ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.ShippedDate.NotYet"),
                ShippedDateUtc = shipment.ShippedDateUtc,
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.DeliveryDate.NotYet"),
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                AdminComment = shipment.AdminComment,
                SubscriptionOrderId = shipment.OrderItem.SubscriptionOrderId,
            };

           
            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetItemDetailById(shipmentItem.ItemDetailId);
                if (orderItem == null)
                    continue;

                //quantities
                var qtyInThisShipment = shipmentItem.Quantity;
                var maxQtyToAdd = 1;
                var qtyOrdered = orderItem.Quantity;
                var qtyInAllShipments = 1;

                var warehouse = _shippingService.GetWarehouseById(shipmentItem.WarehouseId);
                var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                {
                    Id = shipmentItem.Id,
                    OrderItemId = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    ProductImageUrl = orderItem.Product.
                    Sku = orderItem.Product.Sku,
                    AttributeInfo = orderItem.AttributeDescription,
                    ShippedFromWarehouse = warehouse != null ? warehouse.Name : null,
                    ShipSeparately = orderItem.Product.ShipSeparately,
                    QuantityOrdered = qtyOrdered,
                    QuantityInThisShipment = qtyInThisShipment,
                    QuantityInAllShipments = qtyInAllShipments,
                    QuantityToAdd = maxQtyToAdd,
                };
                  int pictureSize = 150;
                    //prepare picture model
                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, orderItem.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                    shipmentItemModel.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPicturesByProductId(orderItem.ProductId, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                        };
                       
                        
                        return pictureModel;
                    });

               
                //rental info
                //if (orderItem.Product.IsRental)
                //{
                //    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                //    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                //    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                //        rentalStartDate, rentalEndDate);
                //}

                model.Items.Add(shipmentItemModel);
            }

            return model;
        }

        [NonAction]
        protected virtual DeliveryListModel PrepareDeliveryModel(OrderItem orderItem, bool preparePlans, bool prepareShipmentEvent = false)
        {
            //measures
            SubscriptionOrder order = orderItem.SubscriptionOrder;
            //quantities
         //   var qtyOrdered = orderItem.Quantity;

            var deliveryModel = new DeliveryListModel
            {
               BatchId = orderItem.Id,
                SubscriptionOrderId = order.Id,
                OrderItemId = orderItem.Id,
                
            };
            foreach (var itemDetail in orderItem.ItemDetails)
            {
                var model1 = new DeliveryModel
                {
                    ProductId = itemDetail.ProductId,
                    ProductName = itemDetail.Product.Name,
                    //ProductImageUrl = itemDetail.Product.defa
                    Sku = itemDetail.Product.Sku,
                    AttributeInfo = itemDetail.AttributeDescription,
                    //ShipSeparately = itemDetail.Product.ShipSeparately,
                    //QuantityOrdered = qtyOrdered,
                };

                var itm=  orderItem.Shipments.Where(x => x.ShipmentItems.Where(y => y.ItemDetailId==itemDetail.Id).Any());
                if (itm.Count() > 0)
                    model1.IsPendingDelivery = false;
                else
                    model1.IsPendingDelivery = true;

                int pictureSize = 150;
            //prepare picture model
                var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, itemDetail.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                model1.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPicturesByProductId(itemDetail.ProductId, 1).FirstOrDefault();
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                    };

                    return pictureModel;
                });

          //  rental info
            //if (itemDetail.Product.IsRental)
            //{
            //    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
            //    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
            //    model1.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
            //        rentalStartDate, rentalEndDate);
            //}
            deliveryModel.Deliveries.Add(model1);
            }

            deliveryModel.IsPendingDeliveryList = preparePlans;

            return deliveryModel;
        }

        #endregion
        
        #region Methods

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerReturnRequests()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerReturnRequestsModel();

            var returnRequests = _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                 _workContext.CurrentCustomer.Id, rs: ReturnRequestStatus.Pending);

          
            List<CustomerReturnRequestsModel.TransactionItemModel.ReturnRequestModel> returnRequestItems = new List<CustomerReturnRequestsModel.TransactionItemModel.ReturnRequestModel>();

            foreach (var returnRequest in returnRequests)
            {
                var itemDetail = _orderService.GetItemDetailById(returnRequest.ItemDetailId);

                if (itemDetail != null)
                {
                    var product = itemDetail.Product;
                  
                    var itemModel = new CustomerReturnRequestsModel.TransactionItemModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        ReturnRequestStatus = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ProductId = product.Id,
                        OrderItemId = itemDetail.OrderItemId,
                        ItemDetailId = itemDetail.Id,
                        ProductName = product.GetLocalized(x => x.Name),
                        ProductSeName = product.GetSeName(),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.CustomerComments,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    int pictureSize = 150;
                    //prepare picture model
                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, itemDetail.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                    itemModel.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPicturesByProductId(itemDetail.ProductId, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                        };

                        return pictureModel;
                    });

                    returnRequestItems.Add(itemModel);
                }
            }


            List<int> result = returnRequestItems.Select(o => o.OrderItemId).Distinct().ToList();

            foreach(int i in result)
            {
                var transModel = new CustomerReturnRequestsModel.TransactionItemModel();
                transModel.Id = i;
                foreach (var item in returnRequestItems.Where(x => x.OrderItemId == i))
                {
                    transModel.Items.Add(item);
                }
                model.TransactionItems.Add(transModel);
            }

          //  model.TransactionItem = transModel;
            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerReturnedItems()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerReturnRequestsModel();

            var returnRequests = _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id,rs:ReturnRequestStatus.Received);
           List<CustomerReturnRequestsModel.TransactionItemModel.ReturnRequestModel> returnRequestItems = new List<CustomerReturnRequestsModel.TransactionItemModel.ReturnRequestModel>();

            foreach (var returnRequest in returnRequests)
            {
                var itemDetail = _orderService.GetItemDetailById(returnRequest.ItemDetailId);

                if (itemDetail != null)
                {
                    var product = itemDetail.Product;
                  
                    var itemModel = new CustomerReturnRequestsModel.TransactionItemModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        ReturnRequestStatus = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ProductId = product.Id,
                        OrderItemId = itemDetail.OrderItemId,
                        ItemDetailId = itemDetail.Id,
                        ProductName = product.GetLocalized(x => x.Name),
                        ProductSeName = product.GetSeName(),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.CustomerComments,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    int pictureSize = 150;
                    //prepare picture model
                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, itemDetail.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                    itemModel.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPicturesByProductId(itemDetail.ProductId, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                        };

                        return pictureModel;
                    });

                    returnRequestItems.Add(itemModel);
                }
            }


            List<int> result = returnRequestItems.Select(o => o.OrderItemId).Distinct().ToList();

            foreach(int i in result)
            {
                var transModel = new CustomerReturnRequestsModel.TransactionItemModel();
                transModel.Id = i;
                foreach (var item in returnRequestItems.Where(x => x.OrderItemId == i))
                {
                    transModel.Items.Add(item);
                }
                model.TransactionItems.Add(transModel);
            }

          //  model.TransactionItem = transModel;
            return View(model);
        
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerPendingDeliveryItems()
        {
            var model = new DeliveryBatchListModel();
            var orderItems = _orderService.GetAllOrderItemsCount(orderId: 0, customerId: _workContext.CurrentCustomer.Id, createdFromUtc: null, createdToUtc: null, os: SubscriptionOrderStatus.Complete, ps: PaymentStatus.Paid, ss: ShippingStatus.Shipped);
            //var shipments = _shipmentService.GetAllShipments( customerId:_workContext.CurrentCustomer.Id );

            if (orderItems.Count > 0)
            {
                foreach (var orderItem in orderItems)
                {
                    if (orderItem.ItemDetails.Count > 0)
                    {
                        SubscriptionOrder order = orderItem.SubscriptionOrder;
                        //quantities
                       // var qtyOrdered = orderItem.Quantity;

                        var deliveryModel = new DeliveryListModel
                        {
                            BatchId = orderItem.Id,
                            SubscriptionOrderId = order.Id,
                            OrderItemId = orderItem.Id,

                        };
                        foreach (var itemDetail in orderItem.ItemDetails)
                        {
                            var model1 = new DeliveryModel
                            {
                                ProductId = itemDetail.ProductId,
                                ProductName = itemDetail.Product.Name,
                                //ProductImageUrl = itemDetail.Product.defa
                                Sku = itemDetail.Product.Sku,
                                AttributeInfo = itemDetail.AttributeDescription,
                               // TransactionDate = itemDetail.OrderItem.SubscriptionOrder.CreatedOnUtc,
                                //QuantityOrdered = qtyOrdered,
                            };

                            int pictureSize = 150;
                            //prepare picture model
                            var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, itemDetail.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                            model1.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                            {
                                var picture = _pictureService.GetPicturesByProductId(itemDetail.ProductId, 1).FirstOrDefault();
                                var pictureModel = new PictureModel
                                {
                                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                                };

                                return pictureModel;
                            });

                            //  rental info
                            //if (itemDetail.Product.IsRental)
                            //{
                            //    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                            //    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                            //    model1.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                            //        rentalStartDate, rentalEndDate);
                            //}

                            var itm = orderItem.Shipments.Where(x => x.ShipmentItems.Where(y => y.ItemDetailId == itemDetail.Id && x.DeliveryDateUtc !=null).Any());
                            if (itm.Count() == 0)
                            {
                                deliveryModel.Deliveries.Add(model1);
                            }
                        }



                        foreach (var shipment in orderItem.Shipments)
                        {
                            var shipmentModel = new ShipmentModel();
                            if (shipment.DeliveryDateUtc != null)
                            {

                                shipmentModel.TrackingNumber = shipment.TrackingNumber;
                                shipmentModel.ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.ShippedDate.NotYet");
                                shipmentModel.ShippedDateUtc = shipment.ShippedDateUtc;

                                //  ShipSeparately = orderItem.Product.ShipSeparately,
                                //deliveryModel.QuantityInThisShipment = qtyInThisShipment;
                                shipmentModel.DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.DeliveryDate.NotYet");
                                shipmentModel.DeliveryDateUtc = shipment.DeliveryDateUtc;
                                shipmentModel.CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue;
                                if (shipment.DeliveryDateUtc.HasValue)
                                    deliveryModel.Shipments.Add(shipmentModel);
                            }
                        }

                        //   if (displayOrderitem ==1)
                        model.DeliveryList.Add(deliveryModel);
                    }
                }
            }

            return View(model);
        }

      
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerDeliveredItems()
        {
            var model = new DeliveryBatchListModel();
            var orderItems = _orderService.GetAllOrderItemsCount(orderId: 0, customerId: _workContext.CurrentCustomer.Id, createdFromUtc: null, createdToUtc: null, os: SubscriptionOrderStatus.Complete, ps: PaymentStatus.Paid, ss: ShippingStatus.Shipped);
            //var shipments = _shipmentService.GetAllShipments( customerId:_workContext.CurrentCustomer.Id );
            
            if (orderItems.Count > 0)
            {
                foreach (var orderItem in orderItems)
                {
                    if (orderItem.ItemDetails.Count > 0)
                    {
                        SubscriptionOrder order = orderItem.SubscriptionOrder;
                        //quantities
                       // var qtyOrdered = orderItem.Quantity;

                        var deliveryModel = new DeliveryListModel
                        {
                            BatchId = orderItem.Id,
                            SubscriptionOrderId = order.Id,
                            OrderItemId = orderItem.Id,

                        };
                        foreach (var itemDetail in orderItem.ItemDetails)
                        {
                            var model1 = new DeliveryModel
                            {
                                ProductId = itemDetail.ProductId,
                                ProductName = itemDetail.Product.Name,
                                //ProductImageUrl = itemDetail.Product.defa
                                Sku = itemDetail.Product.Sku,
                                AttributeInfo = itemDetail.AttributeDescription,
                                //ShipSeparately = itemDetail.Product.ShipSeparately,
                                //QuantityOrdered = qtyOrdered,
                            };

                            var returnRequest = _returnRequestService.SearchReturnRequests(itemDetailId:itemDetail.Id);

                            if (returnRequest.Count > 0)
                            {
                                if (returnRequest.FirstOrDefault().ReturnRequestStatus == ReturnRequestStatus.Cancelled
                                    || returnRequest.FirstOrDefault().ReturnRequestStatus == ReturnRequestStatus.RequestRejected)
                                {
                                    deliveryModel.IsPendingReturn = true;
                                }

                                if (returnRequest.FirstOrDefault().ReturnRequestStatus == ReturnRequestStatus.Received)
                                    deliveryModel.IsPendingReturn = false;

                                model1.ReturnStatus = "Return " + returnRequest.FirstOrDefault().ReturnRequestStatus.ToString();
                            }else
                            {
                                deliveryModel.IsPendingReturn = true;
                            }

                            int pictureSize = 150;
                            //prepare picture model
                            var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, itemDetail.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, false, _storeContext.CurrentStore.Id);
                            model1.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                            {
                                var picture = _pictureService.GetPicturesByProductId(itemDetail.ProductId, 1).FirstOrDefault();
                                var pictureModel = new PictureModel
                                {
                                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                                };

                                return pictureModel;
                            });

                            //  rental info
                            //if (itemDetail.Product.IsRental)
                            //{
                            //    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                            //    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? itemDetail.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                            //    model1.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                            //        rentalStartDate, rentalEndDate);
                            //}

                            var itm = orderItem.Shipments.Where(x => x.ShipmentItems.Where(y => y.ItemDetailId == itemDetail.Id && x.DeliveryDateUtc != null).Any());
                            if (itm.Count() > 0)
                            {
                                deliveryModel.Deliveries.Add(model1);
                            }
                          
                        }

                      

                        foreach (var shipment in orderItem.Shipments)
                        {
                            var shipmentModel = new ShipmentModel();
                            if (shipment.DeliveryDateUtc != null)
                            {

                                shipmentModel.TrackingNumber = shipment.TrackingNumber;
                                shipmentModel.ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.ShippedDate.NotYet");
                                shipmentModel.ShippedDateUtc = shipment.ShippedDateUtc;

                                //  ShipSeparately = orderItem.Product.ShipSeparately,
                                //deliveryModel.QuantityInThisShipment = qtyInThisShipment;
                                shipmentModel.DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.SubscriptionOrders.Shipments.DeliveryDate.NotYet");
                                shipmentModel.DeliveryDateUtc = shipment.DeliveryDateUtc;
                                shipmentModel.CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue;
                                if (shipment.DeliveryDateUtc.HasValue)
                                deliveryModel.Shipments.Add(shipmentModel);
                            }
                        }

                     //   if (displayOrderitem ==1)
                        model.DeliveryList.Add(deliveryModel);
                    }
                }
            }

            return View(model);
        }

     
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ReturnRequest(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("HomePage");

            var model = new SubmitReturnRequestModel();
            model = PrepareReturnRequestModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("ReturnRequest")]
        [ValidateInput(false)]
        [PublicAntiForgery]
        public ActionResult ReturnRequestSubmit(int orderId, SubmitReturnRequestModel model, FormCollection form)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("HomePage");

            int count = 0;
            foreach (var orderItem in order.OrderItems)
            {
                int quantity = 0; //parse quantity
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("quantity{0}", orderItem.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out quantity);
                        break;
                    }
                if (quantity > 0)
                {
                    var rrr = _returnRequestService.GetReturnRequestReasonById(model.ReturnRequestReasonId);
                    var rra = _returnRequestService.GetReturnRequestActionById(model.ReturnRequestActionId);

                    var rr = new ReturnRequest
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        ItemDetailId = orderItem.Id,
                        Quantity = quantity,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        ReasonForReturn = "not available",
                        RequestedAction = "not available",
                        CustomerComments = model.Comments,
                        StaffNotes = string.Empty,
                        ReturnRequestStatus = ReturnRequestStatus.Pending,
                        AvailableDateUtc = model.AvailableDate,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    _workContext.CurrentCustomer.ReturnRequests.Add(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    //notify store owner here (email)
                    _workflowMessageService.SendNewReturnRequestStoreOwnerNotification(rr, orderItem, _localizationSettings.DefaultAdminLanguageId);

                    count++;
                }
            }

            model = PrepareReturnRequestModel(model, order);
            if (count > 0)
                model.Result = _localizationService.GetResource("ReturnRequests.Submitted");
            else
                model.Result = _localizationService.GetResource("ReturnRequests.NoItemsSubmitted");

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ReturnItems(int orderItemId)
        {
            var orderItem = _orderService.GetOrderItemById(orderItemId);
            if (orderItem == null || orderItem.SubscriptionOrder.Deleted || _workContext.CurrentCustomer.Id != orderItem.SubscriptionOrder.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_orderProcessingService.IsReturnRequestAllowed(orderItem.SubscriptionOrder))
                return RedirectToRoute("HomePage");

            var model = new SubmitReturnRequestModel();
            model = PrepareReturnItemsModel(model, orderItem);
            return View(model);
        }

        [HttpPost, ActionName("ReturnItems")]
        [ValidateInput(false)]
        [PublicAntiForgery]
        public ActionResult ReturnItemsSubmit(int orderItemId, SubmitReturnRequestModel model, FormCollection form)
        {
            var orderItem = _orderService.GetOrderItemById(orderItemId);
            if (orderItem == null || orderItem.SubscriptionOrder.Deleted || _workContext.CurrentCustomer.Id != orderItem.SubscriptionOrder.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_orderProcessingService.IsReturnRequestAllowed(orderItem.SubscriptionOrder))
                return RedirectToRoute("HomePage");

            int count = 0;
            foreach (var itemDetail in orderItem.ItemDetails)
            {
                bool quantity = false; //parse quantity
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("quantity{0}", itemDetail.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                         
                        quantity=true;
                        break;
                    }
                if (quantity )
                {
                    var rrr = _returnRequestService.GetReturnRequestReasonById(model.ReturnRequestReasonId);
                    var rra = _returnRequestService.GetReturnRequestActionById(model.ReturnRequestActionId);

                    var rr = new ReturnRequest
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        ItemDetailId = itemDetail.Id,
                        Quantity = 1,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        ReasonForReturn = "not available",
                        RequestedAction = "not available",
                        CustomerComments = model.Comments,
                        StaffNotes = string.Empty,
                        ReturnRequestStatus = ReturnRequestStatus.Pending,
                        AvailableDateUtc = model.AvailableDate,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    _workContext.CurrentCustomer.ReturnRequests.Add(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    //notify store owner here (email)
                    _workflowMessageService.SendNewReturnRequestStoreOwnerNotification(rr, orderItem, _localizationSettings.DefaultAdminLanguageId);

                    count++;
                }
            }

            model = PrepareReturnItemsModel(model, orderItem);
            if (count > 0)
                model.Result = _localizationService.GetResource("ReturnRequests.Submitted");
            else
                model.Result = _localizationService.GetResource("ReturnRequests.NoItemsSubmitted");

            return View(model);
        }

        #endregion
    }
}
