using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{

    public static class FileManagement
    {
        public static readonly string MessagesDirectory = $@"Messages/{GameStateManager._instance.GetCurrentLanguageCode()}";
        public static readonly string MessagesDialogueDirectory = $@"{MessagesDirectory}/Dialogue";
        public static readonly string MessagesUIDirectory = $@"{MessagesDirectory}/UI";
        public static readonly string MessagesCharactersDirectory = $@"{MessagesDirectory}/Characters";
        public static readonly string MessagesCollectablesDirectory = $@"{MessagesDirectory}/Collectables";
        public static readonly string MessagesRadiants = $@"{MessagesDirectory}/Radiants";

    }
}

