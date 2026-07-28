namespace BaseDoc_OI_GRC.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BaseDoc_OI_GRC.Models.BddContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BaseDoc_OI_GRC.Models.BddContext context)
        {
            context.Referentiels.AddOrUpdate(r => r.Id,
                new BaseDoc_OI_GRC.Models.Referentiel { Id = 1, Nom = "Général" },
                new BaseDoc_OI_GRC.Models.Referentiel { Id = 2, Nom = "Energie" },
                new BaseDoc_OI_GRC.Models.Referentiel { Id = 3, Nom = "Environnement" },
                new BaseDoc_OI_GRC.Models.Referentiel { Id = 4, Nom = "Qualité" },
                new BaseDoc_OI_GRC.Models.Referentiel { Id = 5, Nom = "Sécurité" }
            );

            context.Secteurs.AddOrUpdate(s => s.Id,
                new BaseDoc_OI_GRC.Models.Secteur { Id = 1, Nom = "Aucun" },
                new BaseDoc_OI_GRC.Models.Secteur { Id = 2, Nom = "Chaud" },
                new BaseDoc_OI_GRC.Models.Secteur { Id = 3, Nom = "Froid" },
                new BaseDoc_OI_GRC.Models.Secteur { Id = 4, Nom = "EAP" }
            );
        }
    }
}
