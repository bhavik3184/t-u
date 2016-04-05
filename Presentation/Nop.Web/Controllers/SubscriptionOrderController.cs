using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.SubscriptionOrders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Models.SubscriptionOrder;

namespace Nop.Web.Controllers
{
    public partial class SubscriptionOrderController : BasePublicController
    {
		#region Fields

        private readonly ISubscriptionOrderService _subscriptionService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISubscriptionOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IPdfService _pdfService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWebHelper _webHelper;
        private readonly IDownloadService _downloadService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IStoreContext _storeContext;
        private readonly ISubscriptionOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;

        private readonly SubscriptionOrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;

        #endregion

		#region Constructors

        public SubscriptionOrderController(ISubscriptionOrderService subscriptionService, 
            IShipmentService shipmentService, 
            IWorkContext workContext,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            ISubscriptionOrderProcessingService orderProcessingService, 
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService, 
            ILocalizationService localizationService,
            IPdfService pdfService, 
            IShippingService shippingService,
            ICountryService countryService, 
            IProductAttributeParser productAttributeParser,
            IWebHelper webHelper,
            IDownloadService downloadService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IStoreContext storeContext,
            ISubscriptionOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            CatalogSettings catalogSettings,
            SubscriptionOrderSettings orderSettings,
            TaxSettings taxSettings,
            ShippingSettings shippingSettings, 
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            PdfSettings pdfSettings)
        {
            this._subscriptionService = subscriptionService;
            this._shipmentService = shipmentService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._pdfService = pdfService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._webHelper = webHelper;
            this._downloadService = downloadService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;

            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual CustomerSubscriptionOrderListModel PrepareCustomerSubscriptionOrderListModel()
        {
            var model = new CustomerSubscriptionOrderListModel();
            var orders = _subscriptionService.SearchSubscriptionOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id);
            foreach (var order in orders)
            {
                var orderModel = new CustomerSubscriptionOrderListModel.SubscriptionOrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    SubscriptionOrderStatusEnum = order.SubscriptionOrderStatus,
                    SubscriptionOrderStatus = order.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                    
                };
                if (order.SubscriptionOrderItems.FirstOrDefault() !=null) {
                    orderModel.RentalStartDate = _dateTimeHelper.ConvertToUserTime(order.SubscriptionOrderItems.FirstOrDefault().RentalStartDateUtc ?? DateTime.Now, DateTimeKind.Utc);
                    orderModel.RentalEndDate = _dateTimeHelper.ConvertToUserTime(order.SubscriptionOrderItems.FirstOrDefault().RentalEndDateUtc?? DateTime.Now, DateTimeKind.Utc);
                    orderModel.PlanName = order.SubscriptionOrderItems.FirstOrDefault().Plan.Name;
                }
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderTotal, order.CurrencyRate);
                orderModel.SubscriptionOrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                model.SubscriptionOrders.Add(orderModel);
            }

            var recurringPayments = _subscriptionService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerSubscriptionOrderListModel.RecurringSubscriptionOrderModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)),
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialSubscriptionOrderId = recurringPayment.InitialSubscriptionOrder.Id,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                };

                model.RecurringSubscriptionOrders.Add(recurringPaymentModel);
            }

            return model;
        }

        [NonAction]
        protected virtual SubscriptionOrderDetailsModel PrepareSubscriptionOrderDetailsModel(SubscriptionOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            var model = new SubscriptionOrderDetailsModel();

            model.Id = order.Id;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.SubscriptionOrderStatus = order.SubscriptionOrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.IsReSubscriptionOrderAllowed = _orderSettings.IsReSubscriptionOrderAllowed;
            model.IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order);
            model.PdfInvoiceDisabled = false;
            model.SecurityDeposit = _priceFormatter.FormatPrice(order.SecurityDeposit);
            model.RegistrationCharge = _priceFormatter.FormatPrice(order.RegistrationCharge);
            //shipping info
            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickUpInStore = order.PickUpInStore;
                if (!order.PickUpInStore)
                {
                    model.ShippingAddress.PrepareModel(
                        address: order.ShippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);
                }
                model.ShippingMethod = order.ShippingMethod;
   

                //shipments (only already shipped)
                var orderItems1 = order.OrderItems;
                foreach (var orderItem1 in orderItems1)
                {
                    var shipments = orderItem1.Shipments.Where(x => x.ShippedDateUtc.HasValue).OrderBy(x => x.CreatedOnUtc).ToList();
                    foreach (var shipment in shipments)
                    {
                        var shipmentModel = new SubscriptionOrderDetailsModel.ShipmentBriefModel
                        {
                            Id = shipment.Id,
                            TrackingNumber = shipment.TrackingNumber,
                        };
                        if (shipment.ShippedDateUtc.HasValue)
                            shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                        if (shipment.DeliveryDateUtc.HasValue)
                            shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                        model.Shipments.Add(shipmentModel);
                    }
                }
            }


            //billing info
            model.BillingAddress.PrepareModel(
                address: order.BillingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings,
                addressAttributeFormatter: _addressAttributeFormatter);

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            model.PaymentMethodStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = order.DeserializeCustomValues();

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderSubtotalInclTax, order.CurrencyRate);
                model.SubscriptionOrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.SubscriptionOrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderSubtotalExclTax, order.CurrencyRate);
                model.SubscriptionOrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.SubscriptionOrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderShippingInclTax, order.CurrencyRate);
                model.SubscriptionOrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderShippingExclTax, order.CurrencyRate);
                model.SubscriptionOrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (_taxSettings.HideTaxInSubscriptionOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.SubscriptionOrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    displayTaxRates = _taxSettings.DisplayTaxRates && order.TaxRatesDictionary.Count > 0;
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in order.TaxRatesDictionary)
                    {
                        model.TaxRates.Add(new SubscriptionOrderDetailsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = false;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.SubscriptionOrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);


            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new SubscriptionOrderDetailsModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.SubscriptionOrderTotal, order.CurrencyRate);
            model.SubscriptionOrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in order.SubscriptionOrderNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.SubscriptionOrderNotes.Add(new SubscriptionOrderDetailsModel.SubscriptionOrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = orderNote.FormatSubscriptionOrderNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }


            //purchased products
            model.ShowSku = _catalogSettings.ShowProductSku;
            var orderItems = _subscriptionService.GetAllSubscriptionOrderItems(order.Id, null, null, null, null, null, null);
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = new SubscriptionOrderDetailsModel.SubscriptionOrderItemModel
                {
                    Id = orderItem.Id,
                    SubscriptionOrderItemGuid = orderItem.SubscriptionOrderItemGuid,
                    Sku = orderItem.Plan.Sku,
                    ProductId = orderItem.Plan.Id,
                    ProductName = orderItem.Plan.GetLocalized(x => x.Name),
                    ProductSeName = orderItem.Plan.GetSeName(),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                    MaxNoOfDeliveries = orderItem.Plan.MaxNoOfDeliveries,
                    NoOfItemsToBorrow = orderItem.Plan.NoOfItemsToBorrow,
                    Duration = orderItem.Plan.RentalPriceLength.ToString() + (RentalPricePeriod?)orderItem.Plan.RentalPricePeriodId,
                };
                //rental info
                if (orderItem.Plan.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Plan.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Plan.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("SubscriptionOrder.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(orderItemModel);

                //unit price, subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }

                //downloadable products
                
            }

            return model;
        }

      

        #endregion

        #region Methods

        //My account / SubscriptionOrders
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerSubscriptionOrders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = PrepareCustomerSubscriptionOrderListModel();
            return View(model);
        }

        //My account / SubscriptionOrders / Cancel recurring order
        [HttpPost, ActionName("CustomerSubscriptionOrders")]
        [PublicAntiForgery]
        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        public ActionResult CancelRecurringPayment(FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //get recurring payment identifier
            int recurringPaymentId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue.Substring("cancelRecurringPayment".Length));

            var recurringPayment = _subscriptionService.GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
            {
                return RedirectToRoute("CustomerSubscriptionOrders");
            }

            if (_orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
            {
                var errors = _orderProcessingService.CancelRecurringPayment(recurringPayment);

                var model = PrepareCustomerSubscriptionOrderListModel();
                model.CancelRecurringPaymentErrors = errors;

                return View(model);
            }
            else
            {
                return RedirectToRoute("CustomerSubscriptionOrders");
            }
        }

        //My account / Reward points
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerRewardPoints()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_rewardPointsSettings.Enabled)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerRewardPointsModel();
            foreach (var rph in _rewardPointService.GetRewardPointsHistory(customer.Id))
            {
                model.RewardPoints.Add(new CustomerRewardPointsModel.RewardPointsHistoryModel
                {
                    Points = rph.Points,
                    PointsBalance = rph.PointsBalance,
                    Message = rph.Message,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc)
                });
            }
            //current amount/balance
            int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
            model.RewardPointsBalance = rewardPointsBalance;
            model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
            //minimum amount/balance
            int minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
            decimal minimumRewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(minimumRewardPointsBalance);
            decimal minimumRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minimumRewardPointsAmountBase, _workContext.WorkingCurrency);
            model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
            model.MinimumRewardPointsAmount = _priceFormatter.FormatPrice(minimumRewardPointsAmount, true, false);
            return View(model);
        }

        //My account / SubscriptionOrder details page
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Details(int orderId)
        {
            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var model = PrepareSubscriptionOrderDetailsModel(order);

            return View(model);
        }

        //My account / SubscriptionOrder details page / Print
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PrintSubscriptionOrderDetails(int orderId)
        {
            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var model = PrepareSubscriptionOrderDetailsModel(order);
            model.PrintMode = true;

            return View("Details", model);
        }

        //My account / SubscriptionOrder details page / PDF invoice
        public ActionResult GetPdfInvoice(int orderId)
        {
            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var orders = new List<SubscriptionOrder>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", string.Format("order_{0}.pdf", order.Id));
        }

        //My account / SubscriptionOrder details page / re-order
        public ActionResult ReSubscriptionOrder(int orderId)
        {
            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            _orderProcessingService.ReSubscriptionOrder(order);
            return RedirectToRoute("SubscriptionCart");
        }

        //My account / SubscriptionOrder details page / Complete payment
        [HttpPost, ActionName("Details")]
        [PublicAntiForgery]
        [FormValueRequired("repost-payment")]
        public ActionResult RePostPayment(int orderId)
        {
            var order = _subscriptionService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_paymentService.CanRePostProcessPayment(order))
                return RedirectToRoute("SubscriptionOrderDetails", new { orderId = orderId });

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                SubscriptionOrder = order
            };
            _paymentService.PostProcessPayment(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("Redirected");
            }

            //if no redirection has been done (to a third-party payment page)
            //theoretically it's not possible
            return RedirectToRoute("SubscriptionOrderDetails", new { orderId = orderId });
        }

        //My account / SubscriptionOrder details page / Shipment details page
        


        #endregion
    }
}
