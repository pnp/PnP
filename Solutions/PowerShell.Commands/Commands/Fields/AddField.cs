using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOField")]
    public class AddField : SPOWebCmdlet, IDynamicParameters
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ListPara")]
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
                var fieldCI = new FieldCreationInformation(Type)
                {
                    Id = Id.Id,
                    InternalName = InternalName,
                    DisplayName = DisplayName,
                    Group = Group,
                    AddToDefaultView = AddToDefaultView
                };

                if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
                {
                    f = list.CreateField<FieldChoice>(fieldCI);
                    ((FieldChoice)f).Choices = context.Choices;
                    f.Update();
                    ClientContext.ExecuteQuery();
                }
                else
                {
                    f = list.CreateField(fieldCI);

                }
                if (Required)
                {
                    f.Required = true;
                    f.Update();
                    ClientContext.Load(f);
                    ClientContext.ExecuteQuery();
                }
                WriteObject(f);
            }
            else
            {
                Field f = null;

                var fieldCI = new FieldCreationInformation(Type)
                {
                    Id = Id.Id,
                    InternalName = InternalName,
                    DisplayName = DisplayName,
                    Group = Group,
                    AddToDefaultView = AddToDefaultView
                };

                if (Type == FieldType.Choice || Type == FieldType.MultiChoice)
                {
                    f = this.SelectedWeb.CreateField<FieldChoice>(fieldCI);
                    ((FieldChoice)f).Choices = context.Choices;
                    f.Update();
                    ClientContext.ExecuteQuery();
                }
                else
                {
                    f = this.SelectedWeb.CreateField(fieldCI);
                }

                if (Required)
                {
                    f.Required = true;
                    f.Update();
                    ClientContext.Load(f);
                    ClientContext.ExecuteQuery();
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
