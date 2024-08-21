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
    [SerializeField] GameObject cardSelecter;
    [SerializeField] GameObject multicellInvenPanel;

    [HideInInspector] public float currentGameTime;

    [HideInInspector] public bool isNight = false;

    bool isCardSetting = true;

    GameManager gameManager;
    Character character;
    GameSceneUI gameSceneUI;
    ItemManager itemManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        gameSceneUI = GameSceneUI.Instance;
        itemManager = ItemManager.Instance;

        character.GetComponent<NavMeshAgent>().enabled = true;

        itemManager.pieceItemsList = itemManager.startPieceList;

        currentGameTime = gameManager.gameDayTime;
        StartCoroutine(DayRoutine());
    }

    private void Update()
    {
        if (character.currentHp > 0 && currentGameTime > 0 && !gameManager.isPause && !isCardSetting)
        {
            currentGameTime -= Time.deltaTime;
        }
    }

    IEnumerator DayRoutine()
    {
#if UNITY_EDITOR

#else
        gameManager.dashCount = 0;
#endif
        gameManager.bloodDamage = 0;
        gameSceneUI.CursorChange(CursorType.Normal);
        isNight = false;
        nightFilter.SetActive(false);
        character.ChangeAnimationController(0);

        character.UpdateStat();
        character.weaponParent.gameObject.SetActive(false);

        if (gameManager.round % 5 == 0)
        {
            StartCoroutine(SpawnTree());
        }

        StartCoroutine(SpawnBush());

        cardSelecter.SetActive(true);

        isCardSetting = true;
        character.isCanControll = false;
        //yield return new WaitWhile(() => cardSelecter.activeSelf);
        yield return CoroutineCaching.WaitWhile(() => cardSelecter.activeSelf);

        gameManager.round++;
        currentGameTime = gameManager.gameDayTime;

        if (gameManager.round > 1)
            multicellInvenPanel.SetActive(true);

        character.canWeaponChange = true;

        for (int i = 0; i < character.weaponParent.childCount; ++i)
        {
            if (!character.weaponParent.GetChild(i).gameObject.activeSelf)
            {
                character.weaponParent.GetChild(i).gameObject.SetActive(true);
                character.weaponParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        //yield return new WaitWhile(() => multicellInvenPanel.activeSelf);
        yield return CoroutineCaching.WaitWhile(() => multicellInvenPanel.activeSelf);

        gameSceneUI.ChangeDayText(0, "아침이 밝았습니다.");

        isCardSetting = false;
        character.isCanControll = true;

        //yield return new WaitForSeconds(gameManager.gameDayTime);
        yield return CoroutineCaching.WaitForSeconds(gameManager.gameDayTime);

        StartCoroutine(NightRoutine()); 
    }

    IEnumerator NightRoutine()
    {
        gameSceneUI.ChangeDayText(1, "밤이 되었습니다.");
        gameSceneUI.CursorChange(CursorType.Attack);
        isNight = true;
        nightFilter.SetActive(true);

        character.UpdateStat();
        character.weaponParent.gameObject.SetActive(true);
        character.transform.position = new Vector3(0f, 0f, -40f);

        campFire.GetComponent<Campfire>().ToNightScene();

        gameManager.dashCount = 10;
        character.dashCount = gameManager.dashCount;

        currentGameTime = gameManager.gameNightTime;

        //yield return new WaitForSeconds(gameManager.gameNightTime);
        yield return CoroutineCaching.WaitForSeconds(gameManager.gameNightTime);

        gameManager.fishLowGradeCount = 0;
        gameManager.fishHighGradeCount = 0;

        if (character.IsTamingPet)
            character.TamedPed.GetComponent<Pet>().RunAway();

        campFire.GetComponent<Campfire>().ToDayScene();

        itemManager.pieceItemsList = itemManager.nightPieceList;

        if (gameManager.round < 30)
            StartCoroutine(DayRoutine());
    }

    IEnumerator SpawnTree()
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
