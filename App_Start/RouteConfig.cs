using System.Web.Mvc;
using System.Web.Routing;

namespace BaseDoc_OI_GRC
{
    /// <summary>
    /// Configure le mécanisme de routage d'URL
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Indique les filtres d'URL autorisées et interdites.
        /// La première partie du slash "/" sera un contrôleur, puis la deuxième sera 
        /// une des méthodes de ce contrôleur et pourra prendre un paramètre en troisième partie.
        /// 
        /// N.B.: Les méthodes de l'application utilises plutôt le caractère '?' pour indiquer les données
        /// à utiliser comme paramètre de la méthode.
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Accueil", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
