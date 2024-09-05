using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum Status
{
    Maxhp,
    Damage,
    CloseDamage,
    LongDamage,
    Recover,
    Defence,
    AttackSpeed,
    MoveSpeed,
    Critical,
    Avoid,
}

public enum SpecialStatus
{
    DoubleAxe,
    Rum,
    AmmoPouch,
    RustyHarpoon,
    BaitWarm,
    Soulmate,
    Grape,
    Tabatiere,
    Invincible,
    SilverBullet,
    BloodMadness,
    RottenCheese,
    TurTle,
    Mirror,
    Count,
}

public class GameManager : Singleton<GameManager>       
{
    [SerializeField] public Texture2D[] cursorNormal;
    [SerializeField] public Texture2D[] cursorAttack;
    [SerializeField] public Transform bulletStorage;
    [SerializeField] public Transform damageStorage;

    [Header("GameData")]
    [SerializeField] GameObject characterPrefab;
    [SerializeField] public float gameDayTime;
    [SerializeField] public float gameNightTime;
    [SerializeField] public int round;
    [SerializeField] public int woodCount;
    public int fishHighGradeCount = 0;
    public int fishLowGradeCount = 0;

    [Header("StatData")]
    [SerializeField] int maxHp;
    [SerializeField] int damage;
    [SerializeField] int closeDamage;
    [SerializeField] int longDamage;
    [SerializeField] int recoverHp;
    [SerializeField] int defence;
    [SerializeField] int attackSpeed;
    [SerializeField] int speed;
    [SerializeField] int critical;
    [SerializeField] int avoid;
    [SerializeField] public int dashCount;

    public int percentDamage;
    public int percentDefence;
    public int bloodDamage;

    [HideInInspector] public string currentScene;

    [HideInInspector] public bool isPause;
    [HideInInspector] public bool isClear;
    [HideInInspector] public bool isBossDead;

    [HideInInspector] public Texture2D useCursorNormal;
    [HideInInspector] public Texture2D useCursorAttack;

    [HideInInspector] public int cursorSize;

    [HideInInspector] public bool isTuto = false;

    public Vector3 characterSpawnPos = new Vector3(0, 0, -69f);

    public Dictionary<Status, int> status = new Dictionary<Status, int>();
    public Dictionary<SpecialStatus, bool> specialStatus = new Dictionary<SpecialStatus, bool>();

    public static string[] statNames = { "최대 체력", "공격력", "근거리 공격력", "원거리 공격력", "회복 수치", "방어력", "공격 속도", "이동 속도", "크리티컬", "회피율" };

    public int totalBulletCount;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        InitSetting();
    }

    private void Start()
    {
        cursorSize = PlayerPrefs.GetInt("CursorSize", 0);
        useCursorNormal = cursorNormal[cursorSize];
        useCursorAttack = cursorAttack[cursorSize];

        Vector2 cursorHotSpot = new Vector3(useCursorNormal.width * 0.5f, useCursorNormal.height * 0.5f);
        Cursor.SetCursor(useCursorNormal, cursorHotSpot, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

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
        gameDayTime = 120;
        gameNightTime = 60;
        woodCount = 0;
        round = 0;
        maxHp = 100;
        recoverHp = 1;
        defence = 3;
        attackSpeed = 1;
        speed = 3;
        critical = 5;
        avoid = 3;

        dashCount = 0;

        fishLowGradeCount = 0;
        fishHighGradeCount = 0;

        percentDamage = 100;
        percentDefence = 100;
        bloodDamage = 0;

        totalBulletCount = 1;
#endif

        status.Add(Status.Maxhp, maxHp);
        status.Add(Status.Damage, damage);
        status.Add(Status.CloseDamage, closeDamage);
        status.Add(Status.LongDamage, longDamage);
        status.Add(Status.Recover, recoverHp);
        status.Add(Status.Defence, defence);
        status.Add(Status.AttackSpeed, attackSpeed);
        status.Add(Status.MoveSpeed, speed);
        status.Add(Status.Critical, critical);
        status.Add(Status.Avoid, avoid);

        for (int i = 0; i < (int)SpecialStatus.Count; ++i)
        {
            specialStatus.Add((SpecialStatus)i, false);
        }

        //specialStatus[SpecialStatus.Mirror] = true;
    }

    public void ToNextScene(string sceneName)
    {
        Character character = Instantiate(characterPrefab, Vector3.zero, characterPrefab.transform.rotation).GetComponent<Character>();
        character.GetComponent<NavMeshAgent>().enabled = false;
        character.transform.position = characterSpawnPos;
        
        currentScene = sceneName;

        SceneManager.LoadScene(sceneName);
        //isClear = false;
    }
}