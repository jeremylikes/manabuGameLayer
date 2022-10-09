using Characters;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Collectables
{

    public class LiquidBlue : Item
    {

        public override CollectableNames GetReferenceName()
        {
            return CollectableNames.LiquidBlue;
        }

        public override void Use(Character target)
        {
            var ac = Resources.Load<AudioClip>(@"Audio/SE/Items/Recovery/replenish");
            AudioManager._instance.PlaySoundEffect(ac);
            target.AdjustStat(CharacterStats.MP, 50);
        }
    }

}
