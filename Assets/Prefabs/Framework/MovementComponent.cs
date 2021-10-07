using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Ground Check")] 
    [SerializeField] Transform GroundCheck;
    [SerializeField] float WalkingSpeed = 5f;
    [SerializeField] float RotationSpeed = 24f;
    [SerializeField] float GroundCheckRadius = 0.1f;
    [SerializeField] float TraceingDistance = 1f;
    [SerializeField] float TraceingDipth = 0.8f;
    [SerializeField] LayerMask GroundCheckMask;


    bool isClimbing;
    Vector3 LadderDir;
    Vector2 MoveInput;
    [SerializeField] Vector3 Velocity;
    float Gravity = -9.81f;
    CharacterController characterController;

    Transform currentFloor;
    Vector3 PreviousWorldPos;
    Vector3 PreviousFloorLocalPos;
    Quaternion PreviousWorldRot;
    Quaternion PreviousFloorLocalRot;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void SetMovementInput(Vector2 inputVal)
    {
        MoveInput = inputVal;
    }

    public void ClearVerticalVelocity()
    {
        Velocity.y = 0;
    }

    public void SetClimbingInfo(Vector3 ladderDir, bool climbing)
    {
        LadderDir = ladderDir;
        isClimbing = climbing;
    }

    void CheckFloor()
    {
        Collider[] cols = Physics.OverlapSphere(GroundCheck.position, GroundCheckRadius, GroundCheckMask);
        if (cols.Length != 0)
        {
            if (currentFloor != cols[0].transform)
            {
                currentFloor = cols[0].transform;
                SnapShotPostitionAndRotation();

            }
        }
    }

    void SnapShotPostitionAndRotation()
    {
        PreviousWorldPos = transform.position;
        PreviousWorldRot = transform.rotation;
        if (currentFloor != null)
        {
            PreviousFloorLocalPos = currentFloor.InverseTransformPoint(transform.position);
            PreviousFloorLocalRot = Quaternion.Inverse(currentFloor.rotation) * transform.rotation;
        }
    }

    bool IsOnGround()
    {
        return Physics.CheckSphere(GroundCheck.position, GroundCheckRadius, GroundCheckMask);
    }
    private void Update()
    {
        if (isClimbing)
        {
            CalculateClimbingVelocity();
        }
        else
        {
            CalculateWalkingVelocity();
        }
        CheckFloor();
        FollowFloor();
        characterController.Move(Velocity * Time.deltaTime);
        UpdateRotation();

        SnapShotPostitionAndRotation();
    }
    public IEnumerator MoveToTransform(Transform Destination, float transformTime)
    {

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
    }

    void FollowFloor()
    {
        if (currentFloor)
        {
            Vector3 DeltaMove = currentFloor.TransformPoint(PreviousFloorLocalPos) - PreviousWorldPos;
            Velocity += DeltaMove / Time.deltaTime;

            Quaternion DestinationRot = currentFloor.rotation * PreviousFloorLocalRot; // we are adding
            Quaternion DeltaRot = Quaternion.Inverse(PreviousWorldRot) * DestinationRot;
            transform.rotation = transform.rotation * DeltaRot;
        }
    }

    void CalculateClimbingVelocity()
    {
        if (MoveInput.magnitude == 0)
        {
            Velocity = Vector3.zero;
            return;
        }
        Vector3 PlayerDesiredMoveDir = GetPlayerDesiredMoveDir();

        float Dot = Vector3.Dot(LadderDir, PlayerDesiredMoveDir);
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

    public Vector3 GetPlayerDesiredMoveDir()
    {
        return new Vector3(-MoveInput.y, 0f, MoveInput.x).normalized;
    }

    void UpdateRotation()
    {
        if (isClimbing)
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

    // Update is called once per frame

}
