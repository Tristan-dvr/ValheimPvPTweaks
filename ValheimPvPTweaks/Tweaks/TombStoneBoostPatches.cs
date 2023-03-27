using HarmonyLib;

namespace ValheimPvPTweaks.Tweaks
{
    [HarmonyPatch(typeof(TombStone))]
    class TombStoneBoostPatches
    {
        //  Disabled effect after looting tombstone
        [HarmonyPrefix, HarmonyPatch(nameof(TombStone.GiveBoost))]
        private static bool TombStone_GiveBoost() => Plugin.Configuration.EnableTombStoneBoost.Value;
    }
}
