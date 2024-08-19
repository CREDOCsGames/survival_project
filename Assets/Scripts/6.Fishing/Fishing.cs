using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : Singleton<Fishing>
{
    [SerializeField] Slider catchBar;
    [SerializeField] RectTransform catchPoint;
    [SerializeField] Text catchText;
    [SerializeField] Text maxFishingCount;
    [SerializeField] Text currentFishingCount;
    [SerializeField] Text[] catchItemsText;
    [SerializeField] Image catchFishImage;
    [SerializeField] Sprite[] fishTypeImage;

    int[] catchItemsCount = { 0, 0, 0 };
    bool isCatch = false;
    [HideInInspector] public bool isCatchingStart = false;
    
    int maxFishCount;
    int currentFishCount;

    GameManager gameManager;
    GamesceneManager gamesceneManager;
    FishingAnim fishingAnim;

    float randSpeedRatio;

    float catchBarWidth;
    float catchPointStart;
    float catchPointEnd;

    float initCatchPointStart;
    float initCatchPointEnd;

    protected override void Awake()
    {
        base.Awake();

        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;
        fishingAnim = FishingAnim.Instance;

        gameObject.SetActive(false);
        catchBarWidth = catchBar.GetComponent<RectTransform>().sizeDelta.x;

        initCatchPointStart = catchPoint.offsetMin.x;
        initCatchPointEnd = -catchPoint.offsetMax.x;

        catchPointEnd = -catchPoint.offsetMax.x;
    }

    private void OnEnable()
    {
        maxFishCount = 5;
        currentFishCount = maxFishCount;

        maxFishingCount.text = maxFishCount.ToString();
        currentFishingCount.text = currentFishCount.ToString();

        for (int i = 0; i < catchItemsText.Length; i++)
        {
            catchItemsText[i].text = catchItemsCount[i].ToString();
        }

        catchPointStart = gameManager.specialStatus[SpecialStatus.RustyHarpoon] ? initCatchPointStart - (catchBarWidth - initCatchPointEnd - initCatchPointStart) : initCatchPointStart;

        catchPoint.offsetMin = new Vector2(catchPointStart, catchPoint.offsetMin.y);

        Initialize();
    }

    void Update()
    {
        if (gamesceneManager.isNight)
        {
            Character.Instance.isCanControll = true;
            gameObject.SetActive(false);

            Initialize();
        }

        if (!isCatch && isCatchingStart)
        {
            catchBar.gameObject.SetActive(true);
            isCatchingStart = false;

            randSpeedRatio = Random.Range(1, 5);
        }

        MoveBar();
    }

    void MoveBar()
    {
        if (!catchBar.IsActive())
            return;

        if (!isCatch && !isCatchingStart)
        {
            if (Input.GetMouseButton(0))
            {
                catchBar.value += Time.deltaTime * 200 * randSpeedRatio;

                if (catchBar.value >= catchBar.maxValue)
                {
                    isCatch = true;
                    fishingAnim.isCatch = true;

                    fishingAnim.CatchSuccess = false;
                    catchText.text = "놓쳤다...";
                    catchText.color = Color.red;

                    catchText.gameObject.SetActive(true);

                    return;
                }
            }

            else if (Input.GetMouseButtonUp(0))
            {
                isCatch = true;
                fishingAnim.isCatch = true;

                if (catchBar.value < catchPointStart || catchBar.value > catchBarWidth - catchPointEnd)
                {
                    fishingAnim.CatchSuccess = false;
                    catchText.text = "놓쳤다...";
                    catchText.color = Color.red;
                }

                else if (catchBar.value >= catchPointStart && catchBar.value <= catchBarWidth - catchPointEnd)
                {
                    GetItem();
                    fishingAnim.CatchSuccess = true;
                    catchText.text = "낚아챘다!";
                    catchText.color = Color.yellow;
                }

                catchText.gameObject.SetActive(true);
            }
        }
    }

    void GetItem()
    {
        int rand = Random.Range(0, 100);

        rand += (int)((randSpeedRatio - 1) * 5);

        int high = gameManager.specialStatus[SpecialStatus.BaitWarm] ? 46 : 66;

        if (rand < 33)
        {
            catchItemsCount[0]++;
            catchItemsText[0].text = catchItemsCount[0].ToString();
            catchFishImage.sprite = fishTypeImage[0];
        }

        else if (rand < high)
        {
            catchItemsCount[1]++;
            catchItemsText[1].text = catchItemsCount[1].ToString();
            catchFishImage.sprite = fishTypeImage[1];
        }

        else
        {
            catchItemsCount[2]++;
            catchItemsText[2].text = catchItemsCount[2].ToString();
            catchFishImage.sprite = fishTypeImage[1];
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
            gameManager.fishLowGradeCount = catchItemsCount[0];
            gameManager.fishHighGradeCount = catchItemsCount[1];
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
