using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndUI : Singleton<EndUI>
{
    Texture2D cursorNormal;

    [Header("UI")]
    [SerializeField] GameObject overUI;
    [SerializeField] GameObject clearUI;
    [SerializeField] Text round;

    [Header("Time")]
    [SerializeField] Text hTimeText;
    [SerializeField] Text mTimeText;
    [SerializeField] Text sTimeText;

    [Header("WeaponSlot")]
    [SerializeField] Transform[] weaponSlotParents;
    [SerializeField] GameObject weaponSlotPrefab;

    [Header("PassSlot")]
    [SerializeField] Transform[] passSlotParents;
    [SerializeField] GameObject passSlotPrefab;

    [Header("Stat")]
    [SerializeField] Text lv;
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

    GameManager gameManager;
    Character character;
    ItemManager itemManager;

    [HideInInspector] public int[] weaponCount;

    float endTime;
    float endTimeS;
    float endTimeM;
    float endTimeH;

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        itemManager = ItemManager.Instance;

        cursorNormal = gameManager.useCursorNormal;
        Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
        Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);

        SoundManager.Instance.PlayBGM(7, false);
        weaponCount = new int[6];
        UISetting();
        SettingStatText();
        WeaponSlotSetting();
        PassiveSlotSetting();
        TimeSetting();

        character.gameObject.SetActive(false);
    }

    void TimeSetting()
    {
        endTime = Mathf.Round(gameManager.gameEndTime - gameManager.gameStartTime);

        endTimeS = Mathf.Floor((endTime % 3600) % 60);
        endTimeM = Mathf.Floor(((endTime - endTimeS) % 3600) / 60);
        endTimeH = Mathf.Floor((endTime - endTimeM) / 3600);

        hTimeText.text = endTimeH.ToString();
        mTimeText.text = endTimeM.ToString();
        sTimeText.text = endTimeS.ToString();
    }

    void UISetting()
    {
        if (character.isDead)
        {
            if (gameManager.isedolCount != 5)
            {
                overUI.SetActive(true);
                clearUI.SetActive(false);
                round.text = gameManager.round.ToString();
            }

            else if(gameManager.isedolCount == 5)
            {
                overUI.SetActive(false);
                clearUI.SetActive(true);
            }
        }

        else if (!character.isDead)
        {
            if (gameManager.woodCount >= gameManager.woodMaxCount)
            {
                overUI.SetActive(false);
                clearUI.SetActive(true);
            }

            else
            {
                overUI.SetActive(true);
                clearUI.SetActive(false);
                round.text = gameManager.round.ToString();
            }
        }
    }

    void WeaponSlotSetting()
    {
        int count = 0;
        for (int i = 0; i < itemManager.storedWeapon.Length; i++)
        {
            if (itemManager.storedWeapon[i] != null)
            {
                GameObject slot = Instantiate(weaponSlotPrefab, weaponSlotParents[count]);
                slot.transform.SetParent(weaponSlotParents[count]);
                weaponCount[count] = i;
                count++;
            }
        }
    }

    void PassiveSlotSetting()
    {
        for (int i = 0; i < itemManager.storedPassive.Length; i++)
        {
            if (itemManager.storedPassive[i] != null)
            {
                GameObject slot = Instantiate(passSlotPrefab, passSlotParents[i]);
                slot.transform.SetParent(passSlotParents[i]);
            }
        }
    }

    void SettingStatText()
    {
        lv.text = character.level.ToString();
        maxHp.text = character.maxHp.ToString();
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

    public void ToTitle()
    {
        Destroy(ItemManager.Instance.gameObject);
        Destroy(Character.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("StartTitle");
    }
}
