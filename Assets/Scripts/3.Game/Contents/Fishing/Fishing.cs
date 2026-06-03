using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{
    [SerializeField] GameObject catchBar;
    [SerializeField] Text catchText;
    [SerializeField] Text maxFishingCount;
    [SerializeField] Text currentFishingCount;
    [SerializeField] Text[] catchItemsText;
    [SerializeField] Image catchFishImage;
    [SerializeField] Sprite[] fishTypeImage;
    [SerializeField] GameObject pieceCard;
    [SerializeField] DiabolicItemInfo[] fishingPieceList;
    [SerializeField] GameObject clickUI;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip failSound;

    [Header("Bar Physics (normalized units)")]
    [SerializeField] float riseAccel = 18f;  // 누를 때 가속(위)
    [SerializeField] float gravity = 28f;  // 놓을 때 가속(아래)
    [SerializeField] float damping = 10f;  // 감쇠
    [SerializeField] float maxRise = 12f;  // 상승 속도 제한
    [SerializeField] float maxFall = 14f;  // 하강 속도 제한

    [Header("UI Refs")]
    [SerializeField] RectTransform playArea;   // CatchArea
    [SerializeField] RectTransform playerBar;  // PlayerBar
    [SerializeField] RectTransform fish;       // Fish
    [SerializeField] Image gauge;

    [Header("")]
    [Range(0.05f, 0.8f)] public float barBaseLength = 0.25f;
    float barLength;

    [SerializeField, Range(0f, 1f)] float barY = 0.5f;
    float barV = 0f; // normalized velocity

    [SerializeField] public List<FishDifficulty> fishMoves;
    float fishY = 0.5f;
    FishMove fishMove;
    int rewardIndex = 0;

    [Range(0f, 1f)] float catchingProgress = 0f;

    bool isCatch = false;
    [HideInInspector] public bool isCatchingStart = false;

    /// <summary> 최대 낚시 횟수 </summary>
    int maxFishCount;
    /// <summary> 남은 낚시 횟수 </summary>
    int currentFishCount;

    [SerializeField]FishingAnim fishingAnim;
    GameManager gameManager;
    GamesceneManager gamesceneManager;
    ItemManager itemManager;
    SoundManager soundManager;

    List<DiabolicItemInfo> itemList = new List<DiabolicItemInfo>();

    bool isPress = false;

    protected virtual void Awake()
    {
        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;
        itemManager = ItemManager.Instance;
        soundManager = SoundManager.Instance;

        pieceCard.SetActive(false);

        clickUI.SetActive(false);

        fishMove = new();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        maxFishCount = 55;
#else
            maxFishCount = 10;
#endif
        currentFishCount = maxFishCount;

        maxFishingCount.text = maxFishCount.ToString();
        currentFishingCount.text = currentFishCount.ToString();

        barLength = gameManager.specialStatus[SpecialStatus.RustyHarpoon] ? barBaseLength * 2 : barBaseLength;

        Initialize();
    }

    private void OnDisable()
    {
        pieceCard.SetActive(false);
        catchFishImage.gameObject.SetActive(false);

        Character.Instance.isCanControll = true;

        Initialize();
    }

    void Update()
    {
        if (gameManager.isPause)
            return;

        if (gamesceneManager.isNight)
        {
            Character.Instance.isCanControll = true;
            gameObject.SetActive(false);

            Initialize();
        }


        bool held = Input.GetMouseButton(0);
        
        if (!isCatch && isCatchingStart && held)
        {
            isPress = true;
            SetReward();
            catchBar.gameObject.SetActive(true);
            isCatchingStart = false;
        }
        if (!catchBar.activeSelf)
            return;

        TickBar(Time.deltaTime, held);
        TickFish();
        CalculateProgress();
    }

    void LateUpdate()
    {
        ApplyToUI();
    }

    void ApplyToUI()
    {
        float h = playArea.rect.height;

        var size = playerBar.sizeDelta;
        size.y = barLength * h;
        playerBar.sizeDelta = size;

        playerBar.anchoredPosition = new Vector2(playerBar.anchoredPosition.x, barY * h);

        float fishY = fishMove.FishY;
        fish.anchoredPosition = new Vector2(fish.anchoredPosition.x, fishY * h);

        UpdateGauge();
    }

    void SetReward()
    {
        int rand = Random.Range(0, 100);

#if UNITY_EDITOR
        int high = gameManager.specialStatus[SpecialStatus.BaitWarm] ? 0 : 0;
#else
            int high = gameManager.specialStatus[SpecialStatus.BaitWarm] ? 60 : 80;
#endif
        high = 50;
        if (rand < high)
            rewardIndex = 0;
        else rewardIndex = 1;
    }

    void TickBar(float dt, bool held)
    {
        float ay = held ? riseAccel : -gravity;
        barV += ay * dt;
        barV *= Mathf.Exp(-damping * dt);
        barV = Mathf.Clamp(barV, -maxFall, maxRise);

        barY += barV * dt;

        float half = barLength * 0.5f;
        float minY = half;
        float maxY = 1f - half;

        if (minY > maxY)
        {
            barY = 0.5f;
            barV = 0f;
            return;
        }

        if (barY < minY) { barY = minY; if (barV < 0f) barV = 0f; }
        if (barY > maxY) { barY = maxY; if (barV > 0f) barV = 0f; }
    }
    private void TickFish()
    {
        fishMove.Update();
        fishY = fishMove.FishY;
    }
    void CalculateProgress()
    {
        if (catchingProgress > 0.9999f)
        {
            CatchFish(true);
            return;
        }
        else if (catchingProgress < 0.0001f)
        {
            CatchFish(false);
            return;
        }
        if (IsFishTouchingBar())
            catchingProgress += Time.deltaTime * 0.3f;
        else 
            catchingProgress -= Time.deltaTime * 0.1f;

    }
    void UpdateGauge()
    {
        gauge.fillAmount = catchingProgress;
    }

    bool IsFishTouchingBar()
    {
        float barHalf = barLength * 0.5f;

        float barBottom = barY - barHalf;
        float barTop = barY + barHalf;
        
        float fishHalf = GetFishHalfHeightNorm();
        float fishBottom = fishY - fishHalf;
        float fishTop = fishY + fishHalf;

        // 1D overlap
        return fishTop >= barBottom && fishBottom <= barTop;
    }
    float GetFishHalfHeightNorm()
    {
        float areaH = playArea.rect.height;
        if (areaH <= 0.0001f) return 0f;

        return (fish.rect.height / areaH) * 0.5f;
    }

    void CatchFish(bool success)
    {
        isCatch = true;
        fishingAnim.isCatch = true;

        if (success)
        {
            GetItem();
            fishingAnim.CatchSuccess = true;
            catchText.text = $"<color=#B09F5E>낚아챘다!</color>";
            soundManager.PlaySFX(successSound);
        }
        else
        {
            fishingAnim.CatchSuccess = false;
            catchText.text = $"<color=#A52D39>놓쳤다...</color>";
            soundManager.PlaySFX(failSound);
        }
        isPress = false;
        catchText.gameObject.SetActive(true);
        catchBar.SetActive(false);
    }

    void GetItem()
    {
        //        int rand = Random.Range(0, 100);

        //#if UNITY_EDITOR
        //        int high = gameManager.specialStatus[SpecialStatus.BaitWarm] ? 0 : 0;
        //#else
        //            int high = gameManager.specialStatus[SpecialStatus.BaitWarm] ? 60 : 80;
        //#endif

        switch (rewardIndex)
        {
            case 0:
                gameManager.fishLowGradeCount++;
                GetExtraFishBySubstatus(true);

                catchFishImage.sprite = fishTypeImage[0];
                break;
            case 1:
                gameManager.fishHighGradeCount++;
                GetExtraFishBySubstatus(false);

                catchFishImage.sprite = fishTypeImage[1];

                int rand = Random.Range(0, 100);

#if UNITY_EDITOR
                if (rand >= 0)
                {
                    GetRandomPiece();
                }
#else
                if (rand >= 100 - gameManager.pieceCardGetRate)
                {
                    GetRandomPiece();
                }
#endif
                break;

            default:
                Debug.Log("out of index");
                break;

        }
//        if (rand < high)
//        {
//            gameManager.fishLowGradeCount++;
//            GetExtraFishBySubstatus(true);

//            catchFishImage.sprite = fishTypeImage[0];
//        }

//        else
//        {
//            gameManager.fishHighGradeCount++;
//            GetExtraFishBySubstatus(false);

//            catchFishImage.sprite = fishTypeImage[1];

//            int rand = Random.Range(0, 100);

//#if UNITY_EDITOR
//            if (rand >= 0)
//            {
//                GetRandomPiece();
//            }
//#else
//                if (rand >= 100 - gameManager.pieceCardGetRate)
//                {
//                    GetRandomPiece();
//                }
//#endif
//        }
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
                {
                    itemList.Add(fishingPieceList[i]);
                }
            }
        }

        if (itemList.Count <= 0)
        {
            return;
        }

        int totalWeightValue = 0;

        for (int i = 0; i < itemList.Count; ++i)
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
    private void GetExtraFishBySubstatus(bool isLowGradeFish)
    {
        float addChance = gameManager.substatus[SubStatus.Fishing_AdditionalChance];
        int addAmount = (int)gameManager.substatus[SubStatus.Fishing_AdditionalAmount];
        int addAmountByChance = (int)gameManager.substatus[SubStatus.Fishing_AdditionalAmount_ByChance];

        int extraCount = 0;

        if (addChance > 99.99f)
        {
            extraCount += addAmount;
        }

        float rand = Random.Range(0f, 100f);
        if (addChance > rand)
        {
            extraCount += addAmountByChance;
        }

        if (extraCount == 0)
            return;

        if (isLowGradeFish)
            gameManager.fishLowGradeCount += extraCount;
        else
            gameManager.fishHighGradeCount += extraCount;
    }
    void SetFishDifficulty()
    {
        if (fishMoves == null || fishMoves.Count == 0)
        {
            Debug.LogError("Fish presets list is empty");
            return;
        }

        fishMove.SetPreset(fishMoves[rewardIndex]);
    }

    public IEnumerator CatchingStart()
    {
        clickUI.SetActive(true);
        isPress = false;
       
        yield return CoroutineCaching.WaitForSeconds(0.7f);

        clickUI.SetActive(false);
        SetFishDifficulty();

        if (isPress)
            yield break;

        isCatch = true;
        fishingAnim.isCatch = true;

        fishingAnim.CatchSuccess = false;
        catchText.text = $"<color=#A52D39>놓쳤다...</color>";
        soundManager.PlaySFX(failSound);

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
        fishMove.OnEnable();

        if (currentFishCount > 0)
        {
            fishingAnim.isCatch = false;
        }

        catchingProgress = 0.3f;
    }
}
