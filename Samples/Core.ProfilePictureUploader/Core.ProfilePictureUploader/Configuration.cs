using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.ProfilePictureUploader
{
    [System.Xml.Serialization.XmlRootAttribute("configuration")]
    public class Configuration
    {
        [System.Xml.Serialization.XmlElementAttribute("tenantName")]
        public string TenantName;
        [System.Xml.Serialization.XmlElementAttribute("pictureSourceCsv")]
        public string PictureSourceCsv;
        [System.Xml.Serialization.XmlElementAttribute("thumbs")]
        public Thumbs Thumbs;
        [System.Xml.Serialization.XmlElementAttribute("targetLibraryPath")]
        public string TargetLibraryPath;
        [System.Xml.Serialization.XmlElementAttribute("additionalProfileProperties")]
        public AdditionalProfileProperties AdditionalProfileProperties;
        [System.Xml.Serialization.XmlElementAttribute("logFile")]
        public LogFile LogFile;
        [System.Xml.Serialization.XmlElementAttribute("uploadDelay")]
        public int UploadDelay;
    }

    [System.Xml.Serialization.XmlRootAttribute("thumbs")]
    public class Thumbs
    {
        [System.Xml.Serialization.XmlAttributeAttribute("upload3Thumbs")]
        public bool Upload3Thumbs = false;
        [System.Xml.Serialization.XmlAttributeAttribute("createSMLThumbs")]
        public bool CreateSMLThumbs = false;
    }

    [System.Xml.Serialization.XmlRootAttribute("additionalProfileProperties")]
    public class AdditionalProfileProperties
    {
        [System.Xml.Serialization.XmlElementAttribute("property")]
        public ProfileProperty[] Properties;
    }

    [System.Xml.Serialization.XmlRootAttribute("property")]
    public class ProfileProperty
    {
        [System.Xml.Serialization.XmlAttributeAttribute("name")]
        public string Name;
        [System.Xml.Serialization.XmlAttributeAttribute("value")]
        public string Value;
    }

    [System.Xml.Serialization.XmlRootAttribute("logFile")]
    public class LogFile
    {
        [System.Xml.Serialization.XmlAttributeAttribute("path")]
        public string Path;
        [System.Xml.Serialization.XmlAttributeAttribute("enableLogging")]
        public bool EnableLogging;
        [System.Xml.Serialization.XmlAttributeAttribute("loggingLevel")]
        public string LoggingLevel;
    }
}
