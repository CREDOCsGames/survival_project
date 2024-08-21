using UnityEngine;
using UnityEngine.Pool;

public class MonsterBullet : MonoBehaviour
{
    public float bulletDamage;

    IObjectPool<MonsterBullet> managedPool;

    GameManager gameManager;
    GamesceneManager gameSceneManager;

    float realDamage;

    [HideInInspector] public Vector3 destroyPos;

    [HideInInspector] public Transform parentObject;

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
            other.transform.parent.GetComponent<Character>().OnDamaged(realDamage, parentObject.GetComponentInChildren<MonsterHit>().gameObject);
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
