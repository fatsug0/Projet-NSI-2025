using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WeaponShotgunScript : MonoBehaviour
{
    [Header("Weapon Stats & References")]
    public WeaponStats stats;  // This will be a dropdown in the Inspector
    private ItemIdentificationVariables _itemIdentificationVariables;
    private HandleGUIStats _handleGUIStats;

    [Header("Keybindings")]
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference reloadAction;
    
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    private Transform _firePoint;
    [SerializeField] private float quaternionCoef;

    [Header("Other Weapon Settings")]
    [SerializeField] private bool allowContinuousShots;
    [SerializeField] private int numberOfShellsShot;
    private bool _isShooting;
    public float bulletSpeed;
    public int currentNumberOfBullet;
    private bool _isReloading;
    private float _reloadTimer;
    public int ammoInReserve;
    
    private void OnEnable()
    {
        // Enable all input actions when the object is enabled
        shootAction.action.Enable();
        reloadAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable all input actions when the object is disabled
        shootAction.action.Disable();
        reloadAction.action.Disable();
    }
    
    private void Awake()
    {
        currentNumberOfBullet = (int)stats.weaponMagazineSize;
        ammoInReserve = Mathf.RoundToInt((int)stats.weaponMagazineSize * 1.5f);
    }

    private void Start()
    {
        _itemIdentificationVariables = GetComponent<ItemIdentificationVariables>();
        _firePoint = transform.GetChild(0);
        
        _handleGUIStats = GameObject.FindWithTag("StatsHolder").GetComponent<HandleGUIStats>();
    }
    
    private void Update()
    {
        // If is not equipped, can't shoot nor reload
        if (!_itemIdentificationVariables.isEquipped) return;
        
        // Shoot continuous bullets if has the bullets
        if (shootAction.action.WasPressedThisFrame() && currentNumberOfBullet > 0 && !_isReloading && !_isShooting) 
        {
            StartCoroutine(Shoot());
            _handleGUIStats.UpdateAmmo(currentNumberOfBullet, ammoInReserve);
        }
        
        // Reload the weapon
        if (reloadAction.action.WasPressedThisFrame() && !_isReloading)
        {
            _isReloading = true;
            _reloadTimer = (int)stats.weaponReloadSpeed;
        }

        if (_isReloading)
        {
            ReloadWeapon();
        }
    }

    private IEnumerator Shoot()
    {
        _isShooting = true;

        for (int i = 0; i < numberOfShellsShot; i++)
        {
            // Apply shoot spread
            float spreadAmount =
                (int)stats.weaponShootSpread * quaternionCoef / 10; // Tweak this multiplier to adjust spread sharpness
            float spreadAngle = Random.Range(-spreadAmount, spreadAmount);

            // Apply spread to the rotation
            Quaternion spreadRotation =
                Quaternion.Euler(0, 0, _firePoint.eulerAngles.z + spreadAngle - quaternionCoef);

            // Instantiate bullet with spread applied
            GameObject bullet = Instantiate(bulletPrefab, _firePoint.position, spreadRotation);
            bullet.GetComponent<BulletInformation>().damage = (int)stats.damagePerBullet;
            Destroy(bullet, 2f);

            // Set bullet velocity based on its "right" direction
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = bullet.transform.right * bulletSpeed;
            
            bullet.GetComponent<SpriteRenderer>().sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
            bullet.layer = GameObject.FindGameObjectWithTag("Player").layer;
        }
        
        currentNumberOfBullet--;
        _itemIdentificationVariables.UpdateAmmo();
        
        yield return new WaitForSeconds((int)stats.timeBetweenEachShot / 10);
        _isShooting = false;
    }

    private void ReloadWeapon()
    {
        _isReloading = true;
        
        if (_reloadTimer >= 0)
        {
            _reloadTimer -= Time.deltaTime;
        }
        else
        {
            if (ammoInReserve >= (int)stats.weaponMagazineSize)
            {
                ammoInReserve -= (int)stats.weaponMagazineSize - currentNumberOfBullet;
                currentNumberOfBullet = (int)stats.weaponMagazineSize;
            }
            else
            {
                currentNumberOfBullet = ammoInReserve;
                ammoInReserve = 0;
            }
            _isReloading = false;
        }
        
        _itemIdentificationVariables.UpdateAmmo();
        _handleGUIStats.UpdateAmmo(currentNumberOfBullet, ammoInReserve);
    }
}
