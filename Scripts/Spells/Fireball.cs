using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using TMPro;
using UnityEngine.Assertions.Must;

namespace Spells
{
    public class Fireball : Spell
    {
        
        private void Awake()
        {
            _effectRadius = 1.5f;
        }

        public override string GetJapaneseNameInRomaji()
        {
            return "hi";
        }

        public override int GetRequiredMP()
        {
            // get this from experience level in that particular orientation
            return 10;
        }

        protected override void PlayCastSoundEffect()
        {
            var audioSource = gameObject.GetComponent<AudioSource>();
            AudioManager._instance.PlaySoundEffect(_castInit, false);
        }

        public override void ExecuteEffect()
        {
            ScanForAffectedObjects();
            StartCoroutine(DoBurstDamage(1f));
            StartCoroutine(KillAfterTime(0.5f));
        }

        public override string GetNameInKanji()
        {
            return "火";
        }

        public override SpellNames SpellName => SpellNames.Fireball;

        public override bool CanEnchant()
        {
            return true;
        }
    }

}
