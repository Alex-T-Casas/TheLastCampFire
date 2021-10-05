using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampMesh : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] Transform Transform;
    // Start is called before the first frame update

    private void OnTriggernEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = Transform;
        }
    }

    private void OnCollisionExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = null;
        }
    }
}
