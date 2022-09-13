using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool canMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(SprintKey);
    private bool ShouldJump => Input.GetKeyDown(JumpKey) && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;

    [Header("Controls")]
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;

    [Header("Movement Parameters")]
    [SerializeField] private float WalkSpeed = 3.0f;
    [SerializeField] private float SprintSpeed = 6.0f;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float LookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float LookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float UpperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float LowerLookLimit = 80.0f;


    [Header("Jumping Parameters")]
    [SerializeField] private float JumpForce = 8.0f;
    [SerializeField] private float Gravity = 30.0f;

    private Camera PlayerCamera;
    private CharacterController characterController;

    private Vector3 moveDirecton;
    private Vector2 currentInput;

    private float RotationX = 0;

    private void Awake()
    {
        PlayerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            
            if(canJump)
                HandleJump();

            ApplyFinalyMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isSprinting ? SprintSpeed : WalkSpeed) * Input.GetAxis("Vertical"), (isSprinting ? SprintSpeed : WalkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirecton.y;

        moveDirecton = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);

        moveDirecton.y = moveDirectionY;



    }

    private void HandleMouseLook()
    {
        RotationX -= Input.GetAxis("Mouse Y") * LookSpeedY;
        RotationX = Mathf.Clamp(RotationX, -UpperLookLimit, LowerLookLimit);
        PlayerCamera.transform.localRotation = Quaternion.Euler(RotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeedX, 0);

    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirecton.y = JumpForce;
        }
    }


    private void ApplyFinalyMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirecton.y -= Gravity * Time.deltaTime;//en yakin yuzeye ceker
        }

        characterController.Move(moveDirecton * Time.deltaTime);
    }








}
