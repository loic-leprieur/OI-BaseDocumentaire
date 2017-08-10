using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseDoc_OI_GRC.Properties;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     Modèle de Document.
    ///     Les fichiers sont stockés dans un répertoire, le nom du fichier
    ///     est analysé dans la DAL pour affecter son titre, son code et son type.
    ///     Le référentiel et le secteur ne sont ajoutés qu'à la modification sur le site
    ///     de la base documentaire.
    ///     
    /// </summary>
    public sealed class Document
    {
        /// <summary>
        ///     Constructeur d'un Document dont l'Id est connu
        /// </summary>
        /// <param name="idSec">Identifiant d'un document existant dans la base de données</param>
        public Document(int idSec)
        {
            IdSec = idSec;
        }

        /// <summary>
        ///     Constructeur vide par défaut d'un Document
        /// </summary>
        public Document()
        {
            
        }

        /// <summary>
        ///     Identifiant numérique unique auto-incrémenté
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Code usine utilisé dans les sites de O-I Manufacturing.
        ///     En France, il est formé comme ceci : 'FR05 IT 001'
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Titre ou alias d'un Document.
        ///     Lorsqu'un document est enregistré en pdf, il faut que le nom du fichier
        ///     contienne au moins son code et de préférence le même titre.
        /// </summary>
        [RegularExpression(@"[^|?|\\|/|?|.|||:|<|>]*$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Document_Titre_Le_titre_du_document_ne_peut_contenir_les_caractères_______________________")]
        public string Titre { get; set; }

        /// <summary>
        ///     Nom complet du fichier pour un document.
        ///     Il comprend son extension.
        /// </summary>
        public string NomFichier { get; set; }

        /// <summary>
        ///     Description de la fonction d'un document.
        ///     Il est déterminé automatiquement selon le code grâce
        ///     aux deux lettres après le numéro de l'usine.
        ///     
        ///     Les Types sont recensés dans un Enum avec leur alias
        ///     et leur nom. Ex: IT = Intruction de Travail.
        ///     L'Enum se trouve dans l'espace de noms 'ViewModel'
        ///     dans la classe 'DocFormViewModel'.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Clé étrangère du référentiel auquel le document est affecté.
        /// </summary>
        public int IdRef { get; set; }

        /// <summary>
        ///     Objet Referentiel correspondant à la clé étrangère du référentiel.
        /// </summary>
        [ForeignKey("IdRef")]
        public Referentiel Referentiel { get; set; }

        /// <summary>
        ///     Clé étrangère du secteur auquel le document est affecté.
        /// </summary>
        public int IdSec { get; private set; }

        /// <summary>
        ///     Objet Secteur correspondant à la clé étrangère du secteur.
        /// </summary>
        [ForeignKey("IdSec")]
        public Secteur Secteur { get; set; }
    }
}