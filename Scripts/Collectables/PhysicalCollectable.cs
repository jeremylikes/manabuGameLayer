using Characters;
using Collectables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class PhysicalCollectable : MonoBehaviour
    {
        [SerializeField] private CollectableNames _parentItem;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var manabu = collision.GetComponent<Manabu>();
            if (manabu != null)
            {
                Item item = (Item)CollectableManager.GetCollectableByName(_parentItem);
                if (manabu._itemInventory.AddToItemInventory(item))
                {
                    Destroy(gameObject);
                }
            }
        }

    }

}
