using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeDevPnP.Core.AppModelExtensions
{
    /// <summary>
    /// Class that provides methods for variations
    /// </summary>
    public static partial class VariationExtensions
    {
        const string VARIATIONRELATIONSHIPSLISTID = "_VarRelationshipsListId";
        const string VARIATIONLABELSLISTID = "_VarLabelsListId";

        /// <summary>
        /// Configures the variation settings
        /// 1. Go to "Site Actions" -> "Site settings"
        /// 2. Under "Site collection administration", click "Variation Settings".
        /// This method is for the page above to change or update the "Variation Settings"
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="variationSettings">Variation settings</param>
        public static void ConfigureVariationsSettings(this ClientContext context, VariationInformation variationSettings)
        {
            if (variationSettings == null)
            {
                throw new ArgumentException("variationSettings");
            }

            // Get current web
            Web web = context.Web;
            context.Load(web, w => w.ServerRelativeUrl);
            context.ExecuteQueryRetry();

            // Try to get _VarRelationshipsListId property from web property bag
            string variationListId = web.GetPropertyBagValueString(VARIATIONRELATIONSHIPSLISTID, string.Empty);

            if (!string.IsNullOrEmpty(variationListId))
            {
                // Load the lists
                var lists = web.Lists;
                context.Load(lists);
                context.ExecuteQueryRetry();

                // Get the "Variation RelationShips" List by id
                Guid varRelationshipsListId = new Guid(variationListId);
                var variationRelationshipList = lists.GetById(varRelationshipsListId);

                if (variationRelationshipList != null)
                {
                    // Get the root folder
                    Folder rootFolder = variationRelationshipList.RootFolder;
                    context.Load(rootFolder);
                    context.Load(variationRelationshipList);
                    context.ExecuteQueryRetry();

                    // Automatic creation
                    rootFolder.Properties["EnableAutoSpawnPropertyName"] = variationSettings.AutomaticCreation.ToString();

                    // Recreate Deleted Target Page; set to false to enable recreation
                    rootFolder.Properties["AutoSpawnStopAfterDeletePropertyName"] = variationSettings.RecreateDeletedTargetPage.ToString();

                    // Update Target Page Web Parts
                    rootFolder.Properties["UpdateWebPartsPropertyName"] = variationSettings.UpdateTargetPageWebParts.ToString();

                    // Resources
                    rootFolder.Properties["CopyResourcesPropertyName"] = variationSettings.CopyResources.ToString();

                    // Notification
                    rootFolder.Properties["SendNotificationEmailPropertyName"] = variationSettings.SendNotificationEmail.ToString();

                    // Configuration setting site template to be used for the top sites of each label
                    rootFolder.Properties["SourceVarRootWebTemplatePropertyName"] = variationSettings.RootWebTemplate;

                    rootFolder.Update();
                    context.ExecuteQueryRetry();

                    // Get the variationRelationshipList list items
                    ListItemCollection collListItems = variationRelationshipList.GetItems(CamlQuery.CreateAllItemsQuery());
                    context.Load(collListItems);
                    context.ExecuteQueryRetry();

                    if (collListItems.Count > 0)
                    {
                        // Update the first item
                        ListItem item = collListItems[0];
                        item["Deleted"] = false;
                        item["ObjectID"] = web.ServerRelativeUrl;
                        item["ParentAreaID"] = String.Empty;

                        item.Update();
                        context.ExecuteQueryRetry();
                    }
                    else
                    {
                        // Create the new item
                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem olistItem = variationRelationshipList.AddItem(itemCreateInfo);

                        // Root web relationship which should always have this guid 
                        olistItem["GroupGuid"] = new Guid("F68A02C8-2DCC-4894-B67D-BBAED5A066F9");
                        olistItem["Deleted"] = false;
                        olistItem["ObjectID"] = web.ServerRelativeUrl;
                        olistItem["ParentAreaID"] = String.Empty;

                        olistItem.Update();
                        context.ExecuteQueryRetry();
                    }
                }
            }
        }

        /// <summary>
        /// Creates source variation label
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="sourceVariationLabel">Source variation label</param>
        public static void ProvisionSourceVariationLabel(this ClientContext context, VariationLabel sourceVariationLabel)
        {
            if (sourceVariationLabel == null)
            {
                throw new ArgumentException("sourceVariationLabel");
            }

            // Compose the parameters
            List<VariationLabel> sourceVariations = new List<VariationLabel>();
            sourceVariations.Add(sourceVariationLabel);

            // Create source variation label
            CreateVariationLabels(context, sourceVariations);

            WaitForVariationLabelCreation(context, sourceVariationLabel);
        }

        /// <summary>
        /// Creates target variation labels
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="variationLabels">Variation labels</param>
        public static void ProviosionTargetVariationLabels(this ClientContext context, List<VariationLabel> variationLabels)
        {
            if (variationLabels == null)
            {
                throw new ArgumentException("variationLabels");
            }

            // Get the target variation labels
            List<VariationLabel> targetVariations = variationLabels.Where(x => x.IsSource == false).ToList();

            // Create target variation labels
            if ((targetVariations != null) && (targetVariations.Count > 0))
            {
                CreateVariationLabels(context, targetVariations);
            }
        }

        /// <summary>
        /// Wait for the variation label creation
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="variationLabel">Variation label</param>
        public static void WaitForVariationLabelCreation(this ClientContext context, VariationLabel variationLabel)
        {
            if (variationLabel == null)
            {
                throw new ArgumentException("variationLabel");
            }

            while (!CheckForHierarchyCreation(context, variationLabel))
            {
                // Wait for 60 seconds and then try again
                System.Threading.Thread.Sleep(60000);
            }
        }

        #region Helper methods

        /// <summary>
        /// Create variation labels
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="variationLabels">Variation labels</param>
        private static void CreateVariationLabels(this ClientContext context, List<VariationLabel> variationLabels)
        {
            // Get current web
            Web web = context.Web;
            context.Load(web, w => w.ServerRelativeUrl);
            context.ExecuteQueryRetry();

            // Try to get _VarLabelsListId property from web property bag
            string variationLabelsListId = web.GetPropertyBagValueString(VARIATIONLABELSLISTID, string.Empty);

            if (!string.IsNullOrEmpty(variationLabelsListId))
            {
                // Load the lists
                var lists = web.Lists;
                context.Load(lists);
                context.ExecuteQueryRetry();

                // Get the "Variation Labels" List by id
                Guid varRelationshipsListId = new Guid(variationLabelsListId);
                var variationLabelsList = lists.GetById(varRelationshipsListId);

                // Get the variationLabelsList list items
                ListItemCollection collListItems = variationLabelsList.GetItems(CamlQuery.CreateAllItemsQuery());
                context.Load(collListItems);
                context.ExecuteQueryRetry();

                if (variationLabelsList != null)
                {
                    foreach (VariationLabel label in variationLabels)
                    {
                        // Check if variation label already exists
                        var varLabel = collListItems.FirstOrDefault(x => x["Language"].ToString() == label.Language);

                        if (varLabel == null)
                        {
                            // Create the new item
                            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                            ListItem olistItem = variationLabelsList.AddItem(itemCreateInfo);

                            olistItem["Title"] = label.Title;
                            olistItem["Description"] = label.Description;
                            olistItem["Flag_x0020_Control_x0020_Display"] = label.FlagControlDisplayName;
                            olistItem["Language"] = label.Language;
                            olistItem["Locale"] = label.Locale;
                            olistItem["Hierarchy_x0020_Creation_x0020_M"] = label.HierarchyCreationMode;
                            olistItem["Is_x0020_Source"] = label.IsSource;
                            olistItem["Hierarchy_x0020_Is_x0020_Created"] = false;

                            olistItem.Update();
                            context.ExecuteQueryRetry();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if hierarchy is created for the variation label.
        /// Get the "Hierarchy_x0020_Is_x0020_Created" list item value
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="variationLabel">Variation label</param>
        /// <returns>True, if hierarchy is created for the variation label</returns>
        private static bool CheckForHierarchyCreation(this ClientContext context, VariationLabel variationLabel)
        {
            bool hierarchyIsCreated = false;

            // Get current web
            Web web = context.Web;
            context.Load(web, w => w.ServerRelativeUrl);
            context.ExecuteQueryRetry();

            // Try to get _VarLabelsListId property from web property bag
            string variationLabelsListId = web.GetPropertyBagValueString(VARIATIONLABELSLISTID, string.Empty);

            if (!string.IsNullOrEmpty(variationLabelsListId))
            {
                // Load the lists
                var lists = web.Lists;
                context.Load(lists);
                context.ExecuteQueryRetry();

                // Get the "Variation Labels" List by id
                Guid varRelationshipsListId = new Guid(variationLabelsListId);
                var variationLabelsList = lists.GetById(varRelationshipsListId);

                // Get the variationLabelsList list items
                ListItemCollection collListItems = variationLabelsList.GetItems(CamlQuery.CreateAllItemsQuery());
                context.Load(collListItems);
                context.ExecuteQueryRetry();

                if (variationLabelsList != null)
                {
                    // Check hierarchy is created
                    ListItem varLabel = collListItems.FirstOrDefault(x => x["Language"].ToString() == variationLabel.Language);
                    hierarchyIsCreated = (bool)varLabel["Hierarchy_x0020_Is_x0020_Created"];
                }
            }

            return hierarchyIsCreated;
        }

        #endregion
    }
}
