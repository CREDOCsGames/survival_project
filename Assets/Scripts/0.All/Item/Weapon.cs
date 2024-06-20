using UnityEngine;
using static WeaponInfo;

public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponInfo weaponInfo;

    [HideInInspector] public DamageUI damageUI;
    [HideInInspector] public float weaponDamage;
    [HideInInspector] public float swordBulletDamage;
    [HideInInspector] public int count;

    [HideInInspector] public int grade;

    protected int criRand;

    protected GameManager gameManager;
    protected ItemManager itemManager;

    public void WeaponSetting()
    {
        if (weaponInfo.Type == WEAPON_TYPE.스태프)
        {
            if (!gameManager.doubleShot)
                weaponDamage = weaponInfo.MagicDamage * grade + gameManager.magicDamage + gameManager.longDamage;

            else if (gameManager.doubleShot)
            {
                if (weaponInfo.WeaponName == "번개 스태프")
                    weaponDamage = weaponInfo.MagicDamage * grade + gameManager.magicDamage + gameManager.longDamage;

                else
                    weaponDamage = (weaponInfo.MagicDamage * grade + gameManager.magicDamage + gameManager.longDamage) * 0.7f;
            }

            damageUI.damageText.color = Color.cyan;
        }

        else if (weaponInfo.Type == WEAPON_TYPE.총)
        {
            if (!gameManager.doubleShot)
                weaponDamage = weaponInfo.WeaponDamage * grade + gameManager.physicDamage + gameManager.longDamage;

            else if (gameManager.doubleShot)
                weaponDamage = (weaponInfo.WeaponDamage * grade + gameManager.physicDamage + gameManager.longDamage) * 0.7f;

            damageUI.damageText.color = Color.green;
        }

        else if (weaponInfo.Type == WEAPON_TYPE.검)
        {
            if (criRand <= gameManager.critical || gameManager.critical >= 100)
            {
                if (!gameManager.luckCritical)
                {
                    weaponDamage = (weaponInfo.WeaponDamage * grade + gameManager.physicDamage + gameManager.shortDamage) * 1.5f;
                    swordBulletDamage = (weaponInfo.WeaponDamage * grade + gameManager.magicDamage + gameManager.longDamage) * 1.5f;
                }

                else if (gameManager.luckCritical)
                {
                    int rand = Random.Range(1, 101);

                    if (rand <= gameManager.luck || gameManager.luck >= 100)
                    {
                        weaponDamage = (weaponInfo.WeaponDamage * grade + gameManager.physicDamage + gameManager.shortDamage) * 2f;
                        swordBulletDamage = (weaponInfo.WeaponDamage * grade + gameManager.magicDamage + gameManager.longDamage) * 2f;
                    }

                    else if (rand > gameManager.luck)
                    {
                        weaponDamage = (weaponInfo.WeaponDamage * grade + gameManager.physicDamage + gameManager.shortDamage) * 1.5f;
                        swordBulletDamage = (weaponInfo.WeaponDamage * grade + gameManager.magicDamage + gameManager.longDamage) * 1.5f;
                    }
                }
            }

            else if (criRand > gameManager.critical)
            {
                weaponDamage = (weaponInfo.WeaponDamage * grade + gameManager.physicDamage + gameManager.shortDamage);
                swordBulletDamage = (weaponInfo.WeaponDamage * grade + gameManager.magicDamage + gameManager.longDamage);
            }

            if (gameManager.doubleShot)
                swordBulletDamage = swordBulletDamage * 0.7f;
        }

        weaponDamage = Mathf.Round(weaponDamage * gameManager.percentDamage * 10) * 0.1f;
        swordBulletDamage = Mathf.Round(swordBulletDamage * gameManager.percentDamage * 10) * 0.1f;

        if (weaponDamage < 0)
        {
            weaponDamage = 0;
            swordBulletDamage = 0;
        }

        if (gameManager.luckDamage)
        {
            int rand = Random.Range(1, 101);

            if (rand <= gameManager.luck || gameManager.luck >= 100)
            {
                weaponDamage = weaponDamage * 2f;
                swordBulletDamage = swordBulletDamage * 2f;
            }
        }
    }
}
