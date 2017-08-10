using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using BaseDoc_OI_GRC.Models;

namespace BaseDoc_OI_GRC.ViewModels
{
    /// <summary>
    ///     ViewModel pour modifier individuellement un document,
    ///     les types sont définis, la liste des référentiels et des secteurs.
    /// </summary>
    public class DocFormViewModel
    {
        /**
         * Enumération des types applicables à un document.
         * Pour ajouter un nouveau type, il faut lui donner une valeur en un seul mot
         * sur une nouvelle ligne (précédée d'une virgule).
         *
         * Si la valeur correspond à plusieurs mots ou utilise un apostrophe,
         * rajoutez l'attribut : [Description("Valeur de l'élément à afficher")]
         */
        /// <summary>
        ///     Enumération des Types de document.
        ///     Ils décrivent la fonction d'un document, et ils 
        ///     sont identifié par deux lettres dans le code d'un document.
        /// </summary>
        public enum Types
        {
            /// <summary>
            ///     Type par défaut des documents sans type ou sans code ou dont le 
            ///     code n'est pas formatté correctement ou d'un type inconnu.
            /// </summary>
            Indéfini,
            /// <summary>
            ///     Bonne pratique   
            /// </summary>
            [Description("Bonne pratique")]
            BP,
            /// <summary>
            ///     Définitions de Fonction
            /// </summary>
            [Description("Définitions de Fonction")]
            DF,
            /// <summary>
            ///     Document d'organisation
            /// </summary>
            [Description("Document d'organisation")]
            DO,
            /// <summary>
            ///     Enregistrement
            /// </summary>
            [Description("Enregistrement")]
            EN,
            /// <summary>
            ///     Instructions de travail
            /// </summary>
            [Description("Instructions de travail")]
            IT,
            /// <summary>
            ///     Produire
            /// </summary>
            [Description("Produire")]
            PR,
            /// <summary>
            ///     Mode opératoire
            /// </summary>
            [Description("Mode opératoire")]
            MO,
            /// <summary>
            ///     Consignes de sécurité
            /// </summary>
            [Description("Consignes de sécurité")]
            CS,
            /// <summary>
            ///     Spécification
            /// </summary>
            [Description("Spécification")]
            SP
        }

        /// <summary>
        ///     Liste des référentiels existant dans la base.
        /// </summary>
        public List<Referentiel> Referentiels { private get; set; }

        /// <summary>
        ///     Liste des Secteurs existant dans la base.
        /// </summary>
        public List<Secteur> Secteurs { private get; set; }

        /// <summary>
        ///     Instance de Document à modifier.
        /// </summary>
        public Document Doc { get; set; }

        /// <summary>
        ///     Nouveau Code du document.
        /// </summary>
        public string DocCode { get; set; }

        /// <summary>
        ///     Type sélectionné dans la liste déroulante.
        /// </summary>
        public string SelectedTypeString { get; set; }

        /// <summary>
        ///     Id du référentiel sélectionné dans la liste déroulante.
        /// </summary>
        [Display(Name = "Référentiel")]
        public int SelectedReferentielId { get; set; }

        /// <summary>
        ///     Id du secteur selectionné dans la liste déroulante.
        /// </summary>
        [Display(Name = "Secteur")]
        public int SelectedSecteurId { get; set; }

        /// <summary>
        ///     Liste déroulante dans laquelle chaque item est associé à la liste de référentiels.
        /// </summary>
        public IEnumerable<SelectListItem> ReferentielItems => new SelectList(Referentiels, "Id", "Nom");

        /// <summary>
        ///     Liste déroulante dans laquelle chaque item est associé à la liste de secteurs.
        /// </summary>
        public IEnumerable<SelectListItem> SecteurItems => new SelectList(Secteurs, "Id", "Nom");

        /**
         * Prend un élément d'un Enum et retourne la valeur de 
         * sa description en chaînes de caractère si l'attribut
         * description est défini.
         */
        /// <summary>
        ///     Récupère dans la liste déroulante l'item avec la valeur donnée en paramètre (IT, DO,...)
        ///     et retourne la description complète du type (Instruction de travail, Document d'Organisation)
        /// </summary>
        /// <param name="value">Type en deux lettres du document</param>
        /// <returns>Description du type donné</returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /**
         * Génère une énumérations d'éléments 'Types' dont la valeur textuelle
         * correspond à leur description. La liste générée sera utilisée dans la vue pour la liste
         * de sélection de Type de document lors de la modification. 
         */
        /// <summary>
        ///     Liste déroulante dans laquelle chaque item est associé à un type sélectionnable.
        /// </summary>
        public IEnumerable<SelectListItem> TypeItems
        {          
            get
            {
                return (Enum.GetValues(typeof(Types)).Cast<Types>().Select(
                    enu => new SelectListItem()
                    {
                        Text = GetEnumDescription(enu),
                        Value = GetEnumDescription(enu)
                    })).ToList().OrderBy(sli => sli.Text);
            }
        }
    }
}