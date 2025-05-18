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
    private GameManager _gameManager;
    private PlayerInventory _playerInventory;
    
    [Header("Rounds Settings")]
    [SerializeField] private int numberOfRounds;
    [SerializeField] private int _currentRound = 1;
    private int _currentAlive;
    private bool _roundActive;
    [SerializeField] private float pauseBetweenSpawns;
    private bool _spawnPhaseActive;
    private bool _activeSpawn;
    
    [Header("Round Pause Settings")]
    [SerializeField] private float pauseBetweenRounds = 2.0f;
    private bool _paused;
    private float _currentPauseBetweenRounds;
    
    [Header("Round Win Settings")]
    private GameObject _winScreen;

    [Header("Gun Spawn Settings")] 
    [SerializeField] private GameObject[] weapons;

    private void Start()
    {
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        
        _currentPauseBetweenRounds = pauseBetweenRounds;
        _winScreen = GameObject.FindGameObjectWithTag("WinScreen");
        _winScreen.SetActive(false);

        foreach (var weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[0].SetActive(true);
        
        StartRound();
    }

    private void Update()
    {
        if (_currentRound < numberOfRounds)
        {
            if (_roundActive && EnemiesAllDead()) EndRound();
            if (_paused) PauseBetweenRounds();
            if (_spawnPhaseActive && !_activeSpawn) StartCoroutine(HandleEnemySpawn(_currentRound));
        }
        else
        {
            if (EnemiesAllDead())
            {
                Time.timeScale = 0;
                _winScreen.SetActive(true);
            }
        }

        if (_currentRound == 2)
        {
            weapons[1].SetActive(true);
        }
        if(_currentRound == 4)
        {
            weapons[2].SetActive(true);
        }
    }

    private void StartRound()
    {
        Debug.Log($"Starting Round {_currentRound}");

        _roundActive = true;

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
            
            enemy.layer = usedSpawnPoint.layer;
            enemy.GetComponent<SpriteRenderer>().sortingLayerName = GameObjectLayerToUiLayer(enemy);
            
            yield return new WaitForSeconds(pauseBetweenSpawns);
        }
    }

    private string GameObjectLayerToUiLayer(GameObject obj)
    {
        switch (obj.layer)
        {
            case 6:
                return "LEVEL1";
            
            case 7:
                return "LEVEL2";
            
            case 8:
                return "LEVEL3";
            
            default:
                return "LEVEL1";
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
        
        _gameManager.SpawnUtility();
        UpdateAmmo();
    }
    
    private bool EnemiesAllDead()
    {
        return GameObject.FindGameObjectsWithTag("Zombie").Length == 0;
    }

    private void UpdateAmmo()
    {
        foreach (var slot in _playerInventory.inventory.Values)
        {
            if (slot.transform.childCount > 0)
            {
                slot.transform.GetChild(0).GetComponent<ItemIdentificationVariables>().ResyncAmmo(20);
            }
        }
    }
}