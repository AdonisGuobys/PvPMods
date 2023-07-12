using PvPMods.Commands;
using PvPMods.Hooks;
using PvPMods.Systems;
using System;
using UnityEngine.Rendering.HighDefinition;

namespace PvPMods.Utils
{
    public static class AutoSaveSystem
    {
        //-- AutoSave is now directly hooked into the Server game save activity.
        public const string mainSaveFolder = "BepInEx/config/PvPMods/Saves/";
        public const string backupSaveFolder = "BepInEx/config/PvPMods/Saves/Backup/";
        private static int saveCount = 0;
        public static int backupFrequency = 5;
        public static bool saveLogging = false;
        public static void SaveDatabase(){
            saveCount++;
            string saveFolder = mainSaveFolder;
            if(saveCount % backupFrequency == 0) {
                saveFolder = backupSaveFolder;
            }
            //PermissionSystem.SaveUserPermission(); //-- Nothing new to save.
            //-- System Related
            PvPSystem.SavePvPStat(saveFolder);
            //BanSystem.SaveBanList();

            Plugin.Logger.LogInfo(DateTime.Now+": All databases saved to JSON file.");
        }

        public static void LoadDatabase()
        {
            //-- Commands Related
            PermissionSystem.LoadPermissions();

            //-- System Related
            PvPSystem.LoadPvPStat();

            Plugin.Logger.LogInfo("All database is now loaded.");
        }
    }
}
