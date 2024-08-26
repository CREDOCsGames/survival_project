using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    float moveTime;
    float waitTime;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] BoxCollider weaponBoxCollider;
    [SerializeField] Animator anim;
    [SerializeField] GameObject weapon;

    Character character;
    GameManager gameManager;

    Vector3 dir;

    [HideInInspector] public NavMeshAgent agent;

    float initScaleX;
    float initColliderX;
    float initWeaponColliderX;

    [SerializeField] float initMoveTime;
    [SerializeField] float initWaitTime;

    [SerializeField] float initSpeed;

    private void Awake()
    {
        character = Character.Instance;
        gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;

        initScaleX = transform.localScale.x;
        initColliderX = boxCollider.size.x;

        if (weaponBoxCollider != null)
            initWeaponColliderX = weaponBoxCollider.size.x;

        initMoveTime = moveTime;
        initWaitTime = waitTime;

    }

    private void Update()
    {
        anim.SetBool("isWalk", agent.enabled);

        if (!GetComponent<Monster>().CanMove || gameManager.isClear)
        {
            if(gameManager.isClear) 
            {
                agent.enabled = false;
            }

            return;
        }

        Flip();

        if (agent.enabled)
        {
            agent.SetDestination(character.transform.position);

            if (initMoveTime != 0)
            {
                moveTime -= Time.deltaTime;

                if (moveTime <= 0)
                {
                    agent.enabled = false;
                    moveTime = initMoveTime;
                }
            }
        }

        else
        {
            if (initWaitTime != 0)
            {
                waitTime -= Time.deltaTime;

                if (waitTime <= 0)
                {
                    agent.enabled = true;
                    waitTime = initWaitTime;
                }
            }
        }
    }

    public void InitSetting(float speed)
    {
        initSpeed = speed;
        agent.speed = initSpeed;
    }

    public void InitailizeCoolTime()
    {
        waitTime = initWaitTime;
        moveTime = initMoveTime;
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

        float scaleX = dir.x > 0 ? -initScaleX : initScaleX;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        float colliderX = dir.x > 0 ? -initColliderX : initColliderX;
        boxCollider.size = new Vector3(colliderX, boxCollider.size.y, boxCollider.size.z);

        if (weaponBoxCollider != null)
        {
            float weaponColliderX = dir.x > 0 ? -initWeaponColliderX : initWeaponColliderX;
            weaponBoxCollider.size = new Vector3(weaponColliderX, weaponBoxCollider.size.y, weaponBoxCollider.size.z);
        }

        //rend.flipX = dir.x > 0;
    }

    public void RotateWeapon(Vector3 bulletDir)
    {
        float scaleX = transform.localScale.x > 0 ? 1 : -1;
        weapon.transform.localScale =  new Vector3(scaleX, weapon.transform.localScale.y, transform.localScale.z);

        float angle = Mathf.Atan2(bulletDir.z, bulletDir.x) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(90, -angle + 180, 0);

        if (bulletDir.x < 0)
            weapon.transform.rotation *= Quaternion.Euler(0, 0, 0);

        else
            weapon.transform.rotation *= Quaternion.Euler(180, 0, 0);
    }
}
