using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using JetBrains.Annotations;
using Controllers;
using Calculators;
using Effects;
using Interactables;

namespace Spells
{
    public class Stop : Spell
    {

        public override SpellNames SpellName => SpellNames.Stop;
        private void Awake()
        {
            _effectRadius = 1f;
            
        }

        public override string GetJapaneseNameInRomaji()
        {
            return "shi";
        }

        public override string GetNameInKanji()
        {
            return "止";
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
            //StartCoroutine(ExecuteIcePrison());
            ScanForAffectedObjects();
            StartCoroutine(StopTime());
        }

        public IEnumerator StopTime()
        {
            var castDelay = 0.5f;
            var effectDuration = 5f;
            StartCoroutine(DisableIcon());
            yield return new WaitForSeconds(castDelay);
            //gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            var ph = new PhysicsHelpers();
            var affectedCharacters = ph.GetCharactersWithinRadius2D(transform.position, CharacterTypes.Enemy, 10f);
            ToggleStunCharacter(affectedCharacters, true);
            var affectedObjects = ph.GetObjectsAffectedByTime(transform.position, 0.5f);
            ToggleStunObjects(affectedObjects);
            while (effectDuration > 0f)
            {
                effectDuration -= Time.deltaTime;
                yield return null;
            }
            ToggleStunCharacter(affectedCharacters, false);
            Destroy(gameObject);
            
        }
        public IEnumerator DisableIcon()
        {
            yield return new WaitForSeconds(2f);
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        public void ToggleStunCharacter(List<Character> characters, bool setting)
        {
            foreach (Character c in characters)
            {
                if (c != null)
                {
                    var aiController = c.GetComponent<EnemyAI>() ?? null;
                    if (setting == true && aiController._currentState == EnemyAI._states.Stun)
                        return;
                    aiController._currentState = setting ? EnemyAI._states.Stun : EnemyAI._states.Patrol;
                }

            }
        }

        public void ToggleStunObjects(List<ITimeInteractable> objectsToStun)
        {
            objectsToStun.ForEach(obj => obj.ExcecuteTimeStopEffect());
        }

        public override bool CanEnchant()
        {
            return false;
        }
    }

}
