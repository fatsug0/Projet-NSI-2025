using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Keybindings")]
    [SerializeField] private InputActionReference pickUpAction;
    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private InputActionReference inventoryAction1; 
    [SerializeField] private InputActionReference inventoryAction2; 
    [SerializeField] private InputActionReference inventoryAction3; 
    [SerializeField] private InputActionReference inventoryAction4; 
    [SerializeField] private InputActionReference inventoryAction5; 
    
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize;
    [HideInInspector] public string currentSlot; 
    public Dictionary<string, GameObject> inventory = new Dictionary<string, GameObject>();
    
    [SerializeField] private Sprite inventoryUpperSprite;
    [SerializeField] private Sprite inventoryLowerSprite;
    
    [Header("Graphical Settings")]
    [SerializeField] private float inUseFloatDifference; 
    private Vector3 _defaultPosition; 
    private Dictionary<string, GameObject> _inventorySpriteRenderers = new Dictionary<string, GameObject>(); 
    private GameObject _primaryState;
    private GameObject _primaryStateHand;
    private GameObject _secondaryState; 
    private GameObject _secondaryStateHand;
    
    [Header("PickUp Settings")]
    [SerializeField] private bool isItemInRange;
    [SerializeField] private GameObject itemInRange;
    
    private HandleGUIStats _handleGUIStats;

    private void OnEnable()
    {
        // Enable all input actions when the object becomes active
        pickUpAction.action.Enable();
        dropAction.action.Enable();
        inventoryAction1.action.Enable();
        inventoryAction2.action.Enable();
        inventoryAction3.action.Enable();
        inventoryAction4.action.Enable();
        inventoryAction5.action.Enable();
    }
    
    private void OnDisable()
    {
        // Disable all input actions when the object is disabled
        pickUpAction.action.Disable();
        dropAction.action.Disable();
        inventoryAction1.action.Disable();
        inventoryAction2.action.Disable();
        inventoryAction3.action.Disable();
        inventoryAction4.action.Disable();
        inventoryAction5.action.Disable();
    }
    
    private void Start()
    {
        // Map inventory slots to child GameObjects (weapon holders)
        inventory["Primary"] = transform.GetChild(0).transform.GetChild(0).gameObject;
        inventory["Secondary"] = transform.GetChild(0).transform.GetChild(1).gameObject;
        inventory["Melee"] = transform.GetChild(0).transform.GetChild(2).gameObject;
        inventory["Utility 1"] = transform.GetChild(0).transform.GetChild(3).gameObject;
        inventory["Utility 2"] = transform.GetChild(0).transform.GetChild(4).gameObject;    
        
        currentSlot = "Primary"; // Default slot at start
        
        // Disable all inventory items at start
        foreach (var item in inventory)
        {
            item.Value.gameObject.SetActive(false);
        }

        // Link UI elements (inventory slot indicators) using tags
        _inventorySpriteRenderers["Primary"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot1");
        _inventorySpriteRenderers["Primary"].transform.GetChild(1).GetComponent<RawImage>().texture = inventoryUpperSprite.texture;
        _inventorySpriteRenderers["Primary"].transform.GetChild(2).GetComponent<RawImage>().texture = inventoryLowerSprite.texture;
        
        _inventorySpriteRenderers["Secondary"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot2");
        _inventorySpriteRenderers["Secondary"].transform.GetChild(1).GetComponent<RawImage>().texture = inventoryUpperSprite.texture;
        _inventorySpriteRenderers["Secondary"].transform.GetChild(2).GetComponent<RawImage>().texture = inventoryLowerSprite.texture;
        
        _inventorySpriteRenderers["Melee"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot3");
        _inventorySpriteRenderers["Melee"].transform.GetChild(1).GetComponent<RawImage>().texture = inventoryUpperSprite.texture;
        _inventorySpriteRenderers["Melee"].transform.GetChild(2).GetComponent<RawImage>().texture = inventoryLowerSprite.texture;
        
        _inventorySpriteRenderers["Utility 1"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot4");
        _inventorySpriteRenderers["Utility 1"].transform.GetChild(1).GetComponent<RawImage>().texture = inventoryUpperSprite.texture;
        _inventorySpriteRenderers["Utility 1"].transform.GetChild(2).GetComponent<RawImage>().texture = inventoryLowerSprite.texture;
        
        _inventorySpriteRenderers["Utility 2"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot5");
        _inventorySpriteRenderers["Utility 2"].transform.GetChild(1).GetComponent<RawImage>().texture = inventoryUpperSprite.texture;
        _inventorySpriteRenderers["Utility 2"].transform.GetChild(2).GetComponent<RawImage>().texture = inventoryLowerSprite.texture;

        // Store default UI position for resetting later
        _defaultPosition = _inventorySpriteRenderers["Primary"].transform.GetChild(1).position;

        // References to visual states (e.g., weapon icons/sprites)
        _primaryState = transform.GetChild(1).transform.GetChild(1).gameObject;
        _primaryStateHand = transform.GetChild(1).transform.GetChild(1).transform.GetChild(2).gameObject;
        _secondaryState = transform.GetChild(1).transform.GetChild(2).gameObject;
        _secondaryStateHand = transform.GetChild(1).transform.GetChild(2).transform.GetChild(2).gameObject;
        
        _handleGUIStats = GameObject.FindWithTag("StatsHolder").GetComponent<HandleGUIStats>();
    }
    
    private void Update()
    {
        HandleInventory(); // Handle input for switching inventory slots

        // Check if the player is near an item and tries to pick it up
        if (isItemInRange && pickUpAction.action.WasPressedThisFrame()) 
        {
            PickUpItem(itemInRange);
        }

        // Drop item from current inventory slot
        if (dropAction.action.WasPressedThisFrame())
        {
            try
            {
                DropItem(inventory[currentSlot].transform.GetChild(0).gameObject);
            }
            catch (UnityException)
            {
                Debug.LogWarning("Can't drop a null slot !");
            }
        }
    }
    
    private void HandleInventory()
    {
        // Check each input and switch slots accordingly
        if (inventoryAction1.action.WasPressedThisFrame() && !currentSlot.Equals("Primary"))
            EquipItem("Primary");
        
        if (inventoryAction2.action.WasPressedThisFrame() && !currentSlot.Equals("Secondary"))
            EquipItem("Secondary");

        if (inventoryAction3.action.WasPressedThisFrame() && !currentSlot.Equals("Melee"))
            EquipItem("Melee");

        if (inventoryAction4.action.WasPressedThisFrame() && !currentSlot.Equals("Utility 1"))
            EquipItem("Utility 1");

        if (inventoryAction5.action.WasPressedThisFrame() && !currentSlot.Equals("Utility 2"))
            EquipItem("Utility 2");
    }
    
    private void EquipItem(string slot)
    {
        // Visually unselect the current slot
        _inventorySpriteRenderers[currentSlot].transform.GetChild(1).transform.localPosition = Vector3.zero;
        _inventorySpriteRenderers[currentSlot].transform.GetChild(2).transform.localPosition = new Vector3(0, -1, 0);
        inventory[currentSlot].gameObject.SetActive(false);
        
        // Mark item as not equipped
        if (inventory[currentSlot].transform.childCount > 0)
        {
            inventory[currentSlot].transform.GetChild(0).GetComponent<ItemIdentificationVariables>().isEquipped = false;
        }
        
        currentSlot = slot; // Change current slot
        
        // Visually highlight the new slot
        _inventorySpriteRenderers[slot].transform.GetChild(1).transform.localPosition = new Vector3(0, 0 - inUseFloatDifference, 0);
        _inventorySpriteRenderers[slot].transform.GetChild(2).transform.localPosition = new Vector3(0, -1 + inUseFloatDifference, 0);
        inventory[slot].gameObject.SetActive(true);

        if (inventory[slot].transform.childCount > 0)
        {
            ItemIdentificationVariables newItem = inventory[slot].transform.GetChild(0).GetComponent<ItemIdentificationVariables>();
            newItem.isEquipped = true;

            UpdateSkinStates();
            
            _handleGUIStats.UpdateAmmo(newItem.currentAmmo, newItem.maxAmmo);
        }
        else
        {
            // Equip empty hand
            _handleGUIStats.ammoTextHolder.SetActive(false);
        }
    }

    private void PickUpItem(GameObject item)
    {
        string targetSlot = LayerMask.LayerToName(item.layer); // Determine which slot to place item in
        Transform transformUISlot = _inventorySpriteRenderers[targetSlot].transform.GetChild(0).transform;

        try
        {
            // If there's already an item, drop it and remove UI icon
            var currentItem = inventory[targetSlot].transform.GetChild(0);

            if (currentItem != null)
            {
                DropItem(currentItem.gameObject);
                Destroy(transformUISlot.GetChild(0).gameObject);
            }

            HandleItem(item, targetSlot); // Attach item to inventory

            // Create UI icon for inventory
            GameObject uiItem = new GameObject($"{item.name} + - UI Item");
            uiItem.transform.SetParent(transformUISlot, false);
            SpriteRenderer itemSprite = uiItem.AddComponent<SpriteRenderer>();
            itemSprite.sprite = item.GetComponent<ItemIdentificationVariables>().spriteId;
            itemSprite.sortingOrder = 1;
            uiItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            
            // Update ammo
            ItemIdentificationVariables newItem = item.GetComponent<ItemIdentificationVariables>();
            _handleGUIStats.UpdateAmmo(newItem.currentAmmo, newItem.maxAmmo);
        }
        catch (UnityException)
        {
            // If no item was in slot, just add the new one
            HandleItem(item, targetSlot); // Attach item to inventory
            
            // Create UI icon for inventory
            GameObject uiItem = new GameObject($"{item.name} + - UI Item");
            uiItem.transform.SetParent(transformUISlot, false);
            SpriteRenderer itemSprite = uiItem.AddComponent<SpriteRenderer>();
            itemSprite.sprite = item.GetComponent<ItemIdentificationVariables>().spriteId;
            itemSprite.sortingOrder = 1;            
            uiItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            
            // Update ammo
            ItemIdentificationVariables newItem = item.GetComponent<ItemIdentificationVariables>();
            _handleGUIStats.UpdateAmmo(newItem.currentAmmo, newItem.maxAmmo);
        }
    }

    private void HandleItem(GameObject item, string targetSlot)
    {
        // Set item as a child of the slot, align it visually
        item.transform.SetParent(inventory[targetSlot].transform);
        item.transform.position = targetSlot == "Primary" ? _primaryStateHand.transform.position : _secondaryStateHand.transform.position;
        item.transform.localRotation = Quaternion.Euler(0, 0, item.GetComponent<ItemIdentificationVariables>().rotDiff);
        // item.GetComponent<SpriteRenderer>().sortingLayerName = inHandSortingLayer; // Ensure it's rendered in front
        EquipItem(targetSlot); // Equip immediately
    }

    private void DropItem(GameObject item)
    {
        // Reset item state when dropped
        ItemIdentificationVariables itemIdentificationVariables = item.GetComponent<ItemIdentificationVariables>();
        item.transform.SetParent(null);
        // item.GetComponent<SpriteRenderer>().sortingLayerName = onGroundSortingLayer;
        itemIdentificationVariables.isEquipped = false;

        // Remove item from UI
        Destroy(_inventorySpriteRenderers[currentSlot].transform.GetChild(0).transform.GetChild(0).gameObject);

        // Skin State
        _primaryState.SetActive(false);
        _secondaryState.SetActive(true);
        
        _handleGUIStats.UpdateAmmo(-1, -1);
    }

    public void UpdateInventoryGraphics(string layer)
    {
    // UPDATE WEAPON LAYER ON HEIGHT CHANGE
        foreach (var obj in inventory.Values)
        {
            try
            {
                obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = layer;
            }
            catch (Exception e)
            {
                break;
            }
        }
    }

    private void UpdateSkinStates()
    {
        if (currentSlot.Equals("Primary"))
        {
            _primaryState.SetActive(true);
            _secondaryState.SetActive(false);
        }
        else
        {
            _primaryState.SetActive(false);
            _secondaryState.SetActive(true);
        }
    }

    // Detect if the player is in range of a pickup item
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PickUpable"))
        {
            isItemInRange = true;
            itemInRange = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PickUpable"))
        {
            isItemInRange = false;
            itemInRange = null;
        }
    }
}
