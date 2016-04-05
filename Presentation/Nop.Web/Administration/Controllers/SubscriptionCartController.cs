using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.SubscriptionCart;
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
    public partial class SubscriptionCartController : BaseAdminController
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

        public SubscriptionCartController(ICustomerService customerService,
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

      
        [HttpPost]
        public ActionResult GetCartDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.SubscriptionCartItems.Where(x => x.SubscriptionCartType == SubscriptionCartType.SubscriptionCart).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId); 
                    var sciModel = new SubscriptionCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        PlanId = sci.PlanId,
                        Quantity = sci.Quantity,
                        PlanName = sci.Plan.Name,
                        AttributeInfo = "",
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetUnitPrice(sci), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }





        

        [HttpPost]
        public ActionResult GetMyToyBoxDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.SubscriptionCartItems.Where(x => x.SubscriptionCartType == SubscriptionCartType.MyToyBox).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId); 
                    var sciModel = new SubscriptionCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        PlanId = sci.PlanId,
                        Quantity = sci.Quantity,
                        PlanName = sci.Plan.Name,
                        AttributeInfo = "",
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetUnitPrice(sci), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetPlanPrice(sci.Plan, _priceCalculationService.GetSubTotal(sci), out taxRate)),
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
