using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.SubscriptionCart
{
    public partial class SubscriptionOrderTotalsModel : BaseNopModel
    {
        public SubscriptionOrderTotalsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
        }
        public bool IsEditable { get; set; }

        public string SubTotal { get; set; }

        public string SubTotalDiscount { get; set; }
        public bool AllowRemovingSubTotalDiscount { get; set; }
        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public string PreviousRentalBalance { get; set; }
        public string PreviousRentalBalanceDesc { get; set; }
        public string SecurityDeposit { get; set; }
        public string SecurityDepositPaid { get; set; }
        public decimal PreviousRentalBalanceAmt { get; set; }
        public decimal SecurityDepositAmt { get; set; }
        public decimal SecurityDepositPaidAmt { get; set; }
        public decimal SecurityDepositBalanceAmt { get; set; }
        public string SecurityDepositBalance { get; set; }
        public string RegistrationCharge { get; set; }
        public string RegistrationChargePaid { get; set; }
        public decimal RegistrationChargeAmt { get; set; }
        public decimal RegistrationChargePaidAmt { get; set; }
        public decimal RegistrationChargeBalanceAmt { get; set; }
        public string RegistrationChargeBalance { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }

        public string RegistrationChargeBalanceLabel { get; set; }

        public string SecurityDepositBalanceLabel { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }


        public IList<GiftCard> GiftCards { get; set; }

        public string SubscriptionOrderTotalDiscount { get; set; }
        public bool AllowRemovingSubscriptionOrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }

        public int WillEarnRewardPoints { get; set; }

        public string SubscriptionOrderTotal { get; set; }

        #region Nested classes

        public partial class TaxRate: BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopEntityModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
            public string Remaining { get; set; }
        }
        #endregion
    }
}