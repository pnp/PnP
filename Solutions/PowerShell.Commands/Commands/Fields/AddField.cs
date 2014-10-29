using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOField")]
    public class AddField : SPOWebCmdlet, IDynamicParameters
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ListXML")]
        public ListPipeBind List;

        [Parameter(Mandatory = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ParameterSetName = "WebPara")]
        public string DisplayName;

        [Parameter(Mandatory = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ParameterSetName = "WebPara")]
        public string InternalName;

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

            if (Id.Id == Guid.Empty)
            {
                Id = new GuidPipeBind(Guid.NewGuid());
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
                        string choicesCAML = GetChoicesCAML(context.Choices);

                        //var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, context.Choices, Group);
                        f = list.CreateField(Id.Id, InternalName, Type, DisplayName, Group, choicesCAML);

                        //f = list.CreateField(fieldXml);
                    }
                    else
                    {
                        f = list.CreateField(Id.Id, InternalName, Type, DisplayName, Group);

                        //var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, group: Group);
                        //f = list.CreateField(fieldXml);
                    }
                }
                if (Required)
                {
                    f.Required = true;
                    f.Update();
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
                        var choicesCAML = GetChoicesCAML(context.Choices);

                        f = this.SelectedWeb.CreateField(Id.Id, InternalName, Type, DisplayName, Group, choicesCAML);
                       
                        //var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, context.Choices, Group);
                        //f = this.SelectedWeb.CreateField(fieldXml);
                    }
                    else
                    {
                        f = this.SelectedWeb.CreateField(Id.Id, InternalName, Type, DisplayName, Group);
                      //  var fieldXml = GetFieldCAML(DisplayName, InternalName, StaticName, Type, Id.Id, Required, group: Group);
                      //  f = this.SelectedWeb.CreateField(fieldXml);
                    }

                }
                if(Required)
                {
                    f.Required = true;
                    f.Update();
                }
                ClientContext.Load(f);
                ClientContext.ExecuteQuery();
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

        private static string GetChoicesCAML(string[] choices)
        {
            var fieldXml = "<CHOICES>";
            foreach (var choice in choices)
            {
                fieldXml += string.Format("<CHOICE>{0}</CHOICE>", choice);
            }
            fieldXml += "</CHOICES>";
            return fieldXml;
        }

    }

}
