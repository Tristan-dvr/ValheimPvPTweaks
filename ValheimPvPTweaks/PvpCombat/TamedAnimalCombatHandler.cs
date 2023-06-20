using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ValheimPvPTweaks.PvpCombat
{
    class TamedAnimalCombatHandler : MonoBehaviour
    {
        private Character _character;
        private ZNetView _view;
        private HashSet<string> _markedPlayersEnemy = new HashSet<string>();
        private PrivateArea _defendingArea;

        private static event Action<Character, Player> _onPlayerAttackerCharacter;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _view = _character.m_nview;

            InvokeRepeating(nameof(RefreshDefendingArea), 0, 5);
        }

        private bool TryGetTameable(out Tameable tameable) => TryGetComponent(out tameable);

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

        public Player GetFollowPlayer()
        {
            return TryGetTameable(out var tameable)
                ? tameable.m_monsterAI?.GetFollowTarget()?.GetComponent<Player>()
                : null;
        }

        public bool CheckIsEnemy(TamedAnimalCombatHandler character)
        {
            var followTarget = character.GetFollowPlayer();
            var myTarget = GetFollowPlayer();
            if (followTarget != null && CheckIsEnemy(followTarget) && myTarget != followTarget)
                return true;

            if (myTarget != null && character.CheckIsEnemy(myTarget))
                return true;

            return false;
        }

        public bool CheckIsEnemy(Player player)
        {
            if (IsEnemy(player.GetPlayerName()))
                return true;

            if (IsDefendingArea() && !_defendingArea.HaveAccess(player) && player.InCombat())
            {
                _markedPlayersEnemy.Add(player.GetPlayerName());
                return true;
            }
            return false;
        }

        private bool IsEnemy(string name)
        {
            return _markedPlayersEnemy.Contains(name);
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
    }
}
