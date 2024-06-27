using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] LayerMask checkLayer;
    [SerializeField] float detectSize;

    Character character;
    SpriteRenderer rend;

    bool isNeedToChangeDir = true;
    Vector3 dir;

    NavMeshAgent agent;
    NavMeshPath path;

    private void Start()
    {
        character = Character.Instance;
        rend = transform.GetChild(1).GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        path = new NavMeshPath();
    }

    private void Update()
    {
        Flip();
    }

    private void FixedUpdate()
    {
        //transform.position = Vector3.MoveTowards(transform.position, character.transform.position, speed * Time.deltaTime);
        //Move();
        if (agent.enabled)
        {
            agent.SetDestination(character.transform.position);
            agent.speed = GetComponent<Monster>().Speed;
            //
            //agent.updateRotation = false;
            //Debug.Log(agent.nextPosition);
            //transform.position = agent.nextPosition * speed * Time.deltaTime;
            //Debug.Log(agent.nextPosition);
            //transform.position = Vector3.MoveTowards(transform.position, agent.nextPosition, speed * Time.deltaTime);
        }
            //agent.destination = character.transform.position;
            //agent.SetDestination(character.transform.position);
    }

    Vector3[] corners;
    int count = 1;

    void Flip()
    {
        if (corners == null || corners != agent.path.corners)
        {
            if (agent.path.corners.Length > 1)
            {
                corners = agent.path.corners;
                count = 1;
            }

            else
            {
                dir = (character.transform.position - transform.position).normalized;
            }
        }

        if (corners != null)
        {
            dir = (corners[count] - transform.position).normalized;

            if (transform.position == corners[count] && count < agent.path.corners.Length - 1)
                count++;
        }

        rend.flipX = dir.x < 0;
    }

    void Move()
    {
        if (isNeedToChangeDir)
        {
            Vector3 characterPos = character.transform.position;
            dir = (characterPos - transform.position).normalized;

            if (dir.y != 0)
                dir.y = 0;
        }

        CheckObstacle2();

        rend.flipX = dir.x < 0;

        transform.position += dir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

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

    Vector3 obsDir = Vector3.zero;
    bool canDetectRay = false;

    void CheckObstacle()
    {
        Debug.DrawRay(transform.position, dir * detectSize, Color.yellow);

        if (isNeedToChangeDir)
        {
            // 몬스터와 캐릭터 사이의 장애물 체크
            if (Physics.Raycast(transform.position, dir, detectSize, checkLayer))
            {
                // 위아래 혹은 좌우 방향 장애물 체크 후 이동 방향 전환.
                DirMeetObstacle();
                Debug.Log(obsDir);
            }
        }

        // 장애물을 만나 이동 방향이 변경된 경우 
        else
        {
            // 캐릭터가 이동해 이동 방향을 다시 계산해야 할 경우
            //if (MathF.Sign(character.charDir.x) != Vector3.zero)
            if(character.charDir != Vector3.zero)
            {
                obsDir = Vector3.zero;
                isNeedToChangeDir = true;
                return;
            }

            canDetectRay = false;
        }

        if (!canDetectRay)
        {
            //Collider[] colls = Physics.OverlapBox(transform.position, Vector3.one * detectSize, Quaternion.identity, checkLayer);
            Collider[] colls = Physics.OverlapSphere(transform.position, detectSize, checkLayer);

            if (colls.Length == 0)
            {
                if (!isNeedToChangeDir)
                {
                    Debug.Log("obszero");
                    obsDir = Vector3.zero;
                    isNeedToChangeDir = true;
                }
            }

            else
            {
                if (obsDir == Vector3.zero)
                {
                    Debug.Log("!!");
                    DirMeetObstacle();
                }

                if (obsDir.x == 0)
                {
                    obsDir = new Vector3(1, 0, dir.z).normalized;
                }

                else if (obsDir.z == 0)
                {
                    obsDir = new Vector3(dir.z, 0, 1).normalized;
                }
            }
        }

        if (obsDir != Vector3.zero)
            dir = obsDir;
    }

    void CheckObstacle2()
    {
        //Debug.DrawRay(transform.position, dir.normalized * (detectSize + 0.2f), Color.yellow);

        Collider[] colls = Physics.OverlapSphere(transform.position, detectSize, checkLayer);

        if (colls.Length > 0)
        {
            if (isNeedToChangeDir)
            {
                if (Physics.Raycast(transform.position, dir, detectSize + 0.2f, checkLayer))
                {
                    Debug.DrawRay(transform.position, dir.normalized * (detectSize + 0.2f), Color.magenta);

                    if (Physics.Raycast(transform.position - new Vector3(dir.x, 0, 0).normalized * detectSize, new Vector3(0, 0, dir.z).normalized, detectSize + 0.2f, checkLayer))
                    {
                        Debug.DrawRay(transform.position - new Vector3(dir.x, 0, 0).normalized, new Vector3(0, 0, dir.z).normalized * (detectSize + 0.2f), Color.yellow);
                        dir = new Vector3(dir.x, 0, 0).normalized;
                        isNeedToChangeDir = false;
                    }

                    else if (Physics.Raycast(transform.position - new Vector3(0, 0, dir.z).normalized * detectSize, new Vector3(dir.x, 0, 0).normalized, detectSize + 0.2f, checkLayer))
                    {
                        Debug.DrawRay(transform.position - new Vector3(0, 0, dir.z).normalized, new Vector3(dir.x, 0, 0).normalized * (detectSize + 0.2f), Color.cyan);
                        dir = new Vector3(0, 0, dir.z).normalized;
                        isNeedToChangeDir = false;
                    }

                    /*else
                    {
                        Debug.Log("1");
                        //Debug.DrawRay(transform.position, dir.normalized * (detectSize + 0.2f), Color.yellow);
                        dir = new Vector3(dir.x, 0, 0).normalized;
                        isNeedToChangeDir = false;
                    }*/
                }

                else
                {
                    if (Physics.Raycast(transform.position - new Vector3(0, 0, dir.z) * detectSize, new Vector3(0, 0, dir.z).normalized, detectSize + 0.2f, checkLayer))
                    {
                        Debug.DrawRay(transform.position - new Vector3(0, 0, dir.z), dir.normalized * (detectSize + 0.2f), Color.yellow);
                        dir = new Vector3(dir.x, 0, 0).normalized;
                        isNeedToChangeDir = false;
                    }

                    else if (Physics.Raycast(transform.position - new Vector3(dir.x, 0, 0) * detectSize, new Vector3(dir.x, 0, 0).normalized, detectSize + 0.2f, checkLayer))
                    {
                        Debug.DrawRay(transform.position - new Vector3(dir.x, 0, 0), dir.normalized * (detectSize + 0.2f), Color.yellow);
                        dir = new Vector3(0, 0, dir.z).normalized;
                        isNeedToChangeDir = false;
                    }
                }
            }

            else
            {
                if (character.charDir != Vector3.zero)
                    isNeedToChangeDir = true;
            }
        }

        else
        {
            if (!isNeedToChangeDir)
            {
                Debug.Log("!!");
                isNeedToChangeDir = true;
            }
        }
    }

    void DirMeetObstacle()
    {
        if (Physics.Raycast(transform.position, new Vector3(dir.x, 0, 0).normalized, detectSize, checkLayer))
        {
            obsDir = isNeedToChangeDir ? new Vector3(0, 0, dir.z).normalized : -dir;
            isNeedToChangeDir = false;
            canDetectRay = true;
        }

        else if (Physics.Raycast(transform.position, new Vector3(0, 0, dir.z).normalized, detectSize, checkLayer))
        {
            obsDir = isNeedToChangeDir ? new Vector3(dir.x, 0, 0).normalized : -dir;
            isNeedToChangeDir = false;
            canDetectRay = true;
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
        //Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1, 1f) * detectSize * 2f);
        Gizmos.DrawWireSphere(transform.position, detectSize);
    }
}
