using System.Collections;
using UnityEngine;

public class Summons : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletTransform;
    [SerializeField] protected float speed;
    [SerializeField] protected float moveRange;
    [SerializeField] protected float attackDelay;

    protected float defaultSpeed;

    protected bool isAttack;
    protected bool canAttack;

    protected Animator anim;
    protected SpriteRenderer rend;

    protected Vector3 randomPos;
    protected float distance;
    protected bool isNear;    // 캐릭터 주변에 있는지

    protected Character character;
    protected GameManager gameManager;

    virtual protected void InitSetting()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        character = Character.Instance;
        gameManager = GameManager.Instance;
        isAttack = false;
        isNear = true;
        canAttack = true;
        defaultSpeed = speed;
        InvokeRepeating("GetRandomPos", 0.5f, 1f);
    }

    protected void CheckDistance()
    {
        distance = Vector3.Magnitude(character.transform.position - transform.position);
        if (distance > 3)
        {
            isNear = false;
            CancelInvoke("GetRandomPos");
        }

        else
        {
            if (!isNear)
            {
                randomPos = character.transform.position;
                InvokeRepeating("GetRandomPos", 0.5f, 1f);
                isNear = true;
            }
        }
    }

    protected void GetRandomPos()
    {
        Vector3 randPoint = Random.onUnitSphere * moveRange;
        randPoint.y = 0;

        randomPos = character.transform.position + randPoint;
    }

    protected IEnumerator IEAttackDelay()
    {
        yield return new WaitForSeconds(attackDelay - gameManager.summonASpd);

        canAttack = true;
    }

    virtual public void EndAttack()
    {
        isAttack = false;
        speed = defaultSpeed;
        InvokeRepeating("GetRandomPos", 0.5f, 1f);
        StartCoroutine(IEAttackDelay());
    }

    virtual protected void OnTriggerStay(Collider other)
    {
        if (canAttack)
        {
            if (other.CompareTag("Monster"))
            {
                SoundManager.Instance.PlayES("Chick");
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
                SummonsBullet bullet = Instantiate(bulletPrefab).GetComponent<SummonsBullet>();
                bullet.transform.position = new Vector3(transform.position.x + firePos.x, 0, transform.position.z + firePos.y);
                bullet.Fire(dir);
                speed = 0;
                canAttack = false;
            }
        }
    }
}
