namespace ValheimPvPTweaks.PvpCombat
{
    public class SE_Combat : SE_Stats
    {
        public const string Name = nameof(SE_Combat);

        public override bool CanAdd(Character character) => character.IsPlayer();

        public override void Setup(Character character)
        {
            base.Setup(character);
            m_healthRegenMultiplier = Plugin.Configuration.CombatHealthRegen.Value;
            m_ttl = Plugin.Configuration.CombatDuration.Value;
            m_character.m_nview?.GetZDO()?.Set(Constants.InCombatZdoKey, true);
        }

        public override void Stop()
        {
            base.Stop();
            if (Game.instance.IsShuttingDown()) return;

            m_character.m_nview?.GetZDO()?.Set(Constants.InCombatZdoKey, false);
        }

        public static SE_Combat Create()
        {
            var se = CreateInstance<SE_Combat>();
            se.name = Name;
            se.m_name = "$vpo_se_combat_name";

            se.m_startMessage = "$vpo_se_combat_start_msg";
            se.m_startMessageType = MessageHud.MessageType.Center;
            se.m_stopMessage = "$vpo_se_combat_stop_msg";
            se.m_stopMessageType = MessageHud.MessageType.TopLeft;

            return se;
        }
    }
}
