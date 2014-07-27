using OfficeAMS.Core.Entities;
using OfficeAMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YammerJSONTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Simple tester console to ensure that token works as expected. Details on getting token from here - https://developer.yammer.com/authentication
            string accessToken = "GetYourOwnAccessTokenFromYammer";
            YammerGroup group = YammerUtility.GetYammerGroupByName("fuu", accessToken);
        }

    }
}
