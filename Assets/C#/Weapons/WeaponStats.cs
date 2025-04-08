using UnityEngine;

[System.Serializable]  // Allows editing in the Inspector
public class WeaponStats
{
    public enum DamageLevel { Low = 10, Medium = 20, High = 30 }
    public enum ReloadSpeed { Slow = 3, Normal = 2, Fast = 1 }
    public enum Range { Short = 5, Medium = 10, Long = 15 }
    public enum ShootSpread { None = 0, Small = 5, Medium = 10, Long = 15 }
    public enum BulletSpeed { Slow = 20, Normal = 30, Fast = 40 }

    public DamageLevel damage;
    public ReloadSpeed reloadSpeed;
    public Range range;
    public ShootSpread shootSpread;
    public BulletSpeed bulletSpeed;
}
