using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOFieldFromXml")]
    public class AddFieldFromXml : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public ListPipeBind List;

        [Parameter(Mandatory = true, HelpMessage = "CAML snippet containing the field definition. See http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx", Position = 0)]
        public string FieldXml;

        protected override void ExecuteCmdlet()
        {
            Field f = null;

            if (List != null)
            {
                List list = SelectedWeb.GetList(List);

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
