using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class PlanOverviewModel : BaseNopEntityModel
    {
        public PlanOverviewModel()
        {
            PlanPrice = new PlanPriceModel();
            DefaultPictureModel = new PictureModel();
        }

        public string Name { get; set; }
        public string PlanCategoryProductsName { get; set; }

        public bool CurrentPlan { get; set; }
        public decimal RegistrationCharge { get; set; }
        public int MaxNoOfDeliveries { get; set; }
        public int NoOfItemsToBorrow { get; set; }
        public string Duration { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }

        public bool MarkAsNew { get; set; }

        //price
        public PlanPriceModel PlanPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //specification attributes

		#region Nested Classes

        public partial class PlanPriceModel : BaseNopModel
        {
            public string OldPrice { get; set; }
            public string Price { get; set; }
            public decimal PriceValue { get; set; }
            public string SecurityDeposit { get; set; }
            public decimal SecurityDepositValue { get; set; }
            public bool DisableBuyButton { get; set; }
            public bool DisableMyToyBoxButton { get; set; }
            public bool DisableAddToCompareListButton { get; set; }

            public bool AvailableForPreSubscription { get; set; }
            public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }

            public bool IsRental { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
        }

		#endregion
    }
}