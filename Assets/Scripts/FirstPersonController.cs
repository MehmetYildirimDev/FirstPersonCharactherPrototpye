using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool canMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(SprintKey) && !isCrouching;
    // !isCrouching bunu ben ekledim hata olursa bak
    private bool ShouldJump => Input.GetKeyDown(JumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(CrouchKey) && !DuringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHaedBob = true;

    [Header("Controls")]
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float WalkSpeed = 3.0f;
    [SerializeField] private float SprintSpeed = 6.0f;
    [SerializeField] private float CrouchSpeed = 1.5f;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float LookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float LookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float UpperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float LowerLookLimit = 80.0f;


    [Header("Jumping Parameters")]
    [SerializeField] private float JumpForce = 8.0f;
    [SerializeField] private float Gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float CroucHeight = 0.5f;
    [SerializeField] private float StandHeight = 2f;
    [SerializeField] private float TimeToCrouch = 0.25f;
    [SerializeField] private Vector3 CrouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 StandingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool DuringCrouchAnimation;


    [Header("HeadBob Parameters")]
    [SerializeField] private float WalkBobSpeed = 14f;
    [SerializeField] private float WalkBobAmount = 0.05f;
    [SerializeField] private float SprintBobSpeed = 18f;
    [SerializeField] private float SprintBobAmount = 0.1f;
    [SerializeField] private float CrouchBobSpeed = 8f;
    [SerializeField] private float CrouchBobAmount = 0.025f;
    private float defultYPos = 0f;
    private float timer;



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
        defultYPos = PlayerCamera.transform.localPosition.y;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
                HandleJump();

            if (canCrouch)
                HandleCrouch();

            if (canUseHaedBob)
                HandleHeadBob();

            ApplyFinalyMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? CrouchSpeed : isSprinting ? SprintSpeed : WalkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? CrouchSpeed : isSprinting ? SprintSpeed : WalkSpeed) * Input.GetAxis("Horizontal"));

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

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirecton.x) > 0.1f || Mathf.Abs(moveDirecton.z) > 0.1f)//bunun yerine magnutide olur mu acaba
        {
            timer += Time.deltaTime * (isCrouching ? CrouchBobSpeed : isSprinting ? SprintBobSpeed : WalkBobSpeed);
            PlayerCamera.transform.localPosition = new Vector3(
                PlayerCamera.transform.localPosition.x,
                defultYPos + Mathf.Sin(timer) * (isCrouching ? CrouchBobAmount : isSprinting ? SprintBobAmount : WalkBobAmount),
                PlayerCamera.transform.localPosition.x);
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

    private IEnumerator CrouchStand()
    {
        //tavani fark edebilmek icin ///yoksa kafamiz icine giriyo

        if (isCrouching && Physics.Raycast(PlayerCamera.transform.position, Vector3.up, 1f))
            yield break;


        DuringCrouchAnimation = true;

        float timeElapsed = 0f;
        float targetHeight = isCrouching ? StandHeight : CroucHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? StandingCenter : CrouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < TimeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / TimeToCrouch);
            //Saniyenin 4de 1 kadar surede
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / TimeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //Yukarda minik aciklar veriyor o yuzden net olsun diye buraya yazdik
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        DuringCrouchAnimation = false;
    }






}
