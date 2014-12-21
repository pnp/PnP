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

        #endregion


    }
}
