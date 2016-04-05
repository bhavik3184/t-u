using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.BorrowCart;

namespace Nop.Web.Models.BorrowCart
{
    [Validator(typeof(MyToyBoxEmailAFriendValidator))]
    public partial class MyToyBoxEmailAFriendModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("MyToyBox.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("MyToyBox.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("MyToyBox.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}