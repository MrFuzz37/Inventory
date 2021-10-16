// handles the player inputs. Brad 11/5/2020
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NetworkInv_Interaction
{
    public class PlayerController : NetworkBehaviour
    {
        [Tooltip("The speed at which the player moves")]
        public float moveSpeed = 5f;
        [Tooltip("The range at which the player can interact with objects")]
        public float interactRange = 2;

        public InventorySlot activeSlot;

        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 movement;
        private int index = 0;
        private Outline[] actionSlots;
        private bool isShopping;

        // Start is called before the first frame update
        void Start()
        {
            // Set up instance references separately for each player when they join
            if (isLocalPlayer)
            {
                ActionBar.instance.player = this;
                GoldPanel.instance.player = this;

                InventoryUI.instance.SetPlayer(this);
                InventoryUI.instance.SetInventoryUI();
                InventoryUI.instance.itemsParent.gameObject.SetActive(false);
                InventoryUI.instance.shopUI.gameObject.SetActive(false);

                actionSlots = ActionBar.instance.gameObject.GetComponentsInChildren<Outline>();
                activeSlot = actionSlots[0].GetComponent<InventorySlot>();
                actionSlots[0].enabled = true;
            }

            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                return;

            // if the player is currently in a shop menu
            if (isShopping)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var hit = new List<RaycastResult>();
                    PointerEventData eventData = new PointerEventData(EventSystem.current);
                    eventData.position = Input.mousePosition;
                    EventSystem.current.RaycastAll(eventData, hit);

                    foreach (var i in hit)
                    {
                        Interactable target = i.gameObject.GetComponent<Interactable>();
                        
                        // and hit a target invoke the event
                        if (target)
                        {
                            target.GetComponent<Interactable>().Use(gameObject);
                            return;
                        }
                    }
                }

                // close shop on escape
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    InventoryUI.instance.shopUI.gameObject.SetActive(!InventoryUI.instance.shopUI.gameObject.activeSelf);
                    InventoryUI.instance.itemsParent.gameObject.SetActive(!InventoryUI.instance.itemsParent.gameObject.activeSelf);
                    isShopping = false;
                }
            }
            else
            {
                // movement
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                if (movement != Vector2.zero)
                {
                    animator.SetFloat("Horizontal", movement.x);
                    animator.SetFloat("Vertical", movement.y);
                }
                animator.SetFloat("Speed", movement.sqrMagnitude);

                // inventory button
                if (Input.GetKeyDown(KeyCode.I))
                {
                    InventoryUI.instance.itemsParent.gameObject.SetActive(!InventoryUI.instance.itemsParent.gameObject.activeSelf);
                }

                // pickup/interact button
                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = SelectObjectInRange();

                    // if hit nothing return
                    if (!target)
                        return;

                    // if we hit a shop, display the shop
                    if (target.GetComponent<Inventory>() && !target.GetComponent<PlayerController>())
                    {
                        DisplayShop();
                        return;
                    }

                    // make sure the object is a pickup
                    if (target.GetComponent<Interactable>().isPickUp)
                        CmdUse(target.gameObject, gameObject);
                }

                // use current item button
                if (Input.GetMouseButtonDown(0))
                {
                    Transform target = SelectObjectInRange();

                    if (!target)
                        return;

                    // make sure the object isn't a pickup 
                    if (!target.GetComponent<Interactable>().isPickUp)
                        CmdUse(target.gameObject, gameObject);
                }

                // selecting the active item in the actionbar
                if (Input.mouseScrollDelta.y > 0)
                {
                    // make sure we aren't on the end of the action bar
                    if (index + 1 >= actionSlots.Length)
                        return;

                    actionSlots[index].enabled = false;
                    index++;
                    actionSlots[index].enabled = true;
                    activeSlot = actionSlots[index].gameObject.GetComponent<InventorySlot>();
                }

                if (Input.mouseScrollDelta.y < 0)
                {
                    if (index - 1 < 0)
                        return;

                    actionSlots[index].enabled = false;
                    index--;
                    actionSlots[index].enabled = true;
                    activeSlot = actionSlots[index].gameObject.GetComponent<InventorySlot>();
                }
            }
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // return a transform of an object within the specified range
        // If none in range return null
        private Transform SelectObjectInRange()
        {
            Vector3 worldPos = GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitData = Physics2D.Raycast(new Vector2(worldPos.x, worldPos.y), Vector2.zero, 0);
            if (hitData.collider != null)
                // make sure we're within 2m
                if (Vector2.Distance(hitData.point, transform.position) < interactRange)
                    return hitData.transform;

            return null;
        }

        // shows the shopkeeper
        private void DisplayShop()
        {
            InventoryUI.instance.shopUI.gameObject.SetActive(true);
            InventoryUI.instance.itemsParent.gameObject.SetActive(!InventoryUI.instance.itemsParent.gameObject.activeSelf);
            isShopping = true;
        }

        // command from a client to run on the server for interacting with interactables
        [Command]
        public void CmdUse(GameObject target, GameObject user)
        {
            RpcUse(target, user);
        }

        // server command to the client for interactables
        [ClientRpc]
        public void RpcUse(GameObject target, GameObject user)
        {
            target.GetComponent<Interactable>().Use(user);
        }       
    }

}
