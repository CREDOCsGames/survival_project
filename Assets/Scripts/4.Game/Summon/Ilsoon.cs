using UnityEngine;

public class Ilsoon : Summons
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitSetting();
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
                SoundManager.Instance.PlayES("Ilsoon");
                isAttack = true;
                Vector3 dir = other.gameObject.transform.position - transform.position;

                if (dir.x < 0)
                    rend.flipX = true;

                else if (dir.x >= 0)
                    rend.flipX = false;

                CancelInvoke("GetRandomPos");
                IlsoonBullet bullet = Instantiate(bulletPrefab).GetComponent<IlsoonBullet>();
                bullet.transform.position = other.transform.position;
                speed = 0;
                canAttack = false;
            }
        }
    }
}
