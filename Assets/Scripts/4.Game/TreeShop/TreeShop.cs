using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class TreeShop : Singleton<TreeShop>
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Transform weaponParent;
    [SerializeField] Text upgradeCostText;
    [SerializeField] GameObject ShopPanel;
    [SerializeField] Text dialogText;

    ItemManager itemManager;
    GameManager gameManager;

    [HideInInspector] public int upgradeCost;

    void Start()
    {
        itemManager = ItemManager.Instance;
        gameManager = GameManager.Instance;

        upgradeCost = 10;

        if (gameManager.woodCount >= upgradeCost)
        {
            ShopPanel.SetActive(true);

            dialogText.text = "???: 가지고 있는 나무를 나눠 주지 않으련...?";
            upgradeCostText.text = (-upgradeCost).ToString();

            for (int i = 0; i < itemManager.storedWeapon.Length; i++)
            {
                if (itemManager.storedWeapon[i] != null && itemManager.weaponGrade[i] != Grade.신화)
                {
                    TreeShopWeaponInfo weapon;
                    weapon = Instantiate(weaponPrefab, weaponParent).GetComponent<TreeShopWeaponInfo>();
                    weapon.siblingNum = i;
                }
            }
        }

        else 
        {
            ShopPanel.SetActive(false);
            dialogText.text = "???: 나무를 충분히 가지고 있지 않구나...";
        }
    }

    public void ToShop()
    {
        gameManager.ToNextScene("Shop");
    }
}
