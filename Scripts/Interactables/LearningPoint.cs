using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;
using Managers;
using TMPro;
using System;
using Messages;
using KanjiUnity;

namespace Interactables
{
    public class LearningPoint : MonoBehaviour, IInteractable
    {
        public AudioClip _hummingSound;
        public AudioClip _bondStart;
        public AudioClip _bondFail;
        public AudioClip _bondSuccess;
        public AudioClip _bondAttemptMusic;
        public string _learnableSpell;
        //public string _kanjiSample;
        public GameObject _samplePanel;
        public AudioSource _audioSource;
        public float _timeLimit;
        //public Object _sampleImageDirectory;
        protected Sprite[] _sampleAnimationImages;
        protected float _timer;
        public bool _attemptActive = false;
        protected float _notificationRadius = 1.5f;
        protected string _kanjiAnimationParentDirectory = "Sprites/Radiants/anim/";
        protected Coroutine _currentCoroutine;
        protected GameObject _radiantNotification;
        protected Manabu _manabu;
        protected bool _kanjiLearned = false;
        public Sprite _radiantImage;
        [TextArea]
        [SerializeField] protected string _learnedMessage;
        [SerializeField] private Transform _reflectionSprite;
        public string _kanjiString;

        public Action _onKanjiLearned;

        protected virtual void Awake()
        {
            //_audioSource.clip = _hummingSound;
            //_audioSource.Play();
            var file = $@"{FileManagement.MessagesRadiants}/{_kanjiString}";
            var text = Resources.Load<TextAsset>(file).text;
            _learnedMessage = text;
        }

        public string LearnedMessage => _learnedMessage;

        private void Start()
        {
            _sampleAnimationImages = Resources.LoadAll<Sprite>(
                _kanjiAnimationParentDirectory + _learnableSpell + "/");
            _timeLimit = 10f;
            _manabu = FindObjectOfType<Manabu>();

        }

        private void Update()
        {
            if (_attemptActive)
            {
                //if (_radiantNotification)
                //    ToggleNotification(false);
                _timer += Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Escape) || _timer >= _timeLimit)
                {
                    _attemptActive = false;
                    CancelBondAttempt();
                }
            }
        }

        public void InitializeBondAttempt()
        {
            //ToggleNotification(false);
            if (!_attemptActive)
                _attemptActive = true;
            AudioManager._instance.PauseBGM();
            _manabu._activeBondingPoint = this;
            GameManager._instance._canvasManager.ToggleKanjiPanel();
            ControlsManager._instance.SetBondControls();
            AudioManager._instance.FadeBGM(0.5f, 0.5f);
            AudioManager._instance.PlaySoundEffect(_bondStart);
            SwapBGMTrack(_bondAttemptMusic);
            _currentCoroutine = StartCoroutine(RenderStrokesToKanjiCanvas(_sampleAnimationImages));
        }

        public void CancelBondAttempt()
        {
            _manabu._activeBondingPoint = null;
            GameManager._instance._canvasManager.ToggleKanjiPanel();
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _timer = 0f;
            ControlsManager._instance.SetActiveControls();
            StopAllCoroutines();//this only kills the coroutines on THIS behavior
            AudioManager._instance.FadeBGM(1f, 0.5f);
            SwapBGMTrack(_hummingSound);
            AudioManager._instance.PlaySoundEffect(_bondFail);
            //Take care of the visuals
            ToggleSamplePanel(false);
            if (_attemptActive)
                _attemptActive = false;
            AudioManager._instance.StartBGM();
        }

        protected void ToggleSamplePanel(bool setting)
        {
            _samplePanel.SetActive(setting);
        }

        public void ConcludeSuccessfulBond()
        {
            _kanjiLearned = true;
            _attemptActive = false;
            _manabu._activeBondingPoint = null;
            GameManager._instance._canvasManager.ToggleKanjiPanel();
            //GameObject.Find("KanjiPanel").GetComponent<DrawKanji>().Reset();
            StopAllCoroutines();
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            AudioManager._instance.FadeBGM(1f, 05f);
            _audioSource.Stop();
            AudioManager._instance.PlaySoundEffect(_bondSuccess);
            ToggleSamplePanel(false);
            DisableVisualElements();
            DialogueManager._instance.RenderLearnedKanjiMessage(this);
            if (_reflectionSprite != null)
                Destroy(_reflectionSprite.gameObject);
            _onKanjiLearned?.Invoke();
            AudioManager._instance.StartBGM();
            Destroy(gameObject);
        }

        protected void DisableVisualElements()
        {
            GetComponent<SpriteRenderer>().enabled = false;
            var light = transform.Find("Light") ?? null;
            if (light != null)
                light.gameObject.SetActive(false);
        }

        public IEnumerator RenderStrokesToKanjiCanvas(Sprite[] sprites)
        {
            int index = 0;
            ToggleSamplePanel(true);
            while (index < sprites.Length)
            {
                _samplePanel.GetComponent<Image>().sprite = sprites[index];
                index++;
                if (index == sprites.Length)
                {
                    index = 0;
                    yield return new WaitForSeconds(0.5f);
                }

                yield return new WaitForSeconds(0.05f);
            }
        }

        //protected void OnMouseDown()
        //{
        //    if (!_attemptActive)
        //    {
        //        InitializeBondAttempt();
        //    }
        //}

        protected void SwapBGMTrack(AudioClip newClip)
        {
            _audioSource.clip = newClip;
            _audioSource.Play();
        }

        protected void ToggleNotification(bool setting)
        {
            if (!setting && _radiantNotification)
               Destroy(_radiantNotification);
            else
            {
                Vector2 startPos = new Vector2(_manabu.transform.position.x, _manabu.transform.position.y);
                //var startRotation = _manabu.transform.rotation;
                var initPos = new Vector3(_manabu.transform.position.x, _manabu.transform.position.y + 0.5f, 0f);
                var initRot = _manabu.transform.rotation;
                _radiantNotification = (GameObject)Instantiate(Resources.Load(@"Prefabs/RadiantNotification"), initPos, initRot);
                _radiantNotification.transform.parent = _manabu.transform;
            }

        }

        //bool CheckForManabu()
        //{
        //    Collider2D[] colliders2d = Physics2D.OverlapCircleAll(transform.position, _notificationRadius);
        //    Collider[] colliders = Physics.OverlapSphere(transform.position, _notificationRadius);
        //    foreach (Collider2D c in colliders2d)
        //    {
        //        if (c.GetComponent<Manabu>())
        //            return true;
        //    }
        //    return false;
        //}

        public void Interact()
        {
            if (!_attemptActive)
            {
                InitializeBondAttempt();
            }
            else
                CancelBondAttempt();
        }

    }
}


