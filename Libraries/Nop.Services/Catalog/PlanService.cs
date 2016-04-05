using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Plan service
    /// </summary>
    public partial class PlanService : IPlanService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        #endregion

        #region Fields

        private readonly IRepository<Plan> _productRepository;
        private readonly IRepository<TierPrice> _tierPriceRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<PlanPicture> _productPictureRepository;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productRepository">Plan repository</param>
        /// <param name="relatedPlanRepository">Related product repository</param>
        /// <param name="crossSellPlanRepository">Cross-sell product repository</param>
        /// <param name="tierPriceRepository">Tier price repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="productPictureRepository">Plan picture repository</param>
        /// <param name="productSpecificationAttributeRepository">Plan specification attribute repository</param>
        /// <param name="productReviewRepository">Plan review repository</param>
        /// <param name="productWarehouseInventoryRepository">Plan warehouse inventory repository</param>
        /// <param name="productAttributeService">Plan attribute service</param>
        /// <param name="productAttributeParser">Plan attribute parser service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="workContext">Work context</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        public PlanService(ICacheManager cacheManager,
            IRepository<Plan> productRepository,
            IRepository<TierPrice> tierPriceRepository,
            IRepository<PlanPicture> productPictureRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider, 
            IDbContext dbContext,
            IWorkContext workContext,
            LocalizationSettings localizationSettings, 
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IAclService aclService,
            IStoreMappingService storeMappingService)
        {
            this._cacheManager = cacheManager;
            this._productRepository = productRepository;
            this._tierPriceRepository = tierPriceRepository;
            this._productPictureRepository = productPictureRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._languageService = languageService;
            this._workflowMessageService = workflowMessageService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._localizationSettings = localizationSettings;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
        }

        #endregion
        
        #region Methods

        #region Plans

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Plan</param>
        public virtual void DeletePlan(Plan product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            product.Deleted = true;
            //delete product
            UpdatePlan(product);
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Plans</returns>
        public virtual IList<Plan> GetAllPlansDisplayedOnHomePage()
        {
            var query = from p in _productRepository.Table
                        orderby p.DisplayOrder, p.Name
                        where p.Published &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        select p;
            var products = query.ToList();
            return products;
        }
        
        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Plan identifier</param>
        /// <returns>Plan</returns>
        public virtual Plan GetPlanById(int productId)
        {
            if (productId == 0)
                return null;
            
            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            return _cacheManager.Get(key, () => _productRepository.GetById(productId));
        }

        /// <summary>
        /// Get products by identifiers
        /// </summary>
        /// <param name="productIds">Plan identifiers</param>
        /// <returns>Plans</returns>
        public virtual IList<Plan> GetPlansByIds(int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<Plan>();

            var query = from p in _productRepository.Table
                        where productIds.Contains(p.Id)
                        select p;
            var products = query.ToList();
            //sort by passed identifiers
            var sortedPlans = new List<Plan>();
            foreach (int id in productIds)
            {
                var product = products.Find(x => x.Id == id);
                if (product != null)
                    sortedPlans.Add(product);
            }
            return sortedPlans;
        }

        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Plan</param>
        public virtual void InsertPlan(Plan product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //insert
            _productRepository.Insert(product);

            //clear cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            
            //event notification
            _eventPublisher.EntityInserted(product);
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Plan</param>
        public virtual void UpdatePlan(Plan product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //update
            _productRepository.Update(product);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(product);
        }

        /// <summary>
        /// Get (visible) product number in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Plan number</returns>
        public virtual int GetCategoryPlanNumber(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted && p.Published && p.VisibleIndividually);

            //category filtering
            if (categoryIds != null && categoryIds.Count > 0)
            {
                query = from p in query
                        from pc in p.PlanMembershipCategories.Where(pc => categoryIds.Contains(pc.MembershipCategoryId))
                        select p;
            }

            if (!_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Plan" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Plan" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            //only distinct products
            var result = query.Select(p => p.Id).Distinct().Count();
            return result;
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Plan type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredPlans">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Plan tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchPlanTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">SubscriptionOrder by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Plans</returns>
        public virtual IPagedList<Plan> SearchPlans(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            PlanType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredPlans = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchPlanTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            IList<int> filterableSpecificationAttributeOptionIds;
            return SearchPlans(out filterableSpecificationAttributeOptionIds, false,
                pageIndex, pageSize, categoryIds, manufacturerId,
                storeId, vendorId, warehouseId,
                productType, visibleIndividuallyOnly, markedAsNewOnly, featuredPlans,
                priceMin, priceMax, productTagId, keywords, searchDescriptions, searchSku,
                searchPlanTags, languageId, filteredSpecs, 
                orderBy, showHidden, overridePublished);
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="filterableSpecificationAttributeOptionIds">The specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Plan type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredPlans">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Plan tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchPlanTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">SubscriptionOrder by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Plans</returns>
        public virtual IPagedList<Plan> SearchPlans(
            out IList<int> filterableSpecificationAttributeOptionIds,
            bool loadFilterableSpecificationAttributeOptionIds = false,
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            PlanType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredPlans = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchPlanTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            filterableSpecificationAttributeOptionIds = new List<int>();

            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds !=null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                #region Use stored procedure
                
                //pass category identifiers as comma-delimited string
                string commaSeparatedCategoryIds = categoryIds == null ? "" : string.Join(",", categoryIds);


                //pass customer role identifiers as comma-delimited string
                string commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);


                //pass specification identifiers as comma-delimited string
                string commaSeparatedSpecIds = "";
                if (filteredSpecs != null)
                {
                    ((List<int>)filteredSpecs).Sort();
                    commaSeparatedSpecIds = string.Join(",", filteredSpecs);
                }

                //some databases don't support int.MaxValue
                if (pageSize == int.MaxValue)
                    pageSize = int.MaxValue - 1;
                
                //prepare parameters
                var pCategoryIds = _dataProvider.GetParameter();
                pCategoryIds.ParameterName = "CategoryIds";
                pCategoryIds.Value = commaSeparatedCategoryIds != null ? (object)commaSeparatedCategoryIds : DBNull.Value;
                pCategoryIds.DbType = DbType.String;
                
                var pManufacturerId = _dataProvider.GetParameter();
                pManufacturerId.ParameterName = "ManufacturerId";
                pManufacturerId.Value = manufacturerId;
                pManufacturerId.DbType = DbType.Int32;

                var pStoreId = _dataProvider.GetParameter();
                pStoreId.ParameterName = "StoreId";
                pStoreId.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
                pStoreId.DbType = DbType.Int32;

                var pVendorId = _dataProvider.GetParameter();
                pVendorId.ParameterName = "VendorId";
                pVendorId.Value = vendorId;
                pVendorId.DbType = DbType.Int32;

                var pWarehouseId = _dataProvider.GetParameter();
                pWarehouseId.ParameterName = "WarehouseId";
                pWarehouseId.Value = warehouseId;
                pWarehouseId.DbType = DbType.Int32;

                var pPlanTypeId = _dataProvider.GetParameter();
                pPlanTypeId.ParameterName = "PlanTypeId";
                pPlanTypeId.Value = productType.HasValue ? (object)productType.Value : DBNull.Value;
                pPlanTypeId.DbType = DbType.Int32;

                var pVisibleIndividuallyOnly = _dataProvider.GetParameter();
                pVisibleIndividuallyOnly.ParameterName = "VisibleIndividuallyOnly";
                pVisibleIndividuallyOnly.Value = visibleIndividuallyOnly;
                pVisibleIndividuallyOnly.DbType = DbType.Int32;

                var pMarkedAsNewOnly = _dataProvider.GetParameter();
                pMarkedAsNewOnly.ParameterName = "MarkedAsNewOnly";
                pMarkedAsNewOnly.Value = markedAsNewOnly;
                pMarkedAsNewOnly.DbType = DbType.Int32;

                var pPlanTagId = _dataProvider.GetParameter();
                pPlanTagId.ParameterName = "PlanTagId";
                pPlanTagId.Value = productTagId;
                pPlanTagId.DbType = DbType.Int32;

                var pFeaturedPlans = _dataProvider.GetParameter();
                pFeaturedPlans.ParameterName = "FeaturedPlans";
                pFeaturedPlans.Value = featuredPlans.HasValue ? (object)featuredPlans.Value : DBNull.Value;
                pFeaturedPlans.DbType = DbType.Boolean;

                var pPriceMin = _dataProvider.GetParameter();
                pPriceMin.ParameterName = "PriceMin";
                pPriceMin.Value = priceMin.HasValue ? (object)priceMin.Value : DBNull.Value;
                pPriceMin.DbType = DbType.Decimal;
                
                var pPriceMax = _dataProvider.GetParameter();
                pPriceMax.ParameterName = "PriceMax";
                pPriceMax.Value = priceMax.HasValue ? (object)priceMax.Value : DBNull.Value;
                pPriceMax.DbType = DbType.Decimal;

                var pKeywords = _dataProvider.GetParameter();
                pKeywords.ParameterName = "Keywords";
                pKeywords.Value = keywords != null ? (object)keywords : DBNull.Value;
                pKeywords.DbType = DbType.String;

                var pSearchDescriptions = _dataProvider.GetParameter();
                pSearchDescriptions.ParameterName = "SearchDescriptions";
                pSearchDescriptions.Value = searchDescriptions;
                pSearchDescriptions.DbType = DbType.Boolean;

                var pSearchSku = _dataProvider.GetParameter();
                pSearchSku.ParameterName = "SearchSku";
                pSearchSku.Value = searchSku;
                pSearchSku.DbType = DbType.Boolean;

                var pSearchPlanTags = _dataProvider.GetParameter();
                pSearchPlanTags.ParameterName = "SearchPlanTags";
                pSearchPlanTags.Value = searchPlanTags;
                pSearchPlanTags.DbType = DbType.Boolean;

                var pUseFullTextSearch = _dataProvider.GetParameter();
                pUseFullTextSearch.ParameterName = "UseFullTextSearch";
                pUseFullTextSearch.Value = _commonSettings.UseFullTextSearch;
                pUseFullTextSearch.DbType = DbType.Boolean;

                var pFullTextMode = _dataProvider.GetParameter();
                pFullTextMode.ParameterName = "FullTextMode";
                pFullTextMode.Value = (int)_commonSettings.FullTextMode;
                pFullTextMode.DbType = DbType.Int32;

                var pFilteredSpecs = _dataProvider.GetParameter();
                pFilteredSpecs.ParameterName = "FilteredSpecs";
                pFilteredSpecs.Value = commaSeparatedSpecIds != null ? (object)commaSeparatedSpecIds : DBNull.Value;
                pFilteredSpecs.DbType = DbType.String;

                var pLanguageId = _dataProvider.GetParameter();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = searchLocalizedValue ? languageId : 0;
                pLanguageId.DbType = DbType.Int32;

                var pOrderBy = _dataProvider.GetParameter();
                pOrderBy.ParameterName = "OrderBy";
                pOrderBy.Value = (int)orderBy;
                pOrderBy.DbType = DbType.Int32;

                var pAllowedCustomerRoleIds = _dataProvider.GetParameter();
                pAllowedCustomerRoleIds.ParameterName = "AllowedCustomerRoleIds";
                pAllowedCustomerRoleIds.Value = commaSeparatedAllowedCustomerRoleIds;
                pAllowedCustomerRoleIds.DbType = DbType.String;

                var pPageIndex = _dataProvider.GetParameter();
                pPageIndex.ParameterName = "PageIndex";
                pPageIndex.Value = pageIndex;
                pPageIndex.DbType = DbType.Int32;

                var pPageSize = _dataProvider.GetParameter();
                pPageSize.ParameterName = "PageSize";
                pPageSize.Value = pageSize;
                pPageSize.DbType = DbType.Int32;

                var pShowHidden = _dataProvider.GetParameter();
                pShowHidden.ParameterName = "ShowHidden";
                pShowHidden.Value = showHidden;
                pShowHidden.DbType = DbType.Boolean;

                var pOverridePublished = _dataProvider.GetParameter();
                pOverridePublished.ParameterName = "OverridePublished";
                pOverridePublished.Value = overridePublished != null ? (object)overridePublished.Value : DBNull.Value;
                pOverridePublished.DbType = DbType.Boolean;

                var pLoadFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
                pLoadFilterableSpecificationAttributeOptionIds.ParameterName = "LoadFilterableSpecificationAttributeOptionIds";
                pLoadFilterableSpecificationAttributeOptionIds.Value = loadFilterableSpecificationAttributeOptionIds;
                pLoadFilterableSpecificationAttributeOptionIds.DbType = DbType.Boolean;
                
                var pFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
                pFilterableSpecificationAttributeOptionIds.ParameterName = "FilterableSpecificationAttributeOptionIds";
                pFilterableSpecificationAttributeOptionIds.Direction = ParameterDirection.Output;
                pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
                pFilterableSpecificationAttributeOptionIds.DbType = DbType.String;

                var pTotalRecords = _dataProvider.GetParameter();
                pTotalRecords.ParameterName = "TotalRecords";
                pTotalRecords.Direction = ParameterDirection.Output;
                pTotalRecords.DbType = DbType.Int32;

                //invoke stored procedure
                var products = _dbContext.ExecuteStoredProcedureList<Plan>(
                    "PlanLoadAllPaged",
                    pCategoryIds,
                    pManufacturerId,
                    pStoreId,
                    pVendorId,
                    pWarehouseId,
                    pPlanTypeId,
                    pVisibleIndividuallyOnly,
                    pMarkedAsNewOnly,
                    pPlanTagId,
                    pFeaturedPlans,
                    pPriceMin,
                    pPriceMax,
                    pKeywords,
                    pSearchDescriptions,
                    pSearchSku,
                    pSearchPlanTags,
                    pUseFullTextSearch,
                    pFullTextMode,
                    pFilteredSpecs,
                    pLanguageId,
                    pOrderBy,
                    pAllowedCustomerRoleIds,
                    pPageIndex,
                    pPageSize,
                    pShowHidden,
                    pOverridePublished,
                    pLoadFilterableSpecificationAttributeOptionIds,
                    pFilterableSpecificationAttributeOptionIds,
                    pTotalRecords);
                //get filterable specification attribute option identifier
                string filterableSpecificationAttributeOptionIdsStr = (pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value) ? (string)pFilterableSpecificationAttributeOptionIds.Value : "";
                if (loadFilterableSpecificationAttributeOptionIds &&
                    !string.IsNullOrWhiteSpace(filterableSpecificationAttributeOptionIdsStr))
                {
                     filterableSpecificationAttributeOptionIds = filterableSpecificationAttributeOptionIdsStr
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => Convert.ToInt32(x.Trim()))
                        .ToList();
                }
                //return products
                int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                return new PagedList<Plan>(products, pageIndex, pageSize, totalRecords);

                #endregion
            }
            else
            {
                //stored procedures aren't supported. Use LINQ

                #region Search products

                //products
                var query = _productRepository.Table;
                query = query.Where(p => !p.Deleted);
                if (!overridePublished.HasValue)
                {
                    //process according to "showHidden"
                    if (!showHidden)
                    {
                        query = query.Where(p => p.Published);
                    }
                }
                else if (overridePublished.Value)
                {
                    //published only
                    query = query.Where(p => p.Published);
                }
                else if (!overridePublished.Value)
                {
                    //unpublished only
                    query = query.Where(p => !p.Published);
                }
                if (visibleIndividuallyOnly)
                {
                    query = query.Where(p => p.VisibleIndividually);
                }
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                if (markedAsNewOnly)
                {
                    query = query.Where(p => p.MarkAsNew);
                    query = query.Where(p =>
                        (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc));
                }
                if (productType.HasValue)
                {
                    var productTypeId = (int) productType.Value;
                    query = query.Where(p => p.PlanTypeId == productTypeId);
                }

                if (priceMin.HasValue)
                {
                    //min price
                    query = query.Where(p =>
                                        //special price (specified price and valid date range)
                                        ((p.SpecialPrice.HasValue &&
                                          ((!p.SpecialPriceStartDateTimeUtc.HasValue ||
                                            p.SpecialPriceStartDateTimeUtc.Value < nowUtc) &&
                                           (!p.SpecialPriceEndDateTimeUtc.HasValue ||
                                            p.SpecialPriceEndDateTimeUtc.Value > nowUtc))) &&
                                         (p.SpecialPrice >= priceMin.Value))
                                        ||
                                        //regular price (price isn't specified or date range isn't valid)
                                        ((!p.SpecialPrice.HasValue ||
                                          ((p.SpecialPriceStartDateTimeUtc.HasValue &&
                                            p.SpecialPriceStartDateTimeUtc.Value > nowUtc) ||
                                           (p.SpecialPriceEndDateTimeUtc.HasValue &&
                                            p.SpecialPriceEndDateTimeUtc.Value < nowUtc))) &&
                                         (p.Price >= priceMin.Value)));
                }
                if (priceMax.HasValue)
                {
                    //max price
                    query = query.Where(p =>
                                        //special price (specified price and valid date range)
                                        ((p.SpecialPrice.HasValue &&
                                          ((!p.SpecialPriceStartDateTimeUtc.HasValue ||
                                            p.SpecialPriceStartDateTimeUtc.Value < nowUtc) &&
                                           (!p.SpecialPriceEndDateTimeUtc.HasValue ||
                                            p.SpecialPriceEndDateTimeUtc.Value > nowUtc))) &&
                                         (p.SpecialPrice <= priceMax.Value))
                                        ||
                                        //regular price (price isn't specified or date range isn't valid)
                                        ((!p.SpecialPrice.HasValue ||
                                          ((p.SpecialPriceStartDateTimeUtc.HasValue &&
                                            p.SpecialPriceStartDateTimeUtc.Value > nowUtc) ||
                                           (p.SpecialPriceEndDateTimeUtc.HasValue &&
                                            p.SpecialPriceEndDateTimeUtc.Value < nowUtc))) &&
                                         (p.Price <= priceMax.Value)));
                }
                if (!showHidden)
                {
                    //available dates
                    query = query.Where(p =>
                        (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < nowUtc) &&
                        (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > nowUtc));
                }

                //searching by keyword
                if (!String.IsNullOrWhiteSpace(keywords))
                {
                    query = from p in query
                            join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                            from lp in p_lp.DefaultIfEmpty()
                            where (p.Name.Contains(keywords)) ||
                                  (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                                  (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                                  //localized values
                                  (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Plan" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Plan" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Plan" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                            select p;
                }

                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    query = from p in query
                            join acl in _aclRepository.Table
                            on new { c1 = p.Id, c2 = "Plan" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                            from acl in p_acl.DefaultIfEmpty()
                            where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select p;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from p in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = p.Id, c2 = "Plan" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                            from sm in p_sm.DefaultIfEmpty()
                            where !p.LimitedToStores || storeId == sm.StoreId
                            select p;
                }
                
               

                //category filtering
                if (categoryIds != null && categoryIds.Count > 0)
                {
                    query = from p in query
                            from pc in p.PlanMembershipCategories.Where(pc => categoryIds.Contains(pc.MembershipCategoryId))
                            where (!featuredPlans.HasValue || featuredPlans.Value == pc.IsFeaturedPlan)
                            select p;
                }

               

                //vendor filtering
                if (vendorId > 0)
                {
                    query = query.Where(p => p.VendorId == vendorId);
                }

                

                //related products filtering
                //if (relatedToPlanId > 0)
                //{
                //    query = from p in query
                //            join rp in _relatedPlanRepository.Table on p.Id equals rp.PlanId2
                //            where (relatedToPlanId == rp.PlanId1)
                //            select p;
                //}

                

                //only distinct products (group by ID)
                //if we use standard Distinct() method, then all fields will be compared (low performance)
                //it'll not work in SQL Server Compact when searching products by a keyword)
                query = from p in query
                        group p by p.Id
                        into pGroup
                        orderby pGroup.Key
                        select pGroup.FirstOrDefault();

                //sort products
                if (orderBy == ProductSortingEnum.Position && categoryIds != null && categoryIds.Count > 0)
                {
                    //category position
                    var firstCategoryId = categoryIds[0];
                    query = query.OrderBy(p => p.PlanMembershipCategories.FirstOrDefault(pc => pc.MembershipCategoryId == firstCategoryId).DisplayOrder);
                }
                
                else if (orderBy == ProductSortingEnum.Position)
                {
                    //otherwise sort by name
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.NameAsc)
                {
                    //Name: A to Z
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.NameDesc)
                {
                    //Name: Z to A
                    query = query.OrderByDescending(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.PriceAsc)
                {
                    //Price: Low to High
                    query = query.OrderBy(p => p.Price);
                }
                else if (orderBy == ProductSortingEnum.PriceDesc)
                {
                    //Price: High to Low
                    query = query.OrderByDescending(p => p.Price);
                }
                else if (orderBy == ProductSortingEnum.CreatedOn)
                {
                    //creation date
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                }
                else
                {
                    //actually this code is not reachable
                    query = query.OrderBy(p => p.Name);
                }

                var products = new PagedList<Plan>(query, pageIndex, pageSize);

                

                //return products
                return products;

                #endregion
            }
        }

        

     
        
        /// <summary>
        /// Gets a product by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Plan</returns>
        public virtual Plan GetPlanBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _productRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var product = query.FirstOrDefault();
            return product;
        }
        
        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="product">Plan</param>
        public virtual void UpdateHasTierPricesProperty(Plan product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            product.HasTierPrices = product.TierPrices.Count > 0;
            UpdatePlan(product);
        }

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="product">Plan</param>
        public virtual void UpdateHasDiscountsApplied(Plan product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            product.HasDiscountsApplied = product.AppliedDiscounts.Count > 0;
            UpdatePlan(product);
        }

        #endregion

        #region Inventory management methods

        /// <summary>
        /// Adjust inventory
        /// </summary>
        /// <param name="product">Plan</param>
        /// <param name="quantityToChange">Quantity to increase or descrease</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        public virtual void AdjustInventory(Plan product, int quantityToChange, string attributesXml = "")
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (quantityToChange == 0)
                return;
            
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
            {
                //previous stock
                
                 
            }

             

            //TODO send back in stock notifications?
            //also do not forget to uncomment some code above ("prevStockQuantity")
            //if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
            //    product.BackorderMode == BackorderMode.NoBackorders &&
            //    product.AllowBackInStockSubscriptions &&
            //    product.GetTotalStockQuantity() > 0 &&
            //    prevStockQuantity <= 0 &&
            //    product.Published &&
            //    !product.Deleted)
            //{
            //    //_backInStockSubscriptionOrderService.SendNotificationsToSubscribers(product);
            //}
        }

        /// <summary>
        /// Reserve the given quantity in the warehouses.
        /// </summary>
        /// <param name="product">Plan</param>
        /// <param name="quantity">Quantity, must be negative</param>
        public virtual void ReserveInventory(Plan product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", "quantity");

            var qty = -quantity;

            
            // 1st pass: Applying reserved
          

            this.UpdatePlan(product);
        }

        /// <summary>
        /// Unblocks the given quantity reserved items in the warehouses
        /// </summary>
        /// <param name="product">Plan</param>
        /// <param name="quantity">Quantity, must be positive</param>
        public virtual void UnblockReservedInventory(Plan product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (quantity < 0)
                throw new ArgumentException("Value must be positive.", "quantity");

         

            UpdatePlan(product);
        }

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="product">Plan</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        public virtual void BookReservedInventory(Plan product, int warehouseId, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", "quantity");

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                return;
            if (!product.UseMultipleWarehouses)
                return;

            //var pwi = product.PlanWarehouseInventory.FirstOrDefault(pi => pi.WarehouseId == warehouseId);
            //if (pwi == null)
            //    return;

            //pwi.ReservedQuantity = Math.Max(pwi.ReservedQuantity + quantity, 0);
            //pwi.StockQuantity += quantity;
            //UpdatePlan(product);

            //TODO add support for bundled products (AttributesXml)
        }

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>Quantity reversed</returns>
        public virtual int ReverseBookedInventory(Plan product, ShipmentItem shipmentItem)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");
            
            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                return 0;
            if (!product.UseMultipleWarehouses)
                return 0;

            //var pwi = product.PlanWarehouseInventory.FirstOrDefault(x => x.WarehouseId == shipmentItem.WarehouseId);
            //if (pwi == null)
            //    return 0;

            var shipment = shipmentItem.Shipment;

            //not shipped yet? hence "BookReservedInventory" method was not invoked
            if (!shipment.ShippedDateUtc.HasValue)
                return 0;

            var qty = shipmentItem.Quantity;

            //pwi.StockQuantity += qty;
            //pwi.ReservedQuantity += qty;
            //UpdatePlan(product);

            //TODO add support for bundled products (AttributesXml)

            return qty;
        }

        #endregion

         

         
        
        #region Tier prices
        
        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void DeleteTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRepository.Delete(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(tierPrice);
        }

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        public virtual TierPrice GetTierPriceById(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;
            
            return _tierPriceRepository.GetById(tierPriceId);
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void InsertTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRepository.Insert(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(tierPrice);
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void UpdateTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRepository.Update(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tierPrice);
        }

        #endregion

        #region Plan pictures

        /// <summary>
        /// Deletes a product picture
        /// </summary>
        /// <param name="productPicture">Plan picture</param>
        public virtual void DeletePlanPicture(PlanPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Delete(productPicture);

            //event notification
            _eventPublisher.EntityDeleted(productPicture);
        }

        /// <summary>
        /// Gets a product pictures by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Plan pictures</returns>
        public virtual IList<PlanPicture> GetPlanPicturesByPlanId(int productId)
        {
            var query = from pp in _productPictureRepository.Table
                        where pp.PlanId == productId
                        orderby pp.DisplayOrder
                        select pp;
            var productPictures = query.ToList();
            return productPictures;
        }

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Plan picture identifier</param>
        /// <returns>Plan picture</returns>
        public virtual PlanPicture GetPlanPictureById(int productPictureId)
        {
            if (productPictureId == 0)
                return null;

            return _productPictureRepository.GetById(productPictureId);
        }

        /// <summary>
        /// Inserts a product picture
        /// </summary>
        /// <param name="productPicture">Plan picture</param>
        public virtual void InsertPlanPicture(PlanPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Insert(productPicture);

            //event notification
            _eventPublisher.EntityInserted(productPicture);
        }

        /// <summary>
        /// Updates a product picture
        /// </summary>
        /// <param name="productPicture">Plan picture</param>
        public virtual void UpdatePlanPicture(PlanPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Update(productPicture);

            //event notification
            _eventPublisher.EntityUpdated(productPicture);
        }

        #endregion

         

      

        #endregion
    }
}
