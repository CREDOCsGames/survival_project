using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GamesceneManager : Singleton<GamesceneManager>
{
    [SerializeField] GameObject monsterSpawner;
    [SerializeField] public Transform treeParent;
    [SerializeField] Transform bushParent;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject bushPrefab;
    [SerializeField] GameObject campFire;
    [SerializeField] LayerMask interactionLayer;
    [SerializeField] public Collider walkableArea;
    [SerializeField] Collider treeBushSpawnArea;
    [SerializeField] GameObject nightFilter;

    [HideInInspector] public float currentGameTime;

    [HideInInspector] public bool isNight = false;

    GameManager gameManager;
    Character character;
    GameSceneUI gameSceneUI;

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        gameSceneUI = GameSceneUI.Instance;

        character.GetComponent<NavMeshAgent>().enabled = true;

        StartCoroutine(DayRoutine());
    }

    private void Update()
    {
        if (character.currentHp > 0 && currentGameTime >= 0)
            currentGameTime -= Time.deltaTime;
    }

    IEnumerator DayRoutine()
    {
        isNight = false;
        gameSceneUI.DayNightAlarmUpdate(isNight);
        nightFilter.SetActive(false);
        //monsterSpawner.SetActive(false);
        
        StartCoroutine(SpawnTree());
        StartCoroutine(SpawnBush());

        character.weaponParent.gameObject.SetActive(false); 

        currentGameTime = gameManager.gameDayTime;

        yield return new WaitForSeconds(gameManager.gameDayTime);

        campFire.GetComponent<Campfire>().ToNightScene();

        StartCoroutine(NightRoutine()); 
    }

    IEnumerator NightRoutine()
    {
        isNight = true;
        gameSceneUI.DayNightAlarmUpdate(isNight);
        nightFilter.SetActive(true);

        character.weaponParent.gameObject.SetActive(true);

        currentGameTime = gameManager.gameNightTime;

        yield return new WaitForSeconds(gameManager.gameNightTime);

        /*gameManager.fishLowGradeCount = 0;
        gameManager.fishHighGradeCount = 0;*/

        gameManager.round++;

        if (character.IsTamingPet)
            character.TamedPed.GetComponent<Pet>().RunAway();

        campFire.GetComponent<Campfire>().ToDayScene();

        if (gameManager.round <= 30)
            StartCoroutine(DayRoutine());
    }

    IEnumerator SpawnTree()
    {
        if ((gameManager.round - 1) % 5 == 0)
        {
            foreach (Transform trees in treeParent)
            {
                Destroy(trees.gameObject);
            }

            for (int i = 0; i < 25; i++)
            {
                SpawnOneBushOrTree(treePrefab, treeParent);
                yield return null;      // 보통 0.002초
            }
        }
    }

    IEnumerator SpawnBush()
    {
        foreach (Transform bushs in bushParent)
        {
            Destroy(bushs.gameObject);
        }


        for (int i = 0; i < 15; i++)
        {
            SpawnOneBushOrTree(bushPrefab, bushParent);
            yield return null;
        }
    }

    Vector3 SpawnTreeAndBushPos()
    {
        Vector3 spawnPos = TreeAndBushPos();

        while (true)
        {
            if (Physics.OverlapSphere(spawnPos, 2f, interactionLayer).Length <= 0)
            {
                break;
            }

            spawnPos = TreeAndBushPos();
        }

        return spawnPos;
    }

    void SpawnOneBushOrTree(GameObject spawnObject, Transform parent)
    {
        Instantiate(spawnObject, SpawnTreeAndBushPos(), spawnObject.transform.rotation, parent);
    }

    Vector3 TreeAndBushPos()
    {
        float groundX = treeBushSpawnArea.bounds.size.x;
        float groundZ = treeBushSpawnArea.bounds.size.z;

        groundX = Random.Range((groundX / 2f) * -1f + treeBushSpawnArea.bounds.center.x, (groundX / 2f) + treeBushSpawnArea.bounds.center.x);
        groundZ = Random.Range((groundZ / 2f) * -1f + treeBushSpawnArea.bounds.center.z, (groundZ / 2f) + treeBushSpawnArea.bounds.center.z);

        if (Mathf.Abs(groundX) < 3f)
        {
            groundX = groundX < 0f ? groundX - 3f : groundX + 3f;
        }

        if (Mathf.Abs(groundZ) < 3f)
        {
            groundZ = groundZ < 0f ? groundZ - 3f : groundZ + 3f;
        }

        return new Vector3(groundX, 0, groundZ);
    }
}