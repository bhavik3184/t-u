using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
		#region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ILocalityService _localityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

	    #endregion

		#region Constructors

        public CountryController(ICountryService countryService, 
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ILocalityService localityService, 
            ILocalizationService localizationService, 
            IWorkContext workContext,
            ICacheManager cacheManager)
		{
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._localityService = localityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
		}

        #endregion

        #region States / provinces

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            string cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();


                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some country is selected
                    if (result.Count == 0)
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                        }
                    }
                }

                return result;
            });
            
            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCitiesByStateProvinceId(string stateProvinceId, bool addSelectStateItem)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(stateProvinceId))
                throw new ArgumentNullException("stateProvinceId");

            string cacheKey = string.Format(ModelCacheEventConsumer.CITIES_BY_COUNTRY_MODEL_KEY, stateProvinceId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateProvinceId));
                var states = _cityService.GetCitysByStateProvinceId(country != null ? country.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();


                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some country is selected
                    if (result.Count == 0)
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                        }
                    }
                }

                return result;
            });

            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalitiesByCityId(string cityId, bool addSelectStateItem)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(cityId))
                throw new ArgumentNullException("cityId");

            string cacheKey = string.Format(ModelCacheEventConsumer.LOCALITIES_BY_COUNTRY_MODEL_KEY, cityId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _cityService.GetCityById(Convert.ToInt32(cityId));
                var states = _localityService.GetLocalitysByCityId(country != null ? country.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();


                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some country is selected
                    if (result.Count == 0)
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                        }
                    }
                }

                return result;
            });

            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
