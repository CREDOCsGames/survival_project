using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class TreeShopWeaponInfo : MonoBehaviour
{
    [SerializeField] Image gradeColor;
    [SerializeField] Image weaponImage;
    [SerializeField] Text weaponName;
    [SerializeField] Text weaponGrade;

    ItemManager itemManager;
    WeaponInfo weaponInfo;
    GameManager gameManager;
    TreeShop treeShop;
    GameSceneUI gameSceneUI;

    [HideInInspector] public int siblingNum;

    Grade selectGrade;

    void Start()
    {
        itemManager = ItemManager.Instance;
        gameManager = GameManager.Instance;
        treeShop = TreeShop.Instance;
        gameSceneUI = GameSceneUI.Instance;

        weaponInfo = itemManager.storedWeapon[siblingNum];

        selectGrade = itemManager.weaponGrade[siblingNum];

        weaponName.text = weaponInfo.WeaponName;
        weaponGrade.text = selectGrade.ToString();
        weaponImage.sprite = weaponInfo.ItemSprite;
        CardColor();
    }

    void CardColor()
    {
        if (selectGrade == Grade.¿œπ›)
            gradeColor.color = new Color(0.142f, 0.142f, 0.142f, 1f);

        else if (selectGrade == Grade.»Ò±Õ)
            gradeColor.color = new Color(0f, 0.6f, 0.8f, 1f);

        else if (selectGrade == Grade.¿¸º≥)
            gradeColor.color = new Color(0.5f, 0.2f, 0.4f, 1f);

        else if (selectGrade == Grade.Ω≈»≠)
            gradeColor.color = new Color(0.7f, 0.1f, 0.1f, 1f);
    }

    public void SelectUpgrade()
    {
        if (selectGrade != Grade.Ω≈»≠)
        {
            gameManager.woodCount -= treeShop.upgradeCost;
            itemManager.weaponGrade[siblingNum]++;
            gameSceneUI.treeShopCount--;

            selectGrade = itemManager.weaponGrade[siblingNum];

            weaponName.text = weaponInfo.WeaponName;
            weaponGrade.text = selectGrade.ToString();
            weaponImage.sprite = weaponInfo.ItemSprite;
            CardColor();
        }
    }
}
