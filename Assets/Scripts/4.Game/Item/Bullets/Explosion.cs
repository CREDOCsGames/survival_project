using UnityEngine;
using UnityEngine.Pool;

public class Explosion : MonoBehaviour
{
    [SerializeField] float exDamage;
    [SerializeField] ExDamageUI damageUI;

    protected IObjectPool<Explosion> managedPool;
    protected IObjectPool<ExDamageUI> pool;

    public int grade;

    GameManager gameManager;

    private void Awake()
    {
        pool = new ObjectPool<ExDamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 10);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void Setting()
    {
        SoundManager.Instance.PlayES("Explosion");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Monster")
        {
            ExDamageUI damagePool = pool.Get();
            exDamage = exDamage + gameManager.exDmg;
            damagePool.damage = exDamage * grade;
            damagePool.damageSetting();
            damagePool.transform.SetParent(GameManager.Instance.damageStorage);
            collision.collider.GetComponent<Monster>().PureOnDamaged(exDamage * grade);
        }
    }

    private ExDamageUI CreateDamageUI()
    {
        ExDamageUI damageUIPool = Instantiate(damageUI, transform.position, Quaternion.Euler(90, 0, 0)).GetComponent<ExDamageUI>();
        damageUIPool.SetManagedPool(pool);
        damageUIPool.transform.SetParent(gameManager.bulletStorage);
        return damageUIPool;
    }

    private void OnGetDamageUI(ExDamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(true);
    }

    private void OnReleaseDamageUI(ExDamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(false);
    }

    private void OnDestroyDamageUI(ExDamageUI damageUIPool)
    {
        Destroy(damageUIPool.gameObject);
    }

    public void SetManagedPool(IObjectPool<Explosion> pool)
    {
        managedPool = pool;
    }

    public virtual void DestroyEx()
    {
        if (gameObject.activeSelf)
        {
            managedPool.Release(this);
        }
    }

    private void OnDestroy()
    {
        if (pool != null)
            pool.Clear();
    }
}
