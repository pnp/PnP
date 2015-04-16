using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Core.DocumentPickerWeb
{
    public static class DocumentPickerHelper
    {
        public static void SetData(HiddenField hiddenField, List<PickedDocument> documents)
        {
            var json = new JavaScriptSerializer().Serialize(documents);
            hiddenField.Value = json;
        }

        public static List<PickedDocument> GetData(HiddenField hiddenField)
        {
            return new JavaScriptSerializer().Deserialize <List<PickedDocument>>(hiddenField.Value);
        }
    }

    public class PickedDocument
    {
        public string DocumentUrl { get; set; }
        public string DocumentPath { get; set; }
        public string ListName { get; set; }
        public string ItemId { get; set; }
    }
}