using System.Collections;
using System.Collections.Generic;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOListItem")]
    [CmdletHelp("Adds an item to a list", Category = "Lists")]
    [CmdletExample(
        Code = @"Add-SPOListItem -List ""Demo List"" -Values @{""Title"" = ""Test Title"", ""Category""=""Test Category""}", 
        Remarks = @"Adds a new list item to the ""Demo List"", and sets both the Title and Category fields with the specified values.",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"Add-SPOListItem -List ""Demo List"" -ContentType ""Company"" -Values @{""Title"" = ""Test Title"", ""Category""=""Test Category""}", 
        Remarks = @"Adds a new list item to the ""Demo List"", sets the content type to ""Company"" and sets both the Title and Category fields with the specified values.",
        SortOrder = 2)]
    public class AddListItem : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID, Title or Url of the list.")]
        public ListPipeBind List;

        [Parameter(Mandatory = false, HelpMessage = "Specify either the name, ID or an actual content type.")]
        public ContentTypePipeBind ContentType;

        [Parameter(Mandatory = false)]
        public Hashtable Values;

        protected override void ExecuteCmdlet()
        {
            List list = null;
            if (List != null)
            {
                list = List.GetList(SelectedWeb);
            }
            if (list != null)
            {
                ListItemCreationInformation liCI = new ListItemCreationInformation();
                var item = list.AddItem(liCI);

                if (ContentType != null)
                {
                    ContentType ct = null;
                    if (ContentType.ContentType == null)
                    {
                        if (ContentType.Id != null)
                        {
                            ct = SelectedWeb.GetContentTypeById(ContentType.Id, true);
                        }
                        else if (ContentType.Name != null)
                        {
                            ct = SelectedWeb.GetContentTypeByName(ContentType.Name, true);
                        }
                    }
                    else
                    {
                        ct = ContentType.ContentType;
                    }
                    if (ct != null)
                    {
                        if (!ct.IsPropertyAvailable("StringId"))
                        {
                            ClientContext.Load(ct, c => c.StringId);
                            ClientContext.ExecuteQueryRetry();
                        }
                        item["ContentTypeId"] = ct.StringId;
                        item.Update();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
                
                foreach (var key in Values.Keys)
                {
                    item[key as string] = Values[key];
                }


                item.Update();
                ClientContext.Load(item);
                ClientContext.ExecuteQueryRetry();
                WriteObject(item);
            }
        }
    }

}
