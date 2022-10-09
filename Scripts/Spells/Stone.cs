using Characters;
using Effects;
using Interactables;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Spells
{
    public class Stone : Spell, IDestructable
    {
        [SerializeField] private int _health;

        private void Awake()
        {
            //_effectRadius = 1.5f;
            _health = 20;
        }

        private void Start()
        {
            StartCoroutine(MonitorHealth());
            StartCoroutine(ExecuteLifeCountdownSequence());
        }

        public override string GetJapaneseNameInRomaji()
        {
            return "ishi";
        }

        public override int GetRequiredMP()
        {
            // get this from experience level in that particular orientation
            return 5;
        }

        protected override void PlayCastSoundEffect()
        {
            AudioManager._instance.PlaySoundEffect(_castInit);
        }

        public override void ExecuteEffect()
        {
            //StartCoroutine(DoBurstDamage());
            //StartCoroutine(killAfterTime(4f));
        }

        public override string GetNameInKanji()
        {
            return "石";
        }

        public IEnumerator ExecuteLifeCountdownSequence()
        {
            var timer = 20f;
            Coroutine flashCoroutine = null;
            bool flashStarted = false;
            while (timer > 0f)
            {
                if (!flashStarted && timer <= 5f) 
                {
                    var sfx = GameManager._instance.gameObject.GetComponent<SpecialEffects>();
                    var sr = GetComponent<SpriteRenderer>();
                    flashCoroutine = StartCoroutine(sfx.FlashSpriteForTime(sr, 5f));
                    flashStarted = true;
                }

                timer -= Time.deltaTime;
                yield return null;
            }
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            Destroy(gameObject);
        }

        public IEnumerator MonitorHealth()
        {
            while (_health > 0)
            {
                yield return null;
            }
            var sr = GetComponent<SpriteRenderer>();
            StartCoroutine(GameManager._instance.gameObject.GetComponent<SpecialEffects>().FlashSpriteOnce(sr));
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }

        public void TakeDamage(int dmg)
        {
            _health -= dmg;
            var sr = GetComponent<SpriteRenderer>();
            StartCoroutine(GameManager._instance.gameObject.GetComponent<SpecialEffects>().FlashSpriteOnce(sr));
        }

        public override bool CanEnchant()
        {
            return false;
        }

        public override SpellNames SpellName => SpellNames.Stone;
    }

}
