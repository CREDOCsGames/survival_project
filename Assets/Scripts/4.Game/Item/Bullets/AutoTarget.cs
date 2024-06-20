using System.Linq;
using UnityEditor;
using UnityEngine;

public class AutoTarget : Bullet
{
    [SerializeField] LayerMask monsterLayer;
    Monster targetMonster;

    bool isFind;

    Monster beforeTarget;

    float moveDistance = 0;
    float afterRange;

    private void Start()
    {
        gameManager = GameManager.Instance;
        afterRange = range;
        isFind = false;
        isAbsorb = false;
    }

    void Update()
    {
        FindTarget();
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, initPos) > afterRange)
        {
            DestroyBullet();
        }

        // ÃÑ¾Ë °¢µµ
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);
    }

    void FindTarget()
    {
        if (targetMonster != null && targetMonster.GetComponent<Monster>().hp <= 0)
        {
            targetMonster = null;
            isFind = false;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, monsterLayer);
        float[] distances = new float[colliders.Length];

        if (colliders.Length > 0 && !isFind)
        {
            var find = from target in colliders
                       where target.gameObject.CompareTag("Monster") && target.GetComponent<Monster>() != null
                       orderby Vector3.Distance(transform.position, target.transform.position)
                       select target.gameObject;

            if (find.Count() > 0)
            {
                foreach (var target in find)
                {
                    Monster monster = target.GetComponent<Monster>();

                    if (beforeTarget != null)
                    {
                        if (monster != beforeTarget)
                        {
                            targetMonster = monster;
                            moveDistance = Vector3.Distance(targetMonster.transform.position, beforeTarget.transform.position);
                            afterRange = afterRange - moveDistance;
                            beforeTarget = targetMonster;

                            break;
                        }
                    }

                    if (beforeTarget == null)
                    {
                        targetMonster = monster;
                        beforeTarget = targetMonster;
                    }
                }
            }


            if (targetMonster != null && targetMonster.hp > 0)
            {
                if (!isFind)
                {
                    dir = (targetMonster.gameObject.transform.position - transform.position).normalized;
                    dir.y = 0;
                    isFind = true;
                }
            }
        }
    }

    public override void Reflect(Collision collision)
    {
        base.Reflect(collision);
        targetMonster = null;
        isFind = false;
    }

    public override void OnePenetrate()
    {
        base.OnePenetrate();
        targetMonster = null;
        isFind = false;
    }

    public override void LowPenetrate()
    {
        base.LowPenetrate();
        targetMonster = null;
        isFind = false;
    }

    public override void DestroyBullet()
    {
        targetMonster = null;
        beforeTarget = null;
        isFind = false;
        afterRange = range;
        base.DestroyBullet();
    }
}
