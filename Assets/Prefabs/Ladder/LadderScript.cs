using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    [SerializeField] PlayerControler Player;
    [SerializeField] Transform TopSnapTransform;
    [SerializeField] Transform BottomSnapTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        LadderClimbingComp otherAsPlayer = other.GetComponent<LadderClimbingComp>();
        if(otherAsPlayer!=null)
        {
            otherAsPlayer.NotifyLadderNearby(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LadderClimbingComp otherAsPlayer = other.GetComponent<LadderClimbingComp>();
        if (otherAsPlayer != null)
        {
            otherAsPlayer.NotifyLadderExit(this);
        }
    }

    public Transform GetClosestSnapTransform(Vector3 Postion)
    {
        float DistanceToTop = Vector3.Distance(Postion, TopSnapTransform.position);
        float DistanceToBot = Vector3.Distance(Postion, BottomSnapTransform.position);
        return DistanceToTop < DistanceToBot ? TopSnapTransform : BottomSnapTransform;

    }
}
