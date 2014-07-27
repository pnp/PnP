using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAMS.Core.Entities
{
    /// <summary>
    /// Class that describes information about a web part 
    /// </summary>
    public class WebPartEntity
    {
        /// <summary>
        /// XML representation of the web part
        /// </summary>
        public string WebPartXml
        {
            get;
            set;
        }

        /// <summary>
        /// Zone that will need to hold the web part
        /// </summary>
        public string WebPartZone
        {
            get;
            set;
        }

        /// <summary>
        /// Index (order) of the web part in it's zone
        /// </summary>
        public int WebPartIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Title of the web part
        /// </summary>
        public string WebPartTitle
        {
            get;
            set;
        }

    }
}
