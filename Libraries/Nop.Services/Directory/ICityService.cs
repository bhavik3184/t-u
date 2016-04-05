using System.Collections.Generic;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service interface
    /// </summary>
    public partial interface ICityService
    {
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="City">The state/province</param>
        void DeleteCity(City City);

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="CityId">The state/province identifier</param>
        /// <returns>State/province</returns>
        City GetCityById(int CityId);

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        City GetCityByAbbreviation(string abbreviation);
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">StateProvince identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<City> GetCitysByStateProvinceId(int countryId, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<City> GetCitys(bool showHidden = false);

        IList<City> GetCitysByIds(int[] cityIds);
        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="City">State/province</param>
        void InsertCity(City City);

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="City">State/province</param>
        void UpdateCity(City City);
    }
}
