using System;
using Items;
using UnityEngine;
using GameSystems;
using System.Collections.Generic;
using Interactables;
using Managers;
using Calculators;
using Effects;
using UI;
using UnityEngine.UI;
using System.Text;
using Geography;
using Spells;
using System.Runtime.Remoting.Messaging;
using Inventory;
using Collectables;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

namespace Characters
{
    public class Manabu : Character
    {
        [Space(20)]
        [SerializeField] private bool _testMode = false;
        private LevelSystem _levelSystem;
        public AudioClip _insufficentMP;
        public LearningPoint _activeBondingPoint;
        [SerializeField] ParticleSystem _dust;
        [SerializeField] AudioClip _enchantmentSE;
        [SerializeField] private AudioClip _dashSE;
        [SerializeField] private Spell _enchantment;
        [Space(20)]
        public KanjiInventory _kanjiInventory;
        public EquipmentInventory _equipmentInventory;
        public ItemInventory _itemInventory;
        public List<DaxExpansion> _expansionInventory = new List<DaxExpansion>();
        public bool _castEnabled = true;

        private int _shimmer = 0; // this is money
        private const int MAX_SHIMMER = 999;
        private Coroutine _enchantmentCoroutine;

        private bool _allowEnchantments = true;
        public bool _movementDisabled = false;
        public Spell GetActiveEnchanment()
        {
            return _enchantment;
        }

        public bool AllowEnchantments { get => _allowEnchantments; set => _allowEnchantments = value; }

        void Awake()
        {
            _levelSystem = new LevelSystem();
            _levelSystem.OnExperienceChanged += GainExperience;
            _levelSystem.OnLevelGained += GainLevel;
        }

        void Start()
        {
            if (SceneManager.GetActiveScene().name == "Prologue" || !GameStateManager._instance._playerDataWasSaved)
                SetBaseStats();
            if (!_equipmentInventory.GetEquipmentInventory().Contains(CollectableNames.Dax))
            {
                var dax = new Dax();
                _equipmentInventory.AddToEquipmentInventory(dax);
                Equip(dax);
            }
            if (_testMode)
                InitWithSpecialConditions();
        }

        void InitWithSpecialConditions()
        {
            Debug.Log("Starting Manabu with starter items");
            AddToKanjiInventory("hiFire");
            //AddToKanjiInventory("shiStop");
            //AddToKanjiInventory("ishiStone");
            //AddToKanjiInventory("mizuWater");
            //AddToKanjiInventory("kooriIce");
            //AddDaxExpansionToInventory(new GrappleHook());
            int liquidReds = 5;
            for (; liquidReds > 0; liquidReds--)
            {
                _itemInventory.AddToItemInventory(new LiquidRed());
            }
            var liquidBlue = new LiquidBlue();
            var liquidBlue2 = new LiquidBlue();
            //var waterkey = new WaterKey();
            _itemInventory.AddToItemInventory(liquidBlue);
            _itemInventory.AddToItemInventory(liquidBlue2);
            //_itemInventory.AddToItemInventory(waterkey);
            //_itemInventory.AddToItemInventory(waterkey);
        }

        public void AddDaxExpansionToInventory(DaxExpansion daxExp)
        {
            _expansionInventory.Add(daxExp);
        }

        public GrappleHook GetDaxGrappleHook()
        {
            return (GrappleHook)_expansionInventory.Find(x => x is GrappleHook);
        }

        public void AbsorbEnchantment(Spell enchantment)
        {
            if (_enchantmentCoroutine != null)
                StopCoroutine(_enchantmentCoroutine);
            _enchantment = enchantment;
            AudioManager._instance.PlaySoundEffect(_enchantmentSE);
            _enchantmentCoroutine = StartCoroutine(StartEnchantmentCountdown());
            // maybe some kind of flash or particle impolosion effect
        }

        private IEnumerator StartEnchantmentCountdown()
        {
            yield return new WaitForSeconds(10f);
            _enchantment = null;
        }

        public void AdjustShimmer(int amount)
        {
            int result = _shimmer + amount;
            if (result < 0)
                result = 0;
            if (result > MAX_SHIMMER)
                result = MAX_SHIMMER;
            _shimmer = result;
        }

        public void Equip(Equipment eq)
        {
            _equipmentInventory.Equip(eq);
            eq.AdjustStats(this);
        }

        public void Unequip(Equipment eq)
        {
            _equipmentInventory.Unequip(eq);

            eq.AdjustStats(this, true);
        }

        public Weapon GetEquippedWeapon()
        {
            return (Weapon)_equipmentInventory.GetEquippedItemAtSlot(EquippableSlots.Weapon);
        }



        public void CreateDust()
        {
            _dust.Play();
        }

        /// <summary>
        /// Get the list of Kanji that Manabu has learned
        /// </summary>
        /// <returns></returns>
        public List<string> GetKanjiInventory()
        {
            return _kanjiInventory.GetKnownKanji();
        }

        public void AddToKanjiInventory(string newKanji)
        {
            _kanjiInventory.AddToKnownKanji(newKanji);
        }

        public override string GetName()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            var name = Resources.Load ($"Messages/Characters/Playable/Manabu/{langCode}/manabuName") as TextAsset;
            return name.text;
        }

        public void GainExperience(object sender, EventArgs args)
        {

        }

        public void GainLevel(object sender, EventArgs args)
        {

        }

        /// <summary>
        /// The raw stats based on phsyical level before gear modifications
        /// </summary>
        public override void SetBaseStats()
        {
            _isAlive = true;
            float baseHP = _levelSystem.CurrentLevel == 1 ? 100 : (_levelSystem.CurrentLevel * 10) + 100;
            float baseMP = _levelSystem.CurrentLevel == 1 ? 50 : (_levelSystem.CurrentLevel * 5) + 20;
            float baseSpeed = _levelSystem.CurrentLevel == 1 ? 1.2f : 1.7f;
            float baseStamina = _levelSystem.CurrentLevel == 1 ? 20 : (int)Math.Round((_levelSystem.CurrentLevel * 1.5) + 20);
            float baseStrength = _levelSystem.CurrentLevel == 1 ? 2 : (int)Math.Round((_levelSystem.CurrentLevel * 1.5) + 20);
            float baseDef = _levelSystem.CurrentLevel == 1 ? 0 : (int)Math.Round((_levelSystem.CurrentLevel * 1.1) + 10);
            float baseResolve = _levelSystem.CurrentLevel == 1 ? 5 : (int) Math.Round((_levelSystem.CurrentLevel * 1.1) + 2);
            _stats[CharacterStats.HP] = new Stat(baseHP, baseHP);
            _stats[CharacterStats.MP] = new Stat(baseMP, baseMP);
            _stats[CharacterStats.Speed] = new Stat(baseSpeed, baseSpeed);
            _stats[CharacterStats.Stamina] = new Stat(baseStamina, baseStamina);
            _stats[CharacterStats.Strength] = new Stat(baseStrength, baseStrength);
            _stats[CharacterStats.Defense] = new Stat(baseDef, baseDef);
            _stats[CharacterStats.Resolve] = new Stat(baseResolve, baseResolve);
            //GameObject.Find("StatsPanel").GetComponent<StatsManager>().SetStats();

        }

        public override CharacterTypes GetCharacterType()
        {
            return CharacterTypes.PC;
        }

        public override void Attack(Character target)
        {
            //int damage = GetCharacterStat(CharacterStats.Strength)._current;
            bool wasCritical;
            int damage = Calculators.GlobalCalculator.CalculateDamage(this, target,  out wasCritical);
            //target.AdjustStat(CharacterStats.HP, -damage);
            if (_enchantment != null && target != null)
            {
                StartCoroutine(_enchantment.ExecuteEnchantmentEffect(target, damage));
                return;
            }
            if (target.GetCharacterType() == CharacterTypes.Boss)
            {
                target.TakeDamage(transform, damage, wasCritical, false, true);
                return;
            }
            target.TakeDamage(transform, damage, wasCritical);
        }

        public override string GetPrefabPath()
        {
            throw new NotImplementedException();
        }

        public override int GetSpellDamage(SpellNames spellName)
        {
            switch (spellName)
            {
                default:
                    return 0;
            }
        }
    }
}
