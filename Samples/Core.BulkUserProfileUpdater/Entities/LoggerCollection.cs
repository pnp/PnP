namespace Contoso.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Logger Collection
    /// </summary>
    [XmlRoot("Loggers")]
    [Serializable]
    public class LoggerCollection : Collection<LoggerBase>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerCollection" /> class.
        /// </summary>
        public LoggerCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerCollection"/> class.
        /// </summary>
        /// <param name="list">The collection of properties.</param>
        public LoggerCollection(IList<LoggerBase> list)
            : base(list)
        {
        }

        #endregion Constructors
    }
}