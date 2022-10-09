using Controllers;
using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Calculators;
using Managers;
using System.IO;
using Spells;

namespace Characters
{
    public class IceWraith : Character
    {

        [SerializeField] GameObject _iceSparkPrefab;
        private bool _iceSparkReady = true;

        public override string GetName()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            var name = Resources.Load($"Messages/Characters/Enemies/IceWraith/{langCode}/iceWraithName") as TextAsset;
            return name.text;
        }

        private void Start()
        {
            SetBaseStats();
        }

        // May or may not move this up to the character class if we want to calculate these the same way we did for Manabu
        // For now, let's just do what we need
        public override void SetBaseStats()
        {
            _isAlive = true;
            _stats[CharacterStats.HP] = new Stat(50, 50);
            _stats[CharacterStats.MP] = new Stat(40, 40);
            _stats[CharacterStats.Speed] = new Stat(0.4f, 0.4f);
            _stats[CharacterStats.Stamina] = new Stat(30, 30);
            _stats[CharacterStats.Defense] = new Stat(10, 10);
            _stats[CharacterStats.Strength] = new Stat(15, 15);
            _stats[CharacterStats.Resolve] = new Stat(10, 10);
            _stats[CharacterStats.Sight] = new Stat(2, 2);
        }

        public override CharacterTypes GetCharacterType()
        {
            return CharacterTypes.Enemy;
        }

        public override string GetPrefabPath()
        {
            return @"Prefabs/Characters/Enemies/IceWraith";
        }

        public override void Attack(Character target)
        {
            AudioClip ac = Resources.Load<AudioClip>(@"Audio/SE/Characters/IceWraith/teleport");
            GetComponent<AudioSource>().PlayOneShot(ac);
            StartCoroutine(SwordAttack(target));
        }

        public override bool HasSpecials()
        {
            return true;
        }

        public override void InitSpecialAttack()
        {
            if (_iceSparkReady == true)
            {
                _iceSparkReady = false;
                StartCoroutine(SummonIceSparks());
            }
        }

        public IEnumerator SummonIceSparks()
        {
            var spark = Instantiate(_iceSparkPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(10f);
            _iceSparkReady = true;
        }

        private IEnumerator SwordAttack(Character target)
        {
            var boxCol = GetComponent<BoxCollider2D>();
            boxCol.enabled = false;
            var anim = GetComponent<Animator>();
            anim.SetBool("attack", true);
            var targetPosAtTimeOfWarmup = target.transform.position;
            //yield return new WaitForSeconds(0.7f);
            float moveTimer = 0.7f;
            while (moveTimer > 0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosAtTimeOfWarmup, Time.deltaTime * 1.5f);
                moveTimer -= Time.deltaTime;
                yield return null;
            }
            var controller = GetComponent<EnemyAI>();
            var dir = controller.GetCurrentDirectionVector();
            var hits = Physics2D.CircleCastAll(transform.position, 0.3f, dir, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject.GetComponent<Character>() == target)
                {
                    target.TakeDamage(transform, 15);
                    break;
                }
            }
            anim.SetBool("attack", false);
            GetComponent<EnemyAI>()._attackFinished = true;
            boxCol.enabled = true;
        }

        public override int GetSpellDamage(SpellNames spellName)
        {
            switch (spellName)
            {
                case SpellNames.Fireball:
                    return 30;
                default:
                    return 0;
            }
        }
    }

}
