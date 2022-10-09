using GameSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class LoadMenuManager : MonoBehaviour
    {
        [SerializeField] List<Button> _loadGameSlots;

        private void Start()
        {
            SetLoadTileText();
        }

        private void SetLoadTileText()
        {
            for (int i = 0; i < _loadGameSlots.Count; i++)
            {
                _loadGameSlots[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = SaveSystem.GetSaveTileData(i);
            }
        }

    }

}
