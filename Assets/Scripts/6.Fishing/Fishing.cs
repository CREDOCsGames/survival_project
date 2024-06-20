using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fishing : Singleton<Fishing>
{
    [SerializeField] Slider catchBar;
    [SerializeField] GameObject itemCard;
    [SerializeField] GameObject coinCard;
    [SerializeField] GameObject failCard;
    [SerializeField] Text coinCount;
    [SerializeField] Text roundText;
    [SerializeField] GameObject nextRoundUI;
    [SerializeField] GameObject keyButton;
    [SerializeField] Text keyText;
    [SerializeField] Text maxCount;
    [SerializeField] Text currentCount;
    [SerializeField] RectTransform yellow;
    [SerializeField] RectTransform red;
    [SerializeField] Text timeText;

    [Header("Stat")]
    [SerializeField] GameObject statUI;
    [SerializeField] Text maxHp;
    [SerializeField] Text reHp;
    [SerializeField] Text apHp;
    [SerializeField] Text def;
    [SerializeField] Text avoid;
    [SerializeField] Text percentDamage;
    [SerializeField] Text wAtk;
    [SerializeField] Text eAtk;
    [SerializeField] Text sAtk;
    [SerializeField] Text lAtk;
    [SerializeField] Text aSpd;
    [SerializeField] Text spd;
    [SerializeField] Text ran;
    [SerializeField] Text luk;
    [SerializeField] Text cri;

    bool isMin = true;
    bool isCatch = false;
    bool isGet = false;
    [HideInInspector] public bool isCatchingStart = false;
    int catchMult;
    int money;
    int maxFishCount;
    int currentFishCount;
    float fishingTime;
    float currentTime;

    int yRand;
    int rRand;

    GameManager gameManager;
    FishingAnim fishingAnim;

    void Start()
    {
        gameManager = GameManager.Instance;
        fishingAnim = FishingAnim.Instance;

        maxFishCount = 5;
        currentFishCount = maxFishCount;
        money = 2;
        fishingTime = 5;
        currentTime = fishingTime;

        roundText.text = gameManager.round.ToString();
        keyText.text = ((KeyCode)PlayerPrefs.GetInt("Key_Dash")).ToString();

        maxCount.text = maxFishCount.ToString();
        currentCount.text = currentFishCount.ToString();

        catchBar.gameObject.SetActive(false);
        itemCard.SetActive(false);
        coinCard.SetActive(false);
        failCard.SetActive(false);
        statUI.SetActive(false);
        nextRoundUI.SetActive(false);
        keyButton.SetActive(false);
        timeText.gameObject.SetActive(false);

        RandomBar();
    }

    void Update()
    {
        if (!gameManager.isPause)
        {
            if (!isCatch && isCatchingStart)
            {
                timeText.gameObject.SetActive(true);
                timeText.text = currentTime.ToString("F2");
                currentTime -= Time.deltaTime;

                catchBar.gameObject.SetActive(true);
                keyButton.SetActive(true);
                MoveBar(isCatch);

                if (currentFishCount > 0)
                {
                    if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Key_Dash")) || currentTime <= 0)
                    {
                        timeText.gameObject.SetActive(false);
                        keyButton.SetActive(false);
                        isCatch = true;
                        isGet = true;
                        GetItem(isGet);
                    }
                }
            }

            else if(isCatch && !isCatchingStart)
            {
                if (fishingAnim.isSomeCatch != 2)
                    keyButton.SetActive(true);

                if ((coinCard.activeSelf || failCard.activeSelf) && Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Key_Dash")))
                    Initialize();
            }
        }
    }

    void MoveBar(bool isCatch)
    {
        if (!isCatch)
        {
            if (catchBar.value == catchBar.minValue)
                isMin = true;

            else if (catchBar.value == catchBar.maxValue)
                isMin = false;

            if (isMin)
                catchBar.value += Time.deltaTime * 400;

            else if (!isMin)
                catchBar.value -= Time.deltaTime * 400;
        }
    }

    void RandomBar()
    {
        // offsetMin = new Vector2(left, bottom)
        // offsetMax = new Vector2(-right, -top)

        // yello = 합이 140
        // red = 합이 185

        yRand = Random.Range(30, 130);

        yellow.offsetMin = new Vector2(yRand, 0);
        yellow.offsetMax = new Vector2(yRand - 140, 0);

        rRand = Random.Range(yRand + 15, yRand + 30);
        red.offsetMin = new Vector2(rRand, 0);
        red.offsetMax = new Vector2(rRand - 185, 0);
    }

    void GetItem(bool isCatch)
    {
        if(isCatch)
        {
            if (currentTime > 0)
            {
                if (catchBar.value >= rRand && catchBar.value <= 25 + rRand)    // 200 - (185 - rRand)
                    catchMult = 2;

                else if (catchBar.value >= yRand && catchBar.value <= 60 + yRand)       // 200 - (140 - yRand)
                    catchMult = 1;

                else
                    catchMult = 0;
            }

            else
                catchMult = 0;

            if (catchMult == 0)
            {
                fishingAnim.isSomeCatch = 0;
                failCard.SetActive(true);
            }

            else
            {
                float rand = Random.Range(0, 10);

                if (rand <= (1 + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.02f) * catchMult)
                {
                    fishingAnim.isSomeCatch = 2;
                    itemCard.SetActive(true);
                    statUI.SetActive(true);
                    SettingStatText();
                }

                else
                {
                    fishingAnim.isSomeCatch = 1;
                    coinCount.text = money.ToString();
                    gameManager.money += money;
                    coinCard.SetActive(true);
                }
            }

            fishingAnim.isCatch = true;
            isGet = false;
            currentFishCount--;
            currentCount.text = currentFishCount.ToString();
        }
    }

    public void Initialize()
    {
        itemCard.SetActive(false);
        coinCard.SetActive(false);
        failCard.SetActive(false);
        statUI.SetActive(false);
        catchBar.gameObject.SetActive(false);
        keyButton.SetActive(false);
        isCatch = false;
        isCatchingStart = false;
        currentTime = fishingTime;

        if (currentFishCount > 0)
        {
            catchBar.value = 0;
            RandomBar();

            fishingAnim.isCatch = false;
        }

        else
            nextRoundUI.SetActive(true);
    }

    void SettingStatText()
    {
        maxHp.text = gameManager.maxHp.ToString();
        reHp.text = gameManager.recoverHp.ToString("0.#");
        apHp.text = gameManager.absorbHp.ToString("0.#");
        def.text = gameManager.defence.ToString("0.#");
        avoid.text = gameManager.avoid.ToString("0.#");
        percentDamage.text = gameManager.percentDamage.ToString("0.0#");
        wAtk.text = gameManager.physicDamage.ToString("0.#");
        eAtk.text = gameManager.magicDamage.ToString("0.#");
        sAtk.text = gameManager.shortDamage.ToString("0.#");
        lAtk.text = gameManager.longDamage.ToString("0.#");
        aSpd.text = gameManager.attackSpeed.ToString("0.#");
        spd.text = gameManager.speed.ToString("0.##");
        ran.text = gameManager.range.ToString("0.#");
        luk.text = gameManager.luck.ToString("0.#");
        cri.text = gameManager.critical.ToString("0.#");
    }

    public void NextRound()
    {
        gameManager.round++;
        gameManager.ToNextScene("Game");
    }
}
