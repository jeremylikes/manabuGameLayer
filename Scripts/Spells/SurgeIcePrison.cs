using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using Managers;
using System;

namespace Spells
{
    public class SurgeIcePrison : MonoBehaviour, ISpellAffectedObject
    {
        [SerializeField] private Sprite _frozenGraphic;
        public Action _onPrisonMelted;

        private void Update()
        {
            transform.position = GameManager._instance._mainCharacter.transform.position;
        }

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Fireball;
        }

        public void ReactToSpell()
        {
            ControlsManager._instance.SetActiveControls();
            _onPrisonMelted?.Invoke();
            Destroy(gameObject);
        }
    }

}
