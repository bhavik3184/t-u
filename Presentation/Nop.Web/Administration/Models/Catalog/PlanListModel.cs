
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class PlanListModel : BaseNopModel
    {
        public PlanListModel()
        {
            AvailableMembershipCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailablePlanTypes = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanName")]
        [AllowHtml]
        public string SearchPlanName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchCategory")]
        public int SearchCategoryId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchStore")]
        public int SearchStoreId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchVendor")]
        public int SearchVendorId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchWarehouse")]
        public int SearchWarehouseId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPlanType")]
        public int SearchPlanTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Plans.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Plans.List.GoDirectlyToSku")]
        [AllowHtml]
        public string GoDirectlyToSku { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        public IList<SelectListItem> AvailableMembershipCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public IList<SelectListItem> AvailablePlanTypes { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
    }
}