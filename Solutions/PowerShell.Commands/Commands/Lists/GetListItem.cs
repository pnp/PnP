using System;
using System.Management.Automation;
using System.Text;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Linq;
using System.Xml.Linq;

namespace OfficeDevPnP.PowerShell.Commands.Lists
{
    [Cmdlet(VerbsCommon.Get, "SPOListItem")]
    [CmdletHelp("Retrieves list items", Category = "Lists")]
    [CmdletExample(
        Code = "PS:> Get-SPOListItem -List Tasks",
        Remarks = "Retrieves all list items from the tasks lists",
        SortOrder = 1)]
    [CmdletExample(
        Code = "PS:> Get-SPOListItem -List Tasks -Id 1",
        Remarks = "Retrieves the list item with ID 1 from from the tasks lists. This parameter is ignored if the Query parameter is specified.",
        SortOrder = 2)]
    [CmdletExample(
        Code = "PS:> Get-SPOListItem -List Tasks -UniqueId bd6c5b3b-d960-4ee7-a02c-85dc6cd78cc3",
        Remarks = "Retrieves the list item with unique id bd6c5b3b-d960-4ee7-a02c-85dc6cd78cc3 from from the tasks lists. This parameter is ignored if the Query parameter is specified.",
        SortOrder = 3)]
    [CmdletExample(
        Code = "PS:> Get-SPOListItem -List Tasks -Fields \"Title\",\"GUID\"",
        Remarks = "Retrieves all list items, but only includes the values of the Title and GUID fields in the list item object. This parameter is ignored if the Query parameter is specified.",
        SortOrder = 4)]
    [CmdletExample(
        Code = "PS:> Get-SPOListItem -List Tasks -Query \"<View><Query><Where><Eq><FieldRef Name='GUID'/><Value Type='Guid'>bd6c5b3b-d960-4ee7-a02c-85dc6cd78cc3</Value></Eq></Where></Query></View>\"",
        Remarks = "Retrieves all list items based on the CAML query specified.",
        SortOrder = 5)]
    public class GetListItem : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The list to query", Position = 0)]
        public ListPipeBind List;

        [Parameter(Mandatory = false, HelpMessage = "The ID of the item to retrieve")]
        public int Id = -1;

        [Parameter(Mandatory = false, HelpMessage = "The unique id (GUID) of the item to retrieve")]
        public GuidPipeBind UniqueId;

        [Parameter(Mandatory = false, HelpMessage = "The CAML query to execute against the list")]
        public string Query;

        [Parameter(Mandatory = false, HelpMessage = "The fields to retrieve. If not specified all fields will be loaded in the returned list object.")]
        public string[] Fields;

        protected override void ExecuteCmdlet()
        {
            var list = SelectedWeb.GetList(List);

            if (Id != -1)
            {
                var listItem = list.GetItemById(Id);
                if (Fields != null)
                {
                    foreach (var field in Fields)
                    {
                        ClientContext.Load(listItem, l => l[field]);
                    }
                }
                else
                {
                    ClientContext.Load(listItem);
                }
                ClientContext.ExecuteQueryRetry();
                WriteObject(listItem);
            }
            else if (UniqueId != null && UniqueId.Id != Guid.Empty)
            {
                CamlQuery query = new CamlQuery();
                var viewFieldsStringBuilder = new StringBuilder();
                if (Fields != null)
                {
                    viewFieldsStringBuilder.Append("<ViewFields>");
                    foreach (var field in Fields)
                    {
                        viewFieldsStringBuilder.AppendFormat("<FieldRef Name='{0}'/>", field);
                    }
                    viewFieldsStringBuilder.Append("</ViewFields>");
                }
                query.ViewXml = string.Format("<View><Query><Where><Eq><FieldRef Name='GUID'/><Value Type='Guid'>{0}</Value></Eq></Where></Query>{1}</View>", UniqueId.Id, viewFieldsStringBuilder);
                var listItem = list.GetItems(query);
                ClientContext.Load(listItem);
                ClientContext.ExecuteQueryRetry();
                WriteObject(listItem);
            }
            else if (Query != null)
            {
                CamlQuery query = new CamlQuery { ViewXml = Query };
                var listItems = list.GetItems(query);
                ClientContext.Load(listItems);
                ClientContext.ExecuteQueryRetry();
                WriteObject(listItems, true);
            }
            else
            {
                var query = CamlQuery.CreateAllItemsQuery();
                if (Fields != null)
                {
                    var queryElement = XElement.Parse(query.ViewXml);

                    var viewFields = queryElement.Descendants("ViewFields").FirstOrDefault();
                    if (viewFields != null)
                    {
                        viewFields.RemoveAll();
                    } else
                    {
                        viewFields = new XElement("ViewFields");
                        queryElement.Add(viewFields);
                    }

                    foreach (var field in Fields)
                    {
                        XElement viewField = new XElement("FieldRef");
                        viewField.SetAttributeValue("Name", field);
                        viewFields.Add(viewField);
                    }
                    query.ViewXml = queryElement.ToString();
                }
                var listItems = list.GetItems(query);
                ClientContext.Load(listItems);
                ClientContext.ExecuteQueryRetry();
                WriteObject(listItems, true);
            }
        }
    }
}
