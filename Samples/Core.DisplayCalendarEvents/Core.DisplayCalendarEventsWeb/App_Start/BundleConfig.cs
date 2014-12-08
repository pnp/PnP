using System.Web;
using System.Web.Optimization;

namespace Core.DisplayCalendarEventsWeb {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Scripts/fullcalendar/dist/fullcalendar.css",
                      "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/js")
                .Include(
                    "~/Scripts/JXON.js",
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/deparam.js",
                    "~/Scripts/ramda.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/moment-recur.js",
                    "~/Scripts/moment-range.js",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/momentf.js",
                    "~/Scripts/angular.js",
                    "~/Scripts/fullcalendar/dist/fullcalendar.js",
                    "~/Scripts/calendar.js",
                    "~/Scripts/SP.Calendar.js",
                    "~/Scripts/angularbootstrap/ui-bootstrap-tpls-0.12.0.js"
                )
                .IncludeDirectory("~/App/common", "*.js", true)
                .IncludeDirectory("~/App/index", "*.js", true)
                .Include("~/App/app.js")
            );
        }
    }
}
