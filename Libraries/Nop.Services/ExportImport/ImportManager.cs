using System;
using System.IO;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using OfficeOpenXml;
using Nop.Services.SubscriptionOrders;
using Nop.Core.Domain.SubscriptionOrders;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ILocalityService _localityService;
        

        #endregion

        #region Ctor

        public ImportManager(IProductService productService, 
            ICategoryService categoryService,
            ISpecificationAttributeService specificationAttributeService,
            IProductAttributeService productAttributeService,
            IGiftCardService giftCardService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ILocalityService localityService)
        {
            this._productService = productService;
            this._specificationAttributeService = specificationAttributeService;
            this._productAttributeService = productAttributeService;
            this._giftCardService = giftCardService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._storeContext = storeContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._localityService = localityService;
        }

        #endregion

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            var mimeType = MimeMapping.GetMimeMapping(filePath);

            //little hack here because MimeMapping does not contain all mappings (e.g. PNG)
            if (mimeType == "application/octet-stream")
                mimeType = "image/jpeg";

            return mimeType;
        }

        #endregion

        #region Methods


        public virtual void ImportManufacturersFromXlsx(Stream stream)
        {
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new[]
                {
                    "Id",
                    "Name",
                    "Description",
                    "ManufacturerTemplateId",
                    "MetaKeywords",
                    "MetaDescription",
                    "MetaTitle",
                    "PictureId",
                    "PageSize",
                    "AllowCustomersToSelectPageSize",
                    "PageSizeOptions",
                    "PriceRanges",
                    "HasDiscountsApplied",
                    "SubjectToAcl",
                    "LimitedToStores",
                    "Published",
                    "Deleted",
                    "DisplayOrder"
                   
                };


                int iRow = 2;
                while (true)
                {
                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= properties.Length; i++)
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    if (allColumnsAreEmpty)
                        break;

                    int Id = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "Id")].Value);
                    string Name = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value);
                    string Description = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Description")].Value);
                    int ManufacturerTemplateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerTemplateId")].Value);
                    string MetaKeywords = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value);
                    string MetaDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value);
                    string MetaTitle = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value);
                    int PictureId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PictureId")].Value);
                    int PageSize = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PageSize")].Value);
                    bool AllowCustomersToSelectPageSize = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowCustomersToSelectPageSize")].Value);
                    string PageSizeOptions = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "PageSizeOptions")].Value);
                    string PriceRanges = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "PriceRanges")].Value);
                    bool HasDiscountsApplied = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasDiscountsApplied")].Value);
                    bool SubjectToAcl = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "SubjectToAcl")].Value);
                    bool LimitedToStores = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "LimitedToStores")].Value);
                    bool Published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    bool Deleted = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Deleted")].Value);
                    int DisplayOrder = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DisplayOrder")].Value);

                    //DateTime createdOnUtc = DateTime.FromOADate(Convert.ToDouble(worksheet.Cells[iRow, GetColumnIndex(properties, "CreatedOnUtc")].Value));

                    var manufacturer = _manufacturerService.GetManufacturerById(Id);

                    bool newMan = false;
                    if (manufacturer == null)
                    {
                        manufacturer = new Manufacturer();
                        newMan = true;
                    }

                    manufacturer.Name = Name;
                    manufacturer.Description = Description;
                    manufacturer.ManufacturerTemplateId = ManufacturerTemplateId;
                    manufacturer.Description = MetaKeywords;
                    manufacturer.MetaDescription = MetaDescription;
                    manufacturer.MetaTitle = MetaTitle;
                    manufacturer.PictureId = PictureId;
                    manufacturer.PageSize = PageSize;
                    manufacturer.AllowCustomersToSelectPageSize = AllowCustomersToSelectPageSize;
                    manufacturer.PageSizeOptions = PageSizeOptions;
                    manufacturer.PriceRanges = PriceRanges;
                  //  manufacturer.HasDiscountsApplied = HasDiscountsApplied;
                    manufacturer.SubjectToAcl = SubjectToAcl;
                    manufacturer.LimitedToStores = LimitedToStores;
                    manufacturer.Published = Published;
                    manufacturer.Deleted = Deleted;
                    manufacturer.DisplayOrder = DisplayOrder;
                    manufacturer.UpdatedOnUtc = DateTime.Now;
                    manufacturer.CreatedOnUtc = DateTime.Now;

                    if (newMan)
                    {
                        _manufacturerService.InsertManufacturer(manufacturer);
                    }
                    else
                    {
                        _manufacturerService.UpdateManufacturer(manufacturer);
                    }

                    iRow++;
                }
            }
        }

        public virtual void ImportCategoriesFromXlsx(Stream stream)
        {
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");
               // _categoryService.DeleteCategoryAll();

                //the columns
                var properties = new[]
                {
                    "Id",
                    "Name",
                    "Description",
                    "CategoryTemplateId",
                    "MetaKeywords",
                    "MetaDescription",
                    "MetaTitle",
                    "seourl",
                    "ParentCategoryId",
                    "PictureId",
                    "PageSize",
                    "AllowCustomersToSelectPageSize",
                    "PageSizeOptions",
                    "PriceRanges",
                    "ShowOnHomePage",
                    "IncludeInTopMenu",
                    "HasDiscountsApplied",
                    "SubjectToAcl",
                    "LimitedToStores",
                    "Published",
                    "Deleted",
                    "DisplayOrder",
                    "CreatedOnUtc",
                    "Picture1"
                };


                int iRow = 2;
                while (true)
                {

                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= properties.Length; i++)
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    if (allColumnsAreEmpty)
                        break;

                    int Id = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "Id")].Value);
                    string name = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value);
                    string Description = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Description")].Value);
                    int CategoryTemplateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "CategoryTemplateId")].Value);
                    string metaKeywords = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value);
                    string metaDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value);
                    string metaTitle = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value);
                    string seourl = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "seourl")].Value);
                    int ParentCategoryId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ParentCategoryId")].Value);
                    int PictureId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PictureId")].Value);
                    int PageSize = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PageSize")].Value);
                    bool AllowCustomersToSelectPageSize = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowCustomersToSelectPageSize")].Value);
                    string PageSizeOptions = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "PageSizeOptions")].Value);
                    string PriceRanges = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "PriceRanges")].Value);
                    bool showOnHomePage = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShowOnHomePage")].Value);
                    bool IncludeInTopMenu = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IncludeInTopMenu")].Value);
                    bool HasDiscountsApplied = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasDiscountsApplied")].Value);
                    bool SubjectToAcl = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "SubjectToAcl")].Value);
                    bool LimitedToStores = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "LimitedToStores")].Value);
                    bool published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    bool Deleted = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Deleted")].Value);
                    int DisplayOrder = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DisplayOrder")].Value);

                    //DateTime createdOnUtc = DateTime.FromOADate(Convert.ToDouble(worksheet.Cells[iRow, GetColumnIndex(properties, "CreatedOnUtc")].Value));


                    var category = _categoryService.GetCategoryById(Id);

                    bool newProduct = false;
                    if (category == null)
                    {
                        category = new Category();
                        newProduct = true;
                    }
                    category.Name = name;
                    category.Description = Description;
                    category.CategoryTemplateId = CategoryTemplateId;
                    category.MetaKeywords = metaKeywords;
                    category.MetaDescription = metaDescription;
                    category.MetaTitle = metaTitle;
                    category.ParentCategoryId = ParentCategoryId;
                    category.PictureId = PictureId;
                    category.PageSize = PageSize;
                    category.AllowCustomersToSelectPageSize = AllowCustomersToSelectPageSize;
                    category.PageSizeOptions = PageSizeOptions;
                    category.PriceRanges = PriceRanges;
                    category.ShowOnHomePage = showOnHomePage;
                    category.IncludeInTopMenu = IncludeInTopMenu;
                    //category.HasDiscountsApplied = HasDiscountsApplied;
                    category.SubjectToAcl = SubjectToAcl;
                    category.LimitedToStores = LimitedToStores;
                    category.LimitedToStores = LimitedToStores;
                    category.Published = published;
                    category.UpdatedOnUtc = DateTime.Now;
                    category.CreatedOnUtc = DateTime.Now;
                    category.DisplayOrder = DisplayOrder;

                    if (newProduct)
                    {
                        _categoryService.InsertCategory(category);
                    }
                    else
                    {
                        _categoryService.UpdateCategory(category);
                    }

                    seourl = category.ValidateSeName(seourl, category.Name, true);

                    _urlRecordService.SaveSlug(category, seourl, 0);

                    //search engine name
                    //    _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);


                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    //  _productService.UpdateHasTierPricesProperty(product);
                    // _productService.UpdateHasDiscountsApplied(product);

                    //next product
                    iRow++;
                }
            }
        }

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportProductsFromXlsx(Stream stream)
        {
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new []
                {
                    "ProductTypeId",
                    "ParentGroupedProductId",
                    "VisibleIndividually",
                    "Name",
                    "ShortDescription",
                    "FullDescription",
                    "VendorId",
                    "ProductTemplateId",
                    "ShowOnHomePage",
                    "MetaKeywords",
                    "MetaDescription",
                    "MetaTitle",
                    "SeName",
                    "AllowCustomerReviews",
                    "Published",
                    "SKU",
                    "ManufacturerPartNumber",
                    "Gtin",
                    "IsGiftCard",
                    "GiftCardTypeId",
                    "OverriddenGiftCardAmount",
                    "RequireOtherProducts",
                    "RequiredProductIds",
                    "AutomaticallyAddRequiredProducts",
                    "IsDownload",
                    "DownloadId",
                    "UnlimitedDownloads",
                    "MaxNumberOfDownloads",
                    "DownloadActivationTypeId",
                    "HasSampleDownload",
                    "SampleDownloadId",
                    "HasUserAgreement",
                    "UserAgreementText",
                    "IsRecurring",
                    "RecurringCycleLength",
                    "RecurringCyclePeriodId",
                    "RecurringTotalCycles",
                    "IsRental",
                    "RentalPriceLength",
                    "RentalPricePeriodId",
                    "IsShipEnabled",
                    "IsFreeShipping",
                    "ShipSeparately",
                    "AdditionalShippingCharge",
                    "DeliveryDateId",
                    "IsTaxExempt",
                    "TaxCategoryId",
                    "IsTelecommunicationsOrBroadcastingOrElectronicServices",
                    "ManageInventoryMethodId",
                    "UseMultipleWarehouses",
                    "WarehouseId",
                    "StockQuantity",
                    "DisplayStockAvailability",
                    "DisplayStockQuantity",
                    "MinStockQuantity",
                    "LowStockActivityId",
                    "NotifyAdminForQuantityBelow",
                    "BackorderModeId",
                    "AllowBackInStockSubscriptions",
                    "OrderMinimumQuantity",
                    "OrderMaximumQuantity",
                    "AllowedQuantities",
                    "AllowAddingOnlyExistingAttributeCombinations",
                    "DisableBuyButton",
                    "DisableMyToyBoxButton",
                    "AvailableForPreOrder",
                    "PreOrderAvailabilityStartDateTimeUtc",
                    "CallForPrice",
                    "Price",
                    "OldPrice",
                    "ProductCost",
                    "SpecialPrice",
                    "SpecialPriceStartDateTimeUtc",
                    "SpecialPriceEndDateTimeUtc",
                    "CustomerEntersPrice",
                    "MinimumCustomerEnteredPrice",
                    "MaximumCustomerEnteredPrice",
                    "BasepriceEnabled",
                    "BasepriceAmount",
                    "BasepriceUnitId",
                    "BasepriceBaseAmount",
                    "BasepriceBaseUnitId",
                    "MarkAsNew",
                    "MarkAsNewStartDateTimeUtc",
                    "MarkAsNewEndDateTimeUtc",
                    "Weight",
                    "Length",
                    "Width",
                    "Height",
                    "CreatedOnUtc",
                    "CategoryIds",
                    "ManufacturerIds",
                    "Picture1",
                    "Picture2",
                    "Picture3"
                };


                int iRow = 2;
                while (true)
                {
                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= properties.Length; i++)
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    if (allColumnsAreEmpty)
                        break;

                    int productTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductTypeId")].Value);
                    int parentGroupedProductId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ParentGroupedProductId")].Value);
                    bool visibleIndividually = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "VisibleIndividually")].Value);
                    string name = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value);
                    string shortDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ShortDescription")].Value);
                    string fullDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "FullDescription")].Value);
                    int vendorId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "VendorId")].Value);
                    int productTemplateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductTemplateId")].Value);
                    bool showOnHomePage = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShowOnHomePage")].Value);
                    string metaKeywords = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value);
                    string metaDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value);
                    string metaTitle = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value);
                    string seName = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SeName")].Value);
                    bool allowCustomerReviews = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowCustomerReviews")].Value);
                    bool published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    string sku = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SKU")].Value);
                    string manufacturerPartNumber = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerPartNumber")].Value);
                    string gtin = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Gtin")].Value);
                    bool isGiftCard = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsGiftCard")].Value);
                    int giftCardTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "GiftCardTypeId")].Value);
                    decimal? overriddenGiftCardAmount = null;
                    var overriddenGiftCardAmountExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "OverriddenGiftCardAmount")].Value;
                    if (overriddenGiftCardAmountExcel != null)
                        overriddenGiftCardAmount = Convert.ToDecimal(overriddenGiftCardAmountExcel);
                    bool requireOtherProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "RequireOtherProducts")].Value);
                    string requiredProductIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "RequiredProductIds")].Value);
                    bool automaticallyAddRequiredProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AutomaticallyAddRequiredProducts")].Value);
                    bool isDownload = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsDownload")].Value);
                    int downloadId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DownloadId")].Value);
                    bool unlimitedDownloads = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "UnlimitedDownloads")].Value);
                    int maxNumberOfDownloads = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MaxNumberOfDownloads")].Value);
                    int downloadActivationTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DownloadActivationTypeId")].Value);
                    bool hasSampleDownload = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasSampleDownload")].Value);
                    int sampleDownloadId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "SampleDownloadId")].Value);
                    bool hasUserAgreement = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasUserAgreement")].Value);
                    string userAgreementText = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "UserAgreementText")].Value);
                    bool isRecurring = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsRecurring")].Value);
                    int recurringCycleLength = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringCycleLength")].Value);
                    int recurringCyclePeriodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringCyclePeriodId")].Value);
                    int recurringTotalCycles = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringTotalCycles")].Value);
                    bool isRental = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsRental")].Value);
                    int rentalPriceLength = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RentalPriceLength")].Value);
                    int rentalPricePeriodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RentalPricePeriodId")].Value);
                    bool isShipEnabled = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsShipEnabled")].Value);
                    bool isFreeShipping = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsFreeShipping")].Value);
                    bool shipSeparately = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShipSeparately")].Value);
                    decimal additionalShippingCharge = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "AdditionalShippingCharge")].Value);
                    int deliveryDateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DeliveryDateId")].Value);
                    bool isTaxExempt = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsTaxExempt")].Value);
                    int taxCategoryId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "TaxCategoryId")].Value);
                    bool isTelecommunicationsOrBroadcastingOrElectronicServices = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsTelecommunicationsOrBroadcastingOrElectronicServices")].Value);
                    int manageInventoryMethodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ManageInventoryMethodId")].Value);
                    bool useMultipleWarehouses = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "UseMultipleWarehouses")].Value);
                    int warehouseId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "WarehouseId")].Value);
                    int stockQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "StockQuantity")].Value);
                    bool displayStockAvailability = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisplayStockAvailability")].Value);
                    bool displayStockQuantity = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisplayStockQuantity")].Value);
                    int minStockQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MinStockQuantity")].Value);
                    int lowStockActivityId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "LowStockActivityId")].Value);
                    int notifyAdminForQuantityBelow = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "NotifyAdminForQuantityBelow")].Value);
                    int backorderModeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "BackorderModeId")].Value);
                    bool allowBackInStockSubscriptions = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowBackInStockSubscriptions")].Value);
                    int orderMinimumQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "OrderMinimumQuantity")].Value);
                    int orderMaximumQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "OrderMaximumQuantity")].Value);
                    string allowedQuantities = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowedQuantities")].Value);
                    bool allowAddingOnlyExistingAttributeCombinations = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowAddingOnlyExistingAttributeCombinations")].Value);
                    bool disableBuyButton = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisableBuyButton")].Value);
                    bool disableMyToyBoxButton = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisableMyToyBoxButton")].Value);
                    bool availableForPreOrder = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AvailableForPreOrder")].Value);
                    DateTime? preOrderAvailabilityStartDateTimeUtc = null;
                    var preOrderAvailabilityStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "PreOrderAvailabilityStartDateTimeUtc")].Value;
                    if (preOrderAvailabilityStartDateTimeUtcExcel != null)
                        preOrderAvailabilityStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(preOrderAvailabilityStartDateTimeUtcExcel));
                    bool callForPrice = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "CallForPrice")].Value);
                    decimal price = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Price")].Value);
                    decimal oldPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "OldPrice")].Value);
                    decimal productCost = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductCost")].Value);
                    decimal? specialPrice = null;
                    var specialPriceExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPrice")].Value;
                    if (specialPriceExcel != null)
                        specialPrice = Convert.ToDecimal(specialPriceExcel);
                    DateTime? specialPriceStartDateTimeUtc = null;
                    var specialPriceStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPriceStartDateTimeUtc")].Value;
                    if (specialPriceStartDateTimeUtcExcel != null)
                        specialPriceStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(specialPriceStartDateTimeUtcExcel));
                    DateTime? specialPriceEndDateTimeUtc = null;
                    var specialPriceEndDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPriceEndDateTimeUtc")].Value;
                    if (specialPriceEndDateTimeUtcExcel != null)
                        specialPriceEndDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(specialPriceEndDateTimeUtcExcel));

                    bool customerEntersPrice = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "CustomerEntersPrice")].Value);
                    decimal minimumCustomerEnteredPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "MinimumCustomerEnteredPrice")].Value);
                    decimal maximumCustomerEnteredPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "MaximumCustomerEnteredPrice")].Value);
                    bool basepriceEnabled = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceEnabled")].Value);
                    decimal basepriceAmount = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceAmount")].Value);
                    int basepriceUnitId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceUnitId")].Value);
                    decimal basepriceBaseAmount = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceBaseAmount")].Value);
                    int basepriceBaseUnitId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceBaseUnitId")].Value);
                    bool markAsNew = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "MarkAsNew")].Value);
                    DateTime? markAsNewStartDateTimeUtc = null;
                    var markAsNewStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "MarkAsNewStartDateTimeUtc")].Value;
                    if (markAsNewStartDateTimeUtcExcel != null)
                        markAsNewStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(markAsNewStartDateTimeUtcExcel));
                    DateTime? markAsNewEndDateTimeUtc = null;
                    var markAsNewEndDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "MarkAsNewEndDateTimeUtc")].Value;
                    if (markAsNewEndDateTimeUtcExcel != null)
                        markAsNewEndDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(markAsNewEndDateTimeUtcExcel));
                    decimal weight = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Weight")].Value);
                    decimal length = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Length")].Value);
                    decimal width = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Width")].Value);
                    decimal height = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Height")].Value);
                    DateTime createdOnUtc = DateTime.FromOADate(Convert.ToDouble(worksheet.Cells[iRow, GetColumnIndex(properties, "CreatedOnUtc")].Value));
                    string categoryIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "CategoryIds")].Value);
                    string manufacturerIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerIds")].Value);
                    string picture1 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture1")].Value);
                    string picture2 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture2")].Value);
                    string picture3 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture3")].Value);



                    var product = _productService.GetProductBySku(sku);
                    bool newProduct = false;
                    if (product == null)
                    {
                        product = new Product();
                        newProduct = true;
                    }
                    product.ProductTypeId = productTypeId;
                    product.ParentGroupedProductId = parentGroupedProductId;
                    product.VisibleIndividually = visibleIndividually;
                    product.Name = name;
                    product.ShortDescription = shortDescription;
                    product.FullDescription = fullDescription;
                    product.VendorId = vendorId;
                    product.ProductTemplateId = productTemplateId;
                    product.ShowOnHomePage = showOnHomePage;
                    product.MetaKeywords = metaKeywords;
                    product.MetaDescription = metaDescription;
                    product.MetaTitle = metaTitle;
                    product.AllowCustomerReviews = allowCustomerReviews;
                    product.Sku = sku;
                    product.ManufacturerPartNumber = manufacturerPartNumber;
                    product.Gtin = gtin;
                    product.IsGiftCard = isGiftCard;
                    product.GiftCardTypeId = giftCardTypeId;
                    product.OverriddenGiftCardAmount = overriddenGiftCardAmount;
                    product.RequireOtherProducts = requireOtherProducts;
                    product.RequiredProductIds = requiredProductIds;
                    product.AutomaticallyAddRequiredProducts = automaticallyAddRequiredProducts;
                    product.IsDownload = isDownload;
                    product.DownloadId = downloadId;
                    product.UnlimitedDownloads = unlimitedDownloads;
                    product.MaxNumberOfDownloads = maxNumberOfDownloads;
                    product.DownloadActivationTypeId = downloadActivationTypeId;
                    product.HasSampleDownload = hasSampleDownload;
                    product.SampleDownloadId = sampleDownloadId;
                    product.HasUserAgreement = hasUserAgreement;
                    product.UserAgreementText = userAgreementText;
                    product.IsRecurring = isRecurring;
                    product.RecurringCycleLength = recurringCycleLength;
                    product.RecurringCyclePeriodId = recurringCyclePeriodId;
                    product.RecurringTotalCycles = recurringTotalCycles;
                    product.IsRental = isRental;
                    product.RentalPriceLength = rentalPriceLength;
                    product.RentalPricePeriodId = rentalPricePeriodId;
                    product.IsShipEnabled = isShipEnabled;
                    product.IsFreeShipping = isFreeShipping;
                    product.ShipSeparately = shipSeparately;
                    product.AdditionalShippingCharge = additionalShippingCharge;
                    product.DeliveryDateId = deliveryDateId;
                    product.IsTaxExempt = isTaxExempt;
                    product.TaxCategoryId = taxCategoryId;
                    product.IsTelecommunicationsOrBroadcastingOrElectronicServices = isTelecommunicationsOrBroadcastingOrElectronicServices;
                    product.ManageInventoryMethodId = manageInventoryMethodId;
                    product.UseMultipleWarehouses = useMultipleWarehouses;
                    product.WarehouseId = warehouseId;
                    product.StockQuantity = stockQuantity;
                    product.DisplayStockAvailability = displayStockAvailability;
                    product.DisplayStockQuantity = displayStockQuantity;
                    product.MinStockQuantity = minStockQuantity;
                    product.LowStockActivityId = lowStockActivityId;
                    product.NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow;
                    product.BackorderModeId = backorderModeId;
                    product.AllowBackInStockSubscriptions = allowBackInStockSubscriptions;
                    product.OrderMinimumQuantity = orderMinimumQuantity;
                    product.OrderMaximumQuantity = orderMaximumQuantity;
                    product.AllowedQuantities = allowedQuantities;
                    product.AllowAddingOnlyExistingAttributeCombinations = allowAddingOnlyExistingAttributeCombinations;
                    product.DisableBuyButton = disableBuyButton;
                    product.DisableMyToyBoxButton = disableMyToyBoxButton;
                    product.AvailableForPreOrder = availableForPreOrder;
                    product.PreOrderAvailabilityStartDateTimeUtc = preOrderAvailabilityStartDateTimeUtc;
                    product.CallForPrice = callForPrice;
                    product.Price = price;
                    product.OldPrice = oldPrice;
                    product.ProductCost = productCost;
                    product.SpecialPrice = specialPrice;
                    product.SpecialPriceStartDateTimeUtc = specialPriceStartDateTimeUtc;
                    product.SpecialPriceEndDateTimeUtc = specialPriceEndDateTimeUtc;
                    product.CustomerEntersPrice = customerEntersPrice;
                    product.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
                    product.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
                    product.BasepriceEnabled = basepriceEnabled;
                    product.BasepriceAmount = basepriceAmount;
                    product.BasepriceUnitId = basepriceUnitId;
                    product.BasepriceBaseAmount = basepriceBaseAmount;
                    product.BasepriceBaseUnitId = basepriceBaseUnitId;
                    product.MarkAsNew = markAsNew;
                    product.MarkAsNewStartDateTimeUtc = markAsNewStartDateTimeUtc;
                    product.MarkAsNewEndDateTimeUtc = markAsNewEndDateTimeUtc;
                    product.Weight = weight;
                    product.Length = length;
                    product.Width = width;
                    product.Height = height;
                    product.Published = published;
                    product.CreatedOnUtc = createdOnUtc;
                    product.UpdatedOnUtc = DateTime.UtcNow;
                    if (newProduct)
                    {
                        _productService.InsertProduct(product);
                    }
                    else
                    {
                        _productService.UpdateProduct(product);
                    }

                    //search engine name
                    _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);

                    //category mappings
                    if (!String.IsNullOrEmpty(categoryIds))
                    {
                        foreach (var id in categoryIds.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (product.ProductCategories.FirstOrDefault(x => x.CategoryId == id) == null)
                            {
                                //ensure that category exists
                                var category = _categoryService.GetCategoryById(id);
                                if (category != null)
                                {
                                    var productCategory = new ProductCategory
                                    {
                                        ProductId = product.Id,
                                        CategoryId = category.Id,
                                        IsFeaturedProduct = false,
                                        DisplayOrder = 1
                                    };
                                    _categoryService.InsertProductCategory(productCategory);
                                }
                            }
                        }
                    }

                    //manufacturer mappings
                    if (!String.IsNullOrEmpty(manufacturerIds))
                    {
                        foreach (var id in manufacturerIds.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (product.ProductManufacturers.FirstOrDefault(x => x.ManufacturerId == id) == null)
                            {
                                //ensure that manufacturer exists
                                var manufacturer = _manufacturerService.GetManufacturerById(id);
                                if (manufacturer != null)
                                {
                                    var productManufacturer = new ProductManufacturer
                                    {
                                        ProductId = product.Id,
                                        ManufacturerId = manufacturer.Id,
                                        IsFeaturedProduct = false,
                                        DisplayOrder = 1
                                    };
                                    _manufacturerService.InsertProductManufacturer(productManufacturer);
                                }
                            }
                        }
                    }

                    //pictures
                    foreach (var picturePath in new [] { picture1, picture2, picture3 })
                    {
                        if (String.IsNullOrEmpty(picturePath))
                            continue;

                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = File.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!newProduct)
                        {
                            //compare with existing product pictures
                            var existingPictures = _pictureService.GetPicturesByProductId(product.Id);
                            foreach (var existingPicture in existingPictures)
                            {
                                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                //picture binary after validation (like in database)
                                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                                if (existingBinary.SequenceEqual(validatedPictureBinary) || existingBinary.SequenceEqual(newPictureBinary))
                                {
                                    //the same picture content
                                    pictureAlreadyExists = true;
                                    break;
                                }
                            }
                        }

                        if (!pictureAlreadyExists)
                        {
                            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType , _pictureService.GetPictureSeName(name));
                            product.ProductPictures.Add(new ProductPicture
                            {
                                //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                                //pictures are duplicated
                                //maybe because entity size is too large
                                PictureId = newPicture.Id,
                                DisplayOrder = 1,
                            });
                            _productService.UpdateProduct(product);
                        }
                    }

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    _productService.UpdateHasTierPricesProperty(product);
                    _productService.UpdateHasDiscountsApplied(product);



                    //next product
                    iRow++;
                }
            }
        }


        public virtual void ImportSimpleProductsFromXlsx(Stream stream)
        {
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new[]
                {
                    "Name",
                    "FullDescription",
                    "VendorId",
                    "MetaKeywords",
                    "MetaDescription",
                    "MetaTitle",
                    "SeName",
                    "Published",
                    "SKU",
                    "AdditionalShippingCharge",
                    "TaxCategoryId",
                    "StockQuantity",
                    "MinStockQuantity",
                    "NotifyAdminForQuantityBelow",
                    "Price",
                    "OldPrice",
                    "ProductCost",
                    "SpecialPrice",
                    "SpecialPriceStartDateTimeUtc",
                    "SpecialPriceEndDateTimeUtc",
                    "Weight",
                    "CategoryIds",
                    "ManufacturerIds",
                    "Picture1",
                    "Picture2",
                    "Picture3",
                    "Picture4",
                    "Picture5",
                    "Picture6",
                    "Picture7",
                    "Picture8",
                    "Picture9",
                    "AttributeColor",
                    "Color",
                    "ColorType",
                    "Player",
                    "PlayerType",
                    "Dimension",
                    "DimensionType",
                    "Age",
                    "AgeType",
                    "Gender",
                    "GenderType",
                    "Power",
                    "PowerType",
                    "Light",
                    "LightType",
                    "Material",
                    "MaterialType" 
                    
                };

                string prevsku = "";
                int iRow = 2;
                while (true)
                {

                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= properties.Length; i++)
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    if (allColumnsAreEmpty)
                        break;

                    int productTypeId = 5;
                    int parentGroupedProductId = 0;
                    bool visibleIndividually = true;
                    string name = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value);
                    string shortDescription = name;
                    string fullDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "FullDescription")].Value);
                    int vendorId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "VendorId")].Value);
                    int productTemplateId = 1;
                    bool showOnHomePage = true;
                    string metaKeywords = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value);
                    string metaDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value);
                    string metaTitle = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value);
                    string seName = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SeName")].Value);
                    bool allowCustomerReviews = true;
                    bool published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    string sku = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SKU")].Value);
                    string manufacturerPartNumber = "";
                    string gtin = "";
                    bool isGiftCard = false;
                    int giftCardTypeId = 1;
                    bool requireOtherProducts = false;
                    string requiredProductIds = "";
                    bool automaticallyAddRequiredProducts = false;
                    bool isDownload = false;
                    int downloadId = 0;
                    bool unlimitedDownloads = false;
                    int maxNumberOfDownloads = 0;
                    int downloadActivationTypeId = 0;
                    bool hasSampleDownload = false;
                    int sampleDownloadId = 0;
                    bool hasUserAgreement = false;
                    string userAgreementText = "";
                    bool isRecurring = false;
                    int recurringCycleLength = 0;
                    int recurringCyclePeriodId = 0;
                    int recurringTotalCycles = 0;
                    bool isRental = false;
                    int rentalPriceLength = 0;
                    int rentalPricePeriodId = 0;
                    bool isShipEnabled = true;
                    bool isFreeShipping = false;
                    bool shipSeparately = false;
                    decimal additionalShippingCharge = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "AdditionalShippingCharge")].Value);
                    int deliveryDateId = 1;
                    bool isTaxExempt = false;
                    int taxCategoryId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "TaxCategoryId")].Value);
                    bool isTelecommunicationsOrBroadcastingOrElectronicServices = false;
                    int manageInventoryMethodId = 1;
                    bool useMultipleWarehouses = false;
                    int warehouseId = 0;
                    int stockQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "StockQuantity")].Value);
                    bool displayStockAvailability = true;
                    bool displayStockQuantity = false;
                    int minStockQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MinStockQuantity")].Value);
                    int lowStockActivityId = 0;
                    int notifyAdminForQuantityBelow = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "NotifyAdminForQuantityBelow")].Value);
                    int backorderModeId = 0;
                    bool allowBackInStockSubscriptions = false;
                    int orderMinimumQuantity = 1;
                    int orderMaximumQuantity = 1000;
                    string allowedQuantities = "";
                    bool allowAddingOnlyExistingAttributeCombinations = false;
                    bool disableBuyButton = false;
                    bool disableWishlistButton = true;
                    bool availableForPreOrder = true;
                    DateTime? preOrderAvailabilityStartDateTimeUtc = null;

                    bool callForPrice = false;
                    decimal price = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Price")].Value);
                    decimal oldPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "OldPrice")].Value);
                    decimal productCost = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductCost")].Value);
                    decimal? specialPrice = null;
                    var specialPriceExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPrice")].Value;
                    if (specialPriceExcel != null)
                        specialPrice = Convert.ToDecimal(specialPriceExcel);
                    DateTime? specialPriceStartDateTimeUtc = null;
                    var specialPriceStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPriceStartDateTimeUtc")].Value;
                    if (specialPriceStartDateTimeUtcExcel != null)
                        specialPriceStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(specialPriceStartDateTimeUtcExcel));
                    DateTime? specialPriceEndDateTimeUtc = null;
                    var specialPriceEndDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPriceEndDateTimeUtc")].Value;
                    if (specialPriceEndDateTimeUtcExcel != null)
                        specialPriceEndDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(specialPriceEndDateTimeUtcExcel));

                    bool customerEntersPrice = false;
                    decimal minimumCustomerEnteredPrice = 0;
                    decimal maximumCustomerEnteredPrice = 0;
                    bool basepriceEnabled = false;
                    decimal basepriceAmount = 0;
                    int basepriceUnitId = 1;
                    decimal basepriceBaseAmount = 0;
                    int basepriceBaseUnitId = 1;
                    decimal weight = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Weight")].Value);
                    decimal length = 0;
                    decimal width = 0;
                    decimal height = 0;
                    DateTime createdOnUtc = DateTime.Now;
                    string categoryIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "CategoryIds")].Value);
                    string manufacturerIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerIds")].Value);
                    string picture1 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture1")].Value);
                    string picture2 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture2")].Value);
                    string picture3 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture3")].Value);
                    string picture4 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture4")].Value);
                    string picture5 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture5")].Value);
                    string picture6 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture6")].Value);
                    string picture7 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture7")].Value);
                    string picture8 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture8")].Value);
                    string picture9 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture9")].Value);
                    string attributeColor = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "AttributeColor")].Value);
                    string color = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Color")].Value);
                    int colorType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ColorType")].Value);
                    string player = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Player")].Value);
                    int playerType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PlayerType")].Value);
                    string dimension = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Dimension")].Value);
                    int dimensionType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DimensionType")].Value);
                    string age = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Age")].Value);
                    int ageType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "AgeType")].Value);
                    string gender = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Gender")].Value);
                    int genderType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "GenderType")].Value);
                    string power = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Power")].Value);
                    int powerType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PowerType")].Value);
                    string light = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Light")].Value);
                    int lightType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "LightType")].Value);
                    string material = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Material")].Value);
                    int materialType = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MaterialType")].Value);
                   

                    if (sku == null && attributeColor != null)
                    {
                        if (attributeColor.Length > 0)
                        {
                            var product1 = _productService.GetProductBySku(prevsku);

                            //pictures
                            foreach (var picturePath in new[] { picture1, picture2, picture3, picture4, picture5, picture6, picture7, picture8, picture9 })
                            {

                                if (String.IsNullOrEmpty(picturePath))
                                    continue;

                                String ext = System.IO.Path.GetExtension(picturePath);
                                string picturePath1 = "";
                                if (ext.ToLower() != ".jpg" && ext.ToLower() != ".png" && ext.ToLower() != ".bmp" && ext.ToLower() != ".gif" && ext.ToLower() != ".jpeg")
                                {
                                    picturePath1 = "F:\\KhiloonaImport\\Import\\Product\\" + picturePath + ".jpg";
                                    // picturePath1 = "H:\\Khiloonadeploy\\Import\\Product\\" + picturePath + ".jpg";

                                }
                                else
                                {
                                    picturePath1 = "F:\\KhiloonaImport\\Import\\Product\\" + picturePath;
                                    //  picturePath1 = "H:\\Khiloonadeploy\\Import\\Product\\" + picturePath;
                                }


                                if (System.IO.File.Exists(picturePath1))
                                {
                                    var mimeType = GetMimeTypeFromFilePath(picturePath1);
                                    var newPictureBinary = File.ReadAllBytes(picturePath1);
                                    var pictureAlreadyExists = false;

                                    //compare with existing product pictures
                                    var existingPictures = _pictureService.GetPicturesByProductId(product1.Id);
                                    foreach (var existingPicture in existingPictures)
                                    {
                                        var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                        //picture binary after validation (like in database)
                                        var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                                        if (existingBinary.SequenceEqual(validatedPictureBinary))
                                        {
                                            //the same picture content
                                            pictureAlreadyExists = true;
                                            break;
                                        }
                                    }


                                    if (!pictureAlreadyExists)
                                    {
                                        if (attributeColor != null)
                                            if (attributeColor.Length > 0)
                                            {
                                                product1.ProductPictures.Add(new ProductPicture
                                                {
                                                    Picture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product1.Name), attributeColor, _pictureService.GetPictureSeName(product1.Name)),
                                                    DisplayOrder = 1,
                                                });
                                            }
                                            else
                                            {
                                                product1.ProductPictures.Add(new ProductPicture
                                                {
                                                    Picture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product1.Name)),
                                                    DisplayOrder = 1,
                                                });
                                            }
                                        // _productService.UpdateProduct(product);
                                    }
                                }

                                //update "HasTierPrices" and "HasDiscountsApplied" properties
                                //  _productService.UpdateHasTierPricesProperty(product);
                                // _productService.UpdateHasDiscountsApplied(product);


                            }
                            SaveProductAttributes("Color", attributeColor, product1);
                            iRow++;
                        }
                    }
                    else
                    {
                        sku = sku.Trim();
                        prevsku = sku;
                        var product = _productService.GetProductBySku(sku);
                        bool newProduct = false;
                        if (product == null)
                        {
                            product = new Product();
                            newProduct = true;
                            product.CreatedOnUtc = createdOnUtc;
                        }
                        product.ProductTypeId = productTypeId;
                        product.ParentGroupedProductId = parentGroupedProductId;
                        product.VisibleIndividually = visibleIndividually;
                        product.Name = name;
                        product.ShortDescription = shortDescription;
                        product.FullDescription = fullDescription;
                        product.VendorId = vendorId;
                        product.ProductTemplateId = productTemplateId;
                        product.ShowOnHomePage = showOnHomePage;
                        product.MetaKeywords = metaKeywords;
                        product.MetaDescription = metaDescription;
                        product.MetaTitle = metaTitle;
                        product.AllowCustomerReviews = allowCustomerReviews;
                        product.Sku = sku;
                        product.ManufacturerPartNumber = manufacturerPartNumber;
                        product.Gtin = gtin;
                        product.IsGiftCard = isGiftCard;
                        product.GiftCardTypeId = giftCardTypeId;
                        product.RequireOtherProducts = requireOtherProducts;
                        product.RequiredProductIds = requiredProductIds;
                        product.AutomaticallyAddRequiredProducts = automaticallyAddRequiredProducts;
                        product.IsDownload = isDownload;
                        product.DownloadId = downloadId;
                        product.UnlimitedDownloads = unlimitedDownloads;
                        product.MaxNumberOfDownloads = maxNumberOfDownloads;
                        product.DownloadActivationTypeId = downloadActivationTypeId;
                        product.HasSampleDownload = hasSampleDownload;
                        product.SampleDownloadId = sampleDownloadId;
                        product.HasUserAgreement = hasUserAgreement;
                        product.UserAgreementText = userAgreementText;
                        product.IsRecurring = isRecurring;
                        product.RecurringCycleLength = recurringCycleLength;
                        product.RecurringCyclePeriodId = recurringCyclePeriodId;
                        product.RecurringTotalCycles = recurringTotalCycles;
                        product.IsRental = isRental;
                        product.RentalPriceLength = rentalPriceLength;
                        product.RentalPricePeriodId = rentalPricePeriodId;
                        product.IsShipEnabled = isShipEnabled;
                        product.IsFreeShipping = isFreeShipping;
                        product.ShipSeparately = shipSeparately;
                        product.AdditionalShippingCharge = additionalShippingCharge;
                        product.DeliveryDateId = deliveryDateId;
                        product.IsTaxExempt = isTaxExempt;
                        product.TaxCategoryId = taxCategoryId;
                        product.IsTelecommunicationsOrBroadcastingOrElectronicServices = isTelecommunicationsOrBroadcastingOrElectronicServices;
                        product.ManageInventoryMethodId = manageInventoryMethodId;
                        product.UseMultipleWarehouses = useMultipleWarehouses;
                        product.WarehouseId = warehouseId;
                        product.StockQuantity = stockQuantity;
                        product.DisplayStockAvailability = displayStockAvailability;
                        product.DisplayStockQuantity = displayStockQuantity;
                        product.MinStockQuantity = minStockQuantity;
                        product.LowStockActivityId = lowStockActivityId;
                        product.NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow;
                        product.BackorderModeId = backorderModeId;
                        product.AllowBackInStockSubscriptions = allowBackInStockSubscriptions;
                        product.OrderMinimumQuantity = orderMinimumQuantity;
                        product.OrderMaximumQuantity = orderMaximumQuantity;
                        product.AllowedQuantities = allowedQuantities;
                        product.AllowAddingOnlyExistingAttributeCombinations = allowAddingOnlyExistingAttributeCombinations;
                        product.DisableBuyButton = disableBuyButton;
                        product.DisableMyToyBoxButton = disableWishlistButton;
                        product.AvailableForPreOrder = availableForPreOrder;
                        product.PreOrderAvailabilityStartDateTimeUtc = preOrderAvailabilityStartDateTimeUtc;
                        product.CallForPrice = callForPrice;
                        product.Price = price;
                        product.OldPrice = oldPrice;
                        product.ProductCost = productCost;
                        product.SpecialPrice = specialPrice;
                        product.SpecialPriceStartDateTimeUtc = specialPriceStartDateTimeUtc;
                        product.SpecialPriceEndDateTimeUtc = specialPriceEndDateTimeUtc;
                        product.CustomerEntersPrice = customerEntersPrice;
                        product.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
                        product.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
                        product.BasepriceEnabled = basepriceEnabled;
                        product.BasepriceAmount = basepriceAmount;
                        product.BasepriceUnitId = basepriceUnitId;
                        product.BasepriceBaseAmount = basepriceBaseAmount;
                        product.BasepriceBaseUnitId = basepriceBaseUnitId;
                        product.Weight = weight;
                        product.Length = length;
                        product.Width = width;
                        product.Height = height;
                        product.Published = published;
                        product.UpdatedOnUtc = DateTime.UtcNow;
                        if (newProduct)
                        {
                            _productService.InsertProduct(product);
                        }
                        else
                        {
                            _productService.UpdateProduct(product);
                        }

                        //search engine name
                        _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);

                        //category mappings
                        if (!String.IsNullOrEmpty(categoryIds))
                        {
                            foreach (var id in categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                            {
                                if (product.ProductCategories.FirstOrDefault(x => x.CategoryId == id) == null)
                                {
                                    //ensure that category exists
                                    var category = _categoryService.GetCategoryById(id);
                                    if (category != null)
                                    {
                                        var productCategory = new ProductCategory
                                        {
                                            ProductId = product.Id,
                                            CategoryId = category.Id,
                                            IsFeaturedProduct = false,
                                            DisplayOrder = 1
                                        };
                                        _categoryService.InsertProductCategory(productCategory);
                                    }
                                }
                            }
                        }

                        //manufacturer mappings
                        if (!String.IsNullOrEmpty(manufacturerIds))
                        {
                            foreach (var id in manufacturerIds.Split(new[] { ';' }))
                            {
                                if (product.ProductManufacturers.FirstOrDefault(x => x.Manufacturer.Name == id) == null)
                                {
                                    if (id.Length > 0)
                                    {
                                        //ensure that manufacturer exists
                                        var manufacturer = _manufacturerService.GetAllManufacturers(id).FirstOrDefault();
                                        if (manufacturer != null)
                                        {
                                            var productManufacturer = new ProductManufacturer
                                            {
                                                ProductId = product.Id,
                                                ManufacturerId = manufacturer.Id,
                                                IsFeaturedProduct = false,
                                                DisplayOrder = 1
                                            };
                                            _manufacturerService.InsertProductManufacturer(productManufacturer);
                                        }
                                    }
                                }
                            }
                        }


                        //pictures
                        foreach (var picturePath in new[] { picture1, picture2, picture3, picture4, picture5, picture6, picture7, picture8, picture9 })
                        {

                            if (String.IsNullOrEmpty(picturePath))
                                continue;

                            String ext = System.IO.Path.GetExtension(picturePath);
                            string picturePath1 = "";
                            if (ext.ToLower() != ".jpg" && ext.ToLower() != ".png" && ext.ToLower() != ".bmp" && ext.ToLower() != ".gif" && ext.ToLower() != ".jpeg")
                            {
                                picturePath1 = "F:\\KhiloonaImport\\Import\\Product\\" + picturePath + ".jpg";
                                //picturePath1 = "H:\\Khiloonadeploy\\Import\\Product\\" + picturePath + ".jpg";

                            }
                            else
                            {
                                picturePath1 = "F:\\KhiloonaImport\\Import\\Product\\" + picturePath;
                                //picturePath1 = "H:\\Khiloonadeploy\\Import\\Product\\" + picturePath;
                            }


                            if (System.IO.File.Exists(picturePath1))
                            {
                                var mimeType = GetMimeTypeFromFilePath(picturePath1);
                                var newPictureBinary = File.ReadAllBytes(picturePath1);
                                var pictureAlreadyExists = false;
                                if (!newProduct)
                                {
                                    //compare with existing product pictures
                                    var existingPictures = _pictureService.GetPicturesByProductId(product.Id);
                                    foreach (var existingPicture in existingPictures)
                                    {
                                        var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                        //picture binary after validation (like in database)
                                        var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                                        if (existingBinary.SequenceEqual(validatedPictureBinary))
                                        {
                                            //the same picture content
                                            pictureAlreadyExists = true;
                                            break;
                                        }
                                    }
                                }


                                if (!pictureAlreadyExists)
                                {
                                    if (attributeColor != null)
                                    {
                                        if (attributeColor.Length > 0)
                                        {
                                            product.ProductPictures.Add(new ProductPicture
                                            {
                                                Picture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.Name), attributeColor, _pictureService.GetPictureSeName(product.Name)),
                                                DisplayOrder = 1,
                                            });
                                        }
                                        else
                                        {
                                            product.ProductPictures.Add(new ProductPicture
                                            {
                                                Picture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.Name)),
                                                DisplayOrder = 1,
                                            });
                                        }
                                    }
                                    else
                                    {
                                        product.ProductPictures.Add(new ProductPicture
                                        {
                                            Picture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.Name)),
                                            DisplayOrder = 1,
                                        });
                                    }
                                    // _productService.UpdateProduct(product);
                                }
                            }

                            //update "HasTierPrices" and "HasDiscountsApplied" properties
                            //  _productService.UpdateHasTierPricesProperty(product);
                            // _productService.UpdateHasDiscountsApplied(product);


                        }

                        if (product != null)
                        {
                            if (color != null)
                                if (color.Length > 0)
                                    SaveSpecificationAttributes("Color", color, colorType, product);

                            if (player != null)
                                if (player.Length > 0)
                                    SaveSpecificationAttributes("Player", player, playerType, product);

                            if (dimension != null)
                                if (dimension.Length > 0)
                                    SaveSpecificationAttributes("Dimension", dimension, dimensionType, product);

                            if (age != null)
                                if (age.Length > 0)
                                    SaveSpecificationAttributes("Age", age, ageType, product);

                            if (gender != null)
                                if (gender.Length > 0)
                                    SaveSpecificationAttributes("Gender", gender, genderType, product);

                            if (power != null)
                                if (power.Length > 0)
                                    SaveSpecificationAttributes("Power", power, powerType, product);

                            if (light != null)
                                if (light.Length > 0)
                                    SaveSpecificationAttributes("Light", light, lightType, product);

                            if (material != null)
                                if (material.Length > 0)
                                    SaveSpecificationAttributes("Material", material, materialType, product);


                            if (attributeColor != null)
                                if (attributeColor.Length > 0)
                                    SaveProductAttributes("Color", attributeColor, product);
                        }

                        //next product
                        iRow++;
                    }
                }

            }

        }

        public void SaveSpecificationAttributes(string Specification, string SpecificationValue, int AttributeTypeId, Product product)
        {

            if (SpecificationValue.Length > 0)
            {
                if (product != null)
                {
                    var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeByName(Specification);

                    if (specificationAttribute.Count > 0)
                    {

                        if (AttributeTypeId == 0)
                        {

                            var specificationAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionBySpecificationIdAndValue(specificationAttribute.ElementAt(0).Id, SpecificationValue);

                            _specificationAttributeService.DeleteProductSpecificationAttributeByProductId(product.Id, specificationAttribute.FirstOrDefault().Id);

                            var productSpecificationAttribute = _specificationAttributeService.GetProductSpecificationAttributesOne(product.Id, specificationAttributeOption.Id);

                            bool newProduct = false;

                            if (productSpecificationAttribute == null)
                            {
                                productSpecificationAttribute = new ProductSpecificationAttribute();
                                newProduct = true;
                            }

                            productSpecificationAttribute.ProductId = product.Id;
                            productSpecificationAttribute.AttributeTypeId = AttributeTypeId;

                            if (AttributeTypeId == 0)
                            {
                                productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOption.Id;
                                productSpecificationAttribute.CustomValue = "";
                                productSpecificationAttribute.AllowFiltering = true;
                                productSpecificationAttribute.ShowOnProductPage = true;
                                productSpecificationAttribute.DisplayOrder = 0;
                            }
                            else
                            {
                                productSpecificationAttribute.SpecificationAttributeOptionId = 0;
                                productSpecificationAttribute.CustomValue = SpecificationValue;

                                productSpecificationAttribute.AllowFiltering = false;
                                productSpecificationAttribute.ShowOnProductPage = true;
                                productSpecificationAttribute.DisplayOrder = 0;
                            }


                            if (newProduct)
                            {
                                _specificationAttributeService.InsertProductSpecificationAttribute(productSpecificationAttribute);
                            }
                            else
                            {
                                _specificationAttributeService.UpdateProductSpecificationAttribute(productSpecificationAttribute);
                            }
                        }

                        else
                        {
                            var specificationAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionBySpecificationIdAndValue(specificationAttribute.ElementAt(0).Id, SpecificationValue);

                            _specificationAttributeService.DeleteProductSpecificationAttributeByProductId(product.Id, specificationAttribute.FirstOrDefault().Id);

                            var productSpecificationAttribute = _specificationAttributeService.GetProductSpecificationAttributesByCustomValue(product.Id, specificationAttribute.FirstOrDefault().Id, SpecificationValue);

                            bool newProduct = false;

                            if (productSpecificationAttribute == null)
                            {
                                productSpecificationAttribute = new ProductSpecificationAttribute();
                                newProduct = true;
                            }

                            productSpecificationAttribute.ProductId = product.Id;
                            productSpecificationAttribute.AttributeTypeId = AttributeTypeId;

                            var s = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttribute.FirstOrDefault().Id);

                            if (AttributeTypeId == 0)
                            {
                                var a = s.Where(psa => psa.Name == SpecificationValue);
                                productSpecificationAttribute.SpecificationAttributeOptionId = a.FirstOrDefault().Id;
                                productSpecificationAttribute.CustomValue = "";

                                productSpecificationAttribute.AllowFiltering = true;
                                productSpecificationAttribute.ShowOnProductPage = true;
                                productSpecificationAttribute.DisplayOrder = 0;
                            }
                            else
                            {
                                productSpecificationAttribute.SpecificationAttributeOptionId = s.FirstOrDefault().Id;
                                productSpecificationAttribute.CustomValue = SpecificationValue;
                                productSpecificationAttribute.AllowFiltering = false;
                                productSpecificationAttribute.ShowOnProductPage = true;
                                productSpecificationAttribute.DisplayOrder = 0;
                            }

                            if (newProduct)
                            {
                                _specificationAttributeService.InsertProductSpecificationAttribute(productSpecificationAttribute);
                            }
                            else
                            {
                                _specificationAttributeService.UpdateProductSpecificationAttribute(productSpecificationAttribute);
                            }

                        }
                        //search engine name
                        //  _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);

                        //category mappings


                        //update "HasTierPrices" and "HasDiscountsApplied" properties
                        //   _productService.UpdateHasTierPricesProperty(product);
                        //  _productService.UpdateHasDiscountsApplied(product);
                    }

                }
            }

        }

        public void SaveProductAttributes(string Attribute, string AttributeValue, Product product)
        {
            var productAttribute = _productAttributeService.GetProductAttributeByName(Attribute);

            if (productAttribute.Count > 0)
            {
                var productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                if (productAttributeMapping.Count == 0)
                {
                    var productAttributeMapping1 = new ProductAttributeMapping
                    {
                        ProductId = product.Id,
                        ProductAttributeId = productAttribute.FirstOrDefault().Id,
                        TextPrompt = null,
                        IsRequired = true,
                        AttributeControlTypeId = 1,
                        DisplayOrder = 1
                    };
                    _productAttributeService.InsertProductAttributeMapping(productAttributeMapping1);
                    productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                }


                var predefinedValues = _productAttributeService.GetPredefinedProductAttributeValueByName(AttributeValue);
                if (predefinedValues.Count == 0)
                {
                    var ppav = new PredefinedProductAttributeValue
                    {
                        ProductAttributeId = productAttribute.FirstOrDefault().Id,
                        Name = AttributeValue,
                        PriceAdjustment = 0,
                        WeightAdjustment = 0,
                        Cost = 0,
                        IsPreSelected = false,
                        DisplayOrder = 0
                    };
                    _productAttributeService.InsertPredefinedProductAttributeValue(ppav);

                    var pav = new ProductAttributeValue
                    {
                        ProductAttributeMappingId = productAttributeMapping.FirstOrDefault().Id,
                        AttributeValueType = AttributeValueType.Simple,
                        Name = ppav.Name,
                        PriceAdjustment = ppav.PriceAdjustment,
                        WeightAdjustment = ppav.WeightAdjustment,
                        Cost = ppav.Cost,
                        IsPreSelected = ppav.IsPreSelected,
                        DisplayOrder = ppav.DisplayOrder
                    };
                    _productAttributeService.InsertProductAttributeValue(pav);
                }
                else
                {
                    var t = _productAttributeService.GetProductAttributeValuesByProductAndByName(product.Id, AttributeValue);
                    if (t.Count == 0)
                    {
                        foreach (var predefinedValue in predefinedValues)
                        {
                            if (predefinedValue.Name.ToLower() == AttributeValue.ToLower())
                            {
                                var pav = new ProductAttributeValue
                                {
                                    ProductAttributeMappingId = productAttributeMapping.FirstOrDefault().Id,
                                    AttributeValueType = AttributeValueType.Simple,
                                    Name = predefinedValue.Name,
                                    PriceAdjustment = predefinedValue.PriceAdjustment,
                                    WeightAdjustment = predefinedValue.WeightAdjustment,
                                    Cost = predefinedValue.Cost,
                                    IsPreSelected = predefinedValue.IsPreSelected,
                                    DisplayOrder = predefinedValue.DisplayOrder
                                };
                                _productAttributeService.InsertProductAttributeValue(pav);
                            }
                        }
                    }
                }
            }
        }




        public virtual void ImportGiftCardsFromXlsx(Stream stream)
        {
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new[]
                {
                    "GiftCardTypeId",
                    "PurchasedWithOrderId",
                    "Amount",
                    "IsGiftCardActivated",
                    "GiftCardCouponCode",
                    "RecipientName",
                    "RecipientEmail",
                    "SenderName",
                    "SenderEmail",
                    "Message",
           
                };


                int iRow = 2;
                while (true)
                {
                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= properties.Length; i++)
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    if (allColumnsAreEmpty)
                        break;

                    int giftCardTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "GiftCardTypeId")].Value);
                    decimal amount = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Amount")].Value);
                    int purchasedWithOrderItemId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "PurchasedWithOrderId")].Value);
                    bool isGiftCardActivated = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsGiftCardActivated")].Value);
                    string giftCardCouponCode = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "GiftCardCouponCode")].Value);
                    string recipientName = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "RecipientName")].Value);
                    string recipientEmail = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "RecipientEmail")].Value);
                    string senderEmail = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SenderEmail")].Value);
                    string senderName = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SenderName")].Value);
                    string message = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Message")].Value);


                    var gc = _giftCardService.GetGiftCardByCouponCode(giftCardCouponCode);
                    bool newGc = false;
                    if (gc == null)
                    {
                        gc = new GiftCard();
                        newGc = true;
                    }

                    gc.GiftCardTypeId = giftCardTypeId;
                    gc.PurchasedWithSubscriptionOrderItemId = purchasedWithOrderItemId == 0 ? (int?)null : purchasedWithOrderItemId;
                    gc.Amount = amount;
                    gc.IsGiftCardActivated = isGiftCardActivated;
                    gc.GiftCardCouponCode = giftCardCouponCode;
                    gc.RecipientName = recipientName;
                    gc.RecipientEmail = recipientEmail;
                    gc.SenderName = senderName;
                    gc.SenderEmail = senderEmail;
                    gc.Message = message;
                    gc.IsRecipientNotified = false;
                    gc.CreatedOnUtc = DateTime.UtcNow;

                    if (newGc)
                    {
                        _giftCardService.InsertGiftCard(gc);
                    }
                    else
                    {
                        _giftCardService.UpdateGiftCard(gc);
                    }

                    //next giftcard
                    iRow++;
                }
            }



        }
        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        public virtual int ImportNewsletterSubscribersFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    string email;
                    bool isActive = true;
                    int storeId = _storeContext.CurrentStore.Id;
                    //parse
                    if (tmp.Length == 1)
                    {
                        //"email" only
                        email = tmp[0].Trim();
                    }
                    else if (tmp.Length == 2)
                    {
                        //"email" and "active" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                    }
                    else if (tmp.Length == 3)
                    {
                        //"email" and "active" and "storeId" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                        storeId = Int32.Parse(tmp[2].Trim());
                    }
                    else
                        throw new NopException("Wrong file format");

                    //import
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    }
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        public virtual int ImportStatesFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new NopException("Wrong file format");
                    
                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    bool published = Boolean.Parse(tmp[3].Trim());
                    int displayOrder = Int32.Parse(tmp[4].Trim());

                    var country = _countryService.GetCountryByTwoLetterIsoCode(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplayOrder = displayOrder;
                        _stateProvinceService.UpdateStateProvince(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder,
                        };
                        _stateProvinceService.InsertStateProvince(state);
                    }
                    count++;
                }
            }

            return count;
        }

        public virtual int ImportCitiesFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new NopException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    bool published = Boolean.Parse(tmp[3].Trim());
                    int displayOrder = Int32.Parse(tmp[4].Trim());

                    var state = _stateProvinceService.GetStateProvinceByAbbreviation(countryTwoLetterIsoCode);
                    if (state == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var cities = _cityService.GetCitysByStateProvinceId(state.Id, showHidden: true);
                    var city = cities.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (city != null)
                    {
                        city.Abbreviation = abbreviation;
                        city.Published = published;
                        city.DisplayOrder = displayOrder;
                        _cityService.UpdateCity(city);
                    }
                    else
                    {
                        city = new City
                        {
                            StateProvinceId = state.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder,
                        };
                        _cityService.InsertCity(city);
                    }
                    count++;
                }
            }

            return count;
        }

        public virtual int ImportLocalitiesFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    if (tmp.Length != 7)
                        throw new NopException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    var zipcode = tmp[3].Trim();
                    var areaname = tmp[4].Trim();
                    bool published = Boolean.Parse(tmp[5].Trim());
                    int displayOrder = Int32.Parse(tmp[6].Trim());

                    var city = _cityService.GetCityByAbbreviation(countryTwoLetterIsoCode);
                    if (city == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var localities = _localityService.GetLocalitysByCityId(city.Id, showHidden: true);
                    var locality = localities.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (locality != null)
                    {
                        locality.Abbreviation = abbreviation;
                        locality.AreaName = areaname;
                        locality.Pincode = zipcode;
                        locality.Published = published;
                        locality.DisplayOrder = displayOrder;
                        _localityService.UpdateLocality(locality);
                    }
                    else
                    {
                        locality = new Locality
                        {
                            CityId = city.Id,
                            Name = name,
                            Pincode = zipcode,
                            AreaName = areaname,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder,
                        };
                        _localityService.InsertLocality(locality);
                    }
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}
