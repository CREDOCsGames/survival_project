using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float detectSize;

    Character character;
    SpriteRenderer rend;

    Vector3 dir;

    public NavMeshAgent agent;

    private void Start()
    {
        character = Character.Instance;
        rend = transform.GetChild(1).GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;

        agent.speed = GetComponent<Monster>().Speed;
    }

    private void Update()
    {
        Flip();
    }

    private void FixedUpdate()
    {
        if (agent.enabled)
        {
            agent.SetDestination(character.transform.position);
        }
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
}
