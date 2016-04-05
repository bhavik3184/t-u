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
    public partial class LocalityService : ILocalityService
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
        private const string STATEPROVINCES_ALL_KEY = "Nop.locality.all-{0}-{1}-{2}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STATEPROVINCES_PATTERN_KEY = "Nop.locality.";

        #endregion

        #region Fields

        private readonly IRepository<Locality> _LocalityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="LocalityRepository">State/province repository</param>
        /// <param name="eventPublisher">Event published</param>
        public LocalityService(ICacheManager cacheManager,
            IRepository<Locality> LocalityRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _LocalityRepository = LocalityRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="Locality">The state/province</param>
        public virtual void DeleteLocality(Locality Locality)
        {
            if (Locality == null)
                throw new ArgumentNullException("Locality");
            
            _LocalityRepository.Delete(Locality);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(Locality);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="LocalityId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public virtual Locality GetLocalityById(int LocalityId)
        {
            if (LocalityId == 0)
                return null;

            return _LocalityRepository.GetById(LocalityId);
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public virtual Locality GetLocalityByAbbreviation(string abbreviation)
        {
            var query = from sp in _LocalityRepository.Table
                        where sp.Abbreviation == abbreviation
                        select sp;
            var Locality = query.FirstOrDefault();
            return Locality;
        }
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">City identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<Locality> GetLocalitysByCityId(int countryId, int languageId = 0, bool showHidden = false)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId, languageId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _LocalityRepository.Table
                            orderby sp.Name
                            where sp.CityId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var Localitys = query.ToList();

               
                return Localitys;
            });
        }

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<Locality> GetLocalitys(bool showHidden = false)
        {
            var query = from sp in _LocalityRepository.Table
                        orderby   sp.Name
                where showHidden || sp.Published
                select sp;
            var Localitys = query.ToList();
            return Localitys;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="Locality">State/province</param>
        public virtual void InsertLocality(Locality Locality)
        {
            if (Locality == null)
                throw new ArgumentNullException("Locality");

            _LocalityRepository.Insert(Locality);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(Locality);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="Locality">State/province</param>
        public virtual void UpdateLocality(Locality Locality)
        {
            if (Locality == null)
                throw new ArgumentNullException("Locality");

            _LocalityRepository.Update(Locality);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(Locality);
        }

        #endregion
    }
}
