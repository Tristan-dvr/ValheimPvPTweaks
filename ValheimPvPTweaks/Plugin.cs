using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using ServerSync;
using System;
using System.Linq;
using System.Reflection;

namespace ValheimPvPTweaks
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(Guid, Name, Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string Guid = "org.tristan.pvptweaks";
        public const string Name = "Valheim PvP Tweaks";
        public const string Version = "1.0.16";

        internal static Configuration Configuration { get; private set; }
        internal static ConfigSync Sync { get; private set; }

        private void Awake()
        {
            Sync = new ConfigSync(Guid)
            {
                DisplayName = Name,
                CurrentVersion = Version,
                MinimumRequiredVersion = Version,
            };
            Configuration = new Configuration(Config, Sync);
            Log.CreateInstance(Logger);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Guid);

            LoadLocalization();

            SynchronizationManager.OnAdminStatusChanged += OnAdminStatusChanged;
        }

        private void LoadLocalization()
        {
            var localization = LocalizationManager.Instance.GetLocalization();
            localization.AddJsonFile("English", Helper.LoadTextFromResources("English.json"));
            localization.AddJsonFile("Russian", Helper.LoadTextFromResources("Russian.json"));
        }

        private static void OnAdminStatusChanged()
        {
            var consoleMode = Configuration.Console.Value;
            var isAdmin = SynchronizationManager.Instance.PlayerIsAdmin || ZNet.instance.IsServer();
            Console.SetConsoleEnabled(consoleMode == Configuration.ConsoleMode.Enabled
                || consoleMode == Configuration.ConsoleMode.AdminOnly && isAdmin);
            Log.Debug($"Admin status changed {isAdmin}");
        }

        private static void RefreshStatusEffects(Configuration config)
        {
            SetStatusEffectConfig(config.BonemassConfig.Value, "GP_Bonemass");
            SetStatusEffectConfig(config.YagluthConfig.Value, "GP_Yagluth");
            SetStatusEffectConfig(config.EikthyrConfig.Value, "GP_Eikthyr");
            SetStatusEffectConfig(config.ModerConfig.Value, "GP_Moder");
            SetStatusEffectConfig(config.ElderConfig.Value, "GP_TheElder"); 
            SetStatusEffectConfig(config.SeekerQueenConfig.Value, "GP_Queen");
            Log.Debug("Boss powers updated");
        }

        private static void RefreshSwordsSecondaryAttack()
        {
            var damageMultiplier = Configuration.SwordsSecondaryAttackDamage.Value;
            var excludedPrefabs = Configuration.ExcludedSwordsPrefabs.Value.Split(',');
            var includedPrefabs = Configuration.IncludedSwordPrefabs.Value.Split(',');

            foreach (var item in ObjectDB.instance.m_items
                .Select(i => i.GetComponent<ItemDrop>())
                .Where(i => IsSword(i) || includedPrefabs.Contains(i.name))
                .Where(i => !excludedPrefabs.Contains(i.name)))
            {
                if (damageMultiplier > 0)
                {
                    Log.Debug($"{item.name} secondary attack damage changed {item.m_itemData.m_shared.m_secondaryAttack.m_damageMultiplier}->{damageMultiplier}");
                    item.m_itemData.m_shared.m_secondaryAttack.m_damageMultiplier = damageMultiplier;
                }
            }
        }

        private static void UpdateStaffsDamage(Configuration configuration)
        {
            SetStaffConfig("StaffFireball", 
                configuration.StaffFireDamage.Value, 
                configuration.StaffFireEitr.Value,
                (hit, d) =>
                {
                    hit.m_blunt = d;
                    hit.m_fire = d;
                    return hit;
                }, (hit, d) =>
                {
                    hit.m_fire = d;
                    return hit;
                });
            SetStaffConfig("StaffIceShards", configuration.StaffIceDamage.Value, 
                configuration.StaffIceEitr.Value,
                (hit, d) =>
                {
                    hit.m_frost = d;
                    return hit;
                }, (hit, d) =>
                {
                    hit.m_frost = d;
                    return hit;
                });
            SetShieldStaffConfig("StaffShield", configuration.StaffShield.Value, configuration.StaffShieldEitr.Value);
            Log.Debug("Staffs updated");
        }

        private static void SetShieldStaffConfig(string name, string config, int eitrConsumption)
        {
            var staff = ObjectDB.instance.GetItemPrefab(name);
            if (staff != null && staff.TryGetComponent<ItemDrop>(out var item))
            {
                if (TryParseTwoValues(config, out var absorbDamage, out var maxAbsorbValue))
                {
                    var se = item.m_itemData.m_shared.m_attackStatusEffect as SE_Shield;
                    se.m_absorbDamage = absorbDamage;
                    var absorbValuePerSkillLevel = (maxAbsorbValue - absorbDamage) / 100f;
                    se.m_absorbDamagePerSkillLevel = absorbValuePerSkillLevel;
                    Log.Debug($"Staff shield updated {se.m_absorbDamage}:{se.m_absorbDamagePerSkillLevel}");
                }
                else if (!string.IsNullOrEmpty(config))
                {
                    Log.Warning($"Cannot update shield staff values. Cannot parse config {config}");
                }

                if (eitrConsumption > 0)
                    item.m_itemData.m_shared.m_attack.m_attackEitr = eitrConsumption;
            }
        }

        private static void SetStatusEffectConfig(string config, string effectName)
        {
            var effect = ObjectDB.instance.GetStatusEffect(effectName.GetStableHashCode());
            if (TryParseTwoValues(config, out var duration, out var cooldown) && effect != null)
            {
                effect.m_ttl = duration;
                effect.m_cooldown = cooldown;
            }
            else if (!string.IsNullOrEmpty(config))
            {
                Log.Warning($"Error updating {effectName} - {effect}");
            }
        }

        private static bool IsSword(ItemDrop item)
        {
            var skillType = item.m_itemData.m_shared.m_skillType;
            var itemType = item.m_itemData.m_shared.m_itemType;
            return skillType == Skills.SkillType.Swords
                && (itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon || itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon)
                && item.m_itemData.m_shared.m_secondaryAttack.m_attackType != Attack.AttackType.None
                && HasRecipe(item);
        }

        private static void SetStaffConfig(string name, 
            string config,
            int eitrConsumption,
            Func<HitData.DamageTypes, int, HitData.DamageTypes> onSetDamage,
            Func<HitData.DamageTypes, int, HitData.DamageTypes> onSetDamagePerLevel)
        {
            var staff = ObjectDB.instance.GetItemPrefab(name);
            if (staff != null && staff.TryGetComponent<ItemDrop>(out var item))
            {
                if (TryParseTwoValues(config, out var damage, out var damagePerLevel))
                {
                    item.m_itemData.m_shared.m_damages = onSetDamage.Invoke(item.m_itemData.m_shared.m_damages, damage);
                    item.m_itemData.m_shared.m_damagesPerLevel = onSetDamagePerLevel.Invoke(item.m_itemData.m_shared.m_damagesPerLevel, damagePerLevel);
                    Log.Debug($"{name} updated damage {damage}:{damagePerLevel}");
                }
                else if (!string.IsNullOrEmpty(config))
                {
                    Log.Warning($"Cannot update staff damage {name}, cannot parse config {config}");
                }
                if (eitrConsumption > 0)
                    item.m_itemData.m_shared.m_attack.m_attackEitr = eitrConsumption;
            }
        }

        private static void UpdateArbalestDamage(Configuration configuration)
        {
            var prefabName = "CrossbowArbalest";
            var config = configuration.CrossbowDamage.Value;
            if (TryParseTwoValues(config, out var damage, out var damagePerLevel))
            {
                var item = ZNetScene.instance.GetPrefabComponent<ItemDrop>(prefabName);

                var damages = item.m_itemData.m_shared.m_damages;
                damages.m_pierce = damage;
                item.m_itemData.m_shared.m_damages = damages;

                damages = item.m_itemData.m_shared.m_damagesPerLevel;
                damages.m_pierce = damagePerLevel;
                item.m_itemData.m_shared.m_damagesPerLevel = damages;
                Log.Debug($"{prefabName} updated damage {damage}:{damagePerLevel}");
            }
            else if (!string.IsNullOrEmpty(config))
            {
                Log.Warning($"Cannot update crossbow damage {prefabName}, cannot parse config {config}");
            }
        }

        private static bool HasRecipe(ItemDrop item)
        {
            return ObjectDB.instance.m_recipes.Any(r => r.m_item == item);
        }

        private static bool TryParseTwoValues(string text, out int value1, out int value2)
        {
            try
            {
                var split = text.Split(':');
                value1 = int.Parse(split[0]);
                value2 = int.Parse(split[1]);
                return true;
            }
            catch
            {
                value1 = value2 = default;
                return false;
            }
        }

        private static void ApplyConfiguration()
        {
            if (ZNet.instance.IsDedicated())
                return;

            RefreshSwordsSecondaryAttack();
            RefreshStatusEffects(Configuration);
            UpdateStaffsDamage(Configuration);
            UpdateArbalestDamage(Configuration);
            Log.Message("Config applied");
        }

        [HarmonyPatch]
        private class ApplyConfigPatch
        {
            [HarmonyFinalizer]
            [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_PeerInfo))]
            [HarmonyPatch(typeof(ZNet), nameof(ZNet.Start))]
            private static void ZNet_Finalizers(ZNet __instance)
            {
                ApplyConfiguration();
            }
        }

        [HarmonyPatch]
        private class BossPowerPatch
        {
            //  Reset power cooldown after changing power
            [HarmonyPostfix, HarmonyPatch(typeof(ItemStand), nameof(ItemStand.DelayedPowerActivation))]
            private static void ItemStand_DelayedPowerActivation()
            {
                if (Player.m_localPlayer != null && Configuration.ResetBossPowerOnChange.Value)
                    Player.m_localPlayer.m_guardianPowerCooldown = 0;
            }
        }
    }
}
