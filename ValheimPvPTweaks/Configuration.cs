using BepInEx.Configuration;
using ServerSync;

namespace ValheimPvPTweaks
{
    class Configuration
    {
        public enum ConsoleMode { Disabled, AdminOnly, Enabled };

        public ConfigEntry<int> MaxDeathPingRadius;
        public ConfigEntry<int> CombatDuration;
        public ConfigEntry<bool> AllowTeleportInCombat;
        public ConfigEntry<bool> AllowChangeEquipmentInCombat;
        public ConfigEntry<bool> CreaturesProtectWard;
        public ConfigEntry<float> TamedCreaturesHealthMultiplier;
        public ConfigEntry<float> TamedCreaturesDamageMultiplier;

        public ConfigEntry<bool> ResetBossPowerOnChange;
        public ConfigEntry<string> BonemassConfig;
        public ConfigEntry<string> EikthyrConfig;
        public ConfigEntry<string> YagluthConfig;
        public ConfigEntry<string> ModerConfig;
        public ConfigEntry<string> ElderConfig;
        public ConfigEntry<string> SeekerQueenConfig;

        public ConfigEntry<string> DiscordWebhook;
        public ConfigEntry<string> KillFeedName;
        public ConfigEntry<string> DeadMessageFormat;
        public ConfigEntry<string> KilledMessageFormat;
        public ConfigEntry<string> DisconnectedInCombatFormat;

        public ConfigEntry<ConsoleMode> Console;

        public ConfigEntry<float> SwordsSecondaryAttackDamage;
        public ConfigEntry<string> ExcludedSwordsPrefabs;
        public ConfigEntry<string> IncludedSwordPrefabs;

        public ConfigEntry<string> CrossbowDamage;

        public ConfigEntry<string> StaffFireDamage;
        public ConfigEntry<int> StaffFireEitr;
        public ConfigEntry<string> StaffIceDamage;
        public ConfigEntry<int> StaffIceEitr;
        public ConfigEntry<string> StaffShield;
        public ConfigEntry<int> StaffShieldEitr;

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

            CreaturesProtectWard = Bind(config, sync, "Tamed cratures", "Attack players", true,
                "Allow tamed creatures attack players who do something agressive inside the ward they defending");
            TamedCreaturesHealthMultiplier = Bind(config, sync, "Tamed cratures", "Health multiplier", 1.25f,
                "Increase health of tamed creatures");
            TamedCreaturesDamageMultiplier = Bind(config, sync, "Tamed cratures", "Damage multiplier", 1f,
                "Increase tamed creatures damage");

            ResetBossPowerOnChange = Bind(config, sync, "Boss power", 
                "Reset cooldown", false,
                "If true then cooldown of boss power will be reset when change active boss power");
            BonemassConfig = Bind(config, sync, "Boss power", 
                "Bonemass", "300:1200",
                "Duration and cooldown of Bonemass power");
            YagluthConfig = Bind(config, sync, "Boss power",
                "Yagluth", "300:1200",
                "Duration and cooldown of Yagluth power");
            EikthyrConfig = Bind(config, sync, "Boss power",
                "Eikthyr", "300:1200",
                "Duration and cooldown of Eikthyr power");
            ModerConfig = Bind(config, sync, "Boss power",
                "Moder", "300:1200",
                "Duration and cooldown of Moder power");
            ElderConfig = config.Bind("Boss power",
                "The Elder", "300:1200",
                "Duration and cooldown of The Elder power");
            SeekerQueenConfig = config.Bind("Boss power",
                "Seeker Queen", "300:1200",
                "Duration and cooldown of Seeker Queen power");

            Console = Bind(config, sync, "Console", "Mode", ConsoleMode.AdminOnly,
                "Allow to force enable or disable console");

            DiscordWebhook = Bind(config, sync, "Discord (Server only)",
                "Webhook url", "", "Discord webhook url to show killfeed", false);
            KillFeedName = Bind(config, sync, "Discord (Server only)",
                "Username", "KillFeed",
                "The name on whose behalf the mod will post the killfeed in the discord channel", false);
            KilledMessageFormat = Bind(config, sync, "Discord (Server only)",
                "Killed format", "{player} killed by {attacker}",
                "Message when player killed by another player or creature", false);
            DeadMessageFormat = Bind(config, sync, "Discord (Server only)",
                "Dead format", "{player} is dead",
                "Message when player dead by other reasons (suicide, drowned etc.)", false);
            DisconnectedInCombatFormat = Bind(config, sync, "Discord (Server only)",
                "Disconnected in combat", "{player} ran away like a coward!",
                "Message when player logged out with combat effect", false);

            SwordsSecondaryAttackDamage = Bind(config, sync, "Swords secondary attack", "Damage multiplier", 0f,
                "3 in vanilla, 2 is recommended, leave 0 to leave vanilla values");
            ExcludedSwordsPrefabs = Bind(config, sync, "Swords secondary attack", "Excluded prefabs", "",
                "Sword prefabs which will not be changed. Divided by ','.");
            IncludedSwordPrefabs = Bind(config, sync, "Swords secondary attack", "Included prefabs", "",
                "Prefabs which will not be changed. Divided by ','.");

            StaffFireDamage = Bind(config, sync, "Staffs", "Fireball staff damage", "",
                "Damage of Fireball staff - Damage:Damage per item level. Recommended: 80:4. Leave empty to leave vanilla values");
            StaffFireEitr = Bind(config, sync, "Staffs", "Fireball staff eitr", 0,
                "Fireball staff Eitr consumption. 35 in vanilla, 40 is recommended, leave 0 to leave vanilla values");
            StaffIceDamage = Bind(config, sync, "Staffs", "Ice staff damage", "",
                "Damage of Ice staff - Damage:Damage per item level. Recommended: 45:4. Leave empty to leave vanilla values");
            StaffIceEitr = Bind(config, sync, "Staffs", "Ice staff eitr", 0,
                "Ice staff Eitr consumption. 5 in vanilla, 5 is recommended, leave 0 to leave vanilla values");
            StaffShield = Bind(config, sync, "Staffs", "Staff shield", "",
                "Shield resistance - Min:Max (with skill level 100). Recommended: 150:400. Leave empty to leave vanilla values");
            StaffShieldEitr = Bind(config, sync, "Staffs", "Staff shield eitr", 0,
                "Staff shield Eitr consumption. 60 in vanilla, 75 is recommended, leave 0 to leave vanilla values");

            CrossbowDamage = Bind(config, sync, "Crossbow", "Damage", "",
                "Damage of crossbow - Damage:Damage per item level. Recommended: 150:5. Leave empty to leave vanilla values");
        }

        private ConfigEntry<T> Bind<T>(ConfigFile config, ConfigSync sync, string section, string key, T value, string description, bool syncWithServer = true)
        {
            var entry = config.Bind(section, key, value, description);
            if (syncWithServer)
                sync.AddConfigEntry(entry);
            return entry;
        }
    }
}
