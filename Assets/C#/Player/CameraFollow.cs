using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float swayAmount;
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        Vector3 playerPosition = new Vector3(_player.transform.position.x, _player.transform.position.y, -10);
        transform.position = Vector3.Lerp(transform.position, playerPosition, swayAmount * Time.deltaTime);
    }
}
