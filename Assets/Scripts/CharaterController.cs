using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _walkSpeed = 2f;
    private float _moveSpeed;

    [Header("Rotation Settings")]
    [SerializeField, Tooltip("How fast the character rotates when flipping")] private float _rotationSpeed = 10f;
    public bool IsFacingRight {get; private set;}


    [Header("Jump")]
    [SerializeField, Tooltip("How big the ground collider check sphere is")] private float _groundCheckRadius = 0.2f;
    [SerializeField, Tooltip("What layers are considered ground")] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _jumpForce = 5f;
    private bool _isGrounded = false;
    private Collider[] _groundColliders;

    [Header("Camera")]
    [SerializeField] private GameObject _camera;
    private CameraFollowObject _cameraFollowObject;


    private Rigidbody _playerRigidbody;
    private Animator _animator;
    private Quaternion _targetRotation;
    private bool _jumpPressed;
    private float _fallSpeedYDampingChangeThreshold;


    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        IsFacingRight = true;
        _targetRotation = transform.rotation;

        _cameraFollowObject = _camera.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraController.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
        _jumpPressed = Input.GetButtonDown("Jump");

        //if we are falling past a certain speed thresshold
        if(_playerRigidbody.velocity.y <_fallSpeedYDampingChangeThreshold && !CameraController.instance.IsLerpingYDamping && !CameraController.instance.LerpedFromPlayerFalling){
            CameraController.instance.LerpYDamping(true);
        }

        //if we are standing still or moving up
        if(_playerRigidbody.velocity.y >= 0f && !CameraController.instance.IsLerpingYDamping && CameraController.instance.LerpedFromPlayerFalling){
            CameraController.instance.LerpedFromPlayerFalling = false;
            CameraController.instance.LerpYDamping(false);
        }
    }

    void FixedUpdate()
    {
        // Check if the player is on the ground
        bool _wasGrounded = _isGrounded;
        _isGrounded = Physics.OverlapSphere(_groundCheck.position, _groundCheckRadius, _groundLayer).Length > 0;
        _animator.SetBool("Grounded", _isGrounded);

        if (!_wasGrounded && _isGrounded && _playerRigidbody.velocity.y < 0.1f)
        {
            _animator.SetTrigger("Land");
        }

        //Jumping
        if (_jumpPressed && _isGrounded)
        {
            _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
            _animator.SetBool("Grounded", false);
            _animator.ResetTrigger("Land");
        }

        float move = Input.GetAxis("Horizontal");
        _animator.SetFloat("RunningSpeed", Mathf.Abs(move));
        float walk = Input.GetAxis("Fire1");
        _animator.SetFloat("WalkingSpeed", Mathf.Abs(walk));

        if (walk > 0 && _isGrounded)
        {
            _moveSpeed = _walkSpeed;
        }
        else
        {
            _moveSpeed = _runSpeed;
        }
        _playerRigidbody.velocity = new Vector3(move * _moveSpeed, _playerRigidbody.velocity.y, 0f);
        _animator.SetFloat("VerticalSpeed", GetComponent<Rigidbody>().velocity.y);

        if (move > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
            _targetRotation = Quaternion.Euler(0f, 90f, 0f);

            _cameraFollowObject.CallTurn();
        }
        else if (move < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            _targetRotation = Quaternion.Euler(0f, -90f, 0f);

            _cameraFollowObject.CallTurn();
        }
    }
}