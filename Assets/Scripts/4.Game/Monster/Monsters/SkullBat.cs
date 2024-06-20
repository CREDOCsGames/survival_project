using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class SkullBat : Monster
{
    [SerializeField] GameObject skullBullet;
    [SerializeField] Transform[] bulletPoses;
    [SerializeField] Slider monsterHpBar;

    float attackTime = 5;
    float initAttackTime = 5;

    private IObjectPool<MonsterBullet> pool;

    bool isBerserk;
    bool isPatern;

    private void Awake()
    {
        pool = new ObjectPool<MonsterBullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 6);
    }

    void Start()
    {
        StartSetting();
        if (gameManager.round == 20)
            hp = 8500;

        else if (gameManager.round == 30)
            hp = 55000;
        maxHp = hp;
        attackTime = 5;
        initAttackTime = 5;
        isBerserk = false;
        isPatern = false;
    }

    protected override void InitMonsterSetting()
    {
        base.InitMonsterSetting();
        attackTime = 5;
        initAttackTime = 5;
        isBerserk = false;
    }

    void Update()
    {
        monsterHpBar.value = 1 - (hp / maxHp);

        freezeEffect.SetActive(isFreeze);

        if (gameManager.currentGameTime <= 0 && !isBerserk)
        {
            int round = gameManager.round;
            isBerserk = true;
            initcolor = new Color(1f, 0.5f, 0.5f, 1f);
            float berserkDam = (round == 20) ? 1.5f : 2f;
            damage = stat.monsterDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f * berserkDam;
            float berserkSpd = (round == 20) ? 1.2f : 1.5f;
            speed = stat.monsterSpeed * (1 - gameManager.monsterSlow * 0.01f) * berserkSpd;
            initSpeed = speed;
            initAttackTime = (round == 20) ? 3.5f : 2.5f;
            rend.color = initcolor;
        }

        if (isDead == false && !isAttack)
        {
            if (!isFreeze)
            {
                Move();

                if (!isPatern)
                    attackTime -= Time.deltaTime;

                if (attackTime <= 0)
                {
                    attackTime = initAttackTime;
                    Attack();
                }
            }
        }

        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isAttack", isAttack);

        BossDead();
    }

    void BossDead()
    {
        if (hp <= 0 || character.isDead)
        {
            anim.speed = 1f;
            rend.color = Color.white;
            isFreeze = false;
            coll.enabled = false;
            isDead = true;
            isAttacked = true;

            anim.SetBool("isAttacked", isAttacked);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    if (hp <= 0)
                    {
                        gameManager.money += 500;
                        SoundManager.Instance.PlayES("LevelUp");
                        character.level++;
                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    void Attack()
    {
        rand = Random.Range(0, 2);

        isWalk = false;
        isAttack = true;
        isPatern = true;
    }

    public void EndAttack()
    {
        if (rand == 0)
        {
            isAttack = false;
            isWalk = true;
            isPatern = false;
        }

        else if(rand == 1)
        {
            isAttack = false;
            StartCoroutine(AttackDelay());
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(2.5f);
        isWalk = true;
        isPatern = false;
    }

    int rand;

    public void Shoot()
    {           
        for (int i = 0; i < bulletPoses.Length; i++)
        {
            MonsterBullet bullet = pool.Get();
            bullet.transform.position = new Vector3(bulletPoses[i].position.x, 0, bulletPoses[i].position.z);
            bullet.monsPos = transform.position;
            bullet.randNum = rand;
            bullet.GetComponent<SkullBullet>().ShootDir();
            bullet.GetComponent<SkullBullet>().AutoDestroyBullet();
        }
    }

    private MonsterBullet CreateBullet()
    {
        MonsterBullet bullet = Instantiate(skullBullet, bulletPoses[0].position, transform.rotation).GetComponent<MonsterBullet>();
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
