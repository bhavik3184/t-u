using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Copy Plan service
    /// </summary>
    public partial class CopyPlanService : ICopyPlanService
    {
        #region Fields

        private readonly IPlanService _planService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IMembershipCategoryService _membershipCategoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CopyPlanService(IPlanService planService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, 
            IPictureService pictureService,
            ICategoryService categoryService, 
            IMembershipCategoryService membershipCategoryService, 
            IManufacturerService manufacturerService,
            ISpecificationAttributeService specificationAttributeService,
            IDownloadService downloadService,
            IUrlRecordService urlRecordService, 
            IStoreMappingService storeMappingService)
        {
            this._planService = planService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._categoryService = categoryService;
            this._membershipCategoryService = membershipCategoryService;
            this._manufacturerService = manufacturerService;
            this._specificationAttributeService = specificationAttributeService;
            this._downloadService = downloadService;
            this._urlRecordService = urlRecordService;
            this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a copy of plan with all depended data
        /// </summary>
        /// <param name="plan">The plan to copy</param>
        /// <param name="newName">The name of plan duplicate</param>
        /// <param name="isPublished">A value indicating whether the plan duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the plan images should be copied</param>
        /// <param name="copyAssociatedPlans">A value indicating whether the copy associated plans</param>
        /// <returns>Plan copy</returns>
        public virtual Plan CopyPlan(Plan plan, string newName,
            bool isPublished = true, bool copyImages = true, bool copyAssociatedPlans = true)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (String.IsNullOrEmpty(newName))
                throw new ArgumentException("Plan name is required");

            //plan download & sample download
            int downloadId = plan.DownloadId;
            int sampleDownloadId = plan.SampleDownloadId;
            if (plan.IsDownload)
            {
                var download = _downloadService.GetDownloadById(plan.DownloadId);
                if (download != null)
                {
                    var downloadCopy = new Download
                    {
                        DownloadGuid = Guid.NewGuid(),
                        UseDownloadUrl = download.UseDownloadUrl,
                        DownloadUrl = download.DownloadUrl,
                        DownloadBinary = download.DownloadBinary,
                        ContentType = download.ContentType,
                        Filename = download.Filename,
                        Extension = download.Extension,
                        IsNew = download.IsNew,
                    };
                    _downloadService.InsertDownload(downloadCopy);
                    downloadId = downloadCopy.Id;
                }

                if (plan.HasSampleDownload)
                {
                    var sampleDownload = _downloadService.GetDownloadById(plan.SampleDownloadId);
                    if (sampleDownload != null)
                    {
                        var sampleDownloadCopy = new Download
                        {
                            DownloadGuid = Guid.NewGuid(),
                            UseDownloadUrl = sampleDownload.UseDownloadUrl,
                            DownloadUrl = sampleDownload.DownloadUrl,
                            DownloadBinary = sampleDownload.DownloadBinary,
                            ContentType = sampleDownload.ContentType,
                            Filename = sampleDownload.Filename,
                            Extension = sampleDownload.Extension,
                            IsNew = sampleDownload.IsNew
                        };
                        _downloadService.InsertDownload(sampleDownloadCopy);
                        sampleDownloadId = sampleDownloadCopy.Id;
                    }
                }
            }

            // plan
            var planCopy = new Plan
            {
                PlanTypeId = plan.PlanTypeId,
                ParentGroupedPlanId = plan.ParentGroupedPlanId,
                VisibleIndividually = plan.VisibleIndividually,
                Name = newName,
                ShortDescription = plan.ShortDescription,
                FullDescription = plan.FullDescription,
                VendorId = plan.VendorId,
                PlanTemplateId = plan.PlanTemplateId,
                AdminComment = plan.AdminComment,
                ShowOnHomePage = plan.ShowOnHomePage,
                MetaKeywords = plan.MetaKeywords,
                MetaDescription = plan.MetaDescription,
                MetaTitle = plan.MetaTitle,
                AllowCustomerReviews = plan.AllowCustomerReviews,
                LimitedToStores = plan.LimitedToStores,
                Sku = plan.Sku,
                ManufacturerPartNumber = plan.ManufacturerPartNumber,
                Gtin = plan.Gtin,
                IsGiftCard = plan.IsGiftCard,
                GiftCardType = plan.GiftCardType,
                OverriddenGiftCardAmount = plan.OverriddenGiftCardAmount,
                RequireOtherPlans = plan.RequireOtherPlans,
                RequiredPlanIds = plan.RequiredPlanIds,
                AutomaticallyAddRequiredPlans = plan.AutomaticallyAddRequiredPlans,
                IsDownload = plan.IsDownload,
                DownloadId = downloadId,
                UnlimitedDownloads = plan.UnlimitedDownloads,
                MaxNumberOfDownloads = plan.MaxNumberOfDownloads,
                DownloadExpirationDays = plan.DownloadExpirationDays,
                DownloadActivationType = plan.DownloadActivationType,
                HasSampleDownload = plan.HasSampleDownload,
                SampleDownloadId = sampleDownloadId,
                HasUserAgreement = plan.HasUserAgreement,
                UserAgreementText = plan.UserAgreementText,
                IsRecurring = plan.IsRecurring,
                RecurringCycleLength = plan.RecurringCycleLength,
                RecurringCyclePeriod = plan.RecurringCyclePeriod,
                RecurringTotalCycles = plan.RecurringTotalCycles,
                IsRental = plan.IsRental,
                RentalPriceLength = plan.RentalPriceLength,
                RentalPricePeriod = plan.RentalPricePeriod,
                IsShipEnabled = plan.IsShipEnabled,
                IsFreeShipping = plan.IsFreeShipping,
                ShipSeparately = plan.ShipSeparately,
                AdditionalShippingCharge = plan.AdditionalShippingCharge,
                DeliveryDateId = plan.DeliveryDateId,
                IsTaxExempt = plan.IsTaxExempt,
                TaxCategoryId = plan.TaxCategoryId,
                IsTelecommunicationsOrBroadcastingOrElectronicServices = plan.IsTelecommunicationsOrBroadcastingOrElectronicServices,
                ManageInventoryMethod = plan.ManageInventoryMethod,
                UseMultipleWarehouses = plan.UseMultipleWarehouses,
                WarehouseId = plan.WarehouseId,
                StockQuantity = plan.StockQuantity,
                DisplayStockAvailability = plan.DisplayStockAvailability,
                DisplayStockQuantity = plan.DisplayStockQuantity,
                MinStockQuantity = plan.MinStockQuantity,
                LowStockActivityId = plan.LowStockActivityId,
                NotifyAdminForQuantityBelow = plan.NotifyAdminForQuantityBelow,
                BackorderMode = plan.BackorderMode,
                AllowBackInStockSubscriptions = plan.AllowBackInStockSubscriptions,
                SubscriptionMinimumQuantity = plan.SubscriptionMinimumQuantity,
                SubscriptionMaximumQuantity = plan.SubscriptionMaximumQuantity,
                AllowedQuantities = plan.AllowedQuantities,
                AllowAddingOnlyExistingAttributeCombinations = plan.AllowAddingOnlyExistingAttributeCombinations,
                DisableBuyButton = plan.DisableBuyButton,
                DisableMyToyBoxButton = plan.DisableMyToyBoxButton,
                AvailableForPreSubscription = plan.AvailableForPreSubscription,
                PreSubscriptionAvailabilityStartDateTimeUtc = plan.PreSubscriptionAvailabilityStartDateTimeUtc,
                CallForPrice = plan.CallForPrice,
                Price = plan.Price,
                OldPrice = plan.OldPrice,
                PlanCost = plan.PlanCost,
                SpecialPrice = plan.SpecialPrice,
                SpecialPriceStartDateTimeUtc = plan.SpecialPriceStartDateTimeUtc,
                SpecialPriceEndDateTimeUtc = plan.SpecialPriceEndDateTimeUtc,
                CustomerEntersPrice = plan.CustomerEntersPrice,
                MinimumCustomerEnteredPrice = plan.MinimumCustomerEnteredPrice,
                MaximumCustomerEnteredPrice = plan.MaximumCustomerEnteredPrice,
                BasepriceEnabled = plan.BasepriceEnabled,
                BasepriceAmount = plan.BasepriceAmount,
                BasepriceUnitId = plan.BasepriceUnitId,
                BasepriceBaseAmount = plan.BasepriceBaseAmount,
                BasepriceBaseUnitId = plan.BasepriceBaseUnitId,
                MarkAsNew = plan.MarkAsNew,
                MarkAsNewStartDateTimeUtc = plan.MarkAsNewStartDateTimeUtc,
                MarkAsNewEndDateTimeUtc = plan.MarkAsNewEndDateTimeUtc,
                NoOfItemsToBorrow = plan.NoOfItemsToBorrow,
                RecurringCyclePeriodId = plan.RecurringCyclePeriodId,
                RegistrationCharge = plan.RegistrationCharge,
                SecurityDeposit = plan.SecurityDeposit,
                MaxNoOfDeliveries = plan.MaxNoOfDeliveries,
                Weight = plan.Weight,
                Length = plan.Length,
                Width = plan.Width,
                Height = plan.Height,
                AvailableStartDateTimeUtc = plan.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = plan.AvailableEndDateTimeUtc,
                DisplayOrder = plan.DisplayOrder,
                Published = isPublished,
                Deleted = plan.Deleted,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            //validate search engine name
            _planService.InsertPlan(planCopy);

            //search engine name
            _urlRecordService.SaveSlug(planCopy, planCopy.ValidateSeName("", planCopy.Name, true), 0);

            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var name = plan.GetLocalized(x => x.Name, lang.Id, false, false);
                if (!String.IsNullOrEmpty(name))
                    _localizedEntityService.SaveLocalizedValue(planCopy, x => x.Name, name, lang.Id);

                var shortDescription = plan.GetLocalized(x => x.ShortDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(shortDescription))
                    _localizedEntityService.SaveLocalizedValue(planCopy, x => x.ShortDescription, shortDescription, lang.Id);

                var fullDescription = plan.GetLocalized(x => x.FullDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(fullDescription))
                    _localizedEntityService.SaveLocalizedValue(planCopy, x => x.FullDescription, fullDescription, lang.Id);

                var metaKeywords = plan.GetLocalized(x => x.MetaKeywords, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaKeywords))
                    _localizedEntityService.SaveLocalizedValue(planCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

                var metaDescription = plan.GetLocalized(x => x.MetaDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaDescription))
                    _localizedEntityService.SaveLocalizedValue(planCopy, x => x.MetaDescription, metaDescription, lang.Id);

                var metaTitle = plan.GetLocalized(x => x.MetaTitle, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaTitle))
                    _localizedEntityService.SaveLocalizedValue(planCopy, x => x.MetaTitle, metaTitle, lang.Id);

                //search engine name
                _urlRecordService.SaveSlug(planCopy, planCopy.ValidateSeName("", name, false), lang.Id);
            }

            //plan tags
          

            //plan pictures
            //variable to store original and new picture identifiers
            var originalNewPictureIdentifiers = new Dictionary<int, int>();
            if (copyImages)
            {
                foreach (var planPicture in plan.PlanPictures)
                {
                    var picture = planPicture.Picture;
                    var pictureCopy = _pictureService.InsertPicture(
                        _pictureService.LoadPictureBinary(picture),
                        picture.MimeType,
                        _pictureService.GetPictureSeName(newName),
                        picture.AltAttribute,
                        picture.TitleAttribute);
                    _planService.InsertPlanPicture(new PlanPicture
                    {
                        PlanId = planCopy.Id,
                        PictureId = pictureCopy.Id,
                        DisplayOrder = planPicture.DisplayOrder
                    });
                    originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
                }
            }


            foreach (var planCategory in plan.PlanCategories)
            {
                var planCategoryCopy = new PlanCategory
                {
                    PlanId = planCopy.Id,
                    CategoryId = planCategory.CategoryId,
                    Quantity = planCategory.Quantity,
                    MyToyBoxQuantity = planCategory.MyToyBoxQuantity,
                    DisplayOrder = planCategory.DisplayOrder
                };

                _categoryService.InsertPlanCategory(planCategoryCopy);
            }

            // plan <-> categories mappings
            foreach (var planMembershipCategory in plan.PlanMembershipCategories)
            {
                var planMembershipCategoryCopy = new PlanMembershipCategory
                {
                    PlanId = planCopy.Id,
                    MembershipCategoryId = planMembershipCategory.MembershipCategoryId,
                    DisplayOrder = planMembershipCategory.DisplayOrder
                };

                _membershipCategoryService.InsertPlanMembershipCategory(planMembershipCategoryCopy);
            }

            // plan <-> manufacturers mappings
                  

            //tier prices
            

            // plan <-> discounts mapping
            foreach (var discount in plan.AppliedDiscounts)
            {
                planCopy.AppliedDiscounts.Add(discount);
                _planService.UpdatePlan(planCopy);
            }


            //update "HasTierPrices" and "HasDiscountsApplied" properties
            _planService.UpdateHasTierPricesProperty(planCopy);
            _planService.UpdateHasDiscountsApplied(planCopy);


           
            return planCopy;
        }

        #endregion
    }
}
