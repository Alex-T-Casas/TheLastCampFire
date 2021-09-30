using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] Transform GroundCheck;
    [SerializeField] float WalkingSpeed = 5f;
    [SerializeField] float RotationSpeed = 24f;
    [SerializeField] float TraceingDistance = 1f;
    [SerializeField] float TraceingDipth = 0.8f;
    [SerializeField] float LadderClimbCommitAngle = 20f;
    [SerializeField] float GroundCheckRadius = 0.1f;
    [SerializeField] LayerMask GroundCheckMask;
    [SerializeField] Transform PickupSocketTransform;
    InputActions inputActions;
    Vector2 MoveInput;
    [SerializeField] Vector3 Velocity;
    float Gravity = -9.81f;
    CharacterController characterController;
    [SerializeField] float Dot;

    LadderScript CurrentClimbingLadder;
    List<LadderScript> LaddersNearby = new List<LadderScript>();

    public Transform GetPickupSocketTransform()
    {
        return PickupSocketTransform;
    }

    public void NotifyLadderNearby(LadderScript ladderNearby)
    {
        LaddersNearby.Add(ladderNearby);
    }

    public void NotifyLadderExit(LadderScript ladderExit)
    {
        if (ladderExit == CurrentClimbingLadder)
        {
            CurrentClimbingLadder = null;
            Velocity.y = 0;
        }
        LaddersNearby.Remove(ladderExit);
    }

    LadderScript FindPlayerClimbingLadder()
    {
        Vector3 PlayerDesiredMoveDir = GetPlayerDesiredMoveDir();
        LadderScript ChosenLadder = null;
        float ClosetAngle = 180.0f;
        foreach (LadderScript ladder in LaddersNearby)
        {
            Vector3 LadderDir = ladder.transform.position - transform.position;
            LadderDir.y = 0;
            LadderDir.Normalize();
            float Dot = Vector3.Dot(PlayerDesiredMoveDir, LadderDir);
            float AngleDegrees = Mathf.Acos(Dot) * Mathf.Rad2Deg;
            Debug.Log($"find Ladder with angle {AngleDegrees}");
            if (AngleDegrees < LadderClimbCommitAngle && AngleDegrees < ClosetAngle)
            {
                ChosenLadder = ladder;
                ClosetAngle = AngleDegrees;
            }
        }
        return ChosenLadder;
    }

    bool IsOnGround()
    {
        return Physics.CheckSphere(GroundCheck.position, GroundCheckRadius, GroundCheckMask);
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
        inputActions.Gameplay.Interact.performed += Interact;
    }

    void Interact(InputAction.CallbackContext ctx)
    {
        InteractComponent interactComp = GetComponentInChildren<InteractComponent>();
        if(interactComp!=null)
        {
            interactComp.Interact();
        }
    }


    void MoveInputUpdated(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }


    void HopOnLadder(LadderScript ladderToHopOn)
    {
        if (ladderToHopOn == null) return;

        if (ladderToHopOn != CurrentClimbingLadder)
        {
            Transform snapToTransform = ladderToHopOn.GetClosestSnapTransform(transform.position);
            CurrentClimbingLadder = ladderToHopOn;
            StartCoroutine(MoveToTransform(snapToTransform, 0.2f));
            Debug.Log("Hopped on Ladder");
        }
    }

    IEnumerator MoveToTransform(Transform Destination, float transformTime)
    {
        inputActions.Gameplay.Move.Disable();

        Vector3 StartPos = transform.position;
        Vector3 EndPos = Destination.position;
        Quaternion StartRot = transform.rotation;
        Quaternion EndRot = Destination.rotation;

        float timmer = 0f;
        while (timmer < transformTime)
        {
            timmer += Time.deltaTime;
            //move in here
            Vector3 DeltaMove = Vector3.Lerp(StartPos, EndPos, timmer / transformTime) - transform.position;
            characterController.Move(DeltaMove);
            //rot here
            transform.rotation = Quaternion.Lerp(StartRot, EndRot, timmer / transformTime);
            yield return new WaitForEndOfFrame();
        }
        inputActions.Gameplay.Move.Enable();
    }

    void Update()
    {
            if (CurrentClimbingLadder == null)
            {
                HopOnLadder(FindPlayerClimbingLadder());
            }

            if (CurrentClimbingLadder)
            {
                CalculateClimbingVelocity();
            }
            else
            {
                CalculateWalkingVelocity();
            }

            characterController.Move(Velocity * Time.deltaTime);
            UpdateRotation();
    }

        void CalculateClimbingVelocity()
        {
            if (MoveInput.magnitude == 0)
            {
                Velocity = Vector3.zero;
                return;
            }

            Vector3 LadderDir = CurrentClimbingLadder.transform.forward;
            Vector3 PlayerDesiredMoveDir = GetPlayerDesiredMoveDir();

            Dot = Vector3.Dot(LadderDir, PlayerDesiredMoveDir);
            Velocity = Vector3.zero;

            if (Dot < 0)
            {
                Velocity = GetPlayerDesiredMoveDir() * WalkingSpeed;
                Velocity.y = WalkingSpeed;
                Velocity.z = 0;

        }
            else
            {
                if (IsOnGround())
                {
                    Velocity = GetPlayerDesiredMoveDir() * WalkingSpeed;
                }
                Velocity.y = -WalkingSpeed;
                Velocity.z = 0;
        }
        }

        void CalculateWalkingVelocity()
        {
            if (IsOnGround())
            {
                Velocity.y = -0.2f;
            }

            Velocity.x = GetPlayerDesiredMoveDir().x * WalkingSpeed;
            Velocity.z = GetPlayerDesiredMoveDir().z * WalkingSpeed;
            Velocity.y += Gravity * Time.deltaTime;

            Vector3 PosXTrancePos = transform.position + new Vector3(TraceingDistance, 0.5f, 0f);
            Vector3 NegXTrancePos = transform.position + new Vector3(-TraceingDistance, 0.5f, 0f);
            Vector3 PosZTrancePos = transform.position + new Vector3(0f, 0.5f, TraceingDistance);
            Vector3 NegZTrancePos = transform.position + new Vector3(0f, 0.5f, -TraceingDistance);

            bool CanGoPosX = Physics.Raycast(PosXTrancePos, Vector3.down, TraceingDipth, GroundCheckMask);
            bool CanGoNegX = Physics.Raycast(NegXTrancePos, Vector3.down, TraceingDipth, GroundCheckMask);
            bool CanGoPosZ = Physics.Raycast(PosZTrancePos, Vector3.down, TraceingDipth, GroundCheckMask);
            bool CanGoNegZ = Physics.Raycast(NegZTrancePos, Vector3.down, TraceingDipth, GroundCheckMask);

            float xMin = CanGoNegX ? float.MinValue : 0f;
            float xMax = CanGoPosX ? float.MaxValue : 0f;
            float zMin = CanGoNegZ ? float.MinValue : 0f;
            float zMax = CanGoPosZ ? float.MaxValue : 0f;

            Velocity.x = Mathf.Clamp(Velocity.x, xMin, xMax);
            Velocity.z = Mathf.Clamp(Velocity.z, zMin, zMax);
        }

        Vector3 GetPlayerDesiredMoveDir()
        {
            return new Vector3(-MoveInput.y, 0f, MoveInput.x).normalized;
        }

        void UpdateRotation()
        {
            if (CurrentClimbingLadder != null)
            {
                return;
            }
            Vector3 PlayerDesiredDir = GetPlayerDesiredMoveDir();
            if (PlayerDesiredDir.magnitude == 0)
            {
                PlayerDesiredDir = transform.forward;
            }

            Quaternion DesiredRotation = Quaternion.LookRotation(PlayerDesiredDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRotation, Time.deltaTime * RotationSpeed);
        }
}
