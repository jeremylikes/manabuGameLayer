using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;
using Managers;
using TMPro;
using System;
using Messages;
using System.IO;

namespace Interactables
{
    public class TutorialLearningPoint : LearningPoint, IInteractable
    {
        private string _targetMessageFile;
        protected override void Awake()
        {
            base.Awake();
            _targetMessageFile = $"intro_help_{ GameStateManager._instance.CurrentPlatform}";
        }

        public new void Interact()
        {
            var path = $"{FileManagement.MessagesUIDirectory}/Help/{_targetMessageFile}";
            var unity = Resources.Load(path) as TextAsset;
            var lines = unity.text.Split('\n');
            ControlsManager._instance.SetBondControls();
            SystemMessageManager._instance.TriggerSystemMessage(lines);
            SystemMessageManager._instance._onSystemMessageEnded = InitializeBondAttempt;
        }

    }
}


