using UnityEngine;

public class ShowPassive : Singleton<ShowPassive>
{
    [SerializeField] GameObject passiveCardUI;

    float[] weightPassiveValue;

    GameManager gameManager;
    ItemManager itemManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;

        weightPassiveValue = new float[4];

        ShowRandomPassiveCard();
    }

    public void ShowRandomPassiveCard()
    {
        float totalWeight = 0;

        weightPassiveValue[0] = 200f - (gameManager.round - 1f) * 6f;
        weightPassiveValue[1] = 10f * gameManager.round;
        weightPassiveValue[2] = (gameManager.round - 1f) * (gameManager.round) * 0.2f * (1f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.01f);
        weightPassiveValue[3] = (gameManager.round - 1f) * (gameManager.round) * 0.04f * (1f + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.01f);


        ChestPassiveCard passiveCard = passiveCardUI.GetComponent<ChestPassiveCard>();

        for (int i = 0; i < passiveCard.passiveInfo.Length; i++)
        {
            passiveCard.passiveInfo[i].weight = weightPassiveValue[(int)passiveCard.passiveInfo[i].ItemGrade];
            totalWeight += passiveCard.passiveInfo[i].weight;
        }

        float rand = Random.Range(0, totalWeight);
        float total = 0;

        for (int i = 0; i < passiveCard.passiveInfo.Length; i++)
        {
            total += passiveCard.passiveInfo[i].weight;

            if (rand <= total)
            {
                if (itemManager.passiveCounts[i] != 0)
                {
                    passiveCard.selectedPassive = passiveCard.passiveInfo[i];
                    break;
                }

                else
                {
                    rand = Random.Range(0, totalWeight);
                    total = 0;
                    i = 0;
                }
            }
        }

        Instantiate(passiveCardUI, transform.GetChild(1));
    }
}