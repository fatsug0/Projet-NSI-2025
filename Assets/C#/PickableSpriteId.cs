using UnityEngine;

public class PickableSpriteId : MonoBehaviour
{
    public Sprite spriteId;
    public bool automaticFill;
    public bool isEquipped;

    private void Awake()
    {
        if (automaticFill)
        {
            spriteId = GetComponent<SpriteRenderer>().sprite;
        }
    }
}
