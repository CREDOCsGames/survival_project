using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour
{
    [SerializeField] protected GameObject freezeEffect;
    [SerializeField] protected Rigidbody rigid;
    [HideInInspector] protected SpriteRenderer rend;
    [HideInInspector] protected Animator anim;
    [HideInInspector] protected Collider coll;

    [HideInInspector] public bool isWalk, isDead, isAttacked, isAttack = false;

    public float hp;
    public float maxHp;

    [HideInInspector] protected Vector3 initScale;
    [HideInInspector] public MonsterStat stat;

    private IObjectPool<Monster> managedPool;

    protected float speed;

    [HideInInspector] public Vector3 dir;

    protected bool isFreeze = false;

    protected float initSpeed = 0;

    protected GameManager gameManager;
    protected Character character;

    protected IEnumerator runningCoroutine;

    public float defence;

    protected float damage;

    bool beforeFreeze;

    protected Color initcolor;

    void Start()
    {
        StartSetting();
    }

    protected void StartSetting()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();

        hp = stat.monsterMaxHp * (2 + Mathf.Floor(gameManager.round / 5) * Mathf.Floor(gameManager.round / 5) * (1 + Mathf.Floor(gameManager.round / 20))) * 0.5f;
        damage = stat.monsterDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;
        maxHp = hp;
        initScale = transform.localScale;
        speed = stat.monsterSpeed * (1 - gameManager.monsterSlow * 0.01f);
        initSpeed = speed;
        defence = stat.monsterDefence * (1 + Mathf.Floor(gameManager.round / 5) * 0.5f) * (1 - gameManager.monsterDef);
        isWalk = true;
        isDead = false;
        isAttacked = false;
        isAttack = false;
        coll.enabled = true;
        beforeFreeze = false;
        initcolor = rend.color;
    }

    void Update()
    {
        freezeEffect.SetActive(isFreeze);

        rigid.velocity = Vector3.zero;

        if (isDead == false)
        {
            Move();
            anim.SetBool("isWalk", isWalk);
        }

        if (isFreeze)
        {
            anim.speed = 0f;
        }

        OnDead();
    }

    protected virtual void InitMonsterSetting()
    {
        hp = stat.monsterMaxHp * (2 + Mathf.Floor(gameManager.round / 5) * Mathf.Floor(gameManager.round / 5) * (1 + Mathf.Floor(gameManager.round / 20) * 0.5f)) * 0.5f;
        damage = stat.monsterDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;
        maxHp = hp;
        speed = stat.monsterSpeed * (1 - gameManager.monsterSlow * 0.01f);
        defence = stat.monsterDefence * (1 + Mathf.Floor(gameManager.round / 5) * 0.5f) * (1 - gameManager.monsterDef);
        initSpeed = speed;
        isWalk = true;
        isDead = false;
        isAttacked = false;
        isAttack = false;
        coll.enabled = true;
        beforeFreeze = false;
        anim.speed = 1f;
        transform.position = Vector3.zero;
        transform.localScale = initScale;
        initScale = transform.localScale;
        rend.color = Color.white;
        initcolor = rend.color;
        rend.sortingOrder = 2;
    }

    public void Move()
    {
        if (!isFreeze)
        {
            anim.speed = 1f;

            Vector3 characterPos = character.transform.position;
            dir = characterPos - transform.position;

            transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, 0, transform.position.z), characterPos, speed * Time.deltaTime);

            isWalk = true;

            if (dir == Vector3.zero || speed == 0)
                isWalk = false;

            if (dir.x < 0)
                rend.flipX = true;

            else if (dir.x >= 0)
                rend.flipX = false;
        }
    }

    protected IEnumerator MonsterColorBlink()
    {
        Color semiWhite = initcolor;
        semiWhite.a = 0.5f;

        for (int i = 0; i < 3; i++)
        {
            rend.color = initcolor;
            yield return new WaitForSeconds(0.1f);

            rend.color = semiWhite;
            yield return new WaitForSeconds(0.1f);
        }

        rend.color = initcolor;
    }   

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character") && !isDead)
        {
            character.OnDamaged(damage);
        }
    }

    public void SubscriptionFee()
    {
        if (gameManager.subscriptionFee)
        {
            int feeRand = Random.Range(0, 100);

            if (feeRand < 4)
            {
                gameManager.feeMoney += 38;
                gameManager.money += 38;
            }
        }
    }

    // 방어력 없이 바로 깎이는 대미지
    public void PureOnDamaged(float damage)
    {
        hp -= damage;

        if (!isFreeze)
        {
            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            runningCoroutine = MonsterColorBlink();
            StartCoroutine(runningCoroutine);
        }
    }

    public void OnDamaged(float damage)
    {
        hp -= damage;

        if (!isFreeze)
        {
            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            runningCoroutine = MonsterColorBlink();
            StartCoroutine(runningCoroutine);
        }
    }

    public void OnDamaged(float damage, bool freeze)
    {
        hp -= damage;

        if (freeze && !beforeFreeze)
        {
            beforeFreeze = freeze;
            isFreeze = freeze;
            anim.speed = 0f;

            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            runningCoroutine = MonsterFreeze();
            StartCoroutine(runningCoroutine);
        }

        if (!isFreeze)
        {
            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            runningCoroutine = MonsterColorBlink();
            StartCoroutine(runningCoroutine);
        }
    }

    protected IEnumerator MonsterFreeze()
    {
        rend.color = Color.cyan;
        yield return new WaitForSeconds(1.5f);

        anim.speed = 1f;
        isFreeze = false;
        beforeFreeze = isFreeze;
        speed = initSpeed;
        rend.color = initcolor;
    }

    public void OnDead()
    {
        if (hp <= 0 || (gameManager.isClear && gameManager.isBossDead) || character.isDead)
        {
            rend.sortingOrder = 0;
            anim.speed = 1f;
            isDead = true;
            rend.color = Color.white;
            isFreeze = false;
            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);
            coll.enabled = false;

            if (hp <= 0 && !isAttacked)
            {
                SubscriptionFee();
                if (gameManager.currentGameTime > 0)
                {
                    int coinValue = Random.Range(stat.monsterCoin - 2, stat.monsterCoin + 1);
                    DropCoin.Instance.Drop(transform.position, coinValue);
                }
                character.exp += stat.monsterExp * (1 + gameManager.increaseExp);
            }

            isAttacked = true;

            anim.SetBool("isAttacked", isAttacked);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    DestroyMonster();
                }
            }
        }
    }

    public void SetManagedPool(IObjectPool<Monster> pool)
    {
        managedPool = pool;
    }

    public void DestroyMonster()
    {
        managedPool.Release(this);
        InitMonsterSetting();
    }
}
