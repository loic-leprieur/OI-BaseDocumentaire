using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     Modèle d'un référentiel qui est unique et 
    ///     possède une liste de documents.
    /// </summary>
    public sealed class Referentiel
    {
        /// <summary>
        ///     Identifiant unique auto-incrémenté
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Nom d'un référentiel 
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        ///     Liste de tous les Documents d'un référentiel en fonction
        ///     des clés étrangère id_ref d'un document
        /// </summary>
        public List<Document> Documents { get; set; }
    }
}