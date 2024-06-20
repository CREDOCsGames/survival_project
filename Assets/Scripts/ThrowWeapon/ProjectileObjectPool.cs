using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField] protected DamageUI damageUI;
    [SerializeField] float projectileDamage;
    //public GameObject effectPrefab;

    protected IObjectPool<ProjectileObjectPool> pool;
    protected IObjectPool<DamageUI> damagePool;

    GameManager gameManager;

    private void Awake()
    {
        damagePool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 20);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Monster") && collision.collider.GetComponent<Monster>() != null)
        {
            Monster monster = collision.collider.GetComponent<Monster>();

            //Instantiate(effectPrefab, transform.position, transform.rotation);

            DamageUI damage = damagePool.Get();

            float mDef = monster.defence;
            damage.realDamage = Mathf.Clamp(projectileDamage * (1 - (mDef / (20 + mDef))), 0, projectileDamage);
            damage.UISetting();
            damage.transform.position = transform.position;
            damage.gameObject.transform.SetParent(gameManager.damageStorage);

            monster.OnDamaged(damage.realDamage);

            DestroyProjectile();
        }
    }

    public void SetManagedPool(IObjectPool<ProjectileObjectPool> pool)
    {
        this.pool = pool;
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
}
