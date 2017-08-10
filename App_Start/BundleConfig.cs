using System.Web.Optimization;

namespace BaseDoc_OI_GRC
{
    /// <summary>
    /// Emplacement de tous les Scripts JavaScript et de toutes les feuilles de style.
    /// JS : jquery / dataTables / Bootstrap / principalScript (script personnalisé spécifique à l'application)
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Enregistrement des objets Bundles chargés lors du lancement de l'application ASP.NET
        /// </summary>
        /// <param name="bundles">Collection des Bundles configurés dans l'application</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts-js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/modernizr-*",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/dataTables.bootstrap.min.js",
                "~/Scripts/dataTables.fixedHeader.min.js",
                "~/Scripts/dataTables.responsive.min.js",
                "~/Scripts/responsive.boostrap.min.js",
                "~/Scripts/principalScript.js"));

            bundles.Add(new StyleBundle("~/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/dataTables.bootstrap.min.css",
                "~/Content/fixedHeader.bootstrap.min.css",
                "~/Content/responsive.bootstrap.min.css",
                "~/Content/Site.css"));
        }
    }
}