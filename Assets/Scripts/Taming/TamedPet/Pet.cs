using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;

public class Pet : MonoBehaviour
{
    [SerializeField] float moveRange;
    [SerializeField] float speed;
    [SerializeField] float detactRange;
    [SerializeField] float damage;
    [SerializeField] float attackLength;
    [SerializeField] LayerMask monsterLayer;

    Character character;
    GameManager gameManager;

    Vector3 randomPos;

    bool isNear = false;
    bool isAttack = false;

    float distance;

    Vector3 startPoint;
    Vector3 attackDir;

    Animator anim;
    SpriteRenderer spriteRenderer;

    bool canAttack = true;


    private void Start()
    {
        character = Character.Instance;
        gameManager = GameManager.Instance;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isAttack)
        {
            CheckDistance();

            if (isNear)
                transform.position = Vector3.MoveTowards(transform.position, randomPos, speed * Time.deltaTime);

            else if (!isNear)
                transform.position = Vector3.MoveTowards(transform.position, character.transform.position, character.speed * 1.5f * Time.deltaTime);

            CheckMonster();
        }

        else
            Attack();

        anim.SetBool("isAttack", isAttack);
    }

    void CheckDistance()
    {
        distance = Vector3.Magnitude(character.transform.position - transform.position);

        if (distance > 3)
        {
            isNear = false;
            CancelInvoke("GetRandomPos");
        }

        else
        {
            if (!isNear)
            {
                randomPos = character.transform.position;
                InvokeRepeating("GetRandomPos", 0.5f, 1f);
                isNear = true;
            }
        }
    }

    void GetRandomPos()
    {
        Vector3 randPoint = Random.onUnitSphere * moveRange;
        randPoint.y = 0;

        randomPos = character.transform.position + randPoint;
    }

    void Flip()
    {
        spriteRenderer.flipX = attackDir.x < 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttack)
            return;

        Debug.Log(isAttack);

        //if (other.CompareTag("Monster") && other.transform.parent.GetComponent<Monster>() != null)
        if(other.GetComponent<IDamageable>() != null)
        {
            Debug.Log(other.name);

            float realDamage = gameManager.specialStatus[SpecialStatus.Soulmate] ? damage + 4 : damage;

            other.GetComponent<IDamageable>().Attacked(realDamage, gameObject);
            other.GetComponent<IDamageable>().RendDamageUI(realDamage, other.transform.position, false, false);
        }
    }

    void CheckMonster()
    {
        if (!canAttack)
            return;

        Collider[] monsters = Physics.OverlapSphere(transform.position, detactRange, monsterLayer);

        if (monsters.Length > 0)
        {
            var find = from monster in monsters
                       where monster.transform.parent.GetComponent<Monster>() != null
                       orderby Vector3.Distance(transform.position, monster.transform.position)
                       select monster.gameObject;

            if (find.Count() > 0)
            {
                foreach (var monster in find)
                {
                    attackDir = (monster.transform.position - transform.position).normalized;
                    attackDir.y = 0;
                    isAttack = true;
                    canAttack = false;
                    startPoint = transform.position;
                    Flip();
                    StartCoroutine(AttackCool());

                    break;
                }
            }
        }
    }

    IEnumerator AttackCool()
    {
        yield return CoroutineCaching.WaitForSeconds(5);

        canAttack = true;
    }

    private void Attack()
    {
        transform.position += attackDir * speed * 4 * Time.deltaTime;

        if(Vector3.Distance(startPoint, transform.position) >= attackLength)
            isAttack = false;
    }

    public void RunAway()
    {
        if (character.GetPetRound + 5 == gameManager.round)
            character.RunAwayPet();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detactRange);
    }
}
