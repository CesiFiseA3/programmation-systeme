﻿using System.Collections.Generic;


using System;
using System.Globalization;  // Assurez-vous que cette ligne est présente
using System.Threading;
using System.Windows;


public static class LanguageManager
{
    private static Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();

    static LanguageManager( )
    {
        languages["en-US"] = new Dictionary<string, string>
        {
            { "WelcomeMessage", "Welcome to EasySave, your backup software!" },
            { "BackupInformation", "File ID    |     name     |    source     |    destination    |   Backup type" },
            { "description" ,"Choose your backup"  },
            { "language" ,"Choose your language  :" },
            { "create" ,"Create" },
            { "update" ,"Update" },
            { "Execut" ,"Execute" },
            { "Delete" ,"Delete" }


        };

        languages["fr-FR"] = new Dictionary<string, string>
        {
            { "WelcomeMessage", "Bienvenue dans EasySave, votre logiciel de sauvegarde !" },
            { "BackupInformation", "ID de fichier   |   nom |     source    |    destination    |   Type" },
            { "description" ,"Choisissez votre sauvegarde"  },
            { "language" ,"Choisissez votre langue : "  },
            { "create" ,"Créer" },
            { "update" ,"Modifier"},
            { "Execut" ,"Executer"},
            { "Delete" ,"Supprimer" }
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
