using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Infrastructure.Cache;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.SubscriptionOrders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Core.Caching;

namespace Nop.Admin.Controllers
{
    public partial class PlanController : BaseAdminController
    {
        #region Fields

        private readonly IPlanService _planService;
        private readonly IMembershipCategoryService _membershipCategoryService;
        private readonly ICopyPlanService _copyPlanService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPictureService _pictureService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IPdfService _pdfService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorService _vendorService;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionOrderService;
        private readonly ISubscriptionCartService _subscriptionCartService;
        private readonly IPlanAttributeFormatter _planAttributeFormatter;
        private readonly IDownloadService _downloadService;

        #endregion

		#region Constructors

        public PlanController(IPlanService planService,
            IMembershipCategoryService membershipCategoryService,
            ICopyPlanService copyPlanService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService, 
            IWorkContext workContext, 
            ILanguageService languageService, 
            ILocalizationService localizationService, 
            ILocalizedEntityService localizedEntityService,
            ISpecificationAttributeService specificationAttributeService, 
            IPictureService pictureService,
            ITaxCategoryService taxCategoryService, 
            IPdfService pdfService,
            IExportManager exportManager, 
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService, 
            IAclService aclService,
            IStoreService storeService,
            ISubscriptionOrderService subscriptionService,
            IStoreMappingService storeMappingService,
             IVendorService vendorService,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ICurrencyService currencyService, 
            CurrencySettings currencySettings,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            ICacheManager cacheManager,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IBackInStockSubscriptionService backInStockSubscriptionOrderService,
            ISubscriptionCartService subscriptionCartService,
            IPlanAttributeFormatter planAttributeFormatter,
            IDownloadService downloadService)
        {
            this._planService = planService;
            this._membershipCategoryService = membershipCategoryService;
            this._copyPlanService = copyPlanService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._specificationAttributeService = specificationAttributeService;
            this._pictureService = pictureService;
            this._taxCategoryService = taxCategoryService;
            this._pdfService = pdfService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._subscriptionService = subscriptionService;
            this._storeMappingService = storeMappingService;
            this._vendorService = vendorService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._discountService = discountService;
            this._backInStockSubscriptionOrderService = backInStockSubscriptionOrderService;
            this._subscriptionCartService = subscriptionCartService;
            this._planAttributeFormatter = planAttributeFormatter;
            this._downloadService = downloadService;
        }

        #endregion 

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Plan plan, PlanModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(plan,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(plan,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(plan,
                                                               x => x.FullDescription,
                                                               localized.FullDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(plan,
                                                               x => x.MetaKeywords,
                                                               localized.MetaKeywords,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(plan,
                                                               x => x.MetaDescription,
                                                               localized.MetaDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(plan,
                                                               x => x.MetaTitle,
                                                               localized.MetaTitle,
                                                               localized.LanguageId);

                //search engine name
                var seName = plan.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(plan, seName, localized.LanguageId);
            }
        }

        

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Plan plan)
        {
            foreach (var pp in plan.PlanPictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(plan.Name));
        }
        
        [NonAction]
        protected virtual void PrepareAclModel(PlanModel model, Plan plan, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCustomerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (plan != null)
                {
                    model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(plan);
                }
            }
        }

        [NonAction]
        protected virtual void SavePlanAcl(Plan plan, PlanModel model)
        {
            var existingAclRecords = _aclService.GetAclRecords(plan);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(plan, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(PlanModel model, Plan plan, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableStores = _storeService
                .GetAllStores()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (plan != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(plan);
                }
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Plan plan, PlanModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(plan);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(plan, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }
       

        [NonAction]
        protected virtual void PreparePlanModel(PlanModel model, Plan plan,
            bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (plan != null)
            {
                var parentGroupedPlan = _planService.GetPlanById(plan.ParentGroupedPlanId);
                if (parentGroupedPlan != null)
                {
                    model.AssociatedToPlanId = plan.ParentGroupedPlanId;
                    model.AssociatedToPlanName = parentGroupedPlan.Name;
                }
            }

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            model.BaseDimensionIn = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId).Name;
            if (plan != null)
            {
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(plan.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(plan.UpdatedOnUtc, DateTimeKind.Utc);
            }

            //little performance hack here
            //there's no need to load attributes, categories, manufacturers when creating a new plan
            //anyway they're not used (you need to save a plan before you map add them)
            if (plan != null)
            {
               

                //categories
                var allMembershipCategories = _membershipCategoryService.GetAllCategories(showHidden: true);
                foreach (var category in allMembershipCategories)
                {
                    model.AvailableMembershipCategories.Add(new SelectListItem
                    {
                        Text = category.GetFormattedBreadCrumb(allMembershipCategories),
                        Value = category.Id.ToString()
                    });
                }

                var allCategories = _categoryService.GetAllCategories(showHidden: true);
                foreach (var category in allCategories)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Text = category.GetFormattedBreadCrumb(allCategories),
                        Value = category.Id.ToString()
                    });
                }

                var allBaseCategories = _categoryService.GetAllBaseCategories(showHidden: true);
                foreach (var category in allBaseCategories)
                {
                    model.AvailableBaseCategories.Add(new SelectListItem
                    {
                        Text = category.GetFormattedBreadCrumb(allBaseCategories),
                        Value = category.Id.ToString()
                    });
                }

                //specification attributes
                model.AddSpecificationAttributeModel.AvailableAttributes = _cacheManager
                    .Get(ModelCacheEventConsumer.SPEC_ATTRIBUTES_MODEL_KEY, () =>
                    {
                        var availableSpecificationAttributes = new List<SelectListItem>();
                        foreach (var sa in _specificationAttributeService.GetSpecificationAttributes())
                        {
                            availableSpecificationAttributes.Add(new SelectListItem
                            {
                                Text = sa.Name, 
                                Value = sa.Id.ToString()
                            });
                        }
                        return availableSpecificationAttributes;
                    });
                
                //options of preselected specification attribute
                if (model.AddSpecificationAttributeModel.AvailableAttributes.Any())
                {
                    var selectedAttributeId = int.Parse(model.AddSpecificationAttributeModel.AvailableAttributes.First().Value);
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(selectedAttributeId))
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem
                        {
                            Text = sao.Name,
                            Value = sao.Id.ToString()
                        });
                }
                //default specs values
                model.AddSpecificationAttributeModel.ShowOnPlanPage = true;
            }


            //copy plan
            if (plan != null)
            {
                model.CopyPlanModel.Id = plan.Id;
                model.CopyPlanModel.Name = "Copy of " + plan.Name;
                model.CopyPlanModel.Published = true;
                model.CopyPlanModel.CopyImages = true;
            }

            //templates
           

            //vendors
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            model.AvailableVendors.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Plans.Fields.Vendor.None"),
                Value = "0"
            });
            var vendors = _vendorService.GetAllVendors(showHidden: true);
            foreach (var vendor in vendors)
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = vendor.Name,
                    Value = vendor.Id.ToString()
                });
            }

            //delivery dates
            model.AvailableDeliveryDates.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Plans.Fields.DeliveryDate.None"),
                Value = "0"
            });
            var deliveryDates = _shippingService.GetAllDeliveryDates();
            foreach (var deliveryDate in deliveryDates)
            {
                model.AvailableDeliveryDates.Add(new SelectListItem
                {
                    Text = deliveryDate.Name,
                    Value = deliveryDate.Id.ToString()
                });
            }

            //warehouses
            var warehouses = _shippingService.GetAllWarehouses();
            model.AvailableWarehouses.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Plans.Fields.Warehouse.None"),
                Value = "0"
            });
            foreach (var warehouse in warehouses)
            {
                model.AvailableWarehouses.Add(new SelectListItem
                {
                    Text = warehouse.Name,
                    Value = warehouse.Id.ToString()
                });
            }

            

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = plan != null && !setPredefinedValues && tc.Id == plan.TaxCategoryId });

            //baseprice units
            var measureWeights = _measureService.GetAllMeasureWeights();
            foreach (var mw in measureWeights)
                model.AvailableBasepriceUnits.Add(new SelectListItem { Text = mw.Name, Value = mw.Id.ToString(), Selected = plan != null && !setPredefinedValues && mw.Id == plan.BasepriceUnitId });
            foreach (var mw in measureWeights)
                model.AvailableBasepriceBaseUnits.Add(new SelectListItem { Text = mw.Name, Value = mw.Id.ToString(), Selected = plan != null && !setPredefinedValues && mw.Id == plan.BasepriceBaseUnitId });

            //discounts
            model.AvailableDiscounts = _discountService
                .GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true)
                .Select(d => d.ToModel())
                .ToList();
            if (!excludeProperties && plan != null)
            {
                model.SelectedDiscountIds = plan.AppliedDiscounts.Select(d => d.Id).ToArray();
            }

            //default values
            if (setPredefinedValues)
            {
                model.MaximumCustomerEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.RecurringCycleLength = 100;
                model.RecurringTotalCycles = 10;
                model.RentalPriceLength = 1;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.SubscriptionMinimumQuantity = 1;
                model.SubscriptionOrderMaximumQuantity = 10000;

                model.UnlimitedDownloads = true;
                model.IsShipEnabled = true;
                model.AllowCustomerReviews = true;
                model.Published = true;
                model.VisibleIndividually = true;
            }
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _membershipCategoryService.GetAllCategoriesByParentMembershipCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

     

        #endregion

        #region Methods

        #region Plan list / create / edit / delete

        //list plans
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var model = new PlanListModel();
            //a vendor should have access only to his plans
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableMembershipCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _membershipCategoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableMembershipCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var wh in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //plan types
            model.AvailablePlanTypes = PlanType.SimplePlan.ToSelectList(false).ToList();
            model.AvailablePlanTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0"});

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Plans.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Plans.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Plans.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public ActionResult PlanList(DataSourceRequest command, PlanListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var plans = _planService.SearchPlans(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                planType: model.SearchPlanTypeId > 0 ? (PlanType?)model.SearchPlanTypeId : null,
                keywords: model.SearchPlanName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );
            var gridModel = new DataSourceResult();
            gridModel.Data = plans.Select(x =>
            {
                var planModel = x.ToModel();
                //little hack here:
                //ensure that plan full descriptions are not returned
                //otherwise, we can get the following error if plans have too long descriptions:
                //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
                //also it improves performance
                planModel.FullDescription = "";

                //picture
                //var defaultPlanPicture = _pictureService.GetPicturesByPlanId(x.Id, 1).FirstOrDefault();
                //planModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultPlanPicture, 75, true);
                //plan type
                planModel.PlanTypeName = x.PlanType.GetLocalizedEnum(_localizationService, _workContext);
                //friendly stock qantity
                //if a simple plan AND "manage inventory" is "Track inventory", then display
               // if (x.PlanType == PlanType.SimplePlan && x.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                    //planModel.StockQuantityStr = x.GetTotalStockQuantity().ToString();
                return planModel;
            });
            gridModel.Total = plans.TotalCount;

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-plan-by-sku")]
        public ActionResult GoToSku(PlanListModel model)
        {
            string sku = model.GoDirectlyToSku;

            //try to load a plan entity
            var plan = _planService.GetPlanBySku(sku);

            //if not found, then try to load a plan attribute combination
            if (plan == null)
            {
                
            }

            if (plan != null)
                return RedirectToAction("Edit", "Plan", new { id = plan.Id });
            
            //not found
            return List();
        }

        //create plan
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var model = new PlanModel();
            PreparePlanModel(model, null, true, true);
            AddLocales(_languageService, model.Locales);
            PrepareAclModel(model, null, false);
            PrepareStoresMappingModel(model, null, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(PlanModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his plans
                if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage)
                {
                    model.ShowOnHomePage = false;
                }

                //plan
                var plan = model.ToEntity();
                plan.CreatedOnUtc = DateTime.UtcNow;
                plan.UpdatedOnUtc = DateTime.UtcNow;
                _planService.InsertPlan(plan);
                //search engine name
                model.SeName = plan.ValidateSeName(model.SeName, plan.Name, true);
                _urlRecordService.SaveSlug(plan, model.SeName, 0);
                //locales
                UpdateLocales(plan, model);
                //ACL (customer roles)
                SavePlanAcl(plan, model);
                //Stores
                SaveStoreMappings(plan, model);
                //tags
              //  SavePlanTags(plan, ParsePlanTags(model.PlanTags));
                //warehouses
              //  SavePlanWarehouseInventory(plan, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        plan.AppliedDiscounts.Add(discount);
                }
                _planService.UpdatePlan(plan);
                _planService.UpdateHasDiscountsApplied(plan);

                //activity log
                _customerActivityService.InsertActivity("AddNewPlan", _localizationService.GetResource("ActivityLog.AddNewPlan"), plan.Name);
                
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Plans.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = plan.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PreparePlanModel(model, null, false, true);
            PrepareAclModel(model, null, true);
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        //edit plan
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plan = _planService.GetPlanById(id);
            if (plan == null || plan.Deleted)
                //No plan found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var model = plan.ToModel();
            PreparePlanModel(model, plan, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
                {
                    locale.Name = plan.GetLocalized(x => x.Name, languageId, false, false);
                    locale.ShortDescription = plan.GetLocalized(x => x.ShortDescription, languageId, false, false);
                    locale.FullDescription = plan.GetLocalized(x => x.FullDescription, languageId, false, false);
                    locale.MetaKeywords = plan.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = plan.GetLocalized(x => x.MetaDescription, languageId, false, false);
                    locale.MetaTitle = plan.GetLocalized(x => x.MetaTitle, languageId, false, false);
                    locale.SeName = plan.GetSeName(languageId, false, false);
                });

            PrepareAclModel(model, plan, false);
            PrepareStoresMappingModel(model, plan, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(PlanModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plan = _planService.GetPlanById(model.Id);
            if (plan == null || plan.Deleted)
                //No plan found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his plans
                if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage != plan.ShowOnHomePage)
                {
                    model.ShowOnHomePage = plan.ShowOnHomePage;
                }
                //some previously used values
                
                int prevDownloadId = plan.DownloadId;
                int prevSampleDownloadId = plan.SampleDownloadId;

                //plan
                plan = model.ToEntity(plan);
                plan.IsShipEnabled = true;
                plan.UpdatedOnUtc = DateTime.UtcNow;
                _planService.UpdatePlan(plan);
                //search engine name
                model.SeName = plan.ValidateSeName(model.SeName, plan.Name, true);
                _urlRecordService.SaveSlug(plan, model.SeName, 0);
                //locales
                UpdateLocales(plan, model);
                //tags
                //SavePlanTags(plan, ParsePlanTags(model.PlanTags));
                //warehouses
              //  SavePlanWarehouseInventory(plan, model);
                //ACL (customer roles)
                SavePlanAcl(plan, model);
                //Stores
                SaveStoreMappings(plan, model);
                //picture seo names
                UpdatePictureSeoNames(plan);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (plan.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            plan.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //remove discount
                        if (plan.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                            plan.AppliedDiscounts.Remove(discount);
                    }
                }
                _planService.UpdatePlan(plan);
                _planService.UpdateHasDiscountsApplied(plan);
                //back in stock notifications
                
                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != plan.DownloadId)
                {
                    var prevDownload = _downloadService.GetDownloadById(prevDownloadId);
                    if (prevDownload != null)
                        _downloadService.DeleteDownload(prevDownload);
                }
                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != plan.SampleDownloadId)
                {
                    var prevSampleDownload = _downloadService.GetDownloadById(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        _downloadService.DeleteDownload(prevSampleDownload);
                }

                //activity log
                _customerActivityService.InsertActivity("EditPlan", _localizationService.GetResource("ActivityLog.EditPlan"), plan.Name);
                
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Plans.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new {id = plan.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PreparePlanModel(model, plan, false, true);
            PrepareAclModel(model, plan, true);
            PrepareStoresMappingModel(model, plan, true);
            return View(model);
        }

        //delete plan
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plan = _planService.GetPlanById(id);
            if (plan == null)
                //No plan found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            _planService.DeletePlan(plan);

            //activity log
            _customerActivityService.InsertActivity("DeletePlan", _localizationService.GetResource("ActivityLog.DeletePlan"), plan.Name);
                
            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Plans.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plans = new List<Plan>();
            if (selectedIds != null)
            {
                plans.AddRange(_planService.GetPlansByIds(selectedIds.ToArray()));

                for (int i = 0; i < plans.Count; i++)
                {
                    var plan = plans[i];

                    //a vendor should have access only to his plans
                    if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                        continue;

                    _planService.DeletePlan(plan);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult CopyPlan(PlanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var copyModel = model.CopyPlanModel;
            try
            {
                var originalPlan = _planService.GetPlanById(copyModel.Id);

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null && originalPlan.VendorId != _workContext.CurrentVendor.Id)
                    return RedirectToAction("List");

                var newProduct = _copyPlanService.CopyPlan(originalPlan,
                    copyModel.Name, copyModel.Published, copyModel.CopyImages,true);
                SuccessNotification("The product has been copied successfully");
                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }
        
        #endregion

        #region Required plans

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LoadPlanFriendlyNames(string planIds)
        {
            var result = "";

            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return Json(new { Text = result });

            if (!String.IsNullOrWhiteSpace(planIds))
            {
                var ids = new List<int>();
                var rangeArray = planIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                foreach (string str1 in rangeArray)
                {
                    int tmp1;
                    if (int.TryParse(str1, out tmp1))
                        ids.Add(tmp1);
                }

                var plans = _planService.GetPlansByIds(ids.ToArray());
                for (int i = 0; i <= plans.Count - 1; i++)
                {
                    result += plans[i].Name;
                    if (i != plans.Count - 1)
                        result += ", ";
                }
            }

            return Json(new { Text = result });
        }

        public ActionResult RequiredPlanAddPopup(string btnId, string planIdsInput)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var model = new PlanModel.AddRequiredPlanModel();
            //a vendor should have access only to his plans
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableMembershipCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _membershipCategoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableMembershipCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //plan types
            model.AvailablePlanTypes = PlanType.SimplePlan.ToSelectList(false).ToList();
            model.AvailablePlanTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });


            ViewBag.planIdsInput = planIdsInput;
            ViewBag.btnId = btnId;

            return View(model);
        }

        [HttpPost]
        public ActionResult RequiredPlanAddPopupList(DataSourceRequest command, PlanModel.AddRequiredPlanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var plans = _planService.SearchPlans(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                planType: model.SearchPlanTypeId > 0 ? (PlanType?)model.SearchPlanTypeId : null,
                keywords: model.SearchPlanName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult();
            gridModel.Data = plans.Select(x => x.ToModel());
            gridModel.Total = plans.TotalCount;

            return Json(gridModel);
        }

        #endregion
        
        #region Plan MembershipCategories

        [HttpPost]
        public ActionResult PlanMembershipCategoryList(DataSourceRequest command, int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null&& plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            var planCategories = _membershipCategoryService.GetPlanCategoriesByPlanId(planId, true);
            var planCategoriesModel = planCategories
                .Select(x => new PlanModel.PlanMembershipCategoryModel
                {
                    Id = x.Id,
                    MembershipCategory = _membershipCategoryService.GetMembershipCategoryById(x.MembershipCategoryId).GetFormattedBreadCrumb(_membershipCategoryService),
                    PlanId = x.PlanId,
                    MembershipCategoryId = x.MembershipCategoryId,
                    IsFeaturedPlan = x.IsFeaturedPlan,
                    DisplayOrder  = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = planCategoriesModel,
                Total = planCategoriesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PlanMembershipCategoryInsert(PlanModel.PlanMembershipCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planId = model.PlanId;
            var categoryId = model.MembershipCategoryId;

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            var existingPlanCategories = _membershipCategoryService.GetPlanCategoriesByMembershipCategoryId(categoryId, showHidden: true);
            if (existingPlanCategories.FindPlanMembershipCategory(planId, categoryId) == null)
            {
                var planCategory = new PlanMembershipCategory
                {
                    PlanId = planId,
                    MembershipCategoryId = categoryId,
                    DisplayOrder = model.DisplayOrder
                };
                //a vendor cannot edit "IsFeaturedPlan" property
                if (_workContext.CurrentVendor == null)
                {
                    planCategory.IsFeaturedPlan = model.IsFeaturedPlan;
                }
                _membershipCategoryService.InsertPlanMembershipCategory(planCategory);
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PlanMembershipCategoryUpdate(PlanModel.PlanMembershipCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planCategory = _membershipCategoryService.GetPlanMembershipCategoryById(model.Id);
            if (planCategory == null)
                throw new ArgumentException("No plan category mapping found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planCategory.PlanId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            planCategory.MembershipCategoryId = model.MembershipCategoryId;
            planCategory.DisplayOrder = model.DisplayOrder;
            //a vendor cannot edit "IsFeaturedPlan" property
            if (_workContext.CurrentVendor == null)
            {
                planCategory.IsFeaturedPlan = model.IsFeaturedPlan;
            }
            _membershipCategoryService.UpdatePlanMembershipCategory(planCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PlanMembershipCategoryDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planCategory = _membershipCategoryService.GetPlanMembershipCategoryById(id);
            if (planCategory == null)
                throw new ArgumentException("No plan category mapping found with the specified id");

            var planId = planCategory.PlanId;

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            _membershipCategoryService.DeletePlanMembershipCategory(planCategory);

            return new NullJsonResult();
        }

        #endregion

        #region Plan categories

        [HttpPost]
        public ActionResult PlanCategoryList(DataSourceRequest command, int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            var planCategories = _categoryService.GetPlanCategoriesByPlanId(planId, true);
            var planCategoriesModel = planCategories
                .Select(x => new PlanModel.PlanCategoryModel
                {
                    Id = x.Id,
                    Category = _categoryService.GetCategoryById(x.CategoryId).GetFormattedBreadCrumb(_categoryService),
                    PlanId = x.PlanId,
                    CategoryId = x.CategoryId,
                    Quantity = x.Quantity,
                    MyToyBoxQuantity = x.MyToyBoxQuantity,
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = planCategoriesModel,
                Total = planCategoriesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PlanCategoryInsert(PlanModel.PlanCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planId = model.PlanId;
            var categoryId = model.CategoryId;

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            var existingPlanCategories = _categoryService.GetPlanCategoriesByCategoryId(categoryId, showHidden: true);
            if (existingPlanCategories.FindPlanCategory(planId, categoryId) == null)
            {
                var planCategory = new PlanCategory
                {
                    PlanId = planId,
                    CategoryId = categoryId,
                    DisplayOrder = model.DisplayOrder
                };

                if (_workContext.CurrentVendor == null)
                {
                    planCategory.Quantity = model.Quantity;
                    planCategory.MyToyBoxQuantity = model.MyToyBoxQuantity;
                }

                _categoryService.InsertPlanCategory(planCategory);
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PlanCategoryUpdate(PlanModel.PlanCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planCategory = _categoryService.GetPlanCategoryById(model.Id);
            if (planCategory == null)
                throw new ArgumentException("No plan category mapping found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planCategory.PlanId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            planCategory.CategoryId = model.CategoryId;
            planCategory.DisplayOrder = model.DisplayOrder;
            //a vendor cannot edit "IsFeaturedPlan" property
            if (_workContext.CurrentVendor == null)
            {
                planCategory.Quantity = model.Quantity;
                planCategory.MyToyBoxQuantity = model.MyToyBoxQuantity;
            }
            _categoryService.UpdatePlanCategory(planCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PlanCategoryDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planCategory = _categoryService.GetPlanCategoryById(id);
            if (planCategory == null)
                throw new ArgumentException("No plan category mapping found with the specified id");

            var planId = planCategory.PlanId;

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            _categoryService.DeletePlanCategory(planCategory);

            return new NullJsonResult();
        }

        #endregion

        #region Plan pictures

        [ValidateInput(false)]
        public ActionResult PlanPictureAdd(int pictureId, int displaySubscriptionOrder, 
            string overrideAltAttribute, string overrideTitleAttribute,
            int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var plan = _planService.GetPlanById(planId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");
            
            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");
            
            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _planService.InsertPlanPicture(new PlanPicture
            {
                PictureId = pictureId,
                PlanId = planId,
                DisplayOrder = displaySubscriptionOrder,
            });

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename, 
                overrideAltAttribute, 
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(plan.Name));

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PlanPictureList(DataSourceRequest command, int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            var planPictures = _planService.GetPlanPicturesByPlanId(planId);
            var planPicturesModel = planPictures
                .Select(x =>
                        {
                            var picture = _pictureService.GetPictureById(x.PictureId);
                            if (picture == null)
                                throw new Exception("Picture cannot be loaded");
                            var m = new PlanModel.PlanPictureModel
                                    {
                                        Id = x.Id,
                                        PlanId = x.PlanId,
                                        PictureId = x.PictureId,
                                        PictureUrl = _pictureService.GetPictureUrl(picture),
                                        OverrideAltAttribute = picture.AltAttribute,
                                        OverrideTitleAttribute = picture.TitleAttribute,
                                        DisplayOrder = x.DisplayOrder
                                    };
                            return m;
                        })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = planPicturesModel,
                Total = planPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PlanPictureUpdate(PlanModel.PlanPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planPicture = _planService.GetPlanPictureById(model.Id);
            if (planPicture == null)
                throw new ArgumentException("No plan picture found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planPicture.PlanId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }

            planPicture.DisplayOrder = model.DisplayOrder;
            _planService.UpdatePlanPicture(planPicture);

            var picture = _pictureService.GetPictureById(planPicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute, 
                model.OverrideTitleAttribute);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PlanPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var planPicture = _planService.GetPlanPictureById(id);
            if (planPicture == null)
                throw new ArgumentException("No plan picture found with the specified id");

            var planId = planPicture.PlanId;

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null)
            {
                var plan = _planService.GetPlanById(planId);
                if (plan != null && plan.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your plan");
                }
            }
            var pictureId = planPicture.PictureId;
            _planService.DeletePlanPicture(planPicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Purchased with order

        [HttpPost]
        public ActionResult PurchasedWithSubscriptionOrders(DataSourceRequest command, int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plan = _planService.GetPlanById(planId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your plan");

            var orders = _subscriptionService.SearchSubscriptionOrders(
                planId: planId,
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new SubscriptionOrderModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        SubscriptionOrderStatus = x.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        CustomerEmail = x.BillingAddress.Email,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = orders.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

         

        #region Low stock reports

        public ActionResult LowStockReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            return View();
        }
        

        #endregion

        #region Tier prices

        [HttpPost]
        public ActionResult TierPriceList(DataSourceRequest command, int planId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plan = _planService.GetPlanById(planId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your plan");

            var tierPricesModel = plan.TierPrices
                .OrderBy(x => x.StoreId)
                .ThenBy(x => x.Quantity)
                .ThenBy(x => x.CustomerRoleId)
                .Select(x =>
                {
                    string storeName;
                    if (x.StoreId > 0)
                    {
                        var store = _storeService.GetStoreById(x.StoreId);
                        storeName = store != null ? store.Name : "Deleted";
                    }
                    else
                    {
                        storeName = _localizationService.GetResource("Admin.Catalog.Plans.TierPrices.Fields.Store.All");
                    }
                    return new PlanModel.TierPriceModel
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        Store = storeName,
                        CustomerRole = x.CustomerRoleId.HasValue ? _customerService.GetCustomerRoleById(x.CustomerRoleId.Value).Name : _localizationService.GetResource("Admin.Catalog.Plans.TierPrices.Fields.CustomerRole.All"),
                        PlanId = x.PlanId,
                        CustomerRoleId = x.CustomerRoleId.HasValue ? x.CustomerRoleId.Value : 0,
                        Quantity = x.Quantity,
                        Price = x.Price
                    };
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = tierPricesModel,
                Total = tierPricesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult TierPriceInsert(PlanModel.TierPriceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var plan = _planService.GetPlanById(model.PlanId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your plan");

            var tierPrice = new TierPrice
            {
                PlanId = model.PlanId,
                StoreId = model.StoreId,
                CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null,
                Quantity = model.Quantity,
                Price = model.Price
            };
            _planService.InsertTierPrice(tierPrice);

            //update "HasTierPrices" property
            _planService.UpdateHasTierPricesProperty(plan);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TierPriceUpdate(PlanModel.TierPriceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var tierPrice = _planService.GetTierPriceById(model.Id);
            if (tierPrice == null)
                throw new ArgumentException("No tier price found with the specified id");

            var plan = _planService.GetPlanById(tierPrice.PlanId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your plan");

            tierPrice.StoreId = model.StoreId;
            tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?) null;
            tierPrice.Quantity = model.Quantity;
            tierPrice.Price = model.Price;
            _planService.UpdateTierPrice(tierPrice);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TierPriceDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlans))
                return AccessDeniedView();

            var tierPrice = _planService.GetTierPriceById(id);
            if (tierPrice == null)
                throw new ArgumentException("No tier price found with the specified id");

            var planId = tierPrice.PlanId;
            var plan = _planService.GetPlanById(planId);
            if (plan == null)
                throw new ArgumentException("No plan found with the specified id");

            //a vendor should have access only to his plans
            if (_workContext.CurrentVendor != null && plan.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your plan");

            _planService.DeleteTierPrice(tierPrice);

            //update "HasTierPrices" property
            _planService.UpdateHasTierPricesProperty(plan);

            return new NullJsonResult();
        }

        #endregion
         

        #endregion
    }
}
