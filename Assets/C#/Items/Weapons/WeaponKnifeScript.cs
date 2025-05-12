using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponKnifeScript : MonoBehaviour
{
    public WeaponStats stats;  // This will be a dropdown in the Inspector
    private ItemIdentificationVariables _itemIdentificationVariables;

    [SerializeField] private InputActionReference shootAction;
    private bool _isAttacking;
    
    private void OnEnable()
    {
        // Enable all input actions when the object is enabled
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable all input actions when the object is disabled
        shootAction.action.Disable();
    }

    private void Start()
    {
        _itemIdentificationVariables = GetComponent<ItemIdentificationVariables>();
    }

    private void Update()
    {
        // If is not equipped, can't shoot nor reload
        if (!_itemIdentificationVariables.isEquipped) return;
        
        if (shootAction.action.WasPressedThisFrame() && !_isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        _isAttacking = true;
        yield return new WaitForSeconds((int)stats.timeBetweenEachShot / 10);
        _isAttacking = false;
    }
}
