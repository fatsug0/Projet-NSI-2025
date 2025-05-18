using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleGUIStats : MonoBehaviour
{
    private PlayerController _player;
    
    [Header("Health Graphic Settings")]
    private List<GameObject> _heartObjectsList = new List<GameObject>();
    [SerializeField] private Texture2D heartSprite;
    [SerializeField] private Texture2D deadHeartSprite;
    
    [Header("Ammunition Graphic Settings")]
    [SerializeField] private TMP_Text ammoText;
    public GameObject ammoTextHolder;
    

    private void Start()
    {
        // Logic to create and assign the hearts in UI according to the player health
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Create the basic heart
        for (int i = 0; i < 15; i++)
        {
            _heartObjectsList.Add(transform.GetChild(0).transform.GetChild(i).gameObject);
            _heartObjectsList[i].SetActive(false);
        }
        
        // Link them to the amount of player health
        for (int i = 0; i < _player.maxHealth; i++)
        {
            _heartObjectsList[i].gameObject.SetActive(true);
            _heartObjectsList[i].GetComponent<RawImage>().texture = heartSprite;
        }
        
        ammoTextHolder = ammoText.gameObject;
        ammoText.text = "--/--";
        
        ammoTextHolder.SetActive(false);
    }
    

    public void UpdateHealth(int current, int max)
    {
        // Update the health amount according to the player health
        for (int i = 0; i < max; i++)
        {
            _heartObjectsList[i].gameObject.SetActive(false);

        }
        for (int i = 0; i < current; i++)
        {
            _heartObjectsList[i].gameObject.SetActive(true);
        }
    }

    public void UpdateAmmo(int ammoCount, int maxAmmo)
    {
        ammoTextHolder.SetActive(true);
        if (ammoCount == -1 && maxAmmo == -1)
        {
            ammoText.text = "--/--";
        }
        else if (ammoCount == 0 && maxAmmo == 0)
        {
            ammoText.text = "0/0";
        }
        else
        {
            ammoText.text = $"{ammoCount}/{maxAmmo}";
        }
    }
}