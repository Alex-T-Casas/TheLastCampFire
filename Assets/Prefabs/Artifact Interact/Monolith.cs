using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monolith : Interactable
{
    [SerializeField] Ramp RampToMove;
    bool MovedObj = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact(GameObject InteractingGameObject = null)
    {
        if (MovedObj == true)
        {
            RampToMove.MoveTo(RampToMove.StartTrans);
        }
        RampToMove.MoveTo(RampToMove.EndTrans);
        MovedObj = true;

    }
}
