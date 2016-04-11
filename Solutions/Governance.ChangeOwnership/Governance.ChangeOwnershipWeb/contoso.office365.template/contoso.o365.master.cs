using System;
using System.Web.UI.WebControls;

namespace Contoso.Office365.common
{
    public partial class contoso_o365 : System.Web.UI.MasterPage
    {
        public HiddenField Hdn_Master_CurrentUserName
        {
            get { return HiddenField_Master_CurrentUserName; }
            set { HiddenField_Master_CurrentUserName = value; }
        }

        public HiddenField Hdn_Master_CurrentUserEmail
        {
            get { return HiddenField_Master_CurrentUserEmail; }
            set { HiddenField_Master_CurrentUserEmail = value; }
        }

        public HiddenField Hdn_Master_CurrentSiteUrl
        {
            get { return HiddenField_Master_CurrentSiteUrl; }
            set { HiddenField_Master_CurrentSiteUrl = value; }
        }

        public HiddenField Hdn_Master_CurrentSiteTitle
        {
            get { return HiddenField_Master_CurrentSiteTitle; }
            set { HiddenField_Master_CurrentSiteTitle = value; }
        }
        public HiddenField Hdn_Master_PageTitle
        {
            get { return HiddenField_Master_PageTitle; }
            set { HiddenField_Master_PageTitle = value; }
        }

        public HiddenField Hdn_Master_ShortPageTitle
        {
            get { return HiddenField_Master_ShortPageTitle; }
            set { HiddenField_Master_ShortPageTitle = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}