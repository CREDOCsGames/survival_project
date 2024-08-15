using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class SwingCutlass : MonoBehaviour
{
    [SerializeField] BoxCollider collder;
    [SerializeField] float initCoolTime;

    Animator anim;
    Character character;

    bool canAttack;

    float coolTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        character = Character.Instance;
    }

    private void OnEnable()
    {
        coolTime = initCoolTime;

        collder.enabled = false;

        canAttack = true;
    }

    private void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (!canAttack)
            return;

        if (Input.GetMouseButton(0) && character.isCanControll)
        {
            character.canWeaponChange = false;
            character.canFlip = false;
            character.anim.SetTrigger("isAttack");
            collder.enabled = true;

            anim.SetBool("canAttack", canAttack);

            if (!character.IsFlip)
                anim.SetBool("RightAttack", true);

            else
                anim.SetBool("RightAttack", false);

            canAttack = false;
        }
    }

    void EndAttack()
    {
        collder.enabled = false;
        character.canFlip = true;
        anim.SetBool("canAttack", canAttack);
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        coolTime = initCoolTime * character.attackSpeed;
        yield return new WaitForSeconds(coolTime);

        canAttack = true;
        character.canWeaponChange = true;
    }
}
