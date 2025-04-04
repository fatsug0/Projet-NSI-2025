using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponKnifeScript : MonoBehaviour
{
    public WeaponStats stats;  // This will be a dropdown in the Inspector
    /// <summary>
    /// (int)stats.damage to access the damage variable
    /// (int)stats.reloadSpeed to access the reload speed variable
    /// (int)stats.range to access the fire range variable
    /// (int).stats.shootSpread to access the shoot spread varible
    /// </summary>

    [SerializeField] private InputActionReference shootAction;
    private GameObject _shootPoint;
    public bool useBullets;
    public GameObject bulletPrefab;
    
    private void Start()
    {
        _shootPoint = transform.GetChild(0).gameObject;
        if (!useBullets) bulletPrefab = null;
    }

    private void Update()
    {
        if (shootAction.action.WasPressedThisFrame())
        {
            Debug.Log("Shoot");
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!useBullets) // Melee weapon don't use bullets
        {
            
        }
        else // Other weapons uses bullets
        {
            
        }
    }
}
