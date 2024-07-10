using System.Collections;
using UnityEngine;

public class TamingGamePetMove : MonoBehaviour
{
    [SerializeField] RectTransform moveArea;
    [SerializeField] float moveSpeed = 300;

    RectTransform rectTranform;

    Rect areaRect;

    Vector3 nextPos;

    Vector3 dir;

    bool isTurn = false;

    public float noiseScale = 0.5f;  // 노이즈 스케일
    private float offsetX, offsetY;  // 노이즈 오프셋

    private void Start()
    {
        rectTranform = GetComponent<RectTransform>();
        areaRect = moveArea.rect;

        /*Debug.Log(areaRect.xMin);
        Debug.Log(areaRect.xMax);
        Debug.Log(areaRect.yMin);
        Debug.Log(areaRect.yMax);*/

        StartCoroutine(GetRandomDir());
    }

    private void Update()
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

    IEnumerator GetRandomDir()
    {
        while (true)
        {
            isTurn = false;

            offsetX = Random.Range(0f, 100f);
            offsetY = Random.Range(0f, 100f);

            yield return new WaitForSeconds(2f);
        }
    }
}
