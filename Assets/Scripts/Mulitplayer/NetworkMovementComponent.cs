using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// Related to client prediction.


public class NetworkMovementComponent : NetworkBehaviour
{
    [SerializeField] private CharacterController _cc;
    [SerializeField] private float _speed;
    [SerializeField] private float _turnSpeed;

    [SerializeField] private Transform _camSocket;
    [SerializeField] private GameObject _vcam;

    private Transform _vcamTransform;

    private int _tick = 0;
    private float _tickRate = 1f / 60f;
    private float _tickRateDeltaTime = 0;

    private const int BUFFER_SIZE = 1024; // Max amount input the server stores before the client answers. 
    private InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] _transStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>(); // The latest value that the server has sent an answer for
    public TransformState _previousTransformState;


    //void Awake()
    //{
    //    if (_vcam != null)
    //    {
    //        _vcamTransform = _vcam.transform;
    //    }
    //}

    private void OnEnable()
    {
        ServerTransformState.OnValueChanged += OnServerStateChanged;
    }


    private void OnServerStateChanged(TransformState previousValue, TransformState newValue)
    {
        _previousTransformState = previousValue;
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        _vcamTransform = _vcam.transform;

    }

    //public void InstanceCheck()
    //{
    //    Debug.Log("I am an instance of an object");
    //}


    public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput) // No longer use Time.DeltaTime alone instead use Tick.DeltaTime to normalize the movemnt rate. This local player movemnt
    {
        _tickRateDeltaTime += Time.deltaTime;

        if(_tickRateDeltaTime > _tickRate)
        {
            int bufferIndex = _tick % BUFFER_SIZE;

            if(!IsServer)
            {
                MovePlayerServerRpc(_tick, movementInput, lookInput); //Send server information on how we moved.
                MovePlayer(movementInput); // Then move locally while the server processes movement. This is the client prediciton aspect.
                RotatePlayer(lookInput);
            }
            else // Host does not need to do client prediction.
            {
                MovePlayer(movementInput);
                RotatePlayer(lookInput);

                TransformState state = new TransformState()
                {
                    Tick = _tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                _previousTransformState = ServerTransformState.Value; // Same process for client but we do not need to send a ServerRPC becuase we are the host.
                ServerTransformState.Value = state;
            }

            InputState inputState = new InputState()
            {
                Tick = _tick,
                movementInput = movementInput,
                lookInput = lookInput
            };

            TransformState transformState = new TransformState() 
            {
                Tick = _tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            _inputStates[bufferIndex] = inputState; // This is the same thing the server does but we are keeping a record locally as well. This can be used to compare local changes to what the server is expecting/ packet losses.
            _transStates[bufferIndex] = transformState;

            _tickRateDeltaTime -= _tickRate;
            _tick++;
        }
    }


    public void ProcessSimulatedPlayerMovement()
    {
        _tickRateDeltaTime += Time.deltaTime;
        if(_tickRateDeltaTime > 0) // Time to process this input
        {
            if (ServerTransformState.Value.HasStartedMoving) 
            {
                transform.position = ServerTransformState.Value.Position;
                transform.rotation = ServerTransformState.Value.Rotation;
            }

            _tickRateDeltaTime -= _tickRate;
            _tick++;
        }
    }


    private void RotatePlayer(Vector2 lookInput)
    {
        _vcamTransform.RotateAround(point: _vcamTransform.position, axis: -_vcamTransform.right, angle: lookInput.y * _tickRate);
        transform.RotateAround(point: transform.position, axis: transform.up, angle: lookInput.x * _turnSpeed * _tickRate);
    }


    private void MovePlayer(Vector2 movementInput)
    {
        Vector3 movement = movementInput.x * _vcamTransform.right + movementInput.y * _vcamTransform.forward;

        movement.y = 0;

        if (!_cc.isGrounded)
        {
            movement.y = -9.61f;
        }

        _cc.Move(motion: movement * _speed * _tickRate); // Move at the same time by the same tick rate. 
    }


    [ServerRpc] // TO-DO review what this does for us
    private void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput) // This processes player movement
    {

        //// This is where you would catch errors on missing package information from a tick that was not properly received and account for it. 
        //if (_tick != _previousTransformState.Tick + 1)
        //{
        //    // Server is missing package from the client. 
        //}

        MovePlayer(movementInput);
        RotatePlayer(lookInput);

        TransformState state = new TransformState() 
        {
            Tick = tick,
            Position = transform.position,
            Rotation = transform.rotation,
            HasStartedMoving = true
        };

        _previousTransformState = ServerTransformState.Value; // This is done because server packet can be lost 
        ServerTransformState.Value = state;
    }
}
