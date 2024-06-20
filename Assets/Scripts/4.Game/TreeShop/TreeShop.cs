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

            dialogText.text = "???: ������ �ִ� ������ ���� ���� ������...?";
            upgradeCostText.text = (-upgradeCost).ToString();

            for (int i = 0; i < itemManager.storedWeapon.Length; i++)
            {
                if (itemManager.storedWeapon[i] != null && itemManager.weaponGrade[i] != Grade.��ȭ)
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
            dialogText.text = "???: ������ ����� ������ ���� �ʱ���...";
        }
    }

    public void ToShop()
    {
        gameManager.ToNextScene("Shop");
    }
}
