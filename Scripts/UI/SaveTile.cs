using GameSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class SaveTile : MonoBehaviour
{
    [SerializeField] private int _saveFileIndex;

    public void Save()
    {
        GameStateManager._instance.Save(_saveFileIndex);
        RenderSaveInfoToTile();
    }

    public void RenderSaveInfoToTile()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = SaveSystem.GetSaveTileData(_saveFileIndex);

    }
}
