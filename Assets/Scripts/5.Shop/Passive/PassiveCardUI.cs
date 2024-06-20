using UnityEngine;
using UnityEngine.UI;

public class PassiveCardUI : PassiveCard
{
    [Header("Lock")]
    [SerializeField] Image lockBackImage;
    [SerializeField] Image lockImage;
    [SerializeField] Text lockText;

    [Header("Card")]
    [SerializeField] Text itemPrice;

    Color LockImageColor;
    Color LockTextColor;

    [HideInInspector] public bool isLock;

    Color initPriceColor;

    int num;

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;
        initPriceColor = itemPrice.color;

        LockImageColor = new Color(0.17f, 0.17f, 0.17f);
        LockTextColor = Color.white;

        num = transform.parent.GetSiblingIndex();

        isLock = itemManager.cardLocks[num];

        Setting();
        CardImage();
        StatArray();
        DescriptionInfo();
        StartLockColor();

        for (int i = 0; i < passiveInfo.Length; i++)
        {
            if (passiveInfo[i].ItemName == selectedPassive.ItemName)
            {
                arrayCount = i;
                break;
            }
        }
    }

    private void Update()
    {
        lockImage.gameObject.SetActive(isLock);

        if (gameManager.money < price)
            itemPrice.color = Color.red;

        else if (gameManager.money >= price)
            itemPrice.color = initPriceColor;
    }

    protected override void Setting()
    {
        if (isLock)
            selectedPassive = itemManager.lockedPassCards[num];

        price = Mathf.CeilToInt(selectedPassive.ItemPrice * (1 - gameManager.salePercent));
        itemPrice.text = price.ToString();
        base.Setting();
        cardImageAlpha = 0.8235f;
    }

    protected override void GetPassiveItem()
    {
        SoundManager.Instance.PlayES("SelectButton");

        isLock = false;
        itemManager.cardLocks[num] = isLock;
        itemManager.lockedPassCards[num] = null;

        base.GetPassiveItem();
    }

    void Ddilpa()
    {
        // 마뎀(5) > 물뎀(4)
        gameManager.stats[4] += Mathf.Round((gameManager.stats[5] / 2) * 10) * 0.1f;
        gameManager.stats[5] = 0;
    }

    void Butterfly()
    {
        // 물뎀(4) > 마뎀(5)
        gameManager.stats[5] += Mathf.Round((gameManager.stats[4] / 2) * 10) * 0.1f;
        gameManager.stats[4] = 0;
    }

    public void Lock()
    {
        if (!isLock)
        {
            lockBackImage.color = Color.white;
            lockText.color = Color.black;
            itemManager.lockedPassCards[num] = selectedPassive;
            isLock = true;
        }

        else if (isLock)
        {
            lockBackImage.color = LockImageColor;
            lockText.color = LockTextColor;
            itemManager.lockedPassCards[num] = null;
            isLock = false;
        }

        itemManager.cardLocks[num] = isLock;
    }

    void StartLockColor()
    {
        if (isLock)
        {
            lockBackImage.color = Color.white;
            lockText.color = Color.black;
        }

        else if (!isLock)
        {
            lockBackImage.color = LockImageColor;
            lockText.color = LockTextColor;
        }
    }
}
