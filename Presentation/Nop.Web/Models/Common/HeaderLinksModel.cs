using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public partial class HeaderLinksModel : BaseNopModel
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerEmailUsername { get; set; }

        public string CustomerName { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string YoutubeLink { get; set; }
        public string GooglePlusLink { get; set; }

        public bool BorrowCartEnabled { get; set; }
        public int BorrowCartItems { get; set; }
        
        public bool MyToyBoxEnabled { get; set; }
        public int MyToyBoxItems { get; set; }

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}