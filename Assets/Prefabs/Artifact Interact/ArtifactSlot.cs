using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSlot : MonoBehaviour
{
    [SerializeField] Platfrom platformToMove;
    [SerializeField] Transform ArtifactTrans;
   public void OnArtifactLeft()
    {
        platformToMove.MoveTo(platformToMove.StartTrans);
        Debug.Log("on artifact");
    }

    public void OnArtifactPlaced()
    {
        platformToMove.MoveTo(platformToMove.EndTrans);
        Debug.Log("Placed on me");

    }

    public Transform GetSlotTrans()
    {
        return ArtifactTrans;
    }
}
