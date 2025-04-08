using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class WeaponMachineGunScript : MonoBehaviour
{
    private PickableSpriteId pickableSpriteId;
    public WeaponStats stats;  // This will be a dropdown in the Inspector
    /// <summary>
    /// (int)stats.damage to access the damage variable
    /// (int)stats.reloadSpeed to access the reload speed variable
    /// (int)stats.range to access the fire range variable
    /// (int).stats.shootSpread to access the shoot spread varible
    /// </summary>

    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private GameObject bulletPrefab;
    private Transform _firePoint;
    [SerializeField] private float quaternionCoef;

    private void OnEnable()
    {
        shootAction.action.Enable();
        reloadAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.Disable();
        reloadAction.action.Disable();
    }

    private void Start()
    {
        pickableSpriteId = GetComponent<PickableSpriteId>();
        _firePoint = transform.GetChild(0);
    }
    
    private void Update()
    {
        if (!pickableSpriteId.isEquipped) return;
        
        if (shootAction.action.IsPressed()) // Continuous shot
        {
            Shoot(false);
        }

        if (shootAction.action.WasPressedThisFrame()) // Single shot
        {
            Shoot(true);
        }
    }

    private void Shoot(bool singleShot)
    {
        if (singleShot)
        {
            // Apply no shoot spread
        }
        else
        {
            // Apply shoot spread
            float spreadAmount = (int)stats.shootSpread * quaternionCoef; // Tweak this multiplier to adjust spread sharpness
            float spreadAngle = Random.Range(-spreadAmount, spreadAmount);

            // Apply spread to the rotation
            Quaternion spreadRotation = Quaternion.Euler(0, 0, _firePoint.eulerAngles.z + spreadAngle - quaternionCoef);

            // Instantiate bullet with spread applied
            GameObject bullet = Instantiate(bulletPrefab, _firePoint.position, spreadRotation);
            Destroy(bullet, 2f);

            // Set bullet velocity based on its "right" direction
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = bullet.transform.right * (int)stats.bulletSpeed;
        }
    }
}
