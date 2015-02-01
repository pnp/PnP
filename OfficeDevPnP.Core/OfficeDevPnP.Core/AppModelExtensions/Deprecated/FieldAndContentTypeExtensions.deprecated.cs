using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This class provides extension methods that will help you work with fields and content types.
    /// </summary>
    public static partial class FieldAndContentTypeExtensions
    {
        /// <summary>
        /// Create a content type based on the classic feature framework structure.
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="xmlDoc">Actual XML document</param>
        [Obsolete("Use CreateContentTypeFromXML(this Web web, XDocument xDocument)")]
        public static void CreateContentTypeFromXML(this Web web, XmlDocument xmlDoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("namespace", xmlDoc.DocumentElement.NamespaceURI);

            XmlNodeList contentTypes = xmlDoc.SelectNodes("//namespace:ContentType", nsmgr);
            int count = contentTypes.Count;
            foreach (XmlNode ct in contentTypes)
            {
                string ctid = ct.Attributes["ID"].Value;
                string name = ct.Attributes["Name"].Value;
                if (web.ContentTypeExistsByName(name))
                {
                    LoggingUtility.Internal.TraceWarning((int)EventId.ContentTypeAlreadyExists, CoreResources.FieldAndContentTypeExtensions_ContentType01AlreadyExists, name, ctid);
                    // Skip
                }
                else
                {
                    var description = "";
                    if (((XmlElement)ct).HasAttribute("Description"))
                    {
                        description = ((XmlElement)ct).GetAttribute("Description");
                    }
                    var group = "";
                    if (((XmlElement)ct).HasAttribute("Group"))
                    {
                        group = ((XmlElement)ct).GetAttribute("Group");
                    }

                    //Create CT
                    web.CreateContentType(name, description, ctid, group);

                    //Add fields to content type 
                    XmlNodeList fieldRefs = ct.SelectNodes(".//namespace:FieldRef", nsmgr);
                    XmlAttribute attr = null;
                    foreach (XmlNode fr in fieldRefs)
                    {
                        bool required = false;
                        bool hidden = false;
                        string frid = fr.Attributes["ID"].Value;
                        string frName = fr.Attributes["Name"].Value;
                        attr = fr.Attributes["Required"];
                        if (attr != null)
                        {
                            required = attr.Value.ToBoolean();
                        }
                        attr = fr.Attributes["Hidden"];
                        if (attr != null)
                        {
                            hidden = attr.Value.ToBoolean();
                        }
                        web.AddFieldToContentTypeById(ctid, frid, required, hidden);
                    }
                }
            }
        }

        #region Site Columns
        [Obsolete("Use CreateField(Web web, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Field CreateField(this Web web, Guid id, string internalName, FieldType fieldType, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);

            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Group = group,
                AdditionalAttributes = additionalAttributes
            };
            return CreateField(web, fieldCreationInformation, executeQuery);

        }

        [Obsolete("Use CreateField<TField>(this Web web, FieldCreationInformation fieldCreationInformation, bool executeQuery = true)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static TField CreateField<TField>(this Web web, Guid id, string internalName, FieldType fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true) where TField : Field
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);

            var fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                AddToDefaultView = addToDefaultView,
                Group = group,
                DisplayName = displayName,
                AdditionalAttributes = additionalAttributes
            };

            return CreateField<TField>(web, fieldCreationInformation, executeQuery);
        }


        [Obsolete("Use CreateField(Web web, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Field CreateField(this Web web, Guid id, string internalName, string fieldType, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);

            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                DisplayName = displayName,
                Group = group,
                InternalName = internalName,
                AddToDefaultView = false,
                AdditionalAttributes = additionalAttributes
            };
            return CreateField(web, fieldCreationInformation, executeQuery);
        }

        [Obsolete("Use CreateField(Web web, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Field CreateField(this Web web, Guid id, string internalName, string fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);

            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Group = group,
                AddToDefaultView = addToDefaultView,
                AdditionalAttributes = additionalAttributes
            };
            return CreateField(web, fieldCreationInformation, executeQuery);
        }

        [Obsolete("Use CreateField<TField>(this Web web, FieldCreationInformation fieldCreationInformation, bool executeQuery = true)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static TField CreateField<TField>(this Web web, Guid id, string internalName, string fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true) where TField : Field
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);

            var fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                AddToDefaultView = addToDefaultView,
                Group = group,
                DisplayName = displayName,
                AdditionalAttributes = additionalAttributes
            };

            return CreateField<TField>(web, fieldCreationInformation, executeQuery);
        }
        #endregion

        #region List Fields
        [Obsolete("Use CreateField(List list, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        public static Field CreateField(this List list, Guid id, string internalName, FieldType fieldType, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);
            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                AddToDefaultView = false,
                DisplayName = displayName,
                Group = group,
                InternalName = internalName,
                AdditionalAttributes = additionalAttributes
            };
            return CreateField(list, fieldCreationInformation, executeQuery);

        }

        [Obsolete("Use CreateField(List list, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Field CreateField(this List list, Guid id, string internalName, string fieldType, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);

            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                AddToDefaultView = false,
                DisplayName = displayName,
                Group = group,
                InternalName = internalName,
                AdditionalAttributes = additionalAttributes
            };
            return CreateField(list, fieldCreationInformation, executeQuery);
        }

        [Obsolete("Use CreateField(List list, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Field CreateField(this List list, Guid id, string internalName, string fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);
            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                AddToDefaultView = false,
                DisplayName = displayName,
                Group = group,
                InternalName = internalName,
                AdditionalAttributes = additionalAttributes
            };
            return CreateField(list, fieldCreationInformation, executeQuery);
        }

        [Obsolete("Use CreateField<TField>(List list, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static TField CreateField<TField>(this List list, Guid id, string internalName, FieldType fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true) where TField : Field
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);
            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                AddToDefaultView = addToDefaultView,
                Group = group,
                AdditionalAttributes = additionalAttributes,
                DisplayName = displayName
            };
            return CreateField<TField>(list, fieldCreationInformation, executeQuery);
        }

        [Obsolete("Use CreateField<TField>(List list, FieldCreationInformation fieldCreationInformation, System.Boolean executeQuery = True)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static TField CreateField<TField>(this List list, Guid id, string internalName, string fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true) where TField : Field
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);
            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                AddToDefaultView = addToDefaultView,
                Group = group,
                AdditionalAttributes = additionalAttributes,
                DisplayName = displayName
            };
            return CreateField<TField>(list, fieldCreationInformation, executeQuery);
        }

        [Obsolete("Use CreateFieldBase<TField>(FieldCollection fields, FieldCreationInformation fieldCreationInformation, bool executeQuery = true")]
        static TField CreateFieldBase<TField>(FieldCollection fields, Guid id, string internalName, string fieldType, bool addToDefaultView, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true) where TField : Field
        {
            var additionalAttributes = ParseAdditionalAttributes(additionalXmlAttributes);
            FieldCreationInformation fieldCreationInformation = new FieldCreationInformation(fieldType)
            {
                Id = id,
                InternalName = internalName,
                AddToDefaultView = addToDefaultView,
                Group = group,
                AdditionalAttributes = additionalAttributes,
                DisplayName = displayName
            };
            return CreateFieldBase<TField>(fields, fieldCreationInformation, executeQuery);
        }


        [Obsolete("Use FormatFieldXml(Guid id, string internalName, string fieldType, string displayName, string group, IEnumerable<KeyValuePair<string,string>> additionalAttributes)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static string FormatFieldXml(Guid id, string internalName, string fieldType, string displayName, string group, string additionalXmlAttributes)
        {
            string newFieldCAML = string.Format(OfficeDevPnP.Core.Constants.FIELD_XML_FORMAT, fieldType, internalName, displayName, id, group, "FALSE", additionalXmlAttributes);
            return newFieldCAML;
        }

        /// <summary>
        /// Binds a field to a termset based on an xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="absolutePathToFile">Absolute path to the xml location</param>
        [Obsolete]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void BindFieldsToTermSetsFromXMLFile(this Web web, string absolutePathToFile)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(absolutePathToFile);
            BindFieldsToTermSetsFromXML(web, xd);
        }

        /// <summary>
        /// Binds a field to a termset based on an xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlStructure">XML structure in string format</param>
        [Obsolete]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void BindFieldsToTermSetsFromXMLString(this Web web, string xmlStructure)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlStructure);
            BindFieldsToTermSetsFromXML(web, xd);
        }

        /// <summary>
        /// Binds a field to a termset based on an xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlDoc">Actual XML document</param>
        [Obsolete]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void BindFieldsToTermSetsFromXML(this Web web, XmlDocument xmlDoc)
        {
            XmlNodeList fields = xmlDoc.SelectNodes("//MMSField");
            foreach (XmlNode mmsfield in fields)
            {
                string fieldGuid = mmsfield.Attributes["FieldGuid"].Value;
                string MMSGroupName = mmsfield.Attributes["MMSGroupName"].Value;
                string TermSet = mmsfield.Attributes["TermSet"].Value;

                TaxonomyExtensions.WireUpTaxonomyField(web, new Guid(fieldGuid), MMSGroupName, TermSet);
            }
        }

        /// <summary>
        /// Creates field from xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlDoc">Actual XML document</param>
        [Obsolete("Use CreateFieldsFromXML(this Web web, XDocument xDocument)")]
        public static void CreateFieldsFromXML(this Web web, XmlDocument xmlDoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("namespace", xmlDoc.DocumentElement.NamespaceURI);

            XmlNodeList fields = xmlDoc.SelectNodes("//namespace:Field", nsmgr);
            int count = fields.Count;
            foreach (XmlNode field in fields)
            {
                string id = field.Attributes["ID"].Value;
                string name = field.Attributes["Name"].Value;

                // If field already existed, let's move on
                if (web.FieldExistsByName(name))
                {
                    LoggingUtility.Internal.TraceWarning((int)EventId.FieldAlreadyExists, CoreResources.FieldAndContentTypeExtensions_Field01AlreadyExists, name, id);
                }
                else
                {
                    web.CreateField(field.OuterXml);
                }
            }
        }
        #endregion


    }
}
