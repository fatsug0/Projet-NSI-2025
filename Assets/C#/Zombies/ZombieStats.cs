using UnityEngine;

[System.Serializable]
public class ZombieStats
{
    public enum WalkingSpeed { ExtraSlow = 1, Slow = 2, Medium = 4, Fast = 6, ExtraFast = 8 }
    public enum Damage { Small = 1, Medium = 2, Large = 3} // Will be divided by two when used !
    public enum Health { Small = 2, Medium = 4, Large = 5}
    public enum AttackRange { Small = 2, Medium = 4, Large = 6}
    public enum AmountXpDrop { Small = 1, Medium = 2, Large = 3}
    public enum ZombieDifficulty { Easy = 0, Medium = 1, Hard = 2, VeryHard = 3}
    
    public WalkingSpeed walkingSpeed;
    public Damage damage;
    public Health health;
    public AttackRange attackRange;
    public AmountXpDrop amountXpDrop;
    public ZombieDifficulty zombieDifficulty;
}
