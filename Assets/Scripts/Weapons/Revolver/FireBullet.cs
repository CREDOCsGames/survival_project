using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : FireProjectile
{
    [SerializeField] Transform bulletParent;

    int bulletCount = 4;

    Character character;

    Coroutine currentCoroutine;

    protected override void Start()
    {
        base.Start();

        character = Character.Instance;

        foreach (Transform bullet in bulletParent)
        {
            bullet.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (bulletCount > 0)
        {
            FireAppliedCoolTime();
        }

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

        projectile.GetComponent<ArrowMovement>().Shoot(dir.normalized, normalFirePos.position);
    }

    IEnumerator FreezeCharacter()
    {
        character.isCanControll = false;

        yield return new WaitForSeconds(1);

        character.isCanControll = true;
    }
}
