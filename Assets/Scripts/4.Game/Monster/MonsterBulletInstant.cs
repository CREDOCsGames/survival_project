using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class MonsterBulletInstant : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletPos;

    private IObjectPool<MonsterBullet> pool;

    Character character;

    float angle;

    private void Awake()
    {
        pool = new ObjectPool<MonsterBullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 5);

        character = Character.Instance;
    }

    public void Shoot()
    {
        if (transform.parent.GetComponent<Monster>().attackCount <= 0)
            return;

        if (weapon != null)
        {
            transform.parent.GetComponent<MonsterMove>().RotateWeapon();
        }

        transform.parent.GetComponent<Monster>().attackCount--;
        MonsterBullet bullet = pool.Get();
        bullet.transform.position = new Vector3(bulletPos.position.x, 0, bulletPos.position.z);
        bullet.bulletDamage = transform.parent.GetComponent<Monster>().stat.monsterDamage;

        if (bullet.GetComponent<DirectianalMovement>() != null)
        {
            bullet.GetComponent<DirectianalMovement>().SetDir(transform.parent.transform.position);
        }

        else if (bullet.gameObject.GetComponent<ThrowObjecMovement>() != null ) 
        {
            bullet.gameObject.GetComponent<ThrowObjecMovement>().SetStartEndPoint(bullet.transform.position, character.transform.position);
            bullet.destroyPos = character.transform.position;
        }
    }

    public void AttackEnd()
    {
        transform.parent.GetComponent<Monster>().attackCount = transform.parent.GetComponent<Monster>().InitAttackCount;
    }

    private MonsterBullet CreateBullet()
    {
        MonsterBullet bullet = Instantiate(bulletPrefab, bulletPos.position, transform.rotation).GetComponent<MonsterBullet>();
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
