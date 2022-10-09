using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public class GemRootNetwork : PuzzleCondition
    {

        [SerializeField] private List<GemRoot> _siblingGemroots;

        public void AllSiblingsActive()
        {
            if (_siblingGemroots.Count <= 0)
                return;

            foreach (var sibling in _siblingGemroots)
            {
                foreach (var gr in _siblingGemroots)
                    if (!gr.Active)
                        return;

            }
            _conditionIsMet = true;
            UpdatePuzzleConditions();
        }

        private void Update()
        {
            if (!_conditionIsMet) AllSiblingsActive();
        }
        
    }

}
