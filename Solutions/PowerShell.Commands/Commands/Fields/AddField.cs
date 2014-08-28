using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

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

        [Parameter(Mandatory = false, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = false, ParameterSetName = "ListXML")]
        public string Group;

        [Parameter(Mandatory = false, DontShow = true)]
        [Obsolete("Not in use")]
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
                    f = list.CreateField(FieldXml);
                }
                else
                {
                    if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
                    {
                        var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, context.Choices, Group);
                        f = list.CreateField(fieldXml);
                    }
                    else
                    {
                        var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, group: Group);
                        f = list.CreateField(fieldXml);
                    }
                }
                WriteObject(f);
            }
            else
            {
                Field f = null;
                if (!string.IsNullOrEmpty(FieldXml))
                {
                    f = this.SelectedWeb.CreateField(FieldXml);
                }
                else
                {
                    if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
                    {
                        var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, context.Choices, Group);
                        f = this.SelectedWeb.CreateField(fieldXml);
                    }
                    else
                    {
                        var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, group: Group);
                        f = this.SelectedWeb.CreateField(fieldXml);
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

        private static string GetFieldCAML(string displayName, string internalName, string staticName, FieldType fieldType, Guid Id, bool required, string[] choices = null, string group = null)
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
                displayName,
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

    }

}
