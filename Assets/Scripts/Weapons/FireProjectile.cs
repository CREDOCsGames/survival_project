using UnityEngine;
using UnityEngine.Pool;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform normalFirePos;
    [SerializeField] protected int poolCount;
    [SerializeField] protected float initCoolTime = 1f;

    protected GameManager gameManager;

    protected IObjectPool<ProjectileObjectPool> projectilePool;

    protected float coolTime;
    protected bool canFire = true;

    protected Vector3 dir, mouse;

    protected ProjectileObjectPool projectile;

    protected Character character;

    protected virtual void Awake()
    {
        projectilePool = new ObjectPool<ProjectileObjectPool>(CreatePool, OnGetPool, OnReleasePool, OnDestroyPool, maxSize: poolCount);
    }

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        coolTime = initCoolTime;
    }

    protected virtual void Fire()
    {
        if(Input.GetMouseButton(0))
        {
            character.canWeaponChange = false;
        }

        if (Input.GetMouseButtonUp(0) && !(gameManager.isClear || gameManager.isPause))
        {
            SetFire();
        }
    }

    protected virtual void SetFire()
    {
        SettingProjectile();
        canFire = false;
    }

    protected virtual void SettingProjectile()
    {
        projectile = projectilePool.Get();
        projectile.transform.position = new Vector3(normalFirePos.position.x, 0f, normalFirePos.position.z);
        FireDirection();
    }

    protected void FireDirection()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.y = 0;

        dir = mouse - transform.position;
    }

    protected virtual void SetInitFire()
    {
        coolTime = initCoolTime * character.attackSpeed;
        canFire = true;
        character.canWeaponChange = true;
    }

    protected virtual void FireAppliedCoolTime()
    {
        if (canFire)
            Fire();

        else
            coolTime -= Time.deltaTime;

        if (coolTime <= 0)
            SetInitFire();
    }

    protected virtual ProjectileObjectPool CreatePool()
    {
        ProjectileObjectPool pool = Instantiate(projectilePrefab, normalFirePos.position, projectilePrefab.transform.rotation).GetComponent<ProjectileObjectPool>();
        pool.SetManagedPool(projectilePool);
        pool.transform.SetParent(gameManager.bulletStorage);
        return pool;
    }

    protected virtual void OnGetPool(ProjectileObjectPool bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    protected virtual void OnReleasePool(ProjectileObjectPool bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    protected virtual void OnDestroyPool(ProjectileObjectPool bullet)
    {
        Destroy(bullet.gameObject);
    }
}
