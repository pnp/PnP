using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    [Serializable()]
    [XmlRoot("sequencesConfiguration")]
    public class SequencesConfiguration
    {
        #region Public Members

        [XmlArray("sequences")]
        [XmlArrayItem("sequence", typeof(Sequence))]
        public List<Sequence> Sequences { get; set; }

        #endregion
    }
}
