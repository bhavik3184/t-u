using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Children service
    /// </summary>
    public partial class ChildrenService : IChildrenService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : children ID
        /// </remarks>
        private const string MANUFACTURERS_BY_ID_KEY = "Nop.children.id-{0}";
      
        private const string MANUFACTURERS_PATTERN_KEY = "Nop.children.";
       

        #endregion

        #region Fields

        private readonly IRepository<Children> _childrenRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="childrenRepository">Category repository</param>
        /// <param name="productChildrenRepository">ProductCategory repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public ChildrenService(ICacheManager cacheManager,
            IRepository<Children> childrenRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._childrenRepository = childrenRepository;
            this._eventPublisher = eventPublisher;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Deletes a children
        /// </summary>
        /// <param name="children">Children</param>
        public virtual void DeleteChildren(Children children)
        {
            if (children == null)
                throw new ArgumentNullException("children");


            _childrenRepository.Delete(children);
        }

        /// <summary>
        /// Gets all childrens
        /// </summary>
        /// <param name="childrenName">Children name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Childrens</returns>
        public virtual IPagedList<Children> GetAllChildrens(string childrenName = "",
            int pageIndex = 0,
            int pageSize = int.MaxValue, 
            bool showHidden = false)
        {
            var query = _childrenRepository.Table;
            
            if (!String.IsNullOrWhiteSpace(childrenName))
                query = query.Where(m => m.Name.Contains(childrenName));
           
            query = query.OrderBy(m => m.DateOfBirth);


            return new PagedList<Children>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a children
        /// </summary>
        /// <param name="childrenId">Children identifier</param>
        /// <returns>Children</returns>
        public virtual Children GetChildrenById(int childrenId)
        {
            if (childrenId == 0)
                return null;
            
            string key = string.Format(MANUFACTURERS_BY_ID_KEY, childrenId);
            return _cacheManager.Get(key, () => _childrenRepository.GetById(childrenId));
        }

        /// <summary>
        /// Inserts a children
        /// </summary>
        /// <param name="children">Children</param>
        public virtual void InsertChildren(Children children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            _childrenRepository.Insert(children);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
          

            //event notification
            _eventPublisher.EntityInserted(children);
        }

        /// <summary>
        /// Updates the children
        /// </summary>
        /// <param name="children">Children</param>
        public virtual void UpdateChildren(Children children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            _childrenRepository.Update(children);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
         

            //event notification
            _eventPublisher.EntityUpdated(children);
        }
        

     

        #endregion
    }
}
