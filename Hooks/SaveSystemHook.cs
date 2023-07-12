using HarmonyLib;
using ProjectM;
using PvPMods.Utils;

namespace PvPMods.Hooks
{
    [HarmonyPatch(typeof(TriggerPersistenceSaveSystem), nameof(TriggerPersistenceSaveSystem.TriggerSave))]
    public class TriggerPersistenceSaveSystem_Patch
    {
        public static void Postfix()
        {
            AutoSaveSystem.SaveDatabase();
        }
    }
}
