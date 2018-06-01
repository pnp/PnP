using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Discussion_Migrator
{
    class Settings
    {
        [Description("List of fields to be fetched for each item.")]
        [Editor(@"System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> fetchedFields = new List<string>() { "ID", "ParentFolderId",
            "Title", "Body", "Created", "Modified", "Author", "Editor", "LastReplyBy",
            "IsQuestion", "IsAnswered", "IsFeatured", "ItemChildCount",
            "ParentItemEditorId", "ParentItemID", "BestAnswerId",
            "FileRef", "FileLeafRef","Attachments" };

        public List<string> nullableFields = new List<string>()
        {
            "ParentItemId", "BestAnswerId"
        };

        //public object[] AppSettings
        //{
        //    get
        //    {

        //    }
        //}
    }
}
