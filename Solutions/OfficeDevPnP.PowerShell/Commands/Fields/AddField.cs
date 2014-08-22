using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOField")]
    public class AddField : SPOWebCmdlet, IDynamicParameters
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ListXML")]
        public SPOListPipeBind List;

        [Parameter(Mandatory = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ParameterSetName = "WebPara")]
        public string DisplayName;

        [Parameter(Mandatory = false, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = false, ParameterSetName = "WebPara")]
        public string InternalName;

        [Parameter(Mandatory = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ParameterSetName = "WebPara")]
        public string StaticName;

        [Parameter(Mandatory = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ParameterSetName = "WebPara")]
        public FieldType Type;

        [Parameter(Mandatory = false, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = false, ParameterSetName = "WebPara")]
        public GuidPipeBind Id = new GuidPipeBind();

        [Parameter(Mandatory = false, ParameterSetName = "ListXML", HelpMessage = "CAML snippet containing the field definition. See http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx")]
        [Parameter(Mandatory = false, ParameterSetName = "WebXML", HelpMessage = "CAML snippet containing the field definition. See http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx")]
        public string FieldXml;

        [Parameter(Mandatory = false, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = false, ParameterSetName = "ListXML")]
        public SwitchParameter AddToDefaultView;

        [Parameter(Mandatory = false, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = false, ParameterSetName = "ListXML")]
        public SwitchParameter Required;

        [Parameter(Mandatory = false)]
        public AddFieldOptions FieldOptions = AddFieldOptions.DefaultValue;

        public object GetDynamicParameters()
        {
            if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
            {
                context = new ChoiceFieldDynamicParameters();
                return context;
            }
            return null;
        }
        private ChoiceFieldDynamicParameters context;

        protected override void ExecuteCmdlet()
        {

            if (string.IsNullOrEmpty(StaticName))
            {
                StaticName = InternalName;
            }

            if (List != null)
            {
                List list = this.SelectedWeb.GetList(List);

                Field f = null;
                if (!string.IsNullOrEmpty(FieldXml))
                {
                    f = SPO.SPOField.AddField(list, FieldXml, AddToDefaultView, FieldOptions, ClientContext);
                }
                else
                {
                    if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
                    {
                        f = SPO.SPOField.AddField(list, DisplayName, InternalName, StaticName, Type, Id.Id, Required, AddToDefaultView, FieldOptions, ClientContext, context.Choices);
                    }
                    else
                    {
                        f = SPO.SPOField.AddField(list, DisplayName, InternalName, StaticName, Type, Id.Id, Required, AddToDefaultView, FieldOptions, ClientContext);
                    }
                }
                WriteObject(f);
            }
            else
            {
                Web web = ClientContext.Web;
                if (Web != null)
                {
                    if (Web.Web != null)
                    {
                        web = Web.Web;
                    }
                    else if (Web.Id != Guid.Empty)
                    {
                        web = SPO.SPOWeb.GetWebById(Web.Id, ClientContext);
                    }
                    else if (!string.IsNullOrEmpty(Web.Url))
                    {
                        web = SPO.SPOWeb.GetWebByUrl(Web.Url, ClientContext);
                    }
                }
                Field f = null;
                if (!string.IsNullOrEmpty(FieldXml))
                {
                    f = SPO.SPOField.AddField(web, FieldXml, FieldOptions, ClientContext);
                }
                else
                {
                    if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
                    {
                        f = SPO.SPOField.AddField(web, DisplayName, InternalName, StaticName, Type, Id.Id, Required, FieldOptions, ClientContext, context.Choices);
                    }
                    else
                    {
                        f = SPO.SPOField.AddField(web, DisplayName, InternalName, StaticName, Type, Id.Id, Required, FieldOptions, ClientContext);
                    }

                }
                WriteObject(f);
            }
        }

        public class ChoiceFieldDynamicParameters
        {
            [Parameter(Mandatory = false)]
            public string[] Choices
            {
                get { return choices; }
                set { choices = value; }
            }
            private string[] choices;
        }

    }

}
