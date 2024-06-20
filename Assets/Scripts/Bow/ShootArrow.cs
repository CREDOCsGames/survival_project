using System;
using Unity.VisualScripting;
using UnityEngine;

public class ShootArrow : FireProjectile
{
    [SerializeField] GameObject catchBar;

    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.y = 0;

        dir = mouse - transform.position;

        FireAppliedCoolTime();
    }

    protected override void SettingProjectile()
    {
        base.SettingProjectile();

        bool isCatch = GetComponent<BowCatchBar>().IsCatch();
        projectile.GetComponent<ArrowTypeChange>().ChangeArrowType(isCatch);

        Transform activeArrow = projectile.transform.GetChild(Convert.ToInt32(isCatch));

        float damage = activeArrow.GetComponent<ArrowStatus>().Damage;
        projectile.GetComponent<ProjectileObjectPool>().SetDamage(damage);

        float speed = activeArrow.GetComponent<ArrowStatus>().Speed;
        projectile.GetComponent<ArrowMovement>().Shoot(dir.normalized, normalFirePos.position, speed);
    }

    public bool checkCanFire()
    {
        return canFire;
    }
}
