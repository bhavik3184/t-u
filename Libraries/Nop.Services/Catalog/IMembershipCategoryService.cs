using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// MembershipCategory service interface
    /// </summary>
    public partial interface IMembershipCategoryService
    {
        /// <summary>
        /// Delete membershipCategory
        /// </summary>
        /// <param name="membershipCategory">MembershipCategory</param>
        void DeleteMembershipCategory(MembershipCategory membershipCategory);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="membershipCategoryName">MembershipCategory name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<MembershipCategory> GetAllCategories(string membershipCategoryName = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent membershipCategory identifier
        /// </summary>
        /// <param name="parentMembershipCategoryId">Parent membershipCategory identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>Categories</returns>
        IList<MembershipCategory> GetAllCategoriesByParentMembershipCategoryId(int parentMembershipCategoryId,
            bool showHidden = false, bool includeAllLevels = false);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<MembershipCategory> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false);
                
        /// <summary>
        /// Gets a membershipCategory
        /// </summary>
        /// <param name="membershipCategoryId">MembershipCategory identifier</param>
        /// <returns>MembershipCategory</returns>
        MembershipCategory GetMembershipCategoryById(int membershipCategoryId);

        /// <summary>
        /// Inserts membershipCategory
        /// </summary>
        /// <param name="membershipCategory">MembershipCategory</param>
        void InsertMembershipCategory(MembershipCategory membershipCategory);

        /// <summary>
        /// Updates the membershipCategory
        /// </summary>
        /// <param name="membershipCategory">MembershipCategory</param>
        void UpdateMembershipCategory(MembershipCategory membershipCategory);
        
        /// <summary>
        /// Deletes a product membershipCategory mapping
        /// </summary>
        /// <param name="planMembershipCategory">Plan membershipCategory</param>
        void DeletePlanMembershipCategory(PlanMembershipCategory planMembershipCategory);

        /// <summary>
        /// Gets product membershipCategory mapping collection
        /// </summary>
        /// <param name="membershipCategoryId">MembershipCategory identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Plan a membershipCategory mapping collection</returns>
        IPagedList<PlanMembershipCategory> GetPlanCategoriesByMembershipCategoryId(int membershipCategoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a product membershipCategory mapping collection
        /// </summary>
        /// <param name="productId">Plan identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Plan membershipCategory mapping collection</returns>
        IList<PlanMembershipCategory> GetPlanCategoriesByPlanId(int productId, bool showHidden = false);
        /// <summary>
        /// Gets a product membershipCategory mapping collection
        /// </summary>
        /// <param name="productId">Plan identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Plan membershipCategory mapping collection</returns>
        IList<PlanMembershipCategory> GetPlanCategoriesByPlanId(int productId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a product membershipCategory mapping 
        /// </summary>
        /// <param name="planMembershipCategoryId">Plan membershipCategory mapping identifier</param>
        /// <returns>Plan membershipCategory mapping</returns>
        PlanMembershipCategory GetPlanMembershipCategoryById(int planMembershipCategoryId);

        /// <summary>
        /// Inserts a product membershipCategory mapping
        /// </summary>
        /// <param name="planMembershipCategory">>Plan membershipCategory mapping</param>
        void InsertPlanMembershipCategory(PlanMembershipCategory planMembershipCategory);

        /// <summary>
        /// Updates the product membershipCategory mapping 
        /// </summary>
        /// <param name="planMembershipCategory">>Plan membershipCategory mapping</param>
        void UpdatePlanMembershipCategory(PlanMembershipCategory planMembershipCategory);
    }
}
