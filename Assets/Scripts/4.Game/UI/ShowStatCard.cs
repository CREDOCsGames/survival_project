using UnityEngine;
using UnityEngine.UI;

public class ShowStatCard : Singleton<ShowStatCard>
{
    [SerializeField] GameObject statCard;
    [SerializeField] Text rerollMoneyText;

    Stat[] statInfo;
    int[] numArray;

    GameObject[] statCards;

    public int rerollMoney;

    Color initPriceColor;

    [HideInInspector] public bool isSelected;

    float[] weightValue;

    GameManager gameManager;

    private void Start()
    {
        weightValue = new float[4];
        gameManager = GameManager.Instance;
        initPriceColor = rerollMoneyText.color;
        rerollMoney = -gameManager.round;

        statCards = new GameObject[4];
        statInfo = new Stat[gameManager.gameObject.GetComponent<StatCardInfo>().statInfos.Length];

        for (int i = 0; i < statInfo.Length; i++)
            statInfo[i] = gameManager.gameObject.GetComponent<StatCardInfo>().statInfos[i];

        numArray = new int[statInfo.Length];

        ShowRandomCards();
    }

    private void Update()
    {
        if (gameManager.money < -rerollMoney)
            rerollMoneyText.color = Color.red;

        else if (gameManager.money >= -rerollMoney)
            rerollMoneyText.color = initPriceColor;

        rerollMoneyText.text = rerollMoney.ToString();

        CardDestroy();
    }

    void CardDestroy()
    {
        for (int i = 1; i < 5; i++)
        {
            if (statCards[i - 1] != null)
            {
                if (isSelected == true)
                    Destroy(gameObject.transform.GetChild(i).GetChild(0).gameObject);
            }
        }

        isSelected = false;
    }

    public void Reroll()
    {
        SoundManager.Instance.PlayES("SelectButton");

        if (gameManager.money >= -rerollMoney)
        {
            for (int i = 1; i < this.transform.childCount - 1; i++)
                Destroy(this.transform.GetChild(i).GetChild(0).gameObject);

            gameManager.money += rerollMoney;
            rerollMoney--;

            ShowRandomCards();
        }
    }

    public void ShowRandomCards()
    {
        for (int j = 0; j < statCards.Length; j++)
        {
            numArray[j] = Random.Range(0, statInfo.Length);
            {
                for (int k = 0; k < j; k++)
                {
                    if (numArray[k] == numArray[j])
                    {
                        j--;
                        break;
                    }
                }
            }
        }

        for (int i = 1; i < this.transform.childCount - 1; i++)
        {
            StatCardUI card = statCard.GetComponent<StatCardUI>();
            card.selectedCard = statInfo[numArray[i - 1]];
            card.cardGrade = RandomGrade();
            GameObject instant = Instantiate(statCard, this.transform.GetChild(i).transform);
            statCards[i - 1] = instant;
            statCards[i - 1].transform.SetParent(this.transform.GetChild(i));
        }
    }

    Grade RandomGrade()
    {
        float totalWeight = 0;

        weightValue[0] = 150 - (gameManager.round - 1) * 5;
        weightValue[1] = (gameManager.round - 4) * gameManager.round;
        weightValue[2] = (gameManager.round - 9) * 2f;
        weightValue[3] = (gameManager.round - 14);

        for (int i = 0; i < weightValue.Length; i++)
            totalWeight += weightValue[i];

        float rand = Random.Range(0, totalWeight);
        float gradeNum = 0;
        float total = 0;

        for (int i = 0; i < weightValue.Length; i++)
        {
            total += weightValue[i];
            if (rand <= total)
            {
                gradeNum = i;
                break;
            }
        }

        Grade grade;

        if (gradeNum == 1)
            grade = Grade.Èñ±Í;

        else if (gradeNum == 2)
            grade = Grade.Àü¼³;

        else if (gradeNum == 3)
            grade = Grade.½ÅÈ­;

        else
            grade = Grade.ÀÏ¹Ý;

        return grade;
    }
}
