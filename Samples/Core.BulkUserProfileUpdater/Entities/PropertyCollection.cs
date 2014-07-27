namespace Contoso.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The collection of properties
    /// </summary>
    [XmlRoot("Properties")]
    [Serializable]
    public class PropertyCollection : Collection<PropertyBase>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCollection"/> class.
        /// </summary>
        public PropertyCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCollection"/> class.
        /// </summary>
        /// <param name="list">The collection of properties.</param>
        public PropertyCollection(IList<PropertyBase> list)
            : base(list)
        {
        }

        #endregion Constructors
    }
}