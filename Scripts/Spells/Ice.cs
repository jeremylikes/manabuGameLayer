using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using JetBrains.Annotations;
using Controllers;
using Calculators;

namespace Spells
{
    public class Ice : Spell
    {

        private void Awake()
        {
            _effectRadius = 1f;
        }

        public override string GetJapaneseNameInRomaji()
        {
            return "koori";
        }

        public override string GetNameInKanji()
        {
            return "氷";
        }

        public override int GetRequiredMP()
        {
            return 10;
        }

        protected override void PlayCastSoundEffect()
        {
            var audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = _castInit;
            audioSource.Play();
        }

        public override void ExecuteEffect()
        {
            StartCoroutine(ExecuteIcePrison());
            ScanForAffectedObjects();
        }

        public IEnumerator ExecuteIcePrison()
        {
            var castDelay = 1f;
            var prisonDuration = 2f;

            yield return new WaitForSeconds(castDelay);
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            var ph = new PhysicsHelpers();
            var affectedCharacters = ph.GetCharactersWithinRadius2D(transform.position, CharacterTypes.Enemy, 0.5f);
            StartCoroutine(DoBurstDamage(affectedCharacters, prisonDuration, 1f, false));
            var icePrisonObjects = new List<GameObject>();

            foreach (Character c in affectedCharacters)
            {
                //Give each one an ice prison prefab
                var prefab = Resources.Load<GameObject>(@"Prefabs/Spells/pfKooriIcePrison");
                var targetPos = c.transform.position;
                var icePrison = Instantiate(prefab, targetPos, Quaternion.identity);
                icePrisonObjects.Add(icePrison);
            }

            ToggleStunCharacter(affectedCharacters, true);

            while (prisonDuration > 0f)
            {
                prisonDuration -= Time.deltaTime;
                yield return null;
            }
            ToggleStunCharacter(affectedCharacters, false);
            icePrisonObjects.ForEach(ipo => Destroy(ipo));
            Destroy(gameObject);
        }

        public void ToggleStunCharacter(List<Character> characters, bool stunActive)
        {
            foreach (Character c in characters)
            {
                if (c)
                {
                    var aiController = c.GetComponent<EnemyAI>();
                    aiController._currentState = stunActive ? EnemyAI._states.Stun : EnemyAI._states.Patrol;
                }

            }
        }

        public override SpellNames SpellName => SpellNames.Ice;


        public override bool CanEnchant()
        {
            return true;
        }
    }

}
