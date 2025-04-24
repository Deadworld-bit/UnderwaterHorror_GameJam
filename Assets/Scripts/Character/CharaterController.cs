using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _crouchSpeed = 2f;
    private float _moveSpeed;

    [Header("Rotation Settings")]
    [SerializeField, Tooltip("How fast the character rotates when flipping")] private float _rotationSpeed = 10f;
    public bool IsFacingRight { get; private set; }

    [Header("Jump")]
    [SerializeField, Tooltip("How big the ground collider check sphere is")] private float _groundCheckRadius = 0.2f;
    [SerializeField, Tooltip("What layers are considered ground")] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _jumpForce = 5f;
    private bool _isGrounded = false;
    private Collider[] _groundColliders;

    [Header("Pushing")]
    [SerializeField] private float _pushSpeed = 2f;
    private PushObject _pushController;

    [Header("Camera")]
    [SerializeField] private GameObject _camera;
    private CameraFollowObject _cameraFollowObject;

    private Rigidbody _playerRigidbody;
    private Animator _animator;
    private Quaternion _targetRotation;
    private bool _jumpPressed;
    private bool _isHiding;
    private float _horizontalInput;
    private float _fire2Input;
    private float _fire1Input;
    private float _fallSpeedYDampingChangeThreshold;
    private bool _wasPushing = false;

    void Start()
    {
        InitializeComponents();
    }

    void Update()
    {
        HandleRotation();
        CheckGrounded();
        HandleInput();
        HandleJumping();
        HandleMovement();
        HandleCameraDamping();
        HandleFlipping();
    }

    private void InitializeComponents()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _pushController = GetComponent<PushObject>();
        IsFacingRight = true;
        _targetRotation = transform.rotation;

        _cameraFollowObject = _camera.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraController.instance._fallSpeedYDampingChangeThreshold;
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
    }

    private void HandleInput()
    {
        _jumpPressed = Input.GetButtonDown("Jump");
        _horizontalInput = Input.GetAxis("Horizontal");
        _fire1Input = Input.GetAxis("Fire1");
        _fire2Input = Input.GetAxis("Fire2");

        _animator.SetFloat("RunningSpeed", Mathf.Abs(_horizontalInput));
        _animator.SetFloat("CrouchSpeed", Mathf.Abs(_fire1Input));
        _animator.SetFloat("WalkingSpeed", Mathf.Abs(_fire2Input));


        if (Input.GetKeyDown("c"))
        {
            _isHiding = !_isHiding;
            _animator.SetBool("Hide", _isHiding);
        }

        if (_pushController.IsPushing != _wasPushing)
        {
            Debug.Log("Pushing state changed: " + _pushController.IsPushing);
            _animator.SetBool("Pushing", _pushController.IsPushing);
            _wasPushing = _pushController.IsPushing;
        }

    }

    private void HandleCameraDamping()
    {
        if (_playerRigidbody.velocity.y < _fallSpeedYDampingChangeThreshold &&
            !CameraController.instance.IsLerpingYDamping &&
            !CameraController.instance.LerpedFromPlayerFalling)
        {
            CameraController.instance.LerpYDamping(true);
        }

        if (_playerRigidbody.velocity.y >= 0f &&
            !CameraController.instance.IsLerpingYDamping &&
            CameraController.instance.LerpedFromPlayerFalling)
        {
            CameraController.instance.LerpedFromPlayerFalling = false;
            CameraController.instance.LerpYDamping(false);
        }
    }

    private void CheckGrounded()
    {
        bool _wasGrounded = _isGrounded;
        _isGrounded = Physics.OverlapSphere(_groundCheck.position, _groundCheckRadius, _groundLayer).Length > 0;
        _animator.SetBool("Grounded", _isGrounded);
        Debug.Log("Grounded: " + _isGrounded);

        if (!_wasGrounded && _isGrounded && _playerRigidbody.velocity.y < 0.1f)
        {
            _animator.SetTrigger("Land");
        }
    }

    public bool CanCastSpell()
    {
        return _isGrounded && Mathf.Approximately(_horizontalInput, 0f);
    }

    private void HandleJumping()
    {
        if (_jumpPressed && _isGrounded)
        {
            _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
            _animator.SetBool("Grounded", false);
            _animator.ResetTrigger("Land");
        }
    }

    private void HandleMovement()
    {
        float move = _horizontalInput;
        float walk = _fire2Input;
        float crouch = _fire1Input;

        if (_pushController.IsPushing && _isGrounded)
        {
            _moveSpeed = _pushSpeed;
        }
        else if (crouch > 0 && _isGrounded)
        {
            _moveSpeed = _crouchSpeed;
        }
        else if (walk > 0 && _isGrounded)
        {
            _moveSpeed = _walkSpeed;
        }
        else
        {
            _moveSpeed = _runSpeed;
        }

        _playerRigidbody.velocity = new Vector3(move * _moveSpeed, _playerRigidbody.velocity.y, 0f);
        _animator.SetFloat("VerticalSpeed", _playerRigidbody.velocity.y);
    }

    private void HandleFlipping()
    {
        float move = _horizontalInput;

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

    public float GetHorizontalInput() => _horizontalInput;
    public Rigidbody GetRigidbody() => _playerRigidbody;
}