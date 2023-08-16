using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private DefaultPlayerInput _input;

    private float _currentX = 0f;
    private float _currentY = 0f;

    public float mouseSensitivity = 50f;

    private float _cameraRotationX = 0f;

    public Transform cameraTransform;
    public Transform playerTransform;

    private void Start()
    {        
        _input = GetComponent<DefaultPlayerInput>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _currentX = _input.lookDir.x * mouseSensitivity * Time.deltaTime;
        _currentY = _input.lookDir.y * mouseSensitivity * Time.deltaTime;

        _cameraRotationX -= _currentY;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(_cameraRotationX, 0f, 0f);
        playerTransform.Rotate(Vector3.up * _currentX);
    }
}