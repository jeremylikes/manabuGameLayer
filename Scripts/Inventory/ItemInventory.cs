using Collectables;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UI;
using UnityEngine;

namespace Inventory
{
    public class ItemInventory : MonoBehaviour
    {
        [SerializeField] private Menu _itemMenu;
        [SerializeField] ItemSlot[] _itemSlots;
        [SerializeField] GameObject _stackPrefab;

        private List<ItemObject> _itemInventory = new List<ItemObject>();
        private const int MAX_STACKS = 99;
        private const int MAX_SLOTS = 15;

        public bool ItemContainedInInventory(CollectableNames name)
        {
            foreach (var itemObj in _itemInventory)
            {
                if (itemObj._item.GetReferenceName() == name)
                    return true;
            }
            return false;
        }

        public void RenderItemsToSlots()
        {
            for (int i = 0; i < _itemSlots.Length; i++)
            {
                if (i < _itemInventory.Count)
                {
                    _itemSlots[i]._contents = _itemInventory[i]._item;
                    _itemSlots[i]._slotText.text = _itemInventory[i]._item.GetName();

                    if (_itemInventory[i]._stacks > 1)
                    {
                        if (_itemSlots[i]._stackCountText == null)
                        {
                            _itemSlots[i]._stackCountText = Instantiate(_stackPrefab);
                            _itemSlots[i]._stackCountText.gameObject.transform.SetParent(_itemSlots[i].transform);
                            var originalPos = _itemSlots[i].transform.position;
                            var halfWidth = _itemSlots[i].GetComponent<RectTransform>().rect.width / 2f;
                            var offset = halfWidth * 1.5f;
                            var targetPos = new Vector3(originalPos.x + offset, originalPos.y, originalPos.z);
                            //_itemSlots[i]._stackCountText.transform.position = _itemSlots[i].transform.position;
                            _itemSlots[i]._stackCountText.transform.position = targetPos;
                        }
                        _itemSlots[i]._stackCountText.GetComponent<TextMeshProUGUI>().text = _itemInventory[i]._stacks.ToString();

                    }
                    else
                    {
                        if (_itemSlots[i]._stackCountText != null)
                        {
                            Destroy(_itemSlots[i]._stackCountText);
                        }
                    }

                }
                else
                {
                    if (_itemSlots[i]._stackCountText != null)
                    {
                        Destroy(_itemSlots[i]._stackCountText);
                    }
                    _itemSlots[i]._contents = null;
                    _itemSlots[i]._slotText.text = "";
                }
            }
        }

        public void ClearItemInventory()
        {
            _itemInventory.Clear();
        }

        public Item GetItemFromInventory(CollectableNames itemName)
        {
            try
            {
                return _itemInventory.Find(x => x._item.GetReferenceName() == itemName)._item;

            }
            catch (NullReferenceException e)
            {
                Debug.Log($"Tried to access an item that wasn't in manabu's inventory with error:\n{e}");
                return null;
            }
        }

        public List<ItemData> GetItemInventory()
        {
            var itemList = new List<ItemData>();
            foreach (var item in _itemInventory)
            {
                var data = new ItemData();
                data._item = item._item.GetReferenceName();
                data._quantity = item._stacks;
                itemList.Add(data);
            }
            return itemList;
        }

        public bool AddToItemInventory(Item item, int quantity = 1)
        {
            int proposedSlotNumber = 1;

            foreach (var itemObj in _itemInventory)
            {
                if (itemObj._item.GetType() == item.GetType() && itemObj._stacks + quantity <= MAX_STACKS)
                {
                    itemObj._stacks += quantity;

                    return true;
                }
                proposedSlotNumber++;
            }

            if (proposedSlotNumber > MAX_SLOTS)
            {
                DisplayItemListIsFull();
                return false;
            }
            else
            {
                _itemInventory.Add(new ItemObject(item, quantity));

                return true;
            }
            
        }

        private void DisplayItemListIsFull()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            string pathToInventoryFullMessage = $"Messages/UI/System/{langCode}/inventoryFull";
            SystemMessageManager._instance.TriggerSystemMessageFromFilePath(pathToInventoryFullMessage);
        }

        /// <summary>
        /// Use the item or drop it by passing in the optional drop parameter
        /// </summary>
        /// <param name="item"></param>
        /// <param name="drop">If true, item stack will decrement without using the item</param>
        public void UseItem(Item item, bool drop = false)
        {
            foreach (var itemObj in _itemInventory)
            {
                if (itemObj._item == item)
                {
                    itemObj._stacks--;
                    if (!drop)
                        item.Use(GameManager._instance._mainCharacter);
                    if (itemObj._stacks <= 0)
                    {
                        _itemInventory.Remove(itemObj);

                    }
                    RenderItemsToSlots();
                    break;
                }
            }
        }

        private class ItemObject
        {
            public Item _item;
            public int _stacks;

            public ItemObject(Item item, int stacks)
            {
                _item = item;
                _stacks = stacks;
            }
        }
    }

    [Serializable]
    public class ItemData
    {
        public CollectableNames _item;
        public int _quantity;

    }
}

