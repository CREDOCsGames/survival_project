using UnityEngine;
using UnityEngine.UI;

public class MoveRewardImage : Singleton<MoveRewardImage>
{
    [SerializeField] RectTransform coinIamgePos;
    [SerializeField] RectTransform treeImagePos;
    [SerializeField] Sprite[] siblingImages;

    Vector3 targetPoint;

    LoggingLilpa lilpa;

    Transform[] siblings;

    float delayTime;
    int count = 0;

    private void Start()
    {
        lilpa = LoggingLilpa.Instance;

        siblings = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            siblings[i] = transform.GetChild(i);
            siblings[i].gameObject.SetActive(true);
        }

        delayTime = 0f;
        count = 0;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (count < transform.childCount - 1)
        {
            delayTime += Time.deltaTime;

            if (delayTime > 0.1f)
            {
                count++;
                delayTime = 0f;
            }
        }

        else
        {
            if (siblings[count].position == targetPoint)
                gameObject.SetActive(false);
        }

        for (int i = 0; i <= count; i++)
        {
            if (siblings[i].position != targetPoint)
                siblings[i].position = Vector3.MoveTowards(siblings[i].position, targetPoint, Time.deltaTime * 800);
        }
    }

    public void ActiveSetting()
    {
        delayTime = 0f;
        count = 0;

        transform.position = lilpa.treePos;

        Vector3 size = Vector3.zero;

        if (lilpa.treeIndex == 0)
        {
            size = new Vector3(1f, 1f, 1f);
            targetPoint = treeImagePos.position;
        }

        else if (lilpa.treeIndex == 1)
        {
            size = new Vector3(1.5f, 1.5f, 1.5f);
            targetPoint = coinIamgePos.position;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            siblings[i].localPosition = Vector3.zero;
            siblings[i].gameObject.GetComponent<Image>().sprite = siblingImages[lilpa.treeIndex];
            siblings[i].localScale = size;
        }
    }

    // ������ �
    /*private float BezierCurve(float a, float b, float c, float d)
    {
        // (0~1)�� ���� ���� ������ � ���� ���ϱ� ������, ������ ���� �ð��� ���ߴ�.
        float t = m_timerCurrent / m_timerMax; // (���� ��� �ð� / ���� ��ġ�� ������ �ð�)

        // ������.
        return Mathf.Pow((1 - t), 3) * a
            + Mathf.Pow((1 - t), 2) * 3 * t * b
            + Mathf.Pow(t, 2) * 3 * (1 - t) * c
            + Mathf.Pow(t, 3) * d;
    }*/
}
