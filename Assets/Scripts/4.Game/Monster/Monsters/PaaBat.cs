using UnityEngine;
using UnityEngine.Pool;

public class PaaBat : Monster
{
    [SerializeField] GameObject batLazor;
    [SerializeField] Transform rightBulletPos;
    [SerializeField] Transform leftBulletPos;

    private IObjectPool<MonsterBullet> pool;

    float xDistance;
    float zDistance;

    private void Awake()
    {
        pool = new ObjectPool<MonsterBullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 5);
    }

    void Start()
    {
        StartSetting();
    }

    private void Update()
    {
        rigid.velocity = Vector3.zero;

        freezeEffect.SetActive(isFreeze);

        if (isDead == false)
        {
            if (!isAttack)
                Move();

            anim.SetBool("isWalk", isWalk);

            if (!isFreeze)
                Attack();

            if (isFreeze)
            {
                anim.speed = 0f;
            }
        }

        OnDead();
    }

    void Attack()
    {
        xDistance = Mathf.Abs(character.transform.position.x - transform.position.x);
        zDistance = Mathf.Abs(character.transform.position.z - transform.position.z);
        anim.SetBool("isAttack", isAttack);

        if (xDistance < 3 && zDistance <= 0.8f)
        {
            isWalk = false;
            isAttack = true;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                isWalk = true;
                isAttack = false;
            }
        }
    }

    public void InstantLazor()
    {
        MonsterBullet lazor = pool.Get();
        Transform bulletPos = rend.flipX ? leftBulletPos : rightBulletPos;
        lazor.transform.position = new Vector3(bulletPos.position.x, 0, bulletPos.position.z);
        lazor.GetComponent<PaaLazor>().isFlip = rend.flipX;
        lazor.transform.SetParent(bulletPos);
    }

    private MonsterBullet CreateBullet()
    {
        Transform bulletPos = rend.flipX ? leftBulletPos : rightBulletPos;
        MonsterBullet bullet = Instantiate(batLazor, bulletPos).GetComponent<MonsterBullet>();
        bullet.SetManagedPool(pool);
        return bullet;
    }

    private void OnGetBullet(MonsterBullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(MonsterBullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(MonsterBullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
