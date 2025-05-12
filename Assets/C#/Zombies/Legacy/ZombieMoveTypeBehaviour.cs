using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieMoveTypeBehaviour : MonoBehaviour
{
    [Header("Zombie Stats")]
    public ZombieStats stats;
    
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed;

    [Header("Attack Settings")]
    [SerializeField] private float timeToAttack;
    private bool _isAttacking;
    private bool _hasAttacked;
    private float _attackTimer;
    
    [Header("Experience Settings")]
    [SerializeField] private GameObject experiencePrefab;

    [Header("Multiplied Zombie Stats")] 
    public float currentHealth;
    public float movementSpeed;
    public int damage;
    
    public int baseHealth;
    public int baseMovementSpeed;
    public int baseDamage;
    
    [Header("Misc")]
    private GameObject _player;
    private RoundManager _roundManager;
    private Rigidbody2D _rb;


    private void Start()
    {
        _attackTimer = timeToAttack;
        _player = GameObject.FindWithTag("Player");
        _roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        _rb = GetComponent<Rigidbody2D>();
        
        baseHealth = (int)stats.health;
        baseMovementSpeed = (int)stats.walkingSpeed;
        baseDamage = (int)stats.damage;
        
        currentHealth = (int)stats.health;
        movementSpeed = (int)stats.walkingSpeed;
        damage = (int)stats.damage;
    }

    private void Update()
    {
        if (!_isAttacking)
        {
            Vector2 moveDirection = (_player.transform.position - transform.position).normalized;

            //  ADD Crowd Avoidance  //
            //                       //
            //  ADD Crowd Avoidance  //

            moveDirection.Normalize();
            _rb.MovePosition(_rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);
        }
        
        if ((_player.transform.position - transform.position).magnitude <= (int)stats.attackRange)
        {
            // Attack loop
            _isAttacking = true;

            if (!_hasAttacked)
            {
                // Attack logic
                _hasAttacked = true;
                _attackTimer = timeToAttack;
                Debug.Log("ATTACK !");

                // _player.GetComponent<PlayerController>().HandleHealth(damage);
            }
            else
            {
                _attackTimer -= Time.deltaTime;

            }
            if (_attackTimer <= 0) _hasAttacked = false;
        }
        else
        {
            _isAttacking = false;
        }
    }

    private void Rotate(GameObject player)
    {
        float targetAngle = Mathf.Atan2(player.transform.position.y, player.transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            currentHealth -= other.GetComponent<BulletInformation>().damage;

            if (currentHealth <= 0)
            {
                // Kill Zombie
                Destroy(gameObject);
                
                for (int i = 0; i < Random.Range(1, (int)stats.amountXpDrop); i++)
                {
                    Instantiate(experiencePrefab, transform.position, Quaternion.identity); ;
                }
            }
        }
    }
}
