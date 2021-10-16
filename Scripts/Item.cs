// Represents any item in the game that a player has access to and can have in their inventory. Brad 14/5/2020
using UnityEngine;

namespace NetworkInv_Interaction
{
    [CreateAssetMenu(fileName = "Item", menuName = "InventoryItem", order = 1)]
    public class Item : ScriptableObject
    {
        [Tooltip("Item icon for the UI")]
        public Sprite icon;

        [Tooltip("How much the item costs to buy")]
        public int cost;
        [Tooltip("How much the item sells for")]
        public int sellValue;
        [Tooltip("Stack limit of the item")]
        public int stackLimit;

        [Tooltip("Is the item stackable?")]
        public bool isStackable;
        [Tooltip("Can the item be sold?")]
        public bool isSellable;

        [Tooltip("Prefab linked to the item")]
        public GameObject prefab;

        private void OnEnable()
        {
            Inventory.itemsByName[name] = this;
        }
    }
}

