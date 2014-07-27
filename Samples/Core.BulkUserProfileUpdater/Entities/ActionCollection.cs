namespace Contoso.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Actions Collection
    /// </summary>
    [XmlRoot("Actions")]
    [Serializable]
    public class ActionCollection : Collection<BaseAction>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCollection"/> class.
        /// </summary>
        public ActionCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCollection"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public ActionCollection(IList<BaseAction> list)
            : base(list)
        {
        }

        #endregion Constructors
    }
}