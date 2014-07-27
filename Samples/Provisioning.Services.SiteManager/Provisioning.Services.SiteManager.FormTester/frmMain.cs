using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contoso.Provisioning.Services.SiteManager.FormTester
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnCreateSiteCollection_Click(object sender, EventArgs e)
        {
            SiteManager.SiteManagerClient managerClient = GetSiteManagerClient();

            SiteManager.SiteData newSite = new SiteManager.SiteData() { Description=txtSiteDescription.Text, LcId=txtSiteLanguageId.Text,
                                                                         OwnerLogin=txtSiteOwnerAccount.Text, SecondaryContactLogin=txtSiteSecondaryAccount.Text, 
                                                                         Title=txtSiteTitle.Text, Url=txtSiteUrl.Text, WebTemplate=txtSiteTemplate.Text};

            string url = managerClient.CreateSiteCollection(newSite);
            txtStatus.Text = string.Format("Site collection creation was called and return value was '{0}'.", url);
        }

        private void btnGetSiteCollections_Click(object sender, EventArgs e)
        {
            SiteManager.SiteManagerClient managerClient = GetSiteManagerClient();

            //Execute WCF call
            List<SiteManager.SiteData> list = managerClient.ListSiteCollections();
            txtStatus.Text = string.Format("Site collection enum has been called and we got {0} items.", list.Count);

            foreach (var item in list)
            {
                txtStatus.Text = txtStatus.Text + string.Format("\r\n{0}", item.Url);
            }


        }

        private SiteManager.SiteManagerClient GetSiteManagerClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            if (txtWebApplicationUrl.Text.ToLower().Contains("https://"))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }
            else
            {
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            }
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

            EndpointAddress endPoint = new EndpointAddress(txtWebApplicationUrl.Text + "/_vti_bin/provisioning.services.sitemanager/sitemanager.svc");
            //Set time outs
            binding.ReceiveTimeout = TimeSpan.FromMinutes(15);
            binding.CloseTimeout = TimeSpan.FromMinutes(15);
            binding.OpenTimeout = TimeSpan.FromMinutes(15);
            binding.SendTimeout = TimeSpan.FromMinutes(15);

            //Create proxy instance
            SiteManager.SiteManagerClient managerClient = new SiteManager.SiteManagerClient(binding, endPoint);
            managerClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var impersonator = new System.Net.NetworkCredential(txtAccount.Text, txtPassword.Text, txtDomain.Text);
            managerClient.ClientCredentials.Windows.ClientCredential = impersonator;

            return managerClient;
        }

        private void btnLocaleSet_Click(object sender, EventArgs e)
        {
            SiteManager.SiteManagerClient managerClient = GetSiteManagerClient();

            //Execute WCF call
            managerClient.SetSiteLocale(txtLocaleSiteCollection.Text, txtLocaleSetString.Text);
            txtStatus.Text = string.Format("Site collection root web locale has been changed.");

        }
    }
}
