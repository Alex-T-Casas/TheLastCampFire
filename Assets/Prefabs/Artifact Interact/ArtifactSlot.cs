using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSlot : MonoBehaviour
{
    [SerializeField] GameObject ToggelingObject;

    [SerializeField] Transform ArtifactTrans;

   public void OnArtifactLeft()
    {
        ToggelingObject.GetComponent<Toggleable>().ToggleOff();
    }

    public void OnArtifactPlaced()
    {
        ToggelingObject.GetComponent<Toggleable>().ToggleOn();
    }

    public Transform GetSlotTrans()
    {
        return ArtifactTrans;
    }
}
