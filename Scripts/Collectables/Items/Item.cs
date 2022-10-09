using Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectables
{
    public abstract class Item: Collectable
    {
        public bool _isUsable = true;

        public abstract void Use(Character target);

    }

}
