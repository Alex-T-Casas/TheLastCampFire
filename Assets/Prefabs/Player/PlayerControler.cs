using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] Transform GroundCheck;
    [SerializeField] Transform GroundCheck_front;
    [SerializeField] Transform GroundCheck_back;
    [SerializeField] Transform GroundCheck_left;
    [SerializeField] Transform GroundCheck_right;
    [SerializeField] float WalkingSpeed = 5f;
    [SerializeField] float RotationSpeed = 24f;
    [SerializeField] float GroundCheckRadius = 0.1f;
    [SerializeField] float SideGroundCheckRadius = 0.01f;
    [SerializeField] LayerMask GroundCheckMask;
    InputActions inputActions;
    Vector2 MoveInput;
    Vector3 Velocity;
    float Gravity = -9.81f;
    CharacterController characterController;

    bool IsOnGround()
    {
        return Physics.CheckSphere(GroundCheck.position, GroundCheckRadius, GroundCheckMask);
    }

    bool GroundInFront()
    {
        return Physics.CheckSphere(GroundCheck_front.position, SideGroundCheckRadius, GroundCheckMask);
    }

    bool GroundInBack()
    {
        return Physics.CheckSphere(GroundCheck_back.position, SideGroundCheckRadius, GroundCheckMask);
    }

    bool GroundInLeft()
    {
        return Physics.CheckSphere(GroundCheck_left.position, SideGroundCheckRadius, GroundCheckMask);
    }

    bool GroundInRight()
    {
        return Physics.CheckSphere(GroundCheck_right.position, SideGroundCheckRadius, GroundCheckMask);
    }
    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputActions.Gameplay.Move.performed += MoveInputUpdated;
        inputActions.Gameplay.Move.canceled += MoveInputUpdated;
    }

    void MoveInputUpdated(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }


    void Update()
    {
        if(IsOnGround())
        {
            Velocity.y = -0.2f;
        }

        if(!GroundInFront())
        {
            Velocity.z = -10f;//-GetPlayerDesiredMoveDir().z * WalkingSpeed;
        }

        if (!GroundInLeft() || !GroundInRight())
        {
            Velocity.x = -10f;//-GetPlayerDesiredMoveDir().x * WalkingSpeed;
        }

        Velocity.x = GetPlayerDesiredMoveDir().x * WalkingSpeed;
        Velocity.z = GetPlayerDesiredMoveDir().z * WalkingSpeed;
        Velocity.y += Gravity * Time.deltaTime;
        characterController.Move(Velocity * Time.deltaTime);
        UpdateRotation();
    }

    Vector3 GetPlayerDesiredMoveDir()
    {
        return new Vector3(-MoveInput.y, 0f, MoveInput.x).normalized;
    }

    void UpdateRotation()
    {
        Vector3 PlayerDesiredDir = GetPlayerDesiredMoveDir();
        if(PlayerDesiredDir.magnitude == 0)
        {
            PlayerDesiredDir = transform.forward;
        }
        Quaternion DesiredRotation = Quaternion.LookRotation(PlayerDesiredDir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRotation, Time.deltaTime * RotationSpeed);
    }
}
