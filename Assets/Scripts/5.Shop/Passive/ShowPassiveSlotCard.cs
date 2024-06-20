using UnityEngine;
using UnityEngine.UI;

public class ShowPassiveSlotCard : MonoBehaviour
{
    [Header("decript")]
    [SerializeField] GameObject[] descriptPrefabs;

    [Header("Card")]
    [SerializeField] Image cardBack;
    [SerializeField] Image cardBackLine;
    [SerializeField] Image itemSprite;
    [SerializeField] Text itemName;
    [SerializeField] Text itemGrade;
    [SerializeField] Text maxCount;
    [SerializeField] public RectTransform rect;

    PassiveInfo selectedPassive;

    float[] stats = new float[15];
    string[] statTypes = new string[15];

    [HideInInspector] public int selectedNum;
    [HideInInspector] public bool infoChange;

    private void Update()
    {
        if(infoChange)
        {
            Setting();
            CardImage();
            StatArray();
            DescriptionInfo();
        }
    }

    void Setting()
    {
        selectedPassive = ItemManager.Instance.storedPassive[selectedNum];

        itemSprite.sprite = selectedPassive.ItemSprite;
        itemName.text = selectedPassive.ItemName;
        itemGrade.text = selectedPassive.ItemGrade.ToString();
        maxCount.text = selectedPassive.MaxCount.ToString();
    }

    void CardImage()
    {
        if (selectedPassive.ItemGrade == Grade.�Ϲ�)
        {
            cardBack.color = new Color(0.142f, 0.142f, 0.142f, 1f);
            cardBackLine.color = Color.black;
            itemName.color = Color.white;
            itemGrade.color = Color.white;
        }

        else if (selectedPassive.ItemGrade == Grade.���)
        {
            cardBack.color = new Color(0f, 0.6f, 0.8f, 1f);
            cardBackLine.color = Color.blue;
            itemName.color = new Color(0.5f, 0.8f, 1f, 1f);
            itemGrade.color = new Color(0.5f, 0.8f, 1f, 1f);
        }

        else if (selectedPassive.ItemGrade == Grade.����)
        {
            cardBack.color = new Color(0.5f, 0.2f, 0.4f, 1f);
            cardBackLine.color = new Color(0.5f, 0f, 0.5f, 1f);
            itemName.color = new Color(0.8f, 0.4f, 1f, 1f);
            itemGrade.color = new Color(0.8f, 0.4f, 1f, 1f);
        }

        else if (selectedPassive.ItemGrade == Grade.��ȭ)
        {
            cardBack.color = new Color(0.7f, 0.1f, 0.1f, 1f);
            cardBackLine.color = Color.red;
            itemName.color = new Color(1f, 0.45f, 0.45f, 1f);
            itemGrade.color = new Color(1f, 0.45f, 0.45f, 1f);
        }
    }

    void StatArray()
    {
        stats[0] = selectedPassive.Hp;
        stats[1] = selectedPassive.RecoverHp;
        stats[2] = selectedPassive.AbsorbHp;
        stats[3] = selectedPassive.Defence;
        stats[4] = selectedPassive.PhysicDamage;
        stats[5] = selectedPassive.MagicDamage;
        stats[6] = selectedPassive.ShortDamage;
        stats[7] = selectedPassive.LongDamage;
        stats[8] = selectedPassive.AttackSpeed;
        stats[9] = selectedPassive.Speed;
        stats[10] = selectedPassive.Luck;
        stats[11] = selectedPassive.Range;
        stats[12] = selectedPassive.Critical;
        stats[13] = selectedPassive.PercentDamage;
        stats[14] = selectedPassive.Avoid;

        statTypes[0] = "�ִ� ü��";
        statTypes[1] = "ü�� ȸ��";
        statTypes[2] = "ü�� ���";
        statTypes[3] = "����";
        statTypes[4] = "���� ���ݷ�";
        statTypes[5] = "���� ���ݷ�";
        statTypes[6] = "�ٰŸ� ���ݷ�";
        statTypes[7] = "���Ÿ� ���ݷ�";
        statTypes[8] = "���� �ӵ�";
        statTypes[9] = "�̵� �ӵ�";
        statTypes[10] = "���";
        statTypes[11] = "��Ÿ�";
        statTypes[12] = "ũ��Ƽ��";
        statTypes[13] = "���ݷ� ����";
        statTypes[14] = "ȸ����";
    }

    void DescriptionInfo()
    {
        int max = descriptPrefabs.Length;
        int count = 0;

        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i] != 0)
            {
                descriptPrefabs[count].transform.GetChild(0).gameObject.SetActive(true);
                descriptPrefabs[count].transform.GetChild(1).gameObject.SetActive(true);
                descriptPrefabs[count].transform.GetChild(2).gameObject.SetActive(true);
                descriptPrefabs[count].transform.GetChild(0).GetComponent<Text>().text = statTypes[i];
                descriptPrefabs[count].transform.GetChild(2).GetComponent<Text>().text = stats[i].ToString();

                if (stats[i] < 0)
                    descriptPrefabs[count].transform.GetChild(2).GetComponent<Text>().color = Color.red;
                else if (stats[i] >= 0)
                    descriptPrefabs[count].transform.GetChild(2).GetComponent<Text>().color = new Color(0.4f, 1, 0);

                descriptPrefabs[count].transform.GetChild(3).gameObject.SetActive(false);
                descriptPrefabs[count].gameObject.SetActive(true);
                count++;
            }
        }

        for (int i = max - 1; i >= count; i--)
        {
            if (i == count)
            {
                descriptPrefabs[count].transform.GetChild(0).gameObject.SetActive(false);
                descriptPrefabs[count].transform.GetChild(1).gameObject.SetActive(false);
                descriptPrefabs[count].transform.GetChild(2).gameObject.SetActive(false);
                descriptPrefabs[count].transform.GetChild(3).gameObject.SetActive(true);
                descriptPrefabs[count].transform.GetChild(3).GetComponent<Text>().text = selectedPassive.Description;
                descriptPrefabs[count].gameObject.SetActive(true);
            }

            else
                descriptPrefabs[i].gameObject.SetActive(false);
        }

        infoChange = false;
    }
}
