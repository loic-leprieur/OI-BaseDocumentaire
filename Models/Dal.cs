using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BaseDoc_OI_GRC.ViewModels;

namespace BaseDoc_OI_GRC.Models
{
    /// <summary>
    ///     DAL : Data Access Layer = Couche d'Accès aux Données
    ///     Implémente les méthodes d'accès (UPDATE, SELECT, INSERT, DELETE) aux données de la base SQL.
    ///     Une seule instance de la Dal est crée pendant l'exécution de l'application sur le serveur web.
    /// </summary>
    public class Dal : IDal
    {
        /// <summary>
        ///     Instance de classe unique de la Dal
        /// </summary>
        private static Dal _dal;

        /// <summary>
        ///     Combinaison des modèles pour accéder à leur équivalent en base de données
        /// </summary>
        public static BddContext Bdd;

        /// <summary>
        ///     Répertoire stockant les fichiers
        /// </summary>
        private readonly DirectoryInfo _repertoire;

        /// <summary>
        ///     Index des titres de documents associés avec leur code.
        /// </summary>
        private readonly Dictionary<string, string> _titresDocs;

        /// <summary>
        ///     Tableau des fichiers récupérés dans le répertoire
        /// </summary>
        private FileInfo[] _fichiers;

        /// <summary>
        ///     Constructeur privée d'une Dal par défaut pour effectuer
        ///     les requêtes SQL sur la base de données.
        ///     Seule la classe peut construire une nouvelle Dal
        ///     pour toujours conserverune unique instance.
        /// </summary>
        private Dal()
        {            
            Bdd = new BddContext();
            Documents = Bdd.Documents.ToList();
            _titresDocs = new Dictionary<string, string>();
            var file = File.ReadAllLines(HttpContext.Current.Server.MapPath("~") + @"\config.txt");
            _repertoire = Directory.CreateDirectory(file[0]);
            //_repertoire = Directory.CreateDirectory(RepertoireRacine);
            _fichiers = _repertoire.GetFiles();
            /*LireFichierConfig();
            */

        }

        /// <summary>
        ///     Chemin vers le répertoire stockant les fichiers : à modifier dans config.txt
        /// </summary>
        public string RepertoireRacine { get; private set; }

        /// <summary>
        ///     Méthode de classe pour récupérer l'instance courante de la Dal sans la créer
        /// </summary>
        public static Dal ObtenirDal
        {
            get
            {
                if (_dal == null)
                {
                    _dal = new Dal();
                }
                return _dal;
            }
        }

        /// <summary>
        ///     Liste de Documents (objets de la classe Document) dans la base de données
        /// </summary>
        public List<Document> Documents { get; set; } = new List<Document>();

        /// <summary>
        ///     Instantie un nouveau référentiel à partir d'un nom donné.
        ///     Si le nom n'appartient à aucun référentiel existant,
        ///     alors on insère le référentiel dans la table correspondante.
        ///     L'Id du référentiel est incrémenté automatiquement.
        /// </summary>
        /// <param name="nom">Nom à affecter au nouveau référentiel</param>
        public void CreerReferentiel(string nom)
        {
            if (ReferentielExisteBdd(nom)) return;
            var referentiel = new Referentiel {Nom = nom, Documents = new List<Document>()};
            Bdd.Referentiels.Add(referentiel);
            Bdd.SaveChanges();
        }

        /***********************************************
         * Méthodes de sélection dans la base
         * (Récupérer un modèle depuis la base,
         * vérifier l'existante d'un modèle dans la base)
         ************************************************/

        /// <summary>
        ///     Obtient une instance du référentiel recherché par son nom.
        /// </summary>
        /// <param name="nomref">Nom du référentiel</param>
        /// <returns>Instance de Référentiel dont le nom correspond</returns>
        public Referentiel ObtenirReferentielParNom(string nomref)
        {
            if (ReferentielExisteBdd(nomref))
            {
                return
                    Bdd.Referentiels.FirstOrDefault(
                        referentiel =>
                            string.Compare(referentiel.Nom, nomref, StringComparison.CurrentCultureIgnoreCase) == 0);
            }
            return null;
        }

        /// <summary>
        ///     Instantie un nouveau secteur en fonction d'un nom donné
        /// </summary>
        /// <param name="strNom">Nom à affecter au nouveau Secteur</param>
        public void CreerSecteur(string strNom)
        {
            if (!SecteurExiste(strNom))
            {
                var secteur = new Secteur {Nom = strNom, Documents = new List<Document>()};
                Bdd.Secteurs.Add(secteur);
                Bdd.SaveChanges();
            }
        }

        /// <summary>
        ///     Retourne une instance de Référentiel dont l'Id correspond à l'entier donné en paramètre
        /// </summary>
        /// <param name="idReferentiel">Id - Entier</param>
        /// <returns></returns>
        public Referentiel ObtenirReferentielParId(int idReferentiel)
        {
            var referentiel = Bdd.Referentiels.FirstOrDefault(r => r.Id == idReferentiel);

            return referentiel;
        }

        /// <summary>
        ///     Retourne l'instance de Document dont le nomFichier correspond
        ///     correspond à au nom donné en paramètre
        /// </summary>
        /// <param name="strDoc">Nom de fichier du document</param>
        /// <returns></returns>
        public Document ObtenirUnDocumentParNomFichier(string strDoc)
        {
            if (DocumentExisteBdd(strDoc))
            {
                return
                    Documents.FirstOrDefault(
                        doc =>
                            string.Compare(doc.NomFichier, strDoc, StringComparison.CurrentCultureIgnoreCase) == 0 ||
                            doc.Code.Contains(strDoc));
            }
            return null;
        }


        /// <summary>
        ///     Retourne dans une liste les Référentiels existant dans
        ///     la base de données
        /// </summary>
        /// <returns>Liste de Referentiels</returns>
        public List<Referentiel> ObtientTousLesReferentiels()
        {
            var list = new List<Referentiel>();
            foreach (var referentiel in Bdd.Referentiels)
                list.Add(referentiel);
            return list;
        }

        /// <summary>
        ///     Retourne une liste de tous les secteurs contenus dans la base de données
        /// </summary>
        /// <returns>Liste de Secteurs</returns>
        public List<Secteur> ObtientTousLesSecteurs()
        {
            var list = new List<Secteur>();
            foreach (var secteur in Bdd.Secteurs)
                list.Add(secteur);
            return list;
        }

        /// <summary>
        ///     Vérifie si un référentiel désigné par son nom existe
        /// </summary>
        /// <param name="nomref"></param>
        /// <returns>Existe ou non</returns>
        public bool ReferentielExisteBdd(string nomref)
        {
            var refExiste = false;

            foreach (var referentiel in ObtientTousLesReferentiels())
            {
                if (string.Compare(referentiel.Nom, nomref, StringComparison.CurrentCultureIgnoreCase) == 0)
                    refExiste = true;
            }
            return refExiste;
        }

        /// <summary>
        ///     Retourne l'instance du document dont l'Id est donné en paramètre
        /// </summary>
        /// <param name="idDoc">Id du Document</param>
        /// <returns>Instance de Document</returns>
        public Document ObtenirDocumentParId(int idDoc)
        {
            return Bdd.Documents.FirstOrDefault(d => d.Id == idDoc);
        }

/*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fichierDoc"></param>
        /// <returns></returns>
        public FileInfo ObtenirUnDocumentDuRepertoire(string fichierDoc)
        {
            foreach (var fichier in _fichiers)
            {
                if (string.Compare(fichier.Name, fichierDoc, StringComparison.CurrentCultureIgnoreCase) == 0)
                    return fichier;
            }
            return null;
        }
*/

        /************************************************
         * Méthodes de mise à jour dans la base            
         *
         ************************************************/

        /**
         * Les champs d'un document sont mis à jour avec
         * l'id, le nom et le référentiel en paramètre.
         */

        /// <summary>
        ///     Mise à jour des propriétés d'un document.
        ///     Pour un Id donné, cette méthode
        ///     va modifier le titre, le type ou le code du document.
        /// </summary>
        /// <param name="id">Id unique d'un document</param>
        /// <param name="nouveauCode">Code ISO 14001 ex: 'FR05 AA 001'</param>
        /// <param name="nouveauTitre">Titre décrivant le document</param>
        /// <param name="nouveauType">Type donnant la fonction du document</param>
        public void ModifierDocument(int id, string nouveauCode, string nouveauTitre, string nouveauType)
        {
            var docTrouve = Bdd.Documents.FirstOrDefault(doc => doc.Id == id);

            if (docTrouve != null)
            {
                docTrouve.Titre = nouveauTitre ?? docTrouve.Titre;
                docTrouve.Type = nouveauType ?? docTrouve.Type;
                docTrouve.Code = nouveauCode ?? docTrouve.Code;
                Bdd.SaveChanges();
            }
        }

        /*
                /// <summary>
                /// 
                /// </summary>
                public void MiseAJourReferentielEtSecteurParTitre()
                {
                    foreach (var docTrouve in Documents)
                    {
                        foreach (var s in ObtientTousLesSecteurs())
                        {
                            if (docTrouve.Titre != null && docTrouve.Titre.ToUpper().Contains(s.Nom.ToUpper()))
                            {
                                AjouterDocumentASecteur(docTrouve.Id, s.Id);
                            }
                        }

                        foreach (var r in ObtientTousLesReferentiels())
                        {
                            if (docTrouve.Titre != null && docTrouve.Titre.ToUpper().Contains(r.Nom.ToUpper()))
                            {
                                AjouterDocumentAReferentiel(docTrouve.Id, r.Id);
                            }
                        }
                    }
                }
        */


        /*
                /// <summary>
                /// L'instance d'un référentiel présent dans la base est récupéré, puis
                /// ses champs sont modifiés et le référentiel est mis à jour dans la base
                /// </summary>
                /// <param name="idReferentiel"></param>
                /// <param name="nomReferentiel"></param>
                /// <param name="docs"></param>
                public void ModifierReferentiel(int idReferentiel, string nomReferentiel, List<Document> docs)
                {
                    var refTrouve = Bdd.Referentiels.FirstOrDefault(referentiel => referentiel.Id == idReferentiel);

                    if (refTrouve != null)
                    {
                        refTrouve.Nom = nomReferentiel;
                        refTrouve.Documents = docs;
                        Bdd.SaveChanges();
                    }
                }
        */

        /**
         * Un document possède par défaut le référentiel 'Indéfini'.
         * Pour le modifier les champs 'Ref_Id' et 'Referentiel' sont 
         * remplacés par les champs du référentiel donné en paramètre.
         *
         * Puisque le document a un nouveau référentiel, il est
         * ajouté à la liste des documents du référentiel.
         *
         * Enfin, les changement opérés sur le document et le référentiel
         * sont mis à jour dans la base.
         */

        /// <summary>
        ///     Intègre un document dans un référentiel et lui
        ///     associe son Id comme clé étrangère
        /// </summary>
        /// <param name="idDoc">Id du document</param>
        /// <param name="idRef">Id du référentiel</param>
        public void AjouterDocumentAReferentiel(int idDoc, int idRef)
        {
            var document = Bdd.Documents.FirstOrDefault(doc => doc.Id == idDoc);
            var referentiel = Bdd.Referentiels.FirstOrDefault(r => r.Id == idRef);

            if(referentiel != null && document != null)
            {
                referentiel.Documents.Add(document);
                Bdd.SaveChanges();
            }
        }

        /// <summary>
        ///     Intègre un document dans un secteur par leurs Ids.
        /// </summary>
        /// <param name="idDoc">Id du document</param>
        /// <param name="idSec">Id du secteur</param>
        public void AjouterDocumentASecteur(int idDoc, int idSec)
        {
            var document = Bdd.Documents.FirstOrDefault(doc => doc.Id == idDoc);
            var secteur = Bdd.Secteurs.FirstOrDefault(s => s.Id == idSec);

            if(document != null && secteur != null)
            {
                secteur.Documents.Add(document);
                Bdd.SaveChanges();
            }
        }

        /// <summary>
        ///     Supprime les données dans la base de données (Drop Database)
        /// </summary>
        public void Dispose()
        {
            Bdd.Dispose();
        }

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
        public int AjouterUtilisateur(string nom, string motDePasse)
        {
            var motDePasseEncode = EncodeSHA512(motDePasse);
            var utilisateur = new Utilisateur {Nom = nom, MotDePasse = motDePasseEncode};
            Bdd.Utilisateurs.Add(utilisateur);
            Bdd.SaveChanges();
            return utilisateur.Id;
        }

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
        public Utilisateur Authentifier(string nom, string motDePasse)
        {
            var motDePasseEncode = EncodeSHA512(motDePasse);
            return Bdd.Utilisateurs.FirstOrDefault(u => u.Nom == nom && u.MotDePasse == motDePasseEncode);
        }

        /// <summary>
        ///     Retourne l'instance Utilisateur correspond à l'Id en chaîne de caractère rentré en paramètre
        /// </summary>
        /// <param name="idStr">Id - Chaîne</param>
        /// <returns>Instance de Utilisateur</returns>
        public Utilisateur ObtenirUtilisateur(string idStr)
        {
            int id;
            if (int.TryParse(idStr, out id))
                return ObtenirUtilisateur(id);
            return null;
        }

        /// <summary>
        ///     Modifie la valeur du répertoire en écrivant
        ///     dans le fichier de configuration le nouveau chemin.
        /// </summary>
        /// <param name="nouveauRepertoire"></param>
        public void ModifierRepertoire(string nouveauRepertoire)
        {
            var lignes = File.ReadAllLines(HttpContext.Current.Server.MapPath("~") + "\\" + "config.txt");

            if (!string.IsNullOrEmpty(RepertoireRacine) && lignes[0] != nouveauRepertoire)
            {
                string[] tabRepertoire = {nouveauRepertoire};
                File.WriteAllLines(HttpContext.Current.Server.MapPath("~") + "\\" + "config.txt", tabRepertoire);
            }
        }

        /********************************************
         *
         * Méthodes de création, d'ajout dans la base
         *
         ********************************************/

        /// <summary>
        ///     Instantie un nouveau modèle Document avec un
        ///     nom donné, puis ajoute le document à la
        ///     base de données avec le référentiel par défaut "Indéfini", le secteur "Aucun" et sans type.
        ///     On commence par analyser les documents dont l'extension est .pdf,
        ///     puis on vérifie pour chaque fichier s'il n'existe pas.
        ///     S'il n'existe pas alors on va déterminer ses propriétés selon son nom.
        ///     On analyse la chaîne de caractères du nom du fichier pour
        ///     déterminer son type, son titre, son référentiel, son secteur et son code éventuel.
        /// </summary>
        /// <param name="fichier">Nom du fichier (avec extension) à créer si inexistant</param>
        private void CreerDocument(string fichier)
        {
            if (fichier.Split('.')[fichier.Split('.').Length - 1].Equals("pdf"))
            {
                string code = "",
                    type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.Indéfini);
                var referentiel = Bdd.Referentiels.Find(1);
                var secteur = Bdd.Secteurs.Find(1);

                if (!DocumentExisteBdd(fichier))
                {
                    // on ignore l'extension du fichier
                    var nomFichier = "";
                    for (var i = 0; i < fichier.Split('.').Length - 1; i++)
                    {
                        nomFichier += fichier.Split('.')[i];
                    }

                    // on distingue les mots séparés par un espace
                    var strFichier = nomFichier.Split(' ');

                    // un fichier avec un code commence par "F00" 
                    string titre;
                    if (strFichier[0].Contains("FR05"))
                    {
                        // le type du document correspond au deuxième mot
                        switch (strFichier[1])
                        {
                            case "BP":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.BP);
                                break;
                            case "DF":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.DF);
                                break;
                            case "DO":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.DO);
                                break;
                            case "EN":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.EN);
                                break;
                            case "IT":
                            case "GR":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.IT);
                                break;
                            case "PR":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.PR);
                                break;
                            case "MO":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.MO);
                                break;
                            case "CS":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.CS);
                                break;
                            case "SP":
                                type = DocFormViewModel.GetEnumDescription(DocFormViewModel.Types.SP);
                                break;
                            default:
                                type = DocFormViewModel.Types.Indéfini.ToString();
                                break;
                        }

                        // le code contiend les trois premiers mots
                        code = strFichier[0] + " " + strFichier[1] + " " + strFichier[2];

                        // si le titre du fichier existe dans la liste préexistante alors on l'ajoute
                        titre = ObtenirTitreAvecCode(code);

                        // si le nom du fichier a plus de 3 mots, alors il a un titre
                        if (titre == null && strFichier.Length > 3)
                        {
                            // pour chaque mot supplémentaire, on l'ajoute au titre
                            for (var i = 3; i < strFichier.Length; i++)
                            {
                                titre += strFichier[i] + " ";
                            }
                        }
                    }
                    // si le nom du fichier n'est pas un code, alors il devient le titre du document
                    else
                    {
                        titre = nomFichier;
                    }

                    var doc = new Document
                    {
                        Code = code,
                        Type = type,
                        Titre = titre,
                        NomFichier = fichier,
                        Referentiel = referentiel,
                        Secteur = secteur
                    };
                    Bdd.Documents.Add(doc);
                    Bdd.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Retourne dans une liste les Documents d'un secteur donné par son Id passé en paramètre
        /// </summary>
        /// <param name="idSecteur">Id du secteur</param>
        /// <returns>Collection de tous les documents d'un secteur</returns>
        public List<Document> ObtientTousLesDocumentsDeSecteur(int? idSecteur)
        {
            var docDeSec = new List<Document>();

            foreach (var doc in Documents)
            {
                if (doc.Secteur.Id == idSecteur || (idSecteur == 0 && doc.Secteur.Id != 1))
                {
                    docDeSec.Add(doc);
                }
            }
            return docDeSec;
        }

        /// <summary>
        ///     Recherche dans la base les documents correspondant aux mots choisis.
        ///     Pour chaque mot clé dans le champ de recherche, si un document a le même code
        ///     ou contient ce mot dans son titre alors on ajoute ce document dans la liste de résultats.
        ///     Lorsque tous les mots clé ont été recherché on affiche dans l'action
        ///     Index de PrincipalController les documents trouvés.
        /// </summary>
        /// <param name="strMotsCles">Chaîne de caractères dont chaque espace délimite un nouveau mot-clé</param>
        /// <returns>Liste de Documents</returns>
        public List<Document> RechercheDocumentsParMotsCles(string strMotsCles)
        {
            var docs = new List<Document>();

            foreach (var doc in Documents)
            {
                if (!string.IsNullOrEmpty(doc.Code) &&
                    strMotsCles.ToUpper().Replace(" ", "").Contains(doc.Code.ToUpper().Replace(" ", "")))
                {
                    docs.Add(doc);
                    break;
                }

                if (!string.IsNullOrEmpty(doc.Titre) &&
                    doc.Titre.ToUpper().Replace(" ", "").Contains(strMotsCles.ToUpper().Replace(" ", "")))
                {
                    docs.Add(doc);
                }
            }

            return docs;
        }

        /// <summary>
        ///     Met à jour la liste de Document contenus dans le champs de classe Documents
        ///     avec les Documents de la base de données.
        /// </summary>
        public void RecupereDocumentsBdd()
        {
            var list = new List<Document>();
            foreach (var document in Bdd.Documents)
                list.Add(document);
            Documents = list;
        }

        /// <summary>
        ///     Retourne tous les documents filtré par un référentiel donné par son Id
        ///     Si l'entier donné est égal à 0, alors on retournera les documents
        ///     dont le référentiel n'est pas 'Général'.
        /// </summary>
        /// <param name="idRef">Id falcultatif du référentiel</param>
        /// <returns></returns>
        public List<Document> ObtientTousLesDocumentsDeReferentiel(int? idRef)
        {
            var docDeRef = new List<Document>();

            foreach (var doc in Documents)
            {
                if (doc.Referentiel.Id == idRef || (idRef == 0 && doc.Referentiel.Id != 1))
                {
                    docDeRef.Add(doc);
                }
            }
            return docDeRef;
        }

        /// <summary>
        ///     Vérifie si le document existe dans la base
        /// </summary>
        /// <param name="nomDoc"></param>
        /// <returns>Existe ou non</returns>
        private bool DocumentExisteBdd(string nomDoc)
        {
            var docExiste = false;
            var docTrouve = Documents.FirstOrDefault(d => d.NomFichier == nomDoc);

            if (docTrouve != null)
            {
                docExiste = true;
            }
            return docExiste;
        }

/*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDoc"></param>
        public void RetirerDocumentAReferentiel(int idDoc)
        {
            var doc = ObtenirDocumentParId(idDoc);
            var referentiel = ObtenirReferentielParId(doc.Referentiel.Id);

            doc.IdRef = 0;

            referentiel.Documents.Remove(doc);

            Bdd.SaveChanges();
        }
*/

        /// <summary>
        ///     Analyse des fichiers absents de la base mais présent dans le répertoire.
        ///     Si des documents ne sont pas présent alors ils sont insérés.
        /// </summary>
        public void CreerNouveauxFichiersDuRepertoire()
        {
            //LireFichierTitres();
            LireFichierConfig();
            Document docTrouve;
            _fichiers = _repertoire.GetFiles();

            // recherche les documents à la racine du dossier
            // ajoute les documents à la base s'il ne sont pas encore ajoutés
            foreach (var fichier in _fichiers)
            {
                docTrouve = Documents.FirstOrDefault(d => d.NomFichier == fichier.Name);
                if (docTrouve == null)
                {
                    CreerDocument(fichier.Name);
                }
            }

            // recherche dans les sous-dossiers à la racine les fichiers qui
            // ne sont pas encore ajoutés à la base
            foreach (var dir in _repertoire.GetDirectories())
            {
                foreach (var file in dir.GetFiles())
                {
                    docTrouve = Documents.FirstOrDefault(d => d.NomFichier == file.Name);
                    if (docTrouve == null)
                    {
                        CreerDocument(file.Name);
                    }
                }
            }
        }

        /// <summary>
        ///     Retire de la base de données le document dont le nom
        ///     correspond au nom en paramètre.
        /// </summary>
        /// <param name="nomDoc">Nom du document à supprimer</param>
        public void SupprimerDocument(string nomDoc)
        {
            if (DocumentExisteBdd(nomDoc))
            {
                var doc = ObtenirUnDocumentParNomFichier(nomDoc);
                if (doc != null)
                {
                    Bdd.Documents.Remove(doc);
                    Bdd.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Supprime de la base de données tous les documents
        ///     qui n'existe plus dans le Répertoire.
        ///     L'analyse du Répertoire s'effectue à la racine et dans le premier niveau de ses sous-dossiers.
        ///     (C:\Racine\monDoc.pdf ou C:\Racine\SousDossier\monDoc.pdf)
        /// </summary>
        public void RetirerDeBddLesFichiersSupprimes()
        {
            RecupereDocumentsBdd();
            FileInfo fileTrouve;
            _fichiers = _repertoire.GetFiles();

            foreach (var doc in Documents)
            {
                fileTrouve = _fichiers.FirstOrDefault(d => d.Name == doc.NomFichier);
                if (fileTrouve == null)
                {
                    try
                    {
                        Bdd.Documents.Remove(doc);
                        Bdd.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Id doc : " + doc.Id);
                        var s = e.ToString();
                        Trace.WriteLine(s);
                    }
                }
            }

            foreach (var dir in _repertoire.GetDirectories())
            {
                foreach (var doc in Documents)
                {
                    fileTrouve = dir.GetFiles().FirstOrDefault(d => d.Name == doc.NomFichier);
                    if (fileTrouve == null)
                    {
                        try
                        {
                            Bdd.Documents.Remove(doc);
                            Bdd.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine("Id doc : " + doc.Id);
                            var s = e.ToString();
                            Trace.WriteLine(s);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Retourne l'instance Utilisateur correspond à l'Id rentré en paramètre
        /// </summary>
        /// <param name="id">Id - Entier</param>
        /// <returns>Instance de Utilisateur</returns>
        private Utilisateur ObtenirUtilisateur(int id)
        {
            return Bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
        }

/*
        /// <summary>
        /// Obtient une instance du secteur recherché par son nom.
        /// </summary>
        /// <param name="nomSecteur">Nom du secteur</param>
        /// <returns>Instance de Secteur dont le nom correspond</returns>
        public Secteur ObtenirSecteurParNom(string nomSecteur)
        {
            if (SecteurExiste(nomSecteur))
            {
                return
                    Bdd.Secteurs.FirstOrDefault(
                        secteur =>
                            string.Compare(secteur.Nom, nomSecteur, StringComparison.CurrentCultureIgnoreCase) == 0);
            }
            return null;
        }
*/

        /// <summary>
        ///     Parcourt la table Secteur et indique si le nom donné est un secteur existant
        /// </summary>
        /// <param name="nomSecteur">Nom du secteur recherché</param>
        /// <returns>Secteur existe ou non</returns>
        private bool SecteurExiste(string nomSecteur)
        {
            var secteurExiste = false;

            foreach (var secteur in ObtientTousLesSecteurs())
            {
                if (string.Compare(secteur.Nom, nomSecteur, StringComparison.CurrentCultureIgnoreCase) == 0)
                    secteurExiste = true;
            }
            return secteurExiste;
        }

/*
        private void MiseAJourReferentielEtSecteurParTitre(Document docTrouve)
        {
            if (docTrouve.Titre != null)
            {
                foreach (var s in ObtientTousLesSecteurs())
                {
                    if (docTrouve.Titre.ToUpper().Contains(s.Nom.ToUpper()))
                    {
                        AjouterDocumentASecteur(docTrouve.Id, s.Id);
                    }
                }

                foreach (var r in ObtientTousLesReferentiels())
                {
                    if (docTrouve.Titre.ToUpper().Contains(r.Nom.ToUpper()))
                    {
                        AjouterDocumentAReferentiel(docTrouve.Id, r.Id);
                    }
                }
            }
        }
*/

        /// <summary>
        ///     Retourne le hash de la chaîne de caractère avec l'algorithme SHA512
        ///     et une chaîne salante.
        /// </summary>
        /// <param name="motDePasse"></param>
        /// <returns></returns>
        private string EncodeSHA512(string motDePasse)
        {
            var motDePasseSel = "BaseDocOI" + motDePasse + "Gironcourt-Sur-Vraine";
            return
                BitConverter.ToString(
                    new SHA512CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(motDePasseSel)));
        }

        /// <summary>
        ///     Analyse le fichier texte de configuration 'config.txt' de l'application.
        ///     La valeur obtenue sera le chemin du Répertoire où les fichiers sont stockés.
        /// </summary>
        private void LireFichierConfig()
        {
            var lignes = File.ReadAllLines(HttpContext.Current.Server.MapPath("~") + @"\config.txt");

            if (!string.IsNullOrEmpty(RepertoireRacine) && lignes[0] != RepertoireRacine)
            {
                string[] tabRepertoire = {RepertoireRacine};
                File.WriteAllLines(HttpContext.Current.Server.MapPath("~") + @"\config.txt", tabRepertoire);
            }
            else
            {
                RepertoireRacine = lignes[0];
            }
        }

        /// <summary>
        ///     Retourne le titre d'un document à partir de son code si le couple
        ///     existe dans le Dictionnaire / Index 'TitreDocs'.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ObtenirTitreAvecCode(string code)
        {
            var doc = _titresDocs.FirstOrDefault(titre => titre.Key == code);
            {
                if (doc.Key != null)
                {
                    return doc.Value;
                }
                return null;
            }
        }
    }
}