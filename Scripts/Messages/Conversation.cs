using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Messages
{
    [Serializable]
    public class Conversation
    {
       public DialogueSet[] _dialogueSets;
    }

    [Serializable]
    public class DialogueSet
    {
        public string _speaker;
        [TextArea(3, 10)]
        public string[] _dialogueLines;
        public AudioClip[] _audioLines;
    }
}

