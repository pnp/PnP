using System.Web;
using System.Web.Optimization;

namespace Patterns.Provisioning.UIWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/jquery-ui-1.11.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/site.css"));//,
                      //"~/Content/bootstrap-responsive.css"));

            bundles.Add(new ScriptBundle("~/bundles/wizard").Include(
                      "~/Scripts/knockout-3.0.0.js",
                      "~/Scripts/jquery.steps.js",
                      "~/Content/js/wizard.js"));

            bundles.Add(new StyleBundle("~/bundles/wizardcss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/wizard.css"));
        }
    }
}
