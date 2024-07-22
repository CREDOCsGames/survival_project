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

    bool canAttack;


    private void Start()
    {
        anim = GetComponent<Animator>();
        character = Character.Instance;

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
        yield return new WaitForSeconds(1);

        canAttack = true;
    }
}
