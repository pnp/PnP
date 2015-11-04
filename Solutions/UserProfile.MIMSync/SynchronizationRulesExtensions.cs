using Microsoft.MetadirectoryServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SynchronizationRulesExtensions
{
    /// <summary>
    /// MV rules 
    /// </summary>
    /// <remarks>
    /// The IMVSynchronization interface is implemented by a Microsoft Identity Integration Server 
    /// rules extension to provide rules extension functionality for provisioning.
    /// </remarks>
    public class MVExtensionObject : IMVSynchronization
    {
        /// <summary>
        /// IMVSynchronization.Initialize
        /// Initializes the rules extension object. The Identity Integration Server calls this method 
        /// when it loads the extension. This method is also called if you drop a new or 
        /// updated assembly into the extensions folder.
        /// </summary>
        void IMVSynchronization.Initialize(){}

        /// <summary>
        /// IMVSynchronization.Terminate
        /// Called when the rules extension object is no longer needed. The Identity Integration Server 
        /// calls this method when the extension is unloaded, which normally occurs after 5 minutes of 
        /// inactivity. Note that you cannot change the inactivity period and should not assume the 
        /// period will always remain as 5 minutes in subsequent releases.
        /// </summary>
        void IMVSynchronization.Terminate(){}

        /// <summary>
        /// IMVSynchronization.Provision
        /// Evaluates connected objects in response to changes to a metaverse object. 
        /// The Identity Integration Server calls this method during a management agent run 
        /// when synchronization rules cause a change in the metaverse object.
        /// </summary>
        /// <param name="mventry"></param>
        void IMVSynchronization.Provision(MVEntry mventry)
        {
            ProvisionUPSA(mventry, "SPMA");
        }

        /// <summary>
        /// IMVSynchronization.ShouldDeleteFromMV
        /// Determines if the metaverse object should be deleted along with the connector space object 
        /// after a connector space object has been disconnected from a metaverse object during inbound 
        /// synchronization. The Identity Integration Server calls this method when an object deletion rule, 
        /// which was configured in Identity Manager to use a rules extension, is triggered.
        /// </summary>
        /// <param name="csentry"></param>
        /// <param name="mventry"></param>
        /// <returns></returns>
        bool IMVSynchronization.ShouldDeleteFromMV(CSEntry csentry, MVEntry mventry)
        {
            throw new EntryPointNotImplementedException();
        }

        void ProvisionUPSA(MVEntry mventry, string MA_name)
        {
            var attr_map = new Dictionary<string, string>()
            {
                {"accountName",     "AccountName"},
                {"department",      "Department"},
                {"displayName",     "UserName"},
                {"mail",            "WorkEmail"},
                {"objectSid",       "SID"},
                {"lastName",        "LastName"},
                {"firstName",       "FirstName"},
                {"telephoneNumber", "WorkPhone"},
            };

            ConnectedMA ManagementAgent = mventry.ConnectedMAs[MA_name];
            int Connectors = ManagementAgent.Connectors.Count;

            if (0 == Connectors && mventry.ObjectType.Equals("person", StringComparison.OrdinalIgnoreCase))
            {
                if (mventry["accountName"].IsPresent)
                {
                    string anchor = mventry["accountName"].Value;
                    CSEntry csentry = ManagementAgent.Connectors.StartNewConnector("user");

                    AttributeNameEnumerator iter = mventry.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        string CS_AttrName;
                        if (attr_map.TryGetValue(iter.Current, out CS_AttrName))
                        {
                            csentry[CS_AttrName].Value = mventry[iter.Current].Value;
                        }
                    }
                    csentry["Anchor"].Value = anchor;
                    csentry.CommitNewConnector();
                }
            }
            if (mventry.ObjectType.ToLower() == "group" && 0 == Connectors)
            {
                try
                {
                    if (mventry["accountName"].IsPresent)
                    {
                        string anchor = mventry["accountName"].Value;
                        CSEntry csentry = ManagementAgent.Connectors.StartNewConnector("group");
                        csentry["Anchor"].Value = anchor;
                        csentry.CommitNewConnector();

                    }
                }
                catch (ObjectAlreadyExistsException)
                {
                    // Suppress the exception when an object exists with same distinguished name in the connector space.
                    // The object should join on the next inbound synchronization run
                }

            }
            if (mventry.ObjectType.ToLower() == "contact" && 0 == Connectors)
            {
                try
                {
                    if (mventry["sAMAccountName"].IsPresent)
                    {
                        string anchor = mventry["sAMAccountName"].Value;
                        CSEntry csentry = ManagementAgent.Connectors.StartNewConnector("contact");
                        csentry["Anchor"].Value = anchor;
                        csentry.CommitNewConnector();
                    }
                }
                catch (ObjectAlreadyExistsException)
                {
                    // Suppress the exception when an object exists with same distinguished name in the connector space.
                    // The object should join on the next inbound synchronization run
                }
            }
        }
    }

    /// <summary>
    /// MA rules
    /// </summary>
    /// <remarks>
    /// The IMASynchronization interface is implemented by a Microsoft Identity Integration Server 
    /// rules extension to provide rules extension functionality for a management agent.
    /// </remarks>
    public class MAExtensionObject : IMASynchronization
    {
        /// <summary>
        /// The IMASynchronization.Initialize method initializes the rules extension object.
        /// </summary>
        /// <remarks>
        /// If an exception occurs in this method, the IMASynchronization.Terminate method is not called. 
        /// If the IMASynchronization.Terminate method releases any resources allocated in the initialize method, 
        /// those resources remain when an exception occurs in this method because the IMASynchronization.Terminate 
        /// method is not called. Release any resources allocated in this method as part of your exception handling routine.
        /// </remarks>
        void IMASynchronization.Initialize(){}

        /// <summary>
        /// The IMASynchronization.Terminate method is called when the rules extension object is no longer needed. This method is used to free resources owned by the rules extension.
        /// </summary>
        void IMASynchronization.Terminate(){}

        /// <summary>
        /// The ShouldProjectToMV method is called to determine if a new connector space object should be projected to a new metaverse object when the connector space object does not join to an existing metaverse object.
        /// </summary>
        /// <param name="csentry">Contains a CSEntry object that represents the new connector space entry.</param>
        /// <param name="MVObjectType">A String object that, on output, receives the name of the metaverse class to which the connector space entry should be projected.</param>
        /// <returns>
        /// Returns True if the connector space entry should be projected. The MVObjectType parameter receives the name of the metaverse class to which the connector space entry should be projected.
        /// Returns False if the connector space entry should not be projected.
        /// </returns>
        bool IMASynchronization.ShouldProjectToMV(CSEntry csentry, out string MVObjectType)
        {
            string sourceObjectDN;

            switch (csentry.ObjectType)
            {
                case "user":
                    MVObjectType = "Person";

                    // Cross-forest domain accounts are projected as contacts. They will be flowed to 
                    // to MOSS to establish the cross-forest relationship in order to resolve to 
                    // forest (master) account if user logs in using cross-forest (subordinate) account.
                    if (csentry["msDS-SourceObjectDN"].IsPresent)
                    {
                        sourceObjectDN = csentry["msDS-SourceObjectDN"].Value;
                        if ((!String.IsNullOrEmpty(sourceObjectDN)) &&
                            (sourceObjectDN != csentry.DN.ToString()))
                        {
                            MVObjectType = "contact";
                        }
                    }
                    break;

                case "group":
                    MVObjectType = "group";

                    // Ignore groups for which hideDLMembership == true
                    if (csentry["hideDLMembership"].IsPresent)
                    {
                        Attrib attribute = csentry["hideDLMembership"];
                        if (attribute != null && attribute.IsPresent && attribute.BooleanValue)
                        {
                            return false;
                        }
                    }
                    break;

                case "contact":
                    MVObjectType = "contact";

                    // Ignore contacts without sourceObjectDN or self-referencing ones
                    sourceObjectDN = csentry["msDS-SourceObjectDN"].Value;
                    if (String.IsNullOrEmpty(sourceObjectDN) || String.Compare(sourceObjectDN, csentry.DN.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                        return false;

                    break;

                default:
                    throw new UnexpectedDataException("Unsupported object type - " + csentry.ObjectType);
            }
            return true;
        }

        /// <summary>
        /// The Deprovision method is called when a metaverse entry is deleted and the 
        /// connector space entries connected to the metaverse entry become disconnector objects.
        /// </summary>
        /// <param name="csentry">Contains a CSEntry object that represents the connector space entry that was connected to the deleted metaverse entry.</param>
        /// <returns>Returns one of the DeprovisionAction values that determines which action should be taken on the connector space entry.</returns>
        DeprovisionAction IMASynchronization.Deprovision(CSEntry csentry)
        {
            throw new EntryPointNotImplementedException();
        }

        /// <summary>
        /// The FilterForDisconnection method determines if a connector CSEntry object will be disconnected. 
        /// A connector space or CSEntry object is disconnected when a delta export matches a filter or if 
        /// the filter rules are changed and the new filter criteria for disconnecting an object are met.
        /// </summary>
        /// <param name="csentry">Contains the CSEntry object to which this method applies.</param>
        /// <returns>Returns True if the object will be disconnected or False if the object will not be disconnected.</returns>
        bool IMASynchronization.FilterForDisconnection(CSEntry csentry)
        {
            throw new EntryPointNotImplementedException();
        }

        /// <summary>
        /// The MapAttributesForJoin method generates a list of values based on the CSEntry attribute values that will be used to search the metaverse.
        /// </summary>
        /// <param name="FlowRuleName">Contains the name of the flow rule. You must use only alphanumeric characters for the FlowRuleName parameter, otherwise you can encounter problems in a rules extension.</param>
        /// <param name="csentry">Contains a CSEntry object that represents the connector space entry.</param>
        /// <param name="values">Contains a ValueCollection object that receives the list of attribute values generated by this method to be used to search the metaverse.</param>
        void IMASynchronization.MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            throw new EntryPointNotImplementedException();
        }

        /// <summary>
        /// The ResolveJoinSearch method is called when a join rule is configured to use a rules extension to resolve conflicts, and when one or more results from a metaverse search match the values that are generated by the IMASynchronization.MapAttributesForJoin method.
        /// </summary>
        /// <param name="joinCriteriaName">Contains a string that contains the name of the join criteria. You must use only alphanumeric characters for the joinCriteriaName parameter, otherwise you can encounter problems in a rules extension.</param>
        /// <param name="csentry">Contains the CSEntry object that represents the connector space entry that will be joined to the metaverse entry.</param>
        /// <param name="rgmventry">Contains an array of MVEntry objects that represent the metaverse entries that match the join operation criteria. On return, the imventry parameter receives the index of the object in this array to which the connector space entry will be joined.</param>
        /// <param name="imventry">If the method returns True, this parameter receives the index of the object in the rgmventry parameter to which the connector space entry will be joined.</param>
        /// <param name="MVObjectType">Contains a string that contains the name of the metaverse class.</param>
        /// <returns></returns>
        bool IMASynchronization.ResolveJoinSearch(string joinCriteriaName, CSEntry csentry, MVEntry[] rgmventry, out int imventry, ref string MVObjectType)
        {
            throw new EntryPointNotImplementedException();
        }

        /// <summary>
        /// The MapAttributesForImport method is called to map attributes from a connector space entry to a metaverse entry.
        /// </summary>
        /// <param name="FlowRuleName">Contains the name of the flow rule. You must use only alphanumeric characters for the FlowRuleName parameter, otherwise you can encounter problems in a rules extension.</param>
        /// <param name="csentry">Contains a CSEntry object that represents the source connector space entry.</param>
        /// <param name="mventry">Contains an MVEntry object that represents the destination metaverse entry.</param>
        void IMASynchronization.MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            string ruleName;
            string csAttribute;
            string mvAttribute;

            switch (FlowRuleName)
            {
                case "DomainSIDToString":
                    try
                    {
                        if (csentry["objectSID"].IsPresent)
                        {
                            mventry["stringSID"].Value = Utils.ConvertSidToString(csentry["objectSID"].BinaryValue);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new UnexpectedDataException("Error while processing DomainSIDToString rule extension: " + e.ToString(), e);
                    }
                    break;

                case "ConvertToString:nCName,nCName":

                    GetFlowRuleParameters(FlowRuleName, out ruleName, out csAttribute, out mvAttribute);
                    // Used to convert Reference to string DN
                    mventry[mvAttribute].Value = csentry[csAttribute].Value.ToString();
                    break;

                case "ProxyAddressesToSipAddress:proxyAddresses,SPS-SipAddress":
                    // Used to get the Sip Address from the ProxyAddresses attribute

                    GetFlowRuleParameters(FlowRuleName, out ruleName, out csAttribute, out mvAttribute);
                    try
                    {
                        String[] valueArray;
                        if (csentry[csAttribute].IsPresent)
                        {
                            // Usually proxy address is multivalue, but just in case it is checked here and handled accordingly
                            if (csentry[csAttribute].IsMultivalued == true)
                            {
                                valueArray = csentry[csAttribute].Values.ToStringArray();
                            }
                            else
                            {
                                valueArray = new String[1] { csentry[csAttribute].Value };
                            }
                            foreach (String value in valueArray)
                            {
                                if (value.StartsWith("sip:", StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    mventry[mvAttribute].Value = value.Substring(4); // remove the "sip:"
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new UnexpectedDataException("Error while processing ProxyAddressesToSipAddress rule extension: " + e.ToString(), e);
                    }
                    break;

            }
        }

        /// <summary>
        /// The MapAttributesForExport method is called to map attributes from a metaverse entry to a connector space entry.
        /// </summary>
        /// <param name="FlowRuleName">
        /// Contains the name of the flow rule. You must use only alphanumeric characters for the FlowRuleName parameter, 
        /// otherwise you can encounter problems in a rules extension.
        /// Note:  Flow rules are not executed in the order shown in Identity Manager. 
        /// Identity Integration Server uses these rules according to the state of the metaverse object. 
        /// Configure your rules based on the state of the object rather than the rules being called in a predetermined order.
        /// </param>
        /// <param name="mventry">Contains an MVEntry object that represents the source metaverse entry.</param>
        /// <param name="csentry">Contains a CSEntry object that represents the destination connector space entry.</param>
        /// <remarks>
        /// This method is called when:
        ///   - the export flow rules do not overlap with the import flow rules or
        ///   - if the source attribute has a precedence greater than or equal to the precedence of the overlapping import flow rule. 
        ///     Management agent precedence is set in Metaverse Designer.
        /// </remarks>
        void IMASynchronization.MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            string ruleName;
            string mvAttribute;
            string csAttribute;
            switch (FlowRuleName)
            {
                case "GetDomain:distinguishedName,domain":
                    // This flow rule is to correctly populate the ProfileIdentifier attribute in the SharePoint Connectorspace object.


                    try
                    {
                        GetFlowRuleParameters(FlowRuleName, out ruleName, out mvAttribute, out csAttribute);

                        // TODO: ComesFromID helps us not initializing the domain property if the account comes from LDAP.
                        // It does not help however in the cases of claim based accounts when the domain cannot be importe back
                        // with full import. We need another way to create account names.
                        string domain = GetDomainValue(mventry[mvAttribute].Value);
                        if (string.IsNullOrEmpty(domain))
                        {
                            /*
                             if (csentry[csAttribute].IsPresent)
                             {
                                 csentry[csAttribute].Values.Clear();
                             }
                             */
                        }
                        else
                        {
                            csentry["ProfileIdentifier"].Value = domain + "\\" + mventry["accountName"].Value;
                        }
                    }
                    catch (Exception e)
                    {
                        // For some reason the date was not able to be converted and
                        // an exception occured. Throw decline of mapping which will 
                        // either use a lower precedence mapping or will 
                        // skip the mapping for this attribute.
                        // However, it will not stop the run.
                        //  throw new DeclineMappingException();
                        throw new UnexpectedDataException("Error while processing Set-SPS-ProfileIdentifier-Out rule extension: " + e.ToString(), e);
                    }
                    break;

                case "GetDisplayName:displayName,PreferredName":
                    try
                    {
                        GetFlowRuleParameters(FlowRuleName, out ruleName, out mvAttribute, out csAttribute);
                        // First try to get the name from the designated attribute
                        string displayName = null;
                        if (mventry[mvAttribute].IsPresent)
                            displayName = mventry[mvAttribute].Value.Trim();
                        if (string.IsNullOrEmpty(displayName))
                        {
                            // The attribute is NULL. Get the name from the distinguishedName.
                            displayName = GetDisplayNameFromDistinguishedName(mventry["distinguishedName"].Value);
                        }
                        if (string.IsNullOrEmpty(displayName))
                        {
                            if (csentry[csAttribute].IsPresent)
                            {
                                csentry[csAttribute].Values.Clear();
                            }
                        }
                        else
                        {
                            csentry[csAttribute].Value = displayName;
                        }
                    }
                    catch (Exception)
                    {
                        // For some reason the date was not able to be converted and
                        // an exception occured. Throw decline of mapping which will 
                        // either use a lower precedence mapping or will 
                        // skip the mapping for this attribute.
                        // However, it will not stop the run.
                        throw new DeclineMappingException();
                    }
                    break;
            }


        }

        #region Helper methods
      
        static internal void GetFlowRuleParameters(string flowRule, out string ruleName, out string csAttribute, out string mvAttribute)
        {
            string pattern = @"^([^:]+):([^\,]+),([^\,]+$)";
            Regex regEx = new Regex(pattern, RegexOptions.Compiled |
                                             RegexOptions.CultureInvariant);
            Match m = regEx.Match(flowRule);

            // If we fail to get the rule name and attribute names from the rule then it is probably
            // not formed correctly and is invalid, so tell FIM that the mapping could not made.
            if ((m.Success == false) || (m.Groups.Count != 4))
            {
                // This exception will not stop the run, but
                // may result in the mapping using a lower
                // precedence rule or being skipped.
                throw new DeclineMappingException();
            }

            ruleName = m.Groups[1].ToString();
            csAttribute = m.Groups[2].ToString();
            mvAttribute = m.Groups[3].ToString();
        }

        /// <summary>
        /// Gets the domain value for the CSEntry in MOSS CS by looking up
        /// the DomainInfo objects.
        /// </summary>
        /// <param name="dn">dn of the entry to look up domain value.</param>
        /// <returns>domain value if found.</returns>
        private string GetDomainValue(string distinguishedName)
        {
            // Find the DC part of the distinguished name.
            int dcStart = 0;
            for (;;)
            {
                dcStart = distinguishedName.IndexOf(",DC=", dcStart, StringComparison.OrdinalIgnoreCase);
                if (dcStart < 0)
                    return null;

                // Check if this is not a part of the name, in which case the comma will be escaped.
                bool escaped = false;
                for (int i = dcStart - 1; i >= 0 && distinguishedName[i] == '\\'; --i)
                {
                    escaped = !escaped;
                }

                ++dcStart;
                if (!escaped)
                    break;
            }
            distinguishedName = distinguishedName.Substring(dcStart);

            if (m_domainInfoCached.ContainsKey(distinguishedName))
            {
                return (string)m_domainInfoCached[distinguishedName];
            }
            else
            {
                // Couldn't find the domain, probably need to reload the cache.
                // This happens when a new partition is encountered which has domains
                // that were not flowed in on the prior partitions.
                InitializeDomainInfoCache();
                if (m_domainInfoCached.ContainsKey(distinguishedName))
                {
                    return (string)m_domainInfoCached[distinguishedName];
                }

                // Still can't find it. Get the domain name from the distinguished name.
                string[] dncomps = distinguishedName.Split(',');
                string domain = dncomps[0].Substring(3).ToUpperInvariant();
                m_domainInfoCached.Add(distinguishedName, domain);
                return domain;
            }
        }

        private Hashtable m_domainInfoCached = new Hashtable(StringComparer.OrdinalIgnoreCase);

        private void InitializeDomainInfoCache()
        {
            MVEntry[] domainEntries = Utils.FindMVEntries("type", "DomainInfo");
            foreach (MVEntry entry in domainEntries)
            {
                if (entry.ObjectType == "domain")
                {
                    string dn = entry["distinguishedName"].Value;
                    if (m_domainInfoCached.ContainsKey(dn))
                    {
                        continue;   // We already have the domain name for this object.
                    }

                    // Default to the domain object's name in the default naming context.
                    string domainName = entry["dc"].Value.ToUpperInvariant();

                    // See if domain has a NETBIOS name and use it if available.
                    // These crossRef entries are flowed in from the Partitions 
                    // container in the configuration naming context.
                    MVEntry[] crossRefs = Utils.FindMVEntries("nCName", dn);

                    // There may be multiple crossRefs for the DN if there are multiple
                    // connections to the same AD.  Use the first one with a NETBIOS name attribute.
                    foreach (MVEntry crossRef in crossRefs)
                    {
                        if (crossRef["nETBIOSName"].IsPresent)
                        {
                            string netBIOSName = crossRef["nETBIOSName"].Value;
                            if (string.IsNullOrEmpty(netBIOSName) == false)
                            {
                                domainName = netBIOSName;
                                break;
                            }
                        }
                    }

                    m_domainInfoCached.Add(dn, domainName);
                }
            }
        }

        /// <summary>
        /// Get a display name from a distinguished name.
        /// The function also handles the escaped special characters and UTF-8 encoded strings.
        /// The control characters like '\n' are replaced with spaces.
        /// </summary>
        /// <param name="distinguishedName">The source dn</param>
        /// <returns>The parsed CN</returns>
        private static string GetDisplayNameFromDistinguishedName(string distinguishedName)
        {
            if (String.IsNullOrEmpty(distinguishedName))
                return null;

            StringBuilder sb = new StringBuilder(100);
            MemoryStream ms = null;
            byte prevCharCode = 0;  // 0 - normal, 1 - \, 2 - 1 digit after \
            byte hex = 0;

            // Start from the first character after the first '=' or from the beginning if '=' is not found.
            for (int pos = distinguishedName.IndexOf('=') + 1; pos < distinguishedName.Length; ++pos)
            {
                char ch = distinguishedName[pos];
                if (prevCharCode > 0)
                {
                    if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'f') || ('A' <= ch && ch <= 'F'))
                    {
                        byte b;
                        if (ch > '9')
                        {
                            if (ch <= 'F')
                                b = (byte)(ch - 'A' + 10);
                            else
                                b = (byte)(ch - 'a' + 10);
                        }
                        else
                            b = (byte)(ch - '0');
                        if (prevCharCode == 2)
                        {
                            hex += b;
                            if (ms == null)
                            {
                                ms = new MemoryStream();
                            }
                            ms.WriteByte(hex);
                            prevCharCode = 0;
                        }
                        else
                        {
                            hex = (byte)(b * 0x10);
                            prevCharCode = 2;
                        }
                    }
                    else
                    {
                        if (prevCharCode == 2)
                        {
                            if (ms == null)
                            {
                                ms = new MemoryStream();
                            }
                            ms.WriteByte(hex);
                        }
                        DecodeUTF8(sb, ms);
                        sb.Append(ch);
                        prevCharCode = 0;
                    }
                }
                else if (ch == ',')
                {
                    break;
                }
                else if (ch != '\\')
                {
                    DecodeUTF8(sb, ms);
                    sb.Append(ch);
                }
                else
                {
                    prevCharCode = 1;
                }
            }

            DecodeUTF8(sb, ms);
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Helper to be used inside GetDisplayNameFromDistinguishedName
        /// </summary>
        /// <param name="sb">The place where to put the result</param>
        /// <param name="ms">The memory stream</param>
        private static void DecodeUTF8(StringBuilder sb, MemoryStream ms)
        {
            if (ms == null || ms.Position == 0)
            {
                return;
            }
            string decoded = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position);
            ms.Position = 0;
            foreach (char ch in decoded)
            {
                if (Char.IsControl(ch))
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(ch);
                }
            }
        }

        #endregion
    }
}


