using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTimeZoneId")]
    [CmdletHelp("Returns a time zone ID")]
    [CmdletExample(
        Code = @"PS:> Get-SPOTimeZoneId",
        Remarks = @"This will return all time zone IDs in use by Office 365.
 ")]
    [CmdletExample(
        Code = @"PS:> Get-SPOTimeZoneId -Match Stockholm",
        Remarks = @"This will return the time zone IDs for Stockholm
    ")]
    public class GetTimeZoneId : PSCmdlet
    {
        [Parameter(Mandatory = false, Position=0)]
        public string Match;

        protected override void ProcessRecord()
        {
            if (Match != null)
            {
                WriteObject(FindZone(Match));
            }
            else
            {
                WriteObject(AllZones());
            }
        }

        private IEnumerable<Zone> FindZone(string match)
        {
            var zones = AllZones();

            var results = zones.Where(x => x.Description.ToLower().IndexOf(match.ToLower(), StringComparison.Ordinal) > -1 || x.Identifier.ToLower().Contains(match.ToLower()));

            return results;
        }



        public IEnumerable<Zone> AllZones()
        {
            foreach (var zone in Enum.GetValues(typeof(Core.Enums.TimeZone)))
            {
                var description = zone.ToString();
                var identifier = description.Split('_')[0];
                identifier = identifier.Replace("PLUS", "+").Replace("MINUS", "-");
                if (identifier.Length > 3)
                {
                    identifier = identifier.Substring(0, identifier.Length - 2) + ":" + identifier.Substring(identifier.Length - 2, 2);
                }

                description = description.Substring(description.IndexOf('_') + 1).Replace("_", " ");

                yield return new Zone((int)zone, identifier, description);

            }
        }

        public class Zone
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string Identifier { get; set; }

            public Zone(int id, string identifier, string description)
            {
                Id = id;
                Identifier = identifier;
                Description = description;
            }
        }
    }
}
