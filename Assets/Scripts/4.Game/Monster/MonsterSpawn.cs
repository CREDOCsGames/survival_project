using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] Collider[] spawnPoses;
    [SerializeField] GameObject[] normalMonsterPrefab;
    [SerializeField] GameObject[] bossMonsterPrefab;
    [SerializeField] Transform storageParent;
    [SerializeField] Transform bosssParent;
    [SerializeField] GameObject spawnImage;
    [SerializeField] GameObject bossSpawnImage;
    [SerializeField] int poolCount;
    [SerializeField] int spawnRange;
    [SerializeField] float spawnDelay;
    Collider ground;

    private IObjectPool<Monster> pool;

    float[] weightValue;
    float totalWeight = 0;

    GameManager gameManager;
    GamesceneManager gamesceneManager;

    Coroutine currentCoroutine;

    private void Awake()
    {
        pool = new ObjectPool<Monster>(CreateMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: poolCount);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;

        weightValue = new float[] { 100, 0, 0, 0 };
        //ground = GamesceneManager.Instance.walkableArea;

        for (int i = 0; i < weightValue.Length; i++)
        {
            totalWeight += weightValue[i];
        }

        StartCoroutine(UpdateSpawn());
    }

    private void Update()
    {
        if (bosssParent.transform.childCount == 0)
            gameManager.isBossDead = true;
    }

    IEnumerator UpdateSpawn()
    {
        while (true)
        {
            //yield return new WaitUntil(() => gamesceneManager.isNight);
            yield return CoroutineCaching.WaitWhile(() => gamesceneManager.isNight);

            currentCoroutine = StartCoroutine(RendSpawnImage(5));

            //yield return new WaitUntil(() => !gamesceneManager.isNight);
            yield return CoroutineCaching.WaitWhile(() => !gamesceneManager.isNight);

            StopCoroutine(currentCoroutine);
        }
    }

    IEnumerator RendSpawnImage(float time)
    {
        while (gamesceneManager.isNight)
        {
            Vector3 pos = SpawnPosition();
            GameObject spawnMark = Instantiate(spawnImage, pos, spawnImage.transform.rotation, storageParent);
            Destroy(spawnMark, 1f);
            StartCoroutine(SpawnMonster(pos, Color.red));

            SpawnSubordinateMonster(pos, Random.Range(4, 7));

            //yield return new WaitForSeconds(time);
            yield return CoroutineCaching.WaitForSeconds(time);
        }
    }

    Vector3 SpawnPosition()
    {
        int rand = Random.Range(0, spawnPoses.Length);
        Collider spawnTransform = spawnPoses[rand];

        float randX = spawnTransform.bounds.size.x;
        float randZ = spawnTransform.bounds.size.z;

        randX = Random.Range(-(randX / 2), (randX / 2));
        randZ = Random.Range(-(randZ / 2), (randZ / 2));

        Vector3 spawnPos = spawnTransform.transform.position + new Vector3(randX, 0, randZ);

        return spawnPos;
    }

    IEnumerator SpawnMonster(Vector3 pos, Color color)
    {
        yield return new WaitForSeconds(1);

        Monster monster = pool.Get();
        monster.ChangeOutline(color);

        NavMeshHit hit;

        if (NavMesh.SamplePosition(pos, out hit, 500, NavMesh.AllAreas))
        {
            monster.transform.position = hit.position;
        }

        if (monster.GetComponent<NavMeshAgent>())
        {
            monster.GetComponent<NavMeshAgent>().enabled = true;

            if (!monster.GetComponent<NavMeshAgent>().isOnNavMesh)
            {
                Debug.Log("not");
                Debug.Break();
            }
        }
    }


    void SpawnSubordinateMonster(Vector3 pos, int count)
    {
        Vector3 spawnPos;

        for (int i = 0; i < count; i++)
        {
            spawnPos = pos + Random.onUnitSphere * 2;
            spawnPos.y = 0;

            GameObject spawnMark = Instantiate(spawnImage, spawnPos, spawnImage.transform.rotation, storageParent);
            Destroy(spawnMark, 1f);
            StartCoroutine(SpawnMonster(spawnPos, Color.white));
        }
    }

    void RendBossSpawnImage(int round)
    {
        if (round != 30)
        {
            GameObject spawnMark = Instantiate(bossSpawnImage, new Vector3(3, 0, -37), bossSpawnImage.transform.rotation, bosssParent);
            Destroy(spawnMark, 2.1f);
            StartCoroutine(CreateBossMonster(round));
        }

        else if (round == 30)
        {
            GameObject spawnMark = Instantiate(bossSpawnImage, new Vector3(3, 0, -37), bossSpawnImage.transform.rotation, bosssParent);
            Destroy(spawnMark, 2.1f);
            GameObject spawnMark2 = Instantiate(bossSpawnImage, new Vector3(-3, 0, -37), bossSpawnImage.transform.rotation, bosssParent);
            Destroy(spawnMark2, 2.1f);
            StartCoroutine(CreateBossMonster(round));
        }
    }

    private Monster CreateMonster()
    {
        //int num = RandomMonster();
        int num = 0;
        Monster monster = Instantiate(normalMonsterPrefab[num]).GetComponent<Monster>();
        monster.stat = MonsterInfo.Instance.monsterInfos[num];
        monster.SetManagedPool(pool);
        monster.transform.SetParent(storageParent);

        return monster;
    }

    private IEnumerator CreateBossMonster(int round)
    {
        yield return new WaitForSeconds(2);

        if (round != 30)
        {
            GameObject inst = Instantiate(bossMonsterPrefab[(round / 10) - 1]);
            Monster monster = inst.GetComponent<Monster>();
            monster.stat = MonsterInfo.Instance.monsterInfos[normalMonsterPrefab.Length + ((round / 10) - 1)];
            monster.transform.position = new Vector3(3, 0, -37);
            monster.transform.SetParent(bosssParent);
            monster.GetComponent<NavMeshAgent>().enabled = true;
        }

        else if (round == 30)
        {
            GameObject inst1 = Instantiate(bossMonsterPrefab[0]);
            GameObject inst2 = Instantiate(bossMonsterPrefab[1]);
            Monster monster1 = inst1.GetComponent<Monster>();
            Monster monster2 = inst2.GetComponent<Monster>();
            monster1.stat = MonsterInfo.Instance.monsterInfos[normalMonsterPrefab.Length];
            monster1.transform.position = new Vector3(3, 0, -37);
            monster2.stat = MonsterInfo.Instance.monsterInfos[normalMonsterPrefab.Length + 1];
            monster2.transform.position = new Vector3(-3, 0, -37);
            monster2.transform.SetParent(bosssParent);
            monster1.transform.SetParent(bosssParent);
        }
    }

    int RandomMonster()
    {
        weightValue[0] = Mathf.Clamp(100 - (gameManager.round * 4f), 10, 100);
        weightValue[1] = ((gameManager.round - 6) * 5f) * 0.3f;
        weightValue[2] = ((gameManager.round - 13) * 5f) * 0.3f;
        weightValue[3] = ((gameManager.round - 20) * 10f) * 0.5f;

        float rand = Random.Range(0, totalWeight);
        int spawnNum = 0;
        float total = 0;

        for (int i = 0; i < normalMonsterPrefab.Length; i++)
        {
            total += weightValue[i];
            if (rand < total)
            {
                spawnNum = i;
                break;
            }
        }

        return spawnNum;
    }

    private void OnGetMonster(Monster monster)
    {
        monster.gameObject.SetActive(true);
    }

    private void OnReleaseMonster(Monster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void OnDestroyMonster(Monster monster)
    {
        Destroy(monster.gameObject);
    }
}
