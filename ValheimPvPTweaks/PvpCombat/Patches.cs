using HarmonyLib;

namespace ValheimPvPTweaks.PvpCombat
{
    [HarmonyPatch]
    class Patches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        private static void Player_OnSpawned(Player __instance)
        {
            __instance.gameObject.AddComponent<PlayerCombatHandler>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        private static void ObjectDB_Awake(ObjectDB __instance)
        {
            __instance.m_StatusEffects.Add(SE_Combat.Create());
        }

        [HarmonyPostfix, HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Awake))]
        private static void InventoryGui_Awake(InventoryGui __instance)
        {
            var combat = ObjectDB.instance.GetStatusEffect(SE_Combat.Name);
            combat.m_icon = __instance.m_pvp.GetComponent<ToggleImage>().m_onImage;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Humanoid), nameof(Humanoid.IsTeleportable))]
        private static void Player_IsTeleportable(Humanoid __instance, ref bool __result)
        {
            if (!Plugin.Configuration.AllowTeleportInCombat.Value && __instance is Player player)
                __result &= !player.InCombat();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Teleport))]
        private static bool TeleportWorld_Teleport(TeleportWorld __instance, Player player)
        {
            if (!Plugin.Configuration.AllowTeleportInCombat.Value
                && __instance.TargetFound() 
                && player != null 
                && player.InCombat())
            {
                player.Message(MessageHud.MessageType.Center, "$vpo_msg_noteleport_combat");
                return false;
            }
            return true;
        }
    }
}
