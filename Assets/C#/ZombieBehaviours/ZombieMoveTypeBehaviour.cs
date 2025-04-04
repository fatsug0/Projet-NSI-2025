using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieMoveTypeBehaviour : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(1, 5)] [SerializeField] private int moveSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Attack Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float timeToAttack;
    private bool _isAttacking;
    private bool _hasAttacked;
    private float _attackTimer;
    
    [Header("Health Settings")]
    [SerializeField] private float health;
    private float _currentHealth;
    
    [Header("Experience Settings")]
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private int amountOfXpDrop;

    [Header("Misc")]
    private GameObject _player;

    private void Start()
    {
        _currentHealth = health;
        _attackTimer = timeToAttack;
        _player = GameObject.FindWithTag("Player");
        
        Destroy(gameObject, 2);
    }

    private void Update()
    {
        if (!_isAttacking) transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, moveSpeed * Time.deltaTime);
        Rotate(_player);
        
        if ((_player.transform.position - transform.position).magnitude <= attackRange)
        {
            // Attack loop
            _isAttacking = true;

            if (!_hasAttacked)
            {
                // Attack logic
                _hasAttacked = true;
                _attackTimer = timeToAttack;
                Debug.Log("ATTACK !");
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
    
    private void OnDestroy()
    {
        for (int i = 0; i < Random.Range(1, amountOfXpDrop); i++)
        {
            Instantiate(experiencePrefab, transform.position, Quaternion.identity); ;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
