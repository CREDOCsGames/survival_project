using UnityEngine;
using UnityEngine.Pool;

public class DropCoin : Singleton<DropCoin>
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int poolCount;

    private IObjectPool<Coin> objectPool;

    protected override void Awake()
    {
        base.Awake();
        objectPool = new ObjectPool<Coin>(CreatePool, OnGetPool, OnReleasePool, OnDestroyPool, maxSize: poolCount);
    }

    public void Drop(Vector3 dropPos, int coinValue)
    {
        Coin coin = objectPool.Get();
        coin.coinValue = coinValue;
        coin.transform.position = dropPos;
    }

    private Coin CreatePool()
    {
        Coin pool = Instantiate(coinPrefab).GetComponent<Coin>();
        pool.SetManagedPool(objectPool);
        pool.transform.SetParent(ItemManager.Instance.coinStorage);

        return pool;
    }

    private void OnGetPool(Coin pool)
    {
        pool.gameObject.SetActive(true);
    }

    private void OnReleasePool(Coin pool)
    {
        pool.gameObject.SetActive(false);
    }

    private void OnDestroyPool(Coin pool)
    {
        Destroy(pool.gameObject);
    }
}
