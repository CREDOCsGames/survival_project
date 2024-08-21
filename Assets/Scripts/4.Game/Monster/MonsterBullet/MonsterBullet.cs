using UnityEngine;
using UnityEngine.Pool;

public class MonsterBullet : MonoBehaviour
{
    public float bulletDamage;

    protected IObjectPool<MonsterBullet> managedPool;

    protected GameManager gameManager;
    protected GamesceneManager gameSceneManager;

    protected float realDamage;

    [HideInInspector] public Vector3 destroyPos;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameSceneManager = GamesceneManager.Instance;
    }

    void Update()
    {
        if (!gameSceneManager.isNight || transform.position == destroyPos)
        {
            DestroyBullet();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;
            other.transform.parent.GetComponent<Character>().OnDamaged(realDamage, transform.parent.transform.GetComponentInChildren<MonsterHit>().gameObject);
            DestroyBullet();
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
}
