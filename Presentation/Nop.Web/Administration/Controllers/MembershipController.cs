using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class MembershipCategoryController : BaseAdminController
    {
        #region Fields

        private readonly IMembershipCategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPlanService _productService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IDiscountService _discountService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly CatalogSettings _catalogSettings;

        #endregion
        
        #region Constructors

        public MembershipCategoryController(IMembershipCategoryService categoryService, 
            IManufacturerService manufacturerService, IPlanService productService, 
            ICustomerService customerService,
            IUrlRecordService urlRecordService, 
            IPictureService pictureService, 
            ILanguageService languageService,
            ILocalizationService localizationService, 
            ILocalizedEntityService localizedEntityService,
            IDiscountService discountService,
            IPermissionService permissionService,
            IAclService aclService, 
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager, 
            IVendorService vendorService, 
            ICustomerActivityService customerActivityService,
            CatalogSettings catalogSettings)
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._discountService = discountService;
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._catalogSettings = catalogSettings;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(MembershipCategory category, MembershipCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = category.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(MembershipCategory category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }

        [NonAction]
        protected virtual void PrepareAllCategoriesModel(MembershipCategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCategories.Add(new SelectListItem
            {
                Text = "[None]",
                Value = "0"
            });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories),
                    Value = c.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void PrepareTemplatesModel(MembershipCategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

          
        }

        [NonAction]
        protected virtual void PrepareDiscountModel(MembershipCategoryModel model, MembershipCategory category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableDiscounts = _discountService
                .GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true)
                .Select(d => d.ToModel())
                .ToList();

            if (!excludeProperties && category != null)
            {
                model.SelectedDiscountIds = category.AppliedDiscounts.Select(d => d.Id).ToArray();
            }
        }

        [NonAction]
        protected virtual void PrepareAclModel(MembershipCategoryModel model, MembershipCategory category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCustomerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (category != null)
                {
                    model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(category);
                }
            }
        }

        [NonAction]
        protected virtual void SaveMembershipCategoryAcl(MembershipCategory category, MembershipCategoryModel model)
        {
            var existingAclRecords = _aclService.GetAclRecords(category);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(category, customerRole.Id);
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
        protected virtual void PrepareStoresMappingModel(MembershipCategoryModel model, MembershipCategory category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableStores = _storeService
                .GetAllStores()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (category != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(category);
                }
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(MembershipCategory category, MembershipCategoryModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(category);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(category, store.Id);
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

        #endregion
        
        #region List / tree

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new MembershipCategoryListModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, MembershipCategoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var categories = _categoryService.GetAllCategories(model.SearchMembershipCategoryName, 
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetFormattedBreadCrumb(_categoryService);
                    return categoryModel;
                }),
                Total = categories.TotalCount
            };
            return Json(gridModel);
        }
        
        public ActionResult Tree()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            return View();
        }

        [HttpPost,]
        public ActionResult TreeLoadChildren(int id = 0)
        {
            var categories = _categoryService.GetAllCategoriesByParentMembershipCategoryId(id, true)
                .Select(x => new
                             {
                                 id = x.Id,
                                 Name = x.Name,
                                 hasChildren = _categoryService.GetAllCategoriesByParentMembershipCategoryId(x.Id, true).Count > 0,
                                 imageUrl = Url.Content("~/Administration/Content/images/ico-content.png")
                             });

            return Json(categories);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new MembershipCategoryModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.PageSize = _catalogSettings.DefaultCategoryPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            model.Published = true;
            model.IncludeInTopMenu = true;
            model.AllowCustomersToSelectPageSize = true;            

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(MembershipCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                _categoryService.InsertMembershipCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        category.AppliedDiscounts.Add(discount);
                }
                _categoryService.UpdateMembershipCategory(category);
                //update picture seo file name
                UpdatePictureSeoNames(category);
                //ACL (customer roles)
                SaveMembershipCategoryAcl(category, model);
                //Stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewMembershipCategory", _localizationService.GetResource("ActivityLog.AddNewMembershipCategory"), category.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            //ACL
            PrepareAclModel(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetMembershipCategoryById(id);
            if (category == null || category.Deleted) 
                //No category found with the specified id
                return RedirectToAction("List");

            var model = category.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = category.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = category.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = category.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = category.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = category.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = category.GetSeName(languageId, false, false);
            });
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, category, false);
            //ACL
            PrepareAclModel(model, category, false);
            //Stores
            PrepareStoresMappingModel(model, category, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(MembershipCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetMembershipCategoryById(model.Id);
            if (category == null || category.Deleted)
                //No category found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = category.PictureId;
                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                _categoryService.UpdateMembershipCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            category.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //remove discount
                        if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                            category.AppliedDiscounts.Remove(discount);
                    }
                }
                _categoryService.UpdateMembershipCategory(category);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(category);
                //ACL
                SaveMembershipCategoryAcl(category, model);
                //Stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("EditMembershipCategory", _localizationService.GetResource("ActivityLog.EditMembershipCategory"), category.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new {id = category.Id});
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, category, true);
            //ACL
            PrepareAclModel(model, category, true);
            //Stores
            PrepareStoresMappingModel(model, category, true);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetMembershipCategoryById(id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("List");

            _categoryService.DeleteMembershipCategory(category);

            //activity log
            _customerActivityService.InsertActivity("DeleteMembershipCategory", _localizationService.GetResource("ActivityLog.DeleteMembershipCategory"), category.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));
            return RedirectToAction("List");
        }
        

        #endregion

         

        #region Plans

        [HttpPost]
        public ActionResult PlanList(DataSourceRequest command, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productCategories = _categoryService.GetPlanCategoriesByMembershipCategoryId(categoryId,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = productCategories.Select(x => new MembershipCategoryModel.MembershipCategoryPlanModel
                {
                    Id = x.Id,
                    MembershipCategoryId = x.MembershipCategoryId,
                    PlanId = x.PlanId,
                    PlanName = _productService.GetPlanById(x.PlanId).Name,
                    IsFeaturedPlan = x.IsFeaturedPlan,
                    DisplayOrder = x.DisplayOrder
                }),
                Total = productCategories.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult PlanUpdate(MembershipCategoryModel.MembershipCategoryPlanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productMembershipCategory = _categoryService.GetPlanMembershipCategoryById(model.Id);
            if (productMembershipCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            productMembershipCategory.IsFeaturedPlan = model.IsFeaturedPlan;
            productMembershipCategory.DisplayOrder = model.DisplayOrder;
            _categoryService.UpdatePlanMembershipCategory(productMembershipCategory);

            return new NullJsonResult();
        }

        public ActionResult PlanDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productMembershipCategory = _categoryService.GetPlanMembershipCategoryById(id);
            if (productMembershipCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            //var categoryId = productMembershipCategory.MembershipCategoryId;
            _categoryService.DeletePlanMembershipCategory(productMembershipCategory);

            return new NullJsonResult();
        }

        public ActionResult PlanAddPopup(int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            
            var model = new MembershipCategoryModel.AddMembershipCategoryPlanModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

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

            //product types
            model.AvailablePlanTypes = PlanType.SimplePlan.ToSelectList(false).ToList();
            model.AvailablePlanTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.MembershipCategoryId = categoryId;
            return View(model);
        }

        [HttpPost]
        public ActionResult PlanAddPopupList(DataSourceRequest command, MembershipCategoryModel.AddMembershipCategoryPlanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var gridModel = new DataSourceResult();
            var products = _productService.SearchPlans(
                categoryIds: new List<int> { model.SearchMembershipCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                planType: model.SearchPlanTypeId > 0 ? (PlanType?)model.SearchPlanTypeId : null,
                keywords: model.SearchPlanName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }
        
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult PlanAddPopup(string btnId, string formId, MembershipCategoryModel.AddMembershipCategoryPlanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (model.SelectedPlanIds != null)
            {
                foreach (int id in model.SelectedPlanIds)
                {
                    var product = _productService.GetPlanById(id);
                    if (product != null)
                    {
                        var existingPlanCategories = _categoryService.GetPlanCategoriesByMembershipCategoryId(model.MembershipCategoryId, showHidden: true);
                        if (existingPlanCategories.FindPlanMembershipCategory(id, model.MembershipCategoryId) == null)
                        {
                            _categoryService.InsertPlanMembershipCategory(
                                new PlanMembershipCategory
                                {
                                    MembershipCategoryId = model.MembershipCategoryId,
                                    PlanId = id,
                                    IsFeaturedPlan = false,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion
    }
}
