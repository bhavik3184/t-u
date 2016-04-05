using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class MembershipCategoryExtensions
    {
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public static IList<MembershipCategory> SortCategoriesForTree(this IList<MembershipCategory> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<MembershipCategory>();

            foreach (var cat in source.Where(c => c.ParentMembershipCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTree(source, cat.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                //find categories without parent in provided category source and insert them into result
                foreach (var cat in source)
                    if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                        result.Add(cat);
            }
            return result;
        }

        /// <summary>
        /// Returns a ProductMembershipCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="categoryId">MembershipCategory identifier</param>
        /// <returns>A ProductMembershipCategory that has the specified values; otherwise null</returns>
        public static PlanMembershipCategory FindPlanMembershipCategory(this IList<PlanMembershipCategory> source,
            int productId, int categoryId)
        {
            foreach (var productMembershipCategory in source)
                if (productMembershipCategory.PlanId == productId && productMembershipCategory.MembershipCategoryId == categoryId)
                    return productMembershipCategory;

            return null;
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">MembershipCategory</param>
        /// <param name="categoryService">MembershipCategory service</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public static string GetFormattedBreadCrumb(this MembershipCategory category,
            IMembershipCategoryService categoryService,
            string separator = ">>", int languageId = 0)
        {
            string result = string.Empty;

            var breadcrumb = GetMembershipCategoryBreadCrumb(category, categoryService, null, null, true);
            for (int i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = breadcrumb[i].GetLocalized(x => x.Name, languageId);
                result = String.IsNullOrEmpty(result)
                    ? categoryName
                    : string.Format("{0} {1} {2}", result, separator, categoryName);
            }

            return result;
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">MembershipCategory</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public static string GetFormattedBreadCrumb(this MembershipCategory category,
            IList<MembershipCategory> allCategories,
            string separator = ">>", int languageId = 0)
        {
            string result = string.Empty;

            var breadcrumb = GetMembershipCategoryBreadCrumb(category, allCategories, null, null, true);
            for (int i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = breadcrumb[i].GetLocalized(x => x.Name, languageId);
                result = String.IsNullOrEmpty(result)
                    ? categoryName
                    : string.Format("{0} {1} {2}", result, separator, categoryName);
            }

            return result;
        }

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">MembershipCategory</param>
        /// <param name="categoryService">MembershipCategory service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>MembershipCategory breadcrumb </returns>
        public static IList<MembershipCategory> GetMembershipCategoryBreadCrumb(this MembershipCategory category,
            IMembershipCategoryService categoryService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var result = new List<MembershipCategory>();

            //used to prevent circular references
            var alreadyProcessedMembershipCategoryIds = new List<int>();

            while (category != null && //not null
                !category.Deleted && //not deleted
                (showHidden || category.Published) && //published
                (showHidden || aclService.Authorize(category)) && //ACL
                (showHidden || storeMappingService.Authorize(category)) && //Store mapping
                !alreadyProcessedMembershipCategoryIds.Contains(category.Id)) //prevent circular references
            {
                result.Add(category);

                alreadyProcessedMembershipCategoryIds.Add(category.Id);

                category = categoryService.GetMembershipCategoryById(category.ParentMembershipCategoryId);
            }
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">MembershipCategory</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>MembershipCategory breadcrumb </returns>
        public static IList<MembershipCategory> GetMembershipCategoryBreadCrumb(this MembershipCategory category,
            IList<MembershipCategory> allCategories,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var result = new List<MembershipCategory>();

            //used to prevent circular references
            var alreadyProcessedMembershipCategoryIds = new List<int>();

            while (category != null && //not null
                !category.Deleted && //not deleted
                (showHidden || category.Published) && //published
                (showHidden || aclService.Authorize(category)) && //ACL
                (showHidden || storeMappingService.Authorize(category)) && //Store mapping
                !alreadyProcessedMembershipCategoryIds.Contains(category.Id)) //prevent circular references
            {
                result.Add(category);

                alreadyProcessedMembershipCategoryIds.Add(category.Id);

                category = (from c in allCategories
                            where c.Id == category.ParentMembershipCategoryId
                            select c).FirstOrDefault();
            }
            result.Reverse();
            return result;
        }
    }
}
