// Copyright (c) Microsoft Corporation. All rights reserved.// Licensed under the MIT license.

using Microsoft.SharePoint.Client;
using System;
using System.Windows.Forms;

namespace SP_Discussion_Migrator
{
    static class Program
    {
        internal static ClientContext SPContext = null;

        internal static Settings Settings = new Settings();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new ConnectionForm());

            }
            catch (Exception ex)
            {

                string fileName = string.Format("CrashLog_{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
                //string content = string.Format("Message: {0}\r\n{1}"
                System.IO.File.WriteAllText(fileName, ex.ToString());

                MessageBox.Show("Exiting... Critical Exception occured.\r\nPlease see Crash Log file for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
        }
    }
}
