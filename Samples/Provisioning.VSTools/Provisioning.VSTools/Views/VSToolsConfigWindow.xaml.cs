using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Perficient.Provisioning.VSTools
{
    /// <summary>
    /// Interaction logic for VSToolsConfigWindow.xaml
    /// </summary>
    public partial class VSToolsConfigWindow : DialogWindow
    {
        public VSToolsConfigWindow()
        {
            InitializeComponent();
        }
        
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.Validate())
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Site url, username, and password are required.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool Validate()
        {
            return !string.IsNullOrEmpty(this.txtSiteUrl.Text) && !string.IsNullOrEmpty(this.txtUsername.Text) && !string.IsNullOrEmpty(this.txtPassword.Password);
        }
    }
}
