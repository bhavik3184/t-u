using Nop.Core.Configuration;

namespace Nop.Core.Domain.SubscriptionOrders
{
    public class SubscriptionOrderSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether customer can make re-order
        /// </summary>
        public bool IsReSubscriptionOrderAllowed { get; set; }

        /// <summary>
        /// Gets or sets a minimum order subtotal amount
        /// </summary>
        public decimal MinSubscriptionOrderSubtotalAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'inimum order subtotal amount' option
        /// should be evaluated over 'X' value including tax or not
        /// </summary>
        public bool MinSubscriptionOrderSubtotalAmountIncludingTax { get; set; }

        /// <summary>
        /// Gets or sets a minimum order total amount
        /// </summary>
        public decimal MinSubscriptionOrderTotalAmount { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether anonymous checkout allowed
        /// </summary>
        public bool AnonymousCheckoutAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Terms of service' enabled on the shopping cart page
        /// </summary>
        public bool TermsOfServiceOnSubscriptionCartPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Terms of service' enabled on the order confirmation page
        /// </summary>
        public bool TermsOfServiceOnSubscriptionOrderConfirmPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'One-page checkout' is enabled
        /// </summary>
        public bool OnePageCheckoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether order totals should be displayed on 'Payment info' tab of 'One-page checkout' page
        /// </summary>
        public bool OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Billing address" step should be skipped
        /// </summary>
        public bool DisableBillingAddressCheckoutStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "SubscriptionOrder completed" page should be skipped
        /// </summary>
        public bool DisableSubscriptionOrderCompletedPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating we should attach PDF invoice to "SubscriptionOrder placed" email
        /// </summary>
        public bool AttachPdfInvoiceToSubscriptionOrderPlacedEmail { get; set; }
        /// <summary>
        /// Gets or sets a value indicating we should attach PDF invoice to "SubscriptionOrder paid" email
        /// </summary>
        public bool AttachPdfInvoiceToSubscriptionOrderPaidEmail { get; set; }
        /// <summary>
        /// Gets or sets a value indicating we should attach PDF invoice to "SubscriptionOrder completed" email
        /// </summary>
        public bool AttachPdfInvoiceToSubscriptionOrderCompletedEmail { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether "Return requests" are allowed
        /// </summary>
        public bool ReturnRequestsEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets a number of days that the Return Request Link will be available for customers after order placing.
        /// </summary>
        public int NumberOfDaysReturnRequestAvailable { get; set; }

        /// <summary>
        ///  Gift cards are activated when the order status is
        /// </summary>
        public int GiftCards_Activated_SubscriptionOrderStatusId { get; set; }

        /// <summary>
        ///  Gift cards are deactivated when the order status is
        /// </summary>
        public int GiftCards_Deactivated_SubscriptionOrderStatusId { get; set; }

        /// <summary>
        /// Gets or sets an order placement interval in seconds (prevent 2 orders being placed within an X seconds time frame).
        /// </summary>
        public int MinimumSubscriptionOrderPlacementInterval { get; set; }
    }
}