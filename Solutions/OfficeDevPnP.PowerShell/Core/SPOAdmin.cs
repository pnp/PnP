using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core
{
    public static class SPOAdmin
    {
        public static bool IsTenantAdminSite(ClientContext clientContext)
        {
            try
            {
                Tenant tenant = new Tenant((ClientRuntimeContext)clientContext);
                clientContext.ExecuteQuery();
                return true;
            }
            catch (Microsoft.SharePoint.Client.ClientRequestException)
            {
                return false;
            }
            catch (Microsoft.SharePoint.Client.ServerException)
            {
                return false;
            }
        }

        public static IEnumerable<Zone> FindZone(string match)
        {
            var zones = AllZones();

            var results = zones.Where(x => x.Description.ToLower().IndexOf(match.ToLower()) > -1 || x.Identifier.ToLower().Contains(match.ToLower()));

            return results;
        }

        

        public static IEnumerable<Zone> AllZones()
        {
            foreach (var zone in Enum.GetValues(typeof(OfficeDevPnP.Core.Enums.TimeZone)))
            {
                var description = zone.ToString();
                var identifier = description.Split('_')[0];
                identifier = identifier.Replace("PLUS", "+").Replace("MINUS", "-");
                if(identifier.Length > 3)
                {
                    identifier = identifier.Substring(0, identifier.Length - 2) + ":" + identifier.Substring(identifier.Length-2, 2);
                }

                description = description.Substring(description.IndexOf('_') + 1).Replace("_", " ");

                yield return new Zone((int)zone,identifier,description);
                 
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

        [Obsolete("Use OfficeDev/PnP.Core")]
        public static SPOTenantWebTemplateCollection GetWebTemplates(uint lcid, int compatibilityLevel, ClientContext clientContext)
        {
            Tenant tenant = new Tenant((ClientRuntimeContext)clientContext);

            var templates = tenant.GetSPOTenantWebTemplates(lcid, compatibilityLevel);

            clientContext.Load(templates);

            clientContext.ExecuteQuery();

            return templates;
        }
    }
}
