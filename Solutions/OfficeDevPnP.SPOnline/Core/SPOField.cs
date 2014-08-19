using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOField
    {
        public static Field AddField(List list, string fieldXml, bool addToDefaultView, AddFieldOptions fieldOptions, ClientContext clientContext)
        {
            clientContext.Load(list.Fields);
            clientContext.ExecuteQuery();
            Field field = list.Fields.AddFieldAsXml(fieldXml, addToDefaultView, fieldOptions);
            clientContext.Load(field);
            clientContext.ExecuteQuery();

            return field;
        }

        public static Field AddField(Web web, string fieldXml, AddFieldOptions fieldOptions, ClientContext clientContext)
        {
            clientContext.Load(web.Fields);
            clientContext.ExecuteQuery();
            Field field = web.Fields.AddFieldAsXml(fieldXml, false, fieldOptions);
            clientContext.Load(field);
            clientContext.ExecuteQuery();

            return field;
        }

        public static Field AddField(List list, string displayName, string internalName, string staticName, FieldType fieldType, Guid Id, bool required, bool addToDefaultView, AddFieldOptions fieldOptions, ClientContext clientContext, string[] choices = null, string group = null)
        {
            string fieldString = GetFieldCAML(internalName, staticName, fieldType, Id, required, choices, group);

            Field f = AddField(list, fieldString, addToDefaultView, fieldOptions, clientContext);

            f.Title = displayName;
            f.Update();
            clientContext.Load(f);
            clientContext.ExecuteQuery();
            return f;
        }

        public static Field AddField(Web web, string displayName, string internalName, string staticName, FieldType fieldType, Guid Id, bool required, AddFieldOptions fieldOptions, ClientContext clientContext, string[] choices = null, string group = null)
        {

            string fieldString = GetFieldCAML(internalName, staticName, fieldType, Id, required, choices, group);


            Field f = AddField(web, fieldString, fieldOptions, clientContext);

            f.Title = displayName;
            f.Update();
            clientContext.Load(f);
            clientContext.ExecuteQuery();
            return f;
        }

        public static FieldLink AddField(ContentType contentType, Field field, ClientContext clientContext)
        {
            var fieldLink = contentType.FieldLinks.Add(new FieldLinkCreationInformation() { Field = field });
            clientContext.Load(contentType.FieldLinks);
            contentType.Update(true);
            clientContext.Load(fieldLink);
            clientContext.ExecuteQuery();

            return fieldLink;
        }

        private static string GetFieldCAML(string internalName, string staticName, FieldType fieldType, Guid Id, bool required, string[] choices, string group)
        {
            string fieldTypeString = Enum.GetName(typeof(FieldType), fieldType);
            string fieldXml = "<Field DisplayName='{0}' Name='{1}' StaticName='{2}' Type='{3}' ID='{{{4}}}' Required='{5}'{6}>";

            if (choices != null)
            {
                fieldXml += "<CHOICES>";
                foreach (var choice in choices)
                {
                    fieldXml += string.Format("<CHOICE>{0}</CHOICE>", choice);
                }
                fieldXml += "</CHOICES>";
            }
            fieldXml += "</Field>";

            string fieldString = string.Format(fieldXml,
                internalName,
                internalName,
                staticName,
                fieldTypeString,
                Id == Guid.Empty ? Guid.NewGuid() : Id,
                required ? "TRUE" : "FALSE",
                !string.IsNullOrEmpty(group) ? string.Format(" Group='{0}'", group) : ""
                );
            return fieldString;
        }

        public static Field AddTaxonomyField(Web web, string displayName, string internalName, string group, TermStore termStore, TermSet termSet, Guid Id, bool required, bool multiValue, ClientContext clientContext)
        {
            clientContext.Load(web.Fields);
            clientContext.ExecuteQuery();
            string taxField = GetTaxonomyFieldXml(displayName, internalName, group, termStore, termSet, Id, required, multiValue);

            Field f = web.Fields.AddFieldAsXml(taxField, true, AddFieldOptions.AddFieldInternalNameHint);
            clientContext.Load(f);
            clientContext.ExecuteQuery();
            return f;
        }

        public static Field AddTaxonomyField(List list, string displayName, string internalName, string group, TermStore termStore, TermSet termSet, Guid Id, bool required, bool addToDefaultView, bool multiValue, ClientContext clientContext)
        {
            clientContext.Load(list.Fields);
            clientContext.ExecuteQuery();

            string taxField = GetTaxonomyFieldXml(displayName, internalName, group, termStore, termSet, Id, required, multiValue);

            AddFieldOptions options = AddFieldOptions.AddFieldInternalNameHint;
            if (addToDefaultView) options = AddFieldOptions.AddFieldToDefaultView;
            Field f = list.Fields.AddFieldAsXml(taxField, true, options);
            clientContext.Load(f);
            clientContext.ExecuteQuery();
            return f;
        }

        private static string GetTaxonomyFieldXml(string displayName, string internalName, string group, TermStore termStore, TermSet termSet, Guid Id, bool required, bool multiValue)
        {
            Guid txtFieldId = Id;
            Guid taxFieldId = Guid.NewGuid();
            //Single valued, or multiple choice?
            string txType = multiValue ? "TaxonomyFieldTypeMulti" : "TaxonomyFieldType";
            //If it's single value, index it.
            string mult = multiValue ? "Mult=\"TRUE\"" : "Indexed=\"TRUE\"";

            string taxField = string.Format("<Field Type=\"{0}\" DisplayName=\"{1}\" ID=\"{8}\" ShowField=\"Term1033\" Required=\"{2}\" EnforceUniqueValues=\"FALSE\" {3} Sortable=\"FALSE\" Name=\"{4}\" {9}\"><Default/><Customization><ArrayOfProperty><Property><Name>SspId</Name><Value xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q1:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{5}</Value></Property><Property><Name>GroupId</Name></Property><Property><Name>TermSetId</Name><Value xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q2:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{6}</Value></Property><Property><Name>AnchorId</Name><Value xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q3:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">00000000-0000-0000-0000-000000000000</Value></Property><Property><Name>UserCreated</Name><Value xmlns:q4=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q4:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>Open</Name><Value xmlns:q5=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q5:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TextField</Name><Value xmlns:q6=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q6:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{7}</Value></Property><Property><Name>IsPathRendered</Name><Value xmlns:q7=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q7:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">true</Value></Property><Property><Name>IsKeyword</Name><Value xmlns:q8=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q8:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TargetTemplate</Name></Property><Property><Name>CreateValuesInEditForm</Name><Value xmlns:q9=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q9:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>FilterAssemblyStrongName</Name><Value xmlns:q10=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q10:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Value></Property><Property><Name>FilterClassName</Name><Value xmlns:q11=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q11:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy.TaxonomyField</Value></Property><Property><Name>FilterMethodName</Name><Value xmlns:q12=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q12:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">GetFilteringHtml</Value></Property><Property><Name>FilterJavascriptProperty</Name><Value xmlns:q13=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q13:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">FilteringJavascript</Value></Property></ArrayOfProperty></Customization></Field>",
                txType,
                displayName,
                required.ToString().ToUpper(),
                mult,
                internalName,
                termStore.Id.ToString("D"),
                termSet.Id.ToString("D"),
                txtFieldId.ToString("B"),
                taxFieldId.ToString("B"),
                string.IsNullOrEmpty(group) ? string.Format("Group=\"{0}\"", group) : ""
            );
            return taxField;
        }


    }
}
