using UnityEngine;

public class HugBat : Monster
{
    bool isRush = false;
    float rushTime;
    float breakTime;
    float attackCoolTime;
    float distance;

    private void Start()
    {
        rushTime = 2f;
        breakTime = 1f;
        attackCoolTime = 1f;
        StartSetting();
    }

    protected override void InitMonsterSetting()
    {
        base.InitMonsterSetting();
        isRush = false;
        rushTime = 1.5f;
        breakTime = 0f;
        attackCoolTime = 1f;
    }

    private void Update()
    {
        rigid.velocity = Vector3.zero;

        freezeEffect.SetActive(isFreeze);

        if (isDead == false)
        {
            Move();
            anim.SetBool("isWalk", isWalk);

            if (!isFreeze)
                Attack();
        }

        if (isFreeze)
        {
            anim.speed = 0f;
        }

        OnDead();
    }

    void Attack()
    {
        distance = Vector3.Magnitude(character.transform.position - transform.position);
        anim.SetBool("isAttack", isAttack);

        if (distance <= 3f && !isRush)
            isRush = true;

        if (isRush)
        {
            if (attackCoolTime > 0f)
            {
                isAttack = true;
                rushTime -= Time.deltaTime;
                speed = stat.monsterSpeed * (1 - gameManager.monsterSlow * 0.01f) * 1.5f;

                if (rushTime <= 0f)
                {
                    isAttack = false;
                    speed = 0f;
                    breakTime -= Time.deltaTime;
                }

                if (breakTime <= 0f)
                {
                    speed = stat.monsterSpeed * (1 - gameManager.monsterSlow * 0.01f);

                    attackCoolTime -= Time.deltaTime;
                }
            }

            if (attackCoolTime <= 0f)
            {
                rushTime = 2f;
                breakTime = 1f;
                attackCoolTime = 1f;
                isRush = false;
            }
        }
    }
}
