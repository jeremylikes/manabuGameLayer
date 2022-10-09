using Characters;
using Collectables;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class EquipmentSlot : MonoBehaviour
    {
        public EquippableSlots _slot;
        public Equipment _contents;
        public TextMeshProUGUI _slotText;
        public bool _isEquipped = false;
        [SerializeField] Transform _equipIcon;
        [SerializeField] CanvasManager _canvasManager;
        [SerializeField] EquipSubMenu _subMenu;

        private void OnValidate()
        {
            _slotText = GetComponentInChildren<TextMeshProUGUI>();
            _canvasManager = GetComponentInParent<CanvasManager>();
        }

        public void InitSubMenu()
        {
            if (_contents != null)
            {
                _subMenu.SetEquipText(!_isEquipped);
                _subMenu._equipButton.onClick.RemoveAllListeners();
                _subMenu._equipButton.onClick.AddListener(ToggleEquipContents);
                _subMenu.GetComponent<Menu>().DisplayMenu(_subMenu.GetComponent<Menu>());
                _subMenu._dropButton.onClick.RemoveAllListeners();
                _subMenu._dropButton.onClick.AddListener(DropEquipment);

            }
        }

        
        public void SetEquipContents(bool setting)
        {
            _isEquipped = setting;
            if (_contents != null)
            {
                if (_isEquipped)
                {
                    //GameManager._instance._mainCharacter.Equip(_contents);
                    _subMenu.SetEquipText(false);
                }
                else
                {
                    //GameManager._instance._mainCharacter.Unequip(_contents);
                    _subMenu.SetEquipText(true);
                }
                ToggleEquipIcon(_isEquipped);

            }
        }
        public void ToggleEquipContents()
        {

            _isEquipped = !_isEquipped;
            if (_contents != null)
            {
                if (_isEquipped)
                {
                    GameManager._instance._mainCharacter.Equip(_contents);

                    _subMenu.SetEquipText(false);
                }
                else
                {
                    GameManager._instance._mainCharacter.Unequip(_contents);
                    _subMenu.SetEquipText(true);
                }
                ToggleEquipIcon(_isEquipped);

            }
        }

        private void DropEquipment()
        {
            if (_contents != null)
            {
                if (_contents.IsSpecial())
                {
                    var langCode = GameStateManager._instance.GetCurrentLanguageCode();
                    string messagePath = $"Messages/UI/System/{langCode}/specialCollectableWarning";
                    SystemMessageManager._instance.TriggerSystemMessageFromFilePath(messagePath);
                    return;
                }
                var einv = GameManager._instance._mainCharacter._equipmentInventory;
                if (einv.GetEquippedItemAtSlot(_contents.GetEquipmentSlot()) == _contents)
                {
                    einv.Unequip(_contents);
                    if (_equipIcon != null)
                        Destroy(_equipIcon.gameObject);
                }

                einv.DropEquipmentFromInventory(_contents);
                ControlsManager._instance._currentMenu.CloseMenu();
            }
        }

        private void ToggleEquipIcon(bool setting)
        {
            if (setting)
            {
                var icon = Instantiate(_canvasManager._equipIcon);
                _equipIcon = icon.transform;
                icon.gameObject.transform.SetParent(transform);
                var leftEdge = GetComponent<RectTransform>().rect.width / 2f;
                var offset = leftEdge * 1.5f;
                var originalPos = transform.position;
                icon.transform.position = new Vector3(originalPos.x + offset, originalPos.y, originalPos.z);
                //icon.transform.position = transform.position;
            }

            else
            {
                Destroy(_equipIcon.gameObject);
            }

        }
    }

}