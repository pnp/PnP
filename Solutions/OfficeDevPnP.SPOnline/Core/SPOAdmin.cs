using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Core
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
            var zones = GetZones();

            var results = zones.Where(x => x.Description.ToLower().IndexOf(match.ToLower()) > -1 || x.Identifier.ToLower().Contains(match.ToLower()));

            return results;
        }

        public static IEnumerable<Zone> AllZones()
        {
            return GetZones();
        }

        private static List<Zone> GetZones()
        {
            List<Zone> zones = new List<Zone>();
            zones.Add(new Zone(2, "UTC", "Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London"));
            zones.Add(new Zone(3, "UTC+01:00", "Brussels, Copenhagen, Madrid, Paris"));
            zones.Add(new Zone(4, "UTC+01:00", "Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"));
            zones.Add(new Zone(5, "UTC+02:00", "Athens, Bucharest, Istanbul"));
            zones.Add(new Zone(6, "UTC+01:00", "Belgrade, Bratislava, Budapest, Ljubljana, Prague"));
            zones.Add(new Zone(7, "UTC+02:00", "Minsk"));
            zones.Add(new Zone(8, "UTC-03:00", "Brasilia"));
            zones.Add(new Zone(9, "UTC-04:00", "Atlantic Time (Canada)"));
            zones.Add(new Zone(10, "UTC-05:00", "Eastern Time (US and Canada)"));
            zones.Add(new Zone(11, "UTC-06:00", "Central Time (US and Canada)"));
            zones.Add(new Zone(12, "UTC-07:00", "Mountain Time (US and Canada)"));
            zones.Add(new Zone(13, "UTC-08:00", "Pacific Time (US and Canada)"));
            zones.Add(new Zone(14, "UTC-09:00", "Alaska"));
            zones.Add(new Zone(15, "UTC-10:00", "Hawaii"));
            zones.Add(new Zone(16, "UTC-11:00", "Midway Island, Samoa"));
            zones.Add(new Zone(17, "UTC+12:00", "Aukland, Wellington"));
            zones.Add(new Zone(18, "UTC+10:00", "Brisbane"));
            zones.Add(new Zone(19, "UTC+09:30", "Adelaide"));
            zones.Add(new Zone(20, "UTC+09:00", "Osaka, Sapporo, Tokyo"));
            zones.Add(new Zone(21, "UTC+08:00", "Kuala Lumpur, Singapore"));
            zones.Add(new Zone(22, "UTC+07:00", "Bangkok, Hanoi, Jakarta"));
            zones.Add(new Zone(23, "UTC+05:30", "Chennai, Kolkata, Mumbai, New Delhi"));
            zones.Add(new Zone(24, "UTC+04:00", "Abu Dhabi, Muscat"));
            zones.Add(new Zone(25, "UTC+03:30", "Tehran"));
            zones.Add(new Zone(26, "UTC+03:00", "Baghdad"));
            zones.Add(new Zone(27, "UTC+02:00", "Jerusalem"));
            zones.Add(new Zone(28, "UTC-03:30", "Newfoundland and Labrador"));
            zones.Add(new Zone(29, "UTC-01:00", "Azores"));
            zones.Add(new Zone(30, "UTC-02:00", "Mid-Atlantic"));
            zones.Add(new Zone(31, "UTC", "Monrovia"));
            zones.Add(new Zone(32, "UTC-03:00", "Cayenne"));
            zones.Add(new Zone(33, "UTC-04:00", "Georgetown, La PAz, San Juan"));
            zones.Add(new Zone(34, "UTC-05:00", "Indiana (East)"));
            zones.Add(new Zone(35, "UTC-05:00", "Bogota, Lima, Quito"));
            zones.Add(new Zone(36, "UTC-06:00", "Saskatchewan"));
            zones.Add(new Zone(37, "UTC-06:00", "Guadalajara, Mexico City, Monterrey"));
            zones.Add(new Zone(38, "UTC-07:00", "Arizona"));
            zones.Add(new Zone(39, "UTC-12:00", "International Date Line West"));
            zones.Add(new Zone(40, "UTC+12:00", "Fiji Islands, Marshall Islands"));
            zones.Add(new Zone(41, "UTC+11:00", "Madagan, Solomon Islands, New Calendonia"));
            zones.Add(new Zone(42, "UTC+10:00", "Hobart"));
            zones.Add(new Zone(43, "UTC+10:00", "Guam, Port Moresby"));
            zones.Add(new Zone(44, "UTC+09:30", "Darwin"));
            zones.Add(new Zone(45, "UTC+08:00", "Beijing, Chongqing, Hong Kong S.A.R., Urumqi"));
            zones.Add(new Zone(46, "UTC+06:00", "Novosibirsk"));
            zones.Add(new Zone(47, "UTC+05:00", "Tashkent"));
            zones.Add(new Zone(48, "UTC+04:30", "Kabul"));
            zones.Add(new Zone(49, "UTC+02:00", "Cairo"));
            zones.Add(new Zone(50, "UTC+02:00", "Harare, Pretoria"));
            zones.Add(new Zone(51, "UTC+03:00", "Moscow, St. Petersburg, Volgograd"));
            zones.Add(new Zone(53, "UTC-01:00", "Cape Verde Islands"));
            zones.Add(new Zone(54, "UTC+04:00", "Baku"));
            zones.Add(new Zone(55, "UTC-06:00", "Central America"));
            zones.Add(new Zone(56, "UTC+03:00", "Nairobi"));
            zones.Add(new Zone(57, "UTC+01:00", "Sarajevo, Skopje, Warsaw, Zagreb"));
            zones.Add(new Zone(58, "UTC+05:00", "Ekaterinburg"));
            zones.Add(new Zone(59, "UTC+02:00", "Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius"));
            zones.Add(new Zone(60, "UTC-03:00", "Greenland"));
            zones.Add(new Zone(61, "UTC+06:30", "Yangon (Rangoon)"));
            zones.Add(new Zone(62, "UTC+05:45", "Kathmandu"));
            zones.Add(new Zone(63, "UTC+08:00", "Irkutsk"));
            zones.Add(new Zone(64, "UTC+07:00", "Krasnoyarsk"));
            zones.Add(new Zone(65, "UTC-04:00", "Santiago"));
            zones.Add(new Zone(66, "UTC+05:30", "Sri Jayawardenepura"));
            zones.Add(new Zone(67, "UTC+13:00", "Nuku'alofa"));
            zones.Add(new Zone(68, "UTC+10:00", "Vladivostok"));
            zones.Add(new Zone(69, "UTC+01:00", "West Central Africa"));
            zones.Add(new Zone(70, "UTC+09:00", "Yakutsk"));
            zones.Add(new Zone(71, "UTC+06:00", "Astana, Dhaka"));
            zones.Add(new Zone(72, "UTC+09:00", "Seoul"));
            zones.Add(new Zone(73, "UTC+08:00", "Perth"));
            zones.Add(new Zone(74, "UTC+03:00", "Kuwait, Riyadh"));
            zones.Add(new Zone(75, "UTC+08:00", "Taipei"));
            zones.Add(new Zone(76, "UTC+10:00", "Canberra, Melbourne, Sydney"));
            zones.Add(new Zone(77, "UTC-07:00", "Chihuahua, La Paz, Mazatlan"));
            zones.Add(new Zone(78, "UTC-08:00", "Tijuana, Baja Calfornia"));
            zones.Add(new Zone(79, "UTC+02:00", "Amman"));
            zones.Add(new Zone(80, "UTC+02:00", "Beirut"));
            zones.Add(new Zone(81, "UTC-04:00", "Manaus"));
            zones.Add(new Zone(82, "UTC+04:00", "Tbilisi"));
            zones.Add(new Zone(83, "UTC+02:00", "Windhoek"));
            zones.Add(new Zone(84, "UTC+04:00", "Yerevan"));
            zones.Add(new Zone(85, "UTC-03:00", "Buenos Aires"));
            zones.Add(new Zone(86, "UTC", "Casablanca"));
            zones.Add(new Zone(87, "UTC+05:00", "Islamabad, Karachi"));
            zones.Add(new Zone(88, "UTC-04:30", "Caracas"));
            zones.Add(new Zone(89, "UTC+04:00", "Port Louis"));
            zones.Add(new Zone(90, "UTC-03:00", "Montevideo"));
            zones.Add(new Zone(91, "UTC-04:00", "Asuncion"));
            zones.Add(new Zone(92, "UTC+12:00", "Petropavlovsk-Kachatsky"));
            zones.Add(new Zone(93, "UTC", "Coordinated Universal Time"));
            zones.Add(new Zone(94, "UTC-08:00", "Ulaanbaatar"));

            return zones;
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
