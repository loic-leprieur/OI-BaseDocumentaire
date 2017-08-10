using BaseDoc_OI_GRC.Models;
using BaseDoc_OI_GRC.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace BaseDoc_OI_GRC.Controllers
{
    /// <summary>
    ///   Classe chargée de l'appel aux vues (html) à partir des 
    ///   éléments du modèle(Documents, Référentiels) récupérés
    ///   avec une Dal(interagit avec la base de données) en attribut
    /// </summary>
    public class PrincipalController : Controller
    {
        /// Attribut de la Dal utilisée pour interagir avec la base données
        private Dal _dal;

        /// <summary>
        /// Constructeur d'un contrôleur avec la Dal courante
        /// </summary>
        public PrincipalController()
        {
            _dal = Dal.ObtenirDal;
        }

        /// <summary>
        /// Constructeur d'un contrôleur dont la Dal est donnée en paramètre
        /// </summary>
        /// <param name="dalIoc">Instance de la Dal </param>
        public PrincipalController(Dal dalIoc)
        {
            _dal = dalIoc; 
        }

        /// <summary>
        /// Action par défaut de la page Principal.
        /// Les fichiers de la base sont recensés dans une liste qu'on affiche dans la vue Index.
        /// 
        /// Si un paramètre 'referentiel' est présent dans l'URL : /Principal/Index?referentiel=2,
        /// alors on n'affiche que les documents appartenant à ce référentiel
        ///
        /// Si un paramètre 'idSecteur' est présent dans l'URL : /Principal/Index?idSecteur=2
        /// alors seuls les documents de ce secteur seront affichés.
        /// </summary>
        /// <param name="idReferentiel">Si non null, Id du référentiel</param>
        /// <param name="idSecteur">Si non null, Id du secteur</param>
        /// <param name="strMotsCles">Si a une valeur, mots saisis dans la page d'accueil pour filtrer les documents avec un code ou un titre similaire</param>        
        /// <returns>Vue Index avec le modèle correspondant aux paramètres données (chaque paramètre est exclusif)</returns>
        public ActionResult Index(int? idReferentiel, int? idSecteur, string strMotsCles)
        {
            List<Document> listeDocuments;

            if(idReferentiel.HasValue)
            {
                listeDocuments = _dal.ObtientTousLesDocumentsDeReferentiel(idReferentiel);
            }
            else if(idSecteur.HasValue)
            {
                listeDocuments = _dal.ObtientTousLesDocumentsDeSecteur(idSecteur);
            }
            else if(!string.IsNullOrWhiteSpace(strMotsCles))
            {
                _dal.RecupereDocumentsBdd();
                listeDocuments = _dal.RechercheDocumentsParMotsCles(strMotsCles);
            }
            else
            {
                listeDocuments = _dal.Documents;
            }

            if(Request.IsAjaxRequest())
            {
                return PartialView("TableBodyDocuments", listeDocuments);
            }

            return View(listeDocuments);
        }

        /// <summary>
        /// L'administrateur peut mettre à jour la liste des documents pour ajouter les plus récents
        /// </summary>
        /// <returns>Rafraichit la page Principal de la table avec les nouveaux documents ajoutés et les anciens retirés (les documents n'existant plus dans le répertoire ou dont le nom de fichier a été renommé</returns>
        [Authorize]
        public ActionResult ActualiserDocuments()
        {
            _dal.RetirerDeBddLesFichiersSupprimes();
            _dal.RecupereDocumentsBdd();
            _dal.CreerNouveauxFichiersDuRepertoire();
            _dal.RecupereDocumentsBdd();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Effectue une mise à jour du référentiel et du secteur de tous les documents
        /// donnés en paramètre à partir de leur Id.
        /// Cette méthode GET est appelée lorsque le bouton "Modifier" est appuyé
        /// et qu'une requête AJAX renvoie les Ids des documents sélectionnés par leur case à cocher.
        /// </summary>
        /// <param name="docsToModify">Tableau d'entiers correspondant aux Id des documents dont la case est cochée</param>
        /// <returns>Affiche un modal (fenêtre de dialogue de Bootstrap qui prend le focus) avec un formulaire</returns>
        [Authorize]
        [HttpGet]
        public ActionResult ModifierDocuments(int[] docsToModify)
        {
            var viewModel = new DocsSelectionnesViewModel {DocumentsId = docsToModify};
            return PartialView("FenetreDialogueModif", viewModel);
        }

        /// <summary>
        /// Enregistrement des modifications demandées par l'utilisateur lors de l'envoi du formulaire.
        /// Cette méthode POST va mettre à jour chaque document sélectionné préalablement avec le référentiel
        /// et le secteur sélectionnés.  
        /// </summary>
        /// <param name="viewmodel">Modèle contenant le référentiel et le secteur sélectionnés et les documents à modifier</param>
        /// <returns>Redirige vers la vue Index du Controleur Principal</returns>
        [Authorize]
        [HttpPost]
        public ActionResult ModifierDocuments(DocsSelectionnesViewModel viewmodel)
        {
            foreach(int idDoc in viewmodel.DocumentsId)
            {
                Document docTrouve = _dal.ObtenirDocumentParId(idDoc);
                _dal.AjouterDocumentAReferentiel(docTrouve.Id, viewmodel.SelectedReferentielId);
                _dal.AjouterDocumentASecteur(docTrouve.Id, viewmodel.SelectedSecteurId);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Méthode asynchrone de suppression de documents sélectionnés (case cochée).
        /// Ces documents sélectionnés proviennent d'un tableau JavaScript qui a été envoyé par une requête AJAX.
        /// </summary>
        /// <param name="docsToDelete"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult SupprimerDocuments(int[] docsToDelete)
        {
            try
            {
                foreach(int docId in docsToDelete)
                {
                    _dal.SupprimerDocument(_dal.ObtenirDocumentParId(docId).NomFichier);
                }

                return Json("success");
            }
            catch
            {
                return Json("error");
            }
        }

        /// <summary>
        /// Parcours tous les fichiers stockés dans le répertoire correspondant au chemin de la constante DIRECTORY_FICHIERS,
        /// si l'un des fichiers possède le même nom que le paramètre filename, 
        /// alors il est téléchargé par le navigateur web,
        /// sinon on retourne sur la vue de la page Principal.
        /// </summary>
        /// <param name="filename">Nom du fichier avec son extension</param>
        /// <returns>Fichier PDF ou Vue Index si inexistant</returns>
        public ActionResult TelechargerDocument(string filename)
        {
            DirectoryInfo dir = new DirectoryInfo(_dal.RepertoireRacine);
            FileInfo[] fichiers = dir.GetFiles();

            foreach(FileInfo fichier in fichiers)
            {
                if(string.Compare(fichier.Name, filename, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var documentPath = _dal.RepertoireRacine + "/" + fichier.Name;
                    return File(documentPath, "application/octet-stream", fichier.Name);
                }
            }
            return RedirectToAction("Index");
        }
    }
}