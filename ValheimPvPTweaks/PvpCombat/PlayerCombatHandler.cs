using System;
using UnityEngine;

namespace ValheimPvPTweaks.PvpCombat
{
    class PlayerCombatHandler : MonoBehaviour
    {
        const string EnterCombatRpc = nameof(EnterCombatRpc);

        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _player.m_onDamaged = (Action<float, Character>)Delegate.Combine(_player.m_onDamaged, new Action<float, Character>(OnDamaged));

            if (_player.m_nview != null && _player.m_nview.IsValid())
                _player.m_nview.Register(EnterCombatRpc, OnEnteredPvpMode);
        }

        private void OnDamaged(float damage, Character character)
        {
            if (damage <= 0 || character == null || !character.IsPlayer())
                return;

            OnEnteredPvpMode(0);
            character.m_nview.InvokeRPC(EnterCombatRpc);
        }

        private void OnEnteredPvpMode(long obj)
        {
            _player.GetSEMan().AddStatusEffect(SE_Combat.Name.GetStableHashCode(), true);
        }

        public static void EnterCombat(Character character)
        {
            if (character != null && character.m_nview != null && character.m_nview.IsValid())
                character.m_nview.InvokeRPC(EnterCombatRpc);
        }
    }
}
