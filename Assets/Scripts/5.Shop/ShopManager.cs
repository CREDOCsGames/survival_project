using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    Texture2D cursorNormal;
    [SerializeField] GameObject tutoPanel;

    [Header("UI")]
    [SerializeField] Text round;
    [SerializeField] Text money;
    [SerializeField] Text rerollMoneyText;
    [SerializeField] public GameObject backgroundImage;

    [Header("Stat")]
    [SerializeField] Text maxHp;
    [SerializeField] Text reHp;
    [SerializeField] Text apHp;
    [SerializeField] Text def;
    [SerializeField] Text wAtk;
    [SerializeField] Text eAtk;
    [SerializeField] Text sAtk;
    [SerializeField] Text lAtk;
    [SerializeField] Text aSpd;
    [SerializeField] Text spd;
    [SerializeField] Text ran;
    [SerializeField] Text luk;
    [SerializeField] Text cri;
    [SerializeField] Text percentDamage;
    [SerializeField] Text avoid;

    [Header("Prefabs")]
    [SerializeField] Transform cardsParent;
    [SerializeField] GameObject weaponCardUI;
    [SerializeField] GameObject passiveCardUI;
    [SerializeField] GameObject clickUI;


    [Header("Slots")]
    [SerializeField] public GameObject[] weaponSlots;
    [SerializeField] GameObject[] passiveSlots;

    GameObject[] cards;
    int[] wpCheck;      // 무기(0)인지 패시브(1)인지 확인

    //public bool[] lockBools;

    int rerollMoney;

    Color initPriceColor;

    [HideInInspector] public int WeaponSlotCount = 0;

    GameManager gameManager;
    ItemManager itemManager;
    Character character;

    float[] weightWeaponValue;
    float[] weightPassiveValue;

    int clickCount;

    int passivePercent;

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;
        character = Character.Instance;

        cursorNormal = gameManager.useCursorNormal;
        Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
        Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);

        SoundManager.Instance.PlayBGM(3, true);

        weightWeaponValue = new float[4];
        weightPassiveValue = new float[4];
        wpCheck = new int[4];
        cards = new GameObject[4];

        cursorNormal = gameManager.useCursorNormal;

        character.shield = 0f;

        clickUI.gameObject.SetActive(false);

        initPriceColor = rerollMoneyText.color;

        clickCount = 0;

        if (!gameManager.dotgu)
            rerollMoney = -gameManager.round;

        else if (gameManager.dotgu)
            rerollMoney = 0;

        StartCardSlot();

        tutoPanel.SetActive(false);

        if (gameManager.round == 1)
        {
            if (Convert.ToBoolean(PlayerPrefs.GetInt("ShopTuto", 1)))
                tutoPanel.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("ShopTuto", 1)));
        }
    }

    private void Update()
    {
        if (gameManager.currentScene == "Shop")
        {
            backgroundImage.SetActive(clickUI.activeSelf);

            gameObject.SetActive(true);
            SettingStatText();

            WeaponSlot();
            PassiveSlot();
            Refill();

            round.text = gameManager.round.ToString();
            money.text = gameManager.money.ToString();
            rerollMoneyText.text = rerollMoney.ToString();

            if (gameManager.money < -rerollMoney)
                rerollMoneyText.color = Color.red;

            else if (gameManager.money >= -rerollMoney)
                rerollMoneyText.color = initPriceColor;
        }

        else if (gameManager.currentScene == "Game")
        {
            gameObject.SetActive(false);
            rerollMoney = 0;
        }
    }

    void SettingStatText()
    {
        maxHp.text = gameManager.maxHp.ToString();
        reHp.text = gameManager.recoverHp.ToString("0.#");
        apHp.text = gameManager.absorbHp.ToString("0.#");
        def.text = gameManager.defence.ToString("0.#");
        wAtk.text = gameManager.physicDamage.ToString("0.#");
        eAtk.text = gameManager.magicDamage.ToString("0.#");
        sAtk.text = gameManager.shortDamage.ToString("0.#");
        lAtk.text = gameManager.longDamage.ToString("0.#");
        aSpd.text = gameManager.attackSpeed.ToString("0.#");
        spd.text = gameManager.speed.ToString("0.##");
        ran.text = gameManager.range.ToString("0.#");
        luk.text = gameManager.luck.ToString("0.#");
        cri.text = gameManager.critical.ToString("0.#");
        percentDamage.text = gameManager.percentDamage.ToString("0.0#");
        avoid.text = gameManager.avoid.ToString("0.#");
    }

    public void ToGameScene()
    {
        gameManager.round++;
        gameManager.ToNextScene("Game");
    }

    void ImageAlphaChange(int i, int a, Image image)
    {
        Color imageColor = image.color;
        imageColor.a = a;
        image.color = imageColor;
    }

    void TextAlphaChange(int i, int a, Text text)
    {
        Color textColor = text.color;
        textColor.a = a;
        text.color = textColor;
    }

    void Refill()
    {
        int refillNum = 0;

        for (int i = 0; i < 4; i++)
        {
            if (cardsParent.GetChild(i).childCount == 0)
                refillNum++;
        }

        if (refillNum == 4)
            CardSlot();
    }

    public void Reroll()
    {
        int lockCount = 0;

        for (int i = 0; i < itemManager.cardLocks.Length; i++)
        {
            if (itemManager.cardLocks[i] == true)
                lockCount++;
        }

        if (lockCount != 4)
        {
            SoundManager.Instance.PlayES("SelectButton");

            if (gameManager.money >= -rerollMoney)
            {
                for (int i = 0; i < cardsParent.childCount; i++)
                {
                    if (cards[i] != null)
                    {
                        if (itemManager.cardLocks[i] == false)
                            Destroy(cardsParent.GetChild(i).GetChild(0).gameObject);
                    }
                }

                gameManager.money += rerollMoney;
                clickCount++;

                if (clickCount == 1)
                    rerollMoney = -gameManager.round;

                rerollMoney -= Mathf.CeilToInt((float)(gameManager.round) / 2);

                CardSlot();
            }
        }

        else
            SoundManager.Instance.PlayES("CantBuy");
    }
    void CardSlot()
    {
        for (int i = 0; i < 4; i++)
        {
            if (itemManager.cardLocks[i] == false)
            {
                int rand = UnityEngine.Random.Range(0, 100);

                if (rand >= 85)
                {
                    GetRandomWeaponCard();
                    GameObject instant = Instantiate(weaponCardUI, cardsParent.GetChild(i).transform);
                    cards[i] = instant;
                    cards[i].GetComponent<WeaponCardUI>().isLock = false;
                    wpCheck[i] = 0;
                    instant.transform.SetParent(cardsParent.GetChild(i));
                }

                else if (rand < 85)
                {
                    GetRandomPassiveCard();
                    GameObject instant = Instantiate(passiveCardUI, cardsParent.GetChild(i).transform);
                    cards[i] = instant;
                    cards[i].GetComponent<PassiveCardUI>().isLock = false;
                    wpCheck[i] = 1;
                    instant.transform.SetParent(cardsParent.GetChild(i));
                }
            }
        }
    }

    void StartCardSlot()
    {
        if (gameManager.round < 10)
            passivePercent = 50;

        else
            passivePercent = 80;

        for (int i = 0; i < 4; i++)
        {
            if (itemManager.cardLocks[i] == false)
            {
                int rand = UnityEngine.Random.Range(0, 100);

                if (rand >= passivePercent)
                {
                    GetRandomWeaponCard();
                    GameObject instant = Instantiate(weaponCardUI, cardsParent.GetChild(i).transform);
                    cards[i] = instant;
                    wpCheck[i] = 0;
                    instant.transform.SetParent(cardsParent.GetChild(i));
                }

                else if (rand < passivePercent)
                {
                    GetRandomPassiveCard();
                    GameObject instant = Instantiate(passiveCardUI, cardsParent.GetChild(i).transform);
                    cards[i] = instant;
                    wpCheck[i] = 1;
                    instant.transform.SetParent(cardsParent.GetChild(i));
                }
            }

            else if (itemManager.cardLocks[i] == true)
            {
                if (itemManager.lockedWeaCards[i] != null)
                {
                    GameObject instant = Instantiate(weaponCardUI, cardsParent.GetChild(i).transform);
                    cards[i] = instant;
                    wpCheck[i] = 0;
                    instant.transform.SetParent(cardsParent.GetChild(i));
                }

                else if (itemManager.lockedPassCards[i] != null)
                {
                    GameObject instant = Instantiate(passiveCardUI, cardsParent.GetChild(i).transform);
                    cards[i] = instant;
                    wpCheck[i] = 1;
                    instant.transform.SetParent(cardsParent.GetChild(i));
                }
            }
        }
    }

    void WeaponSlot()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            Image image = weaponSlots[i].transform.GetChild(2).GetComponent<Image>();
            Text text = weaponSlots[i].transform.GetChild(3).GetComponent<Text>();
            Text x = weaponSlots[i].transform.GetChild(4).GetComponent<Text>();

            TextAlphaChange(i, 0, text);
            TextAlphaChange(i, 0, x);

            if (itemManager.storedWeapon[i] != null)
            {
                ImageAlphaChange(i, 1, image);
                image.sprite = itemManager.storedWeapon[i].ItemSprite;
            }

            else if (itemManager.storedWeapon[i] == null)
                ImageAlphaChange(i, 0, image);
        }
    }

    void PassiveSlot()
    {
        for (int i = 0; i < passiveSlots.Length; i++)
        {
            Image back = passiveSlots[i].transform.GetChild(0).GetComponent<Image>();
            Image image = passiveSlots[i].transform.GetChild(2).GetComponent<Image>();
            Text countText = passiveSlots[i].transform.GetChild(3).GetComponent<Text>();
            Text x = passiveSlots[i].transform.GetChild(4).GetComponent<Text>();

            if (itemManager.storedPassive[i] != null)
            {
                ImageAlphaChange(i, 1, image);
                image.sprite = itemManager.storedPassive[i].ItemSprite;
                countText.text = itemManager.storedPassiveCount[i].ToString();

                if (itemManager.storedPassive[i].ItemGrade == Grade.일반)
                    back.color = new Color(0.53f, 0.53f, 0.53f, 0.8235f);

                else if (itemManager.storedPassive[i].ItemGrade == Grade.희귀)
                    back.color = new Color(0, 0.77f, 1, 0.8235f);

                else if (itemManager.storedPassive[i].ItemGrade == Grade.전설)
                    back.color = new Color(0.5f, 0.2f, 0.4f, 0.8235f);

                else if (itemManager.storedPassive[i].ItemGrade == Grade.신화)
                    back.color = new Color(1, 0.31f, 0.31f, 0.8235f);

                if (itemManager.storedPassiveCount[i] > 1)
                {
                    TextAlphaChange(i, 1, countText);
                    TextAlphaChange(i, 1, x);
                }

                else if (itemManager.storedPassiveCount[i] <= 1)
                {
                    TextAlphaChange(i, 0, countText);
                    TextAlphaChange(i, 0, x);
                }

                if (itemManager.storedPassive[i].ItemName == "시청료")
                {
                    passiveSlots[i].transform.GetChild(5).GetComponent<Text>().text = $"{gameManager.feeMoney} 코인";
                    passiveSlots[i].transform.GetChild(5).gameObject.SetActive(true);
                }

                else if(itemManager.storedPassive[i].ItemName == "3티어 구독권")
                {
                    passiveSlots[i].transform.GetChild(5).GetComponent<Text>().text = $"{gameManager.subMoney} 코인";
                    passiveSlots[i].transform.GetChild(5).gameObject.SetActive(true);
                }
            }

            else if (itemManager.storedPassive[i] == null)
            {
                back.color = new Color(0.53f, 0.53f, 0.53f, 0.8235f);
                ImageAlphaChange(i, 0, image);
                TextAlphaChange(i, 0, countText);
                TextAlphaChange(i, 0, x);
            }
        }
    }

    Grade RandomWeaponGrade()
    {
        float totalWeight = 0;

        weightWeaponValue[0] = 150f - (gameManager.round - 1f) * 5f;
        weightWeaponValue[1] = 10f + (gameManager.round - 1f) * (gameManager.round);
        weightWeaponValue[2] = (gameManager.round - 1f) * (1f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.01f);
        weightWeaponValue[3] = (gameManager.round - 1f) * 0.5f * (1 + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.01f);

        for (int i = 0; i < weightWeaponValue.Length; i++)
            totalWeight += weightWeaponValue[i];

        float rand = UnityEngine.Random.Range(0, totalWeight);
        float gradeNum = 0;
        float total = 0;

        for (int i = 0; i < weightWeaponValue.Length; i++)
        {
            total += weightWeaponValue[i];
            if (rand <= total)
            {
                gradeNum = i;
                break;
            }
        }

        Grade grade;

        if(gradeNum == 1)
            grade = Grade.희귀;

        else if (gradeNum == 2)
            grade = Grade.전설;

        else if (gradeNum == 3)
            grade = Grade.신화;

        else
            grade = Grade.일반;

        return grade;
    }

    void GetRandomWeaponCard()
    {
        WeaponCardUI weaponCard = weaponCardUI.GetComponent<WeaponCardUI>();
        int rand = UnityEngine.Random.Range(0, weaponCardUI.GetComponent<WeaponCardUI>().weaponInfos.Length);
        weaponCard.selectGrade = RandomWeaponGrade();
        weaponCard.selectedWeapon = weaponCard.weaponInfos[rand];
    }

    void GetRandomPassiveCard()
    {
        float totalWeight = 0;

        weightPassiveValue[0] = 200f - (gameManager.round - 1f) * 6f;
        weightPassiveValue[1] = 10f * gameManager.round;
        weightPassiveValue[2] = (gameManager.round - 1f) * (gameManager.round) * 0.1f * (1f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.01f);
        weightPassiveValue[3] = (gameManager.round - 1f) * (gameManager.round) * 0.02f * (1f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.01f);

        PassiveCardUI passiveCard = passiveCardUI.GetComponent<PassiveCardUI>();

        for (int i = 0; i < passiveCardUI.GetComponent<PassiveCardUI>().passiveInfo.Length; i++)
        {
            passiveCard.passiveInfo[i].weight = weightPassiveValue[(int)passiveCard.passiveInfo[i].ItemGrade];
            totalWeight += passiveCard.passiveInfo[i].weight;
        }

        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float total = 0f;

        for (int i = 0; i < passiveCardUI.GetComponent<PassiveCardUI>().passiveInfo.Length; i++)
        {
            total += passiveCard.passiveInfo[i].weight;

            if (rand <= total)
            {
                if (itemManager.passiveCounts[i] != 0)
                {
                    passiveCard.selectedPassive = passiveCard.passiveInfo[i];
                    break;
                }

                else
                {
                    rand = UnityEngine.Random.Range(0f, totalWeight);
                    total = 0f;
                    i = 0;
                }
            }
        }
    }
}
