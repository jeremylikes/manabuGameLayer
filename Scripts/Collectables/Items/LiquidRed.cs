using Characters;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Collectables
{

    public class LiquidRed : Item
    {
        public override CollectableNames GetReferenceName()
        {
            return CollectableNames.LiquidRed;
        }

        public override void Use(Character target)
        {
            var ac = Resources.Load<AudioClip>(@"Audio/SE/Items/Recovery/replenish");
            AudioManager._instance.PlaySoundEffect(ac);
            Debug.Log("HP increases by 50");
            target.AdjustStat(CharacterStats.HP, 50);
        }
    }

}
