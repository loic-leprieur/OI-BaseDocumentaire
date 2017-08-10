using System.Collections.Generic;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     Modèle d'un Secteur.
    ///     Au sein d'un Process les Secteurs peuvent comprendre
    ///     des documents de plusieurs référentiels ou sans référentiel.
    /// </summary>
    public sealed class Secteur
    {
        /// <summary>
        ///     Identifiant numérique unique
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///     Nom décrivant le secteur
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        ///     Liste de Documents affectés au secteur
        /// </summary>
        public List<Document> Documents {get; set;}
    }
}