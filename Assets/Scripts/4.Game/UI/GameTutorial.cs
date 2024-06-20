using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class GameTutorial : MonoBehaviour
{
    [SerializeField] GameObject[] tutoUis;
    [SerializeField] Text descripText;

    GameManager gameManager;

    int count;

    string[] texts;

    private void Start()
    {
        Time.timeScale = 0;

        gameManager = GameManager.Instance;
        gameManager.isPause = true;
        gameManager.isTuto = true;

        if (gameManager.round == 1)
            count = 0;

        else if (gameManager.round == 10)
            count = 7;

        for (int i = 0; i < tutoUis.Length; i++)
            tutoUis[i].gameObject.SetActive(false);

        TextChange();

        descripText.text = texts[count];

        tutoUis[count].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (gameManager.round == 1)
                tutoUis[count].SetActive(false);

            count++;

            if (gameManager.round == 1)
            {
                if (count < 7)
                {
                    tutoUis[count].SetActive(true);
                    descripText.text = texts[count];
                }

                else if (count >= 7)
                {
                    gameManager.isPause = false;
                    gameManager.isTuto = false;
                    Time.timeScale = 1;
                    gameObject.SetActive(false);
                    PlayerPrefs.SetInt("GameTuto", 0);
                    GameSceneUI.Instance.monsterSpawn.SetActive(true);
                }
            }

            if(gameManager.round == 10)
            {
                if (count < 12)
                    descripText.text = texts[count];

                else if(count >= 12)
                {
                    gameManager.isPause = false;
                    gameManager.isTuto = false;
                    Time.timeScale = 1;
                    gameObject.SetActive(false);
                    PlayerPrefs.SetInt("BossTuto", 0);
                    GameSceneUI.Instance.monsterSpawn.SetActive(true);
                }
            }
        }
    }

    void TextChange()
    {
        texts = new string[12];
        texts[0] = "�ݰ���. ���� ���Ĵٴ�. ���� Ż���� ���� �� �ְ� ���� �������ְڴٴ�.\n�켱 �̰��� �� ü���̴ٴ�.\n���Ϳ��� �ǰݴ��ϸ� ü���� ���̰�, ü���� 0�̵Ǹ� Ż�⿡ �����Ѵٴ�.";
        texts[1] = "�� ���� ����ġ�ٴ�.\n���͸� ������ ����ġ�� ����, ����ġ�� ������ �� ���� �������� �Ѵٴ�.\n�������� �ϸ� ü���� 1������, �Ϸ簡 ���� �� �ɷ�ġ ī���� �ϳ��� ������ �� �ִٴ�!";
        texts[2] = "�� ������ ��¥�� ���� �ð��̴ٴ�.\n���� �ð����� ���͵��� ���´ٴ�. ���� �ð��� ������ �Ϸ簡 ������ �������� �Ѿ�ٴ�.\n��¥�� 30���� �Ǹ� ���� Ż���� �� �ִ� ������ �ȴٴ�.\n�� �Ϸ簡 ������, ���͸� ��� ���� ��Ʈ ���ε��� 1�������� ���´ٴ�.";
        texts[3] = "�� ���� ���ĸ� �����ٴ�.\n���ĸ� ������ ��� �踦 ���� Ż���ҰŴٴ�.\n30���� ������ ������ �� 70���� ������ ��ƾ��Ѵٴ�.\n���� ������ ���Ѵٸ� Ż�⿡ �����Ѵٴ�.";
        texts[4] = "���ĸ� ������ �ı��ϸ� ������ �����µ� ������ ü���� ȸ���ϰ�,\n�ִ� ü���� �� ������ ������ 5�� ���尡 ����ٴ�. �ִ� 10���� ���ٴ�.\n���ѽð� ���� �μ� ���ĸ� �������� ���� Ư�������� ������ �����⵵ �Ѵٴ�!";
        texts[5] = "�������� �뽬 �нú� ī�带 �����ϸ� �� ���� �뽬 ��ųâ�� �����ȴٴ�.\n��ų ��ư�� ������ ĳ���Ͱ� �̵� �������� ������ �����ϰ� �ϴ� ��ų�̴ٴ�.\n���� �� ��� �������°� �ȴٴ�.";
        texts[6] = "���������� �̰��� �Ͻ� ���� ��ư�̴ٴ�.\nEscŰ Ȥ�� �� ��ư�� Ŭ���ϸ� ������ ��� ���߸� �ɼ�â�� �ɷ�ġ â�� ���´ٴ�.\n���� ������ ����ȭ���� ���� �������� �ٽ� Ȯ���� �� �ִٴ�.\n�׷� ������ ���� Ż�����ڴ�!";
        texts[7] = "������ ���� ����ٴ�!\n10, 20, 30���� ���� ����ϱ� ������ �غ� �϶�.\n���� ����� ���� ���Ϳ� �Ϲ� ���Ͱ� �Բ� �����Ѵٴ�.";
        texts[8] = "�Ϲ� ���ʹ� ���� �ð����ȸ� �����Ѵٴ�.\n���� �ð����� ���� �Ϲ� ���ʹ� ��Ʈ���ΰ� ����ġ�� ��� ������, \n���� �ð��� ������ ���� �Ϲ� ���ʹ� ����ġ�� �شٴ�.";
        texts[9] = "���� ���ʹ� 10, 20�Ͽ��� �� ������,\n30�Ͽ��� 10, 20�Ͽ� ���� ���� �θ����� ��� �����Ѵٴ�.\n���� ���͸� ������ 1������ ������ ���� ��Ʈ������ �شٴ�!";
        texts[10] = "10,20 ����� ���� �ð��� ������ �������͸� ���� ���ϸ� �Ϸ簡 ������ �ʴ´ٴ�.\n������ 30���� ������ ��� ������ ������ �����ٴ�.";
        texts[11] = "�� ���� ���ʹ� ���ѽð��� ������ ����ȭ ���°� �ȴٴ�.\n����ȭ�� �Ǹ� �������Ͱ� ���� �������ٴ�!\n�׷� �����ؼ� ��ƺ��ڴ�!";
    }
}
