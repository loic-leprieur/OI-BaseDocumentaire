using System.Collections.Generic;
using BaseDoc_OI_GRC.Models;
namespace BaseDoc_OI_GRC.ViewModels
{
    /// <summary>
    ///     ViewModèle donnant les paramètres de la base de données
    ///     comme le répertoire utilisé ou les référentiels et les secteurs existant.
    ///     Il sera utilisé dans la Vue Configuration et dans la Modification d'un document.
    /// </summary>
    public class ConfigurationViewModel
    {
        /// <summary>
        ///     Chaîne du chemin réseau ou local du répertoire des fichiers.
        /// </summary>
        public string NouveauRepertoire { get; set; }

        /// <summary>
        ///     Liste des Référentiels disponibles dans la base.
        /// </summary>
        public List<Referentiel> RefFormReferentiels { get; private set; }
        /// <summary>
        ///     Instance de Referentiel lors de l'ajout 
        /// </summary>
        public Referentiel RefFormNouveauReferentiel { get; set; }

        /// <summary>
        ///     Liste des Secteurs disponibles dans la base.
        /// </summary>
        public List<Secteur> SectFormSecteurs { get; private set; }
        /// <summary>
        ///     Instance de Secteur lors de l'ajout 
        /// </summary>
        public Secteur SectFormNouveauSecteur { get; set; }

        /// <summary>
        ///     Constructeur par défaut vide d'un ConfigurationViewModel.
        /// </summary>
        public ConfigurationViewModel()
        {
            var dal = Dal.ObtenirDal;
            NouveauRepertoire = dal.RepertoireRacine;
            RefFormReferentiels = dal.ObtientTousLesReferentiels();
            SectFormSecteurs = dal.ObtientTousLesSecteurs();
        }

        /// <summary>
        ///     Constructeur d'un ConfigurationViewModel avec Dal identifiée.
        /// </summary>
        /// <param name="d">Instance de la Dal</param>
        public ConfigurationViewModel(Dal d)
        {
            var dal = d;
            NouveauRepertoire = dal.RepertoireRacine;
            RefFormReferentiels = dal.ObtientTousLesReferentiels();
            SectFormSecteurs = dal.ObtientTousLesSecteurs();
        }
    }
}