using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOField")]
    [CmdletHelp("Adds a field to a list or as a site column", Category = "Fields")]
    [CmdletExample(
     Code = @"PS:> Add-SPOField -List ""Demo list"" -DisplayName ""Location"" -InternalName ""SPSLocation"" -Type Choice -Group ""Demo Group"" -AddToDefaultView -Choices ""Stockholm"",""Helsinki"",""Oslo""",
     Remarks = @"This will add field of type Choice to a the list ""Demo List"".", SortOrder = 1)]
    public class AddField : SPOWebCmdlet, IDynamicParameters
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ListPara")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FieldRef")]
        public ListPipeBind List;

        [Parameter(Mandatory = true, ParameterSetName = "FieldRef")]
        public FieldPipeBind Field;

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
                var list = SelectedWeb.GetList(List);
                Field f;
                if (ParameterSetName != "FieldRef")
                {
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
                        ClientContext.ExecuteQueryRetry();
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
                        ClientContext.ExecuteQueryRetry();
                    }
                    WriteObject(f);
                }
                else
                {
                    Field field = Field.Field;
                    if (field == null)
                    {
                        if (Field.Id != Guid.Empty)
                        {
                            field = SelectedWeb.Fields.GetById(Field.Id);
                        }
                        else if (!string.IsNullOrEmpty(Field.Name))
                        {
                            field = SelectedWeb.Fields.GetByInternalNameOrTitle(Field.Name);
                        }
                        ClientContext.Load(field);
                        ClientContext.ExecuteQueryRetry();
                    }
                    if (field != null)
                    {
                        list.Fields.Add(field);
                        list.Update();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
            }
            else
            {
                Field f;

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
                    f = SelectedWeb.CreateField<FieldChoice>(fieldCI);
                    ((FieldChoice)f).Choices = context.Choices;
                    f.Update();
                    ClientContext.ExecuteQueryRetry();
                }
                else
                {
                    f = SelectedWeb.CreateField(fieldCI);
                }

                if (Required)
                {
                    f.Required = true;
                    f.Update();
                    ClientContext.Load(f);
                    ClientContext.ExecuteQueryRetry();
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
