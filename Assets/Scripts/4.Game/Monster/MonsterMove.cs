using System.Diagnostics.CodeAnalysis;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] LayerMask checkLayer;
    [SerializeField] float detectSize;

    Character character;
    SpriteRenderer rend;

    private void Start()
    {
        character = Character.Instance;
        rend = GetComponent<SpriteRenderer>();

    }

    private void FixedUpdate()
    {
        //transform.position = Vector3.MoveTowards(transform.position, character.transform.position, speed * Time.deltaTime);
        Move();
    }

    Vector3 dir;

    void Move()
    {
        if (chanage)
        {
            Vector3 characterPos = character.transform.position;
            dir = (characterPos - transform.position).normalized;

            if (dir.y != 0)
                dir.y = 0;
        }

        /*if (CheckObstacle(dir))
        {
            if (Mathf.Sign(dir.x) == Mathf.Sign(monNObDir.x))
            {
                Debug.Log("up");
                dir = new Vector3(0, 0, dir.z);
            }

            else if(Mathf.Sign(dir.z) == Mathf.Sign(monNObDir.z))
            {
                Debug.Log("right");
                dir = new Vector3(dir.x, 0, 0);
            }
        }*/

        CheckObstacle();

        /*Vector3 nextPos = transform.position + dir.normalized * speed * Time.deltaTime;

        transform.position = nextPos;*/

        if (dir.x < 0)
            rend.flipX = true;

        else if (dir.x >= 0)
            rend.flipX = false;

        transform.position += dir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    bool chanage = true;

    /*bool CheckObstacle(Vector3 dir)
    {
        Debug.DrawRay(transform.position, dir.normalized * 2, Color.yellow);

        RaycastHit hit;

        Collider[] colls = Physics.OverlapBox(transform.position, Vector3.one, Quaternion.identity, checkLayer);

        if(colls.Length > 0)
        {
            Collider closest = colls[0];
            float shortDistance = Vector3.SqrMagnitude(closest.transform.position - transform.position);

            foreach(Collider coll in colls)
            {
                float distance = Vector3.SqrMagnitude(coll.transform.position - transform.position);

                if(shortDistance > distance)
                    closest = coll;
            }

            if (Physics.Raycast(transform.position, dir.normalized, out hit, 1.5f, checkLayer))
            {
                if (monNObDir == Vector3.zero)
                    monNObDir = (transform.position - closest.transform.position).normalized;

                if (Mathf.Sign(monNObDir.x) != Mathf.Sign(transform.position.x) || Mathf.Sign(monNObDir.z) != Mathf.Sign(transform.position.z))
                    return true;
            }
        }

        if (monNObDir != Vector3.zero)
            monNObDir = Vector3.zero;

        return false;
    }*/

    void CheckObstacle()
    {
        Debug.DrawRay(transform.position, dir * detectSize, Color.yellow);

        RaycastHit hit;

        if (chanage)
        {
            if (Physics.Raycast(transform.position, dir, out hit, detectSize, checkLayer))
            {
                if (Physics.Raycast(transform.position, new Vector3(dir.x, 0, 0).normalized, out hit, detectSize, checkLayer))
                {
                    dir = new Vector3(0, 0, dir.z).normalized;
                    Debug.Log(dir);
                    chanage = false;
                }

                else if (Physics.Raycast(transform.position, new Vector3(0, 0, dir.z).normalized, out hit, detectSize, checkLayer))
                {
                    dir = new Vector3(dir.x, 0, 0).normalized;
                    Debug.Log(dir);
                    chanage = false;
                }
            }
        }

        else
        {
            if (character.charDir != Vector3.zero)
            {
                chanage = true;
                return;
            }
        }

        Collider[] colls = Physics.OverlapBox(transform.position, Vector3.one * detectSize, Quaternion.identity, checkLayer);

        if (colls.Length == 0)
        {
            if (!chanage)
                chanage = true;
        }

        else
        {
            if (chanage)
            {
                if (Physics.Raycast(transform.position, new Vector3(dir.x, 0, 0).normalized, out hit, detectSize, checkLayer))
                {
                    dir = new Vector3(0, 0, dir.z).normalized;
                    Debug.Log(dir);
                    chanage = false;
                }

                else if (Physics.Raycast(transform.position, new Vector3(0, 0, dir.z).normalized, out hit, detectSize, checkLayer))
                {
                    dir = new Vector3(dir.x, 0, 0).normalized;
                    Debug.Log(dir);
                    chanage = false;
                }
            }

            else
            {
                if (Physics.Raycast(transform.position, new Vector3(dir.x, 0, 0).normalized, out hit, detectSize, checkLayer))
                {
                    dir = -dir;
                    Debug.Log(dir);
                    chanage = false;
                }

                else if (Physics.Raycast(transform.position, new Vector3(0, 0, dir.z).normalized, out hit, detectSize, checkLayer))
                {
                    dir = -dir;
                    Debug.Log(dir);
                    chanage = false;
                }
            }

            if (chanage)
            {
                if (dir.x == 0)
                {
                    dir = new Vector3(1, 0, dir.z).normalized;
                    Debug.Log(dir);
                }

                else if (dir.z == 0)
                {
                    dir = new Vector3(dir.z, 0, 1).normalized;
                    Debug.Log(dir);

                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Gizmos.matrix = transform.localToWorldMatrix;
        /*Gizmos.matrix = UnityEngine.Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.5f, 0.5f, 0));*/

        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1, 1f) * detectSize * 2f);
    }
}
