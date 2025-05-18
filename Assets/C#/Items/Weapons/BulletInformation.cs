using System.Security.Cryptography;
using UnityEngine;

public class BulletInformation : MonoBehaviour
{
    // Just a very simple script to hold the critical bullet information
    [HideInInspector] public int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
