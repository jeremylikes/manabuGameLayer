using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using System.IO;
using Effects;
using UnityEngine.SceneManagement;
using GameSystems;

namespace ScriptedEvents
{
    public class TransportCutscene : MonoBehaviour
    {
        private DialogueManager _dialogue;
        [SerializeField] private Animator _anim;
        [SerializeField] private SpriteFade _outroOverlay;
        [SerializeField] private AudioSource _ambiantTrack;
        [SerializeField] private AudioSource _SETrack;
        [SerializeField] private AudioClip _warningSE;
        [SerializeField] private AudioClip _crashSE;
        [SerializeField] private Transform _globalLight;
        [SerializeField] private Transform _dynamicLights;

        private void Awake()
        {
            if (SaveSystem.VerifySaveFilesExist())
                SceneManager.LoadScene("Title");
        }
        private void Start()
        {
            _dialogue = GetComponent<DialogueManager>();
            StartCoroutine(InitCutscene());
        }

        private IEnumerator InitCutscene()
        {
            yield return new WaitForSeconds(4f);
            var path = FileManagement.MessagesDialogueDirectory;
            _dialogue.TriggerConversation($"{path}/PrologueSoldiers/0");
            _dialogue._onDialogueEnded = TriggerOutro;
        }

        public void TriggerOutro()
        {
            _anim.SetTrigger("crash");
            InvokeRepeating("PlayRepeatingWarningSound", 0f, 2f);
            // outro coroutine
            StartCoroutine(InitEnginePitchSlide());
            StartCoroutine(InitCrash());
        }
        private IEnumerator InitEnginePitchSlide()
        {
            float raisePitchTimer = 4f;
            while (raisePitchTimer > 0f)
            {
                _ambiantTrack.pitch += Time.deltaTime;
                raisePitchTimer -= Time.deltaTime;
                yield return null;
            }
        }

        public void PlayRepeatingWarningSound()
        {
            _SETrack.PlayOneShot(_warningSE);
        }

        public IEnumerator InitCrash()
        {
            _globalLight.gameObject.SetActive(false);
            _dynamicLights.gameObject.SetActive(true);
            yield return new WaitForSeconds(8f);
            _outroOverlay.enabled = true;
            _outroOverlay.transform.Find("Light").gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _SETrack.PlayOneShot(_crashSE);
            yield return new WaitForSeconds(4f);
            CancelInvoke();
            _ambiantTrack.Stop();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Prologue");
        }
    }
}

