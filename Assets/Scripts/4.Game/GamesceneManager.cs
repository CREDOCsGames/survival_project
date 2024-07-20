using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GamesceneManager : Singleton<GamesceneManager>
{
    [SerializeField] public Transform treeParent;
    [SerializeField] Transform bushParent;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject bushPrefab;
    [SerializeField] GameObject campFire;
    [SerializeField] LayerMask interactionLayer;
    [SerializeField] public Collider ground;
    [SerializeField] GameObject nightFilter;

    [HideInInspector] public float currentGameTime;

    [HideInInspector] public bool isNight = false;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        StartCoroutine(DayRoutine());
    }

    private void Update()
    {
        if (Character.Instance.currentHp > 0 && currentGameTime >= 0)
            currentGameTime -= Time.deltaTime;
    }

    IEnumerator DayRoutine()
    {
        isNight = false;
        nightFilter.SetActive(false);
        
        StartCoroutine(SpawnTree());
        StartCoroutine(SpawnBush());

        campFire.GetComponent<Campfire>().ToDayScene();

        currentGameTime = gameManager.gameDayTime;

        yield return new WaitForSeconds(gameManager.gameDayTime);

        StartCoroutine(NightRoutine()); 
    }

    IEnumerator NightRoutine()
    {
        isNight = true;
        nightFilter.SetActive(true);

        campFire.GetComponent<Campfire>().OnDebuff();
        campFire.GetComponent<Campfire>().ToNightScene();

        currentGameTime = gameManager.gameNightTime;

        yield return new WaitForSeconds(gameManager.gameNightTime);

        /*gameManager.fishLowGradeCount = 0;
        gameManager.fishHighGradeCount = 0;*/

        gameManager.round++;

        campFire.GetComponent <Campfire>().OffBuffNDebuff();

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
        float groundX = ground.bounds.size.x;
        float groundZ = ground.bounds.size.z;

        groundX = Random.Range((groundX / 2f) * -1f + ground.bounds.center.x, (groundX / 2f) + ground.bounds.center.x);
        groundZ = Random.Range((groundZ / 2f) * -1f + ground.bounds.center.z, (groundZ / 2f) + ground.bounds.center.z);

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
