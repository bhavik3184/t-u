using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service
    /// </summary>
    public partial class CityService : ICityService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : language ID
        /// {2} : show hidden records?
        /// </remarks>
        private const string STATEPROVINCES_ALL_KEY = "Nop.city.all-{0}-{1}-{2}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STATEPROVINCES_PATTERN_KEY = "Nop.city.";

        #endregion

        #region Fields

        private readonly IRepository<City> _CityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="CityRepository">State/province repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CityService(ICacheManager cacheManager,
            IRepository<City> CityRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _CityRepository = CityRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="City">The state/province</param>
        public virtual void DeleteCity(City City)
        {
            if (City == null)
                throw new ArgumentNullException("City");
            
            _CityRepository.Delete(City);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(City);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="CityId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public virtual City GetCityById(int CityId)
        {
            if (CityId == 0)
                return null;

            return _CityRepository.GetById(CityId);
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public virtual City GetCityByAbbreviation(string abbreviation)
        {
            var query = from sp in _CityRepository.Table
                        where sp.Abbreviation == abbreviation
                        select sp;
            var City = query.FirstOrDefault();
            return City;
        }
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">StateProvince identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<City> GetCitysByStateProvinceId(int countryId, int languageId = 0, bool showHidden = false)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId, languageId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _CityRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.StateProvinceId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var Citys = query.ToList();

                if (languageId > 0)
                {
                    //we should sort states by localized names when they have the same display order
                    Citys = Citys
                        .OrderBy(c => c.DisplayOrder)
                        .ThenBy(c => c.GetLocalized(x => x.Name, languageId))
                        .ToList();
                }
                return Citys;
            });
        }

        public virtual IList<City> GetCitysByIds(int[] cityIds)
        {
            if (cityIds == null || cityIds.Length == 0)
                return new List<City>();

            var query = from c in _CityRepository.Table
                        where cityIds.Contains(c.Id)
                        select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<City>();
            foreach (int id in cityIds)
            {
                var country = countries.Find(x => x.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }
        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<City> GetCitys(bool showHidden = false)
        {
            var query = from sp in _CityRepository.Table
                        orderby sp.StateProvinceId, sp.DisplayOrder, sp.Name
                where showHidden || sp.Published
                select sp;
            var Citys = query.ToList();
            return Citys;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="City">State/province</param>
        public virtual void InsertCity(City City)
        {
            if (City == null)
                throw new ArgumentNullException("City");

            _CityRepository.Insert(City);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(City);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="City">State/province</param>
        public virtual void UpdateCity(City City)
        {
            if (City == null)
                throw new ArgumentNullException("City");

            _CityRepository.Update(City);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(City);
        }

        #endregion
    }
}
