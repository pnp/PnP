using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    public static class XMLExtensions
    {
        #region Private extension methods for handling XML content

        /// <summary>
        /// Internal Extension method to convert an XElement into an XmlElement
        /// </summary>
        /// <param name="element">The XElement to convert</param>
        /// <returns>The converted XmlElement</returns>
        internal static XmlElement ToXmlElement(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            using (XmlReader reader = element.CreateReader())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                return (doc.DocumentElement);
            }
        }

        /// <summary>
        /// Internal extension method to convert an XmlElement into an XElement
        /// </summary>
        /// <param name="element">The XmlElement to convert</param>
        /// <returns>The converted XElement</returns>
        internal static XElement ToXElement(this XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            using (XmlReader reader = new XmlNodeReader(element))
            {
                XElement result = XElement.Load(reader);
                return (result);
            }
        }

        /// <summary>
        /// Internal extension method to convert a String into an XElement
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The converted XElement</returns>
        internal static XElement ToXElement(this String xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            XElement element = XElement.Parse(xml);
            return (element);
        }

        /// <summary>
        /// Internal extension method to convert a String into an XmlElement
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The converted XmlElement</returns>
        internal static XmlElement ToXmlElement(this String xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            XElement element = XElement.Parse(xml);
            return (element.ToXmlElement());
        }

        /// <summary>
        /// Internal extension method to convert a String into an XmlNode
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The converted XmlNode</returns>
        internal static XmlNode ToXmlNode(this String xml)
        {
            if (String.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("xml");
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return (doc.DocumentElement);
            }
            catch (XmlException)
            {
                XmlDocument doc = new XmlDocument();
                return (doc.CreateCDataSection(xml));
            }
        }

        /// <summary>
        /// Internal extension method to convert the XML configuration
        /// of a provider into a String
        /// </summary>
        /// <param name="xml">The XmlNode with the provider configuration</param>
        /// <returns>The string representing the provider configuration</returns>
        internal static String ToProviderConfiguration(this XmlNode xml)
        {
            switch (xml.NodeType)
            {
                case XmlNodeType.CDATA:
                    return (((XmlCDataSection)xml).InnerText);
                default:
                    return (xml.OuterXml);
            }
        }

        /// <summary>
        /// Internal extension method to fix XML Namespaces onto a target XML element
        /// </summary>
        /// <param name="element">The XML element to fixup</param>
        /// <returns>The fixed up XML element</returns>
        internal static XmlElement FixupElementNamespace(this XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            XElement xElement = XElement.Parse(element.OuterXml);
            XElement cleanedElement = new XElement(xElement.Name.LocalName,
                from a in xElement.Attributes()
                where a.IsNamespaceDeclaration == false
                select a);
            return (cleanedElement.ToXmlElement());
        }

        #endregion
    }
}

