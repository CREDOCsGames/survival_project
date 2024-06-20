using UnityEngine;
using UnityEngine.UI;

public class FishingCard : ChestPassiveCard
{
    Fishing fishing;

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;
        fishing = Fishing.Instance;

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

    protected override void CardUniqueFunction()
    {
        fishing.Initialize();
    }
}

