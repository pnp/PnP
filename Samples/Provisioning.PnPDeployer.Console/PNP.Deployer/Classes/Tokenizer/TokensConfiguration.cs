using System;
using System.Xml.Serialization;
using System.Collections.Generic;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    [Serializable()]
    [XmlRoot("tokensConfiguration")]
    public class TokensConfiguration
    {
        #region Public Members

        [XmlArray("tokens")]
        [XmlArrayItem("token", typeof(Token))]
        public List<Token> Tokens { get; set; }

        #endregion
    }


    [Serializable()]
    public struct Token
    {
        #region Public Members

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        #endregion
    }
}
