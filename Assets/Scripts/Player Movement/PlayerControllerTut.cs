using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerTutorial : NetworkBehaviour
{
    //[SerializeField] private float _speed;
    //[SerializeField] private float _turnSpeed;
    [SerializeField] private Vector2 _minMaxRotationX;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private NetworkMovementComponent _playerMovement;
    [SerializeField] private float _interactDistance;
    [SerializeField] private LayerMask _interactionLayer;

    private CharacterController _cc;
    private PlayerControl _playerControl;
    private float _cameraAngle;


    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera cvm = _camTransform.gameObject.GetComponent<CinemachineVirtualCamera>(); // SETS EACH PLAYER TO THE RIGHT CAMERA.

        if (IsOwner) // Keep our own camera.
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

        //_playerMovement.InstanceCheck();
    }


    private void Update()
    {
        Vector2 movementInput = _playerControl.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = _playerControl.Player.Look.ReadValue<Vector2>();


        if (IsClient && IsLocalPlayer)
        {
            _playerMovement.ProcessLocalPlayerMovement(movementInput, lookInput);
        }
        else  // DEFINES THE MOVEMENT OF A LOCAL INSTANCE ONLY. 
        {
            _playerMovement.ProcessSimulatedPlayerMovement();
        }

        if (IsLocalPlayer && _playerControl.Player.Interact.inProgress)
        {
            if (Physics.Raycast(origin: _camTransform.position, direction: _camTransform.forward, out RaycastHit hit, _interactDistance, _interactionLayer))
            {
                if (hit.collider.TryGetComponent<ButtonDoor>(out ButtonDoor buttonDoor))
                {
                    UseButtonServerRpc();
                }
            }
        }
    }

    [ServerRpc]
    private void UseButtonServerRpc()
    {
        if (Physics.Raycast(origin: _camTransform.position, direction: _camTransform.forward, out RaycastHit hit, _interactDistance, _interactionLayer))
        {
            if (hit.collider.TryGetComponent<ButtonDoor>(out ButtonDoor buttonDoor))
            {
                buttonDoor.Activate();
            }
        }
    }
}
