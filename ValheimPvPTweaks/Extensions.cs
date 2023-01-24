namespace ValheimPvPTweaks
{
    public static class Extensions
    {
        public static string Localize(this string text) => Localization.instance.Localize(text);

        public static bool InCombat(this Player player)
        {
            return player.m_nview != null && player.m_nview.IsValid() && player.m_nview.GetZDO().GetBool(Constants.InCombatZdoKey);
        }

        public static ZDO GetZdo(this ZNetPeer peer) => ZDOMan.instance.GetZDO(peer.m_characterID);

        public static bool InCombat(this ZNetPeer peer) => GetZdo(peer)?.GetBool(Constants.InCombatZdoKey) ?? false;
    }
}
