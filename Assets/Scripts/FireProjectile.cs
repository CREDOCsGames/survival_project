using UnityEngine;
using UnityEngine.Pool;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform normalFirePos;
    [SerializeField] protected int poolCount;

    protected GameManager gameManager;

    protected IObjectPool<ProjectileObjectPool> projectilePool;

    protected float coolTime;
    protected float initCoolTime = 1f;
    protected bool canFire = true;

    protected Vector3 bulletPos;

    protected Vector3 dir, mouse;

    protected ProjectileObjectPool projectile;

    protected virtual void Awake()
    {
        projectilePool = new ObjectPool<ProjectileObjectPool>(CreatePool, OnGetPool, OnReleasePool, OnDestroyPool, maxSize: poolCount);
    }

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        coolTime = initCoolTime;
    }

    protected virtual void Fire()
    {
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
        bulletPos = new Vector3(normalFirePos.position.x, 0f, normalFirePos.position.z);
        projectile.transform.position = bulletPos;
        FireDirection();
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

    protected virtual void SetInitFire()
    {
        coolTime = initCoolTime;
        canFire = true;
    }

    protected void FireDirection()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.y = 0;

        dir = mouse - transform.position;
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

    protected virtual void OnDestroy()
    {
        if (projectilePool != null)
            projectilePool.Clear();
    }
}
