// Updates the slot information with the item it holds. Brad 15/5/2020
using UnityEngine;
using UnityEngine.UI;

namespace NetworkInv_Interaction
{
    public class InventorySlot : MonoBehaviour
    {
        [Tooltip("Icon of the item in this slot")]
        public Image icon;
        public Item item;
        public Inventory inventory;
        public int index;

        public Item Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        // adds an item to this slot
        public void AddItem(Item newItem)
        {
            item = newItem;

            icon.sprite = item.icon;
            UpdateStackUI();
            icon.color = new Color(1, 1, 1, 1);
        }

        // clears the current slot item
        public void ClearSlot()
        {
            item = null;

            UpdateStackUI();
            icon.color = new Color(1, 1, 1, 0);
        }

        // sets the current slot based off the item passed in
        public void SetItem(Item item)
        {
            if (item != null)
            {
                AddItem(item);
            }
            else
            {
                ClearSlot();
            }
        }

        // updates the current slots text to reflect how many of the current slot item you have
        public void UpdateStackUI()
        {
            if (inventory != null)
            {
                // stack array should always be the same length as the inventory 
                // if not return
                if (inventory.stacks.Length != inventory.items.Length)
                    return;

                // if we have more than one, display how many of the item we have
                if (inventory.stacks[index] > 1)
                {
                    GetComponentInChildren<Text>().text = inventory.stacks[index].ToString();
                }
                else
                    GetComponentInChildren<Text>().text = null;
            }
        }
    }
}

