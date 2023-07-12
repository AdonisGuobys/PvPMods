using HarmonyLib;
using ProjectM;
using System.Text.Json;
using System.IO;
using PvPMods.Systems;
using PvPMods.Utils;
using Unity.Collections;
using Unity.Entities;
using System.Collections.Generic;
using System;

namespace PvPMods.Hooks
{
    [HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
    public class VampireDownedServerEventSystem_Patch
    {
        public static void Postfix(VampireDownedServerEventSystem __instance)
        {
            //if (__instance.__OnUpdate_LambdaJob0_entityQuery == null) return;

            EntityManager em = __instance.EntityManager;
            var EventsQuery = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);

            foreach(var entity in EventsQuery)
            {
                VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, em, out var Victim);

                try {
                    em.TryGetComponentData<VampireDownedBuff>(entity, out VampireDownedBuff deathBuff);

                    //Entity Source = em.GetComponentData<VampireDownedBuff>(entity).Source;
                    Entity Source = deathBuff.Source;
                    VampireDownedServerEventSystem.TryFindRootOwner(Source, 1, em, out var Killer);

                    //-- Update PvP Stats & Check
                    if (em.HasComponent<PlayerCharacter>(Killer) && em.HasComponent<PlayerCharacter>(Victim) && !Killer.Equals(Victim)){
                        PvPSystem.Monitor(Killer, Victim);
                        if (PvPSystem.isPunishEnabled) PvPSystem.PunishCheck(Killer, Victim);
                    }
                }
                catch {
                    // The above code currently results in the following error:
                    //
                    // Attempting to call method 'Unity.Entities.EntityManager::GetComponentData<ProjectM.VampireDownedBuff>'
                    // for which no ahead of time (AOT) code was generated.
                    //
                    // As PvP support is not yet complete, ignore this error as it is not required for PvE support.
                }
            }
        }

    }
}
