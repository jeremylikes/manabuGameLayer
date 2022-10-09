using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{

    public class PuzzleCondition : MonoBehaviour
    {
        public delegate void ConditionMetHandler();
        public event ConditionMetHandler _onConditionMet;

        public bool _conditionIsMet = false;
        private bool _lastConditionStatus = false;

        private void Update()
        {
            if (_conditionIsMet != _lastConditionStatus)
            {
                _lastConditionStatus = _conditionIsMet;
                UpdatePuzzleConditions();
            }
        }

        public void UpdatePuzzleConditions()
        {
            if (_conditionIsMet)
            {
                _onConditionMet?.Invoke();
            }
        }
    }
}

