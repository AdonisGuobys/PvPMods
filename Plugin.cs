using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using PvPMods.Commands;
using PvPMods.Hooks;
using PvPMods.Systems;
using PvPMods.Utils;
using System.IO;
using System.Reflection;
using Unity.Entities;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using VampireCommandFramework;
using ProjectM;

#if WETSTONE
using Wetstone.API;
#endif


namespace PvPMods
{
    [BepInPlugin("PvPMods", "PvPMods", "0.1.0")]
    [BepInDependency("gg.deca.VampireCommandFramework")]

    public class Plugin : BasePlugin
    {
        public static Harmony harmony;


        private static ConfigEntry<bool> AnnouncePvPKills;
        private static ConfigEntry<bool> EnablePvPToggle;

        private static ConfigEntry<bool> EnablePvPLadder;
        private static ConfigEntry<int> PvPLadderLength;
        private static ConfigEntry<bool> HonorSortLadder;
        
        private static ConfigEntry<bool> EnablePvPPunish;
        private static ConfigEntry<bool> EnablePvPPunishAnnounce;
        private static ConfigEntry<bool> ExcludeOfflineKills;
        private static ConfigEntry<int> PunishLevelDiff;
        private static ConfigEntry<float> PunishDuration;
        private static ConfigEntry<int> PunishOffenseLimit;
        private static ConfigEntry<float> PunishOffenseCooldown;

        private static ConfigEntry<bool> EnableHonorSystem;
        private static ConfigEntry<bool> EnableHonorTitle;
        private static ConfigEntry<int> MaxHonorGainPerSpan;
        private static ConfigEntry<bool> EnableHonorBenefit;
        private static ConfigEntry<int> HonorSiegeDuration;
        private static ConfigEntry<bool> EnableHostileGlow;
        private static ConfigEntry<bool> UseProximityGlow;

        private static ConfigEntry<bool> BuffSiegeGolem;
        private static ConfigEntry<float> GolemPhysicalReduction;
        private static ConfigEntry<float> GolemSpellReduction;

        private static ConfigEntry<bool> buffLogging;
        private static ConfigEntry<bool> deathLogging;
        private static ConfigEntry<bool> saveLogging;

        private static ConfigEntry<int> buffID;
        private static ConfigEntry<int> appliedBuff;

        public static bool isInitialized = false;

        public static ManualLogSource Logger;
        private static World _serverWorld;
        public static World Server
        {
            get
            {
                if (_serverWorld != null) return _serverWorld;

                _serverWorld = GetWorld("Server")
                    ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");
                return _serverWorld;
            }
        }

        public static bool IsServer => Application.productName == "VRisingServer";

        private static World GetWorld(string name)
        {
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name)
                {
                    return world;
                }
            }

            return null;
        }

        public void InitConfig(){

            AnnouncePvPKills = Config.Bind("PvP", "Announce PvP Kills", true, "Make a server wide announcement for all PvP kills.");
            EnableHonorSystem = Config.Bind("PvP", "Enable Honor System", false, "Enable the honor system.");
            EnableHonorTitle = Config.Bind("PvP", "Enable Honor Title", true, "When enabled, the system will append the title to their name.\nHonor system will leave the player name untouched if disabled.");
            MaxHonorGainPerSpan = Config.Bind("PvP", "Max Honor Gain/Hour", 250, "Maximum amount of honor points the player can gain per hour.");
            EnableHonorBenefit = Config.Bind("PvP", "Enable Honor Benefit & Penalties", true, "If disabled, the hostility state and custom siege system will be disabled.\n" +
                "All other bonus is also not applied.");
            HonorSiegeDuration = Config.Bind("PvP", "Custom Siege Duration", 180, "In minutes. Player will automatically exit siege mode after this many minutes has passed.\n" +
                "Siege mode cannot be exited while duration has not passed.");
            EnableHostileGlow = Config.Bind("PvP", "Enable Hostile Glow", true, "When set to true, hostile players will glow red.");
            UseProximityGlow = Config.Bind("PvP", "Enable Proximity Hostile Glow", true, "If enabled, hostile players will only glow when they are close to other online player.\n" +
                "If disabled, hostile players will always glow red.");
            EnablePvPLadder = Config.Bind("PvP", "Enable PvP Ladder", true, "Enables the PvP Ladder in the PvP command.");
            PvPLadderLength = Config.Bind("PvP", "Ladder Length", 10, "How many players should be displayed in the PvP Ladders.");
            HonorSortLadder = Config.Bind("PvP", "Sort PvP Ladder by Honor", true, "This will automatically be false if honor system is not enabled.");
            EnablePvPToggle = Config.Bind("PvP", "Enable PvP Toggle", false, "Enable/disable the pvp toggle feature in the pvp command.");

            EnablePvPPunish = Config.Bind("PvP", "Enable PvP Punishment", false, "Enables the punishment system for killing lower level player.");
            EnablePvPPunishAnnounce = Config.Bind("PvP", "Enable PvP Punish Announcement", true, "Announce all grief-kills that occured.");
            ExcludeOfflineKills = Config.Bind("PvP", "Exclude Offline Grief", true, "Do not punish the killer if the victim is offline.");
            PunishLevelDiff = Config.Bind("PvP", "Punish Level Difference", -10, "Only punish the killer if the victim level is this much lower.");
            PunishOffenseLimit = Config.Bind("PvP", "Offense Limit", 3, "Killer must make this many offense before the punishment debuff is applied.");
            PunishOffenseCooldown = Config.Bind("PvP", "Offense Cooldown", 300f, "Reset the offense counter after this many seconds has passed since last offense.");
            PunishDuration = Config.Bind("PvP", "Debuff Duration", 1800f, "Apply the punishment debuff for this amount of time.");

            BuffSiegeGolem = Config.Bind("Siege", "Buff Siege Golem", false, "Enabling this will reduce all incoming physical and spell damage according to config.");
            GolemPhysicalReduction = Config.Bind("Siege", "Physical Damage Reduction", 0.5f, "Reduce incoming damage by this much. Ex.: 0.25 -> 25%");
            GolemSpellReduction = Config.Bind("Siege", "Spell Damage Reduction", 0.5f, "Reduce incoming spell damage by this much. Ex.: 0.75 -> 75%");



            buffID = Config.Bind("Buff System", "Buff GUID", -1465458722, "The GUID of the buff you want to hijack for the buffs from mastery, bloodlines, and everything else from this mod\nDefault is boots, 1409441911 is cloak, but you can set anything else too");
            appliedBuff = Config.Bind("Buff System", "Applied Buff", -1464851863, "The GUID of the buff that gets applied when mastery, bloodline, etc changes. Doesnt need to be the same as the Buff GUID.");
            
            buffLogging = Config.Bind("Debug", "Buff system logging", false, "Logs detailed information about the buff system in your console, enable before sending me any errors with the buff system!");
            deathLogging = Config.Bind("Debug", "Death logging", false, "Logs detailed information about death events in your console, enable before sending me any errors with the xp system!");
            saveLogging = Config.Bind("Debug", "Save system logging", false, "Logs detailed information about the save system in your console, enable before sending me any errors with the buff system!");
           

            if (!Directory.Exists("BepInEx/config/PvPMods")) Directory.CreateDirectory("BepInEx/config/PvPMods");
            if (!Directory.Exists("BepInEx/config/PvPMods/Saves")) Directory.CreateDirectory("BepInEx/config/PvPMods/Saves");
            if (!Directory.Exists("BepInEx/config/PvPMods/Saves/Backup")) Directory.CreateDirectory("BepInEx/config/PvPMods/Saves/Backup");

        }

        public override void Load()
        {
            if(!IsServer)
            {
                Log.LogWarning("PvPMods is a server plugin. Not continuing to load on client.");
                return;
            }
            
            InitConfig();
            Logger = Log;
            harmony = new Harmony("PvPMods");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            TaskRunner.Initialize();

            Log.LogInfo("Plugin PvPMods is loaded!");
        }

        public override bool Unload()
        {
            AutoSaveSystem.SaveDatabase();
            Config.Clear();
            harmony.UnpatchSelf();

            TaskRunner.Destroy();

            return true;
        }

        public void OnGameInitialized()
        {
            Initialize();
        }

        public static void Initialize()
        {
            Logger.LogInfo("Trying to Initalize PvPMods, isInitalized already: " + isInitialized);
            if (isInitialized) return;
            Logger.LogInfo("Initalizing PvPMods");
            //-- Initialize System
            Helper.CreatePlayerCache();
            Helper.GetServerGameSettings(out Helper.SGS);
            Helper.GetServerGameManager(out Helper.SGM);
            Helper.GetUserActivityGridSystem(out Helper.UAGS);
            ProximityLoop.UpdateCache();
            PvPSystem.Interlocked.isSiegeOn = false;


            //-- Commands Related
            AutoSaveSystem.LoadDatabase();

            //-- Apply configs
            Logger.LogInfo("Registering commands");
            CommandRegistry.RegisterAll();

            Logger.LogInfo("Loading PvP config");
            PvPSystem.isPvPToggleEnabled = EnablePvPToggle.Value;
            PvPSystem.isAnnounceKills = AnnouncePvPKills.Value;

            PvPSystem.isHonorSystemEnabled = EnableHonorSystem.Value;
            PvPSystem.isHonorTitleEnabled = EnableHonorTitle.Value;
            PvPSystem.MaxHonorGainPerSpan = MaxHonorGainPerSpan.Value;
            PvPSystem.SiegeDuration = HonorSiegeDuration.Value;
            PvPSystem.isHonorBenefitEnabled = EnableHonorBenefit.Value;
            PvPSystem.isEnableHostileGlow = EnableHostileGlow.Value;
            PvPSystem.isUseProximityGlow = UseProximityGlow.Value;

            PvPSystem.isLadderEnabled = EnablePvPLadder.Value;
            PvPSystem.LadderLength = PvPLadderLength.Value;
            PvPSystem.isSortByHonor = HonorSortLadder.Value;
            
            PvPSystem.isPunishEnabled = EnablePvPPunish.Value;
            PvPSystem.isAnnounceGrief = EnablePvPPunishAnnounce.Value;
            PvPSystem.isExcludeOffline = ExcludeOfflineKills.Value;
            PvPSystem.PunishLevelDiff = PunishLevelDiff.Value;
            PvPSystem.PunishDuration = PunishDuration.Value;
            PvPSystem.OffenseLimit = PunishOffenseLimit.Value;
            PvPSystem.Offense_Cooldown = PunishOffenseCooldown.Value;

            SiegeSystem.isSiegeBuff = BuffSiegeGolem.Value;
            SiegeSystem.GolemPDef.Value = GolemPhysicalReduction.Value;
            SiegeSystem.GolemSDef.Value = GolemSpellReduction.Value;


            // Debug logging
            Helper.buffLogging = buffLogging.Value;
            AutoSaveSystem.saveLogging = saveLogging.Value;
            Helper.deathLogging = deathLogging.Value;
            

            Helper.buffGUID = buffID.Value;
            Helper.appliedBuff = new PrefabGUID(appliedBuff.Value);
            Logger.LogInfo("Finished initialising");

            isInitialized = true;
        }

        private static bool parseLogging = false;
        public static int[] parseIntArrayConifg(string data) {
            if (parseLogging) Plugin.Logger.LogInfo(">>>parsing int array: " + data);
            var match = Regex.Match(data, "([0-9]+)");
            List<int> list = new List<int>();
            while (match.Success) {
                try {
                    if (parseLogging) Plugin.Logger.LogInfo(">>>got int: " + match.Value);
                    int temp = int.Parse(match.Value, CultureInfo.InvariantCulture);
                    if (parseLogging) Plugin.Logger.LogInfo(">>>int parsed into: " + temp);
                    list.Add(temp);
                }
                catch {
                    if (parseLogging) Plugin.Logger.LogWarning("Error interperting integer value: " + match.ToString());
                }
                match = match.NextMatch();
            }
            if (parseLogging) Plugin.Logger.LogInfo(">>>done parsing int array");
            int[] result = list.ToArray();
            return result;
        }
        public static float[] parseFloatArrayConifg(string data) {
            if (parseLogging) Plugin.Logger.LogInfo(">>>parsing float array: " + data);
            var match = Regex.Match(data, "[-+]?[0-9]*\\.?[0-9]+");
            List<float> list = new List<float>();
            while (match.Success) {
                try {
                    if (parseLogging) Plugin.Logger.LogInfo(">>>got float: " + match.Value);
                    float temp = float.Parse(match.Value, CultureInfo.InvariantCulture);
                    if (parseLogging) Plugin.Logger.LogInfo(">>>float parsed into: " + temp);
                    list.Add(temp);
                }
                catch {
                    Plugin.Logger.LogWarning("Error interperting float value: " + match.ToString());
                }

                match = match.NextMatch();
            }

            if (parseLogging) Plugin.Logger.LogInfo(">>>done parsing float array");
            float[] result = list.ToArray();
            return result;
        }
        public static double[] parseDoubleArrayConifg(string data) {
            if(parseLogging) Plugin.Logger.LogInfo(">>>parsing double array: " + data);
            var match = Regex.Match(data, "[-+]?[0-9]*\\.?[0-9]+");
            List<double> list = new List<double>();
            while (match.Success) {
                try {
                    if (parseLogging) Plugin.Logger.LogInfo(">>>got double: " + match.Value);
                    double temp = double.Parse(match.Value, CultureInfo.InvariantCulture);
                    if (parseLogging) Plugin.Logger.LogInfo(">>>double parsed into: " + temp);
                    list.Add(temp);
                }
                catch {
                    Plugin.Logger.LogWarning("Error interperting double value: " + match.ToString());
                }

                match = match.NextMatch();
            }

            if (parseLogging) Plugin.Logger.LogInfo(">>>done parsing double array");
            double[] result = list.ToArray();
            return result;
        }
        public static string[] parseStringArrayConifg(string data) {
            if (parseLogging) Plugin.Logger.LogInfo(">>>parsing comma seperated String array: " + data);
            List<string> list = new List<string>();
            while (data.IndexOf(",") > 0) {
                string str = data.Substring(0, data.IndexOf(","));
                str.Trim();
                list.Add(str);
                data = data.Substring(data.IndexOf(",") + 1);
            }
            data.Trim();
            list.Add(data);
            if (parseLogging) Plugin.Logger.LogInfo(">>>done parsing string array");
            string[] result = list.ToArray();
            return result;
        }
    }
}
