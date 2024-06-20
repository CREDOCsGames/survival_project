using UnityEngine;
using UnityEngine.Pool;

public class Fire : Bullet
{
    [SerializeField] public GameObject explosion;

    protected IObjectPool<Explosion> exPool;

    public int grade;

    bool isInstEx = false;
    Vector3 exInitScale;
    int exCount = 0;

    protected override void Awake()
    {
        base.Awake();
        exPool = new ObjectPool<Explosion>(CreateEx, OnGetEx, OnReleaseEx, OnDestroyEx, maxSize: 10);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        exInitScale = explosion.transform.localScale;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, initPos) > range)
        {
            DestroyBullet();
        }

        transform.position += dir * speed * Time.deltaTime;

        // ÃÑ¾Ë °¢µµ
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Monster") && collision.collider.GetComponent<Monster>() != null)
        {
            if (!isInstEx)
            {
                Explosion();
                isInstEx = true;
            }

            if (gameManager.isReflect || gameManager.lowPenetrate || gameManager.onePenetrate || gameManager.penetrate)
            {
                isInstEx = false;
            }
        }

        base.OnCollisionEnter(collision);
    }

    void Explosion()
    {
        float rand = Random.Range(0f, 100f);
        if (rand <= 10f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.3f)
        {
            Explosion ex = exPool.Get();
            ex.grade = grade;
            ex.transform.localScale = exInitScale * Mathf.Clamp(100 - exCount * 20, 20, 100) * 0.01f;
            ex.Setting();
            exCount++;
        }
    }

    private Explosion CreateEx()
    {
        Explosion explosionPool = Instantiate(explosion, transform.position, transform.rotation).GetComponent<Explosion>();
        explosionPool.SetManagedPool(exPool);
        explosionPool.transform.SetParent(gameManager.bulletStorage);
        return explosionPool;
    }

    private void OnGetEx(Explosion exPool)
    {
        exPool.gameObject.SetActive(true);
    }

    private void OnReleaseEx(Explosion exPool)
    {
        exPool.gameObject.SetActive(false);
    }

    private void OnDestroyEx(Explosion exPool)
    {
        Destroy(exPool.gameObject);
    }

    public override void DestroyBullet()
    {
        exCount = 0;
        isInstEx = false;
        base.DestroyBullet();
    }

    protected override void OnDestroy()
    {
        isInstEx = false;
        base.OnDestroy();
        if (exPool != null)
            exPool.Clear();
    }
}
