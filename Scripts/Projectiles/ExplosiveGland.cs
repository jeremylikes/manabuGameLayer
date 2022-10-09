using Characters;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using Spells;
using Effects;

namespace Projectiles
{
    public class ExplosiveGland : MonoBehaviour, ISpellAffectedObject
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _speed = 0.5f;
        [SerializeField] private float _explosionTimer = 1f;
        [SerializeField] private float _blastRadius = 0.1f;
        [SerializeField] private int _damageAmount = 30;
        [SerializeField] bool _explosionStarted = false;
        [SerializeField] bool _stunned = false;
        [SerializeField] private AudioClip _explosionWarning;
        [SerializeField] private AudioClip _explosion;

        public Action _onGlandDestroyed;

        void Start()
        {
            _target = GameManager._instance._mainCharacter.transform;
        }

        void Update()
        {
            if (/*!_explosionStarted && */!_stunned)
                transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * _speed);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var candidate = collision.gameObject.GetComponent<Character>() ?? null;
            if (candidate != null && !_stunned && !_explosionStarted)
            {
                StartCoroutine(InitExplosion(candidate));
            }
        }

        private IEnumerator InitExplosion(Character triggeringCharacter)
        {
            _explosionStarted = true;
            var fx = GameManager._instance.gameObject.GetComponent<SpecialEffects>();
            AudioManager._instance.PlaySoundEffect(_explosionWarning);
            StartCoroutine(fx.FlashSpriteOnce(GetComponent<SpriteRenderer>()));
            _speed *= 0.85f;
            var anim = GetComponent<Animator>();
            anim.SetTrigger("explode");
            yield return new WaitForSeconds(_explosionTimer);
            AudioManager._instance.PlaySoundEffect(_explosion);
            var origin = transform.position;
            var hits = Physics2D.OverlapCircleAll(origin, _blastRadius);
            foreach (var hit in hits)
            {
                var target = hit.gameObject.GetComponent<Character>() ?? null;
                if (target != null && !(target is Surge))
                {
                    target.TakeDamage(transform, _damageAmount);
                }
            }
            yield return new WaitForSeconds(0.5f);
            _onGlandDestroyed?.Invoke();
            Destroy(gameObject);
        }

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Stop;
        }

        public void ReactToSpell()
        {
            StartCoroutine(InitStun());
        }

        private IEnumerator InitStun()
        {
            _stunned = true;
            yield return new WaitForSeconds(5f);
            _stunned = false;
        }
    }

}