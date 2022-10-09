using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager _instance = null;

        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _loopTrack;

        public float _lowPitch = 0.40f;
        public float _highPitch = 1.05f;
        public bool _pauseBGM;

        public void StopSETrack()
        {
            _sfxSource.Stop();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += this.OnLevelFinishedLoading;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnLevelFinishedLoading;
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {

        }

        private void Awake()
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _loopTrack = gameObject.AddComponent<AudioSource>();

            _loopTrack.loop = true;

            if (_instance == null)
                _instance = this;
            //else if (_instance != this)
            //    Destroy(gameObject);
            //DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            //if (_pauseBGM && _bgmSource.isPlaying)
            //    PauseBGM();
            //if (!_pauseBGM && !_bgmSource.isPlaying)
            //    StartBGM();
        }

        public IEnumerator PlaySoundEffectWithBGMFade(AudioClip se, float bgmVolume = 0.5f)
        {
            float duration = se.length;
            float originalVol = _bgmSource.volume;
            _bgmSource.volume = bgmVolume;
            PlaySoundEffect(se);
            while (duration > 0f)
            {
                duration -= Time.unscaledDeltaTime;
                yield return null;
            }
            _bgmSource.volume = originalVol;
        }

        public void PlayCriticalHitSoundEffect()
        {
            AudioClip clip = Resources.Load<AudioClip>(@"Audio/SE/Battle/se_critical_hit");
            PlaySoundEffect(clip, true);

        }
        public void PlayTakeDamageSoundEffect()
        {
            AudioClip clip = Resources.Load<AudioClip>(@"Audio/SE/Items/Weapons/Dax/connect2");
            PlaySoundEffect(clip, true);
        }
        public void PlaySoundEffect(AudioClip ac, bool withPitchVariance = false)
        {
            if (ac == null)
                return;
            if (withPitchVariance)
                _sfxSource.pitch = GetRandomPitch();
            //if (Object.FindObjectOfType<WorldManager>() != null)
            //    if (Object.FindObjectOfType<WorldManager>()._addEcho)
            //        _sfxSource.reverbZoneMix

            _sfxSource.PlayOneShot(ac);
            _sfxSource.pitch = 1f;
            
        }

        public void PlayLoopingSoundEffect(AudioClip ac)
        {
            _loopTrack.clip = ac;
            _loopTrack.Play();
        }

        public void StopLoopingSoundEffectTrack()
        {
            _loopTrack.Stop();
            _loopTrack.clip = null;
        }
        /// <summary>
        /// Loops a sound effect using the object's audio source
        /// </summary>
        /// <param name="audioSource">The audio source form the object emitting the sound</param>
        /// <param name="ac"></param>
        /// <param name="noOfTimes"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public IEnumerator LoopSoundEffectLocal(AudioSource audioSource, AudioClip ac, int noOfTimes = -1, float interval = 1f)
        {
            audioSource.clip = ac;
            while (noOfTimes > 0 || noOfTimes < 0)
            {
                audioSource.Play();
                yield return new WaitForSeconds(interval);
                noOfTimes = noOfTimes > 0 ? noOfTimes -= 1 : noOfTimes;
            }

        }

        /// <summary>
        /// Loops a sound effect using the global Audio Manager's audio source
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="noOfTimes"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public IEnumerator LoopSoundEffectGlobal(AudioClip ac, int noOfTimes = -1, float interval = 1f)
        {
            while (noOfTimes > 0 || noOfTimes < 0)
            {
                PlaySoundEffect(ac);
                yield return new WaitForSeconds(interval);
                noOfTimes = noOfTimes > 0 ? noOfTimes -= 1 : noOfTimes;
            }

        }

        public void StartBGM()
        {
            _bgmSource.Play();
        }

        public void PauseBGM()
        {
            _bgmSource.Pause();
        }

        public void PauseAllSound(bool setting)
        {
            if (_bgmSource.clip != null)
            {
                if (setting) _bgmSource.Pause();
                else _bgmSource.Play();
            }
            if (_loopTrack.clip != null)
            {
                if (setting) _loopTrack.Pause();
                else _loopTrack.Play();
            }
            if (_sfxSource.clip != null)
            {
                if (setting) _sfxSource.Pause();
                else _sfxSource.Play();
            }
        }

        public void StopBGM()
        {
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }

        public void PlaySong(AudioClip ac, bool loop = true)
        {
            _bgmSource.loop = loop;
            _bgmSource.clip = ac;
            _bgmSource.Play();
        }

        public void PlaySongAfterSeconds(AudioClip ac, float afterSeconds = 2f)
        {
            StartCoroutine(InitPlayAfterDelay(ac, afterSeconds));
        }

        public IEnumerator InitPlayAfterDelay(AudioClip ac, float afterSeconds)
        {
            yield return new WaitForSeconds(afterSeconds);
            PlaySong(ac);
        }

        public float GetRandomPitch()
        {
            float randPitch = Random.Range(_lowPitch, _highPitch);
            return _sfxSource.pitch = randPitch;
        }
        public IEnumerator FadeOutBGM(float tweenTime)
        {
            var timer = tweenTime;
            while (timer > 0f)
            {
                _bgmSource.volume -= Time.deltaTime / tweenTime;
                timer -= Time.deltaTime;
                yield return null;
            }
            _bgmSource.Stop();
            _bgmSource.volume = 1f;
        }
        public IEnumerator FadeBGM(float targetVolume, float tweenTIme)
        {
            float timer = tweenTIme;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                if (_bgmSource.volume < targetVolume)
                    _bgmSource.volume += Time.deltaTime / tweenTIme;
                else
                {
                    _bgmSource.volume -= Time.deltaTime / tweenTIme;
                }
                yield return null;

            }
        }

    }
}

