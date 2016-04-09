using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Controllers
{
    public partial class PlanController : BasePublicController
    {
		#region Fields

        private readonly ICategoryService _categoryService;
        private readonly IMembershipCategoryService _membershipCategoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPlanService _planService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ISubscriptionOrderReportService _orderReportService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IShippingService _shippingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly SubscriptionCartSettings _borrowCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ICacheManager _cacheManager;
        private readonly ISubscriptionOrderService _subscriptionOrderService;
        
        #endregion

		#region Constructors

        public PlanController(ICategoryService categoryService, 
            IMembershipCategoryService membershipCategoryService,
            IManufacturerService manufacturerService,
            IPlanService planService, 
            IVendorService vendorService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ITaxService taxService, 
            ICurrencyService currencyService,
            IPictureService pictureService, 
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper, 
            IDateTimeHelper dateTimeHelper,
            IWorkflowMessageService workflowMessageService, 
            ISubscriptionOrderReportService orderReportService, 
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICustomerActivityService customerActivityService,
            IShippingService shippingService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            SubscriptionCartSettings borrowCartSettings,
            LocalizationSettings localizationSettings, 
            CustomerSettings customerSettings, 
            CaptchaSettings captchaSettings,
            SeoSettings seoSettings,
            ICacheManager cacheManager,
            ISubscriptionOrderService subscriptionOrderService)
        {
            this._categoryService = categoryService;
            this._membershipCategoryService = membershipCategoryService;
            this._manufacturerService = manufacturerService;
            this._planService = planService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._workflowMessageService = workflowMessageService;
            this._orderReportService = orderReportService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._customerActivityService = customerActivityService;
            this._shippingService = shippingService;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._borrowCartSettings = borrowCartSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._seoSettings = seoSettings;
            this._cacheManager = cacheManager;
            this._subscriptionOrderService = subscriptionOrderService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual IEnumerable<PlanOverviewModel> PreparePlanOverviewModels(IEnumerable<Plan> plans,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? planThumbPictureSize = null,  
            bool forceRedirectionAfterAddingToCart = false)
        {
            return this.PreparePlanOverviewModels(_workContext,
                _storeContext, _categoryService, _planService, _subscriptionOrderService,
                _priceCalculationService, _priceFormatter, _permissionService,
                _localizationService, _taxService, _currencyService,
                _pictureService, _webHelper, _cacheManager,
                _catalogSettings, _mediaSettings, plans,
                preparePriceModel, preparePictureModel,
                planThumbPictureSize,  
                forceRedirectionAfterAddingToCart);
        }

        [NonAction]
        protected virtual PlanDetailsModel PreparePlanDetailsPageModel(Plan plan, 
            SubscriptionCartItem updatecartitem = null, bool isAssociatedPlan = false)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            #region Standard properties

            var model = new PlanDetailsModel
            {
                Id = plan.Id,
                Name = plan.GetLocalized(x => x.Name),
                ShortDescription = plan.GetLocalized(x => x.ShortDescription),
                FullDescription = plan.GetLocalized(x => x.FullDescription),
                MetaKeywords = plan.GetLocalized(x => x.MetaKeywords),
                MetaDescription = plan.GetLocalized(x => x.MetaDescription),
                MetaTitle = plan.GetLocalized(x => x.MetaTitle),
                SeName = plan.GetSeName(),
                ShowSku = true,
                Sku = plan.Sku,
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = plan.ManufacturerPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = plan.Gtin,
                HasSampleDownload = plan.IsDownload && plan.HasSampleDownload,
            };

            

            //shipping info
            model.IsShipEnabled = plan.IsShipEnabled;
            if (plan.IsShipEnabled)
            {
                model.IsFreeShipping = plan.IsFreeShipping;
                //delivery date
                var deliveryDate = _shippingService.GetDeliveryDateById(plan.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = deliveryDate.GetLocalized(dd => dd.Name);
                }
            }
            
            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare plans

            #endregion


            #region Breadcrumb

            //do not prepare this model for the associated plans. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedPlan)
            {
                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY,
                    plan.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                model.Breadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                {
                    var breadcrumbModel = new PlanDetailsModel.PlanBreadcrumbModel
                    {
                        Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                        PlanId = plan.Id,
                        PlanName = plan.GetLocalized(x => x.Name),
                        PlanSeName = plan.GetSeName()
                    };
                    var planCategories = _categoryService.GetPlanCategoriesByPlanId(plan.Id);
                    if (planCategories.Count > 0)
                    {
                        var category = planCategories[0].Category;
                        if (category != null)
                        {
                            foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService))
                            {
                                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                                {
                                    Id = catBr.Id,
                                    Name = catBr.GetLocalized(x => x.Name),
                                    SeName = catBr.GetSeName(),
                                    IncludeInTopMenu = catBr.IncludeInTopMenu
                                });
                            }
                        }
                    }
                    return breadcrumbModel;
                });
            }
            
            #endregion

            #region Templates

           
            
            #endregion

            #region Plan price
            
            model.PlanPrice.PlanId = plan.Id;
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.PlanPrice.HidePrices = false;
                if (plan.CustomerEntersPrice)
                {
                    model.PlanPrice.CustomerEntersPrice = true;
                }
                else
                {
                    if (plan.CallForPrice)
                    {
                        model.PlanPrice.CallForPrice = true;
                    }
                    else
                    {
                       // decimal taxRate;
                        decimal oldPriceBase = decimal.Zero;
                        decimal finalPriceWithoutDiscountBase = decimal.Zero;
                        decimal finalPriceWithDiscountBase = decimal.Zero; 

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.PlanPrice.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.PlanPrice.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.PlanPrice.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.PlanPrice.PriceValue = finalPriceWithDiscount;
                        
                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this plan
                        //we also ensure this it's not free shipping
                   

                        //PAngV baseprice (used in Germany)
                        model.PlanPrice.BasePricePAngV = plan.FormatBasePrice(finalPriceWithDiscountBase,
                            _localizationService, _measureService, _currencyService, _workContext, _priceFormatter);

                        //currency code
                        model.PlanPrice.CurrencyCode = _workContext.WorkingCurrency.CurrencyCode;

                        //rental
                        if (plan.IsRental)
                        {
                            model.PlanPrice.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.PlanPrice.RentalPrice = "";
                        }
                    }
                }
            }
            else
            {
                model.PlanPrice.HidePrices = true;
                model.PlanPrice.OldPrice = null;
                model.PlanPrice.Price = null;
            }
            #endregion

            #region 'Add to cart' model

            model.AddToBag.PlanId = plan.Id;
            model.AddToBag.UpdatedSelectionBagItemId = updatecartitem != null ? updatecartitem.Id : 0;

            //quantity
            model.AddToBag.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : plan.SubscriptionMinimumQuantity;
            //allowed quantities
            var allowedQuantities = plan.ParseAllowedQuantities();
            foreach (var qty in allowedQuantities)
            {
                model.AddToBag.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }
            //minimum quantity notification
            if (plan.SubscriptionMinimumQuantity > 1)
            {
                model.AddToBag.MinimumQuantityNotification = string.Format(_localizationService.GetResource("Plans.MinimumQuantityNotification"), plan.SubscriptionMinimumQuantity);
            }

            //'add to cart', 'add to mytoybox' buttons
            model.AddToBag.DisableBuyButton = plan.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart);
            model.AddToBag.DisableMyToyBoxButton = plan.DisableMyToyBoxButton || !_permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.AddToBag.DisableBuyButton = true;
                model.AddToBag.DisableMyToyBoxButton = true;
            }
            //pre-order
            if (plan.AvailableForPreSubscription)
            {
                model.AddToBag.AvailableForPreSubscription = !plan.PreSubscriptionAvailabilityStartDateTimeUtc.HasValue ||
                    plan.PreSubscriptionAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.AddToBag.PreSubscriptionAvailabilityStartDateTimeUtc = plan.PreSubscriptionAvailabilityStartDateTimeUtc;
            }
            //rental
            model.AddToBag.IsRental = plan.IsRental;

            //customer entered price
            model.AddToBag.CustomerEntersPrice = plan.CustomerEntersPrice;
            if (model.AddToBag.CustomerEntersPrice)
            {
                decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(plan.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(plan.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

                model.AddToBag.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
                model.AddToBag.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Plans.EnterPlanPrice.Range"),
                    _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                    _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));
            }

            #endregion

            #region Tier prices

            if (plan.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = plan.TierPrices
                    .OrderBy(x => x.Quantity)
                    .ToList()
                    .FilterByStore(_storeContext.CurrentStore.Id)
                    .FilterForCustomer(_workContext.CurrentCustomer)
                    .RemoveDuplicatedQuantities()
                    .Select(tierPrice =>
                    {
                        var m = new PlanDetailsModel.TierPriceModel
                        {
                            Quantity = tierPrice.Quantity,
                        };
                      //  decimal taxRate;
                        decimal priceBase = decimal.Zero; 
                        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);
                        m.Price = _priceFormatter.FormatPrice(price, false, false);
                        return m;
                    })
                    .ToList();
            }

            #endregion

            #region Rental plans

            if (plan.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            #endregion
            
            return model;
        }

     
        #endregion

        #region Plan details page

        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult PlanDetails(int planId, int updatecartitemid = 0)
        {
            var plan = _planService.GetPlanById(planId);
            if (plan == null || plan.Deleted)
                return InvokeHttp404();

            

            //ACL (access control list)
            if (!_aclService.Authorize(plan))
                return InvokeHttp404();

            //Store mapping
            if (!_storeMappingService.Authorize(plan))
                return InvokeHttp404();

            //availability dates
            if (!plan.IsAvailable())
                return InvokeHttp404();
            
            //visible individually?



            //update existing shopping cart item?
            SubscriptionCartItem updatecartitem = null;
            if (_borrowCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.SubscriptionCartItems
                    .Where(x => x.SubscriptionCartType == SubscriptionCartType.SubscriptionCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Plan", new { SeName = plan.GetSeName() });
                }
                //is it this plan?
                if (plan.Id != updatecartitem.PlanId)
                {
                    return RedirectToRoute("Plan", new { SeName = plan.GetSeName() });
                }
            }

            //prepare the model
            var model = PreparePlanDetailsPageModel(plan, updatecartitem, false);

            //save as recently viewed

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewPlan", _localizationService.GetResource("ActivityLog.PublicStore.ViewPlan"), plan.Name);

            return View(model.PlanTemplateViewPath, model);
        }
     
        #endregion

        #region New (recently added) plans page

        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult NewPlans()
        {
            if (!_catalogSettings.NewProductsEnabled)
                return Content("");

            var plans = _planService.SearchPlans(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);

            var model = new List<PlanOverviewModel>();
            model.AddRange(PreparePlanOverviewModels(plans));

            return View(model);
        }

        public ActionResult NewPlansRss()
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: New plans", _storeContext.CurrentStore.GetLocalized(x => x.Name)),
                                    "Information about plans",
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "NewPlansRSS",
                                    DateTime.UtcNow);

            

            var items = new List<SyndicationItem>();

            var plans = _planService.SearchPlans(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);
            foreach (var plan in plans)
            {
                string planUrl = Url.RouteUrl("Plan", new { SeName = plan.GetSeName() }, "http");
                string planName = plan.GetLocalized(x => x.Name);
                string planDescription = plan.GetLocalized(x => x.ShortDescription);
                var item = new SyndicationItem(planName, planDescription, new Uri(planUrl), String.Format("NewPlan:{0}", plan.Id), plan.CreatedOnUtc);
                items.Add(item);
                //uncomment below if you want to add RSS enclosure for pictures
                //var picture = _pictureService.GetPicturesByPlanId(plan.Id, 1).FirstOrDefault();
                //if (picture != null)
                //{
                //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.PlanDetailsPictureSize);
                //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl)).CreateReader());
                //}

            }
            feed.Items = items;
            return new RssActionResult { Feed = feed };
        }

        #endregion

        #region Home page bestsellers and plans

        [ChildActionOnly]
        public ActionResult SubscriptionPlan(int? planThumbPictureSize)
        {
            var membershipcategories = _membershipCategoryService.GetAllCategories();

            SubscriptionCartPlansModel model = new SubscriptionCartPlansModel();

            foreach (MembershipCategory m in membershipcategories)
            {
                MembershipCategoryModel mc = new MembershipCategoryModel();
                mc.Id = m.Id;
                mc.Name = m.Name;
                var mebershipcat = _membershipCategoryService.GetPlanCategoriesByMembershipCategoryId(m.Id);
                IList<Plan> pl = new List<Plan>();
                foreach (PlanMembershipCategory mcp in mebershipcat)
                {
                    pl.Add(mcp.Plan);
                }
                var p = PreparePlanOverviewModels(pl, true, true, planThumbPictureSize).ToList();
                foreach (PlanOverviewModel mcp in p)
                {
                    mc.Products.Add(mcp);
                }
                model.MembershipCategories.Add(mc);
            }

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult HomepagePlans(int? planThumbPictureSize)
        {
            var membershipcategories = _membershipCategoryService.GetAllCategories();

            SubscriptionCartPlansModel model = new SubscriptionCartPlansModel();

            foreach (MembershipCategory m in membershipcategories)
            {
                MembershipCategoryModel mc = new MembershipCategoryModel();
                mc.Name = m.Name;
                var mebershipcat = _membershipCategoryService.GetPlanCategoriesByMembershipCategoryId(m.Id);
                IList<Plan> pl = new List<Plan>();
                foreach (PlanMembershipCategory mcp in mebershipcat)
                {
                    pl.Add(mcp.Plan);
                }
                var p = PreparePlanOverviewModels(pl, true, true, planThumbPictureSize).ToList();
                foreach (PlanOverviewModel mcp in p)
                {
                    mc.Products.Add(mcp);
                }
                model.MembershipCategories.Add(mc);
            }

            return PartialView(model);
        }

        #endregion
         
    }
}
