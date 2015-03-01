using System.Diagnostics.CodeAnalysis;

namespace OfficeDevPnP.Core.Entities
{
    /// <summary>
    /// Class containing variation configuration information
    /// </summary>
    public class VariationInformation
    {
        /// <summary>
        /// Automatic creation 
        /// Mapped to property "EnableAutoSpawnPropertyName"
        /// </summary>
        public bool AutomaticCreation { get; set; }

        /// <summary>
        /// Recreate Deleted Target Page; set to false to enable recreation
        /// Mapped to property "AutoSpawnStopAfterDeletePropertyName"
        /// </summary>
        public bool RecreateDeletedTargetPage { get; set; }

        /// <summary>
        /// Update Target Page Web Parts
        /// Mapped to property "UpdateWebPartsPropertyName"
        /// </summary>
        public bool UpdateTargetPageWebParts { get; set; }

        /// <summary>
        /// Copy resources
        /// Mapped to property "CopyResourcesPropertyName"
        /// </summary>
        public bool CopyResources { get; set; }

        /// <summary>
        /// Send email notification
        /// Mapped to property "SendNotificationEmailPropertyName"
        /// </summary>
        public bool SendNotificationEmail { get; set; }

        /// <summary>
        /// Configuration setting site template to be used for the top sites of each label
        /// Mapped to property "SourceVarRootWebTemplatePropertyName"
        /// </summary>
        public string RootWebTemplate { get; set; }
    }

    /// <summary>
    /// Class represents variation label
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Helper class")]
    public class VariationLabel
    {
        /// <summary>
        /// The variation label title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The variation label description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The flag to control display name
        /// </summary>
        public string FlagControlDisplayName { get; set; }

        /// <summary>
        /// The variation label language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The variation label locale
        /// </summary>
        public uint Locale { get; set; }

        /// <summary>
        /// The hierarchy creation mode
        /// </summary>
        public string HierarchyCreationMode { get; set; }

        /// <summary>
        /// Set as source variation
        /// </summary>
        public bool IsSource { get; set; }
    }
}
