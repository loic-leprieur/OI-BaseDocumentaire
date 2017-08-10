using System.Web.Mvc;

namespace BaseDoc_OI_GRC.Controllers
{
    /// <summary>
    /// Contrôleur de la page d'accueil de l'application.
    /// 
    /// Il définit l'appel à sa vue Index et la redirection
    /// vers la vue Index du contrôleur Principal avec un paramètre 'searchString', 
    /// sa valeur ou son absence définira le filtrage de cette vue.
    /// </summary>
    public class AccueilController : Controller
    {
        /// <summary>
        /// Méthode par défaut générant la vue Index.
        /// Si son paramètre searchString a une valeur,
        /// alors la vue Index du contrôleur principal sera filtré.
        /// </summary>
        /// <param name="searchString">Mots-clés de la recherche (Code ou Titre)</param>
        /// <returns></returns>
        public ActionResult Index(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return View();
            return RedirectToAction("Index", "Principal", new {strMotsCles = searchString});
        }
    }
}
