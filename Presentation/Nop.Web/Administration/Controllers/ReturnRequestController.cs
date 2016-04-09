using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System.Linq;
using Nop.Admin.Models.SubscriptionOrders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Payments;

namespace Nop.Admin.Controllers
{
    public partial class ReturnRequestController : BaseAdminController
    {
        #region Fields
        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ISubscriptionOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ReturnRequestController(ISubscriptionOrderService subscriptionService,
            IReturnRequestService returnRequestService,
            ISubscriptionOrderService orderService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ICustomerActivityService customerActivityService, 
            IPermissionService permissionService)
        {
            this._subscriptionService = subscriptionService;
            this._returnRequestService = returnRequestService;
            this._orderService = orderService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual bool PrepareReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var itemDetail = _orderService.GetItemDetailById(returnRequest.ItemDetailId);
            if (itemDetail == null)
                return false;

            model.Id = returnRequest.Id;
            model.ItemDetailId =returnRequest.ItemDetailId;
            model.Sku = itemDetail.Product.Sku;
            model.ProductId = itemDetail.ProductId;
            model.ProductName = itemDetail.Product.Name;
            model.OrderItemId = itemDetail.OrderItemId;
            model.SubscriptionOrderId = itemDetail.OrderItem.SubscriptionOrderId;
            model.CustomerId = returnRequest.CustomerId;
            var customer = returnRequest.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.Quantity = returnRequest.Quantity;
            model.QuantityOrdered = itemDetail.Quantity;
            model.ReturnRequestStatusStr = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);
            model.AvailableDateUtc = _dateTimeHelper.ConvertToUserTime(returnRequest.AvailableDateUtc, DateTimeKind.Utc);

            if (!excludeProperties)
            {
                model.ReasonForReturn = returnRequest.ReasonForReturn;
                model.RequestedAction = returnRequest.RequestedAction;
                model.CustomerComments = returnRequest.CustomerComments;
                model.StaffNotes = returnRequest.StaffNotes;
                model.ReturnRequestStatusId = returnRequest.ReturnRequestStatusId;
            }
            //model is successfully prepared
            return true;
        }

        #endregion

        #region Methods

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var model = new ReturnTransactionListModel();
            //countries

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ReturnTransactionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptionOrders))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his plans
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            ////load shipments
            //DateTime? startDateValue = (model.StartDate == null) ? null
            //               : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            //DateTime? endDateValue = (model.EndDate == null) ? null
            //                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionOrderStatus? orderStatus = model.SubscriptionOrderStatusId > 0 ? (SubscriptionOrderStatus?)(model.SubscriptionOrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


            //load orders
            var orders = _subscriptionService.SearchSubscriptionOrders(
                //storeId: model.StoreId,
                //vendorId: model.VendorId,
                //planId: filterByPlanId,
                //warehouseId: model.WarehouseId,
                //paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                //billingEmail: model.BillingEmail,
                //billingLastName: model.BillingLastName,
                //billingCountryId: model.BillingCountryId,
                //orderNotes: model.SubscriptionOrderNotes,
                //orderGuid: model.SubscriptionOrderGuid,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            List<BorrowTransactionModel> orderItems = new List<BorrowTransactionModel>();
            //List<OrderItem> orderItems = new List<OrderItem>();
            
            foreach (var order in orders)
            {
                foreach (var orderitem in order.OrderItems)
                {
                    var Borrowtrans = new BorrowTransactionModel
                   {
                       OrderItemId = orderitem.Id,
                       SubscriptionOrderId = orderitem.SubscriptionOrderId,
                       SubscriptionOrderStatus = orderitem.SubscriptionOrder.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                       ShippingStatus = orderitem.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                       CustomerEmail = orderitem.SubscriptionOrder.Customer.Email,
                       CreatedOn = orderitem.CreatedOnUtc,
                       TotalItems = 0
                   };
                    int status = 0;

                    foreach (ItemDetail itm in orderitem.ItemDetails)
                    {
                        var searchRequestItem = _returnRequestService.SearchReturnRequests(itemDetailId: itm.Id);
                        if(searchRequestItem.Count()==0){
                            Borrowtrans.ShippingStatus = "Pending";
                            status = 1;
                            continue;
                        }
                        else if (searchRequestItem.FirstOrDefault().ReturnRequestStatus != ReturnRequestStatus.Received)
                        {
                            Borrowtrans.TotalItems = Borrowtrans.TotalItems + 1;
                            status = 1;
                            continue;
                                
                        }
                        else if (searchRequestItem.FirstOrDefault().ReturnRequestStatus == ReturnRequestStatus.Received)
                        {
                            Borrowtrans.TotalItems = Borrowtrans.TotalItems + 1;
                            if (status != 1)
                            {
                                status = 10;
                            }
                        }
                    }

                    if (status == 0)
                        Borrowtrans.ShippingStatus = "Pending";
                    if (status == 1)
                        Borrowtrans.ShippingStatus = "Pending";
                    else if(status==10)
                        Borrowtrans.ShippingStatus = "All Received";

                    orderItems.Add(Borrowtrans);
                }
            }



            var gridModel = new DataSourceResult
            {
                Data = orderItems,
                Total = orderItems.Count()
            };


            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult GetReturnTransactionDetails(int orderItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequests = _returnRequestService.SearchReturnRequestByTransactionId(orderItemId:orderItemId);
            var returnRequestModels = new List<ReturnRequestModel>();
            foreach (var rr in returnRequests)
            {
                var m = new ReturnRequestModel();
                if (PrepareReturnRequestModel(m, rr, false))
                    returnRequestModels.Add(m);
            }
            var gridModel = new DataSourceResult
            {
                Data = returnRequestModels,
                Total = returnRequests.Count(),
            };

            return Json(gridModel);

        }

        //edit
        public ActionResult EditTransaction(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequests = _returnRequestService.SearchReturnRequestByTransactionId(orderItemId: id);
            if (returnRequests.Count== 0)
                //No return request found with the specified id
                return RedirectToAction("List");

            var orderItem = _orderService.GetOrderItemById(id);
            if (orderItem == null)
                return RedirectToAction("List");

            var model = new ReturnRequestTransactionModel();
            var customer = returnRequests.FirstOrDefault().Customer;
            model.CustomerId = returnRequests.FirstOrDefault().CustomerId;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.OrderItemId = orderItem.Id;
            model.SubscriptionOrderId = orderItem.SubscriptionOrderId;

            foreach (ReturnRequest r in returnRequests) {
                var model1 = new ReturnRequestModel();
                model1.OrderItemId = id;
                PrepareReturnRequestModel(model1, r, false);
                model.Items.Add(model1);
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult EditTransaction(ReturnRequestTransactionModel model, FormCollection form, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();
            //If we got this far, something failed, redisplay form
            if (ModelState.IsValid)
            {
                var returnRequests = _returnRequestService.SearchReturnRequestByTransactionId(orderItemId: model.OrderItemId);
                if (returnRequests.Count == 0)
                    //No return request found with the specified id
                    return RedirectToAction("List");

                foreach (ReturnRequest model1 in returnRequests)
                {

                    var returnRequest = _returnRequestService.GetReturnRequestById(model1.Id);
                    if (returnRequest == null)
                        //No return request found with the specified id
                        return RedirectToAction("List");

                    int qtyToAdd = 0; //parse quantity
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("qtyToAdd{0}", returnRequest.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out qtyToAdd);
                            break;
                        }

                         if (qtyToAdd > 0)
                         {
                             returnRequest.Quantity = qtyToAdd;
                            returnRequest.ReasonForReturn = "";
                            returnRequest.RequestedAction = "";
                            returnRequest.CustomerComments = model.CustomerComments;
                            returnRequest.StaffNotes = model.StaffNotes;
                            returnRequest.ReturnRequestStatusId = model.ReturnRequestStatusId;
                            returnRequest.AvailableDateUtc = model1.AvailableDateUtc;
                            returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                            _customerService.UpdateCustomer(returnRequest.Customer);

                            //activity log
                            _customerActivityService.InsertActivity("EditReturnRequest", _localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id);
                        }
                    }

                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));
                return continueEditing ? RedirectToAction("EditTransaction", new { id = model.OrderItemId }) : RedirectToAction("List");

            }
              
            return View(model);
        }



        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            var model = new ReturnRequestModel();
            PrepareReturnRequestModel(model, returnRequest, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                returnRequest.Quantity = model.Quantity;
                returnRequest.ReasonForReturn = model.ReasonForReturn;
                returnRequest.RequestedAction = model.RequestedAction;
                returnRequest.CustomerComments = model.CustomerComments;
                returnRequest.StaffNotes = model.StaffNotes;
                returnRequest.ReturnRequestStatusId = model.ReturnRequestStatusId;
                returnRequest.AvailableDateUtc = model.AvailableDateUtc;
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                _customerService.UpdateCustomer(returnRequest.Customer);

                //activity log
                _customerActivityService.InsertActivity("EditReturnRequest", _localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id);

                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareReturnRequestModel(model, returnRequest, true);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public ActionResult NotifyCustomer(ReturnRequestModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            //var customer = returnRequest.Customer;
            var orderItem = _orderService.GetOrderItemById(returnRequest.ItemDetailId);
            int queuedEmailId = _workflowMessageService.SendReturnRequestStatusChangedCustomerNotification(returnRequest, orderItem, _localizationSettings.DefaultAdminLanguageId);
            if (queuedEmailId > 0)
                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Notified"));
            return RedirectToAction("Edit",  new {id = returnRequest.Id});
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            _returnRequestService.DeleteReturnRequest(returnRequest);

            //activity log
            _customerActivityService.InsertActivity("DeleteReturnRequest", _localizationService.GetResource("ActivityLog.DeleteReturnRequest"), returnRequest.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}
