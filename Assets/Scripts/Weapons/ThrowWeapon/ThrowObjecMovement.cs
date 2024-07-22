using UnityEngine;

public class ThrowObjecMovement : ParabolaLineRenderer
{
    [SerializeField] float totalThrowLength;

    float currentThrowLength = 0;

    Vector3 initScale;

    bool isArrive = false;

    private void Start()
    {
        character = Character.Instance;
        initScale = transform.localScale;
        GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        if (!isArrive)
        {
            Throw();
            ScaleChange();
        }
    }

    float beforeDelta = 3;

    void Throw()
    {
        float halfTotal = totalThrowLength / 2;

        if (currentThrowLength < halfTotal)
            beforeDelta = Mathf.Clamp(beforeDelta - Time.deltaTime * 5, 1.5f, 3);

        else
            beforeDelta = Mathf.Clamp(beforeDelta + Time.deltaTime * 1, 1.5f, 3);

        currentThrowLength += (Time.deltaTime * beforeDelta);

        transform.position = ParabolicPos(startPos, endPos, currentThrowLength, totalThrowLength);

        if (currentThrowLength >= totalThrowLength)
        {
            isArrive = true;
            GetComponent<ThrowObject>().Grounded(isArrive);
            GetComponent<SphereCollider>().enabled = true;
        }
    }

    void ScaleChange()
    {
        float change = Mathf.Sin(currentThrowLength / totalThrowLength * Mathf.PI);
        transform.localScale = initScale + new Vector3(change, change, 0);
    }

    public void Fire(Vector3 startPos)
    {
        GetClickPos(startPos);
        currentThrowLength = 0;
        isArrive = false;
        GetComponent<SphereCollider>().enabled = false;
    }

    public bool CheckIsArrive()
    {
        return isArrive;
    }
}