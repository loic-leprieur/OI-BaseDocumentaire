// Script principal : formatte la table, ajoute les icônes et gère les évènements (suppression, modification)
var STYLE_JS = {
    modules : {}
};

// Module d'ajout des icônes sur les boutons généré par les helpers Html ASP.NET
STYLE_JS.modules.ajout_icones = (function () {
    return {
        run: function () {
            /* icones de l'entete de la table */
            $(".colCheckbox").remove(".sorting");
            $(".table-entete-telechargement").remove(".sorting");
            $(".table-entete-modification").remove(".sorting");
            /* icones du body */
            $(".bouton-actualiser").append("<i class=\"glyphicon glyphicon-refresh\"></i>");
            $(".bouton-supprimer").append("<i class=\"glyphicon glyphicon-trash\"></i>");
            $(".bouton-telechargement").append("<i class=\"glyphicon glyphicon-save-file\"></i>");
            $(".bouton-modifier").append("<i class=\"glyphicon glyphicon-edit\"></i>");
            $(".bouton-annuler").append(" <i class=\"glyphicon glyphicon-remove\"></i>");
            $(".bouton-enregistrer").append(" <i class=\"glyphicon glyphicon-floppy-save\"></i>");
            /* icones de l'accueil */
            $("#item-Aucun")
                .addClass("btn-success");
            $("#item-Chaud")
                .addClass("btn-danger")
                .append(" <i class=\"glyphicon glyphicon-fire\"></i>");
            $("#item-Froid")
                .addClass("btn-primary")
                .append(" <i class=\"glyphicon glyphicon-ice-lolly\"></i>");
            $("#item-EAP")
                .addClass("btn-warning")
                .append(" <i class=\"glyphicon glyphicon-road\"></i>");
            $("#bouton-accesbase").append(" <i class=\"glyphicon glyphicon-folder-open\"></i>");
            $("#item-Aucun").append(" <i class=\"glyphicon glyphicon-ban-circle\"></i>");
            /* icones du nav */
            $("#bouton-accueil").prepend("<i class=\"glyphicon glyphicon-home\"></i> ");
            $("#bouton-connexion").prepend("<i class=\"glyphicon glyphicon-log-in\"></i> ");
            $("#bouton-configuration").prepend("<i class=\"glyphicon glyphicon-wrench\"></i> ");
            $("#bouton-deconnexion").prepend("<i class=\"glyphicon glyphicon-log-out\"></i> ");
            $("#bouton-touslesdocuments").prepend("<i class=\"glyphicon glyphicon-folder-open\"></i> ");
            $("#bouton-referentiels").prepend("<i class=\"glyphicon glyphicon-leaf\"></i> ");
            $("#bouton-secteurs").prepend("<i class=\"glyphicon glyphicon-fire\"></i> ");
        }
    }
})();

// Module traduisant la table en français avec un JSON, active la surbrillance des lignes 
// survolées et gère l'affichage de la colonne checkbox
STYLE_JS.modules.formatter_table = (function () {
    var colCheckbox;

    return {
        init: function () {
            if ($('#table-documents').length) {
                var table;
                if ($('#table-documents tr').children('th').length === 8) {
                    table = $('#table-documents').DataTable({
                        responsive: {
                            details: {
                                type: 'column',
                                target: 'tr'
                            }
                        },
                        "aoColumns": [
                            { "bSortable": false },
                            null,
                            null,
                            null,
                            null,
                            null,
                            { "bSortable": false },
                            { "bSortable": false }
                        ],
                        "paging": true,
                        language: {
                            processing:     "Traitement en cours...",
                            search:         "Rechercher&nbsp;:",
                            lengthMenu:    "Afficher _MENU_ &eacute;l&eacute;ments",
                            info:           "Affichage de l'&eacute;lement _START_ &agrave; _END_ sur _TOTAL_ &eacute;l&eacute;ments",
                            infoEmpty:      "Affichage de l'&eacute;lement 0 &agrave; 0 sur 0 &eacute;l&eacute;ments",
                            infoFiltered:   "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)",
                            infoPostFix:    "",
                            loadingRecords: "Chargement en cours...",
                            zeroRecords:    "Aucun &eacute;l&eacute;ment &agrave; afficher",
                            emptyTable:     "Aucune donnée disponible dans le tableau",
                            paginate: {
                                first:      "Premier",
                                previous:   "Pr&eacute;c&eacute;dent",
                                next:       "Suivant",
                                last:       "Dernier"
                            },
                            aria: {
                                sortAscending:  ": activer pour trier la colonne par ordre croissant",
                                sortDescending: ": activer pour trier la colonne par ordre décroissant"
                            }
                        }
                    });
                    new $.fn.dataTable.FixedHeader(table);
                    colCheckbox = $("#table-documents .colCheckbox");
                } else {
                    table = $('#table-documents').DataTable({
                        responsive: {
                            details: {
                                type: 'column',
                                target: 'tr'
                            }
                        },
                        "aoColumns": [
                            null,
                            null,
                            null,
                            null,
                            null,
                            { "bSortable": false }
                        ],
                        "paging": true,
                        language: {
                            processing: "Traitement en cours...",
                            search: "Rechercher&nbsp;:",
                            lengthMenu: "Afficher _MENU_ &eacute;l&eacute;ments",
                            info: "Affichage de l'&eacute;lement _START_ &agrave; _END_ sur _TOTAL_ &eacute;l&eacute;ments",
                            infoEmpty: "Affichage de l'&eacute;lement 0 &agrave; 0 sur 0 &eacute;l&eacute;ments",
                            infoFiltered: "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)",
                            infoPostFix: "",
                            loadingRecords: "Chargement en cours...",
                            zeroRecords: "Aucun &eacute;l&eacute;ment &agrave; afficher",
                            emptyTable: "Aucune donnée disponible dans le tableau",
                            paginate: {
                                first: "Premier",
                                previous: "Pr&eacute;c&eacute;dent",
                                next: "Suivant",
                                last: "Dernier"
                            },
                            aria: {
                                sortAscending: ": activer pour trier la colonne par ordre croissant",
                                sortDescending: ": activer pour trier la colonne par ordre décroissant"
                            }
                        }
                    });
                    new $.fn.dataTable.FixedHeader(table);
                }
            }
        },
        selectionLignes: function () {
            $(".menu-item").hover(function () {
                $(this).toggleClass("active");
            });
        },
        retirerDocument: function (docsArray) {
            $.ajax({
                type: 'POST',
                url: 'Principal/SupprimerDocuments',
                data: { "docsToDelete": docsArray },
                /*contentType: 'application/json; charset=utf-8',*/
                datatype: 'json',
                success: function (result) {
                    alert('Success ' + result.d + this.data.docsToDelete);
                },
                error: function (result) {
                    alert('Fail ' + result.d + " " + this.data.docsToDelete);
                }
            });
        },
        modifierDocument: function (docsArray) {
            $.ajax({
                type: 'POST',
                url: 'Principal/ModifierDocument',
                data: { "docsToModify": docsArray },
                datatype: 'json',
                success: function (result) {
                    alert('Success ' + result.d + this.data.docsToModify);
                },
                error: function (result) {
                    alert('Fail ' + result.d + " " + this.data.docsToModify);
                }
            });
        },
        run: function () {
            this.init();
            this.selectionLignes();

            // handlers du clic sur les boutons d'administration 'Supprimer' et 'Modifier'
            $("#bouton-supprimer").click(function () {
                var selection = STYLE_JS.modules.actionsColCheckbox.obtenirTableau();
                if (selection.length === 0) {
                    alert("Veuillez sélectionner au moins un document à supprimer");
                } else {
                    STYLE_JS.modules.formatter_table.retirerDocument(selection);
                    for (var i = 0; i < selection.length; i++) {
                        console.log("----> " + selection[i] + " supprimé");
                        $('#' + selection[i]).parent("td").parent("tr").remove();
                        //STYLE_JS.modules.actionsColCheckbox.retirerASelection(selection[i]);
                    }
                    STYLE_JS.modules.actionsColCheckbox.viderDocumentsSelectiones();
                }
            });

            /*$("#bouton-modifier").click(function () {
                var selection = STYLE_JS.modules.actionsColCheckbox.obtenirTableau();
                if (selection.length === 0) {
                    alert("Veuillez sélectionner au moins un document à modifier");
                } else {
                    //STYLE_JS.modules.formatter_table.retirerDocument(selection);
                    for (i = 0; i < selection.length; i++) {
                        console.log("----> " + selection[i] + " modifié");
                        //$('#' + selection[i]).parent("td").parent("tr").remove();
                        //STYLE_JS.modules.actionsColCheckbox.retirerASelection(selection[i]);
                    }
                    STYLE_JS.modules.actionsColCheckbox.viderDocumentsSelectiones();
                }
            });*/

            $(".mainCheckbox").change(function () {
                if ($(this).is(':checked')) {
                    console.log("MAIN all checkboxes checked");
                    $(".checkbox-doc").prop("checked", true).change();
                } else {
                    console.log("MAIN all checkboxes unchecked");
                    $(".checkbox-doc").prop("checked", false).change();
                    STYLE_JS.modules.actionsColCheckbox.viderDocumentsSelectiones();
                }
            });

            $(".checkbox-doc").change(function () {
                if ($(this).is(':checked')) {
                    console.log($(this).attr('value') + " checked");
                    $(this).parent("td").parent("tr").toggleClass("active");
                    STYLE_JS.modules.actionsColCheckbox.ajouterASelection(parseInt($(this).attr('value')));
                    STYLE_JS.modules.actionsColCheckbox.afficherDocumentsSelectiones();
                } else {
                    console.log($(this).attr('id') + " unchecked");
                    $(this).parent("td").parent("tr").toggleClass("active");
                    STYLE_JS.modules.actionsColCheckbox.retirerASelection(parseInt($(this).attr('value')));
                    STYLE_JS.modules.actionsColCheckbox.afficherDocumentsSelectiones();
                }
            });
        }
    }
})();

STYLE_JS.modules.actionsColCheckbox = (function () {
    var documentsSelectiones = [];

    return {
        init: function () {
            documentsSelectiones = [];
        },
        ajouterASelection: function (doc) {
            documentsSelectiones.push(doc);
            console.log(documentsSelectiones);
        },
        afficherDocumentsSelectiones: function () {
            console.log("Sélection :");
            for (var i = 0; i < documentsSelectiones.length; i++) {
                console.log("(" + i + ") : " + documentsSelectiones[i]);
            }
            console.log("-----------------");
        },
        viderDocumentsSelectiones : function(){
            documentsSelectiones.length = 0;
        },
        retirerASelection: function (doc) {
            // lorsque décoché
            for (var i = 0; i < documentsSelectiones.length; i++) {
                if (documentsSelectiones[i].document === doc) {
                    documentsSelectiones.splice(i, 1);
                    console.log(documentsSelectiones);
                }
            }
        },
        obtenirTableau: function (){
            return documentsSelectiones;
        }
    }
})();

// Module principal qui active les autres modules avec leur fonction run()
STYLE_JS.modules.app = (function () {
    return {
        run: function () {
            STYLE_JS.modules.formatter_table.run();
            STYLE_JS.modules.ajout_icones.run();
        }
    }
})();

// Active le module principal au chargement initial de la page
window.onload = STYLE_JS.modules.app.run;