using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.Rendering;
using Calculators;
using Interactables;
using System;

namespace Spells
{
    [Serializable]
    public enum SpellNames
    {
        Fireball, Water, Stone, Ice, Stop
    }
    public abstract class Spell : MonoBehaviour
    {
        public Sprite _graphic;
        public float Power { get; set; }
        public AudioClip _burstSE;
        public AudioClip _castInit;
        public float _effectRadius;
        [SerializeField] private GameObject _enchantmentParticles;

        public abstract SpellNames SpellName { get; }

        public abstract string GetJapaneseNameInRomaji();

        public abstract bool CanEnchant();

        public abstract string GetNameInKanji();

        public abstract void ExecuteEffect();

        protected abstract void PlayCastSoundEffect();

        public void CastAtPosition(Vector3 targetPos)
        {
            // let's decouple this from Manabu so they don't presist along with Manabu
            MakeFreestandingObject();

            transform.position = targetPos;
            gameObject.SetActive(true);
            PlayCastSoundEffect();
            ExecuteEffect();

        }

        protected void ScanForAffectedObjects()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (var col in colliders)
            {
                if (col.GetComponent<Character>() is Surge)
                    continue;
                var spellAffected = col.GetComponent<ISpellAffectedObject>() ?? null;
                if (spellAffected != null && spellAffected.GetSpellAffectedBy() == SpellName)
                    spellAffected.ReactToSpell();

            }
        }

        
        public virtual IEnumerator ExecuteEnchantmentEffect(Character target, int baseDamage)
        {
            //var particles = Instantiate(Resources.Load<ParticleSystem>(@"Prefabs/Particles/FireParticles"));
            ParticleSystem particles = Instantiate(_enchantmentParticles).GetComponent<ParticleSystem>();

            particles.transform.position = target.transform.position;

            //yield return new WaitForSeconds(0.2f);
            if (target != null)
            {
                // make fire sparks fly and sounds happen
                var killTimer = particles.main.duration;
                var ac = Resources.Load<AudioClip>(@"Audio/SE/Spells/Fireball/crackle_00");
                AudioManager._instance.PlaySoundEffect(ac);

                // do extra damage (1/2 of spell's total power)
                float enchantmentPenalty = 2f;
                int damage = (int)Math.Ceiling(GetEffectiveSpellDamage(target) / enchantmentPenalty);
                target.TakeDamage(transform, damage, false, false/*, true*/);

                if (target is TutorialSlime)
                {
                    var ts = target as TutorialSlime;
                    if (!ts._hasBeenHitWithEnchantment)
                    {
                        ts._hasBeenHitWithEnchantment = true;
                        ts.ResolveTutorial();
                    }
                }

                yield return new WaitForSeconds(killTimer);
                Destroy(particles.gameObject);
            }
        }

        private void MakeFreestandingObject()
        {
            var trashObject = new GameObject();
            transform.parent = trashObject.transform;
            transform.parent = null;
            Destroy(trashObject);
        }

        private IEnumerator CastAsProjectile(Character assailant, Vector3 targetPos)
        {
            while (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos,
                    Time.deltaTime * 4f);
                yield return null;
            }
            //AudioManager._instance.PlaySoundEffect(_impactSE);
            Destroy(gameObject);
        }

        protected IEnumerator KillAfterTime(float duration)
        {
            while (duration > 0f)
            {
                duration -= Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        private int GetEffectiveSpellDamage(Character t)
        {
            return (int)Math.Ceiling(t.GetSpellDamage(SpellName) * Power * 0.1f);
        }

        protected IEnumerator DoBurstDamage(float duration = 4f, float onInterval = 1f, bool knockback = true)
        {
            var raycasting = new PhysicsHelpers();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (var c in colliders)
            {
                if (c.GetComponent<IAttackInteractable>() != null)
                {
                    var obj = c.gameObject.GetComponent<IAttackInteractable>();
                    obj.Interact();
                }
            }
                
            while (duration > 0f)
            {
                foreach (Character t in raycasting.GetCharactersWithinRadius2D(transform.position, 0.5f))
                {
                    if (t is Manabu)
                        continue;
                    int dmg = GetEffectiveSpellDamage(t);
                    var targetRotaion = t.transform.rotation;
                    GameManager._instance._messageFX.DisplayDamagePopup(-dmg, t.transform.position, targetRotaion,
                        false);
                    if (t is TutorialSlime)
                    {
                        var ts = t as TutorialSlime;
                        if (!ts._hasBeenHitWithSpell) { 
                            ts._hasBeenHitWithSpell = true;
                            ts.ResolveHitWithSpell();
                        }
                        break;
                    }
                    if (!(t is TutorialSlime))
                        t.TakeDamage(transform, dmg);
                    else
                        t.TakeDamage(transform, dmg, false, true);
                        //t.AdjustStat(CharacterStats.HP, -dmg);
                    AudioManager._instance.PlaySoundEffect(_burstSE);
                    //if (knockback && t.GetComponent<Rigidbody2D>() != null)
                    //{
                    //    var physicsHelper = new PhysicsHelpers();
                    //    //StartCoroutine(physicsHelper.ProcessKnockback2D(gameObject.transform, t.transform));
                    //    GameManager._instance.ProcessKnockBack(transform, t.transform);
                    //}
                }
                duration -= onInterval;
                yield return new WaitForSeconds(onInterval);
            }
        }

        protected IEnumerator DoBurstDamage(List<Character> targets, float duration = 4f, float onInterval = 1f, bool knockback = true)
        {
            while (duration > 0f)
            {
                foreach (Character t in targets)
                {
                    if (t == null)
                        continue;
                    int dmg = t.GetSpellDamage(SpellName);
                    var targetRotaion = t.transform.rotation;
                    GameManager._instance._messageFX.DisplayDamagePopup(-dmg, t.transform.position, targetRotaion,
                        false);
                    t.AdjustStat(CharacterStats.HP, -dmg);
                    AudioManager._instance.PlaySoundEffect(_burstSE);
                    if (knockback)
                    {
                        var physicsHelper = new PhysicsHelpers();
                        //StartCoroutine(physicsHelper.ProcessKnockback2D(transform, t.transform));
                        GameManager._instance.ProcessKnockBack(transform, t.transform);
                    }
                }
                duration -= onInterval;
                yield return new WaitForSeconds(onInterval);
            }
        }

        public abstract int GetRequiredMP();

    }
}

