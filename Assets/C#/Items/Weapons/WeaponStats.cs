using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]  // Allows editing in the Inspector
public class WeaponStats
{
    public enum DamagePerBullet { None = 0, Low = 1, Medium = 3, High = 5 }
    public enum WeaponReloadSpeed { None = 0, Slow = 4, Normal = 2, Fast = 1 }
    public enum WeaponShootSpread { None = 0, Small = 2, Medium = 5, Wide = 10 }
    public enum WeaponMagazineSize { None = 0, ExtraSmall = 3, Small = 7, Medium = 15, Large = 20 }
    public enum TimeBetweenEachShot { None = 0, Small = 3, Medium = 6, Long = 9 } // It's divides by 10 when used for milliseconds (can't use float with enums)


    public DamagePerBullet damagePerBullet;
    public WeaponReloadSpeed weaponReloadSpeed;
    public WeaponShootSpread weaponShootSpread;
    public WeaponMagazineSize weaponMagazineSize;
    public TimeBetweenEachShot timeBetweenEachShot;
}
