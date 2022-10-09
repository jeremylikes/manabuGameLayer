using Controllers;
using Managers;
using Spells;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Characters
{
    public class Slime : Character
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private GameObject _spikePrefab;
        [SerializeField] private AudioClip _spawnSound;
        [SerializeField] private AudioClip _slimeAttack;

        private void OnValidate()
        {
            _anim = GetComponent<Animator>();
        }

        private void Start()
        {
            _spikePrefab = Resources.Load<GameObject>(@"Prefabs/Characters/Enemies/slimeSpike");
            SetBaseStats();
            AudioManager._instance.PlaySoundEffect(_spawnSound);
        }

        public override void SetBaseStats()
        {
            _isAlive = true;
            _stats[CharacterStats.HP] = new Stat(15, 15);
            _stats[CharacterStats.MP] = new Stat(0, 0);
            _stats[CharacterStats.Speed] = new Stat(0.2f, 0.2f);
            _stats[CharacterStats.Stamina] = new Stat(30, 30);
            _stats[CharacterStats.Defense] = new Stat(7, 7);
            _stats[CharacterStats.Strength] = new Stat(15, 15);
            _stats[CharacterStats.Resolve] = new Stat(10, 10);
            _stats[CharacterStats.Sight] = new Stat(3, 3);
        }

        public override void Attack(Character target)
        {
            StartCoroutine(PerformSpikeAttack(target));
        }

        private IEnumerator PerformSpikeAttack(Character target)
        {
            AudioManager._instance.PlaySoundEffect(_slimeAttack);
            _anim.SetTrigger("attack");
            Vector3 targetPosAtWarmup = target.transform.position;
            yield return new WaitForSeconds(1f);
            var spike = Instantiate(_spikePrefab, targetPosAtWarmup, Quaternion.identity);
            var enemyAI = GetComponent<EnemyAI>() ?? null;
            if (enemyAI != null)
                GetComponent<EnemyAI>()._attackFinished = true;
        }

        public override CharacterTypes GetCharacterType()
        {
            return CharacterTypes.Enemy;
        }

        public override string GetName()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            var name = Resources.Load($"Messages/Characters/Enemies/Slime/{langCode}/slimeName") as TextAsset;
            return name.text;
        }
        
        public override string GetPrefabPath()
        {
            return @"Prefabs/Characters/Enemies/Slime";
        }

        public override int GetSpellDamage(SpellNames spellName)
        {
            switch (spellName)
            {
                case SpellNames.Fireball:
                    return 20;
                default:
                    return 0;
            }
        }
    }

}
