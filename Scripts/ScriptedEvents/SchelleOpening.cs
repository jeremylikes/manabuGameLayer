using Calculators;
using Effects;
using Geography;
using Managers;
using ScriptedEvents;
using Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Characters
{
    public class SchelleOpening : MonoBehaviour, IAutoController
    {
        [SerializeField] private Transform _startPos;
        [SerializeField] private int _stage = 0;
        [SerializeField] private float _runSpeed;
        [SerializeField] private bool _isFleeing;
        private Animator _anim;
        [SerializeField] List<string> _animStates;
        [SerializeField] float _castAnimLength;
        [SerializeField] GameObject _kanjiTextBubblePrefab;
        [SerializeField] private SpecialEffects _fx;
        [SerializeField] private Transform _manabuPos;
        [SerializeField] private bool _rotateManabu;
        [SerializeField] private Transform _sandMask;
        [SerializeField] private BattleArena _battleArena;
        [SerializeField] private GameObject _meteorObj;
        [SerializeField] private GameObject _prognus;
        [SerializeField] private OpeningSceneManager _openingSceneManager;
        [SerializeField] private AudioClip _prognusInterruptSE;
        [SerializeField] private AudioClip _castSuccess;
        [SerializeField] private AudioClip _pushSE;
        [SerializeField] private AudioClip _chaseBGM;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _animStates.Clear();
            foreach (var p in _anim.parameters)
                _animStates.Add(p.name);
            foreach (var c in _anim.runtimeAnimatorController.animationClips)
                if (c.name.Contains("cast"))
                    _castAnimLength = c.length;
            transform.position = _startPos.position;
            _manabuPos = FindObjectOfType<Manabu>().gameObject.transform;
            _isFleeing = true;
        }
        private void Update()
        {
            if (_isFleeing && _stage < 4)
                Run();
        }

        private void Start()
        {

            AudioManager._instance.PlaySong(_chaseBGM);
        }

        private void Run()
        {
            string targetAnimParam = "running";
            // animation
            if (!_anim.GetBool(targetAnimParam))
                ToggleAnimState(targetAnimParam);

            // movement
            transform.Translate(Vector3.right * Time.deltaTime * 1.3f);
        }


        private void ToggleAnimState(string target)
        {
            foreach (var state in _animStates)
            {
                if (target == "idle")
                {
                    _anim.SetBool(state, false);
                    continue;
                }
                if (state == target)
                {
                    _anim.SetBool(target, true);
                }
                else
                    _anim.SetBool(state, false);
            }
        }

        public void WaitForEvent()
        {

            _isFleeing = false;
            ToggleAnimState("idle");
        }

        // Stage 0
        public IEnumerator CastStopAndPush()
        {
            ToggleAnimState("casting");
            StartCoroutine(_fx.EmitKanjiSpeechBubble("止", transform.position));
            ControlsManager._instance.SetLockedControls();
            Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/schelleStop"), _manabuPos.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/PushSpell"), _manabuPos.position, Quaternion.identity);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("押", transform.position));
            AudioManager._instance.PlaySoundEffect(_pushSE);
            GameManager._instance._mainCharacter.KnockMeBack(this.transform);
            _isFleeing = true;
            ControlsManager._instance.SetActiveControls();
            _stage++;
        }

        // Stage 1
        public IEnumerator CastWindAndEnemies()
        {

            ToggleAnimState("casting");
            StartCoroutine(_fx.EmitKanjiSpeechBubble("止", transform.position));
            ControlsManager._instance.SetLockedControls();
            Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/schelleStop"), _manabuPos.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            StartCoroutine(_fx.EmitKanjiSpeechBubble("風", transform.position));
            _sandMask.gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            StartCoroutine(_fx.EmitKanjiSpeechBubble("敵", transform.position));
            int count = 3;
            Vector3 spawnPoint = transform.position;
            var listOfEnemies = new List<Character>();
            while (count > 0)
            {
                spawnPoint += new Vector3(-0.4f, count < 3 ? -0.4f : 0f, 0f);
                listOfEnemies.Add(SpawnEnemySlime(spawnPoint));
                count--;
                yield return new WaitForSeconds(1f);
            }
            _battleArena.InitBattleArena(listOfEnemies);
            _isFleeing = true;
            ControlsManager._instance.SetActiveControls();
            _stage++;
        }


        // Stage 2
        private IEnumerator CastSakuraCatAndForest()
        {
            ToggleAnimState("casting");
            StartCoroutine(_fx.EmitKanjiSpeechBubble("止", transform.position));
            ControlsManager._instance.SetLockedControls();
            Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/schelleStop"), _manabuPos.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("桜", transform.position));
            StartCoroutine(CastSakura());
            yield return new WaitForSeconds(2f);
            _isFleeing = true;
            yield return new WaitForSeconds(1f);

            ControlsManager._instance.SetActiveControls();
            _stage++;
        }

        private Character SpawnEnemySlime(Vector3 atPos)
        {
            var slimePrefab = Resources.Load<GameObject>(@"Prefabs/Characters/Enemies/Slime");
            return Instantiate(slimePrefab, atPos, Quaternion.identity).GetComponent<Character>();
        }

        // Stage 3
        private IEnumerator CastFinalSpellSequence()
        {
            float wait = 2f;

            StartCoroutine(_fx.EmitKanjiSpeechBubble("止", transform.position));
            Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/schelleStop"), _manabuPos.position, Quaternion.identity);
            ControlsManager._instance.SetLockedControls();

            ToggleAnimState("casting");

            yield return new WaitForSeconds(1f);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("火", transform.position));
            StartCoroutine(CastFire());

            yield return new WaitForSeconds(1f);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("上", transform.position));
            StartCoroutine(CastGravity());

            yield return new WaitForSeconds(2f);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("氷", transform.position));
            StartCoroutine(CastIce());

            yield return new WaitForSeconds(wait * 2f);
            _fx.GeneratePortalEffectAtPosition(transform.position);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("裏返す", transform.position));
            
            yield return new WaitForSeconds(wait);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("回転", transform.position));
            StartCoroutine(CastRotate());

            yield return new WaitForSeconds(wait);
            StartCoroutine(_fx.EmitKanjiSpeechBubble("隕石", transform.position));
            StartCoroutine(CastMeteor());
            // We won't return control to Manabu for the duration
            _stage++;
        }

        private IEnumerator CastMeteor()
        {
            _meteorObj.gameObject.SetActive(true);
            yield return new WaitForSeconds(4f);
            StartCoroutine(InitPrognusIntervention());
        }

        private IEnumerator InitPrognusIntervention()
        {
            _prognus.transform.position = _manabuPos.position;
            _prognus.SetActive(true);
            yield return new WaitForSeconds(2f);
            _prognus.SetActive(false);
            _meteorObj.SetActive(false);
            AudioManager._instance.PlaySoundEffect(_prognusInterruptSE);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(_openingSceneManager.KickoffTransitionToTitle());
        }

        private IEnumerator CastGravity()
        {
            float timer = 2f;
            _manabuPos.gameObject.GetComponent<Animator>().SetBool("floating", true);
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                _manabuPos.Translate(Vector3.up * Time.deltaTime *0.25f);
                yield return null;
            }

        }

        private IEnumerator CastRotate()
        {
            _rotateManabu = true;
            Camera.main.transform.parent = null;
            while (_rotateManabu)
            {
                //_manabuPos.gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;
               _manabuPos.Rotate(Vector3.back * (Time.deltaTime * 48f));
                yield return null;
            }
        }

        private IEnumerator CastSakura()
        {
            var spellObj = Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/sakura"), _manabuPos.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            int strikes = 5;
            while (strikes > 0)
            {
                _manabuPos.GetComponent<Character>().TakeDamage(transform, 0);
                yield return new WaitForSeconds(0.2f);
                strikes--;
            }
            float audioLength = spellObj.GetComponent<AudioSource>().clip.length;
            yield return new WaitForSeconds(audioLength);
            Destroy(spellObj);
        }

        private IEnumerator CastFire()
        {
            // inst prefab
            var spellObj = Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/hiFire"), _manabuPos.position, Quaternion.identity);
            var clip = spellObj.GetComponent<Fireball>()._burstSE;
            AudioManager._instance.PlaySoundEffect(clip);
            _manabuPos.GetComponent<Character>().TakeDamage(transform, 0);
            float animLength = spellObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
            Destroy(spellObj);
        }

        private IEnumerator CastIce()
        {
            // inst prefab
            var spellObj = Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/schelleIce"), _manabuPos.position, Quaternion.identity);
            yield return new WaitForSeconds(3f);
            Destroy(spellObj);
            _manabuPos.GetComponent<Character>().TakeDamage(transform, 0);

        }

        private void PerformAttack()
        {
            switch (_stage)
            {
                case 0:
                    StartCoroutine(CastStopAndPush());
                    break;
                case 1:
                    StartCoroutine(CastWindAndEnemies());
                    break;
                case 2:
                    StartCoroutine(CastSakuraCatAndForest());
                    break;
                case 3:
                    StartCoroutine(CastFinalSpellSequence());
                    break;
                default:
                    break;
            }
        }

        public void DoPostWaitAction()
        {
            PerformAttack();
        }
    }

}
