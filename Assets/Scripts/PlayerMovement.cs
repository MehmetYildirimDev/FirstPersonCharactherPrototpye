using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    CharacterController characterController;

    //Jump
    public float JumpHeight = 1f;
    Vector3 velocity;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;

    //crouching
    public float PlayerHeight;
    public float PlayerPivotCenter;
    public Camera MainCamera;
    


    //Move
    public float MoveSpeed = 3f;

    public float x;
    public float z;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        PlayerHeight = characterController.height;
        PlayerPivotCenter = characterController.center.y;
    }

    private void Update()
    {
        #region Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = 6f;
        }
        else
        {
            MoveSpeed = 3f;
        }


        #region Movement
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * MoveSpeed * Time.deltaTime);

        #endregion


        #region Jump

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //GrandMask => mesala planein layerini da ground yapiyoruz yani esitliyoruz 
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        #endregion



        #endregion

        #region Crouch
        if (Input.GetKey(KeyCode.LeftControl))
        {
            characterController.height = PlayerHeight / 2;        
            MoveSpeed = 1f;
            
        }
        else
        {
            characterController.height = PlayerHeight;
            MoveSpeed = 3f;
        }


        #endregion


    }


}
