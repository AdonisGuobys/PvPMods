
using HarmonyLib;
using System;
using ProjectM;
using ProjectM.Network;
using PvPMods.Systems;
using PvPMods.Utils;
using Unity.Collections;
using Unity.Entities;

namespace PvPMods.Hooks {
    [HarmonyPatch]
    public class DeathEventListenerSystem_Patch {
        [HarmonyPatch(typeof(DeathEventListenerSystem), "OnUpdate")]
        [HarmonyPostfix]
        public static void Postfix(DeathEventListenerSystem __instance) {
            if (Helper.deathLogging) Plugin.Logger.LogInfo(DateTime.Now + ": beginning Death Tracking");
            NativeArray<DeathEvent> deathEvents = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
            if (Helper.deathLogging) Plugin.Logger.LogInfo(DateTime.Now + ": Death events converted successfully, length is " + deathEvents.Length);
            foreach (DeathEvent ev in deathEvents) {
                if (Helper.deathLogging) Plugin.Logger.LogInfo(DateTime.Now + ": Death Event occured");
                //-- Just track whatever died...
                //if (WorldDynamicsSystem.isFactionDynamic) WorldDynamicsSystem.MobKillMonitor(ev.Died);

                //-- Player Creature Kill Tracking
                var killer = ev.Killer;

                // If the entity killing is a minion, switch the killer to the owner of the minion.
                if (__instance.EntityManager.HasComponent<Minion>(killer)) {
                    if (Helper.deathLogging) Plugin.Logger.LogInfo($"{DateTime.Now}: Minion killed entity. Getting owner...");
                    if (__instance.EntityManager.TryGetComponentData<EntityOwner>(killer, out var entityOwner)) {
                        killer = entityOwner.Owner;
                        if (Helper.deathLogging) Plugin.Logger.LogInfo($"{DateTime.Now}: Owner found, switching killer to owner.");
                    }
                }

                if (__instance.EntityManager.HasComponent<PlayerCharacter>(killer) && __instance.EntityManager.HasComponent<Movement>(ev.Died)) {
                    if (Helper.deathLogging) Plugin.Logger.LogInfo(DateTime.Now + ": Killer is a player, running xp and heat and the like");
                    if (PvPSystem.isHonorSystemEnabled) PvPSystem.MobKillMonitor(killer, ev.Died);

                }
            }
        }
    }
}