using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneUI : Singleton<GameSceneUI>
{
    Texture2D cursorAttack;
    Texture2D cursorNormal;

    [SerializeField] public GameObject monsterSpawn;
    [SerializeField] GameObject tutoPanel;
    [SerializeField] GameObject selectPanel;
    public GameObject tamingGame;

    [Header("HP")]
    [SerializeField] Text hpText;
    [SerializeField] Slider hpBar;
    [SerializeField] Slider recoveryGaugeBar;

    [Header("COIN")]
    [SerializeField] Text coinText;

    [Header("Items")]
    [SerializeField] Text woodCount;
    [SerializeField] Text fish1Count;
    [SerializeField] Text fish2Count;

    [Header("Time")]
    [SerializeField] Text timeText;

    [Header("Round")]
    [SerializeField] Text roundText;
    [SerializeField] Text dayNightAlarmText;

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

    [Header("Text")]
    [SerializeField] GameObject roundClearText;
    [SerializeField] Text bossSceneText;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] TypingText gameOverText;
    [SerializeField] GameObject gameOverWoodUI;
    [SerializeField] TypingText gameOverWoodText;
    [SerializeField] GameObject gameOverIsedolUI;
    [SerializeField] TypingText gameOverIsedolText;
    [SerializeField] Image clearImage;
    [SerializeField] Sprite[] clearIllusts;
    [SerializeField] GameObject clickText;
    [SerializeField] GameObject gameClearUI;
    [SerializeField] TypingText gameClearText;

    GameManager gameManager;
    Character character;
    SoundManager soundManager;
    GamesceneManager gamesceneManager;

    [SerializeField] GameObject fishingGame;
    [SerializeField] GameObject weaponUI;

    bool bgmChange;

    Color initTimeColor;

    protected override void Awake()
    {
        base.Awake();
        roundClearText.SetActive(false);
        bossSceneText.gameObject.SetActive(false);
        gameOverUI.SetActive(false);
        gameOverWoodUI.SetActive(false);
        gameOverIsedolUI.SetActive(false);
        gameClearUI.SetActive(false);
        dash.SetActive(false);
        statWindow.SetActive(false);
        clickText.gameObject.SetActive(false);
        clearImage.gameObject.SetActive(false);
        tutoPanel.SetActive(false);
        selectPanel.SetActive(false);
        tamingGame.SetActive(false);
        fishingGame.SetActive(true);
        dayNightAlarmText.gameObject.SetActive(false);
        weaponUI.SetActive(false);

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

        cursorNormal = gameManager.useCursorNormal;
        cursorAttack = gameManager.useCursorAttack;
        Vector2 cursorHotSpot = new Vector3(cursorAttack.width * 0.5f, cursorAttack.height * 0.5f);
        Cursor.SetCursor(cursorAttack, cursorHotSpot, CursorMode.ForceSoftware);

        bgmChange = false;

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
    
    private void Update()
    {
        HpUI();
        RecoveryGauegeUI();
        CoinUI();
        ItemsCountUI();
        RoundUI();
        TimeUI();
        DashUI();
        WeaponUI();
        SettingStatText();

        if (!character.isDead)
        {
            if (gameManager.round != 30)
            {
                if (gameManager.isClear && gameManager.isBossDead)
                {
                    if (!bgmChange)
                    {
                        Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
                        Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);

                        soundManager.PlayBGM(5, false);
                        bgmChange = true;
                    }

                    roundClearText.SetActive(true);
                }

                if (roundClearText.GetComponent<TypingText>().isOver == true)
                {
                    roundClearText.SetActive(false);
                }
            }

            else if (gameManager.round == 30)
            {
                if (gameManager.isBossDead)
                {
                    gameManager.isClear = true;

                    if (!gameClearUI.activeSelf)
                        gameManager.gameEndTime = Time.realtimeSinceStartup;

                    if (gameManager.woodCount >= gameManager.woodMaxCount)
                    {
                        if (!bgmChange)
                        {
                            Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
                            Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);

                            soundManager.PlayBGM(6, false);
                            bgmChange = true;
                        }

                        gameClearUI.SetActive(true);

                        clearImage.sprite = clearIllusts[0];

                        if (gameClearText.isOver)
                        {
                            StartCoroutine(FadeIn());
                            gameClearText.isOver = false;
                        }
                    }

                    else if (gameManager.woodCount < gameManager.woodMaxCount)
                    {
                        gameOverWoodUI.SetActive(true);

                        if (gameOverWoodText.isOver)
                            SceneManager.LoadScene("End");

                        /*if (gameManager.isedolCount != 5)
                        {
                            gameOverWoodUI.SetActive(true);

                            if (gameOverWoodText.isOver)
                                SceneManager.LoadScene("End");
                        }

                        else
                        {
                            gameOverWoodUI.SetActive(true);

                            gameManager.isClear = true;

                            if (gameOverWoodText.isOver)
                            {
                                gameOverWoodUI.SetActive(false);
                                gameOverIsedolUI.SetActive(true);
                            }

                            if (gameOverIsedolText.isOver == true)
                            {
                                clearImage.sprite = clearIllusts[1];
                                StartCoroutine(FadeIn());
                                gameOverIsedolText.isOver = false;
                            }

                            if (clickText.gameObject.activeSelf)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    if (!bgmChange)
                                    {
                                        soundManager.PlayBGM(6, false);
                                        bgmChange = true;
                                    }

                                    clearImage.gameObject.SetActive(false);
                                    gameOverIsedolUI.SetActive(false);
                                    gameClearUI.SetActive(true);
                                }
                            }

                            if (gameClearText.isOver == true && !soundManager.isPlaying)
                                SceneManager.LoadScene("End");
                        }*/
                    }
                }

                if (clickText.gameObject.activeSelf)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        clearImage.gameObject.SetActive(false);
                        SceneManager.LoadScene("End");
                    }
                }
            }
        }

        else if (character.isDead)
        {
            if (!gameOverUI.activeSelf || !gameOverIsedolText)
                gameManager.gameEndTime = Time.realtimeSinceStartup;

            if (clickText.gameObject.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!bgmChange)
                    {
                        soundManager.PlayBGM(6, false);
                        bgmChange = true;
                    }

                    clearImage.gameObject.SetActive(false);
                    gameOverIsedolUI.SetActive(false);
                    gameClearUI.SetActive(true);
                }
            }

            if (gameClearText.isOver == true && !soundManager.isPlaying)
                SceneManager.LoadScene("End");
        }
    }

    bool CursorChange(int playNum)
    {
        Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
        Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);

        soundManager.PlayBGM(playNum, false);
        bgmChange = true;

        return false;
    }

    IEnumerator FadeIn()
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
    }

    void SettingStatText()
    {
        maxHp.text = gameManager.maxHp.ToString();
        reHp.text = gameManager.recoverHp.ToString("0.#");
        apHp.text = gameManager.absorbHp.ToString("0.#");
        def.text = gameManager.defence.ToString("0.#");
        avoid.text = gameManager.avoid.ToString("0.#");
        percentDamage.text = gameManager.percentDamage.ToString("0.0#");
        aSpd.text = gameManager.attackSpeed.ToString("0.#");
        spd.text = gameManager.speed.ToString("0.##");
        ran.text = gameManager.range.ToString("0.#");
        luk.text = gameManager.luck.ToString("0.#");
        cri.text = gameManager.critical.ToString("0.#");
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

    void CoinUI()
    {
        coinText.text = gameManager.money.ToString();
    }

    void ItemsCountUI()
    {
        woodCount.text = gameManager.woodCount.ToString();
        fish1Count.text = gameManager.fishLowGradeCount.ToString();
        fish2Count.text = gameManager.fishHighGradeCount.ToString();
    }

    void RoundUI()
    {
        roundText.text = gameManager.round == 0 ? gameManager.round.ToString() : "1";
    }

    void TimeUI()
    {
        if (gamesceneManager.currentGameTime >= 5)
        {
            if (timeText.color != initTimeColor)
                timeText.color = initTimeColor;

            timeText.text = ((int)gamesceneManager.currentGameTime).ToString();
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

    public void DayNightAlarmUpdate(bool isNight)
    {
        dayNightAlarmText.gameObject.SetActive(true);
        
        dayNightAlarmText.text = isNight ? "밤이 되었습니다." : "아침이 밝았습니다.";

        StartCoroutine(FadeAwayAlarmText(1.5f));
    }

    IEnumerator FadeAwayAlarmText(float time)
    {
        Color textColor = dayNightAlarmText.color;
        float initTime = time;

        while(dayNightAlarmText.color.a > 0)
        {
            time -= Time.deltaTime;
            textColor.a = time / initTime;
            dayNightAlarmText.color = textColor;

            yield return null;
        }

        yield return null;

        textColor.a = 1;
        dayNightAlarmText.color = textColor;
        dayNightAlarmText.gameObject.SetActive(false);
    }
}
