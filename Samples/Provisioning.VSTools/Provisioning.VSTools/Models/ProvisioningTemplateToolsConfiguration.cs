using System.Xml.Serialization;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace Perficient.Provisioning.VSTools.Models
{
    public class ProvisioningTemplateToolsConfiguration
    {
        public bool ToolsEnabled { get; set; }
        public Deployment Deployment { get; set; }

        [XmlArray("Templates")]
        [XmlArrayItem("Template")]
        public List<Template> Templates { get; set; }


        public ProvisioningTemplateToolsConfiguration()
        {
            Deployment = new Deployment();
            Templates = new List<Template>();
        }
    }

    public class Deployment
    {
        public string TargetSite { get; set; }
        public Authentication Authentication { get; set; }
    }

    public class Authentication
    {
        public Office365 Office365 { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
    }

    public class Office365
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Template
    {
        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "ResourcesFolder")]
        public string ResourcesFolder { get; set; }
    }
}
