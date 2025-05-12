using UnityEngine;

public class CheckPointPlacement : MonoBehaviour
{
    [SerializeField] private GameObject stairTarget;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private float xOffset = 0.6f;
    public GameObject link;

    private int _currentLayer;

    private void Start()
    {
        // int stairLayer = stairTarget.layer;
        //
        // if (gameObject.layer == stairLayer) // Checkpoint on same level (so under)
        // {
        //     transform.position = new Vector3(stairTarget.transform.position.x + xOffset, stairTarget.transform.position.y + yOffset * 2.5f, stairTarget.transform.position.z);
        // }
        // else // Checkpoint on a different level (so over)
        // {
        //     transform.position = new Vector3(stairTarget.transform.position.x + xOffset, stairTarget.transform.position.y - yOffset, stairTarget.transform.position.z);
        // }
        //
        // GameObject[] links = GameObject.FindGameObjectsWithTag("Checkpoint");
        // Debug.Log(links.Length);
        // GameObject closest = null;
        // float shortestDistance = Mathf.Infinity;
        //
        // foreach (GameObject obj in links)
        // {
        //     if (obj == gameObject) continue; 
        //     
        //     float distance = Vector3.Distance(transform.position, obj.transform.position); 
        //     if (distance < shortestDistance)
        //     {
        //         shortestDistance = distance;
        //         closest = obj;
        //     }
        // }
        //
        // link = closest;
    }
}
