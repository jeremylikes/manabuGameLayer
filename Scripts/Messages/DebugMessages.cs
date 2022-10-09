using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Messages 
{
    public class DebugMessages : MonoBehaviour
    {

        public void PrintStats(Character c)
        {
            PrintCharacterNameHeader(c);
            Debug.Log("STATS:");
            foreach (var kvp in c.Stats)
                Debug.Log($"{kvp.Key}: {kvp.Value._current} / {kvp.Value._max}");
            
        }

        private void PrintCharacterNameHeader(Character c) => Debug.Log($"==== {c.GetName()} ====");

    }
}

