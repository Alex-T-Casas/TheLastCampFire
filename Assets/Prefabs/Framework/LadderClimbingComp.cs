using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimbingComp : MonoBehaviour
{
    [Header("Ladder Values")]
    [SerializeField] float LadderClimbCommitAngle = 20f;
    [SerializeField] float LadderHopOnTime = 2.0f;
    LadderScript CurrentClimbingLadder;
    List<LadderScript> LaddersNearby = new List<LadderScript>();

    MovementComponent movementComponent;

    IInputActionCollection InputAction;

    void Start()
    {
        movementComponent = GetComponent<MovementComponent>();
    }
    public void SetInput(IInputActionCollection inputAction)
    {
        InputAction = inputAction;
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
            movementComponent.SetClimbingInfo(Vector3.zero, false);

            movementComponent.ClearVerticalVelocity();
        }
        LaddersNearby.Remove(ladderExit);
    }

    LadderScript FindPlayerClimbingLadder()
    {
        Vector3 PlayerDesiredMoveDir = movementComponent.GetPlayerDesiredMoveDir();
        LadderScript ChosenLadder = null;
        float ClosetAngle = 180.0f;
        foreach (LadderScript ladder in LaddersNearby)
        {
            Vector3 LadderDir = ladder.transform.position - transform.position;
            LadderDir.y = 0;
            LadderDir.Normalize();
            float Dot = Vector3.Dot(PlayerDesiredMoveDir, LadderDir);
            float AngleDegrees = Mathf.Acos(Dot) * Mathf.Rad2Deg;
            if (AngleDegrees < LadderClimbCommitAngle && AngleDegrees < ClosetAngle)
            {
                ChosenLadder = ladder;
                ClosetAngle = AngleDegrees;
            }
        }
        return ChosenLadder;
    }

    void HopOnLadder(LadderScript ladderToHopOn)
    {
        if (ladderToHopOn == null) return;

        if (ladderToHopOn != CurrentClimbingLadder)
        {
            Transform snapToTransform = ladderToHopOn.GetClosestSnapTransform(transform.position);
            CurrentClimbingLadder = ladderToHopOn;
            movementComponent.SetClimbingInfo(ladderToHopOn.transform.forward, true);
            DisableMovement();
            StartCoroutine(movementComponent.MoveToTransform(snapToTransform, LadderHopOnTime));
            Invoke("EnableMovement", LadderHopOnTime);
        }
    }

    public void EnableMovement()
    {
        InputAction.Enable();
    }

    public void DisableMovement()
    {
        InputAction.Disable();
    }

    void Update()
    {
        if (CurrentClimbingLadder == null)
        {
            HopOnLadder(FindPlayerClimbingLadder());
        }
    }
}
