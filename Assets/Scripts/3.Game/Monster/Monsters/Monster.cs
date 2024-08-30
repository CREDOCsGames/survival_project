using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Animator anim;
    [SerializeField] Collider attackColl;
    [SerializeField] BoxCollider hitCollder;
    [SerializeField] Vector2 attackRange;
    [SerializeField] float attackDelay;
    [SerializeField] float moveDelay;
    [SerializeField] public int attackCount;
    [SerializeField] AudioClip damagedSound;

    int itemDropPercent;
    int initAttackCount;

    public int InitAttackCount => initAttackCount;
    float moveSpeed;
    float damage;


    [HideInInspector] protected bool isDead = false, isAttack = false;

    public float hp;
    public float maxHp;

    [HideInInspector] protected Vector3 initScale;
    [HideInInspector] public MonsterStat stat;

    private IObjectPool<Monster> managedPool;

    GameManager gameManager;
    Character character;
    GamesceneManager gamesceneManager;
    SoundManager soundManager;

    IEnumerator runningCoroutine;

    Color initcolor;

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
        soundManager = SoundManager.Instance;

        StartSetting();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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
        AttackEnd();
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
        attackCount = stat.attackCount;
        moveDelay = stat.moveDelay;
        initAttackDelay = attackDelay;
        initAttackCount = attackCount;
        initMoveDelay = moveDelay;
        itemDropPercent = stat.itemDropPercent;
        isDead = false;
        isAttack = false;
        canAttack = false;
        anim.speed = 1f;
        transform.localScale = initScale;
        rend.color = initcolor;
        rend.sortingOrder = initOrder;

        GetComponent<MonsterMove>().InitSetting(moveSpeed);

        hitCollder.enabled = true;
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
           character.OnDamaged(damage, gameObject.transform.GetComponentInChildren<MonsterHit>().gameObject);
        }
    }

    void Attack()
    {
        if (isDead || !canAttack)
            return;

        xDistance = Mathf.Abs(character.transform.position.x - transform.position.x);
        zDistance = Mathf.Abs(character.transform.position.z - transform.position.z);

        if (!isAttack && !gameManager.isClear)
        {
            if (xDistance <= attackRange.x && zDistance <= attackRange.y)
            {
                isAttack = true;
                GetComponent<MonsterMove>().agent.enabled = false;
                canMove = false;
                canAttack = false;
            }
        }

        anim.SetBool("isAttack", isAttack);
    }

    void AttackEnd()
    {
        if (!isAttack)
            return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
                isAttack = false;
                anim.SetBool("isAttack", isAttack);
            }
        }
    }

    void AttackDelay()
    {
        if (isAttack || canAttack)
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
        if (canMove || isAttack)
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

        if(hp <= 0)
            soundManager.PlaySFX(damagedSound);
    }

    public void OnDead()
    {
        if (hp <= 0 || !gamesceneManager.isNight)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                GetComponent<MonsterMove>().agent.enabled = false;
                rend.sortingOrder = 0;
                anim.speed = 1f;
                isDead = true;
                rend.color = Color.white;

                if (itemDropPercent > 0)
                {
                    int rand = Random.Range(0, 100);

                    if (rand < itemDropPercent)
                    {
                        gameManager.totalBulletCount++;

                        character.getItemUI.GetComponent<GetItemUI>().SetBulletGetImage();
                        character.getItemUI.gameObject.SetActive(true);
                    }
                }

                if (runningCoroutine != null)
                    StopCoroutine(runningCoroutine);

                if (attackColl != null)
                    attackColl.enabled = false;

                anim.SetTrigger("Die");
            }

            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
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
        GetComponent<MonsterMove>().agent.enabled = false;
        hitCollder.enabled = false;
    }

    
}
