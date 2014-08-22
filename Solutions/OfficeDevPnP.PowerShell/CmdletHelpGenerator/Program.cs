using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;


namespace OfficeDevPnP.PowerShell.CmdletHelpGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string inFile = args[0];
            string outFile = args[1];
            XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", string.Empty));

            XNamespace ns = "http://msh";
            XElement helpItems = new XElement(ns + "helpItems", new XAttribute("schema", "maml"));
            doc.Add(helpItems);


            XNamespace maml = "http://schemas.microsoft.com/maml/2004/10";
            XNamespace command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
            XNamespace dev = "http://schemas.microsoft.com/maml/dev/2004/10";

            XAttribute mamlNsAttr = new XAttribute(XNamespace.Xmlns + "maml", "http://schemas.microsoft.com/maml/2004/10");
            XAttribute commandNsAttr = new XAttribute(XNamespace.Xmlns + "command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
            XAttribute devNsAttr = new XAttribute(XNamespace.Xmlns + "dev", "http://schemas.microsoft.com/maml/dev/2004/10");

            string output = string.Empty;
            Assembly assembly = Assembly.LoadFrom(inFile);
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                if (t.BaseType.Name == "SPOCmdlet" || t.BaseType.Name == "PSCmdlet" || t.BaseType.Name == "SPOWebCmdlet" || t.BaseType.Name == "SPOAdminCmdlet")
                {
                    //XElement examples = new XElement(command + "examples");

                    string verb = string.Empty;
                    string noun = string.Empty;
                    string description = string.Empty;
                    string detaileddescription = string.Empty;
                    string details = string.Empty;
                    string copyright = string.Empty;
                    string version = string.Empty;
                    var attrs = t.GetCustomAttributes();
                    List<CmdletExampleAttribute> examples = new List<CmdletExampleAttribute>();

                    //System.Attribute.GetCustomAttributes(t); 

                    foreach (System.Attribute attr in attrs)
                    {
                        if (attr is CmdletAttribute)
                        {
                            CmdletAttribute a = (CmdletAttribute)attr;
                            verb = a.VerbName;
                            noun = a.NounName;

                        }
                        if (attr is CmdletHelpAttribute)
                        {
                            CmdletHelpAttribute a = (CmdletHelpAttribute)attr;
                            description = a.Description;
                            details = a.Details;
                            copyright = a.Copyright;
                            version = a.Version;
                            detaileddescription = a.DetailedDescription;
                        }
                        if (attr is CmdletExampleAttribute)
                        {
                            CmdletExampleAttribute a = (CmdletExampleAttribute)attr;
                            examples.Add(a);


                        }
                    }

                    XElement commandElement = new XElement(command + "command", mamlNsAttr, commandNsAttr, devNsAttr);
                    XElement detailsElement = new XElement(command + "details");
                    commandElement.Add(detailsElement);

                    detailsElement.Add(new XElement(command + "name", string.Format("{0}-{1}", verb, noun)));
                    detailsElement.Add(new XElement(maml + "description", new XElement(maml + "para", description)));
                    detailsElement.Add(new XElement(maml + "copyright", new XElement(maml + "para", copyright)));
                    detailsElement.Add(new XElement(command + "verb", verb));
                    detailsElement.Add(new XElement(command + "noun", noun));
                    detailsElement.Add(new XElement(dev + "version", version));

                    commandElement.Add(new XElement(maml + "description", new XElement(maml + "para", detaileddescription)));

                    XElement syntaxElement = new XElement(command + "syntax");
                    commandElement.Add(syntaxElement);

                    FieldInfo[] fields = t.GetFields();
                    List<SyntaxItem> syntaxItems = new List<SyntaxItem>();
                    foreach (FieldInfo field in fields)
                    {
                        foreach (System.Attribute attr in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                        {
                            if (attr is ParameterAttribute)
                            {
                                SyntaxItem syntaxItem = null;
                                ParameterAttribute a = (ParameterAttribute)attr;

                                if (a.ParameterSetName != ParameterAttribute.AllParameterSets)
                                {
                                    syntaxItem = syntaxItems.Where(x => x.Name == a.ParameterSetName).FirstOrDefault();
                                    if (syntaxItem == null)
                                    {
                                        syntaxItem = new SyntaxItem(a.ParameterSetName);
                                        syntaxItems.Add(syntaxItem);
                                    }
                                    syntaxItem.Parameters.Add(new SyntaxItem.Parameter() { Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name });
                                }
                            }
                        }
                    }

                    // all parameters
                    foreach (FieldInfo field in fields)
                    {
                        foreach (System.Attribute attr in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                        {
                            if (attr is ParameterAttribute)
                            {
                                ParameterAttribute a = (ParameterAttribute)attr;
                                if (a.ParameterSetName == ParameterAttribute.AllParameterSets)
                                {
                                    foreach (var si in syntaxItems)
                                    {
                                        si.Parameters.Add(new SyntaxItem.Parameter() { Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name });
                                    }
                                }
                            }
                        }
                    }

                    //
                    foreach (var syntaxItem in syntaxItems)
                    {
                        XElement syntaxItemElement = new XElement(command + "syntaxItem");
                        syntaxElement.Add(syntaxItemElement);

                        syntaxItemElement.Add(new XElement(maml + "name", string.Format("{0}-{1}", verb, noun)));
                        foreach (var parameter in syntaxItem.Parameters)
                        {
                            XElement parameterElement = new XElement(command + "parameter", new XAttribute("required", parameter.Required), new XAttribute("position", parameter.Position > 0 ? parameter.Position.ToString() : "named"));

                            parameterElement.Add(new XElement(maml + "name", parameter.Name));

                            parameterElement.Add(new XElement(maml + "description", new XElement(maml + "para", parameter.Description)));
                            parameterElement.Add(new XElement(command + "parameterValue", parameter.Type));

                            syntaxItemElement.Add(parameterElement);
                        }
                    }

                    XElement parametersElement = new XElement(command + "parameters");
                    commandElement.Add(parametersElement);

                    foreach (FieldInfo field in fields)
                    {
                        foreach (System.Attribute attr in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                        {
                            if (attr is ParameterAttribute)
                            {
                                ParameterAttribute a = (ParameterAttribute)attr;
                                XElement parameter2Element = new XElement(command + "parameter", new XAttribute("required", a.Mandatory), new XAttribute("position", a.Position > 0 ? a.Position.ToString() : "named"));

                                parameter2Element.Add(new XElement(maml + "name", field.Name));

                                parameter2Element.Add(new XElement(maml + "description", new XElement(maml + "para", a.HelpMessage)));
                                var parameterValueElement = new XElement(command + "parameterValue", field.FieldType.Name, new XAttribute("required", a.Mandatory));
                                parameter2Element.Add(parameterValueElement);

                                var devElement = new XElement(dev + "type");
                                devElement.Add(new XElement(maml + "name", field.FieldType.Name));
                                devElement.Add(new XElement(maml + "uri"));

                                parameter2Element.Add(devElement);

                                parametersElement.Add(parameter2Element);
                                break;

                            }
                        }
                    }

                    commandElement.Add(
                        new XElement(command + "inputTypes",
                            new XElement(command + "inputType",
                                new XElement(dev + "type",
                                    new XElement(maml + "name", "String"),
                                    new XElement(maml + "uri"),
                                    new XElement(maml + "description",
                                        new XElement(maml + "para", "description"))))));
                    helpItems.Add(commandElement);

                    commandElement.Add(
                        new XElement(command + "returnValues",
                            new XElement(command + "returnValue",
                                new XElement(dev + "type",
                                    new XElement(maml + "name", "String"),
                                    new XElement(maml + "uri"),
                                    new XElement(maml + "description",
                                        new XElement(maml + "para", "description"))))));

                    XElement examplesElement = new XElement(command + "examples");
                    int exampleCount = 1;
                    foreach (var exampleAttr in examples.OrderBy(e => e.SortOrder))
                    {
                        XElement example = new XElement(command + "example");
                        string title = string.Format("------------------EXAMPLE {0}---------------------", exampleCount);
                        example.Add(new XElement(maml + "title", title));
                        example.Add(new XElement(maml + "introduction", new XElement(maml + "para", exampleAttr.Introduction)));
                        example.Add(new XElement(dev + "code", exampleAttr.Code));
                        example.Add(new XElement(maml + "remarks", new XElement(maml + "para", exampleAttr.Remarks)));
                        example.Add(new XElement(command + "commandLines",
                            new XElement(command + "commandLine",
                                new XElement(command + "commandText"))));
                        examplesElement.Add(example);
                        exampleCount++;
                    }
                    commandElement.Add(examplesElement);
                }
            }
            doc.Save(outFile);


        }

        private class SyntaxItem
        {
            public string Name;
            public List<Parameter> Parameters;

            public SyntaxItem(string name)
            {
                this.Name = name;
                this.Parameters = new List<Parameter>();
            }

            public class Parameter
            {
                public bool Required;
                public int Position;
                public string Name;
                public string Description;
                public string Type;
            }
        }
    }
}
