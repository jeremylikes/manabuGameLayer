using Characters;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Managers;

namespace Collectables
{
    public class CrystalHelmet : Armor
    {

        public override EquippableSlots GetEquipmentSlot()
        {
            return EquippableSlots.Head;
        }

        public override CollectableNames GetReferenceName()
        {
            return CollectableNames.CrystalHelmet;
        }

        protected override StatEffect[] GetAffectedStats()
        {
            StatEffect defBonus = new StatEffect(CharacterStats.Defense, 10f);
            return new[] { defBonus };

        }

    }

}
