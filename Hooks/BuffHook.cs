using System;
using HarmonyLib;
using Unity.Entities;
using Unity.Collections;
using ProjectM.Network;
using ProjectM;
using PvPMods.Utils;
using PvPMods.Systems;
using System.Collections.Generic;
using ProjectM.Scripting;

namespace PvPMods.Hooks
{
    [HarmonyPatch(typeof(ModifyUnitStatBuffSystem_Spawn), nameof(ModifyUnitStatBuffSystem_Spawn.OnUpdate))]
    public class ModifyUnitStatBuffSystem_Spawn_Patch
    {
        #region GodMode & Other Buff
        private static ModifyUnitStatBuff_DOTS Cooldown = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.CooldownModifier,
            Value = 0,
            ModificationType = ModificationType.Set,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS SunCharge = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.SunChargeTime,
            Value = 50000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS Hazard = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.ImmuneToHazards,
            Value = 1,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS SunResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.SunResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS Speed = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.MovementSpeed,
            Value = 15,
            ModificationType = ModificationType.Set,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS PResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.PhysicalResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS FResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.FireResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS HResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.HolyResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS SResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.SilverResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS GResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.GarlicResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS SPResist = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.SpellResistance,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS PPower = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.PhysicalPower,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS RPower = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.ResourcePower,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS SPPower = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.SpellPower,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS PHRegen = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.PassiveHealthRegen,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS HRecovery = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.HealthRecovery,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS MaxHP = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.MaxHealth,
            Value = 10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS MaxYield = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.ResourceYield,
            Value = 10,
            ModificationType = ModificationType.Multiply,
            Id = ModificationId.NewId(0)
        };

        private static ModifyUnitStatBuff_DOTS DurabilityLoss = new ModifyUnitStatBuff_DOTS()
        {
            StatType = UnitStatType.ReducedResourceDurabilityLoss,
            Value = -10000,
            ModificationType = ModificationType.Add,
            Id = ModificationId.NewId(0)
        };
        #endregion

        private static void Prefix(ModifyUnitStatBuffSystem_Spawn __instance)
        {
            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Entered Buff System, attempting Old Style");
            oldStyleBuffHook(__instance);
            //if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Old Style Done, attemping New Style, just cause");
            //rebuiltBuffHook(__instance);
        }

        public static void oldStyleBuffApplicaiton(Entity entity, EntityManager entityManager) {

            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Applying RPGMods Buffs");
            Entity Owner = entityManager.GetComponentData<EntityOwner>(entity).Owner;
            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Owner found, hash: " + Owner.GetHashCode());
            if (!entityManager.HasComponent<PlayerCharacter>(Owner)) return;

            PlayerCharacter playerCharacter = entityManager.GetComponentData<PlayerCharacter>(Owner);
            Entity User = playerCharacter.UserEntity/*._Entity*/;
            User Data = entityManager.GetComponentData<User>(User);

            var Buffer = entityManager.GetBuffer<ModifyUnitStatBuff_DOTS>(entity);
            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Buffer acquired, length: " + Buffer.Length);

            //Buffer.Clear();
            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Buffer cleared, to confirm length: " + Buffer.Length);

            
            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Done Adding, Buffer length: " + Buffer.Length);

        }

        public static void oldStyleBuffHook(ModifyUnitStatBuffSystem_Spawn __instance)
        {
            //if (__instance.__OnUpdate_LambdaJob0_entityQuery == null) return;

            EntityManager entityManager = __instance.EntityManager;
            NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);

            if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": Entities Length of " + entities.Length);
            foreach (var entity in entities){
                PrefabGUID GUID = entityManager.GetComponentData<PrefabGUID>(entity);
                if (Helper.buffLogging) Plugin.Logger.LogInfo(System.DateTime.Now + ": GUID of " + GUID.GuidHash);
                //if (GUID.Equals(Database.Buff.Buff_VBlood_Perk_Moose) || GUID.GuidHash == -1465458722)
                if(GUID.GuidHash == Helper.buffGUID)
                {
                    oldStyleBuffApplicaiton(entity, entityManager);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BuffSystem_Spawn_Server), nameof(BuffSystem_Spawn_Server.OnUpdate))]
    public class BuffSystem_Spawn_Server_Patch {
        public static bool buffLogging = false;
        private static void Prefix(BuffSystem_Spawn_Server __instance)
        {
            if (PvPSystem.isPunishEnabled || SiegeSystem.isSiegeBuff || PermissionSystem.isVIPSystem || PvPSystem.isHonorSystemEnabled)
            {
                NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    PrefabGUID GUID = __instance.EntityManager.GetComponentData<PrefabGUID>(entity);
                    //if (WeaponMasterSystem.isMasteryEnabled) WeaponMasterSystem.BuffReceiver(entities[i], GUID);
                    if (PvPSystem.isHonorSystemEnabled) PvPSystem.HonorBuffReceiver(entity, GUID);
                    if (PermissionSystem.isVIPSystem) PermissionSystem.BuffReceiver(entity, GUID);
                    if (PvPSystem.isPunishEnabled) PvPSystem.BuffReceiver(entity, GUID);
                    if (SiegeSystem.isSiegeBuff) SiegeSystem.BuffReceiver(entity, GUID);
                }
            }
        }

        private static void Postfix(BuffSystem_Spawn_Server __instance)
        {

            if (PvPSystem.isPunishEnabled)
            {
                NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    if (!__instance.EntityManager.HasComponent<InCombatBuff>(entity)) continue;
                    Entity e_Owner = __instance.EntityManager.GetComponentData<EntityOwner>(entity).Owner;
                    if (!__instance.EntityManager.HasComponent<PlayerCharacter>(e_Owner)) continue;
                    Entity e_User = __instance.EntityManager.GetComponentData<PlayerCharacter>(e_Owner).UserEntity;
                    
                    if (PvPSystem.isPunishEnabled) PvPSystem.OnCombatEngaged(entity, e_Owner);
                }
            }
        }
    }
    


    [HarmonyPatch(typeof(ModifyBloodDrainSystem_Spawn), nameof(ModifyBloodDrainSystem_Spawn.OnUpdate))]
    public class ModifyBloodDrainSystem_Spawn_Patch
    {
        private static void Prefix(ModifyBloodDrainSystem_Spawn __instance)
        {

            if (PermissionSystem.isVIPSystem || PvPSystem.isHonorSystemEnabled)
            {
                NativeArray<Entity> entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    PrefabGUID GUID = __instance.EntityManager.GetComponentData<PrefabGUID>(entity);
                    //if (WeaponMasterSystem.isMasteryEnabled) WeaponMasterSystem.BuffReceiver(entities[i], GUID);
                    if (PermissionSystem.isVIPSystem) PermissionSystem.BuffReceiver(entity, GUID);
                    if (PvPSystem.isHonorSystemEnabled) PvPSystem.HonorBuffReceiver(entity, GUID);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Destroy_TravelBuffSystem), nameof(Destroy_TravelBuffSystem.OnUpdate))]
    public class Destroy_TravelBuffSystem_Patch
    {
        private static void Postfix(Destroy_TravelBuffSystem __instance)
        {
            var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                PrefabGUID GUID = __instance.EntityManager.GetComponentData<PrefabGUID>(entity);
                //-- Most likely it's a new player!
                if (GUID.Equals(Database.Buff.AB_Interact_TombCoffinSpawn_Travel))
                {
                    var Owner = __instance.EntityManager.GetComponentData<EntityOwner>(entity).Owner;
                    if (!__instance.EntityManager.HasComponent<PlayerCharacter>(Owner)) return;

                    var userEntity = __instance.EntityManager.GetComponentData<PlayerCharacter>(Owner).UserEntity;
                    var playerName = __instance.EntityManager.GetComponentData<User>(userEntity).CharacterName.ToString();

                    if (PvPSystem.isHonorSystemEnabled) PvPSystem.NewPlayerReceiver(userEntity, Owner, playerName);
                    else Helper.UpdatePlayerCache(userEntity, playerName, playerName);
                }
            }
        }
    }
}