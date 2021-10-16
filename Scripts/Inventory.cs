// The back end of a players inventory, stores all items players gain or lose with stacking. Brad 15/5/2020
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkInv_Interaction
{
    public class Inventory : NetworkBehaviour
    {
        [Tooltip("The amount of slots on the action bar")]
        public int actionbarCount;

        [Tooltip("The amount of inventory slots and what's in them")]
        public Item[] items;

        [Tooltip("How many of each item a player has in their inventory")]
        public int[] stacks;

        [Tooltip("Player's gold amount")]
        public int gold;

        public UnityEvent onChanged;

        public static Dictionary<string, Item> itemsByName = new Dictionary<string, Item>();

        private void Start()
        {
            stacks = new int[items.Length];
        }

        public int SetItem(Item item, int index)
        {
            // if item isn't null
            if (item != null)
            {
                // if item is found in inventory, increase stack count
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == item)
                    {
                        stacks[i]++;
                        onChanged.Invoke();
                        return i;
                    }
                }
            }
            // otherwise if the item is null
            else
            {
                // and if we have more than one of the item
                // reduce the stack count by 1
                if (stacks[index] > 1)
                {
                    stacks[index]--;
                    onChanged.Invoke();
                    return index;
                }
            }

            // If the item isn't null and doesn't have a stack count
            // add the item 
            items[index] = item;
            stacks[index] = item != null ? 1 : 0;
            onChanged.Invoke();
            return index;
        }

        public void RemoveItem(int index)
        {
            SetItem(null, index);
        }

        // finds an empty or existing slot
        public int FindSlot(Item item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }

        #region Networking Functions
        [Command]
        public void CmdSwapItems(int index1, int index2)
        {
            RpcSwapItems(index1, index2);
        }

        // Swaps items in a players inventory after a drag and drop has occured
        [ClientRpc]
        void RpcSwapItems(int index1, int index2)
        {
            Item temp = items[index1];
            int tempStack = stacks[index1];

            items[index1] = items[index2];
            stacks[index1] = stacks[index2];

            items[index2] = temp;
            stacks[index2] = tempStack;
            onChanged.Invoke();
        }

        // Spawns the dropped item on the server
        [Command]
        public void CmdDrop(int index)
        {
            GameObject prefab = GetComponent<Inventory>().items[index].prefab;
            GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(instance);
            RpcDrop(index);
        }

        // Update the player dropping the item's inventory
        [ClientRpc]
        public void RpcDrop(int index)
        {
            GetComponent<Inventory>().SetItem(null, index);
        }

        [Command]
        public void CmdSetItem(string item, int index, int stack)
        {
            RpcSetItem(item, index, stack);
        }

        // Set an item in a player's inventory when buying from a shop
        [ClientRpc]
        public void RpcSetItem(string item, int index, int stack)
        {
            if (item == null)
            {
                items[index] = null;
                return;
            }

            if (itemsByName.ContainsKey(item))
            {
                items[index] = itemsByName[item];
                stacks[index] = stack;
            }
            else
            {
                items[index] = null;
                stacks[index] = 0;
            }
        }

        [Command]
        public void CmdSetGold(int value)
        {
            RpcSetGold(value);
        }

        // Changes the player's gold value when buying or selling
        [ClientRpc]
        public void RpcSetGold(int value)
        {
            gold = value;
            onChanged.Invoke();
        }
        #endregion
    }
}