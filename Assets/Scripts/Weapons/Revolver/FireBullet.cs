using System.Collections;
using UnityEngine;

public class FireBullet : FireProjectile
{
    [SerializeField] Transform bulletParent;

    int bulletCount = 4;

    Coroutine currentCoroutine;

    private void OnEnable()
    {
        bulletCount = gameManager.specialStatus[SpecialStatus.AmmoPouch] ? 5 : 4;

        for (int i = 0; i < bulletCount; ++i)
        {
            bulletParent.GetChild(i).gameObject.SetActive(true);
        }

        for(int i = bulletCount; i<bulletParent.childCount; ++i) 
        {
            bulletParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (bulletCount > 0)
        {
            FireAppliedCoolTime();
        }

        else if (bulletCount <= 0)
            character.canWeaponChange = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Transform bullet in bulletParent)
            {
                bullet.gameObject.SetActive(true);
            }

            bulletCount = 4;
        }
#endif
    }

    protected override void Fire()
    {
        if (Input.GetMouseButton(0) && !(gameManager.isClear || gameManager.isPause))
        {
            character.canWeaponChange = false;
            SetFire();
            bulletParent.GetChild(bulletCount - 1).gameObject.SetActive(false);
            bulletCount--;

            if(currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(FreezeCharacter());
        }
    }

    protected override void SettingProjectile()
    {
        base.SettingProjectile();

        projectile.GetComponent<ProjectileMovement>().Shoot(dir.normalized, normalFirePos.position);
    }

    IEnumerator FreezeCharacter()
    {
        character.isCanControll = false;

        yield return new WaitForSeconds(1);

        character.isCanControll = true;
    }
}
