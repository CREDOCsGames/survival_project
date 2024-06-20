using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManager : Singleton<GameManager>       
{
    [SerializeField] public Texture2D[] cursorNormal;
    [SerializeField] public Texture2D[] cursorAttack;
    [SerializeField] public Transform bulletStorage;
    [SerializeField] public Transform damageStorage;

    [Header("GameData")]
    [SerializeField] float initGameTime;
    [SerializeField] float gameTime;
    [SerializeField] public int money;
    [SerializeField] public int round;
    [SerializeField] public int woodCount;
    [SerializeField] public int woodMaxCount;

    [Header("StatData")]
    [SerializeField] public float maxHp;
    [HideInInspector] public float percentDamage;
    /*[HideInInspector]*/ public float physicDamage;
    [HideInInspector] public float magicDamage;
    [HideInInspector] public float shortDamage;
    /*[HideInInspector]*/ public float longDamage;
    [SerializeField] public float recoverHp;
    [SerializeField] public float absorbHp;
    [SerializeField] public float defence;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float speed;
    [SerializeField] public float range;
    [SerializeField] public float luck;
    [SerializeField] public float critical;
    [SerializeField] public float avoid;

    #region 특수 패시브
    [HideInInspector] public int[] passiveIntVariables;
    [HideInInspector] public float[] passiveFloatVariables;
    [HideInInspector] public bool[] passiveBoolVariables;
    /*[HideInInspector]*/ public int dashCount;
    [HideInInspector] public int buffNum;
    [HideInInspector] public int exDmg;
    /*[HideInInspector]*/ public int isedolCount;
    [HideInInspector] public float salePercent;
    [HideInInspector] public float increaseExp;
    [HideInInspector] public float coinRange;
    [HideInInspector] public float monsterSlow;
    [HideInInspector] public float summonASpd;
    [HideInInspector] public float summonPDmg;
    [HideInInspector] public float monsterDef;
    [HideInInspector] public bool luckCoin;
    [HideInInspector] public bool luckDamage;
    [HideInInspector] public bool luckCritical;
    [HideInInspector] public bool doubleShot;
    [HideInInspector] public bool revive;
    [HideInInspector] public bool ggoGgoSummon;
    [HideInInspector] public bool ilsoonSummon;
    [HideInInspector] public bool wakgoodSummon;
    [HideInInspector] public bool ddilpa;
    [HideInInspector] public bool butterfly;
    [HideInInspector] public bool subscriptionFee;
    [HideInInspector] public bool spawnTree;
    [HideInInspector] public bool dotgu;
    [HideInInspector] public bool isReflect;
    [HideInInspector] public bool onePenetrate;
    [HideInInspector] public bool lowPenetrate;
    [HideInInspector] public bool penetrate;
    [HideInInspector] public bool vamAbsorb;
    #endregion

    [HideInInspector] public float currentGameTime;

    [HideInInspector] public string currentScene;

    Scene scene;

    [HideInInspector] public float[] stats;

    [HideInInspector] public bool isPause;
    [HideInInspector] public bool isClear;
    [HideInInspector] public bool isBossDead;

    [HideInInspector] public float gameStartTime;
    [HideInInspector] public float gameEndTime;

    [HideInInspector] public int feeMoney = 0;
    [HideInInspector] public int subMoney = 0;

    [HideInInspector] public Texture2D useCursorNormal;
    [HideInInspector] public Texture2D useCursorAttack;

    [HideInInspector] public int cursorSize;

    [HideInInspector] public float maxAbs;

    [HideInInspector] public bool isTuto = false;

    public Vector3 characterSpawnPos = new Vector3(0, 0, -40);

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
        //InitSetting();
        InitArray();
        currentGameTime = gameTime;
        isPause = false;
        Time.timeScale = 1;
        isPause = false;
        isClear = false;
        isBossDead = false;
    }

    void InitSetting()
    {
        initGameTime = 20;
        gameTime = initGameTime;
        money = 0;
        woodCount = 0;
        woodMaxCount = 70;
        round = 1;
        maxHp = 30;
        recoverHp = 0;
        absorbHp = 0;
        defence = 0;
        physicDamage = 0;
        magicDamage = 0;
        shortDamage = 0;
        longDamage = 0;
        attackSpeed = 0;
        speed = 0;
        range = 0;
        luck = 0;
        critical = 5;

        dashCount = 0;
        buffNum = 0;
        exDmg = 0;
        isedolCount = 0;

        salePercent = 0;
        increaseExp = 0;
        coinRange = 0;
        monsterSlow = 0;
        summonASpd = 0;
        summonPDmg = 0;
        monsterDef = 0;

        luckCoin = false;
        luckDamage = false;
        luckCritical = false;
        doubleShot = false;
        revive = false;
        ggoGgoSummon = false;
        ilsoonSummon = false;
        wakgoodSummon = false;
        ddilpa = false;
        butterfly = false;
        subscriptionFee = false;
        spawnTree = false;
        dotgu = false;
        isReflect = false;
        onePenetrate = false;
        lowPenetrate = false;
        penetrate = false;
    }

    void InitArray()
    {
        stats = new float[15];
        stats[0] = maxHp;
        stats[1] = recoverHp;
        stats[2] = absorbHp;
        stats[3] = defence;
        stats[4] = physicDamage;
        stats[5] = magicDamage;
        stats[6] = shortDamage;
        stats[7] = longDamage;
        stats[8] = attackSpeed;
        stats[9] = speed;
        stats[10] = luck;
        stats[11] = range;
        stats[12] = critical;
        stats[13] = percentDamage;
        stats[14] = avoid;

        passiveIntVariables = new int[4];

        passiveIntVariables[0] = dashCount;
        passiveIntVariables[1] = buffNum;
        passiveIntVariables[2] = exDmg;
        passiveIntVariables[3] = isedolCount;

        passiveFloatVariables = new float[7];

        passiveFloatVariables[0] = coinRange;
        passiveFloatVariables[1] = increaseExp;
        passiveFloatVariables[2] = monsterSlow;
        passiveFloatVariables[3] = salePercent;
        passiveFloatVariables[4] = summonASpd;
        passiveFloatVariables[5] = summonPDmg;
        passiveFloatVariables[6] = monsterDef;

        passiveBoolVariables = new bool[18];

        passiveBoolVariables[0] = luckCoin;
        passiveBoolVariables[1] = luckDamage;
        passiveBoolVariables[2] = luckCritical;
        passiveBoolVariables[3] = doubleShot;
        passiveBoolVariables[4] = revive;
        passiveBoolVariables[5] = ggoGgoSummon;
        passiveBoolVariables[6] = ilsoonSummon;
        passiveBoolVariables[7] = wakgoodSummon;
        passiveBoolVariables[8] = ddilpa;
        passiveBoolVariables[9] = butterfly;
        passiveBoolVariables[10] = subscriptionFee;
        passiveBoolVariables[11] = spawnTree;
        passiveBoolVariables[12] = dotgu;
        passiveBoolVariables[13] = isReflect;
        passiveBoolVariables[14] = onePenetrate;
        passiveBoolVariables[15] = lowPenetrate;
        passiveBoolVariables[16] = penetrate;
        passiveBoolVariables[17] = vamAbsorb;
    }

    void StatArray()
    {
        maxHp = stats[0];
        recoverHp = stats[1];
        absorbHp = stats[2];
        defence = stats[3];
        physicDamage = stats[4];
        magicDamage = stats[5];
        shortDamage = stats[6];
        longDamage = stats[7];
        if (!Character.Instance.isBuff)
            attackSpeed = stats[8];
        speed = stats[9];
        luck = stats[10];
        range = stats[11];
        critical = stats[12];
        if (!Character.Instance.isBuff)
            percentDamage = stats[13];
        avoid = stats[14];
    }

    void IntVariableArray()
    {
        dashCount = passiveIntVariables[0];
        buffNum = passiveIntVariables[1];
        exDmg = passiveIntVariables[2];
        isedolCount = passiveIntVariables[3];
    }

    void FloatVariableArray()
    {
        coinRange = passiveFloatVariables[0];
        increaseExp = passiveFloatVariables[1];
        monsterSlow = passiveFloatVariables[2];
        salePercent = passiveFloatVariables[3];
        summonASpd = passiveFloatVariables[4];
        summonPDmg = passiveFloatVariables[5];
        monsterDef = passiveFloatVariables[6];
    }

    void BoolVariableArray()
    {
        luckCoin = passiveBoolVariables[0];
        luckDamage = passiveBoolVariables[1];
        luckCritical = passiveBoolVariables[2];
        doubleShot = passiveBoolVariables[3];
        revive = passiveBoolVariables[4];
        ggoGgoSummon = passiveBoolVariables[5];
        ilsoonSummon = passiveBoolVariables[6];
        wakgoodSummon = passiveBoolVariables[7];
        ddilpa = passiveBoolVariables[8];
        butterfly = passiveBoolVariables[9];
        subscriptionFee = passiveBoolVariables[10];
        spawnTree = passiveBoolVariables[11];
        dotgu = passiveBoolVariables[12];
        isReflect = passiveBoolVariables[13];
        onePenetrate = passiveBoolVariables[14];
        lowPenetrate = passiveBoolVariables[15];
        penetrate = passiveBoolVariables[16];
        vamAbsorb = passiveBoolVariables[17];
    }

    private void Update()
    {
        scene = SceneManager.GetActiveScene();
        //currentScene = scene.name;

        if (currentGameTime <= 0)
        {
            isClear = true;
        }

        if (scene.buildIndex > 1)
        {
            StatSetting();
            OnGameScene();
        }

        if (vamAbsorb)
            maxAbs = 2f;

        else if(!vamAbsorb)
            maxAbs = 1f;
    }

    public void StatSetting()
    {
        StatArray();
        IntVariableArray();
        FloatVariableArray();
        BoolVariableArray();
    }

    void OnGameScene()
    {
        if (currentScene == "Game")
        {
            if (Character.Instance.currentHp > 0)
                currentGameTime -= Time.deltaTime;

            if (currentGameTime <= 0)
                currentGameTime = 0;

            if (money <= 0)
                money = 0;
        }
    }

    public void ToNextScene(string sceneName)
    {
        Character character = Character.Instance;

        character.transform.position = characterSpawnPos;
        currentScene = sceneName;
        character.thunderMark.transform.localScale = new Vector3(Mathf.Clamp(4f + range * 0.5f, 1, 12), Mathf.Clamp(4f + range * 0.5f, 1, 12), 0);

        if (maxHp < 1)
            character.maxHp = 1;

        else if (maxHp >= 1)
            character.maxHp = maxHp;

        character.shield = 0f;
        character.currentHp = character.maxHp;
        character.dashCount = dashCount;
        character.dashCoolTime = character.initDashCoolTime;

        SceneManager.LoadScene(currentScene);

        gameTime = Mathf.Clamp(initGameTime + (round - 1) * 3f, initGameTime, 60f);

        /*if (round != 8)
            gameTime = 0;

        else
            gameTime = 30;*/

        currentGameTime = gameTime;

        isClear = false;
    }
}