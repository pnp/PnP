using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Entities;

namespace Microsoft.SharePoint.Client
{
    public static partial class TaxonomyExtensions
    {
        [Obsolete("Use CreateTaxonomyField(List,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static Field CreateTaxonomyFieldInternal(this List list, Guid id, string internalName, string displayName, string group, TaxonomyItem taxonomyItem, bool multiValue)
        {
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            taxonomyItem.ValidateNotNullOrEmpty("taxonomyItem");

            try
            {
                List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
                additionalAttributes.Add(new KeyValuePair<string, string>("ShowField", "Term1033"));

                FieldCreationInformation fieldCI = new FieldCreationInformation(multiValue ? "TaxonomyFieldTypeMulti" : "TaxonomyFieldType")
                {
                    Id = id,
                    InternalName = internalName,
                    AddToDefaultView = true,
                    DisplayName = displayName,
                    Group = group,
                    AdditionalAttributes = additionalAttributes
                };
                var _field = list.CreateField(fieldCI);

                WireUpTaxonomyFieldInternal(_field, taxonomyItem, multiValue);
                _field.Update();

                list.Context.ExecuteQueryRetry();

                return _field;
            }
            catch (Exception)
            {
                // If there is an exception the hidden field might be present
                FieldCollection _fields = list.Fields;
                list.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
                list.Context.ExecuteQueryRetry();
                var _hiddenField = id.ToString().Replace("-", "");

                var _field = _fields.FirstOrDefault(f => f.InternalName == _hiddenField);
                if (_field != null)
                {
                    _field.Hidden = false; // Cannot delete a hidden column
                    _field.Update();
                    _field.DeleteObject();
                    list.Context.ExecuteQueryRetry();
                }
                throw;
            }
        }


        [Obsolete("Use CreateTaxonomyField(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static Field CreateTaxonomyFieldInternal(this Web web, Guid id, string internalName, string displayName, string group, TaxonomyItem taxonomyItem, bool multiValue)
        {
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            taxonomyItem.ValidateNotNullOrEmpty("taxonomyItem");

            try
            {
                List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
                additionalAttributes.Add(new KeyValuePair<string, string>("ShowField", "Term1033"));

                FieldCreationInformation fieldCI = new FieldCreationInformation(multiValue ? "TaxonomyFieldTypeMulti" : "TaxonomyFieldType")
                {
                    Id = id,
                    InternalName = internalName,
                    AddToDefaultView = true,
                    DisplayName = displayName,
                    Group = group,
                    AdditionalAttributes = additionalAttributes
                };
                var _field = web.CreateField(fieldCI);

                WireUpTaxonomyFieldInternal(_field, taxonomyItem, multiValue);
                _field.Update();

                web.Context.ExecuteQueryRetry();

                return _field;
            }
            catch (Exception)
            {
                // If there is an exception the hidden field might be present
                FieldCollection _fields = web.Fields;
                web.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
                web.Context.ExecuteQueryRetry();
                var _hiddenField = id.ToString().Replace("-", "");

                var _field = _fields.FirstOrDefault(f => f.InternalName == _hiddenField);
                if (_field != null)
                {
                    _field.DeleteObject();
                    web.Context.ExecuteQueryRetry();
                }
                throw;

            }

        }

        [Obsolete("Use CreateTaxonomyFieldInternal(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            id.ValidateNotNullOrEmpty("id");
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            // Group can be emtpy
            mmsGroupName.ValidateNotNullOrEmpty("mmsGroupName");
            mmsTermSetName.ValidateNotNullOrEmpty("mmsTermSetName");

            TermStore termStore = GetDefaultTermStore(web);

            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");


            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            web.Context.Load(termStore);
            web.Context.Load(termSet);
            web.Context.ExecuteQueryRetry();

            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Group = group,
                TaxonomyItem = termSet,
                MultiValue = multiValue
            };
            return web.CreateTaxonomyField(fieldCI);
        }


        [Obsolete("Use CreateTaxonomyField(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, TermSet termSet, bool multiValue = false)
        {
            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Group = group,
                TaxonomyItem = termSet,
                MultiValue = multiValue
            };
            return web.CreateTaxonomyField(fieldCI);
        }

        [Obsolete("Use CreateTaxonomyField(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, Term anchorTerm, bool multiValue = false)
        {
            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Group = group,
                TaxonomyItem = anchorTerm,
                MultiValue = multiValue
            };
            return web.CreateTaxonomyField(fieldCI);
        }


        [Obsolete("Use CreateTaxonomyField(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Field CreateTaxonomyField(this List list, Guid id, string internalName, string displayName, string group, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            id.ValidateNotNullOrEmpty("id");
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            mmsGroupName.ValidateNotNullOrEmpty("mmsGroupName");
            mmsTermSetName.ValidateNotNullOrEmpty("mmsTermSetName");

            var clientContext = list.Context as ClientContext;
            TermStore termStore = clientContext.Site.GetDefaultSiteCollectionTermStore();


            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");

            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            list.Context.Load(termStore);
            list.Context.Load(termSet);
            list.Context.ExecuteQueryRetry();

            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Required = false,
                Group = group,
                MultiValue = multiValue,
                TaxonomyItem = termSet
            };
            return list.CreateTaxonomyField(fieldCI);
        }


        [Obsolete("Use CreateTaxonomyField(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Field CreateTaxonomyField(this List list, Guid id, string internalName, string displayName, string group, TermSet termSet, bool multiValue = false)
        {
            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Required = false,
                Group = group,
                MultiValue = multiValue,
                TaxonomyItem = termSet
            };
            return list.CreateTaxonomyField(fieldCI);
        }


        [Obsolete("Use CreateTaxonomyField(Web,TaxonomyFieldCreationInformation) instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public static Field CreateTaxonomyField(this List list, Guid id, string internalName, string displayName, string group, Term anchorTerm, bool multiValue = false)
        {
            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = internalName,
                DisplayName = displayName,
                Required = false,
                Group = group,
                MultiValue = multiValue,
                TaxonomyItem = anchorTerm
            };
            return list.CreateTaxonomyField(fieldCI);
        }
    }
}
