using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float moveSpeed;

    [Header("Rotation Settings")]
    [SerializeField, Tooltip("How fast the character rotates when flipping")] private float rotationSpeed = 10f;
    private bool facingRight;


    [Header("Jump")]
    [SerializeField, Tooltip("How big the ground collider check sphere is")] private float groundCheckRadius = 0.2f;
    [SerializeField, Tooltip("What layers are considered ground")] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float jumpForce = 5f;
    private bool isGrounded = false;
    private Collider[] groundColliders;


    private Rigidbody playerRigidbody;
    private Animator animator;
    private Quaternion targetRotation;
    private bool jumpPressed;


    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        facingRight = true;

        targetRotation = transform.rotation;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        jumpPressed = Input.GetButtonDown("Jump");
    }

    void FixedUpdate()
    {
        // Check if the player is on the ground
        bool wasGrounded = isGrounded;
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;
        animator.SetBool("Grounded", isGrounded);

        if (!wasGrounded && isGrounded && playerRigidbody.velocity.y < 0.1f)
        {
            animator.SetTrigger("Land");
        }

        //Jumping
        if (jumpPressed && isGrounded)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetBool("Grounded", false);
            animator.ResetTrigger("Land");
        }

        float move = Input.GetAxis("Horizontal");
        animator.SetFloat("RunningSpeed", Mathf.Abs(move));
        float walk = Input.GetAxis("Fire1");
        animator.SetFloat("WalkingSpeed", Mathf.Abs(walk));

        if (walk > 0 && isGrounded)
        {
            moveSpeed = walkSpeed;
        }
        else
        {
            moveSpeed = runSpeed;
        }
        playerRigidbody.velocity = new Vector3(move * moveSpeed, playerRigidbody.velocity.y, 0f);
        animator.SetFloat("VerticalSpeed", GetComponent<Rigidbody>().velocity.y);

        if (move > 0 && !facingRight)
        {
            facingRight = true;
            targetRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (move < 0 && facingRight)
        {
            facingRight = false;
            targetRotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }
}