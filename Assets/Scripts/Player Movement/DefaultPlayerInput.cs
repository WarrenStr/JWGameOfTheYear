using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html

public class DefaultPlayerInput : MonoBehaviour
{
    public Vector2 moveDir;
    public Vector2 lookDir;

    public bool jumpAction;

    private void OnMove(InputValue value) //value is defined by Unity InputSystem
    {
        moveDir = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        lookDir = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        jumpAction = value.Get<bool>();
    }
}