// Updates the UI when the inventory or gold changes. Also sets up the inventory slots depending on the inventory size. Brad 16/5/2020
using UnityEngine;
using UnityEngine.UI;

namespace NetworkInv_Interaction
{
    public class InventoryUI : MonoBehaviour
    {
        [Tooltip("Parent object of the inventory UI")]
        public Transform itemsParent;
        [Tooltip("Parent object of the action bar UI")]
        public Transform itemsParentHotbar;
        [Tooltip("Parent object of the shop UI")]
        public Transform shopUI;
        [Tooltip("Parent object of the gold panel")]
        public Transform goldUI;
        public PlayerController player;
        public Inventory inventory;
        [Tooltip("Prefab of an action bar slot")]
        public InventorySlot hotbarPrefab;
        [Tooltip("Prefab of an inventory slot")]
        public InventorySlot invPrefab;
        private InventorySlot[] slots;

        #region Singleton
        public static InventoryUI instance;

        void Awake()
        {
            instance = this;
        }
        #endregion

        // creates inventory slots equal to the size of the inventory
        private void CreateSlots()
        {
            slots = new InventorySlot[inventory.items.Length];

            // Instantiates a number of UI elements depending on the size of our inventory
            for (int i = 0; i < inventory.items.Length; i++)
            {
                // while we have less than 10 elements store them in the hotbar
                // and add the rest to our inventory space
                Transform panel = i < inventory.actionbarCount ? itemsParentHotbar : itemsParent;
                slots[i] = i < 10 ? Instantiate(hotbarPrefab, panel) : Instantiate(invPrefab, panel);
                slots[i].index = i;
                slots[i].inventory = inventory;
                slots[i].SetItem(inventory.items[i]);
            }
        }

        // gets called when the something on the back end is changed
        // updates the front end to be the same as the changed back end
        public void SetInventoryUI()
        {
            if (slots == null)
                CreateSlots();

            // update the inventory when changed
            for (int i = 0; i < inventory.items.Length; i++)
            {
                slots[i].SetItem(inventory.items[i]);
            }

            goldUI.gameObject.GetComponentInChildren<Text>().text = inventory.gold.ToString();
        }

        // helper function to set the player reference and immediately create the inventory
        public void SetPlayer(PlayerController p)
        {
            player = p;
            SetInventory(p.GetComponent<Inventory>());
        }

        // used to set up a players inventory UI
        void SetInventory(Inventory inv)
        {
            if (inventory != null)
                inventory.onChanged.RemoveListener(SetInventoryUI);

            inventory = inv;
            if (inventory != null)
            {
                inventory.onChanged.AddListener(SetInventoryUI);
                SetInventoryUI();
            }
        }
    }
}

