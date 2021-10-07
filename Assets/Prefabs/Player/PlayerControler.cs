using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] Transform PickupSocketTransform;
    InputActions inputActions;

    LadderClimbingComp ladderClimbingComp;
    MovementComponent movementComponent;
    
    public Transform GetPickupSocketTransform()
    {
        return PickupSocketTransform;
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
        movementComponent = GetComponent<MovementComponent>();
        ladderClimbingComp = GetComponent<LadderClimbingComp>();
        ladderClimbingComp.SetInput(inputActions);
        inputActions.Gameplay.Move.performed += MoveInputUpdated;
        inputActions.Gameplay.Move.canceled += MoveInputUpdated;
        inputActions.Gameplay.Interact.performed += Interact;
    }

    void Interact(InputAction.CallbackContext ctx)
    {
        InteractComponent interactComp = GetComponentInChildren<InteractComponent>();
        if (interactComp != null)
        {
            interactComp.Interact();
        }
    }

    void MoveInputUpdated(InputAction.CallbackContext ctx)
    {
        movementComponent.SetMovementInput(ctx.ReadValue<Vector2>());
    }


}
