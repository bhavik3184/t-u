using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Admin.Models.Orders
{
    
    public partial class ReturnRequestTransactionModel : BaseNopEntityModel
    {
        public ReturnRequestTransactionModel()
        {
            this.Items = new List<ReturnRequestModel>();
        }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.ID")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Order")]
        public int SubscriptionOrderId { get; set; }

        public int OrderItemId { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Customer")]
        public string CustomerInfo { get; set; }

        public DateTime AvailableDateUtc { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.CustomerComments")]
        public string CustomerComments { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Status")]
        public int ReturnRequestStatusId { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.StaffNotes")]
        public string StaffNotes { get; set; }

      
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }


        public List<ReturnRequestModel> Items { get; set; }

    }
}