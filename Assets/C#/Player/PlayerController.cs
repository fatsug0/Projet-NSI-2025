using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Keybindings")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference shootAction;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speedTransition;
    private float _currentSpeed;
    
    [Header("Health Settings")]
    public float maxHealth = 5;
    [HideInInspector] public float _currentHealth;
    
    [Header("Stamina Settings")]
    [SerializeField] public float maxStamina;
    [SerializeField] private float staminaTransition;
    [HideInInspector] public float _currentStamina;
    private float _resetTimer = 1f;
    private float _currentResetTimer;

    [Header("Experience Settings")]
    [SerializeField] private int level = 1;
    [SerializeField] private float levelUpCoef;
    [SerializeField] private int _currentExperience;

    [Header("Power-Ups Settings")] 
    [SerializeField] private GameObject powerUpMenu;
    [SerializeField] private HandleGUIStats statsHolder;
    
    private int _reloadLevel = 1;
    private float _baseReloadSpeed;
    [SerializeField] private float reloadLevelCoef;
    
    private int _staminaLevel = 1;
    private float _baseStamina;
    [SerializeField] private float staminaLevelCoef;
    
    private int _runSpeedLevel = 1;
    private float _baseRunSpeed;
    [SerializeField] private float runSpeedLevelCoef;
    
    private int _healthLevel = 1;
    [SerializeField] private int maxHealthLevel = 5;

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        shootAction.action.Disable();
    }

    private void Start() // Initialize all the used systems (health, stamina, inventory, vision, ...)
    {
        _currentHealth = maxHealth;
        HandleHealth(0);
        
        _currentStamina = maxStamina;
        _currentSpeed = moveSpeed;

        _baseStamina = maxStamina;
        _baseRunSpeed = sprintSpeed;
        
        powerUpMenu = GameObject.FindWithTag("PowerUpsMenu");
        // powerUpMenu.SetActive(false);
        
        statsHolder = GameObject.FindWithTag("StatsHolder").GetComponent<HandleGUIStats>();
    }

    private void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        bool isSprinting = sprintAction.action.IsPressed();
        Vector2 rawMoveInput = moveAction.action.ReadValue<Vector2>();
        
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

        transform.position += new Vector3(rawMoveInput.x, rawMoveInput.y, 0) * _currentSpeed * Time.deltaTime;

        if (rawMoveInput.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(rawMoveInput.y, rawMoveInput.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }   
    }

    private void HandleExperience(GameObject xpInstance)
    {
        int currentExpTarget = 10; //Mathf.RoundToInt(100 * Mathf.Log(level) * levelUpCoef)
        _currentExperience += xpInstance.GetComponent<XpBehaviour>().xpValue;

        if (_currentExperience >= currentExpTarget)
        {
            level++;
            _currentExperience = 0;
            
            // Show the menu to upgrade stats
            powerUpMenu.GetComponent<HandlePowerUpsMenu>().UpdatePowerUps(_reloadLevel, _staminaLevel, _runSpeedLevel, _healthLevel);
            powerUpMenu.SetActive(true);
        }

        // In any case, we increment the xp bar
        Destroy(xpInstance);
    }

    public void HandlePowerUps(int powerUpId)
    {
        // Logic side
        switch (powerUpId)
        {
            case 1:
                _reloadLevel++;
                // Add reload when added
                break;
            
            case 2:
                _staminaLevel++;
                maxStamina = _baseStamina * Mathf.Log(_staminaLevel + 1) * staminaLevelCoef;
                break;
            
            case 3:
                _runSpeedLevel++;
                sprintSpeed = _baseRunSpeed * Mathf.Log(_runSpeedLevel + 1) * runSpeedLevelCoef;
                break;
            
            case 4:
                _healthLevel++;
                maxHealth ++;
                break;
        }
        statsHolder.UpdateBars();
        powerUpMenu.SetActive(false);
    }

    public void HandleHealth(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            // Kill player
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Experience"))
        {
            HandleExperience(other.gameObject);
        }
    }
}