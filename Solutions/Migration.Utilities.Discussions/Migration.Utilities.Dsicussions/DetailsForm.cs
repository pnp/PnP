// Copyright (c) Microsoft Corporation. All rights reserved.// Licensed under the MIT license.

using Microsoft.SharePoint.Client;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SP_Discussion_Migrator
{
    public partial class DetailsForm : MetroFramework.Forms.MetroForm
    {
        private DataTable outputTable;
        private Thread loadThread;

        private class ProgressInfo
        {
            public int TotalItems { get; set; }
            public int Completed { get; set; }
        };

        private ProgressInfo loadProgress = new ProgressInfo() { TotalItems = 0, Completed = 0 };

        public DetailsForm()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the current form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsForm_Load(object sender, EventArgs e)
        {
            this.linkLabel1.Text = Program.SPContext.Web.Title;
            this.linkLabel1.Links.Add(0, this.linkLabel1.Text.Length, Program.SPContext.Url);

            var webNode = treeView1.Nodes.Add(Program.SPContext.Web.Title);
            webNode.ImageIndex = 0;
            webNode.SelectedImageIndex = 0;

            ListCollection lists = Program.SPContext.Web.Lists;

            Program.SPContext.Load(lists, l => l.Where(i => i.BaseTemplate == (int)ListTemplateType.DiscussionBoard),
                l => l.Include(i => i.Title, i => i.ItemCount));
            Program.SPContext.ExecuteQuery();

            foreach (List item in lists)
            {
                var node = webNode.Nodes.Add(string.Format("{0} ({1})",
                    item.Title, item.ItemCount));
                node.Tag = item.Title;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
            }

            webNode.Expand();
        }

        /// <summary>
        /// Handles the Click event of Exit buttion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_Click(object sender, EventArgs e)
        {
            if (null != this.loadThread && this.loadThread.ThreadState != ThreadState.Stopped)
            {
                this.loadThread.Abort();
            }

            Application.Exit();
        }

        /// <summary>
        /// Handles the Form closed event of the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != this.loadThread && this.loadThread.ThreadState != ThreadState.Stopped)
            {
                this.loadThread.Abort();
            }

            Application.Exit();
        }

        /// <summary>
        /// Handles the After Select event of the tree view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != treeView1.Nodes[0])
            {
                string selectedListName = (string)e.Node.Tag;
                //LoadDiscussionItems(selectedListName);

                this.loadThread = new Thread(new ParameterizedThreadStart(this.LoadDiscussionItems));
                loadThread.Start(selectedListName);

                this.metroProgressBar.Visible = true;
                this.exportLinkLabel.Enabled = false;
            }
        }

        /// <summary>
        /// Shows the progress of currently executing operation
        /// </summary>
        private void ShowProgress()
        {
            this.metroProgressBar.SuspendLayout();
            this.metroProgressBar.Maximum = this.loadProgress.TotalItems;
            this.metroProgressBar.Value = this.loadProgress.Completed;

            //this.metroProgressBar.Visible = (this.metroProgressBar.Value < this.metroProgressBar.Maximum);

            this.infoTextBox.Text = string.Format("Loaded {0} of {1} items...",
                this.loadProgress.Completed, this.loadProgress.TotalItems);

            if (this.loadProgress.Completed >= this.loadProgress.TotalItems)
            {
                this.infoTextBox.Text = string.Format("Loaded {0} threads in total.\r\n\r\nNext Actions\r\n  - Select the Data View tab to review the loaded items, or\r\n  - Click Export button to export it to Xml.", loadProgress.TotalItems);
                this.metroProgressBar.Visible = false;
                this.exportLinkLabel.Enabled = true;
            }

            this.metroProgressBar.ResumeLayout();
        }

        /// <summary>
        /// Loads the top level items from a specified SharePoint List
        /// </summary>
        /// <param name="selectedListName">Name of the List</param>
        /// <remarks>
        /// This method will recursively call itself to load all the sub-items. 
        /// The loaded items are stored in a <see cref="System.Data.DataTable"/> object within the calss.
        /// </remarks>
        private void LoadDiscussionItems(object selectedListName)
        {
            // Initialize the data table
            outputTable = new DataTable();
            outputTable.TableName = (string)selectedListName;

            foreach (var fieldName in Program.Settings.fetchedFields)
            {
                outputTable.Columns.Add(fieldName);
            }

            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><ViewFields>";

            foreach (var field in Program.Settings.fetchedFields)
            {
                query.ViewXml += "<FieldRef Name='" + field + "'/>";
            }

            query.ViewXml += "</ViewFields><RowLimit>4999</RowLimit></View>";

            List list = Program.SPContext.Web.Lists.GetByTitle((string)selectedListName);
            Program.SPContext.Load(list, l => l.RootFolder.ServerRelativeUrl);
            Program.SPContext.ExecuteQuery();

            ListItemCollectionPosition queryPosition = null;

            this.loadProgress.TotalItems = 0;
            this.loadProgress.Completed = 0;

            do
            {
                var topLevelItems = list.GetItems(query);

                Program.SPContext.Load(topLevelItems);
                Program.SPContext.ExecuteQuery();

                this.loadProgress.TotalItems += topLevelItems.Count;
                //this.loadProgress.Completed = 0;
                this.Invoke(new MethodInvoker(ShowProgress));

                queryPosition = query.ListItemCollectionPosition;

                // Output Logic
                for (int i = 0; i < topLevelItems.Count; ++i)
                {
                    ListItem currentItem = topLevelItems[i];

                    // Data Caching logic
                    DataRow row = outputTable.NewRow();
                    foreach (var field in Program.Settings.fetchedFields)
                    {
                        string outputVal = null;
                        try
                        {
                            var value = currentItem[field];

                            if (value is FieldUserValue)
                            {
                                outputVal = (value as FieldUserValue).Email.ToString();
                            }
                            else if (value is FieldLookupValue)
                            {
                                outputVal = (value as FieldLookupValue).LookupValue.ToString();
                            }
                            else if (value is DateTime)
                            {
                                outputVal = ((DateTime)value).ToString("u");
                            }
                            else
                            {
                                outputVal = (null == currentItem[field]) ? string.Empty : currentItem[field].ToString();
                            }
                        }
                        catch (Microsoft.SharePoint.Client.PropertyOrFieldNotInitializedException)
                        {
                            outputVal = "(null)";
                        }

                        row[field] = outputVal;
                    }

                    outputTable.Rows.Add(row);

                    if (0 != int.Parse(currentItem["ItemChildCount"].ToString()))
                    {
                        int discussionId = (int)currentItem["ID"];

                        string fileRef = (string)currentItem["FileRef"];


                        LoadSubItems(list, discussionId, fileRef);
                    }

                    if ((bool)currentItem["Attachments"])
                    {
                        string attachmentsFolderPath = list.RootFolder.ServerRelativeUrl + "/Attachments/" + currentItem["ID"];

                        DownloadAttachments(currentItem, attachmentsFolderPath);
                        //Folder folder = Program.SPContext.Web.GetFolderByServerRelativePath()
                    }

                    this.loadProgress.Completed = i;
                    this.Invoke(new MethodInvoker(ShowProgress));
                }
            }
            while (null != queryPosition);
            
            this.loadProgress.Completed = this.loadProgress.TotalItems;
            this.Invoke(new MethodInvoker(ShowProgress));
        }

        /// <summary>
        /// Loads the sub items from a specified folder in a SharePoint List
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="discussionId">ID of the discussion thread</param>
        /// <param name="folderUrl">relative URL of the folder</param>
        /// <remarks>
        /// This method will recursively call itself to load all the sub-items. 
        /// The loaded items are stored in a <see cref="System.Data.DataTable"/> object within the calss.
        /// </remarks>
        private void LoadSubItems(List list, int discussionId, string folderUrl)
        {
            CamlQuery subItemsQuery = new CamlQuery();
            subItemsQuery.ViewXml = "<View>";

            subItemsQuery.FolderServerRelativeUrl = folderUrl;
            // Add Query
            subItemsQuery.ViewXml += "<Query><Where><Eq><FieldRef Name='ParentFolderId'/><Value Type='Integer'>" + discussionId + "</Value></Eq></Where></Query>";

            // Add ViewFields
            subItemsQuery.ViewXml += "<ViewFields>";
            foreach (var field in Program.Settings.fetchedFields)
            {
                subItemsQuery.ViewXml += "<FieldRef Name='" + field + "'/>";
            }
            subItemsQuery.ViewXml += "</ViewFields>";

            subItemsQuery.ViewXml += "<RowLimit>4999</RowLimit></View>";

            var subItems = list.GetItems(subItemsQuery);

            Program.SPContext.Load(subItems);
            Program.SPContext.ExecuteQuery();

            foreach (var subItem in subItems)
            {
                DataRow row = outputTable.NewRow();
                foreach (var field in Program.Settings.fetchedFields)
                {
                    string outputVal = null;
                    try
                    {
                        var value = subItem[field];
                        if (value is FieldUserValue)
                        {
                            outputVal = (value as FieldUserValue).Email.ToString();
                        }
                        else if (value is FieldLookupValue)
                        {
                            outputVal = (value as FieldLookupValue).LookupValue.ToString();
                        }
                        else if (value is DateTime)
                        {
                            outputVal = ((DateTime)value).ToString("u");
                        }
                        else
                        {
                            outputVal = (null == subItem[field]) ? string.Empty : subItem[field].ToString();
                        }

                    }
                    catch (Microsoft.SharePoint.Client.PropertyOrFieldNotInitializedException)
                    {
                        outputVal = "(null)";
                    }
                    row[field] = outputVal;

                }

                outputTable.Rows.Add(row);

                if ((bool)subItem["Attachments"])
                {
                    string attachmentsFolderPath = list.RootFolder.ServerRelativeUrl + "/Attachments/" + subItem["ID"];

                    DownloadAttachments(subItem, attachmentsFolderPath);
                }

                if (0 != int.Parse(subItem["ItemChildCount"].ToString()))
                {
                    int subDId = (int)subItem["ID"];

                    string subFileRef = (string)subItem["FileRef"];


                    LoadSubItems(list, subDId, subFileRef);
                }
            }

            this.dataGridView1.DataSource = this.outputTable;
        }

        /// <summary>
        /// Downloads attachments from a specified folder url
        /// </summary>
        /// <param name="currentItem">The <see cref="ListItem"/> object</param>
        /// <param name="attachmentsFolderUrl">Server relative URL of the folder</param>
        /// <remarks>
        /// Presently, all the attachments are downloaded to a sub folder named "Attachements" at the location 
        /// from where this utility is executing.
        /// </remarks>
        private void DownloadAttachments(ListItem currentItem, string attachmentsFolderUrl)
        {
            Folder attachmentsFolder = Program.SPContext.Web.GetFolderByServerRelativeUrl(attachmentsFolderUrl);
            FileCollection files = attachmentsFolder.Files;
            Program.SPContext.Load(files, f => f.Include(t => t.Name, t => t.ServerRelativeUrl));
            Program.SPContext.ExecuteQuery();

            foreach (var attachedFile in files)
            {
                // We currently export all the attachments a sub folder named "Attachements" at the location 
                // from where this utility is executing
                // TODO: Get target location from user
                string targetFilePath = System.IO.Path.Combine(Application.StartupPath, "Attachments", currentItem["ID"].ToString(), attachedFile.Name);

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(targetFilePath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetFilePath));

                var fileInfo = File.OpenBinaryDirect(Program.SPContext, attachedFile.ServerRelativeUrl);

                using (var outputStream = System.IO.File.Create(targetFilePath))
                {
                    fileInfo.Stream.CopyTo(outputStream);
                }
            }
        }
        
        /// <summary>
        /// Handles the Link Clicked event of Export link label.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Object of <see cref="LinkLabelLinkClickedEventArgs"/></param>
        private void exportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (null != outputTable)
            {
                outputTable.WriteXml("output.xml", XmlWriteMode.WriteSchema);

                MessageBox.Show(string.Format("Data exported to {0}", System.IO.Path.GetFullPath("output.xml")), 
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show("No data to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Link Clicked event of the Site title link label, by opening the URL of the target site.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Object of <see cref="LinkLabelLinkClickedEventArgs"/></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }
    }
}
