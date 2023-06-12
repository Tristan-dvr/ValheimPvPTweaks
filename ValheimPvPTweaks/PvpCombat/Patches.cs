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

        [HarmonyPrefix, HarmonyPatch(typeof(Player), nameof(Player.ToggleEquipped))]
        private static bool Player_ToggleEquipped(Player __instance, ItemDrop.ItemData item, ref bool __result)
        {
            if (!__instance.IsLocalPlayer() 
                || !__instance.InCombat() 
                || !item.IsEquipable() 
                || Plugin.Configuration.AllowChangeEquipmentInCombat.Value)
                return true;

            switch (item.m_shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.Chest:
                case ItemDrop.ItemData.ItemType.Hands:
                case ItemDrop.ItemData.ItemType.Helmet:
                case ItemDrop.ItemData.ItemType.Legs:
                case ItemDrop.ItemData.ItemType.Shoulder:
                case ItemDrop.ItemData.ItemType.Utility:
                    __instance.Message(MessageHud.MessageType.Center, "$vpo_msg_noequip_combat");
                    __result = true;
                    return false;
                default:
                    return true;
            }
        }
    }
}
