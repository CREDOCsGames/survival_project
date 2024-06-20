using UnityEngine;
using UnityEngine.Pool;

public class WeaponControl : Weapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform normalFirePos;
    [SerializeField] Transform doubleFirePos1;
    [SerializeField] Transform doubleFirePos2;
    [SerializeField] int poolCount;

    private IObjectPool<Bullet> pool;

    float angle;
    Vector3 dir, mouse;

    float delay;
    float bulletDelay;

    bool canAttack;

    Vector3 bulletPos;
    Vector3 bulletPos1;
    Vector3 bulletPos2;

    private void Awake()
    {
        pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: poolCount);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;
        count = itemManager.weaponCount;
        damageUI = ItemManager.Instance.damageUI[count];
        delay = 0;
        bulletDelay = weaponInfo.AttackDelay;
        canAttack = true;
    }

    void Update()
    {
        grade = (int)(itemManager.weaponGrade[count] + 1);

        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.y = 0;

            dir = mouse - transform.position;
            //dir.y = 0;

            LookMousePosition();
            FireBullet();
        }
    }

    void LookMousePosition()
    {
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);

        if (dir.x < 0)
            transform.rotation *= Quaternion.Euler(180, 0, 0);

        else
            transform.rotation *= Quaternion.Euler(0, 0, 0);
    }

    void FireBullet()
    {
        if (canAttack == true)
        {
            if (Input.GetMouseButton(0) && (!gameManager.isClear || !gameManager.isBossDead))
            {
                WeaponSetting();

                if (!gameManager.doubleShot)
                {
                    Bullet bullet = pool.Get();
                    bulletPos = new Vector3(normalFirePos.position.x, 0f, normalFirePos.position.z);
                    bullet.transform.position = bulletPos;
                    bullet.bulletDamage = weaponDamage;
                    bullet.Shoot(dir.normalized, bulletPos, weaponInfo.WeaponRange);
                    bullet.damageUI = damageUI;
                    bullet.speed = weaponInfo.BulletSpeed;
                }

                else if (gameManager.doubleShot)
                {
                    Bullet bullet1 = pool.Get();
                    Bullet bullet2 = pool.Get();
                    bulletPos1 = new Vector3(doubleFirePos1.position.x, 0, doubleFirePos1.position.z);
                    bulletPos2 = new Vector3(doubleFirePos2.position.x, 0, doubleFirePos2.position.z);
                    bullet1.transform.position = bulletPos1;
                    bullet2.transform.position = bulletPos2;
                    bullet1.bulletDamage = weaponDamage;
                    bullet2.bulletDamage = weaponDamage;
                    bullet1.Shoot(dir.normalized, bulletPos1, weaponInfo.WeaponRange);
                    bullet2.Shoot(dir.normalized, bulletPos2, weaponInfo.WeaponRange);
                    bullet1.damageUI = damageUI;
                    bullet2.damageUI = damageUI;
                    bullet1.speed = weaponInfo.BulletSpeed;
                    bullet2.speed = weaponInfo.BulletSpeed;
                }

                SoundManager.Instance.PlayES(weaponInfo.WeaponSound);
                canAttack = false;
            }
        }

        else if (canAttack == false)
        {
            delay += Time.deltaTime;

            if (gameManager.attackSpeed >= 0f)
            {
                if (delay >= ((bulletDelay - (grade * 0.1f)) / (1f + gameManager.attackSpeed * 0.1f)))
                {
                    canAttack = true;
                    delay = 0f;
                }
            }

            else if (gameManager.attackSpeed < 0f)
            {
                if (delay >= ((bulletDelay - (grade * 0.1f)) - gameManager.attackSpeed * 0.1f))
                {
                    canAttack = true;
                    delay = 0f;
                }
            }
        }
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, normalFirePos.position, bulletPrefab.transform.rotation).GetComponent<Bullet>();
        bullet.SetManagedPool(pool);
        bullet.transform.SetParent(gameManager.bulletStorage);
        return bullet;
    }

    private void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    private void OnDestroy()
    {
        if (pool != null)
        {
            pool.Clear();
        }
    }
}
