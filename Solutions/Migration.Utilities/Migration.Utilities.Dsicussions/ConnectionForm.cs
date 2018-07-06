// Copyright (c) Microsoft Corporation. All rights reserved.// Licensed under the MIT license.

using Microsoft.SharePoint.Client;
using System;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

namespace SP_Discussion_Migrator
{
    public partial class ConnectionForm : MetroFramework.Forms.MetroForm
    {
        public ConnectionForm()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of Exit button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Handles the click event of Connect button by creating a <see cref="ClientContext"/> object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            char[] pass = passwordTextbox.Text.ToCharArray();
            SecureString ssPass = new SecureString();
            for (int i = 0; i < pass.Length; ++i)
            {
                ssPass.AppendChar(pass[i]);
            }

            ClientContext ctx = new ClientContext(siteURLTextbox.Text.Trim());
            ctx.Credentials = new SharePointOnlineCredentials(usernameTextbox.Text.Trim(), ssPass);


            // 2/12/18 - Added UserAgent string to avoid throttling
            ctx.ExecutingWebRequest += delegate (object senderObj, WebRequestEventArgs eventArgs)
            {
                eventArgs.WebRequestExecutor.WebRequest.UserAgent = "NONISV|Microsoft|SPDiscussionMigrator/1.0";
            };

            Web web = ctx.Web;

            try
            {
                ctx.Load(web, w => w.Title);
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Program.SPContext = ctx;

            if (radioButton1.Checked)
            {
                DetailsForm details = new DetailsForm();
                details.Show();
            }
            else
            {
                MigrateForm migrate = new MigrateForm();
                migrate.Show();
            }

            this.Hide();
        }

        /// <summary>
        /// Handles the Load event of the current form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            // TODO: Use this block to initialize the site URL and username fields while debugging/testing.
            this.siteURLTextbox.Text = "";
            this.usernameTextbox.Text = "";
#endif
        }
    }
}
