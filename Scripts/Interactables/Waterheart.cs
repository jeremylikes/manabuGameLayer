using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Managers;
using Spells;
using System;
using Effects;
using Controllers;

namespace Interactables
{
    public class Waterheart : MonoBehaviour, IInteractable, ISpellAffectedObject
    {
        [SerializeField] private bool _wasTriggered;
        [SerializeField] private Animator _waterheartAnim;
        [SerializeField] private Transform _flashEffect;
        [SerializeField] private Transform _mecklinger;
        [SerializeField] private AudioClip _waterheartRestoredSE;
        public void Interact()
        {
            RespondToManabu();
        }

        private void RespondToManabu()
        {
            string fileToLoad = _wasTriggered ? @"waterheart_postRestore" : @"waterheart_hint";
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            string fullFilePath = $"{FileManagement.MessagesUIDirectory}/System/{fileToLoad}";
            SystemMessageManager._instance.TriggerSystemMessageFromFilePath(fullFilePath);

        }

        private void ToggleWaterheart()
        {
            _wasTriggered = true;
            _waterheartAnim.enabled = true;

        }

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Water;
        }

        public void ReactToSpell()
        {
            StartCoroutine(InitWaterHeart());

        }

        private IEnumerator InitWaterHeart()
        {
            ToggleWaterheart();
            var audio = GetComponent<AudioSource>();
            audio.clip = _waterheartRestoredSE;
            audio.Play();
            ControlsManager._instance.SetLockedControls();
            _flashEffect.gameObject.SetActive(true);
            float timer = _flashEffect.GetComponent<SpriteFade>().FadeTime;
            yield return new WaitForSeconds(timer);
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            var pathToDemoCompleteMessage = $"{FileManagement.MessagesUIDirectory}/System/demoComplete";
            SystemMessageManager._instance.TriggerSystemMessageFromFilePath(pathToDemoCompleteMessage);
            SystemMessageManager._instance._onSystemMessageEnded = TriggerEndCutscene;
        }

        private void TriggerEndCutscene()
        {
            ControlsManager._instance.SetLockedControls();
            _mecklinger.gameObject.SetActive(true);
        }
    }
}

