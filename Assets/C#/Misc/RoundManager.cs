using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] spawnableEnemies;
    private GameObject[] _spawnPoints;
    
    [Header("Rounds Settings")]
    [SerializeField] private int numberOfRounds;
    [SerializeField] private int _currentRound = 1;
    private int _currentAlive;
    private bool _roundActive;
    [SerializeField] private float pauseBetweenSpawns;
    private bool _spawnPhaseActive;
    private bool _activeSpawn;
    
    [Header("Difficulty Settings")]
    public float enemyHealthMultiplier = 1.0f;
    public float enemySpeedMultiplier = 1.0f;
    public float enemyDamageMultiplier = 1.0f;
    public float difficultyRampUpRate = 0.2f; // Increase 20% per round
    
    [Header("Round Pause Settings")]
    [SerializeField] private float pauseBetweenRounds = 2.0f;
    private bool _paused;
    private float _currentPauseBetweenRounds;
    
    private void Awake()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        
        _currentPauseBetweenRounds = pauseBetweenRounds;
    }

    private void Start()
    {
        StartRound();
    }

    private void Update()
    {
        if (_roundActive && EnemiesAllDead())
        {
            EndRound();
        }

        if (_paused)
        {
            PauseBetweenRounds();
        }

        if (_spawnPhaseActive && !_activeSpawn)
        {
            StartCoroutine(HandleEnemySpawn(_currentRound));
        }
    }

    private void StartRound()
    {
        Debug.Log($"Starting Round {_currentRound}");

        _roundActive = true;

        // Scale difficulty
        enemyHealthMultiplier = 1.0f + (_currentRound - 1) * difficultyRampUpRate;
        enemySpeedMultiplier = 1.0f + (_currentRound - 1) * difficultyRampUpRate;
        enemyDamageMultiplier = 1.0f + (_currentRound - 1) * difficultyRampUpRate;

        StartCoroutine(HandleEnemySpawn(_currentRound));
    }
    
    private GameObject GetEnemyToSpawn(int round)
    {
        float rand = Random.value;

        if (round < 2)
        {
            return spawnableEnemies[0];
        }
        else if (round < 3)
        {
            // 70% chance of spawn after round 2
            if (rand < 0.7f) return spawnableEnemies[0];
            return spawnableEnemies[1];
        }
        else if (round < 5)
        {
            // 50% chance of spawn after round 3
            if (rand < 0.5f) return spawnableEnemies[1];
            return spawnableEnemies[2];
        }
        else
        {
            // 30% chance of spawn after round 5
            if (rand < 0.3f) return spawnableEnemies[2];
            return spawnableEnemies[3];
        }
    }

    private IEnumerator HandleEnemySpawn(int round)
    {
        int enemyCount = round * 3;
        
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject usedSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            GameObject enemy = Instantiate(GetEnemyToSpawn(_currentRound), usedSpawnPoint.transform.position,
                Quaternion.identity);

            // Example: scale difficulty on the enemy
            enemy.GetComponent<ZombieIdentificationVariables>().SetZombieDifficulty(enemyHealthMultiplier,
                enemyDamageMultiplier, enemySpeedMultiplier);
            
            yield return new WaitForSeconds(pauseBetweenSpawns);
        }
    }

    private void PauseBetweenRounds()
    {
        _currentPauseBetweenRounds -= Time.deltaTime;

        if (_currentPauseBetweenRounds <= 0)
        {
            _paused = false;
            StartRound();
        }
    }

    private void EndRound()
    {
        _paused = true;
        _currentPauseBetweenRounds = pauseBetweenRounds;
        
        _roundActive = false;
        _currentRound++;
    }
    
    private bool EnemiesAllDead()
    {
        return GameObject.FindGameObjectsWithTag("Zombie").Length == 0;
    }
}