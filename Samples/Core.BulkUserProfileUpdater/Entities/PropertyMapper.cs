namespace Contoso.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The User property mapper instance
    /// </summary>
    [XmlInclude(typeof(PropertyBase))]
    [XmlInclude(typeof(PropertyCollection))]
    [XmlInclude(typeof(BaseAction))]
    [XmlInclude(typeof(LoggerBase))]
    [Serializable]
    public class PropertyMapper
    {
        #region Properties

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public ActionCollection Actions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the loggers.
        /// </summary>
        /// <value>
        /// The loggers.
        /// </value>
        public LoggerCollection Loggers
        {
            get;
            set;
        }

        [XmlAttribute("logVerbose")]
        public string LogVerbose
        {
            get;
            set;
        }

        #endregion Properties
    }
}