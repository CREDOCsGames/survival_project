using UnityEngine;
using UnityEngine.UI;

public class ChestPassiveCard : PassiveCard
{
    [Header("Card")]
    [SerializeField] protected Text sellPriceText;

    protected int sellPrice;

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;

        Setting();
        CardImage();
        StatArray();
        DescriptionInfo();

        for (int i = 0; i < passiveInfo.Length; i++)
        {
            if (passiveInfo[i].ItemName == selectedPassive.ItemName)
            {
                arrayCount = i;
                break;
            }
        }
    }

    protected override void Setting()
    {
        base.Setting();
        price = 0;
        sellPrice = Mathf.CeilToInt(selectedPassive.ItemPrice * 0.7f);
        sellPriceText.text = sellPrice.ToString();
        cardImageAlpha = 1f;
    }

    protected override void GetPassiveItem()
    {
        SoundManager.Instance.PlayES("ChestSelect");
        base.GetPassiveItem();
        CardUniqueFunction();
    }

    protected virtual void CardUniqueFunction()
    {
        GameSceneUI.Instance.chestCount--;

        if (GameSceneUI.Instance.chestCount > 0)
            ShowPassive.Instance.ShowRandomPassiveCard();
    }

    public void Sell()
    {
        SoundManager.Instance.PlayES("Coin");

        itemManager.passiveCounts[arrayCount]--;
        Destroy(gameObject);
        gameManager.money += sellPrice;
        CardUniqueFunction();
    }
}
