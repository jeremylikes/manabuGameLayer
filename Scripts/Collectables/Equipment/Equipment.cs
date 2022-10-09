using Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectables
{
    public abstract class Equipment : Collectable
    {
        protected abstract StatEffect[] GetAffectedStats();

        public virtual bool IsSpecial() { return false; }
        public abstract EquippableSlots GetEquipmentSlot();

        public virtual void AdjustStats(Character character, bool remove = false)
        {
            foreach (var effect in GetAffectedStats())
            {
                float amountToAdjust = remove ? -effect._effectAmount : effect._effectAmount;
                character.AdjustStat(effect._stat, amountToAdjust, true);
            }

        }

        protected class StatEffect
        {
            public CharacterStats _stat;
            public float _effectAmount;

            public StatEffect(CharacterStats stat, float effectAmount)
            {
                _stat = stat; 
                _effectAmount = effectAmount;
            }
        }
    }

}
