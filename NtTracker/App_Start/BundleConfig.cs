using System.Web.Optimization;

namespace NtTracker
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //jQuery scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //jQuery validation scripts
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //Modernizr scripts
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //Bootstrap scripts
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //Bootstrap datetimepicker
            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/bootstrap-datetimepicker.js"));

            //IE8 script compatibility
            bundles.Add(new ScriptBundle("~/bundles/comp").Include(
                      "~/Scripts/es5-shim.js"));

            //Bootstrap styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //IE8 style compatibility
            bundles.Add(new StyleBundle("~/Content/comp").Include(
                      "~/Content/ie8.css"));

            //Bootstrap datetimepicker style
            bundles.Add(new StyleBundle("~/Content/datetimepicker").Include(
                      "~/Content/bootstrap-datetimepicker.css"));

            //Paged list style
            bundles.Add(new StyleBundle("~/Content/listStyle").Include(
                      "~/Content/PagedList.css"));
        }
    }
}
