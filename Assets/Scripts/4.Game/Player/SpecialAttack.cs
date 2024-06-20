using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SpecialAttack : MonoBehaviour
{
    [SerializeField] GameObject defaultAttack;
    [SerializeField] LayerMask monsterLayer;

    GameManager gameManager;

    float coolTime = 0.5f;
    bool canAttack = true;

    private void Start()
    {
        gameManager = GameManager.Instance;
        defaultAttack.SetActive(false);
    }

    private void Update()
    {
        if (canAttack)
            VampireSkill();

        if (coolTime > 0)
        {
            canAttack = false;
            coolTime -= Time.deltaTime;
        }

        else
        {
            coolTime = 0.5f;
            canAttack = true;
        }
    }

    void VampireSkill()
    {
        gameManager.maxAbs = 2f;

        float detectRange = Mathf.Clamp(4 + gameManager.range * 0.5f, 1f, 12f);

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, detectRange, monsterLayer);
    }
}
