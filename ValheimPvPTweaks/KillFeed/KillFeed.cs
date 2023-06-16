using DiscordTools;
using System;
using System.Linq;
using UnityEngine;

namespace ValheimPvPTweaks.KillFeed
{
    public class KillFeed : MonoBehaviour
    {
        const string KillFeedRpc = "VPT_KillFeedRpc";

        private static UserInfo KillFeedUser = new UserInfo()
        {
            Name = "Kill feed", //  color tags broken, was cyan
        };

        public static event Action<KillData> OnCharacterKilled;

        private int _playerPrefabHash;
        
        private void Awake()
        {
            _playerPrefabHash = Game.instance.m_playerPrefab.name.GetStableHashCode();

            if (ZNet.instance.IsServer())
            {
                ZRoutedRpc.instance.Register<ZDOID, ZDOID, string>(Constants.CharacterDeadRpc, RPC_CharacterKilled);
                OnCharacterKilled += SendKillNotifications;
                OnCharacterKilled += SendDiscordNotification;
            }
            else
            {
                ZRoutedRpc.instance.Register<string, string, Vector3>(KillFeedRpc, RPC_KillFeed);
            }
        }

        internal void OnPlayerDisconnected(ZNetPeer peer)
        {
            var url = Plugin.Configuration.DiscordWebhook.Value;
            if (ZNet.instance.IsServer()
                && peer.InCombat()
                && !string.IsNullOrEmpty(url)
                && !string.IsNullOrEmpty(Plugin.Configuration.DisconnectedInCombatFormat.Value))
            {
                Log.Info($"Player {peer.GetSteamId()}:{peer.m_playerName} disconnected in combat");

                var message = Plugin.Configuration.DisconnectedInCombatFormat.Value
                    .Replace("{player}", peer.m_playerName);
                ThreadinUtil.RunThread(() => DiscordTool.SendMessageToDiscord(url, "KillFeed", message));
            }
        }

        private void RPC_KillFeed(long arg1, string characterName, string attackerName, Vector3 position)
        {
            if (string.IsNullOrEmpty(characterName))
                return;

            if (!string.IsNullOrEmpty(attackerName))
            {
                var killMessage = $"{characterName} $vpo_kill_feed_msg_killed_by {attackerName}".Localize();

                Chat.instance.AddString(KillFeedUser.Name, killMessage, Talker.Type.Shout);
                Chat.instance.AddInworldText(null, arg1, position, Talker.Type.Shout, KillFeedUser, killMessage);
            }
            else
            {
                Chat.instance.AddString(KillFeedUser.Name, $"{characterName} $vpo_kill_feed_msg_dead".Localize(), Talker.Type.Normal);
            }
            Chat.instance.m_hideTimer = 0;
        }

        private void RPC_CharacterKilled(long uid, ZDOID characterId, ZDOID attackerId, string weapon)
        {
            var peer = ZNet.instance.GetPeer(uid);
            try
            {
                if (characterId.IsNone() || !ZDOMan.instance.m_objectsByID.ContainsKey(characterId))
                {
                    Log.Debug($"Received invalid killed character id {characterId} from {peer?.m_playerName}:{peer?.GetSteamId()}");
                    return;
                }

                var characterName = GetName(characterId, out var characterIsPlayer, out var characterZdo, out var characterPeer);
                var attackerName = GetName(attackerId, out var attackerIsPlayer, out var attackerZdo, out var attackerPeer);
                Log.Debug($"Killed {characterName} ({characterId}) by {attackerName} ({attackerId}) with {weapon}");
                if (attackerZdo == characterZdo)
                    return;

                OnCharacterKilled?.Invoke(new KillData
                {
                    killed = new KillData.CharacterData()
                    {
                        displayName = characterName,
                        isPlayer = characterIsPlayer,
                        peer = characterPeer,
                        zdo = characterZdo,
                        prefabName = ZNetScene.instance.GetPrefab(characterZdo.GetPrefab())?.name,
                    },
                    killer = new KillData.CharacterData()
                    {
                        displayName = attackerName,
                        isPlayer = attackerIsPlayer,
                        peer = attackerPeer,
                        zdo = attackerZdo,
                        prefabName = attackerZdo != null ? ZNetScene.instance.GetPrefab(attackerZdo.GetPrefab())?.name : string.Empty,
                    },
                    weapon = weapon,
                });
            }
            catch (Exception e)
            {
                Log.Warning($"Cannot handle kill received from {peer?.m_playerName}:{peer?.GetSteamId()} ({peer?.m_characterID}) {attackerId} {characterId}: {e.Message}\n{e.StackTrace}]");
            }
        }

        private void SendKillNotifications(KillData killData)
        {
            var charcater = killData.killed;
            if (!charcater.isPlayer)
                return;

            var position = charcater.zdo.GetPosition();
            var maxPingRadius = Plugin.Configuration.MaxDeathPingRadius.Value;
            foreach (var peer in ZNet.instance.m_peers)
            {
                if (maxPingRadius > 0 && peer.IsReady() && Utils.DistanceXZ(peer.GetRefPos(), position) < maxPingRadius)
                {
                    ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, KillFeedRpc, charcater.displayName, killData.killer.displayName, position);
                }
            }
        }

        private void SendDiscordNotification(KillData killData)
        {
            var url = Plugin.Configuration.DiscordWebhook.Value;
            var character = killData.killed;
            var attacker = killData.killer;
            if (string.IsNullOrEmpty(url) || !character.isPlayer)
                return;

            Log.Info($"Sending discord notifiaction {character.displayName} killed by {attacker.displayName}");
            var message = "";
            if (!string.IsNullOrEmpty(attacker.displayName) && !string.IsNullOrEmpty(Plugin.Configuration.KilledMessageFormat.Value))
            {
                var attackerName = Localization.instance.Localize(attacker.displayName);
                message = Plugin.Configuration.KilledMessageFormat.Value
                    .Replace("{player}", character.displayName)
                    .Replace("{attacker}", attackerName);
            }
            else if (!string.IsNullOrEmpty(Plugin.Configuration.DeadMessageFormat.Value))
            {
                message = Plugin.Configuration.DeadMessageFormat.Value
                    .Replace("{player}", character.displayName);
            }

            if (!string.IsNullOrEmpty(message))
                ThreadinUtil.RunThread(() => DiscordTool.SendMessageToDiscord(url, Plugin.Configuration.KillFeedName.Value, message));
        }

        private string GetName(ZDOID id, out bool isPlayer, out ZDO zdo, out ZNetPeer peer)
        {
            isPlayer = false;
            zdo = ZDOMan.instance.GetZDO(id);
            peer = null;

            if (id == ZDOID.None)
                return string.Empty;

            if (zdo.GetPrefab() == _playerPrefabHash)
            {
                peer = ZNet.instance.m_peers.Find(p => p.m_characterID == id);
                if (peer != null)
                {
                    isPlayer = true;
                    return peer.m_playerName;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                var prefab = ZNetScene.instance.GetPrefab(zdo.GetPrefab());
                if (prefab == null)
                    return string.Empty;

                if (prefab.TryGetComponent<Character>(out var character))
                    return character.m_name;
                else
                    return prefab.name;
            }
        }

        public struct KillData
        {
            public CharacterData killed;
            public CharacterData killer;
            public string weapon;

            public class CharacterData
            {
                public string displayName;
                public string prefabName;
                public ZNetPeer peer;
                public bool isPlayer;
                public ZDO zdo;
            }
        }
    }
}
