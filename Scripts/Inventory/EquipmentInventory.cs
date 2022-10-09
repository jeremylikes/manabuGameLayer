using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collectables;
using System;
using UnityEngine.UIElements;
using TMPro;
using Characters;
using UI;
using static UI.Menu;
using System.Linq;
using Managers;
using System.IO;

namespace Inventory
{
    public class EquipmentInventory : MonoBehaviour
    {
        [SerializeField] private Menu _equipmentMenu;
        [SerializeField] Transform _equippedPanel;
        [SerializeField] EquipmentSlot[] _equippedSlots;
        [SerializeField] EquipmentSlot[] _equipmentInventorySlots;
        private List<Equipment> _equipmentInventory = new List<Equipment>();
        private const int MAX_SLOTS = 15;
        private Dictionary<EquippableSlots, Equipment> _equippedItems = new Dictionary<EquippableSlots, Equipment>()
        {
            { EquippableSlots.Weapon, null },
            { EquippableSlots.Head, null },
            { EquippableSlots.Body, null },
            { EquippableSlots.Hands, null },
            { EquippableSlots.Legs, null },
            { EquippableSlots.Feet, null },
        };

        public List<Equipment> GetRawEquipmentInenvtory() => _equipmentInventory;

        public void ClearEquipmentData()
        {
            _equipmentInventory.Clear();
            
            foreach (var key in _equippedItems.Keys.ToList())
            {
                _equippedItems[key] = null;
            }
        }


        public List<CollectableNames> GetEquipmentInventory()
        {
            var eqList = new List<CollectableNames>();
            foreach (var eq in _equipmentInventory)
            {
                eqList.Add(eq.GetReferenceName());
            }

            return eqList;
        }

        public List<EquipmentData> GetEquippedItems()
        {
            
            var eqList = new List<EquipmentData>();
            foreach (var eq in _equippedItems)
            {
                if (eq.Value == null)
                    break;
                var data = new EquipmentData();

                data._slot = eq.Key.ToString();
                data._equipment = eq.Value.GetReferenceName();
                eqList.Add(data);
            }
            return eqList;
        }

        public Action _onEquipmentChanged;

        public Equipment GetEquippedItemAtSlot(EquippableSlots es)
        {
            return _equippedItems[es];
        }


        public void RenderEquipmentInventoryToMenu()
        {
            for (int i = 0; i < _equipmentInventorySlots.Length; i++)
            {
                if (i < _equipmentInventory.Count)
                {
                    _equipmentInventorySlots[i]._contents = _equipmentInventory[i];
                    _equipmentInventorySlots[i]._slotText.text = _equipmentInventory[i].GetName();
                    foreach (var kvp in _equippedItems)
                    {
                        if (kvp.Key == _equipmentInventorySlots[i]._slot && kvp.Value != null)
                        {
                            _equipmentInventorySlots[i].SetEquipContents(true);
                            break;
                        }
                    }
                }
                else
                {
                    _equipmentInventorySlots[i]._contents = null;
                    _equipmentInventorySlots[i]._slotText.text = "";
                }
            }

        }

        public void RenderEquippedItemsToSlots()
        {
            foreach (var equip in _equippedItems)
            {
                foreach (var slot in _equippedSlots)
                {
                    if (slot._slot == equip.Key)
                    {
                        if (equip.Value != null)
                        {
                            slot._slotText.text = equip.Value.GetName();
                        }
                        else
                        {
                            slot._slotText.text = "";
                        }
                        break;
                    }

                }
            }
        }

        public bool AddToEquipmentInventory(Equipment eq)
        {
            if (_equipmentInventory.Count < MAX_SLOTS)
            {
                _equipmentInventory.Add(eq);

                foreach(var slot in _equipmentInventorySlots)
                {
                    if (slot._contents == null)
                    {
                        slot._contents = eq;
                        slot._slot = eq.GetEquipmentSlot();
                        break;
                    }
                }
                return true;
            }
            else
            {
                DisplayEquipmentListIsFull();
                return false;
            }
        }
        private void DisplayEquipmentListIsFull()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            string pathToInventoryFullMessage = $"Messages/UI/System/{langCode}/equipmentInventoryFull";
            SystemMessageManager._instance.TriggerSystemMessageFromFilePath(pathToInventoryFullMessage);
        }

        public void DropEquipmentFromInventory(Equipment eq)
        {
            if (_equipmentInventory.Contains(eq))
            {
                foreach (var value in _equippedItems.Values)
                    if (value == eq)
                        Unequip(eq);
                _equipmentInventory.Remove(eq);

                RenderEquipmentInventoryToMenu();
            }
        }

        public void Equip(Equipment equipment)
        {
            if (_equipmentInventory.Contains(equipment))
            {
                foreach (var kvp in _equippedItems)
                {
                    if (kvp.Key == equipment.GetEquipmentSlot())
                    {
                        _equippedItems[kvp.Key] = equipment;

                        _onEquipmentChanged?.Invoke();
                        // We COULD make this slightly faster by updating JUST the slot that needs updating
                        RenderEquippedItemsToSlots();
                        break;
                    }
                }
            }

        }

        public void Unequip(Equipment eq)
        {
            foreach (var kvp in _equippedItems)
            {
                if (kvp.Key == eq.GetEquipmentSlot())
                {
                    _equippedItems[kvp.Key] = null;
                    _onEquipmentChanged?.Invoke();
                    // We COULD make this slightly faster by updating JUST the slot that needs updating
                    RenderEquippedItemsToSlots();
                    break;
                }
            }
        }
    }

    [Serializable]
    public class EquipmentData
    {
        public string _slot;
        public CollectableNames _equipment;
    }

}
