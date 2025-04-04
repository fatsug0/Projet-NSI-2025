using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleGUIStats : MonoBehaviour
{
    private PlayerController _player;
    
    [SerializeField] private List<GameObject> _heartObjectsList = new List<GameObject>();
    [SerializeField] private Sprite _heartSprite;
    [SerializeField] private Sprite _deadHeartSprite;
    

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        for (int i = 0; i < 15; i++)
        {
            _heartObjectsList.Add(transform.GetChild(0).transform.GetChild(i).gameObject);
            _heartObjectsList[i].SetActive(false);
        }
        
        for (int i = 0; i < _player.maxHealth; i++)
        {
            _heartObjectsList[i].gameObject.SetActive(true);
            _heartObjectsList[i].GetComponent<SpriteRenderer>().sprite = _heartSprite;
        }
    }
    

    public void UpdateBars()
    {
        for (int i = 0; i < _player.maxHealth - 1; i++)
        {
            _heartObjectsList[i].gameObject.SetActive(true);
        }
    }
}