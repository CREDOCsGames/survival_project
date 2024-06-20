using UnityEngine;
using UnityEngine.Pool;

public class SwordControl : Weapon
{
    [SerializeField] GameObject swordBullet;
    [SerializeField] Transform firePos;
    [SerializeField] Transform doubleFirePos1;
    [SerializeField] Transform doubleFirePos2;
    [SerializeField] int poolCount;

    private IObjectPool<Bullet> pool;
    protected IObjectPool<DamageUI> damagePool;

    Animator anim;

    Vector3 dir, mouse;

    float delay;
    float swordDelay;
    float attackRange;

    bool canAttack;

    Character character;

    float addRange;

    float angle;

    bool isSwing = false;

    int x, z;
    bool xInput;
    bool zInput;

    Vector3 beforeDir;
    Vector3 bulletDir;

    bool isAttack;

    Vector3 bulletPos;
    Vector3 bulletPos1;
    Vector3 bulletPos2;

    private void Awake()
    {
        pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: poolCount);
        damagePool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: poolCount);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        character = Character.Instance;
        itemManager = ItemManager.Instance;
        count = itemManager.weaponCount;
        damageUI = itemManager.damageUI[count];
        delay = 0;
        swordDelay = weaponInfo.AttackDelay;
        attackRange = weaponInfo.WeaponRange;
        canAttack = true;
        anim.enabled = false;
        isAttack = false;
    }

    void Update()
    {
        grade = (int)(itemManager.weaponGrade[count] + 1);

        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.y = transform.position.y;

            if (!isSwing)
            {
                LookKeyBoardPos();
                Attack();
            }
        }

    }

    void LookKeyBoardPos()
    {
        xInput = (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Left"))) || (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")));
        zInput = (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Up"))) || (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")));

        if (xInput || zInput)
        {
            dir = character.dir;
            beforeDir = dir;
        }

        else if (!xInput && !zInput)
        {
            dir = beforeDir;
        }

        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, -90);
    }

    void Attack()
    {
        if (gameManager.range > 0)
            addRange = (attackRange + gameManager.range * 0.05f);

        else if (gameManager.range <= 0)
            addRange = attackRange;

        Vector3 range = (mouse - transform.position).normalized * addRange;
        range.y = 0;

        if (canAttack == true)
        {
            if (Input.GetMouseButton(0) && (!gameManager.isClear || !gameManager.isBossDead))
            {
                isSwing = true;
                anim.enabled = true;
                criRand = UnityEngine.Random.Range(1, 101);

                if ((mouse.x - transform.position.x) > 0)
                {
                    transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
                }

                else if ((mouse.x - transform.position.x) <= 0)
                {
                    transform.parent.localRotation = Quaternion.Euler(0, -180, 0);
                }

                anim.SetTrigger("RightAttack");

                transform.position = Vector3.MoveTowards(transform.position, transform.position + range, 2);
                //bulletDir = transform.position - character.transform.position + range;
                bulletDir = mouse - transform.position;
                bulletDir.y = 0;

                SoundManager.Instance.PlayES(weaponInfo.WeaponSound);

                if (character.characterNum == (int)CHARACTER_NUM.Legendary)
                {
                    if (character.currentHp / character.maxHp > 0.7)
                    {
                        if (!gameManager.doubleShot)
                        {
                            criRand = UnityEngine.Random.Range(1, 101);
                            int bulletCri = criRand;
                            WeaponSetting();
                            Bullet bullet = pool.Get();
                            bulletPos = new Vector3(firePos.position.x, 0f, firePos.position.z);
                            bullet.transform.position = bulletPos;
                            bulletDir = mouse - bullet.transform.position;
                            bulletDir.y = 0;
                            bullet.bulletDamage = swordBulletDamage;
                            bullet.GetComponent<SwordBullet>().criRand = bulletCri;
                            bullet.damageUI = damageUI;
                            bullet.speed = 9f;
                            bullet.Shoot(bulletDir.normalized, bulletPos, 5f);
                        }

                        else if(gameManager.doubleShot)
                        {
                            criRand = UnityEngine.Random.Range(1, 101);
                            int bulletCri = criRand;
                            WeaponSetting();
                            Bullet bullet = pool.Get();
                            bulletPos1 = new Vector3(doubleFirePos1.position.x, 0f, doubleFirePos1.position.z);
                            bullet.transform.position = bulletPos1;
                            bulletDir = mouse - bullet.transform.position;
                            bulletDir.y = 0;
                            bullet.bulletDamage = swordBulletDamage;
                            bullet.GetComponent<SwordBullet>().criRand = bulletCri;
                            bullet.damageUI = damageUI;
                            bullet.speed = 9f;
                            bullet.Shoot(bulletDir.normalized, bulletPos1, 5f);

                            criRand = UnityEngine.Random.Range(1, 101);
                            bulletCri = criRand;
                            WeaponSetting();
                            Bullet bullet2 = pool.Get();
                            bulletPos2 = new Vector3(doubleFirePos2.position.x, 0f, doubleFirePos2.position.z);
                            bullet2.transform.position = bulletPos2;
                            bulletDir = mouse - bullet2.transform.position;
                            bulletDir.y = 0;
                            bullet2.bulletDamage = swordBulletDamage;
                            bullet2.GetComponent<SwordBullet>().criRand = bulletCri;
                            bullet2.damageUI = damageUI;
                            bullet2.speed = 9f;
                            bullet2.Shoot(bulletDir.normalized, bulletPos2, 5f);
                        }
                    }
                }

                canAttack = false;
            }
        }

        if(canAttack == false)
        {
            delay += Time.deltaTime;

            if (gameManager.attackSpeed >= 0f)
            {
                if (delay >= ((swordDelay - (grade * 0.1f)) / (1f + gameManager.attackSpeed * 0.1f)))
                {
                    canAttack = true;
                    delay = 0f;
                }
            }

            else if (gameManager.attackSpeed < 0f)
            {
                if (delay >= ((swordDelay - (grade * 0.1f)) - gameManager.attackSpeed * 0.1f))
                {
                    canAttack = true;
                    delay = 0f;
                }
            }
        }
    }

    public void EndSwing()
    {
        transform.position = Vector3.MoveTowards(transform.position, character.weaponPoses[count].position, 5);
        transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
        isSwing = false;
        anim.enabled = false;
        isAttack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster") && other.GetComponent<Monster>() != null)
        {
            Monster monster = other.GetComponent<Monster>();

            criRand = UnityEngine.Random.Range(1, 101);

            WeaponSetting();

            DamageUI damage = damagePool.Get();

            if (weaponDamage > 0)
                damage.isMiss = false;

            else if (weaponDamage <= 0)
                damage.isMiss = true;

            float mDef = monster.defence;
            damage.realDamage = Mathf.Clamp(weaponDamage * (1 - (mDef / (20 + mDef))), 0, weaponDamage);

            if (criRand <= gameManager.critical || gameManager.critical >= 100)
            {
                damage.damageText.color = new Color(0.9f, 0, 0.7f, 1);
                damage.damageText.fontSize = 65;
            }

            else if (criRand > gameManager.critical)
            {
                damage.damageText.color = new Color(1, 0.4871f, 0);
                damage.damageText.fontSize = 50;
            }

            damage.UISetting();
            damage.transform.position = transform.position;
            damage.gameObject.transform.SetParent(gameManager.damageStorage);

            other.GetComponent<Monster>().OnDamaged(damage.realDamage);

            if (gameManager.absorbHp > 0 && !damage.isMiss && !isAttack && isSwing)
            {
                character.currentHp += Mathf.Clamp(gameManager.absorbHp, 0f, gameManager.maxAbs);
                isAttack = true;
            }
        }
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(swordBullet, firePos.position, swordBullet.transform.rotation).GetComponent<Bullet>();
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

    private DamageUI CreateDamageUI()
    {
        DamageUI damageUIPool = Instantiate(damageUI, transform.position, Quaternion.Euler(90, 0, 0)).GetComponent<DamageUI>();
        damageUIPool.SetManagedPool(damagePool);
        damageUIPool.transform.SetParent(gameManager.bulletStorage);
        return damageUIPool;
    }

    private void OnGetDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(true);
    }

    private void OnReleaseDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(false);
    }

    private void OnDestroyDamageUI(DamageUI damageUIPool)
    {
        Destroy(damageUIPool.gameObject);
    }

    private void OnDestroy()
    {
        if (pool != null)
            pool.Clear();

        if (damagePool != null)
            damagePool.Clear();
    }
}
