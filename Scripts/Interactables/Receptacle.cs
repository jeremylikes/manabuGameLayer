using Characters;
using Collectables;
using Items;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Interactables
{
    public class Receptacle : PuzzleCondition, IInteractable
    {

        [SerializeField] private CollectableNames _dependentItem;


        public void Interact()
        {
            if (!_conditionIsMet)
            {
                var manabu = FindObjectOfType<Manabu>();
                if (!manabu._itemInventory.ItemContainedInInventory(_dependentItem))
                {
                    var langCode = GameStateManager._instance.GetCurrentLanguageCode();
                    var filePath = $"{FileManagement.MessagesUIDirectory}/System/emptyPillar";
                    SystemMessageManager._instance.TriggerSystemMessageFromFilePath(filePath);
                    return;
                }
                var itemFromInventory = manabu._itemInventory.GetItemFromInventory(_dependentItem);
                if (itemFromInventory != null)
                {
                    manabu._itemInventory.UseItem(itemFromInventory, true);
                    _conditionIsMet = true;
                    var beam = transform.GetChild(0);
                    beam.gameObject.SetActive(true);
                    UpdatePuzzleConditions();
                }
            }

        }
    }
}

