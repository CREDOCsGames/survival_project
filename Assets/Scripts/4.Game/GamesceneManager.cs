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
    [SerializeField] public GameObject cardSelecter;

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

        cardSelecter.SetActive(true);

        StartCoroutine(DayRoutine());
    }

    private void Update()
    {
        if (character.currentHp > 0 && currentGameTime >= 0 && !gameManager.isPause)
            currentGameTime -= Time.deltaTime;
    }

    IEnumerator DayRoutine()
    {
        character.weaponParent.gameObject.SetActive(false);
        currentGameTime = gameManager.gameDayTime;

        StartCoroutine(SpawnTree());
        StartCoroutine(SpawnBush());

        gameManager.isPause = true;
        yield return new WaitWhile(() => cardSelecter.activeSelf);
        gameManager.isPause = false;

        isNight = false;
        gameSceneUI.DayNightAlarmUpdate(isNight);
        nightFilter.SetActive(false);

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
        if (gameManager.round % 5 == 1)
        {
            foreach (Transform trees in treeParent)
            {
                Destroy(trees.gameObject);
            }

            for (int i = 0; i < 25; i++)
            {
                SpawnOneBushOrTree(treePrefab, treeParent);
                yield return null;      // 약 0.002초
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

        return new Vector3(groundX, 0, groundZ);
    }
}
