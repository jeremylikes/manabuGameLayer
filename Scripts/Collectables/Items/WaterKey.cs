using Characters;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Collectables
{
    public class WaterKey : Item
    {
        
        public WaterKey()
        {
            _isUsable = false;
        }

        public override CollectableNames GetReferenceName()
        {
            return CollectableNames.WaterKey;
        }

        public override void Use(Character target)
        {
            
        }
    }
}

