using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

public class AttackCutlass : MonoBehaviour
{
    [SerializeField] DamageUI damageUI;
    [SerializeField] float damage;

    protected IObjectPool<DamageUI> damagePool;

    GameManager gameManager;

    private void Awake()
    {
        damagePool = new ObjectPool<DamageUI>(CreateDamageUI, OnGetDamageUI, OnReleaseDamageUI, OnDestroyDamageUI, maxSize: 20);

        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && other.transform.parent.GetComponent<Monster>() != null)
        {
            Monster monster = other.transform.parent.GetComponent<Monster>();

            DamageUI damageUI = damagePool.Get();

            float mDef = monster.defence;
            damageUI.realDamage = Mathf.Clamp(damage * (1 - (mDef / (20 + mDef))), 0, damage);

            damageUI.UISetting();
            damageUI.transform.position = other.transform.position;
            damageUI.gameObject.transform.SetParent(gameManager.damageStorage);

            monster.OnDamaged(damageUI.realDamage);
        }
    }

    private DamageUI CreateDamageUI()
    {
        DamageUI damageUIPool = Instantiate(damageUI, transform.position, Quaternion.Euler(90, 0, 0)).GetComponent<DamageUI>();
        damageUIPool.SetManagedPool(damagePool);
        damageUIPool.transform.SetParent(gameManager.bulletStorage);
        return damageUIPool;
    }

    private void OnGetDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(true);
    }

    private void OnReleaseDamageUI(DamageUI damageUIPool)
    {
        damageUIPool.gameObject.SetActive(false);
    }

    private void OnDestroyDamageUI(DamageUI damageUIPool)
    {
        if (damageUIPool.gameObject != null)
        {
            Destroy(damageUIPool.gameObject);
        }
    }
}
