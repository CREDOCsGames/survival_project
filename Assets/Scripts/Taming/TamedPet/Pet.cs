using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;
using UnityEngine.Pool;

public class Pet : MonoBehaviour
{
    [SerializeField] DamageUI damageUIPreFab;
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

    protected IObjectPool<DamageUI> pool;

    private void Awake()
    {
        pool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 10);
    }

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

        if (other.CompareTag("Monster") && other.transform.parent.GetComponent<Monster>() != null)
        {
            Monster monster = other.transform.parent.GetComponent<Monster>();

            DamageUI damageUI = pool.Get();
            damageUI.realDamage = damage;

            damageUI.UISetting(false, false);
            damageUI.transform.position = monster.transform.position;
            damageUI.gameObject.transform.SetParent(gameManager.damageStorage);

            monster.OnDamaged(damage);
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
                       where monster.CompareTag("Monster") && monster.transform.parent.GetComponent<Monster>() != null
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
        yield return new WaitForSeconds(5);

        canAttack = true;
    }

    private void Attack()
    {
        transform.position += attackDir * speed * 4 * Time.deltaTime;

        if(Vector3.Distance(startPoint, transform.position) >= attackLength)
            isAttack = false;
    }

    public void EndAttack()
    {

    }

    public void RunAway()
    {
        if (character.GetPetRound + 5 == gameManager.round)
            character.RunAwayPet();
    }

    private DamageUI CreateDamageUI()
    {
        DamageUI damageUIPool = Instantiate(damageUIPreFab, transform.position, damageUIPreFab.transform.rotation).GetComponent<DamageUI>();
        damageUIPool.SetManagedPool(pool);
        damageUIPool.transform.SetParent(gameManager.bulletStorage);
        return damageUIPool;
    }

    private void OnGetDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(true);
    }

    private void OnReleaseDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(false);
    }

    private void OnDestroyDamageUI(DamageUI damageUIPool)
    {
        Destroy(damageUIPool.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detactRange);

    }
}
