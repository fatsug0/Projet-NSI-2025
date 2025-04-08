using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Keybindings")]
    [SerializeField] private InputActionReference pickUpAction;
    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private InputActionReference inventoryAction1; // Primary weapon
    [SerializeField] private InputActionReference inventoryAction2; // Secondary weapon
    [SerializeField] private InputActionReference inventoryAction3; // Melee weapon
    [SerializeField] private InputActionReference inventoryAction4; // Special utility 1
    [SerializeField] private InputActionReference inventoryAction5; // Special utility 2
    
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize;
    private string _currentSlot;
    private Dictionary<string, GameObject> _inventory = new Dictionary<string, GameObject>();
    
    [Header("Graphical Settings")]
    [SerializeField] private float inUseFloatDifference;
    private Vector3 _defaultPosition;
    private Dictionary<string, GameObject> _inventorySpriteRenderers = new Dictionary<string, GameObject>();
    
    [Header("PickUp Settings")]
    [SerializeField] private bool _isItemInRange;
    [SerializeField] private GameObject _itemInRange;

    private void OnEnable()
    {
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
        // For the List of the inventory :
        // slot [0] or 1 is reserved for the primary weapon
        // slot [1] or 2 is reserved for the secondary weapon
        // slot [2] or 3 is reserved for the melee weapon
        // slot [3] or 4 is reserved for the utility 1
        // slot [4] or 5 is reserved for the utility 2
        
        // Initialize dictionary mapping layers to inventory slots
        _inventory["Primary"] = transform.GetChild(2).transform.GetChild(0).gameObject;
        _inventory["Secondary"] = transform.GetChild(2).transform.GetChild(1).gameObject;
        _inventory["Melee"] = transform.GetChild(2).transform.GetChild(2).gameObject;
        _inventory["Utility 1"] = transform.GetChild(2).transform.GetChild(3).gameObject;
        _inventory["Utility 2"] = transform.GetChild(2).transform.GetChild(4).gameObject;
        
        _currentSlot = "Primary";
        foreach (var item in _inventory)
        {
            item.Value.gameObject.SetActive(false);
        }
        
        _inventorySpriteRenderers["Primary"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot1");
        _inventorySpriteRenderers["Secondary"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot2");
        _inventorySpriteRenderers["Melee"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot3");
        _inventorySpriteRenderers["Utility 1"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot4");
        _inventorySpriteRenderers["Utility 2"] = GameObject.FindGameObjectWithTag("InventorySpriteSlot5");
        
        _defaultPosition = _inventorySpriteRenderers["Primary"].transform.GetChild(1).position;
    }
    
    private void Update()
    {
        HandleInventory();

        if (_isItemInRange && pickUpAction.action.WasPressedThisFrame()) 
        {
            PickUpItem(_itemInRange);
        }

        if (dropAction.action.WasPressedThisFrame())
        {
            try
            {
                DropItem(_inventory[_currentSlot].transform.GetChild(0).gameObject);
            }
            catch (UnityException)
            {
                Debug.Log("Can't drop a null slot !");
            }
        }
    }
    
    private void HandleInventory()
    {
        if (inventoryAction1.action.WasPressedThisFrame() && !_currentSlot.Equals("Primary"))
        {
            Debug.Log("Primary Pressed");
            EquipItem("Primary"); // Primary slot
        }

        if (inventoryAction2.action.WasPressedThisFrame() && !_currentSlot.Equals("Secondary"))
        {
            Debug.Log("Secondary Pressed");
            EquipItem("Secondary"); // Secondary slot
        }

        if (inventoryAction3.action.WasPressedThisFrame() && !_currentSlot.Equals("Melee"))
        {
            Debug.Log("Melee Pressed");
            EquipItem("Melee"); // Melee slot
        }

        if (inventoryAction4.action.WasPressedThisFrame() && !_currentSlot.Equals("Utility 1"))
        {
            Debug.Log("Utility 1 Pressed");
            EquipItem("Utility 1"); // Utility 1 slot
        }

        if (inventoryAction5.action.WasPressedThisFrame() && !_currentSlot.Equals("Utility 2"))
        {
            Debug.Log("Utility 2 Pressed");
            EquipItem("Utility 2"); // Utility 2 slot
        }
    }
    
    private void EquipItem(string slot)
    {
        try
        {
            _inventorySpriteRenderers[_currentSlot].transform.GetChild(1).transform.localPosition = Vector3.zero;
            _inventorySpriteRenderers[_currentSlot].transform.GetChild(2).transform.localPosition = new Vector3(0, -1, 0);
            _inventory[_currentSlot].gameObject.SetActive(false);

            if (_inventory[_currentSlot].transform.childCount > 0)
            {
                _inventory[_currentSlot].transform.GetChild(0).GetComponent<PickableSpriteId>().isEquipped = false;
            }

            _currentSlot = slot;

            _inventorySpriteRenderers[slot].transform.GetChild(1).transform.localPosition = new Vector3(0, 0 - inUseFloatDifference, 0);
            _inventorySpriteRenderers[slot].transform.GetChild(2).transform.localPosition = new Vector3(0, -1 + inUseFloatDifference, 0);
            _inventory[slot].gameObject.SetActive(true);

            if (_inventory[slot].transform.childCount > 0)
            {
                _inventory[slot].transform.GetChild(0).GetComponent<PickableSpriteId>().isEquipped = true;
            }
        }
        catch (UnityException e)
        {
            Debug.LogWarning($"EquipItem failed: {e.Message}");
        }
    }

    
    private void PickUpItem(GameObject item)
    {
        string targetSlot = LayerMask.LayerToName(item.layer);
        Transform transformUISlot = _inventorySpriteRenderers[LayerMask.LayerToName(item.layer)].transform.GetChild(0)
            .transform;

        try
        {
            var currentItem = _inventory[targetSlot].transform.GetChild(0);

            if (currentItem != null)
            {
                DropItem(currentItem.gameObject);
                Destroy(_inventorySpriteRenderers[LayerMask.LayerToName(item.layer)].transform.GetChild(0).GetChild(0)
                    .gameObject);
            }

            HandleItem(item, targetSlot);

            // Create a new UI Image instead of Instantiating a sprite
            GameObject uiItem = new GameObject("InventoryItem");
            uiItem.transform.SetParent(transformUISlot, false); // Attach it to the UI slot
            Image itemImage = uiItem.AddComponent<Image>(); // Add Image component
            itemImage.sprite = item.GetComponent<PickableSpriteId>().spriteId; // Assign sprite
            uiItem.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1);
        }
        catch (UnityException)
        {
            HandleItem(item, targetSlot);

            GameObject uiItem = new GameObject(item.name);
            uiItem.transform.SetParent(transformUISlot, false);
            Image itemImage = uiItem.AddComponent<Image>();
            itemImage.sprite = item.GetComponent<PickableSpriteId>().spriteId;
            uiItem.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1);
        }
    }


    private void HandleItem(GameObject item, string targetSlot)
    {
        // item.SetActive(false);
        item.transform.SetParent(_inventory[targetSlot].transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, item.GetComponent<PickableSpriteId>().rotDiff);
        item.GetComponent<SpriteRenderer>().sortingOrder = 3;
        EquipItem(targetSlot);
    }

    private void DropItem(GameObject item)
    {
        PickableSpriteId pickableSpriteId = item.GetComponent<PickableSpriteId>();
        item.transform.SetParent(null);
        item.transform.localRotation = Quaternion.Euler(0, 0, pickableSpriteId.rotDiff);
        item.GetComponent<SpriteRenderer>().sortingOrder = 1;
        pickableSpriteId.isEquipped = false;
        Destroy(_inventorySpriteRenderers[_currentSlot].transform.GetChild(0).transform.GetChild(0).gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PickUpable"))
        {
            _isItemInRange = true;
            _itemInRange = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PickUpable"))
        {
            _isItemInRange = false;
            _itemInRange = null;
        }
    }

}   
