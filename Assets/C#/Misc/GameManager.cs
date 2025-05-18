using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Player Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    
    [Header("Random Utility Spawn Settings")]
    [SerializeField] private GameObject ammoPack;
    [SerializeField] private GameObject healthPack;
    private List<GameObject> _utilitySpawnPoints = new List<GameObject>();
    private void Awake()
    {
        Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        foreach (var spawnPoint in GameObject.FindGameObjectsWithTag("UtilitySpawnPoint"))
        {
            _utilitySpawnPoints.Add(spawnPoint);
        }
    
        Debug.Log(_utilitySpawnPoints.Count);
    }

    public void SpawnUtility()
    {

        var random = Random.Range(0, 2); // Between 0 and 1
        if (random == 0)
        {
            Instantiate(ammoPack, _utilitySpawnPoints[Random.Range(0, _utilitySpawnPoints.Count - 1)].transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(healthPack, _utilitySpawnPoints[Random.Range(0, _utilitySpawnPoints.Count - 1)].transform.position, Quaternion.identity);
        }
    }
}
