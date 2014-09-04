
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Patterns.Provisioning
{
    //This file has been created by copying the XML file and then using
    //Edit --> Paste Special --> Paste XML as classes
    //
    //If the XML configuration changes this file needs to be updated accordingly 




    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Configurations
    {

        private ConfigurationsTemplate[] templatesField;

        private ConfigurationsTheme[] themesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Template", IsNullable = false)]
        public ConfigurationsTemplate[] Templates
        {
            get
            {
                return this.templatesField;
            }
            set
            {
                this.templatesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Theme", IsNullable = false)]
        public ConfigurationsTheme[] Themes
        {
            get
            {
                return this.themesField;
            }
            set
            {
                this.themesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplate
    {

        private ConfigurationsTemplateFeature[] featuresField;

        private ConfigurationsTemplateModule[] modulesField;

        private ConfigurationsTemplateFiles filesField;

        private ConfigurationsTemplateCustomActions customActionsField;

        private object sitesField;

        private ConfigurationsTemplateListInstance[] listsField;

        private ConfigurationsTemplateNavigationNodes navigationNodesField;

        private string nameField;

        private string titleField;

        private bool enabledField;

        private string rootTemplateField;

        private string descriptionField;

        private string siteLogoUrlField;

        private bool rootWebOnlyField;

        private bool subWebOnlyField;

        private string managedPathField;

        private byte storageMaximumLevelField;

        private bool storageMaximumLevelFieldSpecified;

        private byte storageWarningLevelField;

        private bool storageWarningLevelFieldSpecified;

        private byte userCodeMaximumLevelField;

        private bool userCodeMaximumLevelFieldSpecified;

        private byte userCodeWarningLevelField;

        private bool userCodeWarningLevelFieldSpecified;

        private string themeField;

        private string siteLogoURLField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Feature", IsNullable = false)]
        public ConfigurationsTemplateFeature[] Features
        {
            get
            {
                return this.featuresField;
            }
            set
            {
                this.featuresField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Module", IsNullable = false)]
        public ConfigurationsTemplateModule[] Modules
        {
            get
            {
                return this.modulesField;
            }
            set
            {
                this.modulesField = value;
            }
        }

        /// <remarks/>
        public ConfigurationsTemplateFiles Files
        {
            get
            {
                return this.filesField;
            }
            set
            {
                this.filesField = value;
            }
        }

        /// <remarks/>
        public ConfigurationsTemplateCustomActions CustomActions
        {
            get
            {
                return this.customActionsField;
            }
            set
            {
                this.customActionsField = value;
            }
        }

        /// <remarks/>
        public object Sites
        {
            get
            {
                return this.sitesField;
            }
            set
            {
                this.sitesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ListInstance", IsNullable = false)]
        public ConfigurationsTemplateListInstance[] Lists
        {
            get
            {
                return this.listsField;
            }
            set
            {
                this.listsField = value;
            }
        }

        /// <remarks/>
        public ConfigurationsTemplateNavigationNodes NavigationNodes
        {
            get
            {
                return this.navigationNodesField;
            }
            set
            {
                this.navigationNodesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RootTemplate
        {
            get
            {
                return this.rootTemplateField;
            }
            set
            {
                this.rootTemplateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SiteLogoUrl
        {
            get
            {
                return this.siteLogoUrlField;
            }
            set
            {
                this.siteLogoUrlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool SubWebOnly
        {
            get
            {
                return this.subWebOnlyField;
            }
            set
            {
                this.subWebOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ManagedPath
        {
            get
            {
                return this.managedPathField;
            }
            set
            {
                this.managedPathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte StorageMaximumLevel
        {
            get
            {
                return this.storageMaximumLevelField;
            }
            set
            {
                this.storageMaximumLevelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StorageMaximumLevelSpecified
        {
            get
            {
                return this.storageMaximumLevelFieldSpecified;
            }
            set
            {
                this.storageMaximumLevelFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte StorageWarningLevel
        {
            get
            {
                return this.storageWarningLevelField;
            }
            set
            {
                this.storageWarningLevelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StorageWarningLevelSpecified
        {
            get
            {
                return this.storageWarningLevelFieldSpecified;
            }
            set
            {
                this.storageWarningLevelFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte UserCodeMaximumLevel
        {
            get
            {
                return this.userCodeMaximumLevelField;
            }
            set
            {
                this.userCodeMaximumLevelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UserCodeMaximumLevelSpecified
        {
            get
            {
                return this.userCodeMaximumLevelFieldSpecified;
            }
            set
            {
                this.userCodeMaximumLevelFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte UserCodeWarningLevel
        {
            get
            {
                return this.userCodeWarningLevelField;
            }
            set
            {
                this.userCodeWarningLevelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UserCodeWarningLevelSpecified
        {
            get
            {
                return this.userCodeWarningLevelFieldSpecified;
            }
            set
            {
                this.userCodeWarningLevelFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Theme
        {
            get
            {
                return this.themeField;
            }
            set
            {
                this.themeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SiteLogoURL
        {
            get
            {
                return this.siteLogoURLField;
            }
            set
            {
                this.siteLogoURLField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateFeature
    {

        private string nameField;

        private string idField;

        private string scopeField;

        private bool activateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Activate
        {
            get
            {
                return this.activateField;
            }
            set
            {
                this.activateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateModule
    {

        private string ctrlSrcField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CtrlSrc
        {
            get
            {
                return this.ctrlSrcField;
            }
            set
            {
                this.ctrlSrcField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateFiles
    {

        private ConfigurationsTemplateFilesFile fileField;

        /// <remarks/>
        public ConfigurationsTemplateFilesFile File
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateFilesFile
    {

        private string srcField;

        private string targetFolderField;

        private bool uploadToDocumentLibrayField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Src
        {
            get
            {
                return this.srcField;
            }
            set
            {
                this.srcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TargetFolder
        {
            get
            {
                return this.targetFolderField;
            }
            set
            {
                this.targetFolderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool UploadToDocumentLibray
        {
            get
            {
                return this.uploadToDocumentLibrayField;
            }
            set
            {
                this.uploadToDocumentLibrayField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateCustomActions
    {

        private ConfigurationsTemplateCustomActionsCustomAction customActionField;

        /// <remarks/>
        public ConfigurationsTemplateCustomActionsCustomAction CustomAction
        {
            get
            {
                return this.customActionField;
            }
            set
            {
                this.customActionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateCustomActionsCustomAction
    {

        private string scriptSrcField;

        private string nameField;

        private string locationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ScriptSrc
        {
            get
            {
                return this.scriptSrcField;
            }
            set
            {
                this.scriptSrcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateListInstance
    {

        private string titleField;

        private string descriptionField;

        private string urlField;

        private bool onQuickLaunchField;

        private byte templateTypeField;

        private string templateFeatureIdField;

        private bool enableVersioningField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool OnQuickLaunch
        {
            get
            {
                return this.onQuickLaunchField;
            }
            set
            {
                this.onQuickLaunchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte TemplateType
        {
            get
            {
                return this.templateTypeField;
            }
            set
            {
                this.templateTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateFeatureId
        {
            get
            {
                return this.templateFeatureIdField;
            }
            set
            {
                this.templateFeatureIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool EnableVersioning
        {
            get
            {
                return this.enableVersioningField;
            }
            set
            {
                this.enableVersioningField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateNavigationNodes
    {

        private ConfigurationsTemplateNavigationNodesNavigationNode navigationNodeField;

        /// <remarks/>
        public ConfigurationsTemplateNavigationNodesNavigationNode NavigationNode
        {
            get
            {
                return this.navigationNodeField;
            }
            set
            {
                this.navigationNodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTemplateNavigationNodesNavigationNode
    {

        private string titleField;

        private string urlField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationsTheme
    {

        private string nameField;

        private string colorFileField;

        private string fontFileField;

        private string backgroundFileField;

        private string masterPageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ColorFile
        {
            get
            {
                return this.colorFileField;
            }
            set
            {
                this.colorFileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FontFile
        {
            get
            {
                return this.fontFileField;
            }
            set
            {
                this.fontFileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BackgroundFile
        {
            get
            {
                return this.backgroundFileField;
            }
            set
            {
                this.backgroundFileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MasterPage
        {
            get
            {
                return this.masterPageField;
            }
            set
            {
                this.masterPageField = value;
            }
        }
    }


}
