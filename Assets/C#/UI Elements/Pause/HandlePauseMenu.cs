using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;

public class HandlePauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseMenuAction;
    
    private GameObject _pauseMenu;
    private GameObject _keybindingMenu;
    
    private PlayerInput _playerInput;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider audioSlider;
    
    
    private bool _menuActive;

    private void OnEnable()
    {
        pauseMenuAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseMenuAction.action.Disable();
    }

    private void Start()
    {
        _pauseMenu = transform.GetChild(0).gameObject;
        _pauseMenu.SetActive(false);
        
        _keybindingMenu = transform.GetChild(1).gameObject;
        _keybindingMenu.SetActive(false);
        
        _playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (pauseMenuAction.action.WasPressedThisFrame())
        {
            if (_menuActive)
            {
                _pauseMenu.SetActive(false);
                Time.timeScale = 1;
                _playerInput.ActivateInput();
                _menuActive = false;
            }
            else
            {
                _pauseMenu.SetActive(true);
                Time.timeScale = 0;
                _playerInput.DeactivateInput();
                _menuActive = true;
            }
        }
    }

    public void BackToMainMenu(GameObject currentMenu)
    {
        currentMenu.SetActive(false);
        _pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        _playerInput.ActivateInput();
        _menuActive = false;
    }

    public void Keybinds()
    {
        _pauseMenu.SetActive(false);
        _keybindingMenu.SetActive(true);
    }

    public void SetMasterVolume()
    {
        mixer.SetFloat("MasterVolume", audioSlider.value);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
