using System.Collections;
using UnityEngine;

enum MoveType
{
    DIRECT,
    RANDOM,
    COUNT,
}

public class TamingGamePetMove : MonoBehaviour
{
    [SerializeField] RectTransform moveArea;
    [SerializeField] float moveSpeed = 300;

    RectTransform rectTranform;

    Rect areaRect;

    Vector3 nextPos;

    Vector3 dir;

    bool isTurn = false;

    MoveType moveType;

    public float noiseScale = 0.5f;  // 노이즈 스케일
    private float offsetX, offsetY;  // 노이즈 오프셋

    private void Start()
    {
        rectTranform = GetComponent<RectTransform>();
        areaRect = moveArea.rect;

        StartCoroutine(SetMoveType());
        StartCoroutine(Dash());
        StartCoroutine(GetRandomSpeed());
    }

    private void Update()
    {
        switch (moveType)
        {
            case MoveType.DIRECT:
                MoveDirectly();
                break;

            case MoveType.RANDOM:
                MoveRandomDir();
                break;
        }
    }

    void MoveRandomDir()
    {
        float x = Mathf.PerlinNoise(Time.time * noiseScale + offsetX, 0f) * 2 - 1;
        float y = Mathf.PerlinNoise(0f, Time.time * noiseScale + offsetY) * 2 - 1;

        dir = new Vector3(x, y, 0f).normalized;

        if (isTurn)
        {
            dir = -dir;
        }

        nextPos = rectTranform.localPosition + dir * Time.deltaTime * moveSpeed;

        if (areaRect.Contains(nextPos))
        {
            rectTranform.localPosition = nextPos;
        }

        else
        {
            isTurn = !isTurn;
        }
    }

    void MoveDirectly()
    {
        nextPos = rectTranform.localPosition + dir * Time.deltaTime * moveSpeed * 1.5f;

        if (areaRect.Contains(nextPos))
            rectTranform.localPosition = nextPos;

        else
            dir = -dir;
    }

    IEnumerator Dash()
    {
        while (true)
        {
            float xPos = Random.Range(areaRect.xMin, areaRect.xMax);
            float yPos = Random.Range(areaRect.yMin, areaRect.yMax);

            rectTranform.localPosition = new Vector3(xPos, yPos, 0f);

            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator SetMoveType()
    {
        while (true)
        {
            moveType = (MoveType)Random.Range(0, 2);

            switch (moveType)
            {
                case MoveType.DIRECT:
                    GetRandomOffset();
                    break;

                case MoveType.RANDOM:
                    GetRandomDir();
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator GetRandomSpeed()
    {
        while (true) 
        {
            moveSpeed = Random.Range(200f, 400f);

            yield return new WaitForSeconds(1.5f);
        }
    }

    void GetRandomDir()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);

        dir = new Vector3(x, y, 0f).normalized;
    }

    void GetRandomOffset()
    {
        isTurn = false;

        offsetX = Random.Range(0f, 100f);
        offsetY = Random.Range(0f, 100f);
    }
}
