using Characters;
using Geography;
using Interactables;
using Managers;
using Projectiles;
using Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class BossAI : MonoBehaviour
    {
        [SerializeField] private GameObject _iceSpellPrefab;
        [SerializeField] private Transform _orbsParentObject;
        [SerializeField] private int _solvedOrbs;
        [SerializeField] private List<IceOrb> _iceOrbs = new List<IceOrb>();
        [SerializeField] private Transform _iceShield;
        [SerializeField] private Transform _outerBarrier;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [SerializeField] private GameObject _fireVortexPrefab;
        [SerializeField] private Transform _deathParticles;
        [SerializeField] private GameObject _bigIceSpark;
        [SerializeField] private Transform _spawnedObjectsContainer;
        [SerializeField] private Transform _finalRadiant;
        [SerializeField] private AudioClip _radiantAppearBGM;
        [SerializeField] private AudioClip _surgeDeathSE;
        [SerializeField] private AudioClip _surgeScreamSE;
        [Space(10)]
        [SerializeField] private List<int> _stageHPThresholds = new List<int>();
        [SerializeField] private int _currentStage;
        [SerializeField] private Character _thisCharacter;
        [SerializeField] private bool _testMode = false;
        [SerializeField] private bool _allThresholdsPassed = false;
        private bool _setupStarted = false;
        private enum SpecialAttacks { vortex, freeze };
        private SpecialAttacks _currentAttackMode = SpecialAttacks.freeze;
        private bool _specialAttackReady = true;

        private Coroutine _iceSpellCoroutine = null;
        private Coroutine _vortexCoroutine = null;
        private bool _specialAttackChangeOver = false;
        private int _vortexesActive = 0;
        private int _wraithCount = 0;

        public Vector3 _targetPos = Vector3.zero;
        private float _restTime;
        //private const int _maxGlands = 1;
        private bool _spawnGland = true;
        [SerializeField] private List<GameObject> _glands = new List<GameObject>();

        public Action _onSurgeDeath;

        private void OnValidate()
        {
            _thisCharacter = GetComponent<Character>();
        }

        private void Start()
        {
            foreach (var orb in _iceOrbs)
            {
                orb._onBeamObstructed += () => UpdateSolvedOrbs(1);
                orb._onBeamUnobstructed += () => UpdateSolvedOrbs(-1);
            }

        }

        private void Update()
        {
            if (!_allThresholdsPassed && !_testMode)
                MonitorHPGates();
            switch (_currentStage)
            {
                case 0:
                    PerformStageOne();
                    break;
                case 1:
                    PerformStageTwo();
                    break;
                case 2:
                    PerformStageThree();
                    break;
                default:
                    break;
            }

        }

        public void UpdateSolvedOrbs(int amount)
        {
            _solvedOrbs += amount;
            if (_solvedOrbs < 0)
                _solvedOrbs = 0;
            if (_solvedOrbs >= _iceOrbs.Count)
            {
                DisableOrbsAndBarrier();
            }
        }

        private void MonitorHPGates()
        {
            var currHp = _thisCharacter.GetCharacterStat(CharacterStats.HP)._current;
            if (_thisCharacter.GetCharacterStat(CharacterStats.HP)._current <
                _stageHPThresholds[_currentStage])
                _currentStage++;
            if (_currentStage > _stageHPThresholds.Count - 1)
            {
                _allThresholdsPassed = true;
            }
        }

        public IEnumerator InitSurgeDeath()
        {
            var audio = GetComponent<AudioSource>();
            audio.clip = _surgeScreamSE;
            audio.Play();
            GetComponent<BoxCollider2D>().enabled = false;
            AudioManager._instance.PlaySoundEffect(_surgeDeathSE);
            _onSurgeDeath?.Invoke();
            //if (ControlsManager._instance._currentControlSchema == ControlsManager.ControlSchema.DisableMovement)
            //    ControlsManager._instance.SetActiveControls();
            AudioManager._instance.StopBGM();
            AudioManager._instance.PlaySong(_radiantAppearBGM, false);
            var originalAudioClip = FindObjectOfType<MapManager>().MainBGM;
            AudioManager._instance.PlaySongAfterSeconds(originalAudioClip, _radiantAppearBGM.length);
            ControlsManager._instance.SetLockedControls();
            StopAllCoroutines();
            DisposeOfSpawnedObjects();
            GetComponent<Animator>().SetTrigger("die");
            yield return new WaitForSeconds(1f);
            _deathParticles.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            _finalRadiant.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            ControlsManager._instance.SetActiveControls();
            Destroy(gameObject);
        }

        private void DisposeOfSpawnedObjects()
        {
            foreach (Transform t in _spawnedObjectsContainer)
            {
                Destroy(t.gameObject);
            }
        }

        private void PerformStageOne()
        {
            //if (_glands.Count < _maxGlands)
            if (_spawnGland)
                InitExplosiveGland();
        }

        private void InitExplosiveGland()
        {
            _spawnGland = false;
            //var spawnPoint = transform.position + new Vector3(0f, -0.5f, 0f);
            var spawnPoint = transform.position;
            var gland = Instantiate(Resources.Load<GameObject>($@"Prefabs/Projectiles/ExplosiveGland"), spawnPoint, Quaternion.identity);
            _glands.Add(gland);
            gland.transform.parent = _spawnedObjectsContainer;
            //gland.GetComponent<ExplosiveGland>()._onGlandDestroyed = () => _glands.Remove(gland);
            gland.GetComponent<ExplosiveGland>()._onGlandDestroyed = () => _spawnGland = true;
        }

        private void PerformStageTwo()
        {
            if (!_setupStarted)
            {
                _setupStarted = true;
                StartCoroutine(InitIceOrbs());
                SpawnWasp();
            }

        }

        private void DisableOrbsAndBarrier()
        {
            _orbsParentObject.gameObject.SetActive(false);
            _outerBarrier.gameObject.SetActive(false);
            InitIceShield();
            StartCoroutine(SpawnIceWraiths());
        }

        private void SpawnWasp()
        {
            var waspPref = Resources.Load<GameObject>(@"Prefabs/Characters/Enemies/Wasp");
            var wasp = Instantiate(waspPref, _enemySpawnPoints[1].position, Quaternion.identity);
            wasp.transform.parent = _spawnedObjectsContainer;
        }

        private IEnumerator SpawnIceWraiths()
        {
            var wraithCount = 0;
            List<GameObject> wraiths = new List<GameObject>();
            while (wraithCount < 3)
            {
                var prefab = Resources.Load<GameObject>(@"Prefabs/Characters/Enemies/IceWraith");
                var wraith = (Instantiate(prefab, _enemySpawnPoints[wraithCount].position, Quaternion.identity));
                wraith.transform.parent = _spawnedObjectsContainer;
                ToggleWraithActivity(wraith, false);
                wraiths.Add(wraith);
                wraithCount++;
                //wraith.GetComponent<IceWraith>().OnCharacterDeath += CheckSetWraithRespawnTimer;
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(2f);
            foreach (var wraith in wraiths)
            {
                ToggleWraithActivity(wraith, true);
                yield return new WaitForSeconds(0.5f);
            }

        }

        private void CheckSetWraithRespawnTimer()
        {
            _wraithCount--;
            if (_wraithCount < 1)
            {
                StartCoroutine(SpawnWraithsAfterTime());
            }
        }

        private IEnumerator SpawnWraithsAfterTime()
        {
            yield return new WaitForSeconds(15f);
            StartCoroutine(SpawnIceWraiths());
        }

        private void ToggleWraithActivity(GameObject wraith, bool setting)
        {
            wraith.GetComponent<EnemyAI>().enabled = setting;
            wraith.GetComponent<BoxCollider2D>().enabled = setting;
        }

        private IEnumerator InitIceOrbs()
        {
            // Orbs
            _orbsParentObject.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);

            // Shield
            _iceShield.gameObject.SetActive(true);
            _iceShield.gameObject.GetComponent<IceShield>().enabled = false;
            _iceShield.gameObject.GetComponent<BoxCollider2D>().enabled = false;

            // Barrier
            _outerBarrier.gameObject.SetActive(true);

            // Can't hit Surge until outer barrier and Shield are dealt with
            GetComponent<BoxCollider2D>().enabled = false;
        }

        private void InitIceShield()
        {
            _iceShield.gameObject.GetComponent<IceShield>().enabled = true;
            _iceShield.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            _iceShield.gameObject.GetComponent<IceShield>()._onDestroyed = ResolveIceShield;
        }

        private void ResolveIceShield()
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }

        private void PerformStageThree()
        {
            if (!_specialAttackChangeOver)
            {
                _specialAttackChangeOver = true;
                StartCoroutine(ManageSpecialAttackToggle());
            }
            if (_specialAttackReady)
            {
                if (_currentAttackMode == SpecialAttacks.freeze)
                    TriggerFreezeAttack();
                else
                    TriggerVortexes();
            }
        }

        private IEnumerator ManageSpecialAttackToggle()
        {
            float resetAttackAfterSeconds = UnityEngine.Random.Range(10,15);
            yield return new WaitForSeconds(resetAttackAfterSeconds);
            _currentAttackMode = _currentAttackMode == SpecialAttacks.freeze ? SpecialAttacks.vortex : SpecialAttacks.freeze;
            _specialAttackChangeOver = false;
        }

        private IEnumerator ResetSpecialAttackTimer()
        {
            float afterTime = UnityEngine.Random.Range(5, 10);
            yield return new WaitForSeconds(afterTime);
            _specialAttackReady = true;
        }

        private void TriggerFreezeAttack()
        {
            _specialAttackReady = false;
            _iceSpellCoroutine = StartCoroutine(InitFreezeSequence());
        }

        private void TriggerVortexes()
        {
            _specialAttackReady = false;
            _vortexesActive = UnityEngine.Random.Range(1, 3);
            _vortexCoroutine = StartCoroutine(InitFireVortexes());
        }

        public void StopIceSpellRoutine()
        {
            GameManager._instance._mainCharacter._movementDisabled = false;
            if (_iceSpellCoroutine != null)
                StopCoroutine(_iceSpellCoroutine);
        }

        public void SummonBigIceSpark()
        {
            var spark = Instantiate(_bigIceSpark, transform.position, Quaternion.identity);
            spark.transform.parent = _spawnedObjectsContainer;
        }

        private IEnumerator InitFireVortexes()
        {
            var manabu = GameManager._instance._mainCharacter;
            for (int i = 0; i < _vortexesActive; i++)
            {
                var fireVortexObj = Instantiate(_fireVortexPrefab, _enemySpawnPoints[i].position, Quaternion.identity);
                fireVortexObj.transform.parent = _spawnedObjectsContainer;
                //  fireVortex.transform.position = manabu.transform.position;
                FireVortex vortex = fireVortexObj.GetComponent<FireVortex>();
                vortex._onVortexDestoryed += RemoveVortex;
                yield return new WaitForSeconds(1f);
                fireVortexObj.GetComponent<BoxCollider2D>().enabled = true;
                // wait a few before spawning the next one
                if (i < _vortexesActive)
                    yield return new WaitForSeconds(2f);
            }


        }

        private void RemoveVortex()
        {
            _vortexesActive--;
            if (_vortexesActive <= 0)
                ResetAttack();
        }

        private void ResetAttack()
        {
            StartCoroutine(ResetSpecialAttackTimer());
        }

        private IEnumerator InitFreezeSequence()
        {
            var manabu = GameManager._instance._mainCharacter;
            var iceSpell = Instantiate(_iceSpellPrefab, manabu.transform.position, Quaternion.identity);
            iceSpell.transform.parent = _spawnedObjectsContainer;
            iceSpell.GetComponent<SurgeIcePrison>()._onPrisonMelted = ResetAttack;
            iceSpell.GetComponent<SurgeIcePrison>()._onPrisonMelted += StopIceSpellRoutine;
            yield return new WaitForSeconds(1f);
            if (ControlsManager._instance._currentControlSchema == ControlsManager.ControlSchema.Cast)
                manabu.GetComponent<Managers.CharacterController>().CancelCast();
            manabu.GetComponent<Managers.CharacterController>().InterruptAttackDelayRoutine();
            ControlsManager._instance.SetDisabledMovementControls();
            manabu._movementDisabled = true;
            SummonBigIceSpark();

        }

    }
    

}
