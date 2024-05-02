//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerController2 : MonoBehaviour
//{
//    [SerializeField] private float _speed;
//    [SerializeField] private float _turnSpeed;
//    [SerializeField] private float _minMaxRotationX;
//    //[SerializeField] private float _maxMaxRotationY;
//    [SerializeField] private float _camTransform;

//    private CharacterController _charController;
//    private PlayerControl _playerControl;
//    private float _cameraAngle;

//    // Start is called before the first frame update
//    void Start()
//    {
//        _charController = GetComponent<CharacterController>();

//        _playerControl = new PlayerControl();
//        _playerControl.Enable();

//        Cursor.lockState = CursorLockMode.Locked;
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
