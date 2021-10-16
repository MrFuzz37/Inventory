// Sets the text of a shop item to the correct price. Brad 8/6/2020
using UnityEngine;
using UnityEngine.UI;

namespace NetworkInv_Interaction
{
    public class Pricing : MonoBehaviour
    {
        // when the item is spawned set the text to equal the price of the item
        public void Start()
        {
            if (GetComponentInParent<InventorySlot>().Item != null)
                GetComponent<Text>().text = GetComponentInParent<InventorySlot>().Item.cost.ToString();
        }
    }
}

