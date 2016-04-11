using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;


namespace Governance.AddInSecurity
{
    public partial class Form1 : Form
    {
        public string scope = "LocalMachine";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = string.Empty;
            string strEntropy = textBox4.Text.ToString();
            byte[] aditionalEntropy = EncryptionUtility.GetBytes(strEntropy);
            string EncryptText = textBox1.Text.ToString();
            if (EncryptText != "")
            {
                SecureString str = EncryptionUtility.ToSecureString(EncryptText);

                if (radioButton2.Checked)
                    scope = "CurrentUser";
                else
                    scope = "LocalMachine";

                textBox2.Text = EncryptionUtility.EncryptStringWithDPAPI(str, aditionalEntropy, scope);
            }
            else
                MessageBox.Show("Please enter input");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = string.Empty;
            string strEntropy = textBox4.Text.ToString();
            byte[] aditionalEntropy = EncryptionUtility.GetBytes(strEntropy);
           
            if (radioButton2.Checked)
                scope = "CurrentUser";
            else
                scope = "LocalMachine";

            SecureString str = EncryptionUtility.DecryptStringWithDPAPI(textBox2.Text,aditionalEntropy,scope);
            string decryptedValue = EncryptionUtility.ToInsecureString(str);
            if (decryptedValue == string.Empty)
                textBox3.Text = "The keys used for encryption don't match";
            else
                textBox3.Text = decryptedValue;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != null)
            {
                Clipboard.SetText(textBox2.Text.ToString());
                MessageBox.Show("Text is copied");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
        }
    }
}
