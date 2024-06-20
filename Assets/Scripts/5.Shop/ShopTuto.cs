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
        texts[0] = "�� ���� �����̴ٴ�. \n�����ִ� �츮�� ���� ��Ʈ�������� �����۵��� ������ �� �ִٴ�.";
        texts[1] = "�� �� ������ ���õ��ִ� �����۵��� ���Ӱ� �������ִ� �ʱ�ȭ ��ư�̴ٴ�.\n���� ���õ� �������� ��� �����Ѵٸ� �ڵ����� �ʱ�ȭ�ȴٴ�!";
        texts[2] = "�� �� �ɷ�ġ â�̴ٴ�.\n�������� �����ϰ� �ٲ�� �ɷ�ġ���� Ȯ���� �� �ִٴ�.";
        texts[3] = "���콺 Ŀ���� �ɷ�ġ �̸��� �ø��� �̷��� �ɷ�ġ ����â�� ���´ٴ�.\n�Ĳ��� �о�� � �ɷ�ġ���� �˾ƺ��ڴ�!";
        texts[4] = "�� �� �нú� ������ �����̴ٴ�.\n������ �нú� �����۰� ���� ������ ���� ǥ�õȴٴ�.";
        texts[5] = "���� �������� �����ϰ� ���Կ� ���콺�� �ø��� �̷��� ������ ������ ���´ٴ�.";
        texts[6] = "�� �� ���� ������ �����̴ٴ�.\n������ ������� ���⿡ �����ȴٴ�.";
        texts[7] = "ī�带 Ŭ���ϸ� �̷��� ���� ����� �Ǹ�, �ռ���ư�� ���´ٴ�.\n�ռ��� ������ ���⸦ �ռ���Ű�� ����������� ���ϴ� ����̴ٴ�.\n�ռ����� �ռ������ �Ҹ�ȴٴ�.\n���� â�� �ݰ�ʹٸ� ��ҹ�ư�� �������� ����â ���� Ŭ���ϸ� �ȴٴ�.";
        texts[8] = "���������� �� ��ư�� ������ ������ ���� ���� �� ������ �� �� �ִٴ�.\n��Ʈ������ ���� ��Ƽ� ���� �������� ������ ���㼶�� Ż���غ��ڴ�!";
    }
}
