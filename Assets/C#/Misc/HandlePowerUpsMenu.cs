using System;
using TMPro;
using UnityEngine;

public class HandlePowerUpsMenu : MonoBehaviour
{
    [Header("Power Up Settings")]
    private TMP_Text _powerUp1Button;
    private TMP_Text _powerUp2Button;
    private TMP_Text _powerUp3Button; 
    private TMP_Text _powerUp4Button;

    private void Awake()
    {
        gameObject.SetActive(true);
    }

    private void Start()
    {
        // Assign every TextMeshPro Text to its parent
        _powerUp1Button = transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        _powerUp2Button = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        _powerUp3Button = transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        _powerUp4Button = transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void UpdatePowerUps(int power1, int power2, int power3, int power4)
    {
        // Update the values according to the power levels
        _powerUp1Button.text = $"Upgrade Reload Speed to {power1 + 1} ?";
        _powerUp2Button.text = $"Upgrade Stamina to {power2 + 1} ?";
        _powerUp3Button.text = $"Upgrade Run Speed to {power3 + 1} ?";
        _powerUp4Button.text = $"Upgrade Health to {power4 + 1} ?";
    }
    
    
}
