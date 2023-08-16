using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _charController;
    private DefaultPlayerInput _input;

    private float _speed = 1.42f; //maybe define this elsewhere?

    private void Start()
    {
        _charController = GetComponent<CharacterController>();
        _input = GetComponent<DefaultPlayerInput>();
    }

    private void Update()
    {
        Vector3 move = transform.right * _input.moveDir.x + transform.forward * _input.moveDir.y;

        _charController.Move(move * Time.deltaTime * _speed);
    }
}