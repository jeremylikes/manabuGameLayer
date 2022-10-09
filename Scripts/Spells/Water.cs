using Characters;
using Interactables;
using Managers;
using Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public class Water : Spell
    {

        public override void ExecuteEffect()
        {
            ScanForAffectedObjects();
            StartCoroutine(PerformWhirlpool());
            StartCoroutine(DoBurstDamage(1f));
            //StartCoroutine(KillAfterTime(4f));
        }

        private IEnumerator PerformWhirlpool()
        {
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }

        public override string GetJapaneseNameInRomaji()
        {
            return "Mizu";
        }

        public override string GetNameInKanji()
        {
            return "水";
        }

        public override int GetRequiredMP()
        {
            return 20;
        }

        protected override void PlayCastSoundEffect()
        {
            var audio = GetComponent<AudioSource>();
            audio.clip = _castInit;
            audio.Play();
        }

        public override bool CanEnchant()
        {
            return true;
        }

        public override SpellNames SpellName => SpellNames.Water;


    }
}
