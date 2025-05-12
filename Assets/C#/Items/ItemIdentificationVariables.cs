using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemIdentificationVariables : MonoBehaviour
{
    [Header("Sprite & Graphic Settings")]
    public Sprite spriteId;
    public bool automaticFill;
    public bool isEquipped;
    public float rotDiff;

    [Header("Ammunition References (if necessary)")] 
    [SerializeField] private bool usesAmmo;
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public int maxAmmo;
    [HideInInspector] public WeaponGunScript weaponGunScript;
    [HideInInspector] public WeaponShotgunScript shotgunScript;
    [HideInInspector] public bool usingNormalGun;

    private void Start()
    {
        if (automaticFill)
        {
            spriteId = GetComponent<SpriteRenderer>().sprite;
        }

        if (usesAmmo)
        {
            GetWeapon();
            UpdateAmmo();
        }
    }

    private void GetWeapon()
    {
        if (GetComponent<WeaponGunScript>() == null)
        {
            shotgunScript = GetComponent<WeaponShotgunScript>();
            usingNormalGun = false;
        }
        else if (GetComponent<WeaponShotgunScript>() == null)
        {
            weaponGunScript = GetComponent<WeaponGunScript>();
            usingNormalGun = true;
        }
    }
    
    public void UpdateAmmo()
    {
        if (usingNormalGun)
        {
            currentAmmo = weaponGunScript.currentNumberOfBullet;
            maxAmmo = weaponGunScript.ammoInReserve;
        }
        else
        {
            currentAmmo = shotgunScript.currentNumberOfBullet;
            maxAmmo = shotgunScript.ammoInReserve;
        }
    }
}
