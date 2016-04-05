using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class PlanExtensions
    {
        /// <summary>
        /// Get plan special price
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <returns>Special price; null if plan does not have special price specified</returns>
        public static decimal? GetSpecialPrice(this Plan plan)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (!plan.SpecialPrice.HasValue)
                return null;

            //check date range
            DateTime now = DateTime.UtcNow;
            if (plan.SpecialPriceStartDateTimeUtc.HasValue)
            {
                DateTime startDate = DateTime.SpecifyKind(plan.SpecialPriceStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (startDate.CompareTo(now) > 0)
                    return null;
            }
            if (plan.SpecialPriceEndDateTimeUtc.HasValue)
            {
                DateTime endDate = DateTime.SpecifyKind(plan.SpecialPriceEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (endDate.CompareTo(now) < 0)
                    return null;
            }

            return plan.SpecialPrice.Value;
        }
        
        
        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Selected plan attributes in XML format (if specified)</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="planAttributeParser">Plan attribute parser</param>
        /// <returns>The stock message</returns>
     
        
         

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <returns>Result</returns>
        public static int[] ParseAllowedQuantities(this Plan plan)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            var result = new List<int>();
            if (!String.IsNullOrWhiteSpace(plan.AllowedQuantities))
            {
                plan.AllowedQuantities
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(qtyStr =>
                    {
                        int qty;
                        if (int.TryParse(qtyStr.Trim(), out qty))
                        {
                            result.Add(qty);
                        }
                    });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get total quantity
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="useReservedQuantity">
        /// A value indicating whether we should consider "Reserved Quantity" property 
        /// when "multiple warehouses" are used
        /// </param>
        /// <param name="warehouseId">
        /// Warehouse identifier. Used to limit result to certain warehouse.
        /// Used only with "multiple warehouses" enabled.
        /// </param>
        /// <returns>Result</returns>
    

        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        public static int GetRentalPeriods(this Plan plan,
            DateTime startDate, DateTime endDate)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (!plan.IsRental)
                return 1;

            if (startDate.CompareTo(endDate) >= 0)
                return 1;

            int totalPeriods;
            switch (plan.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                {
                    var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                    int configuredPeriodDays = plan.RentalPriceLength;
                    totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent/configuredPeriodDays));
                }
                    break;
                case RentalPricePeriod.Weeks:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        int configuredPeriodDays = 7 * plan.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }
                    break;
                case RentalPricePeriod.Months:
                    {
                        //Source: http://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates
                        var totalMonthsToRent = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
                        if (startDate.AddMonths(totalMonthsToRent) < endDate)
                        {
                            //several days added (not full month)
                            totalMonthsToRent++;
                        }
                        int configuredPeriodMonths = plan.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling((double)totalMonthsToRent / configuredPeriodMonths));
                    }
                    break;
                case RentalPricePeriod.Years:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        int configuredPeriodDays = 365 * plan.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }
                    break;
                default:
                    throw new Exception("Not supported rental period");
            }

            return totalPeriods;
        }



 

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="planAttributeParser">Plan attribute service (used when attributes are specified)</param>
        /// <returns>SKU</returns>
         

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="planAttributeParser">Plan attribute service (used when attributes are specified)</param>
        /// <returns>Manufacturer part number</returns>
       

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="planAttributeParser">Plan attribute service (used when attributes are specified)</param>
        /// <returns>GTIN</returns>
        

        /// <summary>
        /// Formats start/end date for rental plan
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        public static string FormatRentalDate(this Plan plan, DateTime date)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (!plan.IsRental)
                return null;

            return date.ToShortDateString();
        }

        /// <summary>
        /// Format base price (PAngV)
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="planPrice">Plan price (in primary currency). Pass null if you want to use a default produce price</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="measureService">Measure service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <returns>Base price</returns>
        public static string FormatBasePrice(this Plan plan, decimal? planPrice, ILocalizationService localizationService,
            IMeasureService measureService, ICurrencyService currencyService,
            IWorkContext workContext, IPriceFormatter priceFormatter)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            
            if (measureService == null)
                throw new ArgumentNullException("measureService");

            if (currencyService == null)
                throw new ArgumentNullException("currencyService");

            if (workContext == null)
                throw new ArgumentNullException("workContext");

            if (priceFormatter == null)
                throw new ArgumentNullException("priceFormatter");

            if (!plan.BasepriceEnabled)
                return null;

            var planAmount = plan.BasepriceAmount;
            //Amount in plan cannot be 0
            if (planAmount == 0)
                return null;
            var referenceAmount = plan.BasepriceBaseAmount;
            var planUnit = measureService.GetMeasureWeightById(plan.BasepriceUnitId);
            //measure weight cannot be loaded
            if (planUnit == null)
                return null;
            var referenceUnit = measureService.GetMeasureWeightById(plan.BasepriceBaseUnitId);
            //measure weight cannot be loaded
            if (referenceUnit == null)
                return null;

            planPrice = planPrice.HasValue ? planPrice.Value : plan.Price;

            decimal basePrice = planPrice.Value /
                //do not round. otherwise, it can cause issues
                measureService.ConvertWeight(planAmount, planUnit, referenceUnit, false) * 
                referenceAmount;
            decimal basePriceInCurrentCurrency = currencyService.ConvertFromPrimaryStoreCurrency(basePrice, workContext.WorkingCurrency);
            string basePriceStr = priceFormatter.FormatPrice(basePriceInCurrentCurrency, true, false);

            var result = string.Format(localizationService.GetResource("Plans.BasePrice"),
                basePriceStr, referenceAmount.ToString("G29"), referenceUnit.Name);
            return result;
        }
    }
}
