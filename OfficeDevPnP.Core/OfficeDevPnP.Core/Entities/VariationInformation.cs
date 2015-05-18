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

}
