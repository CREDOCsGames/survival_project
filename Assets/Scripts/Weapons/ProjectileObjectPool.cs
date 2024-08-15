using UnityEngine;
using UnityEngine.Pool;

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField] DamageUI damageUI;
    [SerializeField] float projectileDamage;
    [SerializeField] int penetrateCount;
    [SerializeField] float fireRange;
    public bool canCri;
    bool isCri = false;

    IObjectPool<ProjectileObjectPool> pool;
    IObjectPool<DamageUI> damagePool;

    int currentPenetrateCount;

    GameManager gameManager;

    public bool isPenetrate = false;

    private void Awake()
    {
        damagePool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 20);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        currentPenetrateCount = penetrateCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && other.transform.parent.GetComponent<Monster>() != null)
        {
            Monster monster = other.transform.parent.GetComponent<Monster>();

            DamageUI damage = damagePool.Get();

            float mDef = monster.defence;

            damage.realDamage = Mathf.Clamp(projectileDamage * (1 - (mDef / (20 + mDef))), 0, projectileDamage) * gameManager.percentDamage;

            if (canCri)
            {
                isCri = gameManager.critical >= Random.Range(0f, 100f);
                damage.realDamage *= isCri ? 2 : 1;
            }

            damage.UISetting(canCri, isCri);
            damage.transform.position = monster.transform.position;

            monster.OnDamaged(damage.realDamage);

            if (!isPenetrate)
            {
                currentPenetrateCount = 1;
            }

            currentPenetrateCount--;

            if (currentPenetrateCount <= 0)
                DestroyProjectile();

        }

        else if (other.CompareTag("Obstacle"))
        {
            DestroyProjectile();
        }
    }

    public void SetManagedPool(IObjectPool<ProjectileObjectPool> pool)
    {
        this.pool = pool;
    }

    public void DestroyProjectile()
    {
        if (gameObject.activeSelf)
        {
            currentPenetrateCount = penetrateCount;
            pool.Release(this);
        }
    }

    public void SetDamage(float damage)
    {
        projectileDamage = damage;
    }

    private DamageUI CreateDamageUI()
    {
        DamageUI damageUIPool = Instantiate(damageUI, transform.position, Quaternion.Euler(90, 0, 0)).GetComponent<DamageUI>();
        damageUIPool.SetManagedPool(damagePool);
        damageUIPool.transform.SetParent(gameManager.damageStorage);
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
        if (damageUIPool.gameObject != null)
        {
            Destroy(damageUIPool.gameObject);
        }
    }
}
