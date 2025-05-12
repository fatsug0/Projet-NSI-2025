using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieAI : MonoBehaviour
{
    [Header("Stats")]
    public ZombieStats stats;
    private Rigidbody2D rb;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;

    [Header("Wander Settings")]
    public float wanderRadius = 3f;
    public float wanderDelay = 2f;

    [Header("XP")]
    public GameObject experiencePrefab;

    private GameObject _player;
    private float _attackTimer;
    private bool _isAttacking;
    private bool _playerDetected;

    private float _health;
    private float _movementSpeed;
    private float _attackRange;
    private int _damage;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        InitializeStats();
        StartCoroutine(Wander());
    }

    private void Update()
    {
        DetectPlayer();

        if (_playerDetected && !_isAttacking)
            MoveTowardPlayer();

        if (_playerDetected && Vector2.Distance(transform.position, _player.transform.position) <= _attackRange)
        {
            if (!_isAttacking)
                StartCoroutine(Attack());
        }

        AvoidZombies();
    }

    private void FixedUpdate()
    {
        if (_playerDetected && !_isAttacking) {
            Vector2 direction = (_player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * _movementSpeed;
        } else {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void InitializeStats()
    {
        _health = (int)stats.health;
        _movementSpeed = (int)stats.walkingSpeed;
        _attackRange = (int)stats.attackRange;
        _damage = (int)stats.damage / 2; // Using your comment: damage is divided by 2
    }

    private void DetectPlayer()
    {
        float detectionRange = _attackRange * 3;
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
        _playerDetected = hit != null;
    }

    private void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _movementSpeed * Time.deltaTime * 0.5f);
        RotateToward(_player.transform.position);
    }

    IEnumerator Attack()
    {
        _isAttacking = true;
        _attackTimer = attackCooldown;

        // TODO: play animation
        // TODO: _player.GetComponent<PlayerController>().TakeDamage(_damage);
        // _player.GetComponent<PlayerController>().HandleHealth(_damage);

        yield return new WaitForSeconds(_attackTimer);
        _isAttacking = false;
    }

    private void RotateToward(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * 10f);
    }

    private void AvoidZombies()
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, 0.8f, LayerMask.GetMask("Enemy"));
        foreach (var other in others)
        {
            if (other.gameObject != gameObject)
            {
                Vector3 away = transform.position - other.transform.position;
                transform.position += away.normalized * Time.deltaTime * 0.5f;
            }
        }
    }

    IEnumerator Wander()
    {
        while (true)
        {
            if (!_playerDetected)
            {
                Vector2 wanderDir = Random.insideUnitCircle.normalized;
                Vector3 target = transform.position + (Vector3)wanderDir * wanderRadius;
                float elapsed = 0f;

                while (elapsed < wanderDelay && !_playerDetected)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, _movementSpeed * 0.5f * Time.deltaTime);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            _health -= other.GetComponent<BulletInformation>().damage;
            if (_health <= 0) Die();
        }
    }

    private void Die()
    {
        int xpCount = Random.Range(1, (int)stats.amountXpDrop + 1);
        for (int i = 0; i < xpCount; i++)
        {
            Instantiate(experiencePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
