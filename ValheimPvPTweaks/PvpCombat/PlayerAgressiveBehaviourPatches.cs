using HarmonyLib;
using UnityEngine;

namespace ValheimPvPTweaks.PvpCombat
{
    [HarmonyPatch]
    class PlayerAgressiveBehaviourPatches
    {
        [HarmonyFinalizer, HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        public static void Character_Damage(Character __instance, HitData hit)
        {
            if (__instance.IsTamed() && IsAgressiveLocalPlayerHit(hit) && !PrivateArea.CheckAccess(__instance.transform.position))
                PlayerCombatHandler.EnterCombat(Player.m_localPlayer);
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(Destructible), nameof(Destructible.Damage))]
        [HarmonyPatch(typeof(MineRock), nameof(MineRock.Damage))]
        [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Damage))]
        [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.Damage))]
        [HarmonyPatch(typeof(TreeLog), nameof(TreeLog.Damage))]
        public static void Destructible_Damage(Component __instance, HitData hit)
        {
            if (IsAgressiveLocalPlayerHit(hit) && !PrivateArea.CheckAccess(__instance.transform.position))
                PlayerCombatHandler.EnterCombat(Player.m_localPlayer);
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Damage))]
        public static void WearNTear_Damage(Component __instance, HitData hit)
        {
            if (hit.HaveAttacker() && hit.GetAttacker().IsLocalPlayer() && !PrivateArea.CheckAccess(__instance.transform.position))
                PlayerCombatHandler.EnterCombat(Player.m_localPlayer);
        }

        [HarmonyFinalizer, HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
        public static void Pickable_Interact(Pickable __instance, Humanoid character, ref bool __result)
        {
            if (character.IsLocalPlayer() && !PrivateArea.CheckAccess(__instance.transform.position) && __result)
                PlayerCombatHandler.EnterCombat(character);
        }

        private static bool IsAgressiveLocalPlayerHit(HitData hit)
        {
            return hit.HaveAttacker()
                && hit.GetTotalDamage() > 0
                && hit.GetAttacker() == Player.m_localPlayer;
        }
    }
}
