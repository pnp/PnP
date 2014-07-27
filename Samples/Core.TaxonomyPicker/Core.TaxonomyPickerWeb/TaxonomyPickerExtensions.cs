using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Core.TaxonomyPickerWeb
{
    //Extension class for working with TaxonomyFields with the TaxonomyPicker
    public static class TaxonomyPickerExtensions
    {
        public static string Serialize(this TaxonomyFieldValue taxValue)
        {
            return String.Format("[{0}]", taxValue.SerializeTerm());
        }

        public static string Serialize(this TaxonomyFieldValueCollection taxValues)
        {
            string terms = "";
            foreach (TaxonomyFieldValue taxValue in taxValues)
            {
                terms += taxValue.SerializeTerm() + ',';
            }
            if (terms.Length > 0)
                terms = terms.Substring(0, terms.Length - 1);
            return String.Format("[{0}]", terms);
        }

        private static string SerializeTerm(this TaxonomyFieldValue taxValue)
        {
            return "{\"Id\":\"" + taxValue.TermGuid + "\", \"Name\": \"" + taxValue.Label + "\"}";
        }
    }
}