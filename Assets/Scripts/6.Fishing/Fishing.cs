using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : Singleton<Fishing>
{
    [SerializeField] Slider catchBar;
    [SerializeField] Text roundText;
    [SerializeField] Text catchText;
    [SerializeField] Text maxFishingCount;
    [SerializeField] Text currentFishingCount;
    [SerializeField] Text[] catchItemsText;

    int[] catchItemsCount = { 0, 0, 0 };
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

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        maxFishCount = 5;
        currentFishCount = maxFishCount;

        roundText.text = $"{gameManager.round} 일차";

        maxFishingCount.text = maxFishCount.ToString();
        currentFishingCount.text = currentFishCount.ToString();

        for(int i=0;i<catchItemsText.Length; i++)
        {
            catchItemsText[i].text = catchItemsCount[i].ToString();
        }

        Initialize();
    }

    void Update()
    {
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

        rand += (int)((randSpeedRatio - 1) * 5);

        if (rand < 33)
        {
            catchItemsCount[0]++;
            catchItemsText[0].text = catchItemsCount[0].ToString();
            fishingAnim.isSomeCatch = 1;
        }

        else if (rand < 66)
        {
            catchItemsCount[1]++;
            catchItemsText[1].text = catchItemsCount[1].ToString();
            fishingAnim.isSomeCatch = 1;
        }

        else
        {
            catchItemsCount[2]++;
            catchItemsText[2].text = catchItemsCount[2].ToString();
            fishingAnim.isSomeCatch = 2;
        }
    }

    public IEnumerator FishingEnd()
    {
        currentFishCount--;
        currentFishingCount.text = currentFishCount.ToString();

        yield return new WaitForSeconds(2f);

        if (currentFishCount <= 0)
        {
            Character.Instance.isCanControll = true;
            gameManager.fish1 = catchItemsCount[0];
            gameManager.fish2 = catchItemsCount[1];
            gameObject.SetActive(false);
        }

        Initialize();
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
