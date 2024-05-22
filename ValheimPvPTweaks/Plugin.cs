using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using ServerSync;
using System;
using System.Reflection;

namespace ValheimPvPTweaks
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(Guid, Name, Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string Guid = "org.tristan.pvptweaks";
        public const string Name = "Valheim PvP Tweaks";
        public const string Version = "1.1.0";

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
            SetStatusEffectConfig(config.FaderConfig.Value, "GP_Fader");
            Log.Debug("Boss powers updated");
        }

        private static void SetShieldStaffConfig(string name, string config)
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

        private static void ApplyConfiguration()
        {
            if (ZNet.instance.IsDedicated())
                return;

            RefreshStatusEffects(Configuration);
            SetShieldStaffConfig("StaffShield", Configuration.StaffShieldAbsorbtionConfig.Value);
            SetSiegeItemsConfiguration(Configuration);
            Log.Message("Config applied");
        }

        private static void SetSiegeItemsConfiguration(Configuration configuration)
        {
            if (TryParseDamage(configuration.SiegeBombDamageConfig.Value, out var siegeBombDamage))
            {
                var siegeBombAoe = ZNetScene.instance.GetPrefabComponent<Aoe>("siegebomb_explosion");
                siegeBombAoe.m_damage = siegeBombDamage;
            }
            else if (!string.IsNullOrEmpty(configuration.SiegeBombDamageConfig.Value))
            {
                Log.Error("Cannot parse damage for Siege bomb");
            }

            if (TryParseDamage(configuration.RamDamageConfig.Value, out var ramMachineDamage))
            {
                var ram = ZNetScene.instance.GetPrefabComponent<SiegeMachine>("BatteringRam");
                foreach (var aoe in ram.m_aoe.GetComponentsInChildren<Aoe>(true))
                {
                    aoe.m_damage = ramMachineDamage;
                }
            }
            else if (!string.IsNullOrEmpty(configuration.RamDamageConfig.Value))
            {
                Log.Error("Cannot parse damage for Ram");
            }
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

        private static bool TryParseDamage(string text, out HitData.DamageTypes damage)
        {
            try
            {
                var split = text.Split(':');
                damage = new HitData.DamageTypes
                {
                    m_pickaxe = int.Parse(split[0]),
                    m_chop = int.Parse(split[1]),
                    m_damage = int.Parse(split[2]),
                    m_blunt = int.Parse(split[3]),
                    m_pierce = int.Parse(split[4]),
                    m_slash = int.Parse(split[5]),
                    m_fire = int.Parse(split[6]),
                    m_frost = int.Parse(split[7]),
                    m_lightning = int.Parse(split[8]),
                    m_poison = int.Parse(split[9]),
                    m_spirit = int.Parse(split[10]),
                };
                return true;
            }
            catch
            {
                damage = default;
                return false;
            }
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
