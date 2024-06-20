using UnityEngine;
using UnityEngine.Pool;

public class SummonLazor : MonoBehaviour
{
    [SerializeField] DamageUI damageUIPreFab;

    public float damage;

    Vector3 dir;
    float angle;

    protected IObjectPool<DamageUI> pool;

    protected GameManager gameManager;

    protected virtual void Awake()
    {
        pool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 10);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        damage = Mathf.Round(GameManager.Instance.round * 5 * (1 + GameManager.Instance.summonPDmg) * 10) * 0.1f;
    }

    private void Update()
    {
        // ÃÑ¾Ë °¢µµ
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);
    }

    public void Fire(Vector3 dir)
    {
        this.dir = dir.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Monster") && collision.collider.GetComponent<Monster>())
        {
            damage = Mathf.Round(GameManager.Instance.round * 5 * (1 + GameManager.Instance.summonPDmg) * 10) * 0.1f;

            DamageUI damageUI = pool.Get();
            damageUI.realDamage = damage;
            damageUI.isMiss = false;
            damageUI.UISetting();
            damageUI.transform.position = collision.collider.transform.position;
            damageUI.gameObject.transform.SetParent(gameManager.damageStorage);

            collision.collider.GetComponent<Monster>().PureOnDamaged(damage);
        }
    }

    private DamageUI CreateDamageUI()
    {
        DamageUI damageUIPool = Instantiate(damageUIPreFab, transform.position, damageUIPreFab.transform.rotation).GetComponent<DamageUI>();
        damageUIPool.SetManagedPool(pool);
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

    public void BulletDestroy()
    {
        Destroy(gameObject);
    }
}
