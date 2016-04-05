using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// MembershipCategory service
    /// </summary>
    public partial class MembershipCategoryService : IMembershipCategoryService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : membershipMembershipCategory ID
        /// </remarks>
        private const string CATEGORIES_BY_ID_KEY = "Nop.membershipMembershipCategory.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent membershipMembershipCategory ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// {3} : include all levels (child)
        /// </remarks>
        private const string CATEGORIES_BY_PARENT_CATEGORY_ID_KEY = "Nop.membershipMembershipCategory.byparent-{0}-{1}-{2}-{3}-{4}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : membershipMembershipCategory ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY = "Nop.productmembershipMembershipCategory.allbymembershipMembershipCategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : product ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY = "Nop.productmembershipMembershipCategory.allbyproductid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CATEGORIES_PATTERN_KEY = "Nop.membershipMembershipCategory.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTCATEGORIES_PATTERN_KEY = "Nop.productmembershipMembershipCategory.";

        #endregion

        #region Fields

        private readonly IRepository<MembershipCategory> _membershipMembershipCategoryRepository;
        private readonly IRepository<PlanMembershipCategory> _planMembershipCategoryRepository;
        private readonly IRepository<Plan> _productRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly CatalogSettings _catalogSettings;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="membershipMembershipCategoryRepository">MembershipCategory repository</param>
        /// <param name="planMembershipCategoryRepository">PlanMembershipCategory repository</param>
        /// <param name="productRepository">Plan repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public MembershipCategoryService(ICacheManager cacheManager,
            IRepository<MembershipCategory> membershipMembershipCategoryRepository,
            IRepository<PlanMembershipCategory> planMembershipCategoryRepository,
            IRepository<Plan> productRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            CatalogSettings catalogSettings)
        {
            this._cacheManager = cacheManager;
            this._membershipMembershipCategoryRepository = membershipMembershipCategoryRepository;
            this._planMembershipCategoryRepository = planMembershipCategoryRepository;
            this._productRepository = productRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete membershipMembershipCategory
        /// </summary>
        /// <param name="membershipMembershipCategory">MembershipCategory</param>
        public virtual void DeleteMembershipCategory(MembershipCategory membershipMembershipCategory)
        {
            if (membershipMembershipCategory == null)
                throw new ArgumentNullException("membershipMembershipCategory");

            membershipMembershipCategory.Deleted = true;
            UpdateMembershipCategory(membershipMembershipCategory);

            //reset a "Parent membershipMembershipCategory" property of all child subcategories
            var subcategories = GetAllCategoriesByParentMembershipCategoryId(membershipMembershipCategory.Id, true);
            foreach (var submembershipMembershipCategory in subcategories)
            {
                submembershipMembershipCategory.ParentMembershipCategoryId = 0;
                UpdateMembershipCategory(submembershipMembershipCategory);
            }
        }
        
        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="membershipMembershipCategoryName">MembershipCategory name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<MembershipCategory> GetAllCategories(string membershipMembershipCategoryName = "", 
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _membershipMembershipCategoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!String.IsNullOrWhiteSpace(membershipMembershipCategoryName))
                query = query.Where(c => c.Name.Contains(membershipMembershipCategoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentMembershipCategoryId).ThenBy(c => c.DisplayOrder);
            
            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from c in query
                            join acl in _aclRepository.Table
                            on new { c1 = c.Id, c2 = "MembershipCategory" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }
                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = c.Id, c2 = "MembershipCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || currentStoreId == sm.StoreId
                            select c;
                }

                //only distinct categories (group by ID)
                query = from c in query
                        group c by c.Id
                        into cGroup
                        orderby cGroup.Key
                        select cGroup.FirstOrDefault();
                query = query.OrderBy(c => c.ParentMembershipCategoryId).ThenBy(c => c.DisplayOrder);
            }
            
            var unsortedCategories = query.ToList();

            //sort categories
            var sortedCategories = unsortedCategories.SortCategoriesForTree();

            //paging
            return new PagedList<MembershipCategory>(sortedCategories, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all categories filtered by parent membershipMembershipCategory identifier
        /// </summary>
        /// <param name="parentMembershipCategoryId">Parent membershipMembershipCategory identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>Categories</returns>
        public virtual IList<MembershipCategory> GetAllCategoriesByParentMembershipCategoryId(int parentMembershipCategoryId,
            bool showHidden = false, bool includeAllLevels = false)
        {
            string key = string.Format(CATEGORIES_BY_PARENT_CATEGORY_ID_KEY, parentMembershipCategoryId, showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, includeAllLevels);
            return _cacheManager.Get(key, () =>
            {
                var query = _membershipMembershipCategoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentMembershipCategoryId == parentMembershipCategoryId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder);

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "MembershipCategory" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "MembershipCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select c;
                    }
                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.DisplayOrder);
                }

                var categories = query.ToList();
                if (includeAllLevels)
                {
                    var childCategories = new List<MembershipCategory>();
                    //add child levels
                    foreach (var membershipMembershipCategory in categories)
                    {
                        childCategories.AddRange(GetAllCategoriesByParentMembershipCategoryId(membershipMembershipCategory.Id, showHidden, includeAllLevels));
                    }
                    categories.AddRange(childCategories);
                }
                return categories;
            });
        }
        
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<MembershipCategory> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false)
        {
            var query = from c in _membershipMembershipCategoryRepository.Table
                        orderby c.DisplayOrder
                        where c.Published &&
                        !c.Deleted && 
                        c.ShowOnHomePage
                        select c;

            var categories = query.ToList();
            if (!showHidden)
            {
                categories = categories
                    .Where(c => _aclService.Authorize(c) && _storeMappingService.Authorize(c))
                    .ToList();
            }

            return categories;
        }
                
        /// <summary>
        /// Gets a membershipMembershipCategory
        /// </summary>
        /// <param name="membershipMembershipCategoryId">MembershipCategory identifier</param>
        /// <returns>MembershipCategory</returns>
        public virtual MembershipCategory GetMembershipCategoryById(int membershipMembershipCategoryId)
        {
            if (membershipMembershipCategoryId == 0)
                return null;
            
            string key = string.Format(CATEGORIES_BY_ID_KEY, membershipMembershipCategoryId);
            return _cacheManager.Get(key, () => _membershipMembershipCategoryRepository.GetById(membershipMembershipCategoryId));
        }

        /// <summary>
        /// Inserts membershipMembershipCategory
        /// </summary>
        /// <param name="membershipMembershipCategory">MembershipCategory</param>
        public virtual void InsertMembershipCategory(MembershipCategory membershipMembershipCategory)
        {
            if (membershipMembershipCategory == null)
                throw new ArgumentNullException("membershipMembershipCategory");

            _membershipMembershipCategoryRepository.Insert(membershipMembershipCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(membershipMembershipCategory);
        }

        /// <summary>
        /// Updates the membershipMembershipCategory
        /// </summary>
        /// <param name="membershipMembershipCategory">MembershipCategory</param>
        public virtual void UpdateMembershipCategory(MembershipCategory membershipMembershipCategory)
        {
            if (membershipMembershipCategory == null)
                throw new ArgumentNullException("membershipMembershipCategory");

            //validate membershipMembershipCategory hierarchy
            var parentMembershipCategory = GetMembershipCategoryById(membershipMembershipCategory.ParentMembershipCategoryId);
            while (parentMembershipCategory != null)
            {
                if (membershipMembershipCategory.Id == parentMembershipCategory.Id)
                {
                    membershipMembershipCategory.ParentMembershipCategoryId = 0;
                    break;
                }
                parentMembershipCategory = GetMembershipCategoryById(parentMembershipCategory.ParentMembershipCategoryId);
            }

            _membershipMembershipCategoryRepository.Update(membershipMembershipCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(membershipMembershipCategory);
        }
        
        
        /// <summary>
        /// Deletes a product membershipMembershipCategory mapping
        /// </summary>
        /// <param name="planMembershipCategory">Plan membershipMembershipCategory</param>
        public virtual void DeletePlanMembershipCategory(PlanMembershipCategory planMembershipCategory)
        {
            if (planMembershipCategory == null)
                throw new ArgumentNullException("planMembershipCategory");

            _planMembershipCategoryRepository.Delete(planMembershipCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(planMembershipCategory);
        }

        /// <summary>
        /// Gets product membershipMembershipCategory mapping collection
        /// </summary>
        /// <param name="membershipMembershipCategoryId">MembershipCategory identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Plan a membershipMembershipCategory mapping collection</returns>
        public virtual IPagedList<PlanMembershipCategory> GetPlanCategoriesByMembershipCategoryId(int membershipMembershipCategoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (membershipMembershipCategoryId == 0)
                return new PagedList<PlanMembershipCategory>(new List<PlanMembershipCategory>(), pageIndex, pageSize);

            string key = string.Format(PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY, showHidden, membershipMembershipCategoryId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _planMembershipCategoryRepository.Table
                            join p in _productRepository.Table on pc.PlanId equals p.Id
                            where pc.MembershipCategoryId == membershipMembershipCategoryId &&
                                  !p.Deleted &&
                                  (showHidden || p.Published)
                            orderby pc.DisplayOrder
                            select pc;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pc in query
                                join c in _membershipMembershipCategoryRepository.Table on pc.MembershipCategoryId equals c.Id
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "MembershipCategory" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pc;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pc in query
                                join c in _membershipMembershipCategoryRepository.Table on pc.MembershipCategoryId equals c.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "MembershipCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select pc;
                    }
                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(pc => pc.DisplayOrder);
                }

                var productCategories = new PagedList<PlanMembershipCategory>(query, pageIndex, pageSize);
                return productCategories;
            });
        }

        /// <summary>
        /// Gets a product membershipMembershipCategory mapping collection
        /// </summary>
        /// <param name="productId">Plan identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Plan membershipMembershipCategory mapping collection</returns>
        public virtual IList<PlanMembershipCategory> GetPlanCategoriesByPlanId(int productId, bool showHidden = false)
        {
            return GetPlanCategoriesByPlanId(productId, _storeContext.CurrentStore.Id, showHidden);
        }
        /// <summary>
        /// Gets a product membershipMembershipCategory mapping collection
        /// </summary>
        /// <param name="productId">Plan identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Plan membershipMembershipCategory mapping collection</returns>
        public virtual IList<PlanMembershipCategory> GetPlanCategoriesByPlanId(int productId, int storeId, bool showHidden = false)
        {
            if (productId == 0)
                return new List<PlanMembershipCategory>();

            string key = string.Format(PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY, showHidden, productId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _planMembershipCategoryRepository.Table
                            join c in _membershipMembershipCategoryRepository.Table on pc.MembershipCategoryId equals c.Id
                            where pc.PlanId == productId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.DisplayOrder
                            select pc;

                var allPlanCategories = query.ToList();
                var result = new List<PlanMembershipCategory>();
                if (!showHidden)
                {
                    foreach (var pc in allPlanCategories)
                    {
                        //ACL (access control list) and store mapping
                        var membershipMembershipCategory = pc.MembershipCategory;
                        if (_aclService.Authorize(membershipMembershipCategory) && _storeMappingService.Authorize(membershipMembershipCategory, storeId))
                            result.Add(pc);
                    }
                }
                else
                {
                    //no filtering
                    result.AddRange(allPlanCategories);
                }
                return result;
            });
        }

        /// <summary>
        /// Gets a product membershipMembershipCategory mapping 
        /// </summary>
        /// <param name="planMembershipCategoryId">Plan membershipMembershipCategory mapping identifier</param>
        /// <returns>Plan membershipMembershipCategory mapping</returns>
        public virtual PlanMembershipCategory GetPlanMembershipCategoryById(int planMembershipCategoryId)
        {
            if (planMembershipCategoryId == 0)
                return null;

            return _planMembershipCategoryRepository.GetById(planMembershipCategoryId);
        }

        /// <summary>
        /// Inserts a product membershipMembershipCategory mapping
        /// </summary>
        /// <param name="planMembershipCategory">>Plan membershipMembershipCategory mapping</param>
        public virtual void InsertPlanMembershipCategory(PlanMembershipCategory planMembershipCategory)
        {
            if (planMembershipCategory == null)
                throw new ArgumentNullException("planMembershipCategory");
            
            _planMembershipCategoryRepository.Insert(planMembershipCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(planMembershipCategory);
        }

        /// <summary>
        /// Updates the product membershipMembershipCategory mapping 
        /// </summary>
        /// <param name="planMembershipCategory">>Plan membershipMembershipCategory mapping</param>
        public virtual void UpdatePlanMembershipCategory(PlanMembershipCategory planMembershipCategory)
        {
            if (planMembershipCategory == null)
                throw new ArgumentNullException("planMembershipCategory");

            _planMembershipCategoryRepository.Update(planMembershipCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(planMembershipCategory);
        }

        #endregion
    }
}
