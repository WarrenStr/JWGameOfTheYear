 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _charController;
    private DefaultPlayerInput _input;

    private Animator _animator;

    public bool RunningPressed;

    private  float _speedMutiplyer = 1.42f;
    public float runSpeed = 4f;
    public float walkSpeed = 1.42f;

    public void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _input = GetComponent<DefaultPlayerInput>();

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleAnimation();

        if (_input.isRunning) { _speedMutiplyer = runSpeed; } else { _speedMutiplyer = walkSpeed; }

        Vector3 move = transform.right * _input.moveDir.x + transform.forward * _input.moveDir.y;
        _charController.Move(_speedMutiplyer * Time.deltaTime * move);
    }

    void HandleAnimation()
    {
        bool isWalking = _animator.GetBool("isWalking");
        bool isRunning = _animator.GetBool("isRunning");

        if (_input.isMovementPressed && !isWalking)
        {
            _animator.SetBool("isWalking", true);
        }

        else if (!_input.isMovementPressed && isWalking)
        {
            _animator.SetBool("isWalking", false);
        }

        if (_input.isMovementPressed && _input.isRunning)
        {
            _animator.SetBool("isRunning", true);
        }

        else if (!_input.isMovementPressed || !_input.isRunning)
        {
            _animator.SetBool("isRunning", false);
        }
    }

    private void Start()
    {

    }
}