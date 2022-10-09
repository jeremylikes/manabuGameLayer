using GameSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class SaveMenuManager : MonoBehaviour
{
    [SerializeField] private SaveTile[] _saveTiles;
    [SerializeField] private GameStateManager _gameStateManager;
    private void OnValidate()
    {
        _saveTiles = gameObject.GetComponentsInChildren<SaveTile>();
    }

    private void Awake()
    {
        _gameStateManager = FindObjectOfType<GameStateManager>();
    }

    public void InitTiles()
    {
        for (int i = 0; i < _saveTiles.Length; i++)
        {
            _saveTiles[i].RenderSaveInfoToTile();
        }
    }
}
