using Characters;
using Interactables;
using Managers;
using Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class IceSpark : MonoBehaviour, IDestructable, ISpellAffectedObject
    {
        [SerializeField] private float _speed = 0.3f;
        [SerializeField] private int _iceDamage = 10;
        [SerializeField] private AudioClip _initSE;
        [SerializeField] private float _lifetime = 5f;
        private void Start()
        {
            AudioManager._instance.PlaySoundEffect(_initSE);
        }
        void Update()
        {
            if (_lifetime <= 0f)
                Destroy(gameObject);
            _lifetime -= Time.deltaTime;
            var target = GameManager._instance._mainCharacter.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _speed);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var manabu = collision.gameObject.GetComponent<Manabu>() ?? null;
            if (manabu != null)
            {
                DoIceDamage(manabu);
            }
        }

        private void DoIceDamage(Manabu manabu)
        {
            manabu.TakeDamage(transform, _iceDamage, false, false, true);
            Destroy(gameObject);
        }

        public void TakeDamage(int amount)
        {
            Destroy(gameObject);
        }

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Fireball;
        }

        public void ReactToSpell()
        {
            Destroy(gameObject);
        }
    }

}