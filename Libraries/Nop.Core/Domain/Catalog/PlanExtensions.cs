using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Plan extensions
    /// </summary>
    public static class PlanExtensions
    {
        /// <summary>
        /// Parse "required plan Ids" property
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <returns>A list of required plan IDs</returns>
        public static int[] ParseRequiredPlanIds(this Plan plan)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (String.IsNullOrEmpty(plan.RequiredPlanIds))
                return new int[0];

            var ids = new List<int>();

            foreach (var idStr in plan.RequiredPlanIds
                .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
            {
                int id;
                if (int.TryParse(idStr, out id))
                    ids.Add(id);
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a plan is available now (availability dates)
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Plan plan)
        {
            return IsAvailable(plan, DateTime.UtcNow);
        }

        /// <summary>
        /// Get a value indicating whether a plan is available now (availability dates)
        /// </summary>
        /// <param name="plan">Plan</param>
        /// <param name="dateTime">Datetime to check</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Plan plan, DateTime dateTime)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            if (plan.AvailableStartDateTimeUtc.HasValue && plan.AvailableStartDateTimeUtc.Value > dateTime)
            {
                return false;
            }

            if (plan.AvailableEndDateTimeUtc.HasValue && plan.AvailableEndDateTimeUtc.Value < dateTime)
            {
                return false;
            }

            return true;
        }
    }
}
