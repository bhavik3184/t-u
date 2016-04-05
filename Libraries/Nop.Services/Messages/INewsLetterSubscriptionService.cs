using System;
using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Newsletter subscription service interface
    /// </summary>
    public partial interface INewsLetterSubscriptionService
    {
        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionOrder">NewsLetter subscription</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        void InsertNewsLetterSubscription(NewsLetterSubscription newsLetterSubscriptionOrder, bool publishSubscriptionOrderEvents = true);

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionOrder">NewsLetter subscription</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        void UpdateNewsLetterSubscription(NewsLetterSubscription newsLetterSubscriptionOrder, bool publishSubscriptionOrderEvents = true);

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionOrder">NewsLetter subscription</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        void DeleteNewsLetterSubscription(NewsLetterSubscription newsLetterSubscriptionOrder, bool publishSubscriptionOrderEvents = true);

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionOrderId">The newsletter subscription identifier</param>
        /// <returns>NewsLetter subscription</returns>
        NewsLetterSubscription GetNewsLetterSubscriptionById(int newsLetterSubscriptionOrderId);

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionOrderGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetter subscription</returns>
        NewsLetterSubscription GetNewsLetterSubscriptionByGuid(Guid newsLetterSubscriptionOrderGuid);

        /// <summary>
        /// Gets a newsletter subscription by email and store ID
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>NewsLetter subscription</returns>
        NewsLetterSubscription GetNewsLetterSubscriptionByEmailAndStoreId(string email, int storeId);

        /// <summary>
        /// Gets the newsletter subscription list
        /// </summary>
        /// <param name="email">Email to search or string. Empty to load all records.</param>
        /// <param name="storeId">Store identifier. 0 to load all records.</param>
        /// <param name="isActive">Value indicating whether subscriber record should be active or not; null to load all records</param>
        /// <param name="customerRoleId">Customer role identifier. Used to filter subscribers by customer role. 0 to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>NewsLetterSubscription entities</returns>
        IPagedList<NewsLetterSubscription> GetAllNewsLetterSubscriptions(string email = null,
            int storeId = 0, bool? isActive = null, int customerRoleId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
