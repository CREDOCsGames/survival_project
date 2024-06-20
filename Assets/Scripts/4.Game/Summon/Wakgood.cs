using UnityEngine;
using UnityEngine.Pool;

public class Wakgood : Summons
{
    [SerializeField] DamageUI damageUIPreFab;

    float damage;

    Monster monster;

    protected IObjectPool<DamageUI> pool;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        pool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 10);

    }

    void Start()
    {
        InitSetting();
        if (gameManager.shortDamage > 0)
            damage = Mathf.Round(gameManager.shortDamage * 7f * (1f + gameManager.summonPDmg) * 10f) * 0.1f;

        else
            damage = 0;
    }

    private void Update()
    {
        if (!isAttack)
        {
            CheckDistance();

            if (isNear)
                transform.position = Vector3.MoveTowards(transform.position, randomPos, speed * Time.deltaTime);

            else if (!isNear)
                transform.position = Vector3.MoveTowards(transform.position, character.transform.position, gameManager.speed * 2 * Time.deltaTime);
        }

        anim.SetBool("isAttack", isAttack);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (canAttack)
        {
            if (other.CompareTag("Monster"))
            {
                SoundManager.Instance.PlayES("Wakgood");

                if (other.GetComponent<Monster>() != null)
                    monster = other.GetComponent<Monster>();

                isAttack = true;
                Vector3 dir = other.gameObject.transform.position - transform.position;

                if (dir.x < 0)
                    rend.flipX = true;

                else if (dir.x >= 0)
                    rend.flipX = false;

                CancelInvoke("GetRandomPos");

                speed = 0;
                canAttack = false;
            }
        }
    }

    public override void EndAttack()
    {
        base.EndAttack();

        if (gameManager.shortDamage > 0)
            damage = Mathf.Round(gameManager.shortDamage * 7f * (1f + gameManager.summonPDmg) * 10f) * 0.1f;

        else
            damage = 0;

        DamageUI damageUI = pool.Get();
        damageUI.realDamage = damage;

        if (damage > 0)
            damageUI.isMiss = false;

        else if (damage <= 0)
            damageUI.isMiss = true;

        damageUI.UISetting();
        damageUI.transform.position = transform.position;
        damageUI.gameObject.transform.SetParent(gameManager.damageStorage);

        if (monster != null)
            monster.PureOnDamaged(damage);
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
}
