using UnityEngine;

public class PickableSpriteId : MonoBehaviour
{
    public Sprite spriteId;
    public bool automaticFill;
    public bool isEquipped;
    public float rotDiff;

    private void Awake()
    {
        if (automaticFill)
        {
            spriteId = GetComponent<SpriteRenderer>().sprite;
        }
    }
}
