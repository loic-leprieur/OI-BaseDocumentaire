using System.Data.Entity;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     Contexte de données qui recense les enregistrements de 
    ///     la base de données par modèle en ensembles d'objet de classe.
    /// </summary>
    public class BddContext : DbContext
    {
        /// <summary>
        ///     Ensemble des Référentiels de la table Referentiel
        /// </summary>
        public DbSet<Referentiel> Referentiels { get; set; }

        /// <summary>
        ///     Ensemble des Documents de la table Document
        /// </summary>
        public DbSet<Document> Documents { get; set; }
        /// <summary>
        ///     Ensemble des Utilisateurs de la table Utilisateur
        /// </summary>
        public DbSet<Utilisateur> Utilisateurs  { get; set; }
        /// <summary>
        ///     Ensemble des Secteurs de la table Secteur
        /// </summary>
        public DbSet<Secteur> Secteurs { get; set; }
    }
}