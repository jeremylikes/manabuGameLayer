
using Interactables;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Items;

namespace Collectables
{
    public class CollectableContainer : KeyPoint, IInteractable
    {
        [SerializeField] private CollectableContent[] _contents;
        [SerializeField] Sprite _openSprite;
        [SerializeField] bool _isOpen = false;

        public void Interact()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            if (_isOpen || _contents.Length == 0)
            {
                var pathToMessageFile = $"Messages/UI/System/{langCode}/containerIsEmpty";
                SystemMessageManager._instance.TriggerSystemMessageFromFilePath(pathToMessageFile);
                if (!_isOpen)
                    ResolveState();
                return;
            }

            string contentString = "";

            if (_contents.Length > 0)
            {
                foreach (var content in _contents)
                {
                    var thing = CollectableManager.GetCollectableByName(content._content);
                    var manabu = GameManager._instance._mainCharacter;
                    for (int i = 0; i < content._quantity; i++)
                    {
                        if (thing is Equipment)
                            manabu._equipmentInventory.AddToEquipmentInventory((Equipment)thing);
                        if (thing is Item)
                            manabu._itemInventory.AddToItemInventory((Item)thing);
                        if (thing is DaxExpansion)
                            manabu.AddDaxExpansionToInventory((DaxExpansion)thing);
                    }
                    contentString += $"{thing.GetName()} + {content._quantity}\n";
                }
            }

            SystemMessageManager._instance.TriggerSystemMessage(new[] { contentString });
            ResolveState();
        }

        public override void ResolveState()
        {
            //int kpIndex = Array.IndexOf(GameStateManager._instance.KeyPoints, this);
            if (_mapManager._sceneKeypoints.Contains(this)) {
                int kpIndex = _mapManager.GetKeyPointIndex(this);
                GameStateManager._instance.AddToResolvedKeyPoints(kpIndex);
            }
            _isOpen = true;
            ToggleBoxOpen();
        }
        private void ToggleBoxOpen()
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = _openSprite;
        }

        [Serializable]
        private class CollectableContent
        {
            public CollectableNames _content;
            public int _quantity;
        }
    }
}

