using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.CmdletHelpGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdlets = new List<CmdletInfo>();
            var inFile = args[0];
            var outFile = args[1];
            var toc = new List<CmdletInfo>();

            // Specify an additional (third) parameter pointing to the Solution folder to generate Markdown. The markdown 
            // will be created in the Documentation folder underneath the solution folder.
            bool generateMarkdown = false;
            string solutionDir = null;
            if (args.Length > 2)
            {
                solutionDir = args[2];
                generateMarkdown = true;
            }
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", string.Empty));

            XNamespace ns = "http://msh";
            var helpItems = new XElement(ns + "helpItems", new XAttribute("schema", "maml"));
            doc.Add(helpItems);


            XNamespace maml = "http://schemas.microsoft.com/maml/2004/10";
            XNamespace command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
            XNamespace dev = "http://schemas.microsoft.com/maml/dev/2004/10";

            var mamlNsAttr = new XAttribute(XNamespace.Xmlns + "maml", "http://schemas.microsoft.com/maml/2004/10");
            var commandNsAttr = new XAttribute(XNamespace.Xmlns + "command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
            var devNsAttr = new XAttribute(XNamespace.Xmlns + "dev", "http://schemas.microsoft.com/maml/dev/2004/10");

            var assembly = Assembly.LoadFrom(inFile);
            var types = assembly.GetTypes();
            foreach (var t in types)
            {
                if (t.BaseType.Name == "SPOCmdlet" || t.BaseType.Name == "PSCmdlet" || t.BaseType.Name == "SPOWebCmdlet" || t.BaseType.Name == "SPOAdminCmdlet")
                {

                    //XElement examples = new XElement(command + "examples");

                    var verb = string.Empty;
                    var noun = string.Empty;
                    var description = string.Empty;
                    var detaileddescription = string.Empty;
                    var copyright = string.Empty;
                    var version = string.Empty;
                    var category = string.Empty;
                    var attrs = t.GetCustomAttributes();
                    var examples = new List<CmdletExampleAttribute>();

                    //System.Attribute.GetCustomAttributes(t); 

                    foreach (var attr in attrs)
                    {
                        if (attr is CmdletAttribute)
                        {
                            var a = (CmdletAttribute)attr;
                            verb = a.VerbName;
                            noun = a.NounName;

                        }
                        if (attr is CmdletHelpAttribute)
                        {
                            var a = (CmdletHelpAttribute)attr;
                            description = a.Description;
                            copyright = a.Copyright;
                            version = a.Version;
                            detaileddescription = a.DetailedDescription;
                            category = a.Category;
                        }
                        if (attr is CmdletExampleAttribute)
                        {
                            var a = (CmdletExampleAttribute)attr;
                            examples.Add(a);


                        }
                    }

                    var cmdletInfo = new CmdletInfo(verb, noun);
                    cmdletInfo.Description = description;
                    cmdletInfo.DetailedDescription = detaileddescription;
                    cmdletInfo.Version = version;
                    cmdletInfo.Copyright = copyright;
                    cmdletInfo.Category = category;


                    var commandElement = new XElement(command + "command", mamlNsAttr, commandNsAttr, devNsAttr);
                    var detailsElement = new XElement(command + "details");
                    commandElement.Add(detailsElement);

                    detailsElement.Add(new XElement(command + "name", string.Format("{0}-{1}", verb, noun)));
                    detailsElement.Add(new XElement(maml + "description", new XElement(maml + "para", description)));
                    detailsElement.Add(new XElement(maml + "copyright", new XElement(maml + "para", copyright)));
                    detailsElement.Add(new XElement(command + "verb", verb));
                    detailsElement.Add(new XElement(command + "noun", noun));
                    detailsElement.Add(new XElement(dev + "version", version));

                    commandElement.Add(new XElement(maml + "description", new XElement(maml + "para", detaileddescription)));

                    var syntaxElement = new XElement(command + "syntax");
                    commandElement.Add(syntaxElement);

                    var fields = t.GetFields();
                    var syntaxItems = new List<SyntaxItem>();
                    foreach (var field in fields)
                    {
                        foreach (Attribute attr in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                        {
                            if (attr is ParameterAttribute)
                            {
                                var a = (ParameterAttribute)attr;

                                if (!a.DontShow)
                                {
                                    if (a.ParameterSetName != ParameterAttribute.AllParameterSets)
                                    {
                                        var cmdletSyntax = cmdletInfo.Syntaxes.FirstOrDefault(c => c.ParameterSetName == a.ParameterSetName);
                                        if (cmdletSyntax == null)
                                        {
                                            cmdletSyntax = new CmdletSyntax();
                                            cmdletSyntax.ParameterSetName = a.ParameterSetName;
                                            cmdletInfo.Syntaxes.Add(cmdletSyntax);
                                        }

                                        cmdletSyntax.Parameters.Add(new CmdletParameterInfo() {Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name});
                                    }

                                    var syntaxItem = syntaxItems.FirstOrDefault(x => x.Name == a.ParameterSetName);
                                    if (syntaxItem == null)
                                    {
                                        syntaxItem = new SyntaxItem(a.ParameterSetName);
                                        syntaxItems.Add(syntaxItem);
                                    }
                                    syntaxItem.Parameters.Add(new SyntaxItem.Parameter() {Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name});
                                }
                            }
                        }
                    }

                    // all parameters
                    foreach (var field in fields)
                    {
                        foreach (Attribute attr in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                        {
                            if (attr is ParameterAttribute)
                            {
                                var a = (ParameterAttribute)attr;
                                if (!a.DontShow)
                                {
                                    if (a.ParameterSetName == ParameterAttribute.AllParameterSets)
                                    {
                                        foreach (var si in syntaxItems)
                                        {
                                            si.Parameters.Add(new SyntaxItem.Parameter() {Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name});
                                        }

                                        if (cmdletInfo.Syntaxes.Count == 0)
                                        {
                                            cmdletInfo.Syntaxes.Add(new CmdletSyntax() {ParameterSetName = ParameterAttribute.AllParameterSets});
                                        }
                                        foreach (var cmdletSyntax in cmdletInfo.Syntaxes)
                                        {
                                            cmdletSyntax.Parameters.Add(new CmdletParameterInfo() {Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name});
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //
                    foreach (var syntaxItem in syntaxItems)
                    {
                        var syntaxItemElement = new XElement(command + "syntaxItem");
                        syntaxElement.Add(syntaxItemElement);

                        syntaxItemElement.Add(new XElement(maml + "name", string.Format("{0}-{1}", verb, noun)));
                        foreach (var parameter in syntaxItem.Parameters)
                        {

                            var parameterElement = new XElement(command + "parameter", new XAttribute("required", parameter.Required), new XAttribute("position", parameter.Position > 0 ? parameter.Position.ToString() : "named"));

                            parameterElement.Add(new XElement(maml + "name", parameter.Name));

                            parameterElement.Add(new XElement(maml + "description", new XElement(maml + "para", parameter.Description)));
                            parameterElement.Add(new XElement(command + "parameterValue", parameter.Type));

                            syntaxItemElement.Add(parameterElement);
                        }
                    }

                    var parametersElement = new XElement(command + "parameters");
                    commandElement.Add(parametersElement);

                    foreach (var field in fields)
                    {
                        foreach (Attribute attr in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                        {
                            if (attr is ParameterAttribute)
                            {
                                var a = (ParameterAttribute)attr;
                                if (!a.DontShow)
                                {
                                    cmdletInfo.Parameters.Add(new CmdletParameterInfo() {Name = field.Name, Description = a.HelpMessage, Position = a.Position, Required = a.Mandatory, Type = field.FieldType.Name});

                                    var parameter2Element = new XElement(command + "parameter", new XAttribute("required", a.Mandatory), new XAttribute("position", a.Position > 0 ? a.Position.ToString() : "named"));

                                    parameter2Element.Add(new XElement(maml + "name", field.Name));

                                    parameter2Element.Add(new XElement(maml + "description", new XElement(maml + "para", a.HelpMessage)));
                                    var parameterValueElement = new XElement(command + "parameterValue", field.FieldType.Name, new XAttribute("required", a.Mandatory));
                                    parameter2Element.Add(parameterValueElement);

                                    var devElement = new XElement(dev + "type");
                                    devElement.Add(new XElement(maml + "name", field.FieldType.Name));
                                    devElement.Add(new XElement(maml + "uri"));

                                    parameter2Element.Add(devElement);

                                    parametersElement.Add(parameter2Element);
                                }
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

                    var examplesElement = new XElement(command + "examples");
                    var exampleCount = 1;
                    foreach (var exampleAttr in examples.OrderBy(e => e.SortOrder))
                    {
                        var example = new XElement(command + "example");
                        var title = string.Format("------------------EXAMPLE {0}---------------------", exampleCount);
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

                    if (generateMarkdown)
                    {
                        if (!string.IsNullOrEmpty(cmdletInfo.Verb) && !string.IsNullOrEmpty(cmdletInfo.Noun))
                        {
                            toc.Add(cmdletInfo);
                            using (var docfile = new System.IO.StreamWriter(string.Format("{0}\\Documentation\\{1}{2}.md", solutionDir, cmdletInfo.Verb, cmdletInfo.Noun)))
                            {
                                docfile.WriteLine("#{0}", cmdletInfo.FullCommand);
                                docfile.WriteLine("*Topic automatically generated on: {0}*", DateTime.Now.ToString("yyyy-MM-dd"));
                                docfile.WriteLine("");
                                docfile.WriteLine(cmdletInfo.Description);
                                docfile.WriteLine("##Syntax");
                                foreach (var cmdletSyntax in cmdletInfo.Syntaxes)
                                {
                                    var syntaxText = new StringBuilder();
                                    syntaxText.AppendFormat("```powershell\r\n{0}", cmdletInfo.FullCommand);
                                    foreach (var par in cmdletSyntax.Parameters.OrderBy(p => p.Position))
                                    {
                                        syntaxText.Append(" ");
                                        if (!par.Required)
                                        {
                                            syntaxText.Append("[");
                                        }
                                        syntaxText.AppendFormat("-{0} [<{1}>]", par.Name, par.Type);
                                        if (!par.Required)
                                        {
                                            syntaxText.Append("]");
                                        }
                                    }
                                    // Add All ParameterSet ones
                                    docfile.WriteLine(syntaxText);
                                    docfile.WriteLine("```");
                                    docfile.WriteLine("&nbsp;");
                                    docfile.WriteLine("");
                                }

                                if (!string.IsNullOrEmpty(cmdletInfo.DetailedDescription))
                                {
                                    docfile.WriteLine("##Detailed Description");
                                    docfile.WriteLine(cmdletInfo.DetailedDescription);
                                    docfile.WriteLine("");
                                }
                                docfile.WriteLine("##Parameters");
                                docfile.WriteLine("Parameter|Type|Required|Description");
                                docfile.WriteLine("---------|----|--------|-----------");
                                foreach (var par in cmdletInfo.Parameters.OrderBy(x => x.Name))
                                {
                                    docfile.WriteLine("{0}|{1}|{2}|{3}", par.Name, par.Type, par.Required ? "True" : "False", par.Description);
                                }
                                if (examples.Any())
                                    docfile.WriteLine("##Examples");
                                var examplesCount = 1;
                                foreach (var example in examples.OrderBy(e => e.SortOrder))
                                {
                                    docfile.WriteLine(example.Introduction);
                                    docfile.WriteLine("###Example {0}", examplesCount);
                                    docfile.WriteLine("    {0}", example.Code);
                                    docfile.WriteLine(example.Remarks);
                                    examplesCount++;
                                }
                            }
                        }
                    }
                }

            }
            doc.Save(outFile);

            if (generateMarkdown)
            {
                // Create the readme.md
                using (var readme = new System.IO.StreamWriter(string.Format("{0}\\Documentation\\readme.md", solutionDir)))
                {
                    readme.WriteLine("# Cmdlet Documentation #");
                    readme.WriteLine("Below you can find a list of all the available cmdlets. Many commands provide built-in help and examples. Retrieve the detailed help with ");
                    readme.WriteLine("\r\n```powershell\r\nGet-Help Connect-SPOnline -Detailed\r\n```\r\n\r\n");

                    // Get all unique categories
                    var categories = toc.Select(c => c.Category).Distinct();

                    foreach (var category in categories.OrderBy(c => c))
                    {
                        readme.WriteLine("##{0}",category);

                        readme.WriteLine("Cmdlet|Description");
                        readme.WriteLine(":-----|:----------");
                        foreach (var cmdletInfo in toc.Where(c => c.Category == category).OrderBy(c => c.Noun))
                        {
                            var description = cmdletInfo.Description.Replace("\r\n", " ");
                            readme.WriteLine("**[{0}]({1}{2}.md)** |{3}", cmdletInfo.FullCommand, cmdletInfo.Verb, cmdletInfo.Noun, description);
                        }
                    }
                }

            }

        }

        private class SyntaxItem
        {
            public readonly string Name;
            public readonly List<Parameter> Parameters;

            public SyntaxItem(string name)
            {
                Name = name;
                Parameters = new List<Parameter>();
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
