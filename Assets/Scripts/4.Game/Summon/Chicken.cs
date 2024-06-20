using UnityEngine;


public class Chicken : Summons
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
                SoundManager.Instance.PlayES("Chicken");
                isAttack = true;
                Vector3 dir = other.gameObject.transform.position - transform.position;
                Vector3 firePos = bulletTransform.localPosition;

                if (dir.x < 0)
                {
                    firePos.x = firePos.x * -1;
                    rend.flipX = true;
                }

                else if (dir.x >= 0)
                    rend.flipX = false;

                CancelInvoke("GetRandomPos");
                SummonLazor bullet = Instantiate(bulletPrefab).GetComponent<SummonLazor>();
                bullet.transform.SetParent(transform);
                bullet.transform.localPosition = firePos;
                bullet.Fire(dir);

                speed = 0;
                canAttack = false;
            }
        }
    }
}
