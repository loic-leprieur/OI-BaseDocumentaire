using BaseDoc_OI_GRC.Models;

namespace BaseDoc_OI_GRC.ViewModels
{
    /// <summary>
    ///     ViewModel des données pour savoir
    ///     si un utilisateur donné est authentifié ou non.
    /// </summary>
    public class UtilisateurViewModel
    {
        /// <summary>
        ///     Instance de l'utilisateur pour la connexion.
        /// </summary>
        public Utilisateur Utilisateur { get; set; }
        /// <summary>
        ///     L'utilisateur est authentifié ou non,
        ///     pour créer le cookie de session.
        /// </summary>
        public bool Authentifie { get; set; }
    }
}