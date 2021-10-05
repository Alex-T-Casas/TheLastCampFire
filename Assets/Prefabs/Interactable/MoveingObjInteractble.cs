using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingObjInteractble : Interactable
{
    public override void Interact(GameObject InteractingGameObject = null)
    {
        GetComponentInChildren<Platfrom>().MoveTo(true);
    }
}
