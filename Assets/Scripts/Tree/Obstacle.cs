using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] LayerMask collLayer;

    private void OnTriggerEnter(Collider other)
    { 
        //if ((collLayer | (1 << other.gameObject.layer )) != collLayer)
        if(other.gameObject.layer == LayerMask.NameToLayer("MouseInteraction"))
        {
            Destroy(other.gameObject);
        }
    }
}
