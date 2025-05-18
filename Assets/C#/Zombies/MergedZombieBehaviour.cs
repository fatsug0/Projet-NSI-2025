using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class MergedZombieBehaviour : MonoBehaviour
{
    // LAYER HANDLING
    private GameObject _player;

    private List<GameObject> _level1Checkpoints = new List<GameObject>();
    private List<GameObject> _level2Checkpoints = new List<GameObject>();
    private List<GameObject> _level3Checkpoints = new List<GameObject>();

    private enum LayerLevel { Level1 = 6, Level2 = 7, Level3 = 8 }

    private GameObject _currentTarget = null;
    private int _intermediateLayer = -1;

    private bool _doneTransition;
    
    //------------------------------------------------------------------------------//
    //                                                                              //
    //------------------------------------------------------------------------------//
    
    // ZOMBIE BEHVIOUR
    [Header("Stats")]
    public ZombieStats stats;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    
    // [Header("Wander Settings")]
    // public float wanderRadius = 3f;
    // public float wanderDelay = 2f;

    // [Header("XP")]
    // public GameObject experiencePrefab;

    private float _attackTimer;
    private bool _isAttacking;
    private bool _playerDetected;
    
    private float _currentHealth;
    
    [SerializeField] private float wallDetectionRange;
    [SerializeField] private float avoidStrength;
    
    // private Coroutine _wanderCoroutine;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        
        AssignCheckpoints();

        // _wanderCoroutine = StartCoroutine();

        _currentHealth = (int)stats.health;
    }

    private void Update()
    {
        if (_player == null) return;
        
        int playerLayer = _player.layer;
        int currentLayer = gameObject.layer;
        
        AvoidZombies(currentLayer);
        
        if (currentLayer == playerLayer)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= (int)stats.attackRange)
            {
                if (!_isAttacking)
                {
                    StartCoroutine(Attack());
                }
            }
            else
            {
                MoveTowardTarget(_player);
            }
            return;
        }

        // If doing double transitions, step-by-step
        if (_intermediateLayer != -1 && currentLayer != playerLayer)
        {
            PerformLayerChange(_intermediateLayer);
            return;
        }
        
        DecideLayer(playerLayer, currentLayer);
    }
    
    private void MoveTowardTarget(GameObject target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).position, direction, wallDetectionRange, LayerMask.GetMask("Walls"));
        if (hit.collider != null)
        {

            Vector2 hitNormal = hit.normal;
            Vector2 reflected = Vector2.Reflect(direction, hitNormal).normalized;

            // Lerp to smooth transition
            direction = Vector3.Lerp(direction, reflected, Mathf.Clamp01(avoidStrength * Time.deltaTime)).normalized;

            Debug.DrawRay(transform.position, reflected * wallDetectionRange, Color.red);
        }

        transform.position += direction * (int)stats.walkingSpeed * Time.deltaTime * 0.5f;
        RotateToward(transform.position + direction);
    }
    
    private void RotateToward(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle - 90f), Time.deltaTime * 10f);
    }
    
    private IEnumerator Attack()
    {
        _isAttacking = true;
        _attackTimer = attackCooldown;
        
        _player.GetComponent<PlayerController>().HandleHealth((int)stats.damage);

        yield return new WaitForSeconds(_attackTimer);
        _isAttacking = false;
    }

    private void DecideLayer(int playerLayer, int currentLayer)
    {
        switch (playerLayer)
        {
            case (int)LayerLevel.Level1:
                if (currentLayer == (int)LayerLevel.Level2)
                    PerformLayerChange((int)LayerLevel.Level1);
                else if (currentLayer == (int)LayerLevel.Level3)
                    SetIntermediateLayer((int)LayerLevel.Level2);
                break;

            case (int)LayerLevel.Level2:
                if (currentLayer == (int)LayerLevel.Level1)
                    PerformLayerChange((int)LayerLevel.Level2);
                else if (currentLayer == (int)LayerLevel.Level3)
                    PerformLayerChange((int)LayerLevel.Level2);
                break;

            case (int)LayerLevel.Level3:
                if (currentLayer == (int)LayerLevel.Level2)
                    PerformLayerChange((int)LayerLevel.Level3);
                else if (currentLayer == (int)LayerLevel.Level1)
                    SetIntermediateLayer((int)LayerLevel.Level2);
                break;
        }
    }
    
    private void SetIntermediateLayer(int targetLayer)
    {
        _intermediateLayer = targetLayer;
        PerformLayerChange(_intermediateLayer);
    }

    private void PerformLayerChange(int targetLayer)
    {
        // Debug.Log(targetLayer);
        List<GameObject> targetCheckpoints = GetCheckpointsForLayer(targetLayer);
        if (targetCheckpoints.Count == 0)
        {
            // Debug.Log("Checkpoints not found");
            return;
        }

        if (_currentTarget == null)
        {
            _currentTarget = FindClosestCheckpoint(targetCheckpoints);
            // Debug.Log("Checkpoint found: " + _currentTarget.name);
        }

        // Debug.Log("Moving towards checkpoint");
        float step = 2f * Time.deltaTime;
        MoveTowardTarget(_currentTarget);

        if (Vector3.Distance(transform.position, _currentTarget.transform.position) < 0.5f && !_doneTransition)
        {
            // Debug.Log("Closes enough to link checkpoint");
            var cp = _currentTarget.GetComponent<CheckPointInfo>();
            if (cp != null)
            {
                // Debug.Log("Changing current checkpoint");
                _currentTarget = cp.link.gameObject;
                _doneTransition = true;
            }
        }

        // Reached final linked point
        if (_currentTarget != null && Vector3.Distance(transform.position, _currentTarget.transform.position) < 0.25f)
        {
            // Debug.Log("Reached link checkpoint");
            gameObject.layer = targetLayer;
            GetComponent<SpriteRenderer>().sortingLayerName = GetSortingLayer(targetLayer);
            _currentTarget = null;

            // Important: Reset
            _doneTransition = false;

            if (_intermediateLayer != -1)
            {
                // Step to final layer now
                _intermediateLayer = -1;
            }
        }
    }
    
    private GameObject FindClosestCheckpoint(List<GameObject> checkPoints)
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject obj in checkPoints)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }
    
    private List<GameObject> GetCheckpointsForLayer(int layer)
    {
        switch (layer)
        {
            case (int)LayerLevel.Level1: 
                return _level1Checkpoints;
            
            case (int)LayerLevel.Level2: 
                return _level2Checkpoints;
            
            case (int)LayerLevel.Level3: 
                return _level3Checkpoints;
            
            default: 
                return new List<GameObject>();
        }
    }

    private string GetSortingLayer(int layer)
    {
        switch (layer)
        {
            case 6:
                return "LEVEL1";
            case 7:
                return "LEVEL2";
            case 8:
                return "LEVEL3";
            default:
                return "";
        }
    }

    private void AvoidZombies(int currentLayer)
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, 0.8f, currentLayer);
        foreach (var other in others)
        {
            if (other.gameObject != gameObject && other.gameObject.CompareTag("Zombie"))
            {
                Vector3 away = transform.position - other.transform.position;
                transform.position += away.normalized * Time.deltaTime * 0.5f;
            }
        }
    }

    private void AssignCheckpoints()
    {
        foreach (var checkPoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            switch (checkPoint.layer)
            {
                case (int)LayerLevel.Level1:
                    _level1Checkpoints.Add(checkPoint);
                    break;
                case (int)LayerLevel.Level2:
                    _level2Checkpoints.Add(checkPoint);
                    break;
                case (int)LayerLevel.Level3:
                    _level3Checkpoints.Add(checkPoint);
                    break;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            _currentHealth -= other.GetComponent<BulletInformation>().damage;
            Debug.Log("HIT HIT HIT");
            if (_currentHealth <= 0)
            {
                Debug.Log("DIE DIE DIE ");
                Die();
                return;
            }
        }
    }

    private void Die()
    {
        // int xpCount = Random.Range(1, (int)stats.amountXpDrop + 1);
        // for (int i = 0; i < xpCount; i++)
        // {
        //     Instantiate(experiencePrefab, transform.position, Quaternion.identity);
        // }
        _player.GetComponent<PlayerController>().killCount++;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _player.GetComponent<PlayerController>().killCount++;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.rotation * Vector2.up);
        
        if (_currentTarget == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
    }
}
