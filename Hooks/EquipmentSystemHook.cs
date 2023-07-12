using HarmonyLib;
using ProjectM.Gameplay.Systems;
using Unity.Entities;
using Unity.Collections;
using ProjectM.Network;
using ProjectM;
using PvPMods.Systems;
using PvPMods.Utils;
using System;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.Assertions;

namespace PvPMods.Hooks
{

    [HarmonyPatch(typeof(ArmorLevelSystem_Spawn), nameof(ArmorLevelSystem_Spawn.OnUpdate))]
    public class ArmorLevelSystem_Spawn_Patch
    {

        private static void Postfix(ArmorLevelSystem_Spawn __instance)
        {
            //if (__instance.__OnUpdate_LambdaJob0_entityQuery == null) return;

            if (PvPSystem.isPunishEnabled)
            {
                NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);

                foreach (var entity in entities)
                {
                    Entity Owner = __instance.EntityManager.GetComponentData<EntityOwner>(entity).Owner;
                    if (!__instance.EntityManager.HasComponent<PlayerCharacter>(Owner)) return;
                    if (PvPSystem.isPunishEnabled) PvPSystem.OnEquipChange(Owner);
                }
            }
        }
    }

    [HarmonyPatch(typeof(WeaponLevelSystem_Spawn), nameof(WeaponLevelSystem_Spawn.OnUpdate))]
    public class WeaponLevelSystem_Spawn_Patch
    {

        private static void Postfix(WeaponLevelSystem_Spawn __instance)
        {
            //if (__instance.__OnUpdate_LambdaJob0_entityQuery == null) return;

            if (PvPSystem.isPunishEnabled)
            {
                NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    Entity Owner = __instance.EntityManager.GetComponentData<EntityOwner>(entity).Owner;
                    if (!__instance.EntityManager.HasComponent<PlayerCharacter>(Owner)) return;
                    if (PvPSystem.isPunishEnabled) PvPSystem.OnEquipChange(Owner);
                }
            }
        }
    }


        [HarmonyPatch(typeof(SpellLevelSystem_Spawn), nameof(SpellLevelSystem_Spawn.OnUpdate))]
    public class SpellLevelSystem_Spawn_Patch
    {

        private static void Postfix(SpellLevelSystem_Spawn __instance)
        {
            //if (__instance.__OnUpdate_LambdaJob0_entityQuery == null) return;

            if (PvPSystem.isPunishEnabled)
            {
                NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    Entity Owner = __instance.EntityManager.GetComponentData<EntityOwner>(entity).Owner;
                    if (!__instance.EntityManager.HasComponent<PlayerCharacter>(Owner)) return;
                    if (PvPSystem.isPunishEnabled) PvPSystem.OnEquipChange(Owner);
                }
            }
        }
    }
}