using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Other Systems")]
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] private GameObject[] graphicComponents;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        _currentHealth = maxHealth;
        _currentStamina = maxStamina;
        _currentSpeed = moveSpeed;
        
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
        moveInput = moveAction.action.ReadValue<Vector2>();

        HandleStamina();
        HandleRotation();

        if (returnAction.action.WasPressedThisFrame())
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180);
        }
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
        if (moveInput != Vector2.zero)
        {
            Vector2 targetPos = rb.position + moveInput.normalized * _currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
        }
    }

    private void HandleRotation()
    {
        if (moveInput.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg + 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void UpdateGraphics(string layer)
    {
        foreach (var part in graphicComponents)
        {
            if (part.gameObject.activeInHierarchy) part.GetComponent<SpriteRenderer>().sortingLayerName = layer;
            
        }
        
        GetComponent<PlayerInventory>().UpdateInventoryGraphics(layer);
    }
}
