using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html

public class DefaultPlayerInput : MonoBehaviour
{
    public bool isMovementPressed;

    public Vector2 moveDir;
    public Vector2 lookDir;
    public bool isRunning;

    private void Update()
    {
        isMovementPressed = moveDir.x != 0 || moveDir.y != 0;
    }

    private void OnMove(InputValue value) //value is defined by Unity InputSystem
    {
        moveDir = value.Get<Vector2>();  
    }

    private void OnLook(InputValue value)
    {
        lookDir = value.Get<Vector2>();
    }

    private void OnRun(InputValue value) //Issue here somewhere
    {
        isRunning = value.Get<bool>() ;
    }
}