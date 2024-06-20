using UnityEngine;
using UnityEngine.UI;

public class StatCardUI : StatCardInfo
{
    [SerializeField] Image cardBack;
    [SerializeField] Image cardBackLine;
    [SerializeField] Image itemSprite;
    [SerializeField] Text statName;
    [SerializeField] Text statGrade;
    [SerializeField] Text statValue;
    [SerializeField] Text statType;

    [HideInInspector] public Stat selectedCard;

    GameManager gameManager;
    ShowStatCard showStatCard;

    [HideInInspector] public Grade cardGrade;

    private void Start()
    {
        gameManager = GameManager.Instance; 
        Setting();
        CardColor();
    }

    void Setting()
    {
        itemSprite.sprite = Resources.Load<Sprite>(selectedCard.statSprite);
        statName.text = selectedCard.statName;
        statValue.text = (selectedCard.statValue * ((int)cardGrade * 2f + 1)).ToString();
        statType.text = selectedCard.statType;
        statGrade.text = cardGrade.ToString();
    }

    void CardColor()
    {
        if (cardGrade == Grade.¿œπ›)
        {
            cardBack.color = new Color(0.142f, 0.142f, 0.142f, 1f);
            cardBackLine.color = Color.black;
            statName.color = Color.white;
            statGrade.color = Color.white;
        }

        else if (cardGrade == Grade.»Ò±Õ)
        {
            cardBack.color = new Color(0f, 0.6f, 0.8f, 1f);
            cardBackLine.color = Color.blue;
            statName.color = new Color(0.5f, 0.8f, 1f, 1f);
            statGrade.color = new Color(0.5f, 0.8f, 1f, 1f);
        }

        else if (cardGrade == Grade.¿¸º≥)
        {
            cardBack.color = new Color(0.5f, 0.2f, 0.4f, 1f);
            cardBackLine.color = new Color(0.5f, 0f, 0.5f, 1f);
            statName.color = new Color(0.8f, 0.4f, 1f, 1);
            statGrade.color = new Color(0.8f, 0.4f, 1f, 1);
        }

        else if (cardGrade == Grade.Ω≈»≠)
        {
            cardBack.color = new Color(0.7f, 0.1f, 0.1f, 1f);
            cardBackLine.color = Color.red;
            statName.color = new Color(1f, 0.45f, 0.45f, 1f);
            statGrade.color = new Color(1f, 0.45f, 0.45f, 1f);
        }
    }

    public void SelectCard()
    {
        showStatCard = ShowStatCard.Instance;

        SoundManager.Instance.PlayES("StatUp");
        showStatCard.isSelected = true;
        showStatCard.rerollMoney = -gameManager.round;

        Character.Instance.levelUpCount--;

        for (int i = 0; i < 13; i++)
        {
            if (selectedCard.statName == gameManager.gameObject.GetComponent<StatCardInfo>().statInfos[i].statName)
            {
                gameManager.stats[i] += (gameManager.gameObject.GetComponent<StatCardInfo>().statInfos[i].statValue * ((int)cardGrade * 2f + 1));
                gameManager.stats[i] = Mathf.Round(gameManager.stats[i] * 10) * 0.1f;
            }
        }

        if (Character.Instance.levelUpCount > 0)
            showStatCard.ShowRandomCards();
    }
}
