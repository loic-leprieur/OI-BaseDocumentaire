using BaseDoc_OI_GRC.Models;
using BaseDoc_OI_GRC.ViewModels;
using System.Web.Mvc;
using System.Web.Security;
using BaseDoc_OI_GRC.Properties;

namespace BaseDoc_OI_GRC.Controllers
{
    /// <summary>
    /// Contrôleur des actions permises par l'administrateur.
    /// Seul l'administrateur sera capable d'appeler ces méthodes par leur URL,
    /// et obtiendra des vues avec un DOM différent. (Nav, boutons supplémentaires...)
    /// </summary>
    public class AdminController : Controller
    {
        private Dal _dal;

        /// <summary>
        /// Constructeur d'un contrôleur avec l'instance de la Dal courante
        /// </summary>
        public AdminController() : this(Dal.ObtenirDal)
        {
        }

        /// <summary>
        /// Constructeur d'un contrôleur avec une instance donnée de la Dal
        /// </summary>
        /// <param name="dalIoc">Instance de la Dal</param>
        public AdminController(IDal dalIoc)
        {
            _dal = (Dal) dalIoc;
        }

        /// <summary>
        /// Méthode par défaut.
        /// L'utilisateur non connecté obtiendra un formulaire de connection.
        /// Tandis que l'utilisateur connecté comme administrateur obtiendra une déconnexion.
        /// 
        /// Pour vérifier que l'utilisateur est connecté ou non dans l'application,
        /// on vérifie que le cookie de session crée lorsqu'il se connecte est valide.
        /// S'il est expiré ou absent alors l'utilisateur n'est pas connecté.
        /// </summary>
        /// <returns>Page de connexion</returns>
        public ActionResult Index()
        {
            UtilisateurViewModel viewModel = new UtilisateurViewModel { Authentifie = HttpContext.User.Identity.IsAuthenticated };
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                viewModel.Utilisateur = _dal.ObtenirUtilisateur(HttpContext.User.Identity.Name);
            }
            return View(viewModel);
        }

        /// <summary>
        /// Méthode POST de Index.
        /// Envoie les données de connexion au serveur pour vérifier l'identité de l'utilisateur.
        /// S'il échoue, la vue affichera une erreur, sinon un cookie de session sera crée et l'utilisateur
        /// sera redirigé vers la vue Index du contrôleur Principal.
        /// </summary>
        /// <param name="viewModel">Modèle simplifié de l'utilisateur</param>
        /// <param name="returnUrl">Url du demandeur de connexion (vérification que la requête appartient au réseau local)</param>
        /// <returns>Page principale avec ou sans Cookie de session</returns>
        [HttpPost]
        public ActionResult Index(UtilisateurViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Utilisateur utilisateur = _dal.Authentifier(viewModel.Utilisateur.Nom, viewModel.Utilisateur.MotDePasse);
                if (utilisateur != null)
                {
                    FormsAuthentication.SetAuthCookie(utilisateur.Id.ToString(), false);
                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "Principal", null);
                }
                ModelState.AddModelError("Utilisateur.Nom", Resources.AdminController_Index_Nom_et_ou_mot_de_passe_incorrect_s_);
            }
            return View(viewModel);
        }

        /// <summary>
        /// Retourne la vue de création d'un compte Utilisateur
        /// A SUPPRIMER POUR LE DEPLOIEMENT FINAL !
        /// </summary>
        /// <returns>Page de création d'un compte</returns>
        public ActionResult CreerCompte()
        {
            return View();
        }

        /// <summary>
        /// Retourne la vue Index du contrôleur Principal avec un cookie de session.
        /// A SUPPRIMER POUR LE DEPLOIEMENT FINAL !
        /// </summary>
        /// <param name="utilisateur">Instance de Utilisateur</param>
        /// <returns>Vue CreerCompte avec le modèle Utilisateur</returns>
        [HttpPost]
        public ActionResult CreerCompte(Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                int id = _dal.AjouterUtilisateur(utilisateur.Nom, utilisateur.MotDePasse);
                FormsAuthentication.SetAuthCookie(id.ToString(), false);
                return RedirectToAction("Index", "Principal", null);
            }
            return View(utilisateur);
        }

        /// <summary>
        /// Supprimer le cookie de session, déconnectant l'administrateur
        /// et le redirigeant vers la page Index normal du contrôleur Principal. 
        /// </summary>
        /// <returns>Page d'accueil</returns>
        public ActionResult Deconnexion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Accueil", null);
        }

        /// <summary>
        /// Retourne la vue Configuration
        /// permettant l'ajout et la suppression de référentiels / secteurs.
        /// </summary>
        /// <returns>Page de configuration</returns>
        public ActionResult Configuration()
        {
            return View(new ConfigurationViewModel());
        }

        /// <summary>
        ///     Ajoute une nouvelle instance de Secteur dans la base de données.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Page de configuration avec le nouveau secteur</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjouterSecteur(ConfigurationViewModel model)
        {
            _dal.CreerSecteur(model.SectFormNouveauSecteur.Nom);
            return RedirectToAction("Configuration");
        }

        /// <summary>
        ///     Ajoute une nouvelle instance de Référentiel dans la base de données.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Page de configuration avec le nouveau référentiel</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjouterReferentiel(ConfigurationViewModel model)
        {
            _dal.CreerReferentiel(model.RefFormNouveauReferentiel.Nom);
            return RedirectToAction("Configuration");
        }

        /// <summary>
        ///     Modifier dans la Dal le chemin vers le répertoire des fichiers
        ///     et écris dans le fichier le nouveau chemin.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangerRepertoire(ConfigurationViewModel model)
        {
            _dal.ModifierRepertoire(model.NouveauRepertoire);
            return RedirectToAction("Configuration");
        }

        /// <summary>
        ///     Retourne la page de renommage d'un secteur
        /// </summary>
        /// <param name="idSecteur"></param>
        /// <returns>Renommage d'un secteur</returns>
        public ActionResult RenommerSecteur(int idSecteur)
        {
            Secteur s = Dal.Bdd.Secteurs.Find(idSecteur);

            SecteurRenommageViewModel vm = new SecteurRenommageViewModel
            {
                IdSecteur = idSecteur,
                NomSecteur = s.Nom
            };
            return View(vm);
        }

        /// <summary>
        ///     Retourne la page de renommage d'un référentiel.
        /// </summary>
        /// <param name="idReferentiel"></param>
        /// <returns>Page de renommage d'un référentiel</returns>
        public ActionResult RenommerReferentiel(int idReferentiel)
        {
            Referentiel r = Dal.Bdd.Referentiels.Find(idReferentiel);

            ReferentielRenommageViewModel vm = new ReferentielRenommageViewModel
            {
                IdReferentiel = idReferentiel,
                NomReferentiel = r.Nom
            };
            return View(vm);
        }

        /// <summary>
        ///     Met à jour le secteur avec son nouveau nom
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Page de configuration avec secteur renommé</returns>
        [HttpPost]
        public ActionResult RenommerSecteur(SecteurRenommageViewModel model)
        {
            Dal.Bdd.Secteurs.Find(model.IdSecteur).Nom = model.NomSecteur;
            Dal.Bdd.SaveChanges();
            return RedirectToAction("Configuration");
        }

        /// <summary>
        ///      Met à jour le référentiel avec son nouveau nom
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Page de configuration avec secteur renommé</returns>
        [HttpPost]
        public ActionResult RenommerReferentiel(ReferentielRenommageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Dal.Bdd.Referentiels.Find(model.IdReferentiel).Nom = model.NomReferentiel;
                Dal.Bdd.SaveChanges();
                return RedirectToAction("Configuration");
            }
            return RedirectToAction("RenommerReferentiel");
        }

        /// <summary>
        ///     Retire de la base le référentiel sélectionné
        /// </summary>
        /// <param name="idReferentiel">Identifiant du référentiel</param>
        /// <returns>Page configuration à jour</returns>
        public ActionResult SupprimerReferentiel(int idReferentiel)
        {
            Dal.Bdd.Referentiels.Remove(Dal.Bdd.Referentiels.Find(idReferentiel));
            Dal.Bdd.SaveChanges();
            return RedirectToAction("Configuration");
        }

        /// <summary>
        ///     Retire de la base le secteur sélectionné
        /// </summary>
        /// <param name="idSecteur">Identifiant du secteur</param>
        /// <returns>Page configuration à jour</returns>
        public ActionResult SupprimerSecteur(int idSecteur)
        {
            Dal.Bdd.Secteurs.Remove(Dal.Bdd.Secteurs.Find(idSecteur));
            Dal.Bdd.SaveChanges();
            return RedirectToAction("Configuration");
        }
    }
}
