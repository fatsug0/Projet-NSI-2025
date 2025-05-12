using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    [SerializeField] private string targetLayer;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().UpdateGraphics(targetLayer);
            other.gameObject.layer = LayerMask.NameToLayer(targetLayer);
        }
        // else
        // {
        //     other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = targetLayer;
        // }
    }
}
