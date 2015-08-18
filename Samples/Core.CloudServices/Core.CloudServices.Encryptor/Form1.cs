using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contoso.Core.CloudServices.Encryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            txtEncryptedContent.Text = EncryptionUtility.Encrypt(txtTextToEncrypt.Text, txtThumbPrint.Text);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            txtEncryptedContent.Text = EncryptionUtility.Decrypt(txtTextToEncrypt.Text, txtThumbPrint.Text);
        }
    }
}
