using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.CmdletHelpGenerator
{
    public class CmdletInfo
    {
        public string Verb { get; set; }
        public string Noun { get; set; }

        public string Description { get; set; }

        public string DetailedDescription { get; set; }

        public List<CmdletParameterInfo> Parameters { get; set; }

        public string Version { get; set; }

        public string Copyright { get; set; }

        public List<CmdletSyntax> Syntaxes { get; set; } 

        public string FullCommand
        {
            get
            {
                return string.Format("{0}-{1}", Verb, Noun);
            }
        }

        public CmdletInfo(string verb, string noun)
        {
            Verb = verb;
            Noun = noun;
            Parameters = new List<CmdletParameterInfo>();
            Syntaxes = new List<CmdletSyntax>();
        }
    }
}
