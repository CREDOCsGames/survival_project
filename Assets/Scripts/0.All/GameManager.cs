using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum Status
{
    MAXHP,
    DAMAGE,
    RECOVER,
    DEFENCE,
    ATTACK_SPEED,
    SPEED,
    CRITICAL,
    AVOID,
}

public class GameManager : Singleton<GameManager>       
{
    [SerializeField] public Texture2D[] cursorNormal;
    [SerializeField] public Texture2D[] cursorAttack;
    [SerializeField] public Transform bulletStorage;
    [SerializeField] public Transform damageStorage;

    [Header("GameData")]
    [SerializeField] float initGameTime;
    [SerializeField] public float gameDayTime;
    [SerializeField] public float gameNightTime;
    [SerializeField] public int money;
    [SerializeField] public int round;
    [SerializeField] public int woodCount;
    public int fishHighGradeCount = 0;
    public int fishLowGradeCount = 0;

    [Header("StatData")]
    [SerializeField] public int maxHp;
    /*[HideInInspector]*/ public float percentDamage;
    [SerializeField] public float recoverHp;
    [SerializeField] public float absorbHp;
    [SerializeField] public int defence;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float speed;
    [SerializeField] public float range;
    [SerializeField] public float critical;
    [SerializeField] public int avoid;
    [SerializeField] public int dashCount;

    [HideInInspector] public string currentScene;

    Scene scene;

    [HideInInspector] public bool isPause;
    [HideInInspector] public bool isClear;
    [HideInInspector] public bool isBossDead;

    [HideInInspector] public float gameStartTime;
    [HideInInspector] public float gameEndTime;

    [HideInInspector] public Texture2D useCursorNormal;
    [HideInInspector] public Texture2D useCursorAttack;

    [HideInInspector] public int cursorSize;

    [HideInInspector] public bool isTuto = false;

    [HideInInspector] public Vector3 characterSpawnPos = new Vector3(0, 0, -40);

    public Dictionary<Status, int> status = new Dictionary<Status, int>();

    public static string[] statNames = { "최대 체력", "공격력", "회복 수치", "방어력", "공격 속도", "이동 속도", "크리티컬", "회피율" };

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        cursorSize = PlayerPrefs.GetInt("CursorSize", 0);
        useCursorNormal = cursorNormal[cursorSize];
        useCursorAttack = cursorAttack[cursorSize];

        Vector2 cursorHotSpot = new Vector3(useCursorNormal.width * 0.5f, useCursorNormal.height * 0.5f);
        Cursor.SetCursor(useCursorNormal, cursorHotSpot, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.None;

        //PlayerPrefs.SetInt("GameTuto", 1);
        /*PlayerPrefs.SetInt("ShopTuto", 1);
        PlayerPrefs.SetInt("BossTuto", 1);
        PlayerPrefs.SetInt("BagicClear", 0);
*/
        InitSetting();

        isPause = false;
        Time.timeScale = 1;
        isPause = false;
        isClear = false;
        isBossDead = false;
    }

    void InitSetting()
    {

#if UNITY_EDITOR

#else
        initGameTime = 20;
        gameDayTime = initGameTime;
        money = 0;
        woodCount = 0;
        round = 0;
        maxHp = 30;
        recoverHp = 1;
        absorbHp = 0;
        defence = 3;
        attackSpeed = 0;
        speed = 3;
        range = 0;
        luck = 0;
        critical = 5;
        avoid = 1;
#endif

        status.Add(Status.MAXHP, maxHp);
        status.Add(Status.DAMAGE, 1);
        status.Add(Status.RECOVER, 10);
        status.Add(Status.DEFENCE, 1);
        status.Add(Status.ATTACK_SPEED, 1);
        status.Add(Status.SPEED, 3);
        status.Add(Status.CRITICAL, 5);
        status.Add(Status.AVOID, 1);
    }

    private void Update()
    {
        scene = SceneManager.GetActiveScene();
        //currentScene = scene.name;

        if (scene.buildIndex > 1)
        {
            OnGameScene();
        }
    }

    void OnGameScene()
    {
        if (currentScene == "Game")
        {
            if (money <= 0)
                money = 0;
        }
    }

    public void ToNextScene(string sceneName)
    {
        Character character = Character.Instance;
        character.GetComponent<NavMeshAgent>().enabled = false;
        character.transform.position = characterSpawnPos;
        
        currentScene = sceneName;

        character.shield = 0f;
        //character.currentHp = character.maxHp;
        character.dashCount = dashCount;
        character.dashCoolTime = character.initDashCoolTime;

        SceneManager.LoadScene(currentScene);

        //gameTime = Mathf.Clamp(initGameTime + (round - 1) * 3f, initGameTime, 60f);

        /*if (round != 8)
            gameTime = 0;

        else
            gameTime = 30;*/

        isClear = false;
    }
}