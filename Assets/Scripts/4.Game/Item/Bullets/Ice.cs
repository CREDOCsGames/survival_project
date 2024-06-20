using UnityEngine;

public class Ice : Bullet
{
    bool isFreeze = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        isAbsorb = false;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, initPos) > range)
        {
            DestroyBullet();
        }

        transform.position += dir * speed * Time.deltaTime;

        // ÃÑ¾Ë °¢µµ
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Monster") && collision.collider.GetComponent<Monster>() != null)
        {
            Monster monster = collision.collider.GetComponent<Monster>();

            Freeze();

            Instantiate(effectPrefab, transform.position, transform.rotation);
            
            DamageUI damage = damagePool.Get();

            if (gameManager.isReflect)
                Reflect(collision);

            else if (gameManager.onePenetrate)
                OnePenetrate();

            else if (gameManager.lowPenetrate)
                LowPenetrate();

            if (bulletDamage > 0)
                damage.isMiss = false;

            else if (bulletDamage <= 0)
                damage.isMiss = true;

            float mDef = monster.defence;
            damage.realDamage = Mathf.Clamp(bulletDamage * (1 - (mDef / (20 + mDef))), 0, bulletDamage);
            damage.UISetting();
            damage.transform.position = transform.position;
            damage.gameObject.transform.SetParent(gameManager.damageStorage);

            monster.OnDamaged(damage.realDamage, isFreeze);

            if (gameManager.absorbHp > 0 && !damage.isMiss && !isAbsorb)
            {
                Character.Instance.currentHp += Mathf.Clamp(gameManager.absorbHp, 0f, gameManager.maxAbs);
                isAbsorb = true;
            }

            if (!gameManager.isReflect && !gameManager.lowPenetrate && !gameManager.onePenetrate && !gameManager.penetrate)
            {
                if (!isDestroyed)
                    DestroyBullet();
            }
        }
    }

    void Freeze()
    {
        float rand = Random.Range(0f, 100f);

        if (rand <= 5f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.2f)
            isFreeze = true;

        else
            isFreeze = false;
    }
}
