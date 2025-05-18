using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBehaviour : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        transform.position = _camera.ScreenToWorldPoint(mouseScreenPosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
    }
}
