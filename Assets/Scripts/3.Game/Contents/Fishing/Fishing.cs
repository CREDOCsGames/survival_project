using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject pieceCard;
    [SerializeField] DiabolicItemInfo[] fishingPieceList;
    [SerializeField] GameObject clickUI;

    bool isCatch = false;
    [HideInInspector] public bool isCatchingStart = false;
    
    int maxFishCount;
    int currentFishCount;

    GameManager gameManager;
    GamesceneManager gamesceneManager;
    FishingAnim fishingAnim;
    ItemManager itemManager;

    float randSpeedRatio;

    float catchBarWidth;
    float catchPointStart;
    float catchPointEnd;

    float initCatchPointStart;
    float initCatchPointEnd;

    List<DiabolicItemInfo> itemList = new List<DiabolicItemInfo>();

    bool isPress = false;

    protected override void Awake()
    {
        base.Awake();

        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;
        fishingAnim = FishingAnim.Instance;
        itemManager = ItemManager.Instance;

        pieceCard.SetActive(false);

        catchBarWidth = catchBar.GetComponent<RectTransform>().sizeDelta.x;

        initCatchPointStart = catchPoint.offsetMin.x;
        initCatchPointEnd = -catchPoint.offsetMax.x;

        catchPointEnd = -catchPoint.offsetMax.x;

        clickUI.SetActive(false);
    }

    private void OnEnable()
    {
        maxFishCount = 5;
        currentFishCount = maxFishCount;

        maxFishingCount.text = maxFishCount.ToString();
        currentFishingCount.text = currentFishCount.ToString();

        catchPointStart = gameManager.specialStatus[SpecialStatus.RustyHarpoon] ? initCatchPointStart - (catchBarWidth - initCatchPointEnd - initCatchPointStart) : initCatchPointStart;

        catchPoint.offsetMin = new Vector2(catchPointStart, catchPoint.offsetMin.y);

        Initialize();
    }

    private void OnDisable()
    {
        pieceCard.SetActive(false);

        Character.Instance.isCanControll = true;

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

            randSpeedRatio = Random.Range(1f, 3f);
        }

        MoveBar();
    }

    void MoveBar()
    {
        if (!catchBar.IsActive())
            return;

        if (!isCatch && !isCatchingStart)
        {
            if (Input.GetMouseButtonUp(0))
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

            else if (Input.GetMouseButton(0))
            {
                if(!isPress)
                {
                    clickUI.SetActive(false);
                }

                isPress = true;

                if (Input.GetMouseButtonUp(0))
                    return;

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
        }
    }

    void GetItem()
    {
        int rand = Random.Range(0, 100);

        rand += (int)((randSpeedRatio - 1) * 5);

        int high = gameManager.specialStatus[SpecialStatus.BaitWarm] ? 65 : 85;

        if (rand < high)
        {
            gameManager.fishLowGradeCount++;
            catchFishImage.sprite = fishTypeImage[0];
        }

        else
        {
            gameManager.fishHighGradeCount++;
            catchFishImage.sprite = fishTypeImage[1];

            rand = Random.Range(0, 100);

            if (rand >= 96)
            {
                GetRandomPiece();
            }
        }
    }

    void GetRandomPiece()
    {
        itemList.Clear();

        for (int i = 0; i < fishingPieceList.Length; ++i)
        {
            if (!itemManager.getItems.ContainsKey(fishingPieceList[i]))
            {
                itemList.Add(fishingPieceList[i]);
            }

            else
            {
                if (itemManager.getItems[fishingPieceList[i]] < fishingPieceList[i].MaxCount)
                    itemList.Add(fishingPieceList[i]);
            }
        }

        if (itemList.Count <= 0)
        {
            return;
        }

        int totalWeightValue = 0;

        for (int i = 0; i<itemList.Count; ++i)
        {
            totalWeightValue += itemList[i].WeightValue;
        }

        int rand = Random.Range(0, totalWeightValue);

        float total = 0;

        for (int i = 0; i < itemList.Count; i++)
        {
            total += itemList[i].WeightValue;

            if (rand < total)
            {
                rand = i;
                break;
            }
        }

        pieceCard.GetComponent<PieceCard>().GetRandomItem(itemList[rand]);
        pieceCard.gameObject.SetActive(true);
        ItemManager.Instance.AddItem(itemList[rand]);
    }

    public IEnumerator CatchingStart()
    {
        clickUI.SetActive(true);
        isPress = false;

        yield return CoroutineCaching.WaitForSeconds(0.7f);

        clickUI.SetActive(false);

        if (isPress)
            yield break;

        isCatch = true;
        fishingAnim.isCatch = true;

        fishingAnim.CatchSuccess = false;
        catchText.text = "놓쳤다...";
        catchText.color = Color.red;

        catchText.gameObject.SetActive(true);
    }

    public IEnumerator FishingEnd()
    {
        currentFishCount--;
        currentFishingCount.text = currentFishCount.ToString();

        yield return CoroutineCaching.WaitForSeconds(2f);

        pieceCard.SetActive(false);

        if (currentFishCount <= 0)
        {
            Character.Instance.isCanControll = true;
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
