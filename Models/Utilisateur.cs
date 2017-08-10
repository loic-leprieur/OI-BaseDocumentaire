using System.ComponentModel.DataAnnotations;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     Modèle d'un Utilisateur.
    ///     Seul un unique Utilisateur existe 'Admin'
    ///     qui peut en se connectant modifier des documents,
    ///     actualiser la liste des documents ou les référentiels / secteurs disponibles
    /// </summary>
    public class Utilisateur
    {
        /// <summary>
        ///     Constructeur d'un Utilisateur.
        /// </summary>
        public Utilisateur()
        {
        
        }

        /// <summary>
        ///     Identifiant unique d'un Utilisateur.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Nom d'utilisateur qui sert à l'authentification.
        /// </summary>
        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        /// <summary>
        ///     Mot de passe hashé (crypté) sauvegardé dans la base de données
        ///     pour un utilisateur.
        /// </summary>
        [Required]
        [Display (Name = @"Mot de Passe")]
        public string MotDePasse { get; set; }
    }
}