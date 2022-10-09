using UnityEngine;
using Characters;
using Managers;

namespace Collectables
{
    public class Dax : Weapon
    {
        public override bool IsSpecial()
        {
            return true;
        }

        public override WeaponTypes GetWeaponType()
        {
            return WeaponTypes.twoHanded;
        }

        public override EquippableSlots GetEquipmentSlot()
        {
            return EquippableSlots.Weapon;
        }

        public override float GetRange()
        {
            return 0.4f;
        }

        public override CollectableNames GetReferenceName()
        {
            return CollectableNames.Dax;
        }

        protected override StatEffect[] GetAffectedStats()
        {
            var strengthBonus = new StatEffect(CharacterStats.Strength, 10f);
            return new[] { strengthBonus };
        }

    }

}
