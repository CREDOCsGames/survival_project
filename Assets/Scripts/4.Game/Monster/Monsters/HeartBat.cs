using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class HeartBat : Monster
{
    [SerializeField] GameObject plantBullet;
    [SerializeField] Transform bulletPos;

    private IObjectPool<MonsterBullet> pool;

    float attackTime = 3;

    private void Awake()
    {
        pool = new ObjectPool<MonsterBullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 5);
    }

    private void Start()
    {
       StartSetting();
    }

    protected override void InitMonsterSetting()
    {
        base.InitMonsterSetting();
        attackTime = 3;
    }

    private void Update()
    {
        freezeEffect.SetActive(isFreeze);

        if (!isDead)
        {
            Move();

            if (!isFreeze)
            {
                attackTime -= Time.deltaTime;

                if (attackTime <= 0)
                {
                    attackTime = 3;
                    Attack();
                }
            }

            if (isFreeze)
            {
                anim.speed = 0f;
            }
        }

        OnDead();
    }

    void Attack()
    {
        anim.SetTrigger("isAttack");
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.3f);

        MonsterBullet bullet = pool.Get();
        bullet.randNum = 0;
        bullet.transform.position = new Vector3(bulletPos.position.x, 0, bulletPos.position.z);
        bullet.ShootDir();
        bullet.AutoDestroyBullet();
    }

    private MonsterBullet CreateBullet()
    {
        MonsterBullet bullet = Instantiate(plantBullet, bulletPos.position, transform.rotation).GetComponent<MonsterBullet>();
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
