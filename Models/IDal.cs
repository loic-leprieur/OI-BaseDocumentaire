using System;
using System.Collections.Generic;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    /// Interface de la Data Access Layer (Dal) donnant la signature des méthodes.
    /// </summary>
    public interface IDal : IDisposable
    {
        /// <summary>
        ///     Liste de Documents (objets de la classe Document) dans la base de données
        /// </summary>
        List<Document> Documents { get; }

        /// <summary>
        ///     Retourne dans une liste les Référentiels existant dans
        ///     la base de données
        /// </summary>
        /// <returns>Liste de Referentiels</returns>
        List<Referentiel> ObtientTousLesReferentiels();
        /// <summary>
        ///     Retourne l'instance du document dont l'Id est donné en paramètre
        /// </summary>
        /// <param name="idDoc">Id du Document</param>
        /// <returns>Instance de Document</returns>
        Document ObtenirDocumentParId(int idDoc);

        /// <summary>
        ///     Instantie un nouveau référentiel à partir d'un nom donné.
        ///     Si le nom n'appartient à aucun référentiel existant,
        ///     alors on insère le référentiel dans la table correspondante.
        ///     L'Id du référentiel est incrémenté automatiquement. 
        /// </summary>
        /// <param name="nom">Nom à affecter au nouveau référentiel</param>
        void CreerReferentiel(string nom);
        /// <summary>
        ///     Mise à jour des propriétés d'un document.
        ///     Pour un Id donné, cette méthode
        ///     va modifier le titre, le type ou le code du document.
        /// </summary>
        /// <param name="id">Id unique d'un document</param>
        /// <param name="nouveauCode">Code ISO 14001 ex: 'FR05 AA 001'</param>
        /// <param name="nouveauTitre">Titre décrivant le document</param>
        /// <param name="nouveauType">Type donnant la fonction du document</param>
        void ModifierDocument(int id, string nouveauCode, string nouveauTitre, string nouveauType);

        /// <summary>
        ///     Intègre un document dans un référentiel et lui
        ///     associe son Id comme clé étrangère
        /// </summary>
        /// <param name="idDoc">Id du document</param>
        /// <param name="idRef">Id du référentiel</param>
        void AjouterDocumentAReferentiel(int idDoc, int idRef);

        /// <summary>
        ///     Vérifie si un référentiel désigné par son nom existe
        /// </summary>
        /// <param name="nomRef"></param>
        /// <returns>Existe ou non</returns>
        bool ReferentielExisteBdd(string nomRef);

        /// <summary>
        ///     Obtient une instance du référentiel recherché par son nom.
        /// </summary>
        /// <param name="nomRef">Nom du référentiel</param>
        /// <returns>Instance de Référentiel dont le nom correspond</returns>
        Referentiel ObtenirReferentielParNom(string nomRef);

        /// <summary>
        ///     Retourne une liste de tous les secteurs contenus dans la base de données
        /// </summary>
        /// <returns>Liste de Secteurs</returns>
        List<Secteur> ObtientTousLesSecteurs();

        /// <summary>
        ///     Retourne une instance de Référentiel dont l'Id correspond à l'entier donné en paramètre
        /// </summary>
        /// <param name="idReferentiel">Id - Entier</param>
        /// <returns></returns>
        Referentiel ObtenirReferentielParId(int idReferentiel);

        /// <summary>
        ///     Retourne l'instance de Document dont le nomFichier correspond
        ///     correspond à au nom donné en paramètre
        /// </summary>
        /// <param name="fichierDoc">Nom de fichier du document</param>
        /// <returns></returns>
        Document ObtenirUnDocumentParNomFichier(string fichierDoc);

        /// <summary>
        ///     Crée un nouvel utilisateur avec un nom et un mot de passe.
        ///     Le mot de passe rentré (ex: password1234) est hashé et salé,
        ///     on va utiliser l'algorithme d'encodage SHA512 pour obtenir une version illissible et
        ///     irréversible (fonction non bijective) du mot de passe (ex: password1234 ->
        ///     0d6bcf6cefacc3a209e24b81980a8476d1071...)
        /// </summary>
        /// <param name="nom">Nom d'identifiant</param>
        /// <param name="motDePasse">Mot de passe non hashé</param>
        /// <returns></returns>
        int AjouterUtilisateur(string nom, string motDePasse);

        /// <summary>
        ///     Retourne une instance de Utilisateur dont le nom et le hash du mot de passe
        ///     rentré correspondent.
        ///     On ne compare pas directement les mots de passe en clair (password1234 ? password1234),
        ///     mais on va déterminer si le hash de la base est égal au hash obtenu avec le mot de passe rentré
        ///     (password1234 : "0d6bcf6cefacc3a209e24b81980a8476d1071..." ? "0d6bcf6cefacc3a209e24b81980a8476d1071..." :
        ///     password1234)
        ///     Sauf si le mot de passe est trop évident (trop court, caractère alphanumérique uniquement) comme 1234,
        ///     on ne peut déterminer le mot de passe qu'en testant les 92^16 possibilités.
        /// </summary>
        /// <param name="nom">Nom d'utilisateur</param>
        /// <param name="motDePasse">Mot de passe</param>
        /// <returns>Instance de Utilisateur</returns>
        Utilisateur Authentifier(string nom, string motDePasse);

        /// <summary>
        ///     Retourne l'instance Utilisateur correspond à l'Id en chaîne de caractère rentré en paramètre
        /// </summary>
        /// <param name="idStr">Id - Chaîne</param>
        /// <returns>Instance de Utilisateur</returns>
        Utilisateur ObtenirUtilisateur(string idStr);

        /// <summary>
        ///     Intègre un document dans un secteur par leurs Ids.
        /// </summary>
        /// <param name="idDoc">Id du document</param>
        /// <param name="idSec">Id du secteur</param>
        void AjouterDocumentASecteur(int idDoc, int idSec);

        /// <summary>
        ///     Instantie un nouveau secteur en fonction d'un nom donné
        /// </summary>
        /// <param name="strNom">Nom à affecter au nouveau Secteur</param>
        void CreerSecteur(string strNom);

        /// <summary>
        ///     Modifie la valeur du répertoire en écrivant
        ///     dans le fichier de configuration le nouveau chemin.
        /// </summary>
        /// <param name="nouveauRepertoire"></param>
        void ModifierRepertoire(string nouveauRepertoire);
    }
}
