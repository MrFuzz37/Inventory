// Handles the buying and selling of items to a shopkeeper through unity events. Brad 3/6/2020
using UnityEngine;

namespace NetworkInv_Interaction
{
    public class ShopManager : MonoBehaviour
    {
        private Item item;

        // event when a player buys an item
        public void OnBuy(GameObject user)
        {
            Inventory inv = user.GetComponent<Inventory>();
            item = GetComponentInParent<InventorySlot>().Item;
            if (item)
                if (inv.gold >= item.cost)
                {
                    // find a slot for the item
                    int index = inv.FindSlot(null);
                    int changedIndex = inv.SetItem(item, index);

                    // add stacks if we have the item already
                    int stacks = inv.stacks[changedIndex];
                    inv.CmdSetItem(item.name, changedIndex, stacks);

                    // adjust player gold
                    int newGold = inv.gold -= item.cost;
                    inv.CmdSetGold(newGold);
                }
        }

        // event when a player sells an item
        public void OnSell(GameObject user)
        {
            Inventory inv = user.GetComponent<Inventory>();
            item = GetComponentInParent<InventorySlot>().Item;
            if (item && item.isSellable)
            {
                // remove the item from our inventory
                int index = inv.FindSlot(item);
                inv.RemoveItem(index);

                // reduce the stack count if we had more than 1
                int stacks = inv.stacks[index];
                if (stacks < 1)
                    inv.CmdSetItem(null, index, stacks);
                else
                    inv.CmdSetItem(item.name, index, stacks);

                // adjust player gold
                int newGold = inv.gold += item.sellValue;
                inv.CmdSetGold(newGold);
            }
        }
    }
}

