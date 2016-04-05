using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Newsletter subscription service
    /// </summary>
    public class NewsLetterSubscriptionService : INewsLetterSubscriptionService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IDbContext _context;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public NewsLetterSubscriptionService(IDbContext context,
            IRepository<NewsLetterSubscription> subscriptionRepository,
            IRepository<Customer> customerRepository,
            IEventPublisher eventPublisher,
            ICustomerService customerService)
        {
            this._context = context;
            this._subscriptionRepository = subscriptionRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
            this._customerService = customerService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Publishes the subscription event.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="isSubscribe">if set to <c>true</c> [is subscribe].</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        private void PublishSubscriptionOrderEvent(string email, bool isSubscribe, bool publishSubscriptionOrderEvents)
        {
            if (publishSubscriptionOrderEvents)
            {
                if (isSubscribe)
                {
                    _eventPublisher.PublishNewsletterSubscribe(email);
                }
                else
                {
                    _eventPublisher.PublishNewsletterUnsubscribe(email);
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionOrder">NewsLetter subscription</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual void InsertNewsLetterSubscription(NewsLetterSubscription newsLetterSubscriptionOrder, bool publishSubscriptionOrderEvents = true)
        {
            if (newsLetterSubscriptionOrder == null)
            {
                throw new ArgumentNullException("newsLetterSubscriptionOrder");
            }

            //Handle e-mail
            newsLetterSubscriptionOrder.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscriptionOrder.Email);

            //Persist
            _subscriptionRepository.Insert(newsLetterSubscriptionOrder);

            //Publish the subscription event 
            if (newsLetterSubscriptionOrder.Active)
            {
                PublishSubscriptionOrderEvent(newsLetterSubscriptionOrder.Email, true, publishSubscriptionOrderEvents);
            }

            //Publish event
            _eventPublisher.EntityInserted(newsLetterSubscriptionOrder);
        }

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionOrder">NewsLetter subscription</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual void UpdateNewsLetterSubscription(NewsLetterSubscription newsLetterSubscriptionOrder, bool publishSubscriptionOrderEvents = true)
        {
            if (newsLetterSubscriptionOrder == null)
            {
                throw new ArgumentNullException("newsLetterSubscriptionOrder");
            }

            //Handle e-mail
            newsLetterSubscriptionOrder.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscriptionOrder.Email);

            //Get original subscription record
            var originalSubscriptionOrder = _context.LoadOriginalCopy(newsLetterSubscriptionOrder);

            //Persist
            _subscriptionRepository.Update(newsLetterSubscriptionOrder);

            //Publish the subscription event 
            if ((originalSubscriptionOrder.Active == false && newsLetterSubscriptionOrder.Active) ||
                (newsLetterSubscriptionOrder.Active && (originalSubscriptionOrder.Email != newsLetterSubscriptionOrder.Email)))
            {
                //If the previous entry was false, but this one is true, publish a subscribe.
                PublishSubscriptionOrderEvent(newsLetterSubscriptionOrder.Email, true, publishSubscriptionOrderEvents);
            }
            
            if ((originalSubscriptionOrder.Active && newsLetterSubscriptionOrder.Active) && 
                (originalSubscriptionOrder.Email != newsLetterSubscriptionOrder.Email))
            {
                //If the two emails are different publish an unsubscribe.
                PublishSubscriptionOrderEvent(originalSubscriptionOrder.Email, false, publishSubscriptionOrderEvents);
            }

            if ((originalSubscriptionOrder.Active && !newsLetterSubscriptionOrder.Active))
            {
                //If the previous entry was true, but this one is false
                PublishSubscriptionOrderEvent(originalSubscriptionOrder.Email, false, publishSubscriptionOrderEvents);
            }

            //Publish event
            _eventPublisher.EntityUpdated(newsLetterSubscriptionOrder);
        }

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionOrder">NewsLetter subscription</param>
        /// <param name="publishSubscriptionOrderEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual void DeleteNewsLetterSubscription(NewsLetterSubscription newsLetterSubscriptionOrder, bool publishSubscriptionOrderEvents = true)
        {
            if (newsLetterSubscriptionOrder == null) throw new ArgumentNullException("newsLetterSubscriptionOrder");

            _subscriptionRepository.Delete(newsLetterSubscriptionOrder);

            //Publish the unsubscribe event 
            PublishSubscriptionOrderEvent(newsLetterSubscriptionOrder.Email, false, publishSubscriptionOrderEvents);

            //event notification
            _eventPublisher.EntityDeleted(newsLetterSubscriptionOrder);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionOrderId">The newsletter subscription identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual NewsLetterSubscription GetNewsLetterSubscriptionById(int newsLetterSubscriptionOrderId)
        {
            if (newsLetterSubscriptionOrderId == 0) return null;

            return _subscriptionRepository.GetById(newsLetterSubscriptionOrderId);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionOrderGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual NewsLetterSubscription GetNewsLetterSubscriptionByGuid(Guid newsLetterSubscriptionOrderGuid)
        {
            if (newsLetterSubscriptionOrderGuid == Guid.Empty) return null;

            var newsLetterSubscriptionOrders = from nls in _subscriptionRepository.Table
                                          where nls.NewsLetterSubscriptionGuid == newsLetterSubscriptionOrderGuid
                                          orderby nls.Id
                                          select nls;

            return newsLetterSubscriptionOrders.FirstOrDefault();
        }

        /// <summary>
        /// Gets a newsletter subscription by email and store ID
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual NewsLetterSubscription GetNewsLetterSubscriptionByEmailAndStoreId(string email, int storeId)
        {
            if (!CommonHelper.IsValidEmail(email)) 
                return null;

            email = email.Trim();

            var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
                                          where nls.Email == email && nls.StoreId == storeId
                                          orderby nls.Id
                                          select nls;
            if (newsLetterSubscriptions != null)
                return newsLetterSubscriptions.FirstOrDefault();
            else
                return null;
        }

        /// <summary>
        /// Gets the newsletter subscription list
        /// </summary>
        /// <param name="email">Email to search or string. Empty to load all records.</param>
        /// <param name="storeId">Store identifier. 0 to load all records.</param>
        /// <param name="customerRoleId">Customer role identifier. Used to filter subscribers by customer role. 0 to load all records.</param>
        /// <param name="isActive">Value indicating whether subscriber record should be active or not; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>NewsLetterSubscription entities</returns>
        public virtual IPagedList<NewsLetterSubscription> GetAllNewsLetterSubscriptions(string email = null,
            int storeId = 0, bool? isActive = null, int customerRoleId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (customerRoleId == 0)
            {
                //do not filter by customer role
                var query = _subscriptionRepository.Table;
                if (!String.IsNullOrEmpty(email))
                    query = query.Where(nls => nls.Email.Contains(email));
                if (storeId > 0)
                    query = query.Where(nls => nls.StoreId == storeId);
                if (isActive.HasValue)
                    query = query.Where(nls => nls.Active == isActive.Value);
                query = query.OrderBy(nls => nls.Email);

                var subscriptions = new PagedList<NewsLetterSubscription>(query, pageIndex, pageSize);
                return subscriptions;
            }
            else
            {
                //filter by customer role
                var guestRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
                if (guestRole == null)
                    throw new NopException("'Guests' role could not be loaded");

                if (guestRole.Id == customerRoleId)
                {
                    //guests
                    var query = _subscriptionRepository.Table;
                    if (!String.IsNullOrEmpty(email))
                        query = query.Where(nls => nls.Email.Contains(email));
                    if (storeId > 0)
                        query = query.Where(nls => nls.StoreId == storeId);
                    if (isActive.HasValue)
                        query = query.Where(nls => nls.Active == isActive.Value);
                    query = query.Where(nls => !_customerRepository.Table.Any(c => c.Email == nls.Email));
                    query = query.OrderBy(nls => nls.Email);
                    
                    var subscriptions = new PagedList<NewsLetterSubscription>(query, pageIndex, pageSize);
                    return subscriptions;
                }
                else
                {
                    //other customer roles (not guests)
                    var query = _subscriptionRepository.Table.Join(_customerRepository.Table,
                        nls => nls.Email,
                        c => c.Email,
                        (nls, c) => new
                        {
                            NewsletterSubscribers = nls,
                            Customer = c
                        });
                    query = query.Where(x => x.Customer.CustomerRoles.Any(cr => cr.Id == customerRoleId));
                    if (!String.IsNullOrEmpty(email))
                        query = query.Where(x => x.NewsletterSubscribers.Email.Contains(email));
                    if (storeId > 0)
                        query = query.Where(x => x.NewsletterSubscribers.StoreId == storeId);
                    if (isActive.HasValue)
                        query = query.Where(x => x.NewsletterSubscribers.Active == isActive.Value);
                    query = query.OrderBy(x => x.NewsletterSubscribers.Email);

                    var subscriptions = new PagedList<NewsLetterSubscription>(query.Select(x=>x.NewsletterSubscribers), pageIndex, pageSize);
                    return subscriptions;
                }
            }
        }

        #endregion
    }
}