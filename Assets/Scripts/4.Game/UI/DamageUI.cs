using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    [SerializeField] public Text damageText;

    float printTime, initPrintTime;

    public float realDamage;
    public float swordBulletDamage;

    [HideInInspector] public bool isMiss;

    protected IObjectPool<DamageUI> managedPool;

    private void Start()
    {
        UISetting();
    }

    public void UISetting()
    {
        printTime = 1f;
        initPrintTime = printTime;

        if (!isMiss)
            damageText.text = realDamage.ToString("0.##");

        else if (isMiss)
        {
            damageText.color = Color.white;
            damageText.text = "Miss";
        }
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

        ChangeAlpha(printTime / initPrintTime);

        if (printTime <= 0)
            DestroyUI();
    }

    public void SetManagedPool(IObjectPool<DamageUI> pool)
    {
        managedPool = pool;
    }

    public virtual void DestroyUI()
    {
        damageText.fontSize = 50;

        if (gameObject.activeSelf)
            managedPool.Release(this);
    }
}
