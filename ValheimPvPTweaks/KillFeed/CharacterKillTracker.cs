using System;
using UnityEngine;

namespace ValheimPvPTweaks.KillFeed
{
    public class CharacterKillTracker : MonoBehaviour
    {
        private const int AttackTime = 15;

        private Character _character;

        private Character _lastAttacker;
        private string _lastAttackerWeapon;
        private bool _isDead = false;
        private double _lastAttackTime;

        public static event Action<string, string> OnCharacterKilled;
        public static event Action<Character, Character, string> OnCharacterDead;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _character.m_onDamaged = (Action<float, Character>)Delegate.Combine(_character.m_onDamaged, new Action<float, Character>(OnDamaged));

            if (_character.m_nview != null && _character.m_nview.IsValid())
                _character.m_nview.Register<string, string>(Constants.CharacterDeadRpc, RPC_CharacterKilled);
        }

        private void RPC_CharacterKilled(long uid, string name, string weapon)
        {
            OnCharacterKilled?.Invoke(name, weapon);
        }

        private void OnDeath()
        {
            if (_lastAttacker != null && ZNet.instance.GetTimeSeconds() - _lastAttackTime < AttackTime)
            {
                ZRoutedRpc.instance.InvokeRoutedRPC(Constants.CharacterDeadRpc, _character.GetZDOID(), _lastAttacker.GetZDOID(), _lastAttackerWeapon);
                OnCharacterDead?.Invoke(_character, _lastAttacker, _lastAttackerWeapon);

                if (_lastAttacker is Player)
                {
                    var characterName = _character is Player p ? p.GetPlayerName() : _character.m_name;
                    _lastAttacker.m_nview.InvokeRPC(Constants.CharacterDeadRpc, characterName, _lastAttackerWeapon);
                }
            }
            else
            {
                if (_character.IsPlayer())
                    ZRoutedRpc.instance.InvokeRoutedRPC(Constants.CharacterDeadRpc, _character.GetZDOID(), ZDOID.None, "");
                OnCharacterDead?.Invoke(_character, null, "");
            }
        }

        private void OnDamaged(float damage, Character character)
        {
            if (_isDead)
                return;

            if (character != null)
            {
                _lastAttacker = character;
                _lastAttackTime = ZNet.instance.GetTimeSeconds();
                _lastAttackerWeapon = character is Player player ? player.GetCurrentWeapon().m_shared.m_name : "";
            }

            if (_character.GetHealth() <= 0)
            {
                _isDead = true;
                OnDeath();
            }
        }
    }
}
