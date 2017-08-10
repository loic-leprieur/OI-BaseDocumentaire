using System.Web.Mvc;
using BaseDoc_OI_GRC.Models;
using BaseDoc_OI_GRC.ViewModels;

namespace BaseDoc_OI_GRC.Controllers
{
    /// <summary>
    ///     Controleur des documents 
    /// </summary>
    [Authorize]
    public class DocumentsController : Controller
    {
        private IDal _dal;
        /// <summary>
        ///     Constructeur par défaut avec récupération automatique de la Dal
        /// </summary>
        public DocumentsController() : this(Dal.ObtenirDal)
        {
        }

        /// <summary>
        ///     Constructeur avec une Dal donnée en paramètre.
        /// </summary>
        /// <param name="dalIoc">Instance de la Dal</param>
        public DocumentsController(IDal dalIoc)
        {
            _dal = dalIoc;
        }

    /*
        /// <summary>
        ///     Méthode par défaut pour l'URL : /BaseDoc/Documents
        ///     Donne la liste des documents dans l'application
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(_dal.Documents);
        }
    */

        // GET: Documents/Modifier?id=5
        /// <summary>
        ///     Affiche la page de Modification 
        ///     d'un document en fonction de son Id. 
        ///     Un formulaire recense les propriétés éditables
        ///     d'un document.
        /// </summary>
        /// <param name="id">Identifiant numérique d'un document</param>
        /// <returns>Page Modifier avec formulaire de modification d'un document</returns>
        [HttpGet]
        public ActionResult Modifier(int id)
        {
            Document docAmodifier = _dal.ObtenirDocumentParId(id);

            DocFormViewModel vm = new DocFormViewModel()
            {
                Doc = docAmodifier,
                DocCode = docAmodifier.Code,
                Referentiels = _dal.ObtientTousLesReferentiels(),
                SelectedReferentielId = docAmodifier.IdRef,
                SelectedTypeString = docAmodifier.Type,
                Secteurs = _dal.ObtientTousLesSecteurs(),
                SelectedSecteurId = docAmodifier.IdSec
            };

            return View(vm);
        }

        // POST: Documents/Modifier
        /// <summary>
        ///     Envoie les données du formulaire de modification.
        ///     Si les données sont valide alors le Document modifié est enregistré dans la base
        ///     et la vue Principal est redirigée.
        /// </summary>
        /// <param name="vm">ViewModel avec les données à mettre à jour</param>
        /// <returns>Page principale avec les données à jour</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifier(DocFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _dal.ModifierDocument(vm.Doc.Id, vm.DocCode, vm.Doc.Titre, vm.SelectedTypeString);
                _dal.AjouterDocumentAReferentiel(vm.Doc.Id, vm.SelectedReferentielId);
                _dal.AjouterDocumentASecteur(vm.Doc.Id, vm.SelectedSecteurId);
                return RedirectToAction("Index", "Principal");
            }

            return View(vm);
        }
    }
}
