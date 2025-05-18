using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    [Header("Dynamic Camera Settings")]
    [SerializeField] private float swayAmount;
    private GameObject _player;
    private GameObject _sky;
    public bool enabled_ = true;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _sky = GameObject.FindGameObjectWithTag("Sky");
    }

    private void LateUpdate()
    {
        if (!enabled_) return;
        
        // Just a simple script so the camera follows the player with a small delay for "realism"
        Vector3 playerPosition = new Vector3(_player.transform.position.x, _player.transform.position.y, -10);
        Vector3 targetPosition = Vector3.Lerp(transform.position, playerPosition, swayAmount * Time.deltaTime);
        transform.position = targetPosition;
        _sky.transform.position = targetPosition;
    }
}
