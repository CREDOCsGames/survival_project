using UnityEngine;
using UnityEngine.UI;

public class ShopTuto : MonoBehaviour
{
    [SerializeField] GameObject[] tutoUis;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] Text descripText;
    [SerializeField] Transform changePos;

    int count;

    string[] texts;

    void Start()
    {
        for (int i = 0; i < tutoUis.Length; i++)
            tutoUis[i].gameObject.SetActive(false);

        TextChange();

        descripText.text = texts[count];

        tutoUis[count].SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            tutoUis[count].SetActive(false);
            count++;
        }

        if (count < 9)
        {
            if (count == 4)
                descriptionPanel.transform.position = changePos.position;

            tutoUis[count].SetActive(true);
            descripText.text = texts[count];
        }

        else if (count >= 9)
        {
            gameObject.SetActive(false);
            PlayerPrefs.SetInt("ShopTuto", 0);
        }
    }

    void TextChange()
    {
        texts = new string[10];
        texts[0] = "이 곳은 상점이다뇨. \n여기있는 우리가 모은 배트코인으로 아이템들을 구매할 수 있다뇨.";
        texts[1] = "이 건 상점에 전시돼있는 아이템들을 새롭게 갱신해주는 초기화 버튼이다뇨.\n만약 전시된 아이템을 모두 구매한다면 자동으로 초기화된다뇨!";
        texts[2] = "이 건 능력치 창이다뇨.\n아이템을 구매하고 바뀌는 능력치들을 확인할 수 있다뇨.";
        texts[3] = "마우스 커서를 능력치 이름에 올리면 이렇게 능력치 설명창이 나온다뇨.\n꼼꼼히 읽어보고 어떤 능력치인지 알아보자뇨!";
        texts[4] = "이 건 패시브 아이템 슬롯이다뇨.\n구매한 패시브 아이템과 구매 갯수가 여기 표시된다뇨.";
        texts[5] = "만약 아이템을 구매하고 슬롯에 마우스를 올리면 이렇게 아이템 설명이 나온다뇨.";
        texts[6] = "이 건 무기 아이템 슬롯이다뇨.\n구매한 무기들이 여기에 장착된다뇨.";
        texts[7] = "카드를 클릭하면 이렇게 무기 설명과 판매, 합성버튼이 나온다뇨.\n합성은 동일한 무기를 합성시키면 상위등급으로 변하는 기능이다뇨.\n합성에는 합성비용이 소모된다뇨.\n만약 창을 닫고싶다면 취소버튼을 누르던가 정보창 밖을 클릭하면 된다뇨.";
        texts[8] = "마지막으로 이 버튼을 누르면 상점을 나가 다음 날 진행을 할 수 있다뇨.\n배트코인을 많이 모아서 좋은 아이템을 구매해 박쥐섬을 탈출해보자뇨!";
    }
}
