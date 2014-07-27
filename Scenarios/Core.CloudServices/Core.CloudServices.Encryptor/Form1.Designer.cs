namespace Contoso.Core.CloudServices.Encryptor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txtThumbPrint = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTextToEncrypt = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtEncryptedContent = new System.Windows.Forms.TextBox();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thumbprint:";
            this.toolTip1.SetToolTip(this.label1, "Thumbprint of the certifcate used for encryption/decryption:");
            // 
            // txtThumbPrint
            // 
            this.txtThumbPrint.Location = new System.Drawing.Point(149, 27);
            this.txtThumbPrint.Name = "txtThumbPrint";
            this.txtThumbPrint.Size = new System.Drawing.Size(552, 20);
            this.txtThumbPrint.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Text to encrypt/decrypt:";
            // 
            // txtTextToEncrypt
            // 
            this.txtTextToEncrypt.Location = new System.Drawing.Point(149, 70);
            this.txtTextToEncrypt.Name = "txtTextToEncrypt";
            this.txtTextToEncrypt.Size = new System.Drawing.Size(552, 20);
            this.txtTextToEncrypt.TabIndex = 3;
            // 
            // txtEncryptedContent
            // 
            this.txtEncryptedContent.Location = new System.Drawing.Point(149, 106);
            this.txtEncryptedContent.Multiline = true;
            this.txtEncryptedContent.Name = "txtEncryptedContent";
            this.txtEncryptedContent.Size = new System.Drawing.Size(552, 121);
            this.txtEncryptedContent.TabIndex = 4;
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(16, 127);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(118, 23);
            this.btnEncrypt.TabIndex = 5;
            this.btnEncrypt.Text = "Encrypt";
            this.toolTip1.SetToolTip(this.btnEncrypt, "Encrypt the content using the public key of the used certifcate");
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(16, 173);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(118, 23);
            this.btnDecrypt.TabIndex = 6;
            this.btnDecrypt.Text = "Decrypt";
            this.toolTip1.SetToolTip(this.btnDecrypt, "Decrypt the content using the private key of the used certifcate");
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 239);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.txtEncryptedContent);
            this.Controls.Add(this.txtTextToEncrypt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtThumbPrint);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Certificate based encryption/decryption";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtThumbPrint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTextToEncrypt;
        private System.Windows.Forms.TextBox txtEncryptedContent;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;
    }
}

