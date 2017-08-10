using System.Collections.Generic;
using BaseDoc_OI_GRC.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BaseDoc_OI_GRC.ViewModels
{
    /// <summary>
    ///     ViewModel qui recense les champs utilisés dans la fenêtre de sélection multiple des documents 
    ///     et dans la modification d'un document
    ///
    ///     La liste des référentiels, des secteurs et le référentiel / secteur rentré en paramètre
    ///     sont nécessaires pour peupler les dropdownlists et obtenir l'item sélectionné.
    /// </summary>
    public class DocsSelectionnesViewModel
    {
        /// <summary>
        /// instance unique de la couche d'accès aux données
        /// </summary>
        private readonly IDal _dal = Dal.ObtenirDal;

        /// <summary>
        /// ensemble des IDs de chaque document sélectionné, récupéré depuis une requête AJAX 
        public int[] DocumentsId { get; set; }

        /// <summary>
        /// Liste des Référentiels disponibles dans la base
        /// </summary>
        private List<Referentiel> Referentiels { get; }

        /// <summary>
        /// Liste des Secteurs disponibles dans la base
        /// </summary>
        private List<Secteur> Secteurs { get; }

        /// <summary>
        /// Id du référentiel sélectionné ou par défaut
        /// </summary>
        [Display(Name = @"Référentiel")]        
        public int SelectedReferentielId { get; set; }

        /// <summary>
        /// Id du secteur sélectionné ou par défaut
        /// </summary>
        [Display(Name = @"Secteur")]
        public int SelectedSecteurId { get; set; }

        /// <summary>
        /// Instantie un DocsSelectionnes avec tous les référentiels et tous les secteurs de la base
        /// </summary>
        public DocsSelectionnesViewModel()
        {
            Referentiels = _dal.ObtientTousLesReferentiels();
            Secteurs = _dal.ObtientTousLesSecteurs();
        }

        /// <summary>
        /// Collection de référentiels sélectionnables dans une liste déroulante
        /// </summary>
        public IEnumerable<SelectListItem> ReferentielItems => new SelectList(Referentiels, "Id", "Nom");

        /// <summary>
        /// Collection de référentiels sélectionnables dans une liste déroulante
        /// </summary>
        public IEnumerable<SelectListItem> SecteurItems => new SelectList(Secteurs, "Id", "Nom");
    }
}