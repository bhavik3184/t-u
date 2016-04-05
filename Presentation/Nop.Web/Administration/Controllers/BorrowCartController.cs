using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.BorrowCart;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Kendoui;

namespace Nop.Admin.Controllers
{
    public partial class BorrowCartController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;

        #endregion

        #region Constructors

        public BorrowCartController(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IStoreService storeService,
            ITaxService taxService, 
            IPriceCalculationService priceCalculationService,
            IPermissionService permissionService, 
            ILocalizationService localizationService,
            IProductAttributeFormatter productAttributeFormatter)
        {
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._storeService = storeService;
            this._taxService = taxService;
            this._priceCalculationService = priceCalculationService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._productAttributeFormatter = productAttributeFormatter;
        }

        #endregion
        
        #region Methods

        //shopping carts
        public ActionResult CurrentCarts()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult CurrentCarts(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithBorrowCart: true,
                sct: BorrowCartType.BorrowCart,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new BorrowCartModel
                {
                    CustomerId = x.Id,
                    CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    TotalItems = x.BorrowCartItems.Where(sci => sci.BorrowCartType == BorrowCartType.BorrowCart).ToList().GetTotalProducts()
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult GetCartDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.BorrowCartItems.Where(x => x.BorrowCartType == BorrowCartType.BorrowCart).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId); 
                    var sciModel = new BorrowCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = sci.Product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml, sci.Customer),
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci,true), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }

        //mytoyboxs
        public ActionResult CurrentMyToyBoxs()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult CurrentMyToyBoxs(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithBorrowCart: true,
                sct: BorrowCartType.MyToyBox,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new BorrowCartModel
                {
                    CustomerId = x.Id,
                    CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    TotalItems = x.BorrowCartItems.Where(sci => sci.BorrowCartType == BorrowCartType.MyToyBox).ToList().GetTotalProducts()
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult GetMyToyBoxDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.BorrowCartItems.Where(x => x.BorrowCartType == BorrowCartType.MyToyBox).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId); 
                    var sciModel = new BorrowCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = sci.Product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml, sci.Customer),
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }

        #endregion
    }
}
