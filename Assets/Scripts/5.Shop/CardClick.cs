using UnityEngine;
using UnityEngine.UI;
using static WeaponInfo;

public class CardClick : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [Header("Info")]
    [SerializeField] Image cardBack;
    [SerializeField] Image cardBackLine;
    [SerializeField] Image itemSprite;
    [SerializeField] Text weaponName;
    [SerializeField] Text type;
    [SerializeField] Text attackTypes;
    [SerializeField] Text weaponDamage;
    [SerializeField] Text magicDamage;
    [SerializeField] Text attackDelay;
    [SerializeField] Text bulletSpeed;
    [SerializeField] Text weaponRange;
    [SerializeField] Text sellPrice;
    [SerializeField] Text weaponGrade;
    [SerializeField] Text combinePrice;
    [SerializeField] Image descriptBack;
    [SerializeField] Image descriptBackLine;
    [SerializeField] Text description;

    [Header("Panel")]
    [SerializeField] GameObject sellCheckPanel;
    [SerializeField] GameObject combineCheckPanel;
    [SerializeField] GameObject cantSellPanel;
    [SerializeField] GameObject cantCombinePanel;
    [SerializeField] GameObject cantMoneyText;
    [SerializeField] GameObject cantGradeText;

    [HideInInspector] public WeaponInfo selectedWeapon;

    int selectedNum;

    int price;

    ItemManager itemManager;
    Character character;
    GameManager gameManager;

    bool isOver;

    public Vector3 showPos;

    private void Start()
    {
        itemManager = ItemManager.Instance;

        sellCheckPanel.SetActive(false);
        combineCheckPanel.SetActive(false);
        cantSellPanel.SetActive(false);
        cantCombinePanel.SetActive(false);
        isOver = false;
    }

    private void Update()
    {
        if(!isOver)
        {
            if (Input.GetMouseButton(0))
            {
                sellCheckPanel.SetActive(false);
                cantSellPanel.SetActive(false);
                combineCheckPanel.SetActive(false);
                cantCombinePanel.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }

    public void ShowPos()
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, Input.mousePosition, Camera.main, out showPos);
        rect.transform.position = showPos;
    }

    public void Setting(int num)
    {
        itemManager = ItemManager.Instance;
        selectedNum = num;
        selectedWeapon = itemManager.storedWeapon[selectedNum];

        int grade = (int)(itemManager.weaponGrade[selectedNum]);

        price = Mathf.CeilToInt(selectedWeapon.WeaponPrice * (grade * 2f + 1) * (1 - GameManager.Instance.salePercent));
        itemSprite.sprite = selectedWeapon.ItemSprite;
        weaponName.text = selectedWeapon.WeaponName.ToString();
        type.text = selectedWeapon.Type.ToString();
        weaponDamage.text = (selectedWeapon.WeaponDamage * (grade + 1)).ToString();
        magicDamage.text = (selectedWeapon.MagicDamage * (grade + 1)).ToString();
        attackDelay.text = (selectedWeapon.AttackDelay - (grade * 0.1f)).ToString();
        bulletSpeed.text = selectedWeapon.BulletSpeed.ToString();
        weaponRange.text = selectedWeapon.WeaponRange.ToString();
        sellPrice.text = Mathf.CeilToInt(price * 0.7f).ToString();
        weaponGrade.text = itemManager.weaponGrade[num].ToString();
        combinePrice.text = Mathf.CeilToInt(price * 0.5f).ToString();
        description.text = selectedWeapon.Description.ToString();

        if (selectedWeapon.Type == WEAPON_TYPE.검)
            attackTypes.text = "(물리/근거리)";

        else if (selectedWeapon.Type == WEAPON_TYPE.총)
            attackTypes.text = "(물리/원거리)";

        else if (selectedWeapon.Type == WEAPON_TYPE.스태프)
            attackTypes.text = "(마법/원거리)";
    }

    public void CardImage(int num)
    {
        selectedNum = num;
        selectedWeapon = itemManager.storedWeapon[selectedNum];

        if (itemManager.weaponGrade[num] == Grade.일반)
        {
            cardBack.color = new Color(0.142f, 0.142f, 0.142f, 1f);
            descriptBack.color = new Color(0.142f, 0.142f, 0.142f, 1f);
            cardBackLine.color = Color.black;
            descriptBackLine.color = Color.black;
            weaponName.color = Color.white;
            weaponGrade.color = Color.white;
        }

        else if (itemManager.weaponGrade[num] == Grade.희귀)
        {
            cardBack.color = new Color(0f, 0.6f, 0.8f, 1f);
            descriptBack.color = new Color(0f, 0.6f, 0.8f, 1f);
            cardBackLine.color = Color.blue;
            descriptBackLine.color = Color.blue;
            weaponName.color = new Color(0.5f, 0.8f, 1f, 1f);
            weaponGrade.color = new Color(0.5f, 0.8f, 1f, 1f);
        }

        else if (itemManager.weaponGrade[num] == Grade.전설)
        {
            cardBack.color = new Color(0.5f, 0.2f, 0.4f, 1f);
            descriptBack.color = new Color(0.5f, 0.2f, 0.4f, 1f);
            cardBackLine.color = new Color(0.5f, 0f, 0.5f, 1f);
            descriptBackLine.color = new Color(0.5f, 0f, 0.5f, 1f);
            weaponName.color = new Color(0.8f, 0.4f, 1f, 1f);
            weaponGrade.color = new Color(0.8f, 0.4f, 1f, 1f);
        }

        else if (itemManager.weaponGrade[num] == Grade.신화)
        {
            cardBack.color = new Color(0.7f, 0.1f, 0.1f, 1f);
            descriptBack.color = new Color(0.7f, 0.1f, 0.1f, 1f);
            cardBackLine.color = Color.red;
            descriptBackLine.color = Color.red;
            weaponName.color = new Color(1f, 0.45f, 0.45f, 1f);
            weaponGrade.color = new Color(1f, 0.45f, 0.45f, 1f);
        }
    }

    public void PointerEnter()
    {
        isOver = true;
    }

    public void PointerExit()
    {
        isOver = false;
    }

    public void SellWeapon()
    {
        itemManager = ItemManager.Instance;
        character = Character.Instance;

        if (itemManager.equipFullCount > 0)
        {
            selectedWeapon = itemManager.storedWeapon[selectedNum];
            GameManager.Instance.money += Mathf.CeilToInt(price * 0.7f);
            itemManager.storedWeapon[selectedNum] = null;
            itemManager.weaponGrade[selectedNum] = Grade.일반;
            itemManager.equipFullCount--;

            if (selectedWeapon.WeaponName == "번개 스태프")
            {
                StaffControl thunderStaff = character.weaponPoses[selectedNum].GetChild(0).gameObject.GetComponent<StaffControl>();
                itemManager.thunderCount = thunderStaff.thunderCount;
                character.thunderCount--;

                for (int j = 0; j < 6; j++)
                    itemManager.isThunderCountChange[j] = true;

                if (character.thunderCount == 0)
                    character.thunderMark.SetActive(false);
            }

            character.ReleaseEquip(selectedNum);
            ShopManager.Instance.backgroundImage.SetActive(true);
            gameObject.SetActive(false);
        }

        else
        {
            SoundManager.Instance.PlayES("CantBuy");
            cantSellPanel.SetActive(true);
        }
    }

    public void CombineWeapon()
    {
        itemManager = ItemManager.Instance;
        gameManager = GameManager.Instance;
        character = Character.Instance;
        bool canCombine = false;

        for (int i = 0; i < itemManager.storedWeapon.Length; i++)
        {
            if (itemManager.storedWeapon[i] != null)
            {
                if (i != selectedNum && itemManager.weaponGrade[selectedNum] != Grade.신화)
                {
                    if ((selectedWeapon.WeaponName == itemManager.storedWeapon[i].WeaponName) && (itemManager.weaponGrade[selectedNum] == itemManager.weaponGrade[i]))
                    {
                        if (gameManager.money >= Mathf.CeilToInt(price * 0.5f))
                        {
                            canCombine = true;
                            gameManager.money -= Mathf.CeilToInt(price * 0.5f);
                            itemManager.weaponGrade[selectedNum]++;
                            itemManager.storedWeapon[i] = null;
                            itemManager.weaponGrade[i] = Grade.일반;
                            itemManager.equipFullCount--;

                            if (selectedWeapon.WeaponName == "번개 스태프")
                            {

                                StaffControl selectedThunderStaff = character.weaponPoses[selectedNum].GetChild(0).gameObject.GetComponent<StaffControl>();
                                StaffControl combindedThunderStaff = character.weaponPoses[i].GetChild(0).gameObject.GetComponent<StaffControl>();

                                if(selectedThunderStaff.thunderCount > combindedThunderStaff.thunderCount)
                                {
                                    selectedThunderStaff.thunderCount = combindedThunderStaff.thunderCount;
                                    itemManager.thunderCount = selectedThunderStaff.thunderCount;
                                }

                                else
                                    itemManager.thunderCount = combindedThunderStaff.thunderCount;

                                character.thunderCount--;

                                for (int j = 0; j < 6; j++)
                                    itemManager.isThunderCountChange[j] = true;

                                if (character.thunderCount == 0)
                                    character.thunderMark.SetActive(false);
                            }

                            character.ReleaseEquip(i);
                            break;
                        }
                    }
                }
            }
        }

        if (canCombine)
        {
            ShopManager.Instance.backgroundImage.SetActive(true);
            gameObject.SetActive(false);
        }

        else if (!canCombine)
        {
            SoundManager.Instance.PlayES("CantBuy");

            if (itemManager.weaponGrade[selectedNum] == Grade.신화)
            {
                cantMoneyText.SetActive(false);
                cantGradeText.SetActive(true);
            }

            else
            {
                cantMoneyText.SetActive(true);
                cantGradeText.SetActive(false);
            }

            cantCombinePanel.SetActive(true);
        }
    }
}
