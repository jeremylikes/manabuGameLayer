using Collectables;
using Inventory;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ItemSlot : MonoBehaviour
    {
        public Item _contents;
        public int _stacks;
        public TextMeshProUGUI _slotText;
        [SerializeField] ItemSubMenu _subMenu;
        private int _maxStacks = 99;
        public GameObject _stackCountText;

        public void InitSubMenu()
        {
            if (_contents != null)
            {
                _subMenu._useButton.onClick.RemoveAllListeners();
                _subMenu._useButton.onClick.AddListener(UseItemContents);
                _subMenu.GetComponent<Menu>().DisplayMenu(_subMenu.GetComponent<Menu>());
                _subMenu._dropButton.onClick.RemoveAllListeners();
                _subMenu._dropButton.onClick.AddListener(DropItemContents);
            }
        }

        public void UseItemContents()
        {
            if (_contents._isUsable == true)
            {
                //GameManager._instance._mainCharacter.GetComponent<ItemInventory>().UseItem(_contents);
                GameManager._instance._mainCharacter._itemInventory.UseItem(_contents);
                ControlsManager._instance._currentMenu.CloseMenu();
            }

        }
        public void DropItemContents()
        {
            //GameManager._instance._mainCharacter.GetComponent<ItemInventory>().UseItem(_contents, true);
            GameManager._instance._mainCharacter._itemInventory.UseItem(_contents, true);
            ControlsManager._instance._currentMenu.CloseMenu();
        }

    }
}

