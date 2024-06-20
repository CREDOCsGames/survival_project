using UnityEngine;
using UnityEngine.Pool;

public class SummonsBullet : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected DamageUI damageUIPreFab;

    public float damage;

    protected Vector3 dir;
    protected float angle;

    protected GameManager gameManager;

    protected IObjectPool<DamageUI> pool;

    protected virtual void Awake()
    {
        pool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 10);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        Invoke("BulletDestroy", 3);
        damage = Mathf.Round(gameManager.round * 2 * (1 + gameManager.summonPDmg) * 10) * 0.1f;
    }

    private void Update()
    {
        transform.position += new Vector3(dir.x, 0, dir.z) * speed * Time.deltaTime;

        // ÃÑ¾Ë °¢µµ
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);

        if(gameManager.isClear && gameManager.isBossDead)
        {
            CancelInvoke("BulletDestroy");
        }
    }

    public void Fire(Vector3 dir)
    {
        this.dir = dir.normalized;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            damage = Mathf.Round(gameManager.round * 2 * (1 + gameManager.summonPDmg) * 10) * 0.1f;
            CancelInvoke("BulletDestroy");
            DamageUI damageUI = pool.Get();
            damageUI.realDamage = damage;
            damageUI.isMiss = false;
            damageUI.UISetting();
            damageUI.transform.position = transform.position;
            damageUI.gameObject.transform.SetParent(gameManager.damageStorage);
            other.GetComponent<Monster>().PureOnDamaged(damage);
            Destroy(gameObject);
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
