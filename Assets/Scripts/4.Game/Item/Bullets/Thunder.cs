using UnityEngine;

public class Thunder : Bullet
{
    [SerializeField] Transform effectPos;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                DestroyBullet();
            }
        }
    }

    public void AttackEffect()
    {
        Instantiate(effectPrefab, effectPos.position, effectPrefab.transform.rotation);
    }
}
