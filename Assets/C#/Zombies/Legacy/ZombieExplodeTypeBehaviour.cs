using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ZombieExplodeTypeBehaviour : MonoBehaviour
{
    [Header("Zombie Stats")]
    public ZombieStats stats;
    
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed;

    [Header("Attack Settings")]
    [SerializeField] private float timeToExplode;
    private bool _hasAttacked;
    private float _attackTimer;
    
    [Header("Multiplied Zombie Stats")] 
    public float currentHealth;
    public float movementSpeed;
    public int damage;
    
    public int baseHealth;
    public int baseMovementSpeed;
    public int baseDamage;
    
    [Header("Experience Settings")]
    [SerializeField] private GameObject experiencePrefab;
    
    [Header("Misc")]
    [SerializeField] private LayerMask playerLayerMask;
    private GameObject _player;
    private RoundManager _roundManager;

    private void Start()
    {
        currentHealth = (int)stats.health;
        _attackTimer = timeToExplode;
        _player = GameObject.FindWithTag("Player");
        _roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        
        baseHealth = (int)stats.health;
        baseMovementSpeed = (int)stats.walkingSpeed;
        baseDamage = (int)stats.damage;
        
        currentHealth = (int)stats.health;
        movementSpeed = (int)stats.walkingSpeed;
        damage = (int)stats.damage;
    }

    private void Update()
    {
        if (!_hasAttacked) transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, movementSpeed * Time.deltaTime);
        Rotate(_player);
        
        if ((_player.transform.position - transform.position).magnitude <= (int)stats.attackRange)
        {
            _hasAttacked = true;
            
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                // Explode
                if (Physics2D.CircleCast(transform.position, (int)stats.attackRange * 2, Vector2.right,
                        Mathf.Infinity, playerLayerMask))
                {
                    // _player.GetComponent<PlayerController>().HandleHealth(damage);
                }
                
                Destroy(gameObject);
            }
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
        for (int i = 0; i < Random.Range(1, (int)stats.amountXpDrop); i++)
        {
            Instantiate(experiencePrefab, transform.position, Quaternion.identity); ;
        }
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
            }
        }
    }
    

}
