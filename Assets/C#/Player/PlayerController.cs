using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Keybindings")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference returnAction;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speedTransition;
    private float _currentSpeed;

    [Header("Stamina Settings")]
    [SerializeField] public float maxStamina;
    [SerializeField] private float staminaTransition;
    public float _currentStamina;
    private float _resetTimer = 1f;
    private float _currentResetTimer;

    [Header("Health Settings")]
    public int maxHealth = 5;
    public int _currentHealth;

    [Header("Footstep Settings")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float minTimeBetweenFootsteps = 0.3f;
    [SerializeField] private float maxTimeBetweenFootsteps = 0.6f;
    [SerializeField] private float deltaVolume = 0.5f;
    [SerializeField] private float deltaPitch = 1f;
    [SerializeField] private bool allowSfx; // My system is working but the sounds are terrible, so i disable it without deleting it
    private AudioSource _walkingSource;
    private bool _isWalking = false;
    private float _timeSinceLastFootstep;
    
    [Header("Other Systems")]
    [SerializeField] private GameObject[] graphicComponents;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveInput;
    private HandleGUIStats _handleGUIStats;
    private GameObject _deathScreen;
    public int killCount;
    private Camera _camera;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _walkingSource = GetComponent<AudioSource>();
        _handleGUIStats = GameObject.FindWithTag("StatsHolder").GetComponent<HandleGUIStats>();
        _deathScreen = GameObject.FindWithTag("DeathScreen");
        _deathScreen.SetActive(false);

        _currentHealth = maxHealth;
        _currentStamina = maxStamina;
        _currentSpeed = moveSpeed;
        
        _camera = Camera.main;
        
        UpdateGraphics("LEVEL1");
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        shootAction.action.Enable();
        returnAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        shootAction.action.Disable();
        returnAction.action.Disable();
    }

    private void Update()
    {
        _moveInput = moveAction.action.ReadValue<Vector2>();

        _isWalking = _moveInput == Vector2.zero ? false : true;
        
        HandleStamina();
        HandleRotation();
        
        if (_isWalking && allowSfx) HandleFootstepSfx();
    }

    private void FixedUpdate()
    {
        HandleMovementPhysics();
    }

    private void HandleStamina()
    {
        bool isSprinting = sprintAction.action.IsPressed();

        if (isSprinting && _currentStamina > 0)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, sprintSpeed, speedTransition * Time.deltaTime);
            _currentStamina -= staminaTransition * Time.deltaTime;
            _currentResetTimer = _resetTimer;
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, moveSpeed, speedTransition * Time.deltaTime);

            if (_currentResetTimer > 0)
            {
                _currentResetTimer -= Time.deltaTime;
            }
            else if (_currentStamina < maxStamina)
            {
                _currentStamina += staminaTransition / 2 * Time.deltaTime;
            }
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
    }

    private void HandleMovementPhysics()
    {
        if (_moveInput != Vector2.zero)
        {
            Vector2 targetPos = _rigidbody2D.position + _moveInput.normalized * _currentSpeed * Time.fixedDeltaTime;
            _rigidbody2D.MovePosition(targetPos);
        }
    }

    private void HandleRotation()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }


    public void HandleHealth(int damage)
    {
        _currentHealth -= damage;
        Debug.Log("Player Damaged !: " + damage);
        if (_currentHealth <= 0)
        {
            // Kill Player and everything else ...
            GameObject UIholder = GameObject.FindWithTag("UI");
            for (int i = 0; i < UIholder.transform.childCount; i++)
            {
                UIholder.transform.GetChild(i).gameObject.SetActive(false);
            }
            _deathScreen.SetActive(true);
            _deathScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = $"You killed {killCount} zombies !";
            
            Time.timeScale = 0f;
            Camera.main.GetComponent<CameraFollow>().enabled_ = false;
            Destroy(gameObject);
            return;
        }
        _handleGUIStats.UpdateHealth(_currentHealth, maxHealth);
    }

    public void UpdateGraphics(string layer)
    {
        foreach (var part in graphicComponents)
        {
            if (part.gameObject.activeInHierarchy) part.GetComponent<SpriteRenderer>().sortingLayerName = layer;
            
        }
        
        GetComponent<PlayerInventory>().UpdateInventoryGraphics(layer);
    }

    private void HandleFootstepSfx()
    {
        if (Time.time - _timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
        {
            AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];

            _walkingSource.volume = Random.Range(deltaVolume, 1f);
            _walkingSource.pitch = Random.Range(-deltaPitch, deltaPitch);
            _walkingSource.PlayOneShot(footstepSound);

            _timeSinceLastFootstep = Time.time;
        }
    }
}
