// Copyright (c) Microsoft Corporation. All rights reserved.// Licensed under the MIT license.

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SP_Discussion_Migrator
{
    public partial class MigrateForm : MetroFramework.Forms.MetroForm
    {
        private string inputFileDirectory = null;

        DataTable listData;

        // TODO: For future use; to save the history of executions
        //Dictionary<string, int> history = null;

        Dictionary<string, string> itemIdMappings = null;
        Dictionary<string, FieldUserValue> userMappings = null;

        static string executionLogFilename = string.Format("{0}_{1:yyyy-MM-dd_hh-mm-ss-tt}.txt",
            System.Configuration.ConfigurationManager.AppSettings.Get("MigrationLogPrefix"), DateTime.Now);

        private Thread migrateThread;

        /// <summary>
        /// Information about the migration progress.
        /// </summary>
        /// <remarks>Contains the total as well as currently completed item counts.</remarks>
        private class ProgressInfo
        {
            public int TotalItems { get; set; }
            public int Completed { get; set; }
            public int Failed { get; set; }
        };

        private ProgressInfo loadProgress = new ProgressInfo() { TotalItems = 0, Completed = 0, Failed = 0 };

        /// <summary>
        /// Create an instance of <see cref="MigrateForm"/>
        /// </summary>
        public MigrateForm()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the current instance of <see cref="MigrateForm"/>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments</param>
        private void MigrateForm_Load(object sender, EventArgs e)
        {
            this.linkLabel1.Text = Program.SPContext.Web.Title;
            this.linkLabel1.Links.Add(0, this.linkLabel1.Text.Length, Program.SPContext.Url);

            ListCollection lists = Program.SPContext.Web.Lists;

            Program.SPContext.Load(lists, l => l.Where(i => i.BaseTemplate == (int)ListTemplateType.DiscussionBoard),
                l => l.Include(i => i.Title));
            Program.SPContext.ExecuteQuery();

            foreach (List item in lists)
            {
                targetListsComboBox.Items.Add(item.Title);
            }
        }

        /// <summary>
        /// Handles the click event of the Browse button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "XML Files (*.xml)|*.xml";
                ofd.Title = "Select exported XML file for the migrated Discussions List...";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    inputPathTextbox.Text = ofd.FileName;
                    LoadInputFile(ofd.FileName);
                }
            }
        }

        /// <summary>
        /// Loads the input file.
        /// </summary>
        /// <param name="filePath">Path of the selected file.</param>
        private void LoadInputFile(string filePath)
        {
            inputFileDirectory = System.IO.Directory.GetParent(filePath).FullName;

            if (null != listData)
            {
                listData.Clear();
                infoLabel.ResetText();
            }

            listData = new DataTable();
            listData.ReadXml(filePath);

            string listName = listData.TableName;
            string itemCount = listData.Rows.Count.ToString();

            //infoTextbox.AppendText(string.Format("Loaded list '{0}' with {1} items.", listName, itemCount));
            infoLabel.Text = string.Format("Loaded list '{0}' with {1} items.", listName, itemCount);

            if (null != itemIdMappings)
            {
                itemIdMappings.Clear();
            }

            itemIdMappings = new Dictionary<string, string>();

            var sourceIds = from row in listData.AsEnumerable()
                            select row.Field<string>("ID");

            foreach (var srcId in sourceIds)
            {
                itemIdMappings.Add(srcId, null);
            }

            userMappings = new Dictionary<string, FieldUserValue>();

            string fallbackUserAccount = System.Configuration.ConfigurationManager.AppSettings.Get("FallbackUserAccount");
            userMappings.Add(fallbackUserAccount, null);

            var sourceUserIds = (from row in listData.AsEnumerable()
                                 select row.Field<string>("Author")).Distinct().Union(
                                     (from row in listData.AsEnumerable()
                                      select row.Field<string>("Editor")).Distinct()).Distinct();

            foreach (var srcUId in sourceUserIds)
            {
                userMappings.Add(srcUId, null);
            }
        }

        /* TODO: For future use; to save/load the history of executions
        ///// <summary>
        ///// Loads the migration history from previous runs
        ///// </summary>
        ///// <remarks>Not currently used or fully implemented.</remarks>
        //private void LoadHistory()
        //{
        //    if (null == history)
        //    {
        //        history = new Dictionary<string, int>();

        //        if (!System.IO.File.Exists("history.dat"))
        //        {
        //            System.IO.File.CreateText("history.dat");
        //        }

        //        var histContents = System.IO.File.ReadAllLines("history.dat");

        //        foreach (var line in histContents)
        //        {
        //            string[] vals = line.Split('#');
        //            history.Add(vals[0], int.Parse(vals[1]));
        //        }
        //    }
        //}

        ///// <summary>
        ///// Saves the migration history
        ///// </summary>
        ///// <remarks>Not currently used or fully implemented.</remarks>
        //private void SaveHistory()
        //{
        //    System.IO.File.WriteAllLines("history.dat",
        //        history.Select(x => x.Key + "#" + x.Value.ToString()).ToArray());
        //}

        */


        /// <summary>
        /// Click event handler of the Exit button
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event arguments</param>
        private void exitButton_Click(object sender, EventArgs e)
        {
            if (null != this.migrateThread && this.migrateThread.ThreadState != ThreadState.Stopped)
            {
                this.migrateThread.Abort();
            }

            Application.Exit();
        }

        /// <summary>
        /// Handles the <c>FormClosed</c> event of the current form.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event arguments</param>
        private void MigrateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != this.migrateThread && this.migrateThread.ThreadState != ThreadState.Stopped)
            {
                this.migrateThread.Abort();
            }

            Application.Exit();
        }

        /// <summary>
        /// Handles the click even of the Migrate button.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments</param>
        private void migrateButton_Click(object sender, EventArgs e)
        {
            string targetListName = targetListsComboBox.SelectedItem.ToString();
            //MigrateData(targetListName);

            migrateThread = new Thread(MigrateData);
            migrateThread.Start(targetListName);

            this.migrateButton.Enabled = false;
            this.metroProgressBar.Visible = true;
        }

        /// <summary>
        /// Updates the migration process Progress bar.
        /// </summary>
        private void ShowProgress()
        {
            this.metroProgressBar.SuspendLayout();
            this.metroProgressBar.Maximum = this.loadProgress.TotalItems;
            this.metroProgressBar.Value = this.loadProgress.Completed;

            //this.infoTextBox.Text = string.Format("Loaded {0} of {1} items...",
            //    this.loadProgress.Completed, this.loadProgress.TotalItems);

            if (this.loadProgress.Completed >= this.loadProgress.TotalItems)
            {
                MessageBox.Show("Migration Complete!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.metroProgressBar.Visible = false;
                this.migrateButton.Enabled = true;
            }

            this.metroProgressBar.ResumeLayout();
        }

        /// <summary>
        /// Appends the input message in execution log
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        private void LogExecutionMessage(string message)
        {

            System.IO.File.AppendAllText(executionLogFilename,
                    string.Format("[{0:yyyy-MM-dd hh:mm:ss tt}] : {1}\r\n", DateTime.Now, message));
        }

        /// <summary>
        /// Migrates the data to specified target list.
        /// </summary>
        /// <param name="targetListName">Name of the target list.</param>
        private void MigrateData(object targetListName)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            //Program.SPContext.Web

            try
            {
                LogExecutionMessage(string.Format("Starting MigrateData for {0} items", listData.Rows.Count));

                this.loadProgress.TotalItems = listData.Rows.Count;
                this.loadProgress.Completed = 0;
                this.Invoke(new MethodInvoker(ShowProgress));

                stopwatch.Start();

                List targetList = Program.SPContext.Web.Lists.GetByTitle((string)targetListName);

                LogExecutionMessage("Loading Target List information");

                Program.SPContext.Load(targetList, l => l.Id, l => l.Title, l => l.RootFolder);
                Program.SPContext.ExecuteQuery();

                LogExecutionMessage(string.Format("Loaded list \"{0}\" ", targetList.Title));


                LogExecutionMessage("Getting data for top level threads");

                var threads = from f in listData.AsEnumerable()
                              where string.IsNullOrEmpty(f.Field<string>("ParentFolderId"))
                              select f;

                int totalThreads = threads.Count();
                LogExecutionMessage(string.Format("Found {0} threads; starting migration", totalThreads));

                for (int i = 0; i < totalThreads; ++i)
                {
                    var threadData = threads.ElementAt(i);

                    LogExecutionMessage(string.Format("Adding item[{0}] (source ID: {1})", i, threadData["ID"]));

                    ListItem newItem = null;
                    try
                    {
                        newItem = AddDiscussionItem(targetList, threadData);

                    }
                    catch (Microsoft.SharePoint.Client.ServerException)
                    {
                        LogExecutionMessage(string.Format(".. [ERROR] : Failed creating item; skipping to the next one."));
                        continue;
                    }

                    ++this.loadProgress.Completed;
                    this.Invoke(new MethodInvoker(ShowProgress));

                    var replies = from f in listData.AsEnumerable()
                                  where string.Equals(f.Field<string>("ParentFolderId"), threadData["ID"])
                                  select f;

                    int totalReplies = replies.Count();
                    LogExecutionMessage(string.Format(".. Adding {0} replies", totalReplies));

                    // Migrate each reply
                    //foreach (var replyData in replies)
                    for (int j = 0; j < totalReplies; ++j)
                    {
                        var replyData = replies.ElementAt(j);

                        ListItem replyItem = null;
                        try
                        {
                            replyItem = AddDiscussionItem(targetList, replyData, newItem);
                        }
                        catch (Microsoft.SharePoint.Client.ServerException)
                        {
                            LogExecutionMessage(string.Format(".... [ERROR] : Failed creating reply [{0}]; skipping to the next one.", j));
                            ++this.loadProgress.Completed;
                            this.Invoke(new MethodInvoker(ShowProgress));
                            continue;
                        }

                        if (string.Equals(replyData["ID"], threadData["BestAnswerId"]))
                        {
                            try
                            {
                                Program.SPContext.Load(newItem);
                                Program.SPContext.ExecuteQuery();

                                newItem["BestAnswerId"] = replyItem.Id;

                                newItem.SystemUpdate();
                                Program.SPContext.ExecuteQuery();
                            }
                            catch (Microsoft.SharePoint.Client.ServerException)
                            {
                                LogExecutionMessage(string.Format(".. [WARNING] : Failed updating Best Answer ID for item[{0}].", i));
                            }
                        }

                        ++this.loadProgress.Completed;
                        this.Invoke(new MethodInvoker(ShowProgress));

                        LogExecutionMessage(string.Format(".... Reply [{0}] Added.", j));

                    } // reply items loop ends

                    LogExecutionMessage(string.Format("Added item[{0}] (target ID: {1})", i, itemIdMappings[threadData["ID"].ToString()]));

                } // top level thread loop ends
            }
            catch (System.Threading.ThreadAbortException)
            {
                MessageBox.Show("Migration aborted by user.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                string crashLogFilename = string.Format("CrashLog_{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);

                System.IO.File.WriteAllText(crashLogFilename, ex.ToString());

                MessageBox.Show("Exiting... Critical Exception occured.\r\nPlease see Crash Log file for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
            finally
            {
                stopwatch.Stop();
                LogExecutionMessage(string.Format("Exiting MigrateData; total execution time: {0}", stopwatch.Elapsed));
            }
        }

        /// <summary>
        /// Add a discussion item (thread and/or reply) to the specified target list.
        /// </summary>
        /// <param name="targetList">The target <see cref="Microsoft.SharePoint.Client.List"/> object</param>
        /// <param name="itemData">A <see cref="System.Data.DataRow"/> object containing the field data for new item to be created.</param>
        /// <param name="parentItem">A valid <see cref="Microsoft.SharePoint.Client.ListItem"/> object if currently item is a reply; null by default</param>
        /// <returns><see cref="Microsoft.SharePoint.Client.ListItem"/> object of the newly created discussion item (thread/reply).</returns>
        /// <remarks>
        /// All the operations performed are also logged in the filename specified in the app.config
        /// </remarks>
        private ListItem AddDiscussionItem(List targetList, DataRow itemData, ListItem parentItem = null)
        {
            bool isReply = null != parentItem;

            string fallbackUserAccount = System.Configuration.ConfigurationManager.AppSettings.Get("FallbackUserAccount");
            var fallbackUserValue = GetTargetUser(fallbackUserAccount);

            var authorUserValue = GetTargetUser(itemData["Author"].ToString());
            var editorUserValue = GetTargetUser(itemData["Editor"].ToString());

            ListItem newItem = null;

            if (!isReply)
            {
                newItem = Utility.CreateNewDiscussion(Program.SPContext, targetList, (string)itemData["Title"]);
                // We Do this to get the ID of the recently created item
                newItem.Update();
                //Program.SPContext.ExecuteQuery();

                Program.SPContext.Load(newItem, t => t.Id, t => t.Folder, t => t.AttachmentFiles);
            }
            else
            {
                newItem = Utility.CreateNewDiscussionReply(Program.SPContext, parentItem);
                // We Do this to get the ID of the recently created item
                newItem.Update();
                //Program.SPContext.ExecuteQuery();

                Program.SPContext.Load(newItem, t => t.Id, t => t.AttachmentFiles);
            }


            Program.SPContext.ExecuteQuery();

            // Get the last created item ID
            string thisItemID = newItem.Id.ToString();
            itemIdMappings[itemData["ID"].ToString()] = thisItemID;

            newItem["Body"] = itemData["Body"];

            if (string.Equals(itemData["Attachments"], "True"))
            {
                // Perform Link correction in the body
                var itemFileRef = itemData["FileRef"].ToString();

                string sourceListUrl = itemFileRef.Substring(0, itemFileRef.LastIndexOf("/"));

                if (isReply)
                {
                    sourceListUrl = sourceListUrl.Substring(0, sourceListUrl.LastIndexOf("/"));
                }

                sourceListUrl = string.Format("{0}/{1}/{2}", sourceListUrl, "Attachments", itemData["ID"].ToString());

                string targetListUrl = string.Format("{0}/{1}/{2}", targetList.RootFolder.ServerRelativeUrl, "Attachments", newItem.Id.ToString());

                newItem["Body"] = itemData["Body"].ToString().Replace(
                    System.Uri.EscapeUriString(sourceListUrl),
                    System.Uri.EscapeUriString(targetListUrl));
            }

            if (authorUserValue.LookupId != -1)
                newItem["Author"] = authorUserValue;
            else if (fallbackUserValue.LookupId != -1)
                newItem["Author"] = fallbackUserValue;

            if (editorUserValue.LookupId != -1)
                newItem["Editor"] = editorUserValue;
            else if (fallbackUserValue.LookupId != -1)
                newItem["Editor"] = fallbackUserValue;

            newItem["IsQuestion"] = itemData["IsQuestion"];
            newItem["IsAnswered"] = itemData["IsAnswered"];

            if (!string.IsNullOrEmpty(itemData["IsFeatured"].ToString()))
            {
                newItem["IsFeatured"] = itemData["IsFeatured"];
            }

            if (isReply && !string.IsNullOrEmpty(itemData["ParentItemID"].ToString()) && itemIdMappings.ContainsKey(itemData["ParentItemID"].ToString()))
            {
                newItem["ParentItemID"] = itemIdMappings[itemData["ParentItemID"].ToString()];
            }

            newItem["Created"] = itemData["Created"].ToString();
            newItem["Modified"] = DateTime.Parse(itemData["Modified"].ToString());


            newItem.Update();
            Program.SPContext.ExecuteQuery();

            if (string.Equals(itemData["Attachments"], "True"))
            {
                string attachmentDirectory = System.IO.Path.Combine(inputFileDirectory, "Attachments", itemData["ID"].ToString());
                string[] attachments = System.IO.Directory.GetFiles(attachmentDirectory);

                LogExecutionMessage(string.Format(".. Uploading {0} attachments", attachments.Count()));

                foreach (string attachmentFile in attachments)
                {
                    var fileInfo = new System.IO.FileInfo(attachmentFile);

                    using (var inputStream = new System.IO.FileStream(attachmentFile, System.IO.FileMode.Open))
                    {
                        try
                        {
                            var attachInfo = new AttachmentCreationInformation();
                            attachInfo.FileName = fileInfo.Name;
                            attachInfo.ContentStream = inputStream;

                            newItem.AttachmentFiles.Add(attachInfo);

                            Program.SPContext.ExecuteQuery();

                            LogExecutionMessage(string.Format(".... attachment {0} uploaded successfully.", fileInfo.Name));
                        }
                        catch (Exception)
                        {
                            LogExecutionMessage(string.Format(".... [ERROR] Failed to uploaded attachment {0}; skipping to the next one", fileInfo.Name));
                            continue;
                        }
                    }

                }
            }

            return newItem;
        }

        /// <summary>
        /// Get the specified user account in the Target environment.
        /// </summary>
        /// <param name="sourceLogonName">Logon name of the user</param>
        /// <returns>Null if input is invalid, a valid <see cref="Microsoft.SharePoint.Client.FieldUserValue"/> object if found; 
        /// a dummy object (LookupId = -1) if specified user and/or the fallback user is not found.</returns>
        private FieldUserValue GetTargetUser(string sourceLogonName)
        {
            if (string.IsNullOrEmpty(sourceLogonName))
            {
                return null;
            }


            if (null == userMappings[sourceLogonName])
            {
                User userObj = null;

                try
                {
                    userObj = Program.SPContext.Web.EnsureUser(sourceLogonName);
                    Program.SPContext.Load(userObj);
                    Program.SPContext.ExecuteQuery();

                    userMappings[sourceLogonName] = new FieldUserValue()
                    {
                        LookupId = userObj.Id
                    };

                }
                catch (Exception)
                {
                    userMappings[sourceLogonName] = new FieldUserValue()
                    {
                        LookupId = -1
                    };
                }
            }

            return userMappings[sourceLogonName];
        }

        /// <summary>
        /// Handles the Link Clicked event of the Link Label
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Object of <see cref="LinkLabelLinkClickedEventArgs"/></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }
    }
}
