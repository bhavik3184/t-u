using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Settings;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    [Validator(typeof(OrderSettingsValidator))]
    public partial class OrderSettingsModel : BaseNopModel
    {
        public OrderSettingsModel()
        {
            GiftCards_Activated_SubscriptionOrderStatuses = new List<SelectListItem>();
            GiftCards_Deactivated_SubscriptionOrderStatuses = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Admin.Configuration.Settings.Order.IsReOrderAllowed")]
        public bool IsReOrderAllowed { get; set; }
        public bool IsReOrderAllowed_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.MinOrderSubtotalAmount")]
        public decimal MinOrderSubtotalAmount { get; set; }
        public bool MinOrderSubtotalAmount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.MinOrderSubtotalAmountIncludingTax")]
        public bool MinOrderSubtotalAmountIncludingTax { get; set; }
        public bool MinOrderSubtotalAmountIncludingTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.MinOrderTotalAmount")]
        public decimal MinOrderTotalAmount { get; set; }
        public bool MinOrderTotalAmount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.AnonymousCheckoutAllowed")]
        public bool AnonymousCheckoutAllowed { get; set; }
        public bool AnonymousCheckoutAllowed_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.TermsOfServiceOnBorrowCartPage")]
        public bool TermsOfServiceOnBorrowCartPage { get; set; }
        public bool TermsOfServiceOnBorrowCartPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.TermsOfServiceOnOrderConfirmPage")]
        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public bool TermsOfServiceOnOrderConfirmPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.OnePageCheckoutEnabled")]
        public bool OnePageCheckoutEnabled { get; set; }
        public bool OnePageCheckoutEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab")]
        public bool OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab { get; set; }
        public bool OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.DisableBillingAddressCheckoutStep")]
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisableBillingAddressCheckoutStep_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.DisableOrderCompletedPage")]
        public bool DisableOrderCompletedPage { get; set; }
        public bool DisableOrderCompletedPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderPlacedEmail")]
        public bool AttachPdfInvoiceToOrderPlacedEmail { get; set; }
        public bool AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderPaidEmail")]
        public bool AttachPdfInvoiceToOrderPaidEmail { get; set; }
        public bool AttachPdfInvoiceToOrderPaidEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderCompletedEmail")]
        public bool AttachPdfInvoiceToOrderCompletedEmail { get; set; }
        public bool AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestsEnabled")]
        public bool ReturnRequestsEnabled { get; set; }
        public bool ReturnRequestsEnabled_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Order.NumberOfDaysReturnRequestAvailable")]
        public int NumberOfDaysReturnRequestAvailable { get; set; }
        public bool NumberOfDaysReturnRequestAvailable_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.GiftCards_Activated")]
        public int GiftCards_Activated_SubscriptionOrderStatusId { get; set; }
        public IList<SelectListItem> GiftCards_Activated_SubscriptionOrderStatuses { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.GiftCards_Deactivated")]
        public int GiftCards_Deactivated_SubscriptionOrderStatusId { get; set; }
        public IList<SelectListItem> GiftCards_Deactivated_SubscriptionOrderStatuses { get; set; }
        
        public string PrimaryStoreCurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.SubscriptionOrderIdent")]
        public int? SubscriptionOrderIdent { get; set; }
    }
}