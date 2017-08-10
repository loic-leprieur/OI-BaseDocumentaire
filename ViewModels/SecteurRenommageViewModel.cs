namespace BaseDoc_OI_GRC.ViewModels
{
    /// <summary>
    ///     ViewModel pour renommer un secteur.
    /// </summary>
    public class SecteurRenommageViewModel
    {
        /// <summary>
        ///     Identifiant numérique du secteur.
        /// </summary>
        public int IdSecteur { get; set; }
        /// <summary>
        ///     Nom du secteur à remplacer.
        /// </summary>
        public string NomSecteur { get; set; }
    }
}