using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTimeZoneId")]
    [CmdletHelp("Adds a SharePoint App to a site",
        Details = "This commands requires that you have an app package to deploy")]
    [CmdletExample(
        Code = @"PS:> Add-SPOnlineApp -Path c:\files\demo.app -LoadOnly",
        Remarks = @"This will load the app in the demo.app package, but will not install it to the site.
 ")]
    [CmdletExample(
        Code = @"PS:> Add-SPOnlineApp -Path c:\files\demo.app -Force",
        Remarks = @"This load first activate the app sideloading feature, upload and install the app, and deactivate the app sideloading feature.
    ")]
    public class GetTimeZoneId : PSCmdlet
    {
        [Parameter(Mandatory = false)]
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

            var results = zones.Where(x => x.Description.ToLower().IndexOf(match.ToLower()) > -1 || x.Identifier.ToLower().Contains(match.ToLower()));

            return results;
        }



        public IEnumerable<Zone> AllZones()
        {
            foreach (var zone in Enum.GetValues(typeof(OfficeDevPnP.Core.Enums.TimeZone)))
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
                this.Id = id;
                this.Identifier = identifier;
                this.Description = description;
            }
        }
    }
}
