using UnityEngine;
using UnityEngine.Pool;

public class MonsterBullet : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected float bulletDamage;

    protected IObjectPool<MonsterBullet> managedPool;

    protected Vector3 dir;

    [HideInInspector] public int randNum;
    [HideInInspector] public Vector3 monsPos;

    protected GameManager gameManager;

    protected float realDamage;

    private void Start()
    {
        gameManager = GameManager.Instance;
        realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;  // 트리거에도 있음
    }

    void Update()
    {
        if (gameManager.isClear && gameManager.isBossDead)
        {
            CancelInvoke("DestroyBullet");
            DestroyBullet();
        }

        transform.position += dir * speed * Time.deltaTime;
    }

    public virtual void ShootDir()
    {
        dir = (Character.Instance.transform.position - transform.position).normalized;
        dir = new Vector3(dir.x, 0f, dir.z);
    }

    public virtual void AutoDestroyBullet()
    {
        Invoke("DestroyBullet", 2f);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;
            other.GetComponent<Character>().OnDamaged(realDamage);
            DestroyBullet();
            CancelInvoke("DestroyBullet");
        }
    }

    public void SetManagedPool(IObjectPool<MonsterBullet> pool)
    {
        managedPool = pool;
    }

    public virtual void DestroyBullet()
    {
        if (gameObject.activeSelf)
        {
            managedPool.Release(this);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke("DestroyBullet");
    }
}
