// Sets up the UI panels for the shop with the items specified in the inspector. Brad 1/6/2020
using UnityEngine;

namespace NetworkInv_Interaction
{
    public class ShopKeeper : MonoBehaviour
    {
        [Tooltip("The items this shop sells")]
        public Item[] items;
        [Tooltip("Prefab of the UI shop slot")]
        public InventorySlot invPrefab;
        [Tooltip("Object that owns the shop")]
        public GameObject shopOwner;
        private InventorySlot[] slots;

        public void Awake()
        {
            slots = new InventorySlot[items.Length];

            // Instantiates a number of UI elements depending on the total items
            for (int i = 0; i < shopOwner.GetComponent<Inventory>().items.Length; i++)
            {
                if (i > items.Length)
                    break;

                Transform panel = transform;
                slots[i] = Instantiate(invPrefab, panel);
                slots[i].index = i;
                slots[i].SetItem(shopOwner.GetComponent<Inventory>().items[i]);
            }
        }

        // allows for updating a shops inventory 
        public void SetInventoryUI()
        {
            for (int i = 0; i < items.Length; i++)
            {
                slots[i].SetItem(items[i]);
            }
        }
    }
}

