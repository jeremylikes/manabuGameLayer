using Effects;
using Managers;
using Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{

    public class IceShield : MonoBehaviour, ISpellAffectedObject
    {
        [SerializeField] private int _health = 50;
        public Action _onDestroyed;

        public void CheckHealth()
        {
            if (_health <= 0)
                Destroy();
        }
        public void Destroy()
        {
            _onDestroyed?.Invoke();
            Destroy(gameObject);
        }

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Fireball;
        }

        public void ReactToSpell()
        {
            int dmg = 25;
            _health -= dmg;
            GameManager._instance._messageFX.DisplayDamagePopup(-dmg, transform.position, Quaternion.identity,
                false);
            CheckHealth();
        }
    }

}