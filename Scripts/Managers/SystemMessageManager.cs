using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{

    public class SystemMessageManager : MonoBehaviour
    {
        public static SystemMessageManager _instance = null;
        public ControlsManager.ControlSchema _originalControls;
        [SerializeField] private GameObject _systemMessagePanel;
        [SerializeField] private Text _systemMessageText;
        private string[] _dialogueLines;
        private int _dialogueIndex;
        public Action _onSystemMessageAdvanced;
        public Action _onSystemMessageEnded;
        private float _scrollDelay = 5f;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        public void TriggerSystemMessageFromFilePath(string pathToMessageFile)
        {
            var unity = Resources.Load(pathToMessageFile) as TextAsset;
            var lines = unity.text.Split('\n');
            if (lines.Length == 0)
                return;
            TriggerSystemMessage(lines);
        }

        public void TriggerSystemMessage(string[] lines)
        {
            _originalControls = ControlsManager._instance.GetCurrentControlSchema();
            ControlsManager._instance.SetSystemMessageControls();
            _dialogueLines = lines;
            _dialogueIndex = 0;
            ToggleSystemMessagesPanel(true);
            //StartCoroutine(AutoAdvanceSystemMessage());
            AdvanceDialogue();
        }
        private IEnumerator AutoAdvanceSystemMessage()
        {
            int noOfTimesThrough = _dialogueLines.Length + 1;
            for (; noOfTimesThrough > 0; noOfTimesThrough--)
            {
                AdvanceDialogue();
                yield return new WaitForSeconds(_scrollDelay);
            }
        }

        public void ToggleSystemMessagesPanel(bool isActive)
        {
            AudioManager._instance.PlaySoundEffect(isActive ?
                (AudioClip)Resources.Load("Audio/SE/UI/dialogue_open") :
                (AudioClip)Resources.Load("Audio/SE/UI/dialogue_close"));
            _systemMessagePanel.SetActive(isActive);
        }

        public void AdvanceDialogue()
        {
            if (_dialogueLines.Length - 1 < _dialogueIndex)
            {
                EndDialogue();
                return;
            }
            DisplayLine();
            _dialogueIndex++;
            _onSystemMessageAdvanced?.Invoke();
        }

        public void ClearDialogText()
        {
            _systemMessageText.text = "";
        }

        public void EndDialogue()
        {
            ClearDialogText();
            ToggleSystemMessagesPanel(false);
            _dialogueLines = null;
            _dialogueIndex = 0;
            //StopCoroutine(AutoAdvanceSystemMessage());
            ControlsManager._instance.SetControls(_originalControls);
            _onSystemMessageEnded?.Invoke();
            _onSystemMessageAdvanced = null;
            _onSystemMessageEnded = null;
        }

        private void DisplayLine()
        {
            var derp = _dialogueLines[_dialogueIndex];
            _systemMessageText.text = _dialogueLines[_dialogueIndex];
        }

    }

}
