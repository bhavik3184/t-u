using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Services.SubscriptionOrders;

namespace Nop.Web.Extensions
{
    //here we have some methods shared between controllers
    public static class ControllerExtensions
    {
        public static IList<ProductSpecificationModel> PrepareProductSpecificationModel(this Controller controller,
            IWorkContext workContext,
            ISpecificationAttributeService specificationAttributeService,
            ICacheManager cacheManager,
            Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_SPECS_MODEL_KEY, product.Id, workContext.WorkingLanguage.Id);
            return cacheManager.Get(cacheKey, () =>
                specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
                .Select(psa =>
                {
                    var m = new ProductSpecificationModel
                    {
                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                        SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                    };

                    switch (psa.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                            break;
                        case SpecificationAttributeType.CustomText:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            m.ValueRaw = psa.CustomValue;
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
                            break;
                        default:
                            break;
                    }
                    return m;
                }).ToList()
            );
        }

        public static IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(this Controller controller,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICategoryService categoryService,
            IProductService productService,
            ISpecificationAttributeService specificationAttributeService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            CatalogSettings catalogSettings,
            MediaSettings mediaSettings,
            IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = product.GetLocalized(x => x.Name),
                    ShortDescription = product.GetLocalized(x => x.ShortDescription),
                    FullDescription = product.GetLocalized(x => x.FullDescription),
                    SeName = product.GetSeName(),
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };
                //price
                if (preparePriceModel)
                {
                    #region Prepare product price

                    var priceModel = new ProductOverviewModel.ProductPriceModel
                    {
                        ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
                    };

                    switch (product.ProductType)
                    {
                        case ProductType.GroupedProduct:
                            {
                                #region Grouped product

                                var associatedProducts = productService.GetAssociatedProducts(product.Id, storeContext.CurrentStore.Id);

                                switch (associatedProducts.Count)
                                {
                                    case 0:
                                        {
                                            //no associated products
                                            //priceModel.DisableBuyButton = true;
                                            //priceModel.DisableMyToyBoxButton = true;
                                            //compare products
                                            priceModel.DisableAddToCompareListButton = !catalogSettings.CompareProductsEnabled;
                                            //priceModel.AvailableForPreOrder = false;
                                        }
                                        break;
                                    default:
                                        {
                                            //we have at least one associated product
                                            //priceModel.DisableBuyButton = true;
                                            //priceModel.DisableMyToyBoxButton = true;
                                            //compare products
                                            priceModel.DisableAddToCompareListButton = !catalogSettings.CompareProductsEnabled;
                                            //priceModel.AvailableForPreOrder = false;

                                            if (permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                            {
                                                //find a minimum possible price
                                                decimal? minPossiblePrice = null;
                                                Product minPriceProduct = null;
                                                foreach (var associatedProduct in associatedProducts)
                                                {
                                                    //calculate for the maximum quantity (in case if we have tier prices)
                                                    var tmpPrice = priceCalculationService.GetFinalPrice(associatedProduct,
                                                        workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
                                                    if (!minPossiblePrice.HasValue || tmpPrice < minPossiblePrice.Value)
                                                    {
                                                        minPriceProduct = associatedProduct;
                                                        minPossiblePrice = tmpPrice;
                                                    }
                                                }
                                                if (minPriceProduct != null && !minPriceProduct.CustomerEntersPrice)
                                                {
                                                    if (minPriceProduct.CallForPrice)
                                                    {
                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = localizationService.GetResource("Products.CallForPrice");
                                                    }
                                                    else if (minPossiblePrice.HasValue)
                                                    {
                                                        //calculate prices
                                                        decimal taxRate;
                                                        decimal finalPriceBase = taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out taxRate);
                                                        decimal finalPrice = currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workContext.WorkingCurrency);

                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = String.Format(localizationService.GetResource("Products.PriceRangeFrom"), priceFormatter.FormatPrice(finalPrice));
                                                        priceModel.PriceValue = finalPrice;
                                                    }
                                                    else
                                                    {
                                                        //Actually it's not possible (we presume that minimalPrice always has a value)
                                                        //We never should get here
                                                        Debug.WriteLine("Cannot calculate minPrice for product #{0}", product.Id);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //hide prices
                                                priceModel.OldPrice = null;
                                                priceModel.Price = null;
                                            }
                                        }
                                        break;
                                }

                                #endregion
                            }
                            break;
                        case ProductType.SimpleProduct:
                        default:
                            {
                                #region Simple product

                                //add to cart button
                                priceModel.DisableBuyButton = product.DisableBuyButton ||
                                    !permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart) ||
                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                //add to mytoybox button
                                priceModel.DisableMyToyBoxButton = product.DisableMyToyBoxButton ||
                                    !permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox) ||
                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);
                                //compare products
                                priceModel.DisableAddToCompareListButton = !catalogSettings.CompareProductsEnabled;

                                //rental
                                priceModel.IsRental = product.IsRental;

                                //pre-order
                                if (product.AvailableForPreOrder)
                                {
                                    priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                        product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                                    priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
                                }

                                //prices
                                if (permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                {
                                    if (!product.CustomerEntersPrice)
                                    {
                                        if (product.CallForPrice)
                                        {
                                            //call for price
                                            priceModel.OldPrice = null;
                                            priceModel.Price = localizationService.GetResource("Products.CallForPrice");
                                        }
                                        else
                                        {
                                            //prices

                                            //calculate for the maximum quantity (in case if we have tier prices)
                                            decimal minPossiblePrice = priceCalculationService.GetFinalPrice(product,
                                                workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);

                                            decimal taxRate;
                                            decimal oldPriceBase = taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                                            decimal finalPriceBase = taxService.GetProductPrice(product, minPossiblePrice, out taxRate);

                                            decimal oldPrice = currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, workContext.WorkingCurrency);
                                            decimal finalPrice = currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workContext.WorkingCurrency);

                                            //do we have tier prices configured?
                                            var tierPrices = new List<TierPrice>();
                                            if (product.HasTierPrices)
                                            {
                                                tierPrices.AddRange(product.TierPrices
                                                    .OrderBy(tp => tp.Quantity)
                                                    .ToList()
                                                    .FilterByStore(storeContext.CurrentStore.Id)
                                                    .FilterForCustomer(workContext.CurrentCustomer)
                                                    .RemoveDuplicatedQuantities());
                                            }
                                            //When there is just one tier (with  qty 1), 
                                            //there are no actual savings in the list.
                                            bool displayFromMessage = tierPrices.Count > 0 &&
                                                !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                                            if (displayFromMessage)
                                            {
                                                priceModel.OldPrice = null;
                                                priceModel.Price = String.Format(localizationService.GetResource("Products.PriceRangeFrom"), priceFormatter.FormatPrice(finalPrice));
                                                priceModel.PriceValue = finalPrice;
                                            }
                                            else
                                            {
                                                if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                                {
                                                    priceModel.OldPrice = priceFormatter.FormatPrice(oldPrice);
                                                    priceModel.Price = priceFormatter.FormatPrice(finalPrice);
                                                    priceModel.PriceValue = finalPrice;
                                                }
                                                else
                                                {
                                                    priceModel.OldPrice = null;
                                                    priceModel.Price = priceFormatter.FormatPrice(finalPrice);
                                                    priceModel.PriceValue = finalPrice;
                                                }
                                            }
                                            if (product.IsRental)
                                            {
                                                //rental product
                                                priceModel.OldPrice = priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
                                                priceModel.Price = priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
                                            }


                                            //property for German market
                                            //we display tax/shipping info only with "shipping enabled" for this product
                                            //we also ensure this it's not free shipping
                                            priceModel.DisplayTaxShippingInfo = catalogSettings.DisplayTaxShippingInfoProductBoxes
                                                && product.IsShipEnabled &&
                                                !product.IsFreeShipping;
                                        }
                                    }
                                }
                                else
                                {
                                    //hide prices
                                    priceModel.OldPrice = null;
                                    priceModel.Price = null;
                                }

                                #endregion
                            }
                            break;
                    }

                    model.ProductPrice = priceModel;

                    #endregion
                }

                //picture
                if (preparePictureModel)
                {
                    #region Prepare product picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : mediaSettings.ProductThumbPictureSize;
                    //prepare picture model
                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.Id, pictureSize, true, workContext.WorkingLanguage.Id, webHelper.IsCurrentConnectionSecured(), storeContext.CurrentStore.Id);
                    model.DefaultPictureModel = cacheManager.Get(defaultProductPictureCacheKey, () =>
                    {
                        var picture = pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = pictureService.GetPictureUrl(picture)
                        };
                        //"title" attribute
                        pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                            picture.TitleAttribute :
                            string.Format(localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
                        //"alt" attribute
                        pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                            picture.AltAttribute :
                            string.Format(localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);
                        
                        return pictureModel;
                    });

                    #endregion
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.SpecificationAttributeModels = PrepareProductSpecificationModel(controller, workContext,
                         specificationAttributeService, cacheManager, product);
                }

                //reviews
                model.ReviewOverviewModel = new ProductReviewOverviewModel
                {
                    ProductId = product.Id,
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews,
                    AllowCustomerReviews = product.AllowCustomerReviews
                };

                models.Add(model);
            }
            return models;
        }

        public static IEnumerable<PlanOverviewModel> PreparePlanOverviewModels(this Controller controller,
          IWorkContext workContext,
          IStoreContext storeContext,
          ICategoryService categoryService,
          IPlanService planService,
          ISubscriptionOrderService subscriptionOrderService,
          IPriceCalculationService priceCalculationService,
          IPriceFormatter priceFormatter,
          IPermissionService permissionService,
          ILocalizationService localizationService,
          ITaxService taxService,
          ICurrencyService currencyService,
          IPictureService pictureService,
          IWebHelper webHelper,
          ICacheManager cacheManager,
          CatalogSettings catalogSettings,
          MediaSettings mediaSettings,
          IEnumerable<Plan> plans,
          bool preparePriceModel = true, bool preparePictureModel = true,
          int? planThumbPictureSize = null,
          bool forceRedirectionAfterAddingToCart = false)
        {
            if (plans == null)
                throw new ArgumentNullException("plans");

            var models = new List<PlanOverviewModel>();
            var customer = workContext.CurrentCustomer;
            var currentorder = subscriptionOrderService.GetCurrentSubscribedOrder(customer.Id);

            foreach (var plan in plans)
            {
                var model = new PlanOverviewModel
                {
                    Id = plan.Id,
                    Name = plan.GetLocalized(x => x.Name),
                    ShortDescription = plan.GetLocalized(x => x.ShortDescription),
                    FullDescription = plan.GetLocalized(x => x.FullDescription),
                    SeName = plan.GetSeName(),
                    MaxNoOfDeliveries = plan.MaxNoOfDeliveries,
                    NoOfItemsToBorrow = plan.NoOfItemsToBorrow,
                    Duration = plan.RentalPriceLength.ToString() + " " + (RentalPricePeriod?)plan.RentalPricePeriodId,
                    MarkAsNew = plan.MarkAsNew &&
                        (!plan.MarkAsNewStartDateTimeUtc.HasValue || plan.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!plan.MarkAsNewEndDateTimeUtc.HasValue || plan.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                if (currentorder != null) { 
                    foreach (var SubscriptionOrderItem in currentorder.SubscriptionOrderItems)
                    {
                        if (plan.Id == SubscriptionOrderItem.PlanId) {
                            model.CurrentPlan = true;
                        }

                    }
                }
                var pcat = categoryService.GetPlanCategoriesByPlanId(plan.Id);

                foreach (PlanCategory pc in pcat)
                {
                    model.PlanCategoryProductsName = model.PlanCategoryProductsName + "<br>" + pc.Quantity + " " + pc.Category.Name;
                }

                //price
                if (preparePriceModel)
                {
                    #region Prepare plan price

                    var priceModel = new PlanOverviewModel.PlanPriceModel
                    {
                        ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
                    };

                    switch (plan.PlanType)
                    {

                        case PlanType.SimplePlan:
                        default:
                            {
                                #region Simple plan

                                //add to cart button
                                priceModel.DisableBuyButton = plan.DisableBuyButton ||
                                    !permissionService.Authorize(StandardPermissionProvider.EnableBorrowCart) ||
                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                //add to mytoybox button
                                priceModel.DisableMyToyBoxButton = plan.DisableMyToyBoxButton ||
                                    !permissionService.Authorize(StandardPermissionProvider.EnableMyToyBox) ||
                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);
                                //compare plans
                                priceModel.DisableAddToCompareListButton = true;

                                //rental
                                priceModel.IsRental = plan.IsRental;

                                //pre-order
                                if (plan.AvailableForPreSubscription)
                                {
                                    priceModel.AvailableForPreSubscription = !plan.PreSubscriptionAvailabilityStartDateTimeUtc.HasValue ||
                                        plan.PreSubscriptionAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                                    priceModel.PreSubscriptionAvailabilityStartDateTimeUtc = plan.PreSubscriptionAvailabilityStartDateTimeUtc;
                                }

                                //prices
                                if (permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                {
                                    if (!plan.CustomerEntersPrice)
                                    {
                                        if (plan.CallForPrice)
                                        {
                                            //call for price
                                            priceModel.OldPrice = null;
                                            priceModel.Price = localizationService.GetResource("Plans.CallForPrice");
                                        }
                                        else
                                        {
                                            //prices

                                            //calculate for the maximum quantity (in case if we have tier prices)
                                            decimal minPossiblePrice = priceCalculationService.GetFinalPrice(plan,
                                                workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);

                                            decimal taxRate;
                                            decimal oldPriceBase = taxService.GetPlanPrice(plan, plan.OldPrice, out taxRate);
                                            decimal finalPriceBase = taxService.GetPlanPrice(plan, minPossiblePrice, out taxRate);

                                            decimal oldPrice = currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, workContext.WorkingCurrency);
                                            decimal finalPrice = currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workContext.WorkingCurrency);

                                            //do we have tier prices configured?
                                            var tierPrices = new List<TierPrice>();
                                            if (plan.HasTierPrices)
                                            {
                                                tierPrices.AddRange(plan.TierPrices
                                                    .OrderBy(tp => tp.Quantity)
                                                    .ToList()
                                                    .FilterByStore(storeContext.CurrentStore.Id)
                                                    .FilterForCustomer(workContext.CurrentCustomer)
                                                    .RemoveDuplicatedQuantities());
                                            }
                                            //When there is just one tier (with  qty 1), 
                                            //there are no actual savings in the list.
                                            bool displayFromMessage = tierPrices.Count > 0 &&
                                                !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                                            if (displayFromMessage)
                                            {
                                                priceModel.OldPrice = null;
                                                priceModel.Price = String.Format(localizationService.GetResource("Plans.PriceRangeFrom"), priceFormatter.FormatPrice(finalPrice));
                                                priceModel.PriceValue = finalPrice;
                                            }
                                            else
                                            {
                                                if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                                {
                                                    priceModel.OldPrice = priceFormatter.FormatPrice(oldPrice);
                                                    priceModel.Price = priceFormatter.FormatPrice(finalPrice);
                                                    priceModel.PriceValue = finalPrice;
                                                }
                                                else
                                                {
                                                    priceModel.OldPrice = null;
                                                    priceModel.Price = priceFormatter.FormatPrice(finalPrice);
                                                    priceModel.PriceValue = finalPrice;
                                                }
                                            }
                                            //if (plan.IsRental)
                                            //{
                                            //    //rental plan
                                            //    priceModel.OldPrice = "";
                                            //    priceModel.Price = "";
                                            //}

                                            priceModel.SecurityDeposit = priceFormatter.FormatPrice(plan.SecurityDeposit);
                                            priceModel.SecurityDepositValue = plan.SecurityDeposit;
                                            //property for German market
                                            //we display tax/shipping info only with "shipping enabled" for this plan
                                            //we also ensure this it's not free shipping
                                            priceModel.DisplayTaxShippingInfo = catalogSettings.DisplayTaxShippingInfoProductBoxes
                                                && plan.IsShipEnabled &&
                                                !plan.IsFreeShipping;
                                        }
                                    }
                                }
                                else
                                {
                                    //hide prices
                                    priceModel.OldPrice = null;
                                    priceModel.Price = null;
                                }

                                #endregion
                            }
                            break;
                    }

                    model.PlanPrice = priceModel;

                    #endregion
                }

                //picture
                if (preparePictureModel)
                {
                    #region Prepare plan picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSize = planThumbPictureSize.HasValue ? planThumbPictureSize.Value : mediaSettings.ProductThumbPictureSize;
                    //prepare picture model
                    var defaultPlanPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, plan.Id, pictureSize, true, workContext.WorkingLanguage.Id, webHelper.IsCurrentConnectionSecured(), storeContext.CurrentStore.Id);
                    model.DefaultPictureModel = cacheManager.Get(defaultPlanPictureCacheKey, () =>
                    {
                        var picture = pictureService.GetPicturesByProductId(plan.Id, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = pictureService.GetPictureUrl(picture)
                        };
                        //"title" attribute
                        pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                            picture.TitleAttribute :
                            string.Format(localizationService.GetResource("Media.Plan.ImageLinkTitleFormat"), model.Name);
                        //"alt" attribute
                        pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                            picture.AltAttribute :
                            string.Format(localizationService.GetResource("Media.Plan.ImageAlternateTextFormat"), model.Name);

                        return pictureModel;
                    });

                    #endregion
                }


                models.Add(model);
            }
            return models;
        }
    }
}