using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System.Reflection;


namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with records management functionality
    /// </summary>
    public static class RecordsManagementExtensions
    {
        public const string INPLACE_RECORDS_MANAGEMENT_FEATURE_ID = "da2e115b-07e4-49d9-bb2c-35e93bb9fca9";
        public const string ECM_SITE_RECORD_DECLARATION_DEFAULT = "ecm_siterecorddeclarationdefault";
        public const string ECM_SITE_RECORD_RESTRICTIONS = "ecm_siterecordrestrictions";
        public const string ECM_SITE_RECORD_DECLARATION_BY = "ecm_siterecorddeclarationby";
        public const string ECM_SITE_RECORD_UNDECLARATION_BY = "ecm_siterecordundeclarationby";
        public const string ECM_ALLOW_MANUAL_DECLARATION = "ecm_AllowManualDeclaration";
        public const string ECM_IPR_LIST_USE_LIST_SPECIFIC = "ecm_IPRListUseListSpecific";
        public const string ECM_AUTO_DECLARE_RECORDS = "ecm_AutoDeclareRecords";


        #region Site scoped In Place Records Management methods
        /// <summary>
        /// Checks if in place records management functionality is enabled for this site collection
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <returns>True if in place records management is enabled, false otherwise</returns>
        public static bool IsInPlaceRecordsManagementActive(this Site site)
        {
            // First requirement is that the feature is active
            if (site.IsFeatureActive(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID)))
            {
                // Check to see if the necesarry property bag entries are defined
                if (String.IsNullOrEmpty(site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_RESTRICTIONS, "")) ||
                    String.IsNullOrEmpty(site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_DECLARATION_BY, "")) ||
                    String.IsNullOrEmpty(site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_UNDECLARATION_BY, "")))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Activate the in place records management feature
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        public static void ActivateInPlaceRecordsManagementFeature(this Site site)
        {
            site.ActivateFeature(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID));            
        }

        /// <summary>
        /// Deactivate the in place records management feature
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        public static void DisableInPlaceRecordsManagementFeature(this Site site)
        {
            site.DeactivateFeature(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID)); 
        }

        /// <summary>
        /// Enable in place records management. The in place records management feature will be enabled and 
        /// the in place record management will be enabled in all locations with record declaration allowed 
        /// by all contributors and undeclaration by site admins
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        public static void EnableSiteForInPlaceRecordsManagement(this Site site)
        {
            // Activate the "In place records management" feature if not yet enabled
            if (!site.IsFeatureActive(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID)))
            {
                //Note: this also sets the ECM_SITE_RECORD_RESTRICTIONS value to "BlockDelete, BlockEdit"
                site.ActivateInPlaceRecordsManagementFeature();
            }

            // Enable in place records management in all locations
            site.SetManualRecordDeclarationInAllLocations(true);

            // Set restrictions to default values after enablement (is also done at feature activation)
            EcmSiteRecordRestrictions restrictions = EcmSiteRecordRestrictions.BlockDelete | EcmSiteRecordRestrictions.BlockEdit;
            site.SetRecordRestrictions(restrictions);

            // Set record declaration to default value
            site.SetRecordDeclarationBy(EcmRecordDeclarationBy.AllListContributors);

            // Set record undeclaration to default value
            site.SetRecordUnDeclarationBy(EcmRecordDeclarationBy.OnlyAdmins);

        }

        /// <summary>
        /// Defines if in place records management is allowed in all places
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <param name="inAllPlaces">True if allowed in all places, false otherwise</param>
        public static void SetManualRecordDeclarationInAllLocations(this Site site, bool inAllPlaces)
        {
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_DECLARATION_DEFAULT, inAllPlaces.ToString());
        }

        /// <summary>
        /// Get the value of the records management is allowed in all places setting
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <returns>True if records management is allowed in all places, false otherwise</returns>
        public static bool GetManualRecordDeclarationInAllLocations(this Site site)
        {
            string manualDeclare = site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_DECLARATION_DEFAULT, "");

            if (!String.IsNullOrEmpty(manualDeclare))
            {
                bool manual = false;
                if (Boolean.TryParse(manualDeclare, out manual))
                {
                    return manual;
                }
            }

            return false;

        }

        /// <summary>
        /// Defines the restrictions that are placed on a document once it's declared as a record
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <param name="restrictions"><see cref="EcmSiteRecordRestrictions"/> enum that holds the restrictions to be applied</param>
        public static void SetRecordRestrictions(this Site site, EcmSiteRecordRestrictions restrictions)
        {
            string restrictionsProperty = "";

            if (restrictions.Has(EcmSiteRecordRestrictions.None))
            {
                restrictionsProperty = EcmSiteRecordRestrictions.None.ToString();
            }
            else if (restrictions.Has(EcmSiteRecordRestrictions.BlockEdit))
            {
                // BlockEdit is always used in conjunction with BlockDelete
                restrictionsProperty = EcmSiteRecordRestrictions.BlockDelete.ToString() + ", " + EcmSiteRecordRestrictions.BlockEdit.ToString();
            }
            else if (restrictions.Has(EcmSiteRecordRestrictions.BlockDelete))
            {
                restrictionsProperty = EcmSiteRecordRestrictions.BlockDelete.ToString();
            }

            // Set property bag entry
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_RESTRICTIONS, restrictionsProperty);
        }

        /// <summary>
        /// Gets the current restrictions on declared records
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <returns><see cref="EcmSiteRecordRestrictions"/> enum that defines the current restrictions</returns>
        public static EcmSiteRecordRestrictions GetRecordRestrictions(this Site site)
        {
            EcmSiteRecordRestrictions result = EcmSiteRecordRestrictions.None;
            result = result.Remove<EcmSiteRecordRestrictions>(EcmSiteRecordRestrictions.None);

            string restrictionString = site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_RESTRICTIONS, "");

            if (!String.IsNullOrEmpty(restrictionString))
            {
                string[] restrictions = restrictionString.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string restriction in restrictions)
                {
                    EcmSiteRecordRestrictions value;
                    if (Enum.TryParse<EcmSiteRecordRestrictions>(restriction, out value))
                    {
                        result = result.Include<EcmSiteRecordRestrictions>(value);                        
                    }
                }

                return result;
            }

            //Throw exception as apparently in place records management has not yet been setup.
            throw new Exception("No ECM_SITE_RECORD_RESTRICTIONS setting defined"); 
        }

        /// <summary>
        /// Defines who can declare records
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <param name="by"><see cref="EcmRecordDeclarationBy"/> enum that defines who can declare a record</param>
        public static void SetRecordDeclarationBy(this Site site, EcmRecordDeclarationBy by)
        {
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_DECLARATION_BY, by.ToString());
        }

        /// <summary>
        /// Gets who can declare records
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <returns><see cref="EcmRecordDeclarationBy"/> enum that defines who can declare a record</returns>
        public static EcmRecordDeclarationBy GetRecordDeclarationBy(this Site site)
        {
            string by = site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_DECLARATION_BY, "");

            EcmRecordDeclarationBy result;

            if (!String.IsNullOrEmpty(by))
            {
                if (Enum.TryParse<EcmRecordDeclarationBy>(by, out result))
                {
                    return result;
                }
            }

            //Throw exception as apparently in place records management has not yet been setup.
            throw new Exception("No ECM_SITE_RECORD_DECLARATION_BY setting defined");            
        }

        /// <summary>
        /// Defines who can undeclare records
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <param name="by"><see cref="EcmRecordDeclarationBy"/> enum that defines who can undeclare a record</param>
        public static void SetRecordUnDeclarationBy(this Site site, EcmRecordDeclarationBy by)
        {
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_UNDECLARATION_BY, by.ToString());
        }

        /// <summary>
        /// Gets who can undeclare records
        /// </summary>
        /// <param name="site">Site collection to operate on</param>
        /// <returns><see cref="EcmRecordDeclarationBy"/> enum that defines who can undeclare a record</returns>

        public static EcmRecordDeclarationBy GetRecordUnDeclarationBy(this Site site)
        {
            string by = site.RootWeb.GetPropertyBagValueString(ECM_SITE_RECORD_UNDECLARATION_BY, "");

            EcmRecordDeclarationBy result;

            if (!String.IsNullOrEmpty(by))
            {
                if (Enum.TryParse<EcmRecordDeclarationBy>(by, out result))
                {
                    return result;
                }
            }

            //Throw exception as apparently in place records management has not yet been setup.
            throw new Exception("No ECM_SITE_RECORD_UNDECLARATION_BY setting defined");
        }
        #endregion

        #region List scoped In Place Records Management methods
        /// <summary>
        /// Checks if this list has active in place records management settings defined
        /// </summary>
        /// <param name="list">List to operate against</param>
        /// <returns>True if in place records management settings are active for this list</returns>
        public static bool IsListRecordSettingDefined(this List list)
        {
            string useListSpecific = list.GetPropertyBagValueString(ECM_IPR_LIST_USE_LIST_SPECIFIC, "");

            if (!String.IsNullOrEmpty(useListSpecific))
            {
                bool listSpecific = false;
                if (Boolean.TryParse(useListSpecific, out listSpecific))
                {
                    return listSpecific;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Defines the manual in place record declaration for this list
        /// </summary>
        /// <param name="list">List to operate against</param>
        /// <param name="settings"><see cref="EcmListManualRecordDeclaration"/> enum that defines the manual in place record declaration settings for this list</param>
        public static void SetListManualRecordDeclaration(this List list, EcmListManualRecordDeclaration settings)
        {
            if (settings == EcmListManualRecordDeclaration.UseSiteCollectionDefaults)
            {
                //If we set list record declaration back to the default values then we also need to 
                //turn off auto record declaration. Other property bag values are left as is: when 
                //settings are changed again these properties are also again usable
                if (list.PropertyBagContainsKey(ECM_AUTO_DECLARE_RECORDS))
                {
                    list.SetListAutoRecordDeclaration(false);
                }
                //Set the property that dictates custom list record settings to false
                list.SetPropertyBagValue(ECM_IPR_LIST_USE_LIST_SPECIFIC, false.ToString());
            }
            else if (settings == EcmListManualRecordDeclaration.AlwaysAllowManualDeclaration)
            {
                list.SetPropertyBagValue(ECM_ALLOW_MANUAL_DECLARATION, true.ToString());
                //Set the property that dictates custom list record settings to true
                list.SetPropertyBagValue(ECM_IPR_LIST_USE_LIST_SPECIFIC, true.ToString());
            } 
            else if (settings == EcmListManualRecordDeclaration.NeverAllowManualDeclaration)
            {
                list.SetPropertyBagValue(ECM_ALLOW_MANUAL_DECLARATION, false.ToString());
                //Set the property that dictates custom list record settings to true
                list.SetPropertyBagValue(ECM_IPR_LIST_USE_LIST_SPECIFIC, true.ToString());
            }
            else
            {
                throw new ArgumentOutOfRangeException("settings");
            }
        }

        /// <summary>
        /// Gets the manual in place record declaration for this list
        /// </summary>
        /// <param name="list">List to operate against</param>
        /// <returns><see cref="EcmListManualRecordDeclaration"/> enum that defines the manual in place record declaration settings for this list</returns>
        public static EcmListManualRecordDeclaration GetListManualRecordDeclaration(this List list)
        {
            string useListSpecific = list.GetPropertyBagValueString(ECM_IPR_LIST_USE_LIST_SPECIFIC, "");

            if (!String.IsNullOrEmpty(useListSpecific))
            {
                bool listSpecific = false;
                if (Boolean.TryParse(useListSpecific, out listSpecific))
                {
                   if (!listSpecific)
                   {
                       return EcmListManualRecordDeclaration.UseSiteCollectionDefaults;
                   }
                   else
                   {
                       string manualDeclararion = list.GetPropertyBagValueString(ECM_ALLOW_MANUAL_DECLARATION, "");
                       bool manual = false;
                       if (Boolean.TryParse(manualDeclararion, out manual))
                       {
                           if (manual)
                           {
                               return EcmListManualRecordDeclaration.AlwaysAllowManualDeclaration;
                           }
                           else
                           {
                               return EcmListManualRecordDeclaration.NeverAllowManualDeclaration;
                           }
                       }
                   }
                }
            }

            //Throw exception as apparently in place records management has not yet been setup.
            throw new Exception("No ECM_SITE_RECORD_UNDECLARATION_BY setting defined");
        }

        /// <summary>
        /// Defines if auto record declaration is active for this list: all added items will be automatically declared as a record if active
        /// </summary>
        /// <param name="list">List to operate on</param>
        /// <param name="autoDeclareRecords">True to automatically declare all added items as record, false otherwise</param>
        public static void SetListAutoRecordDeclaration(this List list, bool autoDeclareRecords)
        {
            //Determine the SharePoint version based on the loaded CSOM library
            Assembly asm = Assembly.GetAssembly(typeof(Microsoft.SharePoint.Client.Site));
            int sharePointVersion = asm.GetName().Version.Major;

            if (autoDeclareRecords)
            {
                //Set the property that dictates custom list record settings to true
                list.SetPropertyBagValue(ECM_IPR_LIST_USE_LIST_SPECIFIC, true.ToString());
                //Prevent manual declaration
                list.SetPropertyBagValue(ECM_ALLOW_MANUAL_DECLARATION, false.ToString());

                //Hookup the needed event handlers
                list.Context.Load(list.EventReceivers);
                list.Context.ExecuteQuery();

                List<EventReceiverDefinition> currentEventReceivers = new List<EventReceiverDefinition>(list.EventReceivers.Count);
                currentEventReceivers.AddRange(list.EventReceivers);

                // Track changes to see if an list.Update is needed
                bool eventReceiverAdded = false;
                
                //ItemUpdating receiver
                EventReceiverDefinitionCreationInformation newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemUpdating, 1000, sharePointVersion);
                if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
                {
                    list.EventReceivers.Add(newEventReceiver);
                    eventReceiverAdded = true;
                }
                //ItemDeleting receiver
                newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemDeleting, 1000, sharePointVersion);
                if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
                {
                    list.EventReceivers.Add(newEventReceiver);
                    eventReceiverAdded = true;
                }
                //ItemFileMoving receiver
                newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemFileMoving, 1000, sharePointVersion);
                if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
                {
                    list.EventReceivers.Add(newEventReceiver);
                    eventReceiverAdded = true;
                }
                //ItemAdded receiver
                newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemAdded, 1005, sharePointVersion);
                if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
                {
                    list.EventReceivers.Add(newEventReceiver);
                    eventReceiverAdded = true;
                }
                //ItemUpdated receiver
                newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemUpdated, 1007, sharePointVersion);
                if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
                {
                    list.EventReceivers.Add(newEventReceiver);
                    eventReceiverAdded = true;
                }
                //ItemCheckedIn receiver
                newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemCheckedIn, 1006, sharePointVersion);
                if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
                {
                    list.EventReceivers.Add(newEventReceiver);
                    eventReceiverAdded = true;
                }
                                
                if (eventReceiverAdded)
                {
                    list.Update();
                    list.Context.ExecuteQuery();
                }

                //Set the property that dictates the auto declaration
                list.SetPropertyBagValue(ECM_AUTO_DECLARE_RECORDS, autoDeclareRecords.ToString());
            }
            else
            {
                //Set the property that dictates the auto declaration
                list.SetPropertyBagValue(ECM_AUTO_DECLARE_RECORDS, autoDeclareRecords.ToString());
                //Note: existing list event handlers will just stay as they are, no need to remove them
            }
        }

        /// <summary>
        /// Returns if auto record declaration is active for this list
        /// </summary>
        /// <param name="list">List to operate against</param>
        /// <returns>True if auto record declaration is active, false otherwise</returns>
        public static bool GetListAutoRecordDeclaration(this List list)
        {
            string autoDeclare = list.GetPropertyBagValueString(ECM_AUTO_DECLARE_RECORDS, "");

            if (!String.IsNullOrEmpty(autoDeclare))
            {
                bool auto = false;
                if (Boolean.TryParse(autoDeclare, out auto))
                {
                    return auto;
                }
            }

            return false;
        }

        private static EventReceiverDefinitionCreationInformation CreateECMRecordEventReceiverDefinition(EventReceiverType eventType, int sequenceNumber, int sharePointVersion)
        {
            return new EventReceiverDefinitionCreationInformation()
            {
                EventType = eventType,
                ReceiverAssembly = String.Format("Microsoft.Office.Policy, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c", sharePointVersion),
                ReceiverClass = "Microsoft.Office.RecordsManagement.Internal.HoldEventReceiver",
                ReceiverName = "ECM_RecordEventReceiver",
                ReceiverUrl = "",
                SequenceNumber = sequenceNumber,
                Synchronization = EventReceiverSynchronization.Synchronous
            };
        }

        private static bool ContainsECMRecordEventReceiver(EventReceiverDefinitionCreationInformation receiverToAdd, List<EventReceiverDefinition> currentEventReceivers)
        {
            foreach(EventReceiverDefinition eventReceiver in currentEventReceivers)
            {
                if (eventReceiver.EventType.Equals(receiverToAdd.EventType) &&
                    eventReceiver.ReceiverAssembly.Equals(receiverToAdd.ReceiverAssembly) &&
                    eventReceiver.ReceiverClass.Equals(receiverToAdd.ReceiverClass) &&
                    eventReceiver.ReceiverName.Equals(receiverToAdd.ReceiverName) &&
                    eventReceiver.SequenceNumber.Equals(receiverToAdd.SequenceNumber) &&
                    eventReceiver.Synchronization.Equals(receiverToAdd.Synchronization))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
