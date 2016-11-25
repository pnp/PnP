using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessApps.O365ProjectsApp.Infrastructure.Configuration
{
    public class O365ProjectsConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlNodeReader xnr = new XmlNodeReader(section);
            XmlSerializer xs = new XmlSerializer(typeof(O365ProjectsConfiguration));
            return (xs.Deserialize(xnr));
        }
    }
}
