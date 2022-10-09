using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Managers;

namespace Inventory
{
    public class KanjiInventory : MonoBehaviour
    {
        [SerializeField] protected List<string> _knownKanji;
        [SerializeField] protected List<RadiantSlot> _radiantSlots;

        public void AddToKnownKanji(string kanjiString)
        {
            if (!_knownKanji.Contains(kanjiString))
            {
                _knownKanji.Add(kanjiString);

            }
        }

        public void ClearKanjiInventory()
        {
            _knownKanji.Clear();
        }

        public List<string> GetKnownKanji() => _knownKanji;

        public void RenderRadiantsToSlots()
        {
            for (int i = 0; i < _radiantSlots.Count; i++)
            {
                if (i < _knownKanji.Count)
                {
                    _radiantSlots[i].SetSlotKanji(_knownKanji[i]);
                }
                else
                {
                    _radiantSlots[i].SetSlotKanji("");
                }
            }
        }

    }

}
