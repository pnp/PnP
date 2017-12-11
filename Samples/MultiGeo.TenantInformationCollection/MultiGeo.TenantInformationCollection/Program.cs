using System;

namespace GeoTenantInformationCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Check the readme file to learn how to register an application in azure ad and replace these values
                MultiGeoManager multiGeoManager = new MultiGeoManager("<application id>", "<application password>", "<Azure AD domain>");
                var geos = multiGeoManager.GetTenantGeoLocations();
                foreach (var geo in geos)
                {
                    Console.WriteLine($"{geo.GeoLocation} - {geo.RootSiteUrl} - {geo.TenantAdminUrl}");
                }

                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Multi-geo exception: ${ex.ToString()}");
            }

        }
    }
}
