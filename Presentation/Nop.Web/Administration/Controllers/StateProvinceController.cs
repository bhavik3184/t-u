using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Directory;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class StateProvinceController : BaseAdminController
	{
		#region Fields

        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ILocalizationService _localizationService;
	    private readonly IAddressService _addressService;
        private readonly IPermissionService _permissionService;
	    private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;

	    #endregion

		#region Constructors

        public StateProvinceController(IStateProvinceService stateProvinceService,
            ICityService cityService, 
            ILocalizationService localizationService,
            IAddressService addressService, 
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService, 
            ILanguageService languageService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager,
            IImportManager importManager)
		{
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._localizationService = localizationService;
            this._addressService = addressService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._exportManager = exportManager;
            this._importManager = importManager;
		}

		#endregion 

        #region Utilities
        
        [NonAction]
        protected virtual void UpdateLocales(StateProvince stateProvince, StateProvinceModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(stateProvince,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(City city, CityModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(city,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

       

        #endregion

        #region Countries

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult StateProvinceList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var countries = _stateProvinceService.GetStateProvinces(showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = countries.Select(x => x.ToModel()),
                Total = countries.Count
            };

            return Json(gridModel);
        }
        
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var model = new StateProvinceModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            //default values
            model.Published = true;
          
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(StateProvinceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var stateProvince = model.ToEntity();
                _stateProvinceService.InsertStateProvince(stateProvince);
                //locales
                UpdateLocales(stateProvince, model);
                //Stores

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = stateProvince.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var stateProvince = _stateProvinceService.GetStateProvinceById(id);
            if (stateProvince == null)
                //No stateProvince found with the specified id
                return RedirectToAction("List");

            var model = stateProvince.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = stateProvince.GetLocalized(x => x.Name, languageId, false, false);
            });
            //Stores
          
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(StateProvinceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var stateProvince = _stateProvinceService.GetStateProvinceById(model.Id);
            if (stateProvince == null)
                //No stateProvince found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                stateProvince = model.ToEntity(stateProvince);
                _stateProvinceService.UpdateStateProvince(stateProvince);
                //locales
                UpdateLocales(stateProvince, model);
                //Stores
              

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new {id = stateProvince.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            //PrepareStoresMappingModel(model, stateProvince, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var stateProvince = _stateProvinceService.GetStateProvinceById(id);
            if (stateProvince == null)
                //No stateProvince found with the specified id
                return RedirectToAction("List");

            try
            {
                if (_addressService.GetAddressTotalByStateProvinceId(stateProvince.Id) > 0)
                    throw new NopException("The stateProvince can't be deleted. It has associated addresses");

                _stateProvinceService.DeleteStateProvince(stateProvince);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = stateProvince.Id });
            }
        }

        [HttpPost]
        public ActionResult PublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var countries = _stateProvinceService.GetStateProvincesByIds(selectedIds.ToArray());
                foreach (var stateProvince in countries)
                {
                    stateProvince.Published = true;
                    _stateProvinceService.UpdateStateProvince(stateProvince);
                }
            }

            return Json(new { Result = true });
        }
        [HttpPost]
        public ActionResult UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var countries = _stateProvinceService.GetStateProvincesByIds(selectedIds.ToArray());
                foreach (var stateProvince in countries)
                {
                    stateProvince.Published = false;
                    _stateProvinceService.UpdateStateProvince(stateProvince);
                }
            }
            return Json(new { Result = true });
        }

        #endregion

        #region States / provinces

        [HttpPost]
        public ActionResult Cities(int stateProvinceId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var states = _cityService.GetCitysByStateProvinceId(stateProvinceId, showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = states.Select(x => x.ToModel()),
                Total = states.Count
            };
            return Json(gridModel);
        }

        //create
        public ActionResult CityCreatePopup(int stateProvinceId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var model = new CityModel();
            model.StateProvinceId = stateProvinceId;
            //default value
            model.Published = true;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult CityCreatePopup(string btnId, string formId, CityModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var stateProvince = _stateProvinceService.GetStateProvinceById(model.StateProvinceId);
            if (stateProvince == null)
                //No stateProvince found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity();

                _cityService.InsertCity(sp);
                UpdateLocales(sp, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult CityEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var sp = _cityService.GetCityById(id);
            if (sp == null)
                //No state found with the specified id
                return RedirectToAction("List");

            var model = sp.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = sp.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult CityEditPopup(string btnId, string formId, CityModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var sp = _cityService.GetCityById(model.Id);
            if (sp == null)
                //No state found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                sp = model.ToEntity(sp);
                _cityService.UpdateCity(sp);

                UpdateLocales(sp, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult CityDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var state = _cityService.GetCityById(id);
            if (state == null)
                throw new ArgumentException("No state found with the specified id");

            if (_addressService.GetAddressTotalByCityId(state.Id) > 0)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Countries.States.CantDeleteWithAddresses") });
            }

            //int stateProvinceId = state.StateProvinceId;
            _cityService.DeleteCity(state);

            return new NullJsonResult();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCitiesByStateProvinceId(string stateProvinceId,
            bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here


            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(stateProvinceId))
                throw new ArgumentNullException("stateProvinceId");

            var stateProvince = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateProvinceId));
            var states = stateProvince != null ? _cityService.GetCitysByStateProvinceId(stateProvince.Id, showHidden: true).ToList() : new List<City>();
            var result = (from s in states
                         select new { id = s.Id, name = s.Name }).ToList();
            if (addAsterisk.HasValue && addAsterisk.Value)
            {
                //asterisk
                result.Insert(0, new { id = 0, name = "*" });
            }
            else
            {
                if (stateProvince == null)
                {
                    //stateProvince is not selected ("choose stateProvince" item)
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some stateProvince is selected
                    if (result.Count == 0)
                    {
                        //stateProvince does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.OtherNonUS") });
                    }
                    else
                    {
                        //stateProvince has some states
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectState") });
                        }
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export / import

        public ActionResult ExportCsv()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            string fileName = String.Format("states_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

            var states = _cityService.GetCitys(true);
            string result = _exportManager.ExportCitiesToTxt(states);

            return File(Encoding.UTF8.GetBytes(result), "text/csv", fileName);
        }

        [HttpPost]
        public ActionResult ImportCsv(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importcsvfile"];
                if (file != null && file.ContentLength > 0)
                {
                    int count = _importManager.ImportCitiesFromTxt(file.InputStream);
                    SuccessNotification(String.Format(_localizationService.GetResource("Admin.Configuration.Countries.ImportSuccess"), count));
                    return RedirectToAction("List");
                }
                ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}
