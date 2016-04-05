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
    public partial class CityController : BaseAdminController
	{
		#region Fields

        private readonly ICityService _cityService;
        private readonly ILocalityService _localityService;
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

        public CityController(ICityService cityService,
            ILocalityService localityService, 
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
            this._cityService = cityService;
            this._localityService = localityService;
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

        [NonAction]
        protected virtual void UpdateLocales(Locality locality, LocalityModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(locality,
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
        public ActionResult CityList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var countries = _cityService.GetCitys(showHidden: true);
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

            var model = new CityModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            //default values
            model.Published = true;
          
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(CityModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var city = model.ToEntity();
                _cityService.InsertCity(city);
                //locales
                UpdateLocales(city, model);
                //Stores

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = city.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var city = _cityService.GetCityById(id);
            if (city == null)
                //No city found with the specified id
                return RedirectToAction("List");

            var model = city.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = city.GetLocalized(x => x.Name, languageId, false, false);
            });
            //Stores
          
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(CityModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var city = _cityService.GetCityById(model.Id);
            if (city == null)
                //No city found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                city = model.ToEntity(city);
                _cityService.UpdateCity(city);
                //locales
                UpdateLocales(city, model);
                //Stores
              

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new {id = city.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            //PrepareStoresMappingModel(model, city, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var city = _cityService.GetCityById(id);
            if (city == null)
                //No city found with the specified id
                return RedirectToAction("List");

            try
            {
                if (_addressService.GetAddressTotalByCityId(city.Id) > 0)
                    throw new NopException("The city can't be deleted. It has associated addresses");

                _cityService.DeleteCity(city);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = city.Id });
            }
        }

        [HttpPost]
        public ActionResult PublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var countries = _cityService.GetCitysByIds(selectedIds.ToArray());
                foreach (var city in countries)
                {
                    city.Published = true;
                    _cityService.UpdateCity(city);
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
                var countries = _cityService.GetCitysByIds(selectedIds.ToArray());
                foreach (var city in countries)
                {
                    city.Published = false;
                    _cityService.UpdateCity(city);
                }
            }
            return Json(new { Result = true });
        }

        #endregion

        #region States / provinces

        [HttpPost]
        public ActionResult Cities(int cityId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var states = _localityService.GetLocalitysByCityId(cityId, showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = states.Select(x => x.ToModel()),
                Total = states.Count
            };
            return Json(gridModel);
        }

        //create
        public ActionResult LocalityCreatePopup(int cityId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var model = new LocalityModel();
            model.CityId = cityId;
            //default value
            model.Published = true;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult LocalityCreatePopup(string btnId, string formId, LocalityModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var city = _cityService.GetCityById(model.CityId);
            if (city == null)
                //No city found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity();

                _localityService.InsertLocality(sp);
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
        public ActionResult LocalityEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var sp = _localityService.GetLocalityById(id);
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
        public ActionResult LocalityEditPopup(string btnId, string formId, LocalityModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var sp = _localityService.GetLocalityById(model.Id);
            if (sp == null)
                //No state found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                sp = model.ToEntity(sp);
                _localityService.UpdateLocality(sp);

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
        public ActionResult LocalityDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var state = _localityService.GetLocalityById(id);
            if (state == null)
                throw new ArgumentException("No state found with the specified id");

            if (_addressService.GetAddressTotalByLocalityId(state.Id) > 0)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Countries.States.CantDeleteWithAddresses") });
            }

            //int cityId = state.CityId;
            _localityService.DeleteLocality(state);

            return new NullJsonResult();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCitiesByCityId(string cityId,
            bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here


            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(cityId))
                throw new ArgumentNullException("cityId");

            var city = _cityService.GetCityById(Convert.ToInt32(cityId));
            var states = city != null ? _localityService.GetLocalitysByCityId(city.Id, showHidden: true).ToList() : new List<Locality>();
            var result = (from s in states
                         select new { id = s.Id, name = s.Name }).ToList();
            if (addAsterisk.HasValue && addAsterisk.Value)
            {
                //asterisk
                result.Insert(0, new { id = 0, name = "*" });
            }
            else
            {
                if (city == null)
                {
                    //city is not selected ("choose city" item)
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
                    //some city is selected
                    if (result.Count == 0)
                    {
                        //city does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.OtherNonUS") });
                    }
                    else
                    {
                        //city has some states
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

        [HttpPost]
        public ActionResult ImportLocalityCsv(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importcsvfile"];
                if (file != null && file.ContentLength > 0)
                {
                    int count = _importManager.ImportLocalitiesFromTxt(file.InputStream);
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
