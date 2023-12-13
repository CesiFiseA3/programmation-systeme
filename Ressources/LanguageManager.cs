﻿using System.Collections.Generic;
using System;
using System.Globalization;  // Assurez-vous que cette ligne est présente
using System.Threading;
using System.Windows;


public static class LanguageManager
{
    // JB: J'aime bien cette façon de gérer les traductions
    private static Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();

    static LanguageManager()
    {
        languages["en-US"] = new Dictionary<string, string>
        {
            { "WelcomeMessage", "Welcome to EasySave, your backup software!" },
            { "BackupInformation", "ID - Name - Backup type" },
            { "description" ,"Choose your backup"  },
            { "language" ,"Choose your language  :" },
            { "create" ,"Create" },
            { "update" ,"Update" },
            { "Execut" ,"Execute" },
            { "Delete" ,"Delete" },
            { "ComplSave", "Complete" },
            { "DifSave", "Diferencial" },
            { "LabName", "Name :" },
            { "LabId", "ID :" },
            { "LabSource", "Source folder :" },
            { "LabDest", "Destination :" },
            { "LabSaveT", "Backup type :" },
            { "ButCancel", "Cancel" },
            { "ButValidate", "Ok" },
            { "SaveInfo1", "Name" },
            { "SaveInfo2", "Status" },
            { "SaveInfo3", "Progression" },
            { "BackupEnd", "Backups ended without a problem" },
            { "BackupError", "Error : Backup ended with error code " },
            { "Crypt", "Do you want to encrypt your backups ?" }
        };

        languages["fr-FR"] = new Dictionary<string, string>
        {
            { "WelcomeMessage", "Bienvenue dans EasySave, votre logiciel de sauvegarde !" },
            { "BackupInformation", "ID - Nom - Type" },
            { "description" ,"Choisissez votre sauvegarde"  },
            { "language" ,"Choisissez votre langue : "  },
            { "create" ,"Créer" },
            { "update" ,"Modifier"},
            { "Execut" ,"Executer"},
            { "Delete" ,"Supprimer" },
            { "ComplSave","Complète" },
            { "DifSave","Différentielle" },
            { "LabName","Nom :" },
            { "LabId", "ID :" },
            { "LabSource", "Dossier source :" },
            { "LabDest", "Destination :" },
            { "LabSaveT", "Sauvegarde :" },
            { "ButCancel", "Annuler" },
            { "ButValidate", "Valider" },
            { "SaveInfo1", "Nom" },
            { "SaveInfo2", "Status" },
            { "SaveInfo3", "Progression" },
            { "BackupEnd", "Les sauvegardes se sont déroulées sans problème"},
            { "BackupError", "Erreur : Sauvegarde terminée avec le code d'erreur "},
            { "Crypt", "Voulez-vous chiffrer vos sauvegardes ?" }

        };
    }

    public static string GetLocalizedString(string language, string key)
    {

        if (languages.ContainsKey(language) && languages[language].ContainsKey(key))
        {
            return languages[language][key];
        }


        return "";
    }
}
