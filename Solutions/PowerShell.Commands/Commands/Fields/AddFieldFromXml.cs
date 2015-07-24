using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOFieldFromXml")]
    [CmdletHelp("Adds a field to a list or as a site column based upon a CAML/XML field definition", Category = "Fields")]
    public class AddFieldFromXml : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public ListPipeBind List;

        [Parameter(Mandatory = true, HelpMessage = "CAML snippet containing the field definition. See http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx", Position = 0)]
        public string FieldXml;

        protected override void ExecuteCmdlet()
        {
            Field f;

            if (List != null)
            {
                List list = List.GetList(SelectedWeb);

                f = list.CreateField(FieldXml);
            }
            else
            {
                f = SelectedWeb.CreateField(FieldXml);
            }
            ClientContext.Load(f);
            ClientContext.ExecuteQueryRetry();
            WriteObject(f);
        }

    }

}
