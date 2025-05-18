using System;
using UnityEngine;

public class BonusMedkitBehaviour : MonoBehaviour
{
    [SerializeField] private int healthValue = 1;
    private HandleGUIStats _handleGUIStats;
    
    private void Start()
    {
        _handleGUIStats = GameObject.FindWithTag("StatsHolder").GetComponent<HandleGUIStats>();
    }

    // OnTriggerEnter with the Player, he gaines the desired health amount
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player._currentHealth == player.maxHealth) return;
            
            // Get the health variable of the player and increases the health by the desired amount
            other.GetComponent<PlayerController>()._currentHealth += healthValue;
            _handleGUIStats.UpdateHealth(player._currentHealth, player.maxHealth);
            Destroy(gameObject);
        }
    }
}
