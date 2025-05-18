using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HandleOpenMenu : MonoBehaviour
{
    private GameObject _pauseMenu;
    private GameObject _keybindingMenu;
    private PlayerInput _playerInput;

    private void Start()
    {
        _pauseMenu = transform.GetChild(0).gameObject;
        
        _keybindingMenu = transform.GetChild(1).gameObject;
        _keybindingMenu.SetActive(false);
        
        _playerInput = Resources.Load<GameObject>("Player").GetComponent<PlayerInput>();
        Debug.Log(_playerInput.name);
    }
    
    public void BackToMainMenu(GameObject currentMenu)
    {
        currentMenu.SetActive(false);
        _pauseMenu.SetActive(true);
    }
    
    public void Keybinds()
    {
        _pauseMenu.SetActive(false);
        _keybindingMenu.SetActive(true);
    }
    
    public void OpenGameScene()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
