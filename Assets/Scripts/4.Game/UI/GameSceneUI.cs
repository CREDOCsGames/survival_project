using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum CursorType
{
    Normal,
    Attack,
}

public class GameSceneUI : Singleton<GameSceneUI>
{
    Texture2D cursorAttack;
    Texture2D cursorNormal;

    [SerializeField] public GameObject monsterSpawn;
    [SerializeField] GameObject tutoPanel;
    public GameObject tamingGame;

    [Header("HP")]
    [SerializeField] Text hpText;
    [SerializeField] Slider hpBar;
    [SerializeField] Slider recoveryGaugeBar;

    [Header("Items")]
    [SerializeField] Text woodCount;
    [SerializeField] Text fish1Count;
    [SerializeField] Text fish2Count;

    [Header("Time")]
    [SerializeField] Text timeText;

    [Header("Round")]
    [SerializeField] Text roundText;

    [Header("Dash")]
    [SerializeField] GameObject dash;
    [SerializeField] Text dashKey;
    [SerializeField] Image dashImage;
    [SerializeField] Text dashCoolTime;
    [SerializeField] GameObject dashCountParent;
    [SerializeField] Text dashCount;
    [SerializeField] Text maxDashCount;

    [Header("Stat")]
    [SerializeField] GameObject statWindow;
    [SerializeField] Text maxHp;
    [SerializeField] Text def;
    [SerializeField] Text avoid;
    [SerializeField] Text damage;
    [SerializeField] Text aSpd;
    [SerializeField] Text spd;
    [SerializeField] Text cri;

    [Header("Day Change")]
    [SerializeField] GameObject dayChangeGO;
    [SerializeField] Text dayChangeText;
    [SerializeField] Image dayChangeImage;
    [SerializeField] Sprite[] dayChangeSprites;

    [Header("Text")]
    [SerializeField] Text bossSceneText;

    GameManager gameManager;
    Character character;
    SoundManager soundManager;
    GamesceneManager gamesceneManager;

    [SerializeField] GameObject fishingGame;
    [SerializeField] GameObject weaponUI;

    Color initTimeColor;

    Coroutine currentCoroutine;

    protected override void Awake()
    {
        base.Awake();
        bossSceneText.gameObject.SetActive(false);
        dash.SetActive(false);
        statWindow.SetActive(false);
        tutoPanel.SetActive(false);
        tamingGame.SetActive(false);
        fishingGame.SetActive(true);
        weaponUI.SetActive(false);
        dayChangeGO.SetActive(false);

        gameManager = GameManager.Instance;

        if (gameManager.round == 1)
        {
            if (Convert.ToBoolean(PlayerPrefs.GetInt("GameTuto", 1)))
            {
                monsterSpawn.SetActive(!Convert.ToBoolean(PlayerPrefs.GetInt("GameTuto", 1)));
                tutoPanel.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("GameTuto", 1)));
            }
        }

        else if(gameManager.round == 10)
        {
            if (Convert.ToBoolean(PlayerPrefs.GetInt("BossTuto", 1)))
            {
                monsterSpawn.SetActive(!Convert.ToBoolean(PlayerPrefs.GetInt("BossTuto", 1)));
                tutoPanel.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("BossTuto", 1)));
            }
        }
    }

    private void Start()
    {
        character = Character.Instance;
        soundManager = SoundManager.Instance;
        gamesceneManager = GamesceneManager.Instance;

        if (gameManager.round == 10 || gameManager.round == 20 || gameManager.round == 30)
        {
            soundManager.PlayBGM(2, true);
            soundManager.PlayES("Alert");
            bossSceneText.gameObject.SetActive(true);
            StartCoroutine(BlinkBossSceneText());
        }

        else
            soundManager.PlayBGM(1, true);

        if (gameManager.round == 1)
            gameManager.gameStartTime = Time.realtimeSinceStartup;

        character.GetComponent<NavMeshAgent>().enabled = true;

        initTimeColor = timeText.color;
    }

    IEnumerator BlinkBossSceneText()
    {
        float fadeTime = 0.5f;
        Color color = bossSceneText.color;

        while (fadeTime > 0f)
        {
            fadeTime = Mathf.Clamp(fadeTime - Time.deltaTime, 0f, 0.5f);
            color.a = fadeTime + 0.5f;
            bossSceneText.color = color;
            yield return null;
        }

        while (fadeTime < 0.5f)
        {
            fadeTime = Mathf.Clamp(fadeTime + Time.deltaTime, 0f, 0.5f);
            color.a = fadeTime + 0.5f;
            bossSceneText.color = color;
            yield return null;
        }

        while (fadeTime > 0f)
        {
            fadeTime = Mathf.Clamp(fadeTime - Time.deltaTime, 0f, 0.5f);
            color.a = fadeTime + 0.5f;
            bossSceneText.color = color;
            yield return null;
        }

        bossSceneText.gameObject.SetActive(false);
    }
    
    public void ChangeDayText(int index, string text)
    {
        dayChangeText.text = text;
        dayChangeImage.sprite = dayChangeSprites[index];

        dayChangeGO.SetActive(true);

        if(currentCoroutine != null) 
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(FadeOut(dayChangeImage, dayChangeText, dayChangeGO));
    }

    private void Update()
    {
        HpUI();
        RecoveryGauegeUI();
        ItemsCountUI();
        RoundUI();
        TimeUI();
        DashUI();
        WeaponUI();
        SettingStatText();
    }

    public void CursorChange(CursorType cursorType)
    {
        cursorNormal = gameManager.useCursorNormal;
        cursorAttack = gameManager.useCursorAttack;

        Texture2D currentCursor;

        switch (cursorType)
        {
            case CursorType.Normal:
                currentCursor = cursorNormal;
                break;

            case CursorType.Attack:
                currentCursor = cursorAttack;
                break;

            default:
                currentCursor = cursorNormal;
                break;
        }

        Vector2 cursorHotSpot = new Vector3(currentCursor.width * 0.5f, currentCursor.height * 0.5f);
        Cursor.SetCursor(currentCursor, cursorHotSpot, CursorMode.ForceSoftware);
    }

    /*IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1f);

        float fadeTime = 0f;
        Color imageColor = clearImage.color;
        clearImage.gameObject.SetActive(true);

        do
        {
            fadeTime = Mathf.Clamp(fadeTime + Time.deltaTime, 0f, 2f);
            imageColor.a = fadeTime / 2f;
            clearImage.color = imageColor;
            yield return null;
        }

        while (fadeTime < 2f);

        yield return new WaitForSeconds(3.5f);

        clickText.SetActive(true);
    }*/

    IEnumerator FadeOut(Image image, Text text, GameObject go)
    {
        float fadeTime = 2f;
        Color imageColor = image.color;
        Color textColor = text.color;

        do
        {
            fadeTime = Mathf.Clamp(fadeTime - Time.deltaTime, 0f, 2f);

            imageColor.a = fadeTime / 2f;
            textColor.a = imageColor.a;

            image.color = imageColor;
            text.color = textColor;

            yield return null;
        }
        while (fadeTime > 0);

        yield return null;

        go.SetActive(false);
    }

    void SettingStatText()
    {
        maxHp.text = character.maxHp.ToString();
        def.text = character.defence.ToString("0.#");
        avoid.text = character.avoid.ToString("0.#");
        damage.text = gameManager.status[Status.Damage].ToString("0.0#");
        aSpd.text = character.attackSpeed.ToString("0.#");
        spd.text = character.speed.ToString("0.##");
        cri.text = gameManager.status[Status.Critical].ToString("0.#");
    }

    void DashUI()
    {
        if (gameManager.dashCount <= 0)
        {
            if(dash.activeSelf)
                dash.SetActive(false);

            return;
        }

        dash.SetActive(true);
        dashKey.text = KeySetting.keys[KeyAction.DASH].ToString();
        Color color = dashImage.color;
        dashImage.fillAmount = 1;

        if (character.dashCount == 0)
        {
            color.a = 0.5f;
            dashImage.color = color;

            dashCountParent.gameObject.SetActive(false);

            dashCoolTime.gameObject.SetActive(true);
            dashImage.fillAmount = 1 - (character.dashCoolTime / character.initDashCoolTime);
            dashCoolTime.text = character.dashCoolTime.ToString("F2");
        }

        else if (character.dashCount > 0)
        {
            color.a = 1f;
            dashImage.color = color;

            dashCountParent.gameObject.SetActive(true);
            dashCount.text = character.dashCount.ToString();
            maxDashCount.text = gameManager.dashCount.ToString();

            dashCoolTime.gameObject.SetActive(false);
        }
    }

    void HpUI()
    {
        hpText.text = $"{character.currentHp} / {character.maxHp}";

        hpBar.value = (character.currentHp / character.maxHp);
    }

    public void RecoveryGauegeUI()
    {
        recoveryGaugeBar.value = Mathf.Clamp(character.currentRecoveryGauge / character.maxRecoveryGauge, 0f, 1f);
    }

    void ItemsCountUI()
    {
        woodCount.text = gameManager.woodCount.ToString();
        fish1Count.text = gameManager.fishLowGradeCount.ToString();
        fish2Count.text = gameManager.fishHighGradeCount.ToString();
    }

    void RoundUI()
    {
        roundText.text = gameManager.round != 0 ? gameManager.round.ToString() : "1";
    }

    void TimeUI()
    {
        if (gamesceneManager.currentGameTime >= 5)
        {
            if (timeText.color != initTimeColor)
                timeText.color = initTimeColor;

            timeText.text = gamesceneManager.currentGameTime > 0 ? ((int)gamesceneManager.currentGameTime).ToString() : "0.00";
        }

        else
        {
            if(timeText.color != Color.red)
            timeText.color = Color.red;

            timeText.text = (gamesceneManager.currentGameTime).ToString("F2");
        }
    }

    void WeaponUI()
    {
        if(gamesceneManager.isNight)
            weaponUI.SetActive(true);

        else
            weaponUI.SetActive(false);
    }

    public void OnOffStatWindow()
    {
        statWindow.gameObject.SetActive(!statWindow.gameObject.activeSelf);
    }

    public void SelectScene(string sceneName)
    {
        gameManager.ToNextScene(sceneName);
    }

    public void GetRandomSecne()
    {
        gameManager.woodCount -= 5;

        int rand = UnityEngine.Random.Range(0, 10);
        string nextScene;

        if (rand % 2 == 0)
            nextScene = "Fishing";

        else
            nextScene = "Logging";

        SelectScene(nextScene);
    }
}
