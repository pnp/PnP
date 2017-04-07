using System.Web;
using System.Web.Optimization;

namespace MVCTaxonomyPickerWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/spcontext").Include(
                        "~/Scripts/spcontext.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/taxonomypickercontrol.css",
                        "~/Content/site.css",
                        "~/Content/fabric.css",
                        "~/Content/fabric.components.css"));

            bundles.Add(new ScriptBundle("~/bundles/taxpicker").Include(
                      "~/Scripts/taxonomypickercontrol.js",
                      "~/Scripts/taxonomypickercontrol_resources.en.js",
                       "~/Scripts/App.js"));

            bundles.Add(new ScriptBundle("~/bundles/fabric").Include(
                      "~/Scripts/fabric.*"));

        }

    }
}
