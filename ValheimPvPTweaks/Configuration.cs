using BepInEx.Configuration;
using ServerSync;

namespace ValheimPvPTweaks
{
    class Configuration
    {
        public enum ConsoleMode { Disabled, AdminOnly, Enabled };
        public enum SkillLossType { Vanilla, NoLossInPvp, NoLoss }

        public ConfigEntry<int> MaxDeathPingRadius;
        public ConfigEntry<int> CombatDuration;
        public ConfigEntry<float> CombatHealthRegen;
        public ConfigEntry<bool> AllowTeleportInCombat;
        public ConfigEntry<bool> AllowChangeEquipmentInCombat;
        public ConfigEntry<bool> CreaturesProtectWard;
        public ConfigEntry<bool> FollowCreaturesProtectWard;
        public ConfigEntry<float> TamedCreaturesHealthMultiplier;
        public ConfigEntry<float> TamedCreaturesDamageMultiplier;

        public ConfigEntry<bool> ResetBossPowerOnChange;
        public ConfigEntry<string> BonemassConfig;
        public ConfigEntry<string> EikthyrConfig;
        public ConfigEntry<string> YagluthConfig;
        public ConfigEntry<string> ModerConfig;
        public ConfigEntry<string> ElderConfig;
        public ConfigEntry<string> SeekerQueenConfig;
        public ConfigEntry<string> FaderConfig;

        public ConfigEntry<string> StaffShieldAbsorbtionConfig;

        public ConfigEntry<string> SiegeBombDamageConfig;
        public ConfigEntry<string> RamDamageConfig;

        public ConfigEntry<string> DiscordWebhook;
        public ConfigEntry<string> KillFeedName;
        public ConfigEntry<string> DeadMessageFormat;
        public ConfigEntry<string> KilledMessageFormat;
        public ConfigEntry<string> KilledInPvpMessageFormat;
        public ConfigEntry<string> DisconnectedInCombatFormat;

        public ConfigEntry<ConsoleMode> Console;

        public ConfigEntry<bool> EnableTombStoneBoost;
        public ConfigEntry<SkillLossType> SkillsLoss;

        public Configuration(ConfigFile config, ConfigSync sync)
        {
            config.SaveOnConfigSet = true;

            MaxDeathPingRadius = Bind(config, sync, "Death ping", "Radius", 20000,
                "Max radius where players will see death of another player. 20k+ - everywhere on map, 0 - nowhere");

            CombatDuration = Bind(config, sync, "Combat", "Duration", 120,
                "Combat effect duration in seconds");
            AllowTeleportInCombat = Bind(config, sync, "Combat", "Allow teleportation", false,
                "Allow teleporation via portals while player in combat");
            AllowChangeEquipmentInCombat = Bind(config, sync, "Combat", "Allow change equipment", true,
                "Allow equip or unequip clothes, cape and accessories in combat");
            CombatHealthRegen = Bind(config, sync, "Combat", "Health regen", 1f,
                "Health regen multiplier during combat");

            CreaturesProtectWard = Bind(config, sync, "Tamed creatures", "Protect ward", true,
                "Allow tamed creatures attack players who do something agressive inside the ward they defending");
            FollowCreaturesProtectWard = Bind(config, sync, "Tamed creatures", "Protect ward when follow", true,
                "Allow tamed creatures that following player attack other players who do something agressive inside the player's ward");
            TamedCreaturesHealthMultiplier = Bind(config, sync, "Tamed creatures", "Health multiplier", 1.25f,
                "Increase health of tamed creatures");
            TamedCreaturesDamageMultiplier = Bind(config, sync, "Tamed creatures", "Damage multiplier", 1f,
                "Increase tamed creatures damage");

            ResetBossPowerOnChange = Bind(config, sync, "Boss power", 
                "Reset cooldown", false,
                "If true then cooldown of boss power will be reset when change active boss power");
            BonemassConfig = Bind(config, sync, "Boss power", 
                "Bonemass", "300:1200",
                "Duration and cooldown of Bonemass power (leave empty to leave vanilla values)");
            YagluthConfig = Bind(config, sync, "Boss power",
                "Yagluth", "300:1200",
                "Duration and cooldown of Yagluth power (leave empty to leave vanilla values)");
            EikthyrConfig = Bind(config, sync, "Boss power",
                "Eikthyr", "300:1200",
                "Duration and cooldown of Eikthyr power (leave empty to leave vanilla values)");
            ModerConfig = Bind(config, sync, "Boss power",
                "Moder", "300:1200",
                "Duration and cooldown of Moder power (leave empty to leave vanilla values)");
            ElderConfig = config.Bind("Boss power",
                "The Elder", "300:1200",
                "Duration and cooldown of The Elder power (leave empty to leave vanilla values)");
            SeekerQueenConfig = config.Bind("Boss power",
                "Seeker Queen", "300:1200",
                "Duration and cooldown of Seeker Queen power (leave empty to leave vanilla values)");
            FaderConfig = config.Bind("Boss power",
                "Fader", "300:1200",
                "Duration and cooldown of Fader power (leave empty to leave vanilla values)");

            StaffShieldAbsorbtionConfig = Bind(config, sync, "Staffs", "Staff shield", "",
                "Shield resistance - Min:Max (with skill level 100). Recommended: 150:400. Leave empty to leave vanilla values");

            SiegeBombDamageConfig = Bind(config, sync, "Raid", "Siege bomb damage", "",
                "Damage of siege bomb aoe (chop:pickaxe:damage:blunt:pierce:slash:fire:frost:lighting:poison:spirit).\n" +
                "Vanilla - 100:400:100:0:0:0:0:0:0:0:0\n" +
                "Leave empty to leave vanilla values");
            RamDamageConfig = Bind(config, sync, "Raid", "Ram damage", "",
                "Damage of Ram aoe (chop:pickaxe:damage:blunt:pierce:slash:fire:frost:lighting:poison:spirit).\n" +
                "Vanilla - 0:600:0:0:0:0:0:0:0:0:0\n" +
                "Leave empty to leave vanilla values");

            Console = Bind(config, sync, "Console", "Mode", ConsoleMode.AdminOnly,
                "Allow to force enable or disable console");

            DiscordWebhook = Bind(config, sync, "Discord (Server only)",
                "Webhook url", "", "Discord webhook url to show killfeed", false);
            KillFeedName = Bind(config, sync, "Discord (Server only)",
                "Username", "KillFeed",
                "The name on whose behalf the mod will post the killfeed in the discord channel", false);
            KilledMessageFormat = Bind(config, sync, "Discord (Server only)",
                "Killed format", "{player} killed by {attacker}",
                "Message when player killed by creature", false);
            KilledInPvpMessageFormat = Bind(config, sync, "Discord (Server only)",
                "Killed in PvP format", "{player} killed by {attacker}",
                "Message when player killed by another player", false);
            DeadMessageFormat = Bind(config, sync, "Discord (Server only)",
                "Dead format", "{player} is dead",
                "Message when player dead by other reasons (suicide, drowned etc.)", false);
            DisconnectedInCombatFormat = Bind(config, sync, "Discord (Server only)",
                "Disconnected in combat", "{player} ran away like a coward!",
                "Message when player logged out with combat effect", false);

            EnableTombStoneBoost = Bind(config, sync, "Tweaks", "Tomb stone boost", true,
                "Tomb stone boost can be disabled to prevent abuse it to get an advantage in pvp.");
            SkillsLoss = Bind(config, sync, "Tweaks", "Skills loss", SkillLossType.Vanilla,
                $"{SkillLossType.Vanilla} - loose skills as in vanilla game\n" +
                $"{SkillLossType.NoLossInPvp} - if you die in pvp you dont loose skills\n" +
                $"{SkillLossType.NoLoss} - you dont loose skills on death at all");
        }

        private static ConfigEntry<T> Bind<T>(ConfigFile config, ConfigSync sync, string section, string key, T value, string description, bool syncWithServer = true)
        {
            var entry = config.Bind(section, key, value, description);
            if (syncWithServer)
                sync.AddConfigEntry(entry);
            return entry;
        }
    }
}
