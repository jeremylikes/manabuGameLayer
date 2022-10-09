using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using Managers;
using Effects;
using Characters;
using System;

namespace Spells
{
    public class FireVortex : MonoBehaviour, ISpellAffectedObject, IDestructable
    {
        private int _health = 20;
        public Action _onVortexDestoryed;
        private float _countdown = 10f;
        void Start()
        {
            StartCoroutine(InitCountdownSequence());
            StartCoroutine(InitColliderAfterTime());
        }

        private IEnumerator InitColliderAfterTime(float time = 2f)
        {
            yield return new WaitForSeconds(time);
            if (GetComponent<BoxCollider2D>() != null)
                GetComponent<BoxCollider2D>().enabled = true;
        }
        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    var manabu = collision.gameObject.GetComponent<Manabu>() ?? null;
        //    if (manabu != null)
        //        manabu.TakeDamage(transform, 10);
        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var manabu = collision.GetComponent<Manabu>() ?? null;
            if (manabu != null)
                manabu.TakeDamage(transform, 10);
        }

        private IEnumerator InitCountdownSequence()
        {
            yield return new WaitForSeconds(_countdown);
            GameManager._instance._canvasManager.ToggleScreenOverlay(true);
            yield return new WaitForSeconds(0.5f);
            GameManager._instance._canvasManager.ToggleScreenOverlay(false);
            GameManager._instance._mainCharacter.TakeDamage(transform, 40, true);
            DispelVortex();
        }

        private void DispelVortex()
        {
            _onVortexDestoryed?.Invoke();
            Destroy(gameObject);
        }

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Water;
        }

        public void ReactToSpell()
        {
            TakeDamage(20);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                DispelVortex();
            }
            var sr = GetComponent<SpriteRenderer>();
            StartCoroutine(GameManager._instance.gameObject.GetComponent<SpecialEffects>().FlashSpriteOnce(sr));
        }

    }

}
