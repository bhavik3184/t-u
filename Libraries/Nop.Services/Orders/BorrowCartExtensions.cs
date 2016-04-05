using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Services.Localization;

namespace Nop.Services.SubscriptionOrders
{
    /// <summary>
    /// Represents a shopping cart
    /// </summary>
    public static class BorrowCartExtensions
    {
        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <returns>True if the shopping cart requires shipping; otherwise, false.</returns>
        public static bool RequiresShipping(this IList<BorrowCartItem> borrowCart)
        {
            foreach (var borrowCartItem in borrowCart)
                if (borrowCartItem.IsShipEnabled)
                    return true;
            return false;
        }

        /// <summary>
        /// Gets a number of product in the cart
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <returns>Result</returns>
        public static int GetTotalProducts(this IList<BorrowCartItem> borrowCart)
        {
            int result = 0;
            foreach (BorrowCartItem sci in borrowCart)
            {
                result += sci.Quantity;
            }
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether shopping cart is recurring
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <returns>Result</returns>
        public static bool IsRecurring(this IList<BorrowCartItem> borrowCart)
        {
            foreach (BorrowCartItem sci in borrowCart)
            {
                var product = sci.Product;
                if (product != null && product.IsRecurring)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Get a recurring cycle information
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="cycleLength">Cycle length</param>
        /// <param name="cyclePeriod">Cycle period</param>
        /// <param name="totalCycles">Total cycles</param>
        /// <returns>Error (if exists); otherwise, empty string</returns>
        public static string GetRecurringCycleInfo(this IList<BorrowCartItem> borrowCart,
            ILocalizationService localizationService,
            out int cycleLength, out RecurringProductCyclePeriod cyclePeriod, out int totalCycles)
        {
            cycleLength = 0;
            cyclePeriod = 0;
            totalCycles = 0;

            int? _cycleLength = null;
            RecurringProductCyclePeriod? _cyclePeriod = null;
            int? _totalCycles = null;

            foreach (var sci in borrowCart)
            {
                var product= sci.Product;
                if (product == null)
                {
                    throw new NopException(string.Format("Product (Id={0}) cannot be loaded", sci.ProductId));
                }

                if (product.IsRecurring)
                {
                    string conflictError = localizationService.GetResource("BorrowCart.ConflictingShipmentSchedules");

                    //cycle length
                    if (_cycleLength.HasValue && _cycleLength.Value != product.RecurringCycleLength)
                        return conflictError;
                    _cycleLength = product.RecurringCycleLength;

                    //cycle period
                    if (_cyclePeriod.HasValue && _cyclePeriod.Value != product.RecurringCyclePeriod)
                        return conflictError;
                    _cyclePeriod = product.RecurringCyclePeriod;

                    //total cycles
                    if (_totalCycles.HasValue && _totalCycles.Value != product.RecurringTotalCycles)
                        return conflictError;
                    _totalCycles = product.RecurringTotalCycles;
                }
            }

            if (_cycleLength.HasValue && _cyclePeriod.HasValue && _totalCycles.HasValue)
            {
                cycleLength = _cycleLength.Value;
                cyclePeriod = _cyclePeriod.Value;
                totalCycles = _totalCycles.Value;
            }

            return "";
        }

        /// <summary>
        /// Get customer of shopping cart
        /// </summary>
        /// <param name="borrowCart">Shopping cart</param>
        /// <returns>Customer of shopping cart</returns>
        public static Customer GetCustomer(this IList<BorrowCartItem> borrowCart)
        {
            if (borrowCart.Count == 0)
                return null;

            return borrowCart[0].Customer;
        }

        public static IEnumerable<BorrowCartItem> LimitPerStore(this IEnumerable<BorrowCartItem> cart, int storeId)
        {
            //simply replace the following code with "return cart"
            //if you want to share shopping carts between stores

            return cart.Where(x => x.StoreId == storeId);
        }
    }
}
