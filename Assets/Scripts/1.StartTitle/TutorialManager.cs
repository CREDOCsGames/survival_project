using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType
{
    StartTuto,
    StartItemTuto,
    BeachTuto,
    BushTuto,
    LogTuto,
    TamingTuto,
    CampFireTuto,
    FishingTuto,
    NightTuto,
    MulticellTuto,
    Count,
}

public class TutorialManager : Singleton<TutorialManager>
{
#if UNITY_EDITOR
    [SerializeField] bool isTuto;
#endif

    Dictionary<TutoType, int> tutorial;

    string[] tutoTexts;

    bool isTutoProgressing = false;

    [HideInInspector] public bool IsTutoProgressing => isTutoProgressing;

    private void Start()
    {
        tutorial = new Dictionary<TutoType, int>();

#if UNITY_EDITOR
        for (int i = 0; i < (int)TutoType.Count; i++)
        {
            if (!isTuto)
                tutorial.Add((TutoType)i, 1);

            else
                tutorial.Add((TutoType)i, 0);
        }
#else
        for (int i = 0; i < (int)TutoType.Count; i++)
        {
            tutorial.Add((TutoType)i, PlayerPrefs.GetInt(((TutoType)i).ToString(), 0));
        }

#endif

        tutoTexts = new string[(int)TutoType.Count];

        TutoTextContent();
    }

    public void ActiveTutoText(GameObject tutoPanel, GameObject closeText, Text text, TutoType tutoType)
    {
        if (tutorial[tutoType] == 1)
            return;

        isTutoProgressing = true;
        GameManager.Instance.isPause = true;

        text.text = tutoTexts[(int)tutoType];

        tutoPanel.SetActive(true);
        closeText.SetActive(false);

        StartCoroutine(CloseTutoPanel(tutoPanel, closeText, tutoType));
    }

    void TutoTextContent()
    {
        tutoTexts[(int)TutoType.StartTuto] = "이 섬, 화물선이 지나다니는 항로에 있는 섬인걸로 기억하는데...\n10일만 버텨보자. 배가 지나갈지도 몰라.";
        tutoTexts[(int)TutoType.StartItemTuto] = "그 녀석들이 던져주고 간 생필품은 챙겨야지.\n하지만 가지고 있을 손이 부족하니 일단 하나만 챙기자.";
        tutoTexts[(int)TutoType.BeachTuto] = "일단 해안가 주변에 떨어진 판자들을 좀 주워서 불이라도 때워야지.\n왠지 바다에서 떠밀려 온 물품도 주울 수 있을 것 같은데?";
        tutoTexts[(int)TutoType.BushTuto] = "이 수풀의 열매들 두고 두고 먹을 수 있겠는데? 좀 따가야겠어.\n그리고 이 정도 크기면 안에 무언가 더 있을 것 같아";
        tutoTexts[(int)TutoType.LogTuto] = "이 나무도 땔감으로 쓸 수 있겠는데?\n그리고 날 보호할 벽을 세우기에 안성맞춤이겠어.";
        tutoTexts[(int)TutoType.TamingTuto] = "이 케이지는 뭐지? 무언갈 잡을 수 있으려나.\n혹시 모르니 나중에 미끼를 둬봐야겠다.";
        tutoTexts[(int)TutoType.CampFireTuto] = "여기 누군가가 있었나? 불을 지핀 흔적이 있어.\n일단 밤이 오기 전에 아까 캐온 땔감으로 불을 지펴야겠다.\n물고기를 잡는다면 여기서 구워 먹어도 되겠는데?";
        tutoTexts[(int)TutoType.FishingTuto] = "저기 동쪽 방향에 절벽이 있는 것 같은데?\n저기서 낚시를 할 수 있으려나.";
        tutoTexts[(int)TutoType.NightTuto] = "왜 이렇게 음산하지… 꼭 유령이라도 나올 것만 같은데...\n혹시 모르니 준비 해야겠어.";
        tutoTexts[(int)TutoType.MulticellTuto] = "주워둔 물품들을 유용하게 사용할 수 있겠는데? 주머니에서 꺼내 착용해보자.";
    }

    IEnumerator CloseTutoPanel(GameObject tutoPanel, GameObject closeText, TutoType tutoType)
    {
        yield return CoroutineCaching.WaitForSeconds(0.5f);

        closeText.SetActive(true);

        yield return CoroutineCaching.WaitUntil(()=>Input.GetMouseButtonDown(0));

        tutoPanel.SetActive(false);
        GameManager.Instance.isPause = false;
        isTutoProgressing = false;

        PlayerPrefs.SetInt((tutoType).ToString(), 1);
        tutorial[tutoType] = 1;
    }
}
