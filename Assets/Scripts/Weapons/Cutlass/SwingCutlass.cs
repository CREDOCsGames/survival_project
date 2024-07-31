using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class SwingCutlass : MonoBehaviour
{
    [SerializeField] BoxCollider collder;
    Animator anim;
    Character character;
    GameManager gameManager;

    bool canAttack;

    float coolTime = 1;
    float initCoolTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        character = Character.Instance;
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        initCoolTime = coolTime;

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
            collder.enabled = true;
            anim.SetTrigger("RightAttack");
            canAttack = false;
        }
    }

    void EndAttack()
    {
        collder.enabled = false;
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
