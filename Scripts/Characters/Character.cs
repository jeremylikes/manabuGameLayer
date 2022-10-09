using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Items;
using System;
using Managers;
using System.Collections;
using System.Net.Mime;
using GameSystems;
using TMPro;
using UnityEngine.UI;
using Spells;
using Controllers;
using System.Linq;
using System.Security.Cryptography;
using Effects;
using System.Threading;
using Calculators;
using Geography;
using UnityEngine.Assertions.Must;
using System.Xml.Schema;
using System.IO;
using Collectables;
using Interactables;
//using System.Diagnostics;

namespace Characters
{
    public enum CharacterTypes
    {
        NPC, PC, Enemy, Boss
    }

    public enum CharacterStats
    {
        HP, MP, Stamina, Speed, Defense, Strength, Resolve, Sight
    }

    public enum EquippableSlots
    {
        Weapon, Head, Body, Hands, Legs, Feet
    }

    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected float _currentHP;

        //public float _movementSpeed = 2f;
        public Action OnStatChanged;
        public Action OnCharacterDeath;
        [SerializeField] protected List<Debuff> _debuffStack = new List<Debuff>();
        [SerializeField] protected List<Buff> _buffStack = new List<Buff>();
        [SerializeField] protected bool _respawn;
        // Audio
        public List<AudioClip> _attackSound;
        public AudioClip _castSuccess;
        public AudioClip _holdNotification;
        public AudioClip _footstepNormal;
        public AudioClip _footstepWater;
        [SerializeField] protected AudioClip _footstepsSand;
        public AudioClip _reactToDamage;
        public AudioClip _aggroSound;
        //protected List<Coroutine> _specialAttacks = new List<Coroutine>();
        // Stats
        public bool _autoStaminaRegenTriggered = false;
        public bool _autoRegenTriggered;
        public GameObject _statHUD;
        public bool _isAlive;
        [SerializeField]
        private UnityEngine.UI.Image _hpBar;
        [SerializeField]
        private UnityEngine.UI.Image _mpBar;
        [SerializeField]
        private UnityEngine.UI.Image _staminaBar;
        public bool _isMoving;
        private bool _invincible = false;
        private Coroutine _holdSpellCoroutine;
        [SerializeField] protected List<CollectableDrop> _dropList;

        private List<CharacterStats> _statsBelowMax = new List<CharacterStats>();
        protected Dictionary<CharacterStats, Stat> _stats = new Dictionary<CharacterStats, Stat>()
        {
            {CharacterStats.HP, new Stat(0, 0)},
            {CharacterStats.MP, new Stat(0, 0)},
            {CharacterStats.Stamina, new Stat(0, 0)},
            {CharacterStats.Speed, new Stat(0, 0)},
            {CharacterStats.Defense, new Stat(0, 0)},
            {CharacterStats.Strength, new Stat(0, 0)},
            {CharacterStats.Resolve, new Stat(0, 0)},
            {CharacterStats.Sight, new Stat(0, 0)}
        };

        public abstract int GetSpellDamage(SpellNames spellName);

        //public void DisengageSpecialAttacks()
        //{
        //    if (_specialAttacks.Count <= 0)
        //        return;
        //    foreach (var attack in _specialAttacks)
        //    {
        //        StopCoroutine(attack);
        //    }
        //    _specialAttacks.Clear();
        //}

        public bool IsInvincible() => _invincible;

        public void SetRespawn(bool setting)
        {
            _respawn = setting;
        }

        public virtual bool HasSpecials()
        {
            return false;
        }

        public virtual void InitSpecialAttack()
        {
            Debug.Log("No special created for this character");
        }

        public abstract string GetPrefabPath();

        public Vector3 GetStartingPos()
        {
            Vector3 startingPos = Vector3.zero;
            if (GetCharacterType() == CharacterTypes.PC)
                startingPos = GetComponent<Managers.CharacterController>()._startingPosition;
            if (GetCharacterType() == CharacterTypes.Enemy)
                startingPos = GetComponent<EnemyAI>()._startingPos;
            // eventually another case for NPCS
            return startingPos;

        }

        public void AddDebuffToStack(Debuff debuff)
        {
            _debuffStack.Add(debuff);
        }

        public void RemoveDebuffFromStack(Debuff debuff)
        {
            _debuffStack.Remove(debuff);
            AdjustStat(debuff._impactedStat, debuff._debuffAmount);
        }

        public void AddBuffToStack(Buff buff)
        {
            _buffStack.Add(buff);
        }

        public void RemoveBuffFromStack(Buff buff)
        {
            foreach (var b in _buffStack)
            {
                if (b == buff) _buffStack.Remove(b);
                AdjustStat(buff._impactedStat, -buff._buffAmount);
            }
        }

        void Update()
        {
            MonitorPerishableStats();
            ManageBuffsAndDebuffs();
        }

        private void ManageBuffsAndDebuffs()
        {
            if (_debuffStack.Count == 0 && _buffStack.Count == 0)
                return;
            if (_debuffStack.Count > 0)
            {
                foreach (var db in _debuffStack)
                {
                    if (db._isApplied)
                        continue;

                    db._isApplied = true;
                    AdjustStat(db._impactedStat, -db._debuffAmount);
                    Debug.Log($"{db._impactedStat} => {GetCharacterStat(db._impactedStat)._current}");

                }
            }
            if (_buffStack.Count > 0)
            {
                foreach (var buff in _buffStack)
                {
                    if (buff._isApplied)
                        continue;

                    buff._isApplied = true;
                    AdjustStat(buff._impactedStat, buff._buffAmount);
                }
            }
        }

        public abstract void SetBaseStats();

        public abstract string GetName();

        public abstract CharacterTypes GetCharacterType();

        public Stat GetCharacterStat(CharacterStats targetStat)
        {
            return _stats[targetStat];
        }

        public bool CheckStatBelowMax(CharacterStats stat)
        {
            return _stats[stat]._current < _stats[stat]._max;
        }

        public Dictionary<CharacterStats, Stat> Stats { 
            get { return _stats; }
            set { _stats = value; }
        }

        public void AdjustStat(CharacterStats targetStat, float delta, bool changeMax = false)
        {
            float max = _stats[targetStat]._max;
            float current = _stats[targetStat]._current;
            bool isMpOrHp = targetStat == CharacterStats.HP || targetStat == CharacterStats.MP;
            if (!changeMax)
            {
                if (delta >= 0)
                    _stats[targetStat]._current += (current + delta <= max) ? delta : max - current;
                else
                    _stats[targetStat]._current += (current + delta >= 0) ? delta : -current;
            }
            else
            {
                if (delta >= 0)
                {
                    max += delta;
                    _stats[targetStat]._max = max;
                    if (!isMpOrHp)
                        _stats[targetStat]._current = max;
                }
                else
                {
                    max += (max + delta > 0f) ? delta : -max;
                    _stats[targetStat]._max = max;
                    if (current > max || !isMpOrHp)
                        _stats[targetStat]._current = max;
                }

            }
            if (targetStat == CharacterStats.HP && Stats[CharacterStats.HP]._current <= 0)
                StartCoroutine(Die());
            OnStatChanged?.Invoke();
            UpdateStatBar(targetStat);
            //PlayerData.UpdatePlayerData(PlayerDataTypes.stats);
        }

        public IEnumerator InitInvincibility()
        {
            _invincible = true;
            float effectWindow = 0.4f;
            var sfx = GameObject.Find("Managers").GetComponent<SpecialEffects>();
            StartCoroutine(sfx.FlashSpriteTransparent(GetComponent<SpriteRenderer>(), effectWindow));
            yield return new WaitForSeconds(effectWindow);
            _invincible = false;
        }

        private void UpdateStatBar(CharacterStats targetStat)
        {
            UnityEngine.UI.Image barToUpdate;
            switch (targetStat)
            {
                case CharacterStats.HP:
                    barToUpdate = _hpBar;
                    break;
                case CharacterStats.MP:
                    barToUpdate = _mpBar;
                    break;
                case CharacterStats.Stamina:
                    barToUpdate = _staminaBar;
                    break;
                default:
                    return;
            }
            if (barToUpdate != null)
            {
                barToUpdate.fillAmount = (float)Stats[targetStat]._current /
                                    Stats[targetStat]._max;
                if (GetCharacterType() == CharacterTypes.Enemy)
                {
                    if (Stats[CharacterStats.HP]._current < Stats[CharacterStats.HP]._max && !_statHUD.activeInHierarchy)
                        _statHUD.SetActive(true);
                    else if (Stats[CharacterStats.HP]._current >= Stats[CharacterStats.HP]._max && _statHUD.activeInHierarchy)
                        _statHUD.SetActive(false);
                }

            }
        }

        public void AdjustStatBonuses(CharacterStats targetStat, float delta)
        {
            //OnStatChanged?.Invoke();
            if (targetStat == CharacterStats.HP || targetStat == CharacterStats.MP)
            {
                _stats[targetStat]._max += delta;
                if (_stats[targetStat]._current > _stats[targetStat]._max)
                    _stats[targetStat]._current = _stats[targetStat]._max;
                return;
            }
            _stats[targetStat]._max += delta;
            //_stats[targetStat]._current += delta;
            _stats[targetStat]._current = _stats[targetStat]._max;
            OnStatChanged?.Invoke();
        }

        public void SetStat(CharacterStats targetStat, float currentValue)
        {
            _stats[targetStat]._current = currentValue;
            OnStatChanged?.Invoke();
        }

        public virtual void Attack(Character target)
        {
            Debug.Log("Attack not implemented on this character");
        }

        public virtual float GetAttackAnimLength()
        {
            return 3f;
        }

        /// <summary>
        /// Trigger or simulate a character taking damage
        /// </summary>
        /// <param name="assailant">The affector</param>
        /// <param name="damage">The amount of damage</param>
        /// <param name="wasCritical">Was the attack critical</param>
        /// <param name="fakeDamage">If true, the damage will appear to be inflicted but not actually affect the victim's stats</param>
        public void TakeDamage(Transform assailant, int damage, bool wasCritical = false, bool fakeDamage = false, bool ignoreKnockback = false)
        {
            if (_invincible)
                return;
            if (this is Manabu && GetComponent<Managers.CharacterController>()._isCasting && !ignoreKnockback)
                ignoreKnockback = true;
            if (wasCritical)
            {
                AudioManager._instance.PlayCriticalHitSoundEffect();
            }

            else
            {
                //AudioManager._instance.PlayTakeDamageSoundEffect();
                AudioClip ac = Resources.Load<AudioClip>(@"Audio/SE/Characters/generic_take_damage_01");
                AudioManager._instance.PlaySoundEffect(ac);
            }

            if (damage > 0f)
                GameManager._instance._messageFX.DisplayDamagePopup(-damage, transform.position, Quaternion.identity,
                    wasCritical);
            var sfx = GameManager._instance.gameObject.GetComponent<SpecialEffects>();
            var sr = GetComponent<SpriteRenderer>();
            var anim = GetComponent<Animator>();
            anim.SetTrigger("takeDamage");
            var manabu = assailant.GetComponent<Manabu>() ?? null;
            if (!(this is Manabu) && manabu != null)
            {
                if (GameManager._instance._mainCharacter.CheckStatBelowMax(CharacterStats.MP))
                {
                    var spawnPos = transform.position;
                    Instantiate(Resources.Load<GameObject>(@"Prefabs/Items/ManaOrb"), spawnPos, Quaternion.identity);
                }
            }

            if (this is Manabu)
                StartCoroutine(InitInvincibility());

            //knock back
            //we want to know whether the target is N,S,E,or W of assailant
            if (damage > 0f && !ignoreKnockback && GetComponent<Rigidbody2D>() != null)
            {
                //sfx.ProcessDamageParticles(this);
                if (_reactToDamage)
                    GetComponent<AudioSource>().PlayOneShot(_reactToDamage);
                // we don't want the strokes to get messed up if manabu is casting and he gets hit
                var aiCon = GetComponent<EnemyAI>() ?? null;
                if (aiCon != null)
                {
                    StartCoroutine(aiCon.StunEnemyForSeconds(0.5f));
                }
                GameManager._instance.ProcessKnockBack(assailant, transform);
            }
            if (!fakeDamage)
                AdjustStat(CharacterStats.HP, -damage);
            if (!(this is Manabu))
                StartCoroutine(sfx.FlashSpriteOnce(sr));

        }

        public void FullyReplenish(bool withSystemMessage = true)
        {
            AudioClip ac = Resources.Load<AudioClip>(@"Audio/SE/Events/fullReplenish");
            AudioManager._instance.PlaySoundEffect(ac);

            var hpDiff = _stats[CharacterStats.HP]._max - _stats[CharacterStats.HP]._current;
            var mpDiff = _stats[CharacterStats.MP]._max - _stats[CharacterStats.MP]._current;

            AdjustStat(CharacterStats.HP, hpDiff);
            AdjustStat(CharacterStats.MP, mpDiff);

            if (withSystemMessage)
            {
                var langCode = GameStateManager._instance.GetCurrentLanguageCode();
                var unity = Resources.Load($"{FileManagement.MessagesUIDirectory}/System/fullyReplenished") as TextAsset;
                SystemMessageManager._instance.TriggerSystemMessage(unity.text.Split('\n'));
            }
        }

        public void KnockMeBack(Transform assailant)
        {
            var ph = new PhysicsHelpers();
            //StartCoroutine(ph.ProcessKnockback2D(assailant, transform));
            GameManager._instance.ProcessKnockBack(assailant, transform);
            //GetComponent<Rigidbody2D>().velocity = new Vector2(0.5f, 0.5f);
        }

        public virtual IEnumerator ProcessFootstepSE()
        {
            while (_isMoving)
            {
                PlayFootstepsSE();
                yield return new WaitForSeconds(0.3f);
            }
        }

        public void PlayFootstepsSE()
        {
            AudioClip clipToPlay = _footstepNormal;
            var activeTerrain = GetComponent<TerrainManager>().GetActiveTerrain();
            switch(activeTerrain)
            {
                case TerrainTypes.Normal:
                    clipToPlay = _footstepNormal;
                    break;
                case TerrainTypes.Water:
                    clipToPlay = _footstepWater;
                    break;
                case TerrainTypes.Sand:
                    clipToPlay = _footstepsSand;
                    break;
                default:
                    clipToPlay = _footstepNormal;
                    break;
            }

            if (clipToPlay != null)
                AudioManager._instance.PlaySoundEffect(clipToPlay, true);

        }

        public void PlayAggroSound()
        {
            if (_aggroSound != null)
                GetComponent<AudioSource>().PlayOneShot(_aggroSound);
        }

        public void InstantiateSpellAtCurrentLocation(string kanjiString, float power)
        {
            //Quaternion defaultRotation = new Quaternion(0f, 0f, 0f, 1f);
            var spellObject = Instantiate(Resources.Load<GameObject>($"Prefabs/Spells/{kanjiString}"));
            spellObject.transform.parent = transform;
            spellObject.gameObject.SetActive(false);
            if (!GameStateManager._instance._tutorialMode)
                AdjustStat(CharacterStats.MP, -spellObject.GetComponent<Spell>().GetRequiredMP());
            spellObject.transform.position = transform.position;
            spellObject.transform.parent = gameObject.transform;
            if (_holdSpellCoroutine != null)
                StopCoroutine(_holdSpellCoroutine);
            spellObject.GetComponent<Spell>().Power = power;
            _holdSpellCoroutine = StartCoroutine(HoldSpell(spellObject));
        }

        public void PlaySpellHoldNotification()
        {
            AudioManager._instance.PlaySoundEffect(_holdNotification);
            StartCoroutine(FlashCharacterSprite());
        }

        public IEnumerator FlashCharacterSprite(float holdTime = 0.05f, bool flashWhite = true)
        {
            var flashMat = flashWhite ? Resources.Load<Material>(@"Materials/pureWhite") :
                 Resources.Load<Material>(@"Materials/pureBlack");
            var sprite = GetComponent<SpriteRenderer>();
            var originalMat = sprite.material;
            sprite.material = flashMat;
            yield return new WaitForSeconds(holdTime);
            sprite.material = originalMat;
        }

        public IEnumerator HoldSpell(GameObject spellObject)
        {
            AudioManager._instance.PlaySoundEffect(_castSuccess);
            InvokeRepeating("PlaySpellHoldNotification", 0f, 3f);
            bool targetSelected = false;
            var clickedPos = new Vector3(0f, 0f, 0f);
            while (!targetSelected)
            {
                if (Input.GetMouseButtonDown(0) && ControlsManager._instance.CanCast())
                {
                    Plane plane = new Plane(-Vector3.forward, Vector3.zero);
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var mousePos2D = new Vector2(mousePos.x, mousePos.y);
                    clickedPos = new Vector3(ray.origin.x, ray.origin.y, ray.origin.z);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector3.zero);
                    bool disabledMovement = ControlsManager._instance.GetCurrentControlSchema() == ControlsManager.ControlSchema.DisableMovement;
                    if (!disabledMovement && hit && hit.collider.gameObject.GetComponent<Manabu>() != null)
                    {
                        if (spellObject.GetComponent<Spell>().CanEnchant())
                        {
                            var manabu = hit.collider.gameObject.GetComponent<Manabu>();
                            if (manabu.AllowEnchantments)
                                manabu.AbsorbEnchantment(spellObject.GetComponent<Spell>());
                        }
                    }
                    //StartCoroutine(spellObject.GetComponent<Spell>().CastAtPosition(clickedPos));
                    else
                    {
                        spellObject.GetComponent<Spell>().CastAtPosition(clickedPos);
                    }
                    targetSelected = true;
                    CancelInvoke(); // To stop the hold notification sound
                    GetComponent<Managers.CharacterController>()._anim.SetTrigger("finishCast");
                    
                    StartCoroutine(ProcessSpellLock());
                }
                yield return null;
            }
        }

        private IEnumerator ProcessSpellLock()
        {
            var originalControls = ControlsManager._instance.GetCurrentControlSchema();
            ControlsManager._instance.SetLockedControls();
            yield return new WaitForSeconds(0.7f);
            if (ControlsManager._instance.GetCurrentControlSchema() == ControlsManager.ControlSchema.DisableMovement)
                ControlsManager._instance.SetDisabledMovementControls();
            else
                ControlsManager._instance.SetControls(originalControls);
        }

        public bool IsBareHanded()
        {
            //return _equipment[EquippableSlots.LHand] == null &&
            //       _equipment[EquippableSlots.LHand] == null;
            return true;
        }

        /// <summary>
        /// Useful for initializing regen if certain stats are below max
        /// </summary>
        private void MonitorPerishableStats()
        {
            //foreach (var stat in GetPerishableStats())
            //{
            //    if (Stats[stat]._current < Stats[stat]._max && !_statsBelowMax.Contains(stat))
            //        _statsBelowMax.Add(stat);
            //}
            //if (_statsBelowMax.Count > 0 && !_autoRegenTriggered)
            //    StartCoroutine(StartStatRegen());
            _currentHP = Stats[CharacterStats.HP]._current;
            //if (!(this is Surge))
            //{
            //    if (Stats[CharacterStats.HP]._current <= 0 && _isAlive)
            //        StartCoroutine(Die());
            //}

        }

        private IEnumerator StartStatRegen()
        {
            _autoRegenTriggered = true;
            float tickSpeed = Calculators.GlobalCalculator.GetGlobalTickInterval();

            while (_statsBelowMax.Count > 0)
            {
                foreach (var stat in _statsBelowMax)
                {
                    int amountPerTick = (int)Calculators.GlobalCalculator.GetRegenValuePerTick(this, stat);
                    if (Stats[stat]._current >= Stats[stat]._max)
                    {
                        _statsBelowMax.Remove(stat);
                        break;
                    }
                    float amountToAdjust = Stats[stat]._current + amountPerTick <=
                     Stats[stat]._max
                    ? amountPerTick
                    : Stats[stat]._max - Stats[stat]._current;
                    AdjustStat(stat, amountToAdjust);
                }
                yield return new WaitForSeconds(tickSpeed);
            }
            _autoRegenTriggered = false;
        }

        public List<CharacterStats> GetPerishableStats()
        {
            return new List<CharacterStats>() { CharacterStats.HP, CharacterStats.MP, /*CharacterStats.Stamina*/ };
        }

        public int GetPhysicalLevel()
        {
            if (GetComponent<LevelSystem>() != null)
                return GetComponent<LevelSystem>().CurrentLevel;
            return 1;
        }

        public IEnumerator Die()
        {
            if (this is Surge)
            {
                StartCoroutine(GetComponent<BossAI>().InitSurgeDeath());
            }

            _isAlive = false;
            OnCharacterDeath?.Invoke();
            var puzzleCondition = GetComponent<PuzzleCondition>() ?? null;
            if (puzzleCondition != null)
                puzzleCondition._conditionIsMet = true;
            if (this is Manabu)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($@"Sprites/Characters/Manabu/anim_fall");
                GameStateManager._instance._playerDataWasSaved = false;
                GameManager._instance.TriggerGameOver();
            }

            if (GetCharacterType() == CharacterTypes.Enemy)
            {
                var con = GetComponent<EnemyAI>();
                if (_dropList.Count > 0)
                    ReleaseDrops();
                if (_respawn)
                    GameManager._instance.OnEnemyPerish(GetPrefabPath(), GetStartingPos(), con._min, con._max);
                AudioManager._instance.PlaySoundEffect(Resources.Load<AudioClip>(@"Audio/SE/Characters/enemy_death"));
                var targetRot = transform.rotation;
                StartCoroutine(FlashCharacterSprite(1f, false));
                var deathAnim = Instantiate(Resources.Load<GameObject>(@"Prefabs/DeathAnim"), transform.position, targetRot);
                deathAnim.transform.SetParent(this.transform);
                yield return new WaitForSeconds(0.5f);
                bool hpIsDepleted = GameManager._instance._mainCharacter.CheckStatBelowMax(CharacterStats.HP);
                bool mpIsDepleted = GameManager._instance._mainCharacter.CheckStatBelowMax(CharacterStats.MP);
                if (hpIsDepleted || mpIsDepleted)
                {
                    var offset = 0.1f;
                    bool offsetMe = false;
                    if (hpIsDepleted)
                    {
                        Instantiate(Resources.Load<GameObject>(@"Prefabs/Items/HealthOrb"), transform.position, Quaternion.identity);
                        offsetMe = true;
                    }
                    if (mpIsDepleted)
                    {
                        var spawnPos = offsetMe ? new Vector3(transform.position.x + offset, transform.position.y, transform.position.z) : transform.position;
                        Instantiate(Resources.Load<GameObject>(@"Prefabs/Items/ManaOrb"), spawnPos, Quaternion.identity);
                    }
                }
                Destroy(gameObject);
            }
        }

        protected virtual void ReleaseDrops()
        {
            float spawnOffset = 0.2f;
            int noOfDrops = 0;
            foreach (var drop in _dropList)
            {
                bool willDrop = GlobalCalculator.GetYesNoChance(drop._dropRate);
                if (willDrop)
                {
                    Vector3 spawnPos = new Vector3(transform.position.x + (spawnOffset * noOfDrops), transform.position.y, transform.position.z);
                    Instantiate(drop._collectable, spawnPos, Quaternion.identity);
                    noOfDrops++;
                }

            }
        }

    }

    public class Stat
    {
        public float _current;
        public float _max;

        public Stat(float current, float max)
        {
            _current = current;
            _max = max;
        }
    }

    [Serializable]
    public class Debuff
    {
        public CharacterStats _impactedStat;
        public float _debuffAmount;
        public float _duration;
        public bool _isApplied = false;
    }

    [Serializable]
    public class Buff
    {
        public CharacterStats _impactedStat;
        public float _buffAmount;
        public float _duration;
        public bool _isApplied = false;
    }
}

