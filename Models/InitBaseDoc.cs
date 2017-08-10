using System.Collections.Generic;
using System.Data.Entity;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     Initialisation des données de la base avec un seed des référentiels et des secteurs.
    /// </summary>
    public class InitBaseDoc : DropCreateDatabaseAlways<BddContext>
    {
        /// <summary>
        /// Initialise les données de la base.
        /// La méthode Application_Start() de Global.asax drop et créée la base.
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(BddContext context)
        {
            context.Referentiels.Add(new Referentiel { Id = 1, Nom = "Général", Documents = new List<Document>() });
            context.Referentiels.Add(new Referentiel { Id = 2, Nom = "Energie", Documents = new List<Document>() });
            context.Referentiels.Add(new Referentiel { Id = 3, Nom = "Environnement", Documents = new List<Document>() });
            context.Referentiels.Add(new Referentiel { Id = 4, Nom = "Qualité", Documents = new List<Document>() });
            context.Referentiels.Add(new Referentiel { Id = 5, Nom = "Sécurité", Documents = new List<Document>() });

            context.Secteurs.Add(new Secteur { Id = 1, Nom = "Aucun", Documents = new List<Document>() });
            context.Secteurs.Add(new Secteur { Id = 2, Nom = "Chaud", Documents = new List<Document>() });
            context.Secteurs.Add(new Secteur { Id = 3, Nom = "Froid", Documents = new List<Document>() });
            context.Secteurs.Add(new Secteur { Id = 4, Nom = "EAP", Documents = new List<Document>() });

            base.Seed(context);
        }
    }
}