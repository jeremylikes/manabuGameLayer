using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactables
{
    public class Puzzle : MonoBehaviour
    {
        [SerializeField] private PuzzleCondition[] _conditions;
        [SerializeField] private bool _puzzleSolved;
        [SerializeField] private GameObject[] _objectsToDisable;
        [SerializeField] private GameObject[] _objectsToEnable;
        [SerializeField] private AudioClip _soundOnSolve;

        public Action _onPuzzleResolved;

        private void Start()
        {
            foreach (var condition in _conditions)
                condition._onConditionMet += CheckPuzzleResolved;
        }
        
        private void CheckPuzzleResolved()
        {
            var conditionLength = _conditions.Length;
            int resolvedCount = 0;
            foreach (var condition in _conditions)
                resolvedCount += condition._conditionIsMet ? 1 : 0;
            if (resolvedCount >= conditionLength)
                ResolvePuzzle();
        }

        private void ResolvePuzzle()
        {
            _onPuzzleResolved?.Invoke();
            if (_soundOnSolve != null)
                AudioManager._instance.PlaySoundEffect(_soundOnSolve);
            _puzzleSolved = true;
            if (_objectsToDisable.Length > 0)
                foreach (var obj in _objectsToDisable)
                    obj.SetActive(false);
            if (_objectsToEnable.Length > 0)
                foreach (var obj in _objectsToEnable)
                    obj.SetActive(true);
        }

        public bool PuzzleSolved => _puzzleSolved;
    }

}
