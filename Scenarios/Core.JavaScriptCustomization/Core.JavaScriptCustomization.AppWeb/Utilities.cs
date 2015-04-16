using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Core.JavaScriptCustomization.AppWeb
{
    public static class Utilities
    {
        public const string Scenario1Key = "scenario1";
        public const string Scenario2Key = "scenario2";
        public const string Scenario3Key = "scenario3";

        public static string BuildScenarioJavaScriptUrl(string scenarioName, HttpRequest request)
        {
            string scenarioUrl = String.Format("{0}://{1}:{2}/Scripts", request.Url.Scheme, request.Url.DnsSafeHost, request.Url.Port);
            string revision = Guid.NewGuid().ToString().Replace("-", "");

            if (scenarioName.Equals(Scenario1Key, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Format("{0}/{1}?rev={2}", scenarioUrl, "scenario1.js", revision);
            }
            else if (scenarioName.Equals(Scenario2Key, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Format("{0}/{1}?rev={2}", scenarioUrl, "scenario2.js", revision);
            }
            else if (scenarioName.Equals(Scenario3Key, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Format("{0}/{1}?rev={2}", scenarioUrl, "scenario3.js", revision);
            }
            else
            {
                return "";
            }
        }

    
    }
}