using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ExDamageUI : MonoBehaviour
{
    [SerializeField] Text damageText;
    [SerializeField] GameObject explosion;

    public float damage;

    float printTime, initPrintTime;

    protected IObjectPool<ExDamageUI> managedPool;

    private void Start()
    {
        damageSetting();
    }

    public void damageSetting()
    {
        printTime = 1f;
        initPrintTime = printTime;
        damageText.text = damage.ToString();
    }

    private void ChangeAlpha(float alpha)
    {
        Color textXColor = damageText.color;        // 텍스트의 색상값
        textXColor.a = alpha;                       // 색상값의 알파값 변경(직접 변경 불가해서 빼옴)
        damageText.color = textXColor;              // 변경한 색상을 대입
    }

    private void Update()
    {
        transform.position += Vector3.forward * 0.5f * Time.deltaTime;

        printTime -= Time.deltaTime;

        if (printTime <= 0)
            DestroyUI();

        ChangeAlpha(printTime / initPrintTime);
    }

    public void SetManagedPool(IObjectPool<ExDamageUI> pool)
    {
        managedPool = pool;
    }

    public virtual void DestroyUI()
    {
        if (gameObject.activeSelf)
            managedPool.Release(this);
    }
}
