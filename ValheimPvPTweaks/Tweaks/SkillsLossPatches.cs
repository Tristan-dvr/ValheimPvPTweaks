using HarmonyLib;

namespace ValheimPvPTweaks.Tweaks
{
    [HarmonyPatch]
    class SkillsLossPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(Skills), nameof(Skills.OnDeath))]
        private static bool Skills_OnDeath()
        {
            var skillLossType = Plugin.Configuration.SkillsLoss.Value;
            switch (skillLossType)
            {
                case Configuration.SkillLossType.NoLoss:
                    return false;
                case Configuration.SkillLossType.NoLossInPvp:
                    return !Player.m_localPlayer.InCombat();
                case Configuration.SkillLossType.Vanilla:
                    return true;
                default:
                    return true;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player), nameof(Player.OnDeath))]
        private static void Player_OnDeath(Player __instance)
        {
            var skillLossType = Plugin.Configuration.SkillsLoss.Value;
            if (skillLossType == Configuration.SkillLossType.NoLoss 
                || (skillLossType == Configuration.SkillLossType.NoLossInPvp && __instance.InCombat()))
            {
                __instance.m_timeSinceDeath = __instance.m_hardDeathCooldown;
            }
        }
    }
}
