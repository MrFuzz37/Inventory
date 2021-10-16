// Handles all drag and drop functionality within the UI elements. Brad 21/5/2020
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

namespace NetworkInv_Interaction
{
    public class DragItem : NetworkBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        [Tooltip("Transform of the action bar panel")]
        public RectTransform barPanel;
        [Tooltip("Transform of the inventory panel")]
        public RectTransform invPanel;
        GameObject currentDragged;
        Transform parent;
        public int index;

        // callback when a drag is being performed
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        // callback when a drag starts
        public void OnBeginDrag(PointerEventData eventData)
        {
            // grab a reference to the object being dragged
            currentDragged = gameObject;
            // grab a reference to its parent for later
            parent = currentDragged.GetComponentInParent<InventorySlot>().transform;
            // make it's parent the canvas and set it as the last sibling so it appears above all other elements
            currentDragged.transform.SetParent(currentDragged.GetComponentInParent<InventoryUI>().transform);
            currentDragged.transform.SetAsLastSibling();
        }

        // callback when the drag ends
        public void OnEndDrag(PointerEventData eventData)
        {
            // reset the parent to its original
            currentDragged.transform.SetParent(parent);
            // snap it back to its original position
            currentDragged.transform.localPosition = Vector3.zero;
        }

        // callback when a drop starts
        public void OnDrop(PointerEventData eventData)
        {
            currentDragged.transform.SetParent(parent);

            var hit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hit);

            // see if there's a slot under the mouse
            InventorySlot dropSlot = null;
            foreach (var i in hit)
            {
                if (i.gameObject.transform != currentDragged.transform)
                {
                    InventorySlot test = i.gameObject.GetComponent<InventorySlot>();
                    if (test)
                    {
                        dropSlot = test;
                        break;
                    }
                }
            }

            if (dropSlot)
            {
                // get dropslot's index and the source index of our draggable, and do the swap
                InventorySlot oldSlot = currentDragged.transform.parent.GetComponent<InventorySlot>();
                Inventory inv = oldSlot.inventory;

                inv.CmdSwapItems(oldSlot.index, dropSlot.index);

                // snap the dragged item back into its slot
                currentDragged.transform.localPosition = Vector3.zero;
            }
            else
            {
                // drop the item or return to original position if we're still in the container
                //if mouse isn't over the inventory or action bar snap the currently held item back to it's original position
                if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition) && !RectTransformUtility.RectangleContainsScreenPoint(barPanel, Input.mousePosition))
                {
                    InventorySlot itemToDrop = currentDragged.transform.parent.GetComponent<InventorySlot>();

                    if (itemToDrop != null)
                    {
                        itemToDrop.inventory.CmdDrop(itemToDrop.index);
                    }
                    return;
                }
                currentDragged.transform.localPosition = Vector3.zero;
            }
        }
    }
}

