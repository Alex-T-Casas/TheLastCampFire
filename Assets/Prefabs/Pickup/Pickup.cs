using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void PickedUpBy(GameObject PickerGameObject)
    {
        Transform pickUpSocketTransform = PickerGameObject.transform;

        PlayerControler PickerAsPlayer = PickerGameObject.GetComponent<PlayerControler>();
        if (PickerAsPlayer != null)
        {
            pickUpSocketTransform = PickerAsPlayer.GetPickupSocketTransform();
        }
        transform.rotation = pickUpSocketTransform.transform.rotation;
        transform.parent = pickUpSocketTransform.transform;
        transform.localPosition = Vector3.zero;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    public virtual void DropedDown()
    {
        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    public override void Interact(GameObject InteractingGameObject)
    {
        if (transform.parent != null)
        {
            DropedDown();
        }
        else
        {
            Vector3 DirfromInteractingGameObj = (transform.position - InteractingGameObject.transform.position).normalized;
            Vector3 DirOfInteractingGameObj = InteractingGameObject.transform.forward;
            float Dot = Vector3.Dot(DirOfInteractingGameObj, DirfromInteractingGameObj);
            if (Dot > 0.5f)
            {
                PickedUpBy(InteractingGameObject);
            }
        }
    }
}
