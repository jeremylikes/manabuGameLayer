using Managers;
using Spells;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Characters
{
    public class TutorialSlime : Character
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private GameObject _spikePrefab;
        [SerializeField] private AudioClip _spawnSound;
        [SerializeField] private AudioClip _slimeAttack;
        [SerializeField] private PrognusOpening _prognusOpening;
        public bool _hasBeenHitWithSpell;
        public bool _hasBeenHitWithEnchantment;
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
            _stats[CharacterStats.HP] = new Stat(200, 200);
            _stats[CharacterStats.MP] = new Stat(0, 0);
            _stats[CharacterStats.Speed] = new Stat(0.2f, 0.2f);
            _stats[CharacterStats.Stamina] = new Stat(30, 30);
            _stats[CharacterStats.Defense] = new Stat(10, 10);
            _stats[CharacterStats.Strength] = new Stat(15, 15);
            _stats[CharacterStats.Resolve] = new Stat(10, 10);
            _stats[CharacterStats.Sight] = new Stat(3, 3);
        }

        public override CharacterTypes GetCharacterType()
        {
            return CharacterTypes.Enemy;
        }

        public override string GetName()
        {
            return "Slime";
        }

        public override string GetPrefabPath()
        {
            return @"Prefabs/Characters/Enemies/TutorialSlime";
        }

        public void ResolveTutorial()
        {
            ControlsManager._instance.SetCutsceneControls();
            _prognusOpening.StopKanjiStrokes();
            _prognusOpening.AdvanceDemo();
            StartCoroutine(Die());
        }

        public void ResolveHitWithSpell()
        {
            _prognusOpening.AdvanceDemo();
        }

        public override int GetSpellDamage(SpellNames spellName)
        {
            switch (spellName)
            {
                case SpellNames.Fireball:
                    return 25;
                default:
                    return 0;
            }
        }
    }

}
