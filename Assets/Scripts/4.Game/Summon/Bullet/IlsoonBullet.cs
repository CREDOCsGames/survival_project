using UnityEngine;

public class IlsoonBullet : SummonsBullet
{
    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager.longDamage > 0)
            damage = Mathf.Round(gameManager.longDamage * 10f * (1f + gameManager.summonPDmg) * 10f) * 0.1f;
        else
            damage = 0;
    }
    private void Update()
    {
        transform.position += new Vector3(dir.x, 0, dir.z) * speed * Time.deltaTime;

        // ÃÑ¾Ë °¢µµ
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            gameManager = GameManager.Instance;

            if (gameManager.longDamage > 0)
                damage = Mathf.Round(gameManager.longDamage * 10f * (1f + gameManager.summonPDmg) * 10f) * 0.1f;
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
            other.GetComponent<Monster>().PureOnDamaged(damage);
        }
    }
}
