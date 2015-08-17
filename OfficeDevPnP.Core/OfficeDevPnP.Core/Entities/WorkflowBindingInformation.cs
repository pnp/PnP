using System;

namespace OfficeDevPnP.Core.Entities
{
    /// <summary>
    /// Class to represent information about workflow binding.
    /// </summary>
    public class WorkflowBindingInformation
    {
        /// <summary>
        /// Name of the workflow association.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Guid ID of the Workflow template which is being associated.
        /// </summary>
        public Guid BaseTemplateId { get; set; }

        /// <summary>
        /// Relative URL of the workflow history list.
        /// </summary>
        public string HistoryListUrl { get; set; }

        /// <summary>
        /// Relative URL of the workflow tasks list.
        /// </summary>
        public string TaskListUrl { get; set; }

        /// <summary>
        /// Allow this workflow to be manually started by an authenticated user with Edit Item permissions.
        /// </summary>
        public bool AllowManual { get; set; }

        /// <summary>
        /// Creating a new item will start this workflow.
        /// </summary>
        public bool AutoStartCreate { get; set; }

        /// <summary>
        /// Changing an item will start this workflow.
        /// </summary>
        public bool AutoStartChange { get; set; }
    }
}
