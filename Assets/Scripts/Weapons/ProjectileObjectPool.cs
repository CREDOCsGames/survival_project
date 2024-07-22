using UnityEngine;
using UnityEngine.Pool;

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField] protected DamageUI damageUI;
    [SerializeField] float projectileDamage;

    protected IObjectPool<ProjectileObjectPool> pool;
    protected IObjectPool<DamageUI> damagePool;


    GameManager gameManager;

    [HideInInspector] public bool isPenetrate = false;

    private void Awake()
    {
        damagePool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 20);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && other.transform.parent.GetComponent<Monster>() != null)
        {
            Monster monster = other.transform.parent.GetComponent<Monster>();

            DamageUI damage = damagePool.Get();

            float mDef = monster.defence;
            damage.realDamage = Mathf.Clamp(projectileDamage * (1 - (mDef / (20 + mDef))), 0, projectileDamage);
            damage.UISetting();
            damage.transform.position = transform.position;
            damage.gameObject.transform.SetParent(gameManager.damageStorage);

            monster.OnDamaged(damage.realDamage);

            if (!isPenetrate)
                DestroyProjectile();
        }

        else if(other.CompareTag("Obstacle"))
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
        if (damageUIPool.gameObject != null)
        {
            Destroy(damageUIPool.gameObject);
        }
    }
}
