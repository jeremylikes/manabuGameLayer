using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace UI
{
    public class LoadGameTile : MonoBehaviour
    {
        [SerializeField] private int _saveFileIndex;
        [SerializeField] private GameStateManager _gameStateManager;

        private void OnValidate()
        {
            _gameStateManager = FindObjectOfType<GameStateManager>();
        }
        public void LoadFromSaveFile()
        {
            _gameStateManager.LoadAysnc(_saveFileIndex);
        }
    }
}

