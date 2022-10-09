using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using System.Linq;
using Managers;
using Characters;
using System;


namespace Interactables
{
    public class ItemCocoon : KeyPoint, IAttackInteractable
    {
        //[SerializeField] private int _currentPhase = -1, _maxPhase = 3;
        [SerializeField] private int _currentPhase = 0, _maxPhase;
        [SerializeField] private List<Phase> _phases;
        [SerializeField] private LearningPoint _contents;
        [SerializeField] private AudioClip _impactSE;
        [SerializeField] private AudioClip _popSE;
        [SerializeField] private float _spawnOffset = 0.5f;
        [SerializeField] private bool _phaseLock = false;
        [SerializeField] private Sprite[] _animSprites;
        private float _countdownTimer = 6f;
        private int _currentAnimIndex = 0;
        private Coroutine _resetTimer;
        private Color _originalSpriteColor;
        private Color _originalLightColor;

        private void Start()
        {
            _maxPhase = _phases.Count - 1;
            _originalSpriteColor = GetComponent<SpriteRenderer>().color;
            _originalLightColor = GetComponent<UnityEngine.Rendering.Universal.Light2D>().color;
            _contents._onKanjiLearned = ResolveState;
            InvokeRepeating("AlternateSprite", 2f, 2f);
        }

        private void AlternateSprite()
        {
            _currentAnimIndex += _currentAnimIndex + 1 < _animSprites.Length ? 1 : -_currentAnimIndex;
            GetComponent<SpriteRenderer>().sprite = _animSprites[_currentAnimIndex];
        }

        public void Interact()
        {
            if (!_phaseLock)
                ChangePhase();
        }

        private void ChangePhase()
        {
            //GameObject.FindObjectOfType<Manabu>().KnockMeBack(transform);
            if (_currentPhase > _maxPhase)
            {
                ReleaseContents();
                return;
            }
            ChangeColor();
            ReleaseEnemy();
            _currentPhase++;
            PlayPhaseChangeSE();

        }

        private void ResetPhases()
        {
            _currentPhase = 0;
            ChangeColor(true);
            PlayPhaseChangeSE();
        }

        private void PlayPhaseChangeSE()
        {
            if (_impactSE != null)
                AudioManager._instance.PlaySoundEffect(_impactSE);
        }

        private void ChangeColor(bool reset = false)
        {
            GetComponent<SpriteRenderer>().color = reset ? _originalSpriteColor : _phases[_currentPhase]._phaseColor;
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = reset ? _originalLightColor : _phases[_currentPhase]._phaseColor;
        }

        public void ReleaseContents()
        {
            if (_popSE != null) AudioManager._instance.PlaySoundEffect(_popSE);
            _contents.gameObject.SetActive(true);
            // Destroy(gameObject);
            gameObject.SetActive(false);
        }
        public override void ResolveState()
        {
            if (_mapManager._sceneKeypoints.Contains(this))
            {
                int kpIndex = _mapManager.GetKeyPointIndex(this);
                GameStateManager._instance.AddToResolvedKeyPoints(kpIndex);
            }
            if (_contents != null)
                Destroy(_contents.gameObject);
            Destroy(gameObject);
        }
        public void ReleaseEnemy()
        {
            var offsetVect = new Vector3(0f, _spawnOffset, 0f);
            var enemy = Instantiate(_phases[_currentPhase]._enemyToSpawn, transform.position + offsetVect, Quaternion.identity).GetComponent<Character>();
            enemy.OnCharacterDeath += UnlockPhase;
            _phaseLock = true;
            StartCoroutine(ToggleColliderForTime(enemy.gameObject, 0.2f));
            _resetTimer = StartCoroutine(ActivatePhaseResetTimer());
        }

        private IEnumerator ToggleColliderForTime(GameObject go, float time)
        {
            var col = go.GetComponent<BoxCollider2D>();
            col.enabled = false;
            yield return new WaitForSeconds(time);
            col.enabled = true;
        }

        void UnlockPhase()
        {
            StartCoroutine(UnlockCocoon());
            StopCoroutine(_resetTimer);
        }

        public IEnumerator UnlockCocoon()
        {
            yield return new WaitForSeconds(0.5f);
            _phaseLock = false;
        }
        public IEnumerator ActivatePhaseResetTimer()
        {
            yield return new WaitForSeconds(_countdownTimer);
            ResetPhases();
        }

        public IEnumerator ProcessWabble()
        {
            yield return null;
        }



        [Serializable]
        private struct Phase
        {
            public Color _phaseColor;
            public GameObject _enemyToSpawn;
        }

    }
}

