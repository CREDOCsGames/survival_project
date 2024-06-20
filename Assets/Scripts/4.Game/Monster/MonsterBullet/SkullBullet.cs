using UnityEngine;

public class SkullBullet : MonsterBullet
{
    void Start()
    {
        gameManager = GameManager.Instance;
        realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;  // 트리거에도 있음
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isClear && gameManager.isBossDead)
        {
            CancelInvoke("DestroyBullet");
            DestroyBullet();
        }

        if (randNum == 0)
            transform.position += dir * speed * Time.deltaTime;

        else if (randNum == 1)
        {
            transform.RotateAround(monsPos, Vector3.up, speed * 60f * Time.deltaTime);
            transform.position += dir * speed * Time.deltaTime;
            dir = (transform.position - monsPos).normalized;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            if (gameManager.currentGameTime > 0)
                realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;

            else if (gameManager.currentGameTime <= 0)
                realDamage = (bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f) * 2f;

            other.GetComponent<Character>().OnDamaged(realDamage);
            DestroyBullet();
            CancelInvoke("DestroyBullet");
        }
    }

    public override void ShootDir()
    {
        speed = (randNum == 0) ? 6f : 2f;

        if (GameManager.Instance.currentGameTime <= 0)
        {
            if (GameManager.Instance.round == 10)
                speed = speed * 1.2f;

            else if (GameManager.Instance.round == 30)
                speed = speed * 1.5f;
        }

        if (randNum == 0)
        {
            dir = (Character.Instance.transform.position - transform.position).normalized;
            dir = new Vector3(dir.x, 0f, dir.z);
        }

        else if (randNum == 1)
        {
            dir = (transform.position - monsPos).normalized;
            dir = new Vector3(dir.x, 0f, dir.z);
        }
    }

    public override void AutoDestroyBullet()
    {
        Invoke("DestroyBullet", 3f);
    }
}
