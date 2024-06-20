using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class OptionUI : MonoBehaviour
{
    protected Texture2D cursorNormal;
    protected Texture2D cursorAttack;
    [SerializeField] protected GameObject panel;
    [SerializeField] Slider AllSound;
    [SerializeField] Slider bgmSound;
    [SerializeField] Slider sfxSound;
    [SerializeField] GameObject wUnMark;
    [SerializeField] GameObject bUnMark;
    [SerializeField] GameObject sUnMark;
    [SerializeField] GameObject CheckHomePanel;
    [SerializeField] GameObject statPanel;
    [SerializeField] GameObject normalToggle;
    [SerializeField] GameObject doubleToggle;
    [SerializeField] protected GameObject backPanel;

    protected GameManager gameManager;
    SoundManager soundManager;
    LoggingLilpa lilpa;

    bool muteAllVolume;
    bool muteBgmVolume;
    bool muteSfxVolume;

    private void Start()
    {
        panel.SetActive(false);
        CheckHomePanel.SetActive(false);

        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;

        if (LoggingLilpa.Instance != null)
            lilpa = LoggingLilpa.Instance;

        cursorNormal = gameManager.useCursorNormal;
        cursorAttack = gameManager.useCursorAttack;

        AllSound.value = 1 - PlayerPrefs.GetFloat("Sound_All");
        bgmSound.value = 1 - PlayerPrefs.GetFloat("Sound_Bgm");
        sfxSound.value = 1 - PlayerPrefs.GetFloat("Sound_Sfx");

        muteAllVolume = Convert.ToBoolean(PlayerPrefs.GetInt("Mute_All", 0));
        muteBgmVolume = Convert.ToBoolean(PlayerPrefs.GetInt("Mute_Bgm", 0));
        muteSfxVolume = Convert.ToBoolean(PlayerPrefs.GetInt("Mute_Sfx", 0));

        wUnMark.SetActive(muteAllVolume);
        bUnMark.SetActive(muteBgmVolume);
        sUnMark.SetActive(muteSfxVolume);

        normalToggle.SetActive(!Convert.ToBoolean(PlayerPrefs.GetInt("CursorSize", 0)));
        doubleToggle.SetActive(!normalToggle.activeSelf);
    }

    private void Update()
    {
        wUnMark.SetActive(muteAllVolume);
        bUnMark.SetActive(muteBgmVolume);
        sUnMark.SetActive(muteSfxVolume);

        if (lilpa != null)
        {
            if (lilpa.isCanControl)
            {
                if (Input.GetKeyDown(KeyCode.Escape) && !gameManager.isPause)
                    PauseGame();

                else if (Input.GetKeyDown(KeyCode.Escape) && gameManager.isPause)
                    ReturnToGame();
            }
        }

        else
        {
            if (!gameManager.isTuto)
            {
                if (Input.GetKeyDown(KeyCode.Escape) && !gameManager.isPause)
                    PauseGame();

                else if (Input.GetKeyDown(KeyCode.Escape) && gameManager.isPause)
                    ReturnToGame();
            }
        }
    }

    public void CursorSizeSetting(int num)
    {
        PlayerPrefs.SetInt("CursorSize", num);
        normalToggle.SetActive(!Convert.ToBoolean(PlayerPrefs.GetInt("CursorSize", 0)));
        doubleToggle.SetActive(!normalToggle.activeSelf);
        gameManager.cursorSize = num;
        gameManager.useCursorNormal = gameManager.cursorNormal[num];
        gameManager.useCursorAttack = gameManager.cursorAttack[num];
        cursorNormal = gameManager.useCursorNormal;
        cursorAttack = gameManager.useCursorAttack;
        Texture2D useCursorNormal = gameManager.useCursorNormal;
        Vector2 cursorHotSpot = new Vector3(useCursorNormal.width * 0.5f, useCursorNormal.height * 0.5f);
        Cursor.SetCursor(useCursorNormal, cursorHotSpot, CursorMode.ForceSoftware);
    }

    public void ChangeWholeVolume()
    {
        if (panel.activeSelf)
        {
            if (AllSound.value == 1)
            {
                muteAllVolume = true;
                muteBgmVolume = muteAllVolume;
                muteSfxVolume = muteAllVolume;
            }

            else
            {
                muteAllVolume = false;
                muteBgmVolume = muteAllVolume;
                muteSfxVolume = muteAllVolume;
            }

            soundManager.WholeVolume((1 - AllSound.value), muteBgmVolume, muteSfxVolume);
        }
    }

    public void OnOffWholeVolume()
    {
        if (AllSound.value != 1)
            muteAllVolume = !muteAllVolume;

        if (bgmSound.value != 1)
            muteBgmVolume = muteAllVolume;

        if (sfxSound.value != 1)
            muteSfxVolume = muteAllVolume;

        soundManager.WholeVolumeOnOff(muteAllVolume);
    }

    public void ChangeBgmVolume()
    {
        if (panel.activeSelf)
        {
            if (bgmSound.value == 1)
                muteBgmVolume = true;

            else
                muteBgmVolume = false;

            soundManager.BgmVolume((1 - bgmSound.value), muteBgmVolume);
        }
    }

    public void OnOffBgmVolume()
    {
        muteBgmVolume = !muteBgmVolume;
        soundManager.BgmOnOff(muteBgmVolume);

        if (muteBgmVolume == false)
            muteAllVolume = false;
    }

    public void ChangeSfxVolume()
    {
        if (panel.activeSelf)
        {
            if (sfxSound.value == 1)
                muteSfxVolume = true;

            else
                muteSfxVolume = false;

            soundManager.EsVolume((1 - sfxSound.value), muteSfxVolume);
        }
    }

    public void OnOffSfxVolume()
    {
        muteSfxVolume = !muteSfxVolume;
        soundManager.SfxOnOff(muteSfxVolume);

        if (muteSfxVolume == false)
            muteAllVolume = false;
    }

    virtual public void PauseGame()
    {
        if (!gameManager.isPause)
        {
            Cursor.lockState = CursorLockMode.None;

            if (gameManager.currentScene == "Game")
            {
                Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
                Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);
            }

            if (backPanel != null)
                backPanel.SetActive(true);

            if (statPanel != null)
                statPanel.SetActive(true);

            panel.SetActive(true);
            gameManager.isPause = true;
            Time.timeScale = 0;
        }
    }

    virtual public void ReturnToGame()
    {
        if (gameManager.isPause)
        {
            Cursor.lockState = CursorLockMode.Confined;

            if (gameManager.currentScene == "Game")
            {
                if (gameManager.isBossDead && gameManager.isClear)
                {
                    Vector2 cursorHotSpot = new Vector3(cursorNormal.width * 0.5f, cursorNormal.height * 0.5f);
                    Cursor.SetCursor(cursorNormal, cursorHotSpot, CursorMode.ForceSoftware);
                }
                
                else
                {
                    Vector2 cursorHotSpot = new Vector3(cursorAttack.width * 0.5f, cursorAttack.height * 0.5f);
                    Cursor.SetCursor(cursorAttack, cursorHotSpot, CursorMode.ForceSoftware);
                }
            }

            if (backPanel != null)
                backPanel.SetActive(false);

            if (statPanel != null)
                statPanel.SetActive(false);

            panel.SetActive(false);
            gameManager.isPause = false;
            Time.timeScale = 1;
        }
    }

    public void OpenCheckHomePanel()
    {
        CheckHomePanel.SetActive(true);
    }

    public void CloseCheckHomePanel()
    {
        CheckHomePanel.SetActive(false);
    }

    public void TitleScene()
    {
        if (SceneManager.GetActiveScene().buildIndex > 2)
        {
            Destroy(ItemManager.Instance.gameObject);
        }

        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            Destroy(Character.Instance.gameObject);
        }

        if (soundManager.gameObject != null)
            Destroy(soundManager.gameObject);

        if (gameManager.gameObject != null)
            Destroy(gameManager.gameObject);

        SceneManager.LoadScene("StartTitle");
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("Mute_All", Convert.ToInt32(muteAllVolume));
        PlayerPrefs.SetInt("Mute_Bgm", Convert.ToInt32(muteBgmVolume));
        PlayerPrefs.SetInt("Mute_Sfx", Convert.ToInt32(muteSfxVolume));
    }
}
