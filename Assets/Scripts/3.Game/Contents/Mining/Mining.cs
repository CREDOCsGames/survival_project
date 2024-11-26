using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mining : MonoBehaviour
{
    [SerializeField] List<GameObject> objects;
    [SerializeField] List<int> objectProbability;
    [SerializeField] UnityEngine.UI.Text stoneTimerText;
    [SerializeField] GameObject currentObj;

    int probability=0;
    int stoneMaxValue = 10;
    int stoneInputValue = 0;
    bool stone = false;
    float time = 0;

    private void Awake()
    {
        for(int i=0;i<objectProbability.Count-1;i++)
        {
            probability += objectProbability[i];
        }
        RandomCreateObject();
        print(currentObj.name);
        print(currentObj.activeSelf);
    }

    void OpenUI()
    {
        gameObject.SetActive(true);
        RandomCreateObject();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            KeyInputJudgement();
        }
        
    }

    //키 입력
    void KeyInputJudgement()
    {
        int selectType = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectType = 1;
                print(selectType);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectType = 2;
                print(selectType);
            }
            BatJudgement(selectType);
      if (Input.GetKeyDown(KeyCode.Space))
        {
            stoneInputValue++;
            if(stoneMaxValue <stoneInputValue)
            {
                StartCoroutine(StoneAnimetion());
            }
        }
    }

    //키입력을 통해 맞는 오브젝트인지 확인후 애니메이션 재생
    void BatJudgement(int type)
    {
        if(currentObj == objects[type])
        {
            StartCoroutine(BatFlyAnimetion(type));
        }
        else
        {
            StartCoroutine(BatAttackAnimetion());
        }
        RandomCreateObject();
    }

    void RandomCreateObject()
    {
        print("1");
        int type = Random.Range(0, probability);
        if(type < objectProbability[0])
        {
            currentObj = objects[0];
            objects[0].SetActive(true);
        }
        else if(type < objectProbability[2] + objectProbability[1])
        {
            currentObj = objects[1];
            objects[1].SetActive(true);
        }
        else
        {
            stone = true;
            currentObj = objects[2];
            objects[2].SetActive(true);
            StartCoroutine(StoneObjectTimer());
            time = 10;
        }
        currentObj.SetActive(true);
    }
    //박쥐가 날아가는 애니메이션
    IEnumerator BatFlyAnimetion(int type)
    {
        Vector2 startPosition = new Vector2(0, 0);
        Vector2 peakPosition;
        Vector2 endPosition;
        if(type == 0 )
        {
            peakPosition = new Vector2(-3f, 4.5f);
            endPosition = new Vector2(-4, 0);
        }
        else
        {
            peakPosition = new Vector2(3f, 4.5f);
            endPosition = new Vector2(4, 0);
        }
        while (time < 1f)
        { 
            float t = time;
            float xPosition = Mathf.Pow(1 - t, 2) * startPosition.x + 2 * (1 - t) * t * peakPosition.x + Mathf.Pow(t, 2) * endPosition.x;
            float yPosition = Mathf.Pow(1 - t, 2) * startPosition.y + 2 * (1 - t) * t * peakPosition.y + Mathf.Pow(t, 2) * endPosition.y;
            currentObj.transform.localPosition = new Vector2(xPosition, yPosition);
            time += Time.deltaTime;
        }
        currentObj.SetActive(false);
        yield return null;
    }
    //박쥐가 공격하는 애니메이션
    IEnumerator BatAttackAnimetion()
    {
        yield return null;
    }

    //바위 오브젝트일때 타이머
    IEnumerator StoneObjectTimer()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            stoneTimerText.text = time.ToString("0.00");
            yield return null;
        }
        if (time < 0)
        {
            stoneTimerText.gameObject.SetActive(false);
            RandomCreateObject();
            stone = false;
            yield break;
        }
        yield return null;
    }

    IEnumerator StoneAnimetion()
    {
        yield return null;
    }
    
}
