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
        Color textXColor = damageText.color;        // �ؽ�Ʈ�� ����
        textXColor.a = alpha;                       // ������ ���İ� ����(���� ���� �Ұ��ؼ� ����)
        damageText.color = textXColor;              // ������ ������ ����
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
