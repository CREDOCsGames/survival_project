using System.Collections;
using UnityEngine;


public class PaaLazor : MonsterBullet
{
    Character character;
    public SpriteRenderer rend;
    Animator anim;

    [HideInInspector] public bool isFlip;

    bool isAttack;

    private void Start()
    {
        gameManager = GameManager.Instance;

        character = Character.Instance;
        anim = GetComponent<Animator>();

        realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;  // 트리거에도 있음
    }

    private void Update()
    {

        if (gameManager.isClear && gameManager.isBossDead)
        {
            DestroyBullet();
        }

        anim.SetBool("isFlip", isFlip);
        rend.flipX = isFlip;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RightPaaLazor") || anim.GetCurrentAnimatorStateInfo(0).IsName("LeftPaaLazor"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                DestroyBullet();
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character") && !isAttack)
        {
            realDamage = bulletDamage * (1 + Mathf.Floor(gameManager.round / 30)) + Mathf.Floor(gameManager.round / 5) * 2f;  // 트리거에도 있음
            character.OnDamaged(realDamage);
            isAttack = true;
            StartCoroutine(IEInvincible());
        }
    }

    IEnumerator IEInvincible()
    {
        yield return new WaitForSeconds(0.6f);
        isAttack = false;
    }

    public override void DestroyBullet()
    {
        base.DestroyBullet();
        isAttack = false;
    }
}
