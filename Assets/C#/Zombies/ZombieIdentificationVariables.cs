using System;
using UnityEngine;

public class ZombieIdentificationVariables : MonoBehaviour
{
    public float baseHealth;
    public float baseSpeed;

    private ZombieExplodeTypeBehaviour _zombieExplodeTypeBehaviour;
    private ZombieMoveTypeBehaviour _zombieMoveTypeBehaviour;
    private bool _normalZombie;

    private void Awake()
    {
        if (GetComponent<ZombieExplodeTypeBehaviour>() == null)
        {
            _zombieMoveTypeBehaviour = GetComponent<ZombieMoveTypeBehaviour>();
            _normalZombie = true;
        }
        else
        {
            _zombieExplodeTypeBehaviour = GetComponent<ZombieExplodeTypeBehaviour>();
            _normalZombie = false;
        }

        if (_normalZombie)
        {
            baseHealth = _zombieMoveTypeBehaviour.baseHealth;
            baseSpeed = _zombieMoveTypeBehaviour.baseMovementSpeed;
        }
        else
        {
            baseHealth = _zombieExplodeTypeBehaviour.baseHealth;
            baseSpeed = _zombieExplodeTypeBehaviour.baseMovementSpeed;
        }
    }

    public void SetZombieDifficulty(float healthMultiplier, float damageMultiplier, float speedMultiplier)
    {
        if (_normalZombie)
        {
            _zombieMoveTypeBehaviour.currentHealth = _zombieMoveTypeBehaviour.baseHealth * healthMultiplier;
            _zombieMoveTypeBehaviour.movementSpeed = _zombieMoveTypeBehaviour.baseMovementSpeed * speedMultiplier;
            _zombieMoveTypeBehaviour.damage = Mathf.RoundToInt(_zombieMoveTypeBehaviour.baseDamage * damageMultiplier);
        }
        else
        {
            _zombieExplodeTypeBehaviour.currentHealth = _zombieExplodeTypeBehaviour.baseHealth * healthMultiplier;
            _zombieExplodeTypeBehaviour.movementSpeed = _zombieExplodeTypeBehaviour.baseMovementSpeed * speedMultiplier;
            _zombieExplodeTypeBehaviour.damage = Mathf.RoundToInt(_zombieExplodeTypeBehaviour.baseDamage * damageMultiplier);
        }
    }
}