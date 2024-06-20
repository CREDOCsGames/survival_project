using UnityEngine;
using UnityEngine.UI;
using static WeaponInfo;

public class WeaponCardUI : WeaponCard
{
    [Header("Lock")]
    [SerializeField] Image lockBackImage;
    [SerializeField] Image lockImage;
    [SerializeField] Text lockText;

    [Header("Card")]
    [SerializeField] Text weaponPrice;
    [SerializeField] GameObject combineCheckPanel;
    [SerializeField] Text combineMoney;
    [SerializeField] GameObject cantCombinePanel;

    Color LockImageColor;
    Color LockTextColor;

    [HideInInspector] public bool isLock;

    Color initPriceColor;

    int combineNum;

    int num;

    bool isOver = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemManager = ItemManager.Instance;
        character = Character.Instance;

        isOver = false;

        combineCheckPanel.SetActive(false);
        cantCombinePanel.SetActive(false);

        initPriceColor = weaponPrice.color;
        LockImageColor = new Color(0.17f, 0.17f, 0.17f);
        LockTextColor = Color.white;

        num = transform.parent.GetSiblingIndex();

        isLock = itemManager.cardLocks[num];

        Setting();
        CardColor();
        StartLockColor();
    }

    private void Update()
    {
        if (!isOver)
        {
            if (Input.GetMouseButton(0))
            {
                combineCheckPanel.SetActive(false);
                cantCombinePanel.SetActive(false);
            }
        }

        lockImage.gameObject.SetActive(isLock);

        if (gameManager.money < price)
            weaponPrice.color = Color.red;

        else if (gameManager.money >= price)
            weaponPrice.color = initPriceColor;
    }

    protected override void Setting()
    {
        if (isLock)
        {
            selectedWeapon = itemManager.lockedWeaCards[num];
            selectGrade = itemManager.selectedGrades[num];
        }

        base.Setting();

        price = Mathf.CeilToInt(selectedWeapon.WeaponPrice * ((int)selectGrade * 2f + 1) * (1 - gameManager.salePercent));
        weaponPrice.text = price.ToString();
        combineMoney.text = Mathf.CeilToInt(price * 0.5f).ToString();
    }

    public override void Select()
    {
        base.Select();
    }

    protected override void EquipWeapon()
    {
        isLock = false;
        itemManager.cardLocks[num] = isLock;
        itemManager.lockedWeaCards[num] = null;
        base.EquipWeapon();
    }

    protected override void FullWeaponCheckCombine()
    {
        for (int i = 0; i < itemManager.storedWeapon.Length; i++)
        {
            if (itemManager.storedWeapon[i] != null && selectGrade != Grade.½ÅÈ­)
            {
                if ((selectedWeapon.WeaponName == itemManager.storedWeapon[i].WeaponName) && (selectGrade == itemManager.weaponGrade[i]))
                {
                    combineNum = i;
                    combineCheckPanel.SetActive(true);
                    break;
                }
            }
        }
    }

    public void Combine()
    {
        if (gameManager.money - price >= Mathf.CeilToInt(price * 0.5f))
        {
            SoundManager.Instance.PlayES("WeaponSelect");
            gameManager.money -= Mathf.CeilToInt(price * 0.5f);
            gameManager.money -= price;
            itemManager.weaponGrade[combineNum]++;
            isLock = false;
            itemManager.cardLocks[num] = isLock;
            itemManager.lockedWeaCards[num] = null;
            Destroy(gameObject);
        }

        else
        {
            SoundManager.Instance.PlayES("CantBuy");
            cantCombinePanel.SetActive(true);
        }
    }

    public void Lock()
    {
        if (!isLock)
        {
            lockBackImage.color = Color.white;
            lockText.color = Color.black;
            itemManager.lockedWeaCards[num] = selectedWeapon;
            itemManager.selectedGrades[num] = selectGrade;
            isLock = true;
        }

        else if (isLock)
        {
            lockBackImage.color = LockImageColor;
            lockText.color = LockTextColor;
            itemManager.lockedWeaCards[num] = null;
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

    public void PointerEnter()
    {
        isOver = true;
    }

    public void PointerExit()
    {
        isOver = false;
    }
}
