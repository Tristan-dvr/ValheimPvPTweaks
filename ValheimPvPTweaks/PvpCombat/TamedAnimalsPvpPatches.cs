using HarmonyLib;
using UnityEngine;

namespace ValheimPvPTweaks.PvpCombat
{
    [HarmonyPatch]
    class TamedAnimalsPvpPatches
    {
        [HarmonyFinalizer, HarmonyPatch(typeof(Character), nameof(Character.Start))]
        private static void Character_Start(Character __instance)
        {
            if (__instance.IsTamed())
                __instance.gameObject.AddComponent<TamedAnimalCombatHandler>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        private static void Character_Damage_Prefix(Character __instance, HitData hit)
        {
            if (__instance.IsTamed()
                && hit.GetAttacker() is Player player
                && __instance.TryGetComponent<TamedAnimalCombatHandler>(out var handler)
                && !CanTakeControl(handler, player)
                && player.IsLocalPlayer()
                && player.GetCurrentWeapon().m_shared.m_tamedOnly)
            {
                hit.ClearDamage();
            }
        }

        [HarmonyFinalizer, HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        private static void Character_Damage(Character __instance, HitData hit)
        {
            if (hit.GetAttacker() is Player player && hit.GetTotalDamage() > 0)
                TamedAnimalCombatHandler.OnPlayerAttackedCharacter(__instance, player);
        }

        [HarmonyFinalizer, HarmonyPatch(typeof(Character), nameof(Character.SetTamed))]
        private static void Character_SetTamed(Character __instance, bool tamed)
        {
            var hasComponent = __instance.TryGetComponent<TamedAnimalCombatHandler>(out var component);
            if (hasComponent != tamed)
            {
                if (tamed)
                    __instance.gameObject.AddComponent<TamedAnimalCombatHandler>();
                else
                    GameObject.Destroy(component);
            }

            __instance.SetupMaxHealth();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Tameable), nameof(Tameable.RPC_Command))]
        private static void Tameable_RPC_Command(Tameable __instance)
        {
            if (__instance.TryGetComponent<TamedAnimalCombatHandler>(out var handler))
                handler.RefreshFollowingPlayer();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
        private static bool Tameable_Interact(Character __instance, Humanoid user)
        {
            if (__instance.TryGetComponent<TamedAnimalCombatHandler>(out var handler)
                && !CanTakeControl(handler, user))
            {
                user.Message(MessageHud.MessageType.Center, "$piece_noaccess");
                return false;
            }
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]
        private static void Tameable_GetHoverText_Postfix(Character __instance, ref string __result)
        {
            if (__instance.TryGetComponent<TamedAnimalCombatHandler>(out var handler))
            {
                if (!CanTakeControl(handler, Player.m_localPlayer))
                    __result += $"\n$piece_noaccess".Localize();
                if (handler.IsDefendingArea())
                    __result += "\n$vpo_tamed_msg_defending".Localize();
                if (handler.IsFollowingPlayer())
                    __result += $"\n$vpo_tamed_msg_following <color=yellow>{handler.GetFollowPlayer().GetPlayerName()}</color>".Localize();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(BaseAI), nameof(BaseAI.IsEnemy), typeof(Character), typeof(Character))]
        private static void BaseAI_IsEnemey(Character a, Character b, ref bool __result)
        {
            if (__result || !Plugin.Configuration.CreaturesProtectWard.Value)
                return;

            Character tamed;
            Character other;
            if (a.IsTamed())
            {
                tamed = a;
                other = b;
            }
            else if (b.IsTamed())
            {
                tamed = b;
                other = a;
            }
            else
            {
                return;
            }
            if (tamed.TryGetComponent<TamedAnimalCombatHandler>(out var handler))
            {
                if (other is Player player)
                {
                    __result |= handler.CheckIsEnemy(player);
                }
                else if (other.IsTamed() && other.TryGetComponent<TamedAnimalCombatHandler>(out var otherHandler))
                {
                    __result |= handler.CheckIsEnemy(otherHandler);
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Attack), nameof(Attack.GetLevelDamageFactor))]
        private static void Attack_GetLevelDamageFactor(Attack __instance, ref float __result)
        {
            if (__instance.m_character != null && __instance.m_character.IsTamed())
                __result *= Plugin.Configuration.TamedCreaturesDamageMultiplier.Value;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Character), nameof(Character.SetMaxHealth))]
        private static void Character_SetMaxHealth(Character __instance, ref float health)
        {
            if (__instance.IsTamed())
                health *= Plugin.Configuration.TamedCreaturesHealthMultiplier.Value;
        }

        private static bool CanTakeControl(TamedAnimalCombatHandler animal, Character character)
        {
            if (animal.GetFollowPlayer() == character)
                return true;

            if (animal.IsFollowingPlayer() || !PrivateArea.CheckAccess(animal.transform.position, flash: false))
                return false;

            return true;
        }
    }
}
