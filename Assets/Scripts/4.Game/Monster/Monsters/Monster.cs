using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected SpriteRenderer rend;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Collider attackColl;
    [SerializeField] protected Vector2 attackRange;
    [SerializeField] float attackDelay;
    [SerializeField] float moveDelay;
    protected float moveSpeed;
    protected float damage;


    [HideInInspector] protected bool isDead = false, isAttack = false;

    public float hp;
    public float maxHp;

    [HideInInspector] protected Vector3 initScale;
    [HideInInspector] public MonsterStat stat;

    private IObjectPool<Monster> managedPool;

    protected GameManager gameManager;
    protected Character character;
    protected GamesceneManager gamesceneManager;

    protected IEnumerator runningCoroutine;

    protected Color initcolor;

    public float Speed => moveSpeed;

    protected float initSpeed = 0;
    float xDistance;
    float zDistance;

    float initAttackDelay;
    float initMoveDelay;

    bool canAttack = true;
    bool canMove = true;

    int initOrder;

    public bool CanMove => canMove;

    public int monsterNum;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;
        character = Character.Instance;

        StartSetting();
    }

    protected void StartSetting()
    {
        initScale = transform.localScale;
        initcolor = rend.color;
        initOrder = rend.sortingOrder;
    }

    void Update()
    {
        Attack();
        AttackDelay();
        MoveDelay();
        OnDead();
    }

    public void InitMonsterSetting(bool isLeader)
    {
        hp = stat.monsterMaxHp * (2 + Mathf.Floor(gameManager.round / 5) * Mathf.Floor(gameManager.round / 5) * (1 + Mathf.Floor(gameManager.round / 20) * 0.5f)) * 0.5f;
        damage = stat.monsterDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;
        maxHp = hp;
        moveSpeed = stat.monsterSpeed;
        initSpeed = moveSpeed;
        attackDelay = stat.attackDelay;
        moveDelay = stat.moveDelay;
        initAttackDelay = attackDelay;
        initMoveDelay = moveDelay;
        isDead = false;
        isAttack = false;
        canAttack = false;
        anim.speed = 1f;
        transform.localScale = initScale;
        rend.color = initcolor;
        rend.sortingOrder = initOrder;

        GetComponent<MonsterMove>().InitSetting(moveSpeed);
    }

    protected IEnumerator MonsterColorBlink()
    {
        Color semiWhite = initcolor;
        semiWhite.a = 0.5f;

        for (int i = 0; i < 3; i++)
        {
            rend.color = initcolor;
            yield return CoroutineCaching.WaitForSeconds(0.1f);

            rend.color = semiWhite;
            yield return CoroutineCaching.WaitForSeconds(0.1f);
        }

        rend.color = initcolor;
    }   

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && !isDead)
        {
           character.OnDamaged(damage);
        }
    }

    void Attack()
    {
        if (isDead || !canAttack)
            return;

        xDistance = Mathf.Abs(character.transform.position.x - transform.position.x);
        zDistance = Mathf.Abs(character.transform.position.z - transform.position.z);

        if (!isAttack)
        {
            if (xDistance <= attackRange.x && zDistance <= attackRange.y)
            {
                isAttack = true;
                GetComponent<MonsterMove>().agent.enabled = false;
                canMove = false;
            }
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
                isAttack = false;
                canAttack = false;
            }
        }

        anim.SetBool("isAttack", isAttack);
    }

    void AttackDelay()
    {
        if (canAttack)
            return;

        attackDelay -= Time.deltaTime;

        if(attackDelay <= 0)
        {
            canAttack = true;
            attackDelay = initAttackDelay;
        }
    }

    void MoveDelay()
    {
        if (canMove)
            return;

        moveDelay -= Time.deltaTime;

        if(moveDelay <= 0)
        {
            GetComponent<MonsterMove>().InitailizeCoolTime();
            GetComponent<MonsterMove>().agent.enabled = true;
            canMove = true;
            moveDelay = initMoveDelay;
        }
    }
    public void OnDamaged(float damage)
    {
        hp -= damage;

        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = MonsterColorBlink();
        StartCoroutine(runningCoroutine);
    }

    public void OnDead()
    {
        if (hp <= 0 || (gameManager.isClear && gameManager.isBossDead) || character.isDead || !gamesceneManager.isNight)
        {
            GetComponent<MonsterMove>().agent.enabled = false;
            rend.sortingOrder = 0;
            anim.speed = 1f;
            isDead = true;
            rend.color = Color.white;

            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            if (attackColl != null)
                attackColl.enabled = false;

            anim.SetTrigger("Die");

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    DestroyMonster();
                }
            }
        }
    }

    public void ChangeOutline(Color color)
    {
        rend.material.SetColor("_SolidOutline", color);
    }

    public void SetManagedPool(IObjectPool<Monster> pool)
    {
        managedPool = pool;
    }

    public void DestroyMonster()
    {
        managedPool.Release(this);
        transform.position = Vector3.zero;
    }
}
