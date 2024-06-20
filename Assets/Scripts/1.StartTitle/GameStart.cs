using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField] GameObject damageUI;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject gameInfoPanel;
    [SerializeField] GameObject etcInfoPanel;
    SoundManager soundManager;

    protected IObjectPool<DamageUI> damagePool;

    protected virtual void Awake()
    {
        damagePool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 20);
    }

    private void Start()
    {
        damagePool.Get();
        optionPanel.SetActive(false);
        gameInfoPanel.SetActive(false);
        etcInfoPanel.SetActive(false);

        soundManager = SoundManager.Instance;
        soundManager.PlayBGM(0, true);
    }

    public void ClickStart(string scene)
    {
        soundManager.PlayES("StartButton");
        SceneManager.LoadScene(scene);
    }

    public void ClickOption()
    {
        soundManager.PlayES("SelectButton");
        optionPanel.SetActive(true);
    }

    public void ClickExit()
    {
        soundManager.PlayES("SelectButton");
        Application.Quit();
    }

    public void OnSelectSound()
    {
        soundManager.PlayES("SelectButton");
    }

    private DamageUI CreateDamageUI()
    {
        DamageUI damageUIPool = Instantiate(damageUI, Vector3.zero, Quaternion.Euler(90, 0, 0)).GetComponent<DamageUI>();
        damageUIPool.SetManagedPool(damagePool);
        return damageUIPool;
    }

    private void OnGetDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(true);
    }

    private void OnReleaseDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(false);
    }

    private void OnDestroyDamageUI(DamageUI damageUIPool)
    {
        Destroy(damageUIPool.gameObject);
    }
}
