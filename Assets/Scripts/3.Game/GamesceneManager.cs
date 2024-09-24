using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GamesceneManager : Singleton<GamesceneManager>
{
    [SerializeField] public GameObject monsterSpawner;
    [SerializeField] public Transform treeParent;
    [SerializeField] Transform bushParent;
    [SerializeField] Transform beachParent;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject bushPrefab;
    [SerializeField] GameObject campFire;
    [SerializeField] LayerMask interactionLayer;
    [SerializeField] public Collider walkableArea;
    [SerializeField] Collider treeBushSpawnArea;
    [SerializeField] GameObject nightFilter;
    [SerializeField] GameObject cardSelecter;
    [SerializeField] GameObject multicellInvenPanel;
    [SerializeField] GameObject initSpawnPos;
    [SerializeField] Sprite weaponChangeTutoImage;
    [SerializeField] Sprite multicellTutoImage;

    [HideInInspector] public float currentGameTime;

    [HideInInspector] public bool isNight = false;

    bool isSetPieceEnd = true;

    GameManager gameManager;
    Character character;
    GameSceneUI gameSceneUI;
    ItemManager itemManager;
    ItemSpawner beach;
    SoundManager soundManager;

    bool canCharacterContact = false;

    public bool CanCharacterContact => canCharacterContact;

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        gameSceneUI = GameSceneUI.Instance;
        itemManager = ItemManager.Instance;
        beach = ItemSpawner.Instance;
        soundManager = SoundManager.Instance;

        character.GetComponent<NavMeshAgent>().enabled = true;

        itemManager.pieceItemsList = itemManager.startPieceList;

        currentGameTime = gameManager.gameDayTime;

        soundManager.StopBGM();

        nightFilter.SetActive(false);

        StartCoroutine(DayRoutine());
    }

    private void Update()
    {
        if (character.currentHp > 0 && currentGameTime > 0 && !gameManager.isPause && isSetPieceEnd)
        {
            currentGameTime -= Time.deltaTime;

            if (currentGameTime < 0)
                currentGameTime = 0;
        }

        else if(character.currentHp <= 0)
        {
            StopAllCoroutines();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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
        character.ChangeAnimationController(0);

        character.UpdateStat();
        character.weaponParent.gameObject.SetActive(false);

#if UNITY_EDITOR
        StartCoroutine(SpawnTree());
#else
        if (gameManager.round % 3 == 0)
        {
            StartCoroutine(SpawnTree());
        }
#endif

        StartCoroutine(SpawnBush());
        StartCoroutine(beach.SpawnItem(beachParent));


        isSetPieceEnd = false;
        character.isCanControll = false;

        gameSceneUI.ActiveTutoPanel(TutoType.StartTuto, null, TutoType.StartItemTuto);

        yield return CoroutineCaching.WaitWhile(() => TutorialManager.Instance.tutorialCheck);

        cardSelecter.SetActive(true);

        //yield return new WaitWhile(() => cardSelecter.activeSelf);
        yield return CoroutineCaching.WaitWhile(() => cardSelecter.activeSelf);

        character.transform.position = gameManager.round == 0 ? initSpawnPos.transform.position : new Vector3(-1f, 0f, -41f);

        gameManager.round++;

        if (gameManager.round > 1)
        {
            gameManager.gameDayTime -= 7;
            soundManager.PlayBGM(5, true);
            multicellInvenPanel.SetActive(true);

            gameSceneUI.ActiveTutoPanel(TutoType.MulticellTuto, multicellTutoImage);
        }

        currentGameTime = gameManager.gameDayTime;

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

        canCharacterContact = true;
        nightFilter.SetActive(false);
        gameSceneUI.ChangeDayText(0, "아침이 밝았습니다.");

        soundManager.PlayBGM(1, true);

        gameSceneUI.ActiveTutoPanel(TutoType.BeachTuto);

        isSetPieceEnd = true;
        character.isCanControll = true;

        //yield return new WaitForSeconds(gameManager.gameDayTime);
        yield return CoroutineCaching.WaitForSeconds(gameManager.gameDayTime);

        StartCoroutine(NightRoutine()); 
    }

    IEnumerator NightRoutine()
    {
        soundManager.PlayBGM(4, true);

        canCharacterContact = false;
        gameSceneUI.ChangeDayText(1, "밤이 되었습니다.");
        gameSceneUI.CursorChange(CursorType.Attack);

        isNight = true;
        nightFilter.SetActive(true);

        gameSceneUI.ActiveTutoPanel(TutoType.NightTuto, weaponChangeTutoImage);

        character.UpdateStat();
        character.weaponParent.gameObject.SetActive(true);
        character.transform.position = new Vector3(-1f, 0f, -41f);

        campFire.GetComponent<Campfire>().ToNightScene();

        currentGameTime = gameManager.gameNightTime;

        character.isCanControll = true;


        //yield return new WaitForSeconds(gameManager.gameNightTime);
        yield return CoroutineCaching.WaitForSeconds(gameManager.gameNightTime);

        if (gameManager.round != gameManager.maxRound)
        {
            gameManager.fishLowGradeCount = 0;
            gameManager.fishHighGradeCount = 0;

            if (character.IsTamingPet)
                character.TamedPed.GetComponent<Pet>().RunAway();

            campFire.GetComponent<Campfire>().ToDayScene();

            itemManager.pieceItemsList = itemManager.nightPieceList;

            StartCoroutine(DayRoutine());
        }

        else
        {
            character.isCanControll = false;
            character.canFlip = false;

            nightFilter.SetActive(false);
            StopAllCoroutines();
            gameManager.isClear = true;

            soundManager.StopBGM();
            StartCoroutine(gameSceneUI.GameClear(1));
        }
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
