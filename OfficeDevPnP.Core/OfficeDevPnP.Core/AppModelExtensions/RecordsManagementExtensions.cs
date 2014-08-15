using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;


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
        public const string ECM_LIST_READY_FOR_IPR = "ecm_ListReadyForIPR";
        public const string ECM_AUTO_DECLARE_RECORDS = "ecm_AutoDeclareRecords";
        public const string ECM_LIST_FIELDS_READY_FOR_IPR = "ecm_ListFieldsReadyForIPR";


        #region Site scoped In Place Records Management methods
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
        
        public static void ActivateInPlaceRecordsManagementFeature(this Site site)
        {
            site.ActivateFeature(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID));            
        }

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

        public static void SetManualRecordDeclarationInAllLocations(this Site site, bool inAllPlaces)
        {
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_DECLARATION_DEFAULT, inAllPlaces.ToString());
        }

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


        public static void SetRecordDeclarationBy(this Site site, EcmRecordDeclarationBy by)
        {
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_DECLARATION_BY, by.ToString());
        }

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

        public static void SetRecordUnDeclarationBy(this Site site, EcmRecordDeclarationBy by)
        {
            site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_UNDECLARATION_BY, by.ToString());
        }

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
        }

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

        public static void SetListAutoRecordDeclaration(this List list, bool autoDeclareRecords, int sharePointVersion = 16)
        {

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

        public static bool GetListAutoRecordDeclaration(this List list)
        {
            string autoDeclare = list.GetPropertyBagValueString(ECM_AUTO_DECLARE_RECORDS, "");

            if (!String.IsNullOrEmpty(autoDeclare))
            {
                bool auto = false;
                if (Boolean.TryParse(autoDeclare, out auto))
                {
                    return true;
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
                    eventReceiver.ReceiverUrl.Equals(receiverToAdd.ReceiverUrl) &&
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
