using UnityEngine;
using UnityEngine.UI;
using static WeaponInfo;

public class WeaponCard : MonoBehaviour
{
    [SerializeField] public WeaponInfo[] weaponInfos;

    [Header("Card")]
    [SerializeField] protected Image cardBack;
    [SerializeField] protected Image cardBackLine;
    [SerializeField] protected Image itemSprite;
    [SerializeField] protected Text weaponName;
    [SerializeField] protected Text type;
    [SerializeField] protected Text attackTypes;
    [SerializeField] protected Text weaponDamage;
    [SerializeField] protected Text magicDamage;
    [SerializeField] protected Text attackDelay;
    [SerializeField] protected Text bulletSpeed;
    [SerializeField] protected Text weaponRange;
    [SerializeField] protected Text weaponGrade;
    [SerializeField] protected Text description;
    [SerializeField] protected Image canBuyImage;

    [HideInInspector] public WeaponInfo selectedWeapon;

    protected GameManager gameManager;
    protected ItemManager itemManager;
    protected Character character;

    [HideInInspector] public Grade selectGrade;

    protected int price;

    protected virtual void Setting()
    {
        OnOffCanBuyPanel();

        itemSprite.sprite = selectedWeapon.ItemSprite;
        weaponName.text = selectedWeapon.WeaponName.ToString();
        type.text = selectedWeapon.Type.ToString();
        weaponDamage.text = (selectedWeapon.WeaponDamage * ((int)selectGrade + 1)).ToString();
        magicDamage.text = (selectedWeapon.MagicDamage * ((int)selectGrade + 1)).ToString();
        attackDelay.text = (selectedWeapon.AttackDelay - ((int)selectGrade * 0.1f)).ToString("0.##");
        bulletSpeed.text = selectedWeapon.BulletSpeed.ToString();
        weaponRange.text = selectedWeapon.WeaponRange.ToString();
        weaponGrade.text = selectGrade.ToString();
        description.text = selectedWeapon.Description.ToString();

        if (selectedWeapon.Type == WEAPON_TYPE.��)
            attackTypes.text = "(����/�ٰŸ�)";

        else if (selectedWeapon.Type == WEAPON_TYPE.��)
            attackTypes.text = "(����/���Ÿ�)";

        else if (selectedWeapon.Type == WEAPON_TYPE.������)
            attackTypes.text = "(����/���Ÿ�)";
    }

    protected virtual void CardColor()
    {
        if (selectGrade == Grade.�Ϲ�)
        {
            cardBack.color = new Color(0.142f, 0.142f, 0.142f, 0.8235f);
            cardBackLine.color = Color.black;
            weaponName.color = Color.white;
            weaponGrade.color = Color.white;
        }

        else if (selectGrade == Grade.���)
        {
            cardBack.color = new Color(0f, 0.6f, 0.8f, 0.8235f);
            cardBackLine.color = Color.blue;
            weaponName.color = new Color(0.5f, 0.8f, 1f, 1f);
            weaponGrade.color = new Color(0.5f, 0.8f, 1f, 1f);
        }

        else if (selectGrade == Grade.����)
        {
            cardBack.color = new Color(0.5f, 0.2f, 0.4f, 0.8235f);
            cardBackLine.color = new Color(0.5f, 0f, 0.5f, 1f);
            weaponName.color = new Color(0.8f, 0.4f, 1f, 1f);
            weaponGrade.color = new Color(0.8f, 0.4f, 1f, 1f);
        }

        else if (selectGrade == Grade.��ȭ)
        {
            cardBack.color = new Color(0.7f, 0.1f, 0.1f, 0.8235f);
            cardBackLine.color = Color.red;
            weaponName.color = new Color(1f, 0.45f, 0.45f, 1f);
            weaponGrade.color = new Color(1f, 0.45f, 0.45f, 1f);
        }
    }

    void OnOffCanBuyPanel()
    {
        canBuyImage.gameObject.SetActive(!CheckCanUseCharacter());
    }

    bool CheckCanUseCharacter()
    {
        for (int i = 0; i < selectedWeapon.UseCharacter.Length; i++)
        {
            if (character.currentCharacterInfo == selectedWeapon.UseCharacter[i])
                return true;
        }

        return false;
    }

    public virtual void Select()
    {
        bool canBuy = false;

        if (CheckCanUseCharacter())
        {
            if (itemManager.equipFullCount < 6 && gameManager.money >= price)
            {
                canBuy = true;

                if (selectedWeapon.WeaponName == "���� ������")
                {
                    if (character.thunderCount == 0)
                        character.thunderMark.SetActive(true);

                    character.thunderCount++;
                }

                EquipWeapon();
            }

            else if (itemManager.equipFullCount >= 6 && gameManager.money >= price)
            {
                for (int i = 0; i < itemManager.storedWeapon.Length; i++)
                {
                    canBuy = true;
                    FullWeaponCheckCombine();
                }
            }
        }

        if (!canBuy)
            SoundManager.Instance.PlayES("CantBuy");
    }

    /*public virtual void Select()
    {
        bool canBuy = false;

        if (selectedWeapon.Type == WEAPON_TYPE.��)
        {
            if (itemManager.equipFullCount < 6 && gameManager.money >= price)
            {
                canBuy = true;
                EquipWeapon();
            }

            else if (itemManager.equipFullCount >= 6 && gameManager.money >= price)
            {
                for (int i = 0; i < itemManager.storedWeapon.Length; i++)
                {
                    canBuy = true;
                    FullWeaponCheckCombine();
                }
            }
        }

        // �˽�ĳ���ʹ� �˿ܿ� ���� ���ϰ�
        else if (selectedWeapon.Type != WEAPON_TYPE.��)
        {
            if (character.characterNum != (int)CHARACTER_NUM.Legendary)
            {
                if (itemManager.equipFullCount < 6 && gameManager.money >= price)
                {
                    canBuy = true;

                    if (selectedWeapon.WeaponName == "���� ������")
                    {
                        if (character.thunderCount == 0)
                            character.thunderMark.SetActive(true);

                        character.thunderCount++;
                    }

                    EquipWeapon();
                }

                else if (itemManager.equipFullCount >= 6 && gameManager.money >= price)
                {
                    canBuy = true;
                    FullWeaponCheckCombine();
                }
            }

            else if (character.characterNum == (int)CHARACTER_NUM.Legendary)
                SoundManager.Instance.PlayES("CantBuy");
        }

        if (!canBuy)
            SoundManager.Instance.PlayES("CantBuy");
    }*/

    protected virtual void EquipWeapon()
    {
        SoundManager.Instance.PlayES("WeaponSelect");
        gameManager.money -= price;
        itemManager.equipFullCount++;
        itemManager.GetWeaponInfo(selectedWeapon);
        itemManager.weaponGrade[itemManager.weaponCount] = selectGrade;
        character.Equip();
        Destroy(gameObject);
    }

    protected virtual void FullWeaponCheckCombine()
    {

    }
}

