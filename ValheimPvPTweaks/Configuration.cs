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

            MaxDeathPingRadius = config.Bind("Death ping", "Radius", 20000,
                new ConfigDescription("Max radius where players will see death of another player. 20k+ - everywhere on map, 0 - nowhere", null));
            CombatDuration = config.Bind("Combat", "Duration", 120,
                new ConfigDescription("Combat effect duration in seconds", null));
            AllowTeleportInCombat = config.Bind("Combat", "Allow teleportation", false,
                new ConfigDescription("Allow teleporation via portals while player in combat", null));
            CreaturesProtectWard = config.Bind("Tamed cratures", "Attack players", true,
                new ConfigDescription("Allow tamed creatures attack players who do something agressive inside the ward they defending", null));
            TamedCreaturesHealthMultiplier = config.Bind("Tamed cratures",
                "Health multiplier",
                1.25f,
                new ConfigDescription("Increase health of tamed creatures", null));
            TamedCreaturesDamageMultiplier = config.Bind("Tamed cratures",
                "Damage multiplier",
                1f,
                new ConfigDescription("Increase tamed creatures damage", null));
            sync.AddConfigEntry(MaxDeathPingRadius);
            sync.AddConfigEntry(CombatDuration);
            sync.AddConfigEntry(AllowTeleportInCombat);
            sync.AddConfigEntry(CreaturesProtectWard);
            sync.AddConfigEntry(TamedCreaturesHealthMultiplier);
            sync.AddConfigEntry(TamedCreaturesDamageMultiplier);

            ResetBossPowerOnChange = config.Bind("Boss power",
                "Reset cooldown",
                false,
                new ConfigDescription("If true then cooldown of boss power will be reset when change active boss power", null));
            BonemassConfig = config.Bind("Boss power",
                "Bonemass",
                "300:1200",
                new ConfigDescription("Duration and cooldown of Bonemass power", null));
            YagluthConfig = config.Bind("Boss power",
                "Yagluth",
                "300:1200",
                new ConfigDescription("Duration and cooldown of Yagluth power", null));
            EikthyrConfig = config.Bind("Boss power",
                "Eikthyr",
                "300:1200",
                new ConfigDescription("Duration and cooldown of Eikthyr power", null));
            ModerConfig = config.Bind("Boss power",
                "Moder",
                "300:1200",
                new ConfigDescription("Duration and cooldown of Moder power", null));
            ElderConfig = config.Bind("Boss power",
                "The Elder",
                "300:1200",
                new ConfigDescription("Duration and cooldown of The Elder power", null));
            SeekerQueenConfig = config.Bind("Boss power",
                "Seeker Queen",
                "300:1200",
                new ConfigDescription("Duration and cooldown of Seeker Queen power", null));
            sync.AddConfigEntry(ResetBossPowerOnChange);
            sync.AddConfigEntry(BonemassConfig);
            sync.AddConfigEntry(YagluthConfig);
            sync.AddConfigEntry(EikthyrConfig);
            sync.AddConfigEntry(ModerConfig);
            sync.AddConfigEntry(ElderConfig);
            sync.AddConfigEntry(SeekerQueenConfig);

            Console = config.Bind("Console", "Mode", ConsoleMode.AdminOnly,
                new ConfigDescription("Allow to force enable or disable console", null));
            sync.AddConfigEntry(Console);

            DiscordWebhook = config.Bind("Discord (Server only)",
                "Webhook url", "");
            KillFeedName = config.Bind("Discord (Server only)",
                "Username", "KillFeed",
                "The name on whose behalf the mod will post the killfeed in the discord channel");
            KilledMessageFormat = config.Bind("Discord (Server only)",
                "Killed format", "{player} killed by {attacker}",
                "Message when player killed by another player or creature");
            DeadMessageFormat = config.Bind("Discord (Server only)",
                "Dead format", "{player} is dead",
                "Message when player dead by other reasons (suicide, drowned etc.)");
            DisconnectedInCombatFormat = config.Bind("Discord (Server only)",
                "Disconnected in combat", "{player} ran away like a coward!",
                "Message when player logged out with combat effect");

            SwordsSecondaryAttackDamage = config.Bind("Swords secondary attack", "Damage multiplier", 0f,
                new ConfigDescription("3 in vanilla, 2 is recommended, leave 0 to leave vanilla values", null));
            ExcludedSwordsPrefabs = config.Bind("Swords secondary attack", "Excluded prefabs", "",
                new ConfigDescription("Sword prefabs which will not be changed. Divided by ','."));
            IncludedSwordPrefabs = config.Bind("Swords secondary attack", "Included prefabs", "",
                new ConfigDescription("Prefabs which will not be changed. Divided by ','."));
            sync.AddConfigEntry(SwordsSecondaryAttackDamage);
            sync.AddConfigEntry(ExcludedSwordsPrefabs);
            sync.AddConfigEntry(IncludedSwordPrefabs);

            StaffFireDamage = config.Bind("Staffs", "Fireball staff damage", "",
                new ConfigDescription("Damage of Fireball staff - Damage:Damage per item level. Recommended: 80:4. Leave empty to leave vanilla values"));
            StaffFireEitr = config.Bind("Staffs", "Fireball staff eitr", 0,
                new ConfigDescription("Fireball staff Eitr consumption. 35 in vanilla, 40 is recommended, leave 0 to leave vanilla values"));
            StaffIceDamage = config.Bind("Staffs", "Ice staff damage", "",
                new ConfigDescription("Damage of Ice staff - Damage:Damage per item level. Recommended: 45:4. Leave empty to leave vanilla values"));
            StaffIceEitr = config.Bind("Staffs", "Ice staff eitr", 0,
                new ConfigDescription("Ice staff Eitr consumption. 5 in vanilla, 5 is recommended, leave 0 to leave vanilla values"));
            StaffShield = config.Bind("Staffs", "Staff shield", "",
                new ConfigDescription("Shield resistance - Min:Max (with skill level 100). Recommended: 150:400. Leave empty to leave vanilla values"));
            StaffShieldEitr = config.Bind("Staffs", "Staff shield eitr", 0,
                new ConfigDescription("Staff shield Eitr consumption. 60 in vanilla, 75 is recommended, leave 0 to leave vanilla values"));
            sync.AddConfigEntry(StaffFireDamage);
            sync.AddConfigEntry(StaffFireEitr);
            sync.AddConfigEntry(StaffIceDamage);
            sync.AddConfigEntry(StaffIceEitr);
            sync.AddConfigEntry(StaffShield);
            sync.AddConfigEntry(StaffShieldEitr);

            CrossbowDamage = config.Bind("Crossbow", "Damage", "",
                new ConfigDescription("Damage of crossbow - Damage:Damage per item level. Recommended: 150:5. Leave empty to leave vanilla values"));
            sync.AddConfigEntry(CrossbowDamage);
        }
    }
}
