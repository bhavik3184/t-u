using System.Collections.Generic;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service interface
    /// </summary>
    public partial interface ILocalityService
    {
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="Locality">The state/province</param>
        void DeleteLocality(Locality Locality);

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="LocalityId">The state/province identifier</param>
        /// <returns>State/province</returns>
        Locality GetLocalityById(int LocalityId);

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        Locality GetLocalityByAbbreviation(string abbreviation);
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">City identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<Locality> GetLocalitysByCityId(int countryId, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<Locality> GetLocalitys(bool showHidden = false);

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="Locality">State/province</param>
        void InsertLocality(Locality Locality);

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="Locality">State/province</param>
        void UpdateLocality(Locality Locality);
    }
}
