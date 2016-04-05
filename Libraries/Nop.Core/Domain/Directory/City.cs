
using Nop.Core.Domain.Localization;
using System.Collections.Generic;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents a state/province
    /// </summary>
    public partial class City : BaseEntity, ILocalizedEntity
    {
        private ICollection<Locality> _localities;

        public int StateProvinceId { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public string Abbreviation { get; set; }
        

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }


        public virtual ICollection<Locality> Localities
        {
            get { return _localities ?? (_localities = new List<Locality>()); }
            protected set { _localities = value; }
        }

    }

}
