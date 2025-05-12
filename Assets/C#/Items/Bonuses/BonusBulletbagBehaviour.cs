using UnityEngine;

public class BonusBulletbagBehaviour : MonoBehaviour
{
    [SerializeField] private int bulletValue = 5;
    private HandleGUIStats _handleGUIStats;
    
    private void Start()
    {
        _handleGUIStats = GameObject.FindWithTag("StatsHolder").GetComponent<HandleGUIStats>();
    }
    
    // OnTriggerEnter with the Player, he gaines the desired ammo amount for the current weapon 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
         
            if (playerInventory.inventory[playerInventory.currentSlot].transform.childCount == 0) return;
            
            ItemIdentificationVariables itemIdentificationVariables = playerInventory.inventory[playerInventory.currentSlot].transform.GetChild(0).GetComponent<ItemIdentificationVariables>();
            
            // Get the ammo variable of the current weapon in hand and increases the ammo by the desired amountif (pickableSpriteId.usingNormalGun)
            if (itemIdentificationVariables.usingNormalGun)
            {
                itemIdentificationVariables.weaponGunScript.ammoInReserve += bulletValue;
                _handleGUIStats.UpdateAmmo(itemIdentificationVariables.weaponGunScript.currentNumberOfBullet, itemIdentificationVariables.weaponGunScript.ammoInReserve);
                itemIdentificationVariables.UpdateAmmo();
            }
            else
            {
                itemIdentificationVariables.shotgunScript.ammoInReserve += bulletValue;
                _handleGUIStats.UpdateAmmo(itemIdentificationVariables.shotgunScript.currentNumberOfBullet, itemIdentificationVariables.shotgunScript.ammoInReserve);
                itemIdentificationVariables.UpdateAmmo();
            }
            
            Destroy(gameObject);
        }
    }
}