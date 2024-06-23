using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Node
{
    int nodeX;
    int nodeZ;
    bool canWalk;

    public Node(int nodeX, int nodeZ, bool canWalk)
    {
        this.nodeX = nodeX;
        this.nodeZ = nodeZ;
        this.canWalk = canWalk;
    }
}


public class MapNode : MonoBehaviour
{
    [SerializeField] LayerMask checkLayer;

    BoxCollider mapColl;

    private void Start()
    {
        mapColl = GetComponent<BoxCollider>();

        Collider[] colls = Physics.OverlapBox(transform.position, new Vector3(mapColl.size.x, 0, mapColl.size.y) * 0.5f, Quaternion.identity, checkLayer);

        foreach(Collider collider in colls)
        {
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            Debug.Log("1");
    }

    private void OnDrawGizmos()
    {
        // Gizmos.matrix = transform.localToWorldMatrix;
        /*Gizmos.matrix = UnityEngine.Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.5f, 0.5f, 0));*/

        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        //Gizmos.DrawWireCube(transform.position, new Vector3(mapColl.size.x, 0, mapColl.size.y));
    }
}
