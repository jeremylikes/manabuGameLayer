using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Messages;
using Characters;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.Analytics;
using Effects;
using Interactables;
using System.Threading;

namespace Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager _instance = null;
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private Text _speakerText;
        [SerializeField] private Text _dialogueText;
        
        //public Image _dialoguePortrait;
        private string[] _dialogueLines;
        private int _dialogueIndex;
        public Action _onDialogueAdvanced;
        public Action _onDialogueEnded;
        public Action _onKanjiLearned;
        private const string SPEAKER_TOKEN = "[speaker]";
        [SerializeField] private bool _cutscene = false;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        private void Update()
        {
            if (_cutscene)
            {
                if (Input.GetKeyDown(KeyCode.Space) && _dialogueLines != null)
                    AdvanceDialogue();
            }
        }

        public void TriggerConversation(string pathToCurrentDialogueFile)
        {
            if (!_cutscene)
            {
                ControlsManager._instance.SetDialogueControls();
                if (GameManager._instance._mainCharacter._isMoving)
                {
                    GameManager._instance._mainCharacter.GetComponent<CharacterController>()._anim.SetBool("moving", false);
                    GameManager._instance._mainCharacter._isMoving = false;
                }
            }
            var t = Resources.Load(pathToCurrentDialogueFile) as TextAsset;
            _dialogueLines = t.text.Split('\n');
            _dialogueIndex = 0;
            ToggleDialogPanel(true);
            AdvanceDialogue();
        }

        public void TriggerConversation(string[] lines)
        {
            if (!_cutscene)
                ControlsManager._instance.SetDialogueControls();
            if (GameManager._instance._mainCharacter._isMoving)
            {
                GameManager._instance._mainCharacter.GetComponent<CharacterController>()._anim.SetBool("moving", false);
                GameManager._instance._mainCharacter._isMoving = false;
            }
            _dialogueLines = lines;
            _dialogueIndex = 0;
            ToggleDialogPanel(true);
            AdvanceDialogue();
        }

        public void RenderSingleMessage(string message)
        {
            ToggleDialogPanel(true);
            ControlsManager._instance.SetSystemMessageControls();
            ClearDialogText();
            _dialogueText.text = message;
        }

        public void RenderLearnedKanjiMessage(LearningPoint lp)
        {
            ControlsManager._instance.SetKanjiLearnedControls();

            ClearDialogText();
            ToggleDialogPanel(true);

            var sfx = GameObject.Find("Managers").GetComponent<SpecialEffects>();
            StartCoroutine(sfx.ZoomIn(GameManager._instance._defaultCameraOrthoSize - 1f, 1.5f));

            //Display the learned radiant
            GameManager._instance._headsUpImage.sprite = lp._radiantImage;
            GameManager._instance._headsUpImage.gameObject.SetActive(true);

            //Display dialogue box with congrats text
            _dialogueText.text = lp.LearnedMessage;

        }

        public void EndKanjiLearnedSequence()
        {
            var sfx = GameObject.Find("Managers").GetComponent<SpecialEffects>();

            //Close dialogue
            EndDialogue();

            //Close HeadsUp Image
            GameManager._instance._headsUpImage.gameObject.SetActive(false);
            GameManager._instance._headsUpImage.sprite = null;
            StartCoroutine(sfx.ZoomOut(GameManager._instance._defaultCameraOrthoSize));
            ControlsManager._instance.SetActiveControls();
            _onKanjiLearned?.Invoke();
            ////Dispose of in-game Radiant
            ////_onLearnedKanjiCutsceneFinsihed?.Invoke(this, System.EventArgs.Empty);
            //Destroy(learningPoint.gameObject);
            ////learningPoint.gameObject.SetActive(false);
        }

        public void AdvanceDialogue()
        {
            if (_dialogueLines == null)
                return;
            if (_dialogueLines.Length - 1 < _dialogueIndex)
            {
                EndDialogue();
                return;
            }
            DisplayLine();
            _dialogueIndex++;
            _onDialogueAdvanced?.Invoke();
        }


        public void EndDialogue()
        {
            ClearDialogText();
            ToggleDialogPanel(false);
            _dialogueLines = null;
            _dialogueIndex = 0;
            if (!_cutscene)
                ControlsManager._instance.SetActiveControls();
            _onDialogueAdvanced = null;
            _onDialogueEnded?.Invoke();
            _onDialogueEnded = null;
        }

        public void ClearDialogText()
        {
            _dialogueText.text = "";
            _speakerText.text = "";
        }

        public void ToggleDialogPanel(bool isActive)
        {
            if (!_cutscene)
            {
                AudioManager._instance.PlaySoundEffect(isActive ?
                (AudioClip)Resources.Load("Audio/SE/UI/dialogue_open") :
                (AudioClip)Resources.Load("Audio/SE/UI/dialogue_close"));
            }

            _dialoguePanel.SetActive(isActive);
            //if (!isActive)
            //    _dialoguePortrait.sprite = null;
        }

        public void DisplayLine()
        {
            if (_dialogueLines[_dialogueIndex].Contains(SPEAKER_TOKEN))
            {
                var str = _dialogueLines[_dialogueIndex];
                _speakerText.text = str.Replace(SPEAKER_TOKEN, "");
                _dialogueIndex++;
            }
            _dialogueText.text = _dialogueLines[_dialogueIndex];
        }
    }

}
