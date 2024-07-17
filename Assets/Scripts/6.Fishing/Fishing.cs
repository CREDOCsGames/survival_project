using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fishing : Singleton<Fishing>
{
    [SerializeField] Slider catchBar;
    [SerializeField] Text roundText;
    [SerializeField] Text catchText;
    [SerializeField] Text maxFishingCount;
    [SerializeField] Text currentFishingCount;

    bool isCatch = false;
    [HideInInspector] public bool isCatchingStart = false;
    
    int maxFishCount;
    int currentFishCount;

    GameManager gameManager;
    FishingAnim fishingAnim;

    float randSpeedRatio;

    protected override void Awake()
    {
        base.Awake();

        gameManager = GameManager.Instance;
        fishingAnim = FishingAnim.Instance;
    }

    private void OnEnable()
    {
        maxFishCount = 5;
        currentFishCount = maxFishCount;

        roundText.text = string.Format($"{gameManager.round} + 일차");

        maxFishingCount.text = maxFishCount.ToString();
        currentFishingCount.text = currentFishCount.ToString();

        Initialize();
    }

    void Update()
    {
        if(currentFishCount <= 0)
            gameObject.SetActive(false);

        if (!isCatch && isCatchingStart)
        {
            /*if (Input.GetMouseButtonUp(0))
            {*/
                catchBar.gameObject.SetActive(true);
                isCatchingStart = false;

                randSpeedRatio = Random.Range(1, 5);
            //}
        }

        MoveBar();
    }

    void MoveBar()
    {
        if (!catchBar.IsActive())
            return;

        if (!isCatch && !isCatchingStart)
        {
            if (catchBar.value >= 300)
            {
                isCatch = true;
                fishingAnim.isCatch = true;

                catchText.text = "놓쳤다...";
                catchText.color = Color.red;
                fishingAnim.isSomeCatch = 0;

                catchText.gameObject.SetActive(true);
            }

            else
            {
                if (Input.GetMouseButton(0))
                    catchBar.value += Time.deltaTime * 200 * randSpeedRatio;

                else if (Input.GetMouseButtonUp(0))
                {
                    isCatch = true;
                    fishingAnim.isCatch = true;

                    if (catchBar.value < 280)
                    {
                        catchText.text = "놓쳤다...";
                        catchText.color = Color.red;
                        fishingAnim.isSomeCatch = 0;
                    }

                    else if (catchBar.value >= 280 && catchBar.value < 300)
                    {
                        catchText.text = "낚아챘다!";
                        catchText.color = Color.yellow;
                        GetItem();
                    }

                    catchText.gameObject.SetActive(true);
                }
            }
        }
    }

    void GetItem()
    {
        int rand = Random.Range(0, 100);

        if(rand <95)
            fishingAnim.isSomeCatch = 1;

        else
            fishingAnim.isSomeCatch = 2;
    }

    public IEnumerator FishingEnd()
    {
        yield return new WaitForSeconds(2f);

        Initialize();
        currentFishCount--;
    }

    public void Initialize()
    {
        catchBar.gameObject.SetActive(false);
        catchText.gameObject.SetActive(false);
        isCatch = false;
        isCatchingStart = false;

        if (currentFishCount > 0)
        {
            catchBar.value = 0;

            fishingAnim.isCatch = false;
        }
    }
}
