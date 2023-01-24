using HarmonyLib;

namespace ValheimPvPTweaks.KillFeed
{
    [HarmonyPatch]
    class Patches
    {
        private static KillFeed _killFeed;

        [HarmonyPostfix, HarmonyPatch(typeof(Character), nameof(Character.Start))]
        private static void Character_Start(Character __instance)
        {
            __instance.gameObject.AddComponent<CharacterKillTracker>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Game), nameof(Game.Start))]
        private static void Game_Start(Game __instance)
        {
            _killFeed = __instance.gameObject.AddComponent<KillFeed>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
        private static void ZNet_Disconnect(Game __instance, ZNetPeer peer)
        {
            _killFeed.OnPlayerDisconnected(peer);
        }
    }
}
