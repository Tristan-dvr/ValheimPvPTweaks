using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ValheimPvPTweaks.PvpCombat
{
    class TamedAnimalCombatHandler : MonoBehaviour
    {
        private const string FollowTargetZdoKey = "follow_target";

        private Character _character;
        private ZNetView _view;
        private HashSet<long> _markedPlayersEnemy = new HashSet<long>();
        private PrivateArea _defendingArea;

        private static event Action<Character, Player> _onPlayerAttackerCharacter;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _view = _character.m_nview;

            InvokeRepeating(nameof(RefreshDefendingArea), 0, 5);
            InvokeRepeating(nameof(TryFindFollowingPlayer), 1, 5);
        }

        private bool TryGetTameable(out Tameable tameable) => TryGetComponent<Tameable>(out tameable);

        private void OnEnable()
        {
            _onPlayerAttackerCharacter += CheckAttackedTarget;
        }

        private void OnDisable()
        {
            _onPlayerAttackerCharacter -= CheckAttackedTarget;
        }

        internal static void OnPlayerAttackedCharacter(Character attacked, Player attacker)
        {
            _onPlayerAttackerCharacter?.Invoke(attacked, attacker);
        }

        private void CheckAttackedTarget(Character character, Player player)
        {
            //if (IsFollowingPlayer() && player.GetPlayerID() == GetFollowPlayerId() && !_character.GetBaseAI().IsAlerted())
            //{
            //    var ai = _character.GetBaseAI() as MonsterAI;
            //    if (ai == null)
            //        return;

            //    ai.m_targetCreature = character;
            //    ai.SetAlerted(true);
            //    ai.SetTargetInfo(character.GetZDOID());
            //}
        }

        public bool IsFollowingPlayer() => TryGetTameable(out var tameable) && tameable.m_monsterAI.GetFollowTarget() != null;

        public bool IsDefendingArea() => _defendingArea != null;

        public Player GetFollowPlayer() => Player.GetPlayer(GetFollowPlayerId());

        public bool CheckIsEnemy(TamedAnimalCombatHandler character)
        {
            if (character.IsFollowingPlayer() 
                && IsEnemy(character.GetFollowPlayerId()) 
                && GetFollowPlayerId() != character.GetFollowPlayerId())
                return true;

            if (IsFollowingPlayer() && character.CheckIsEnemy(GetFollowPlayer()))
                return true;

            return false;
        }

        public bool CheckIsEnemy(Player player)
        {
            if (IsEnemy(player.GetPlayerID()))
                return true;

            if (IsDefendingArea() && !_defendingArea.HaveAccess(player) && player.InCombat())
            {
                _markedPlayersEnemy.Add(player.GetPlayerID());
                return true;
            }
            return false;
        }

        private bool IsEnemy(long playerId)
        {
            return _markedPlayersEnemy.Contains(playerId);
        }

        public void RefreshFollowingPlayer()
        {
            if (IsFollowingPlayer() 
                && TryGetTameable(out var tameable)
                && tameable.m_monsterAI.GetFollowTarget().TryGetComponent<Player>(out var player))
            {
                _view.GetZDO().Set(FollowTargetZdoKey, player.GetPlayerID());
            }
            else
            {
                _view.GetZDO().Set(FollowTargetZdoKey, 0L);
            }
        }

        private void RefreshDefendingArea()
        {
            if (_view == null || !_view.IsValid() || !_view.IsOwner())
                return;

            if (IsFollowingPlayer())
                _defendingArea = null;
            else
                _defendingArea = PrivateArea.m_allAreas.FirstOrDefault(p => p.IsEnabled() && p.IsInside(transform.position, 0));
        }

        private void TryFindFollowingPlayer()
        {
            if (_view == null || !_view.IsValid() || !_view.IsOwner())
                return;

            if (!IsFollowingPlayer() && GetFollowPlayerId() != 0)
            {
                var player = GetFollowPlayer();
                if (player != null && TryGetTameable(out var tameable))
                    tameable.Command(player);
            }
        }

        private long GetFollowPlayerId() => _view.GetZDO().GetLong(FollowTargetZdoKey);
    }
}
