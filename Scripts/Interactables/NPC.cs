using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Messages;
using System.IO;

namespace Interactables
{
    public class NPC : MonoBehaviour, IInteractable
    {

        public void Interact()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            DialogueManager._instance.TriggerConversation($"{FileManagement.MessagesDialogueDirectory}/Plyster/0");
        }

    }
}

