using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectables
{

    public abstract class Weapon : Equipment
    {

        public enum WeaponTypes
        {
            oneHanded,
            twoHanded
        }

        public abstract WeaponTypes GetWeaponType();

        public abstract float GetRange();
    }
}

