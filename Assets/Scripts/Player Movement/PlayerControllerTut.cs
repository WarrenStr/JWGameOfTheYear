using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PCTutorial : NetworkBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private Vector2 _minMaxRotationX;
    [SerializeField] private Transform _camTransform;

    private CharacterController _cc;
    private PlayerControl _playerControl;
    private float _cameraAngle;


    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera cvm = _camTransform.gameObject.GetComponent<CinemachineVirtualCamera>(); // SETS EACH PLAYER TO THE RIGHT CAMERA.

        if (IsOwner) // Keep our own canera
        {
            cvm.Priority = 1;
        }
        else
        {
            cvm.Priority = 0;
        }
    }


    private void Start()
    {
        _cc = GetComponent<CharacterController>();

        _playerControl = new PlayerControl();
        _playerControl.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        Vector2 movementInput = _playerControl.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = _playerControl.Player.Look.ReadValue<Vector2>();


        if (IsServer && IsLocalPlayer)
        {
            Move(movementInput);
            Rotate(lookInput);
            RotateCamera(lookInput);
        }
        else if (IsLocalPlayer) // DEFINES THE MOVEMENT OF A LOCAL INSTANCE ONLY. 
        {
            Rotate(lookInput);
            MoveServerRPC(movementInput, lookInput);
        }
    }


    private void Rotate(Vector2 lookInput)
    {
        transform.RotateAround(point: transform.position, axis: transform.up, angle: lookInput.x * _turnSpeed * Time.deltaTime);
    }


    private void Move(Vector2 movementInput)
    {
        Vector3 movement = movementInput.x * _camTransform.right + movementInput.y * _camTransform.forward;

        movement.y = 0;

        _cc.Move(movement * _speed * Time.deltaTime);
    }


    private void RotateCamera(Vector2 lookInput)
    {
        _cameraAngle = Vector3.SignedAngle(from: transform.forward, to: _camTransform.forward, axis: _camTransform.right);
        float cameraRotationAmount = lookInput.y * _turnSpeed * Time.deltaTime;
        float newCameraAngle = _cameraAngle - cameraRotationAmount;

        if (newCameraAngle <=  _minMaxRotationX.x && newCameraAngle >= -_minMaxRotationX.y) 
        {
            _camTransform.RotateAround(point: _camTransform.position, axis: _camTransform.right, angle: -lookInput.y * _turnSpeed * Time.deltaTime);
        }
    }

    [ServerRpc] //CLIENT CALL EXECUTED ON THE SERVER
    private void MoveServerRPC(Vector2 movementInput, Vector2 lookInput)
    {
        Move(movementInput);
        Rotate(lookInput);
    }
}
