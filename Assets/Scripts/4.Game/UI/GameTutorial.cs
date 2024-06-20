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
        texts[0] = "반갑뇨. 나는 뇨파다뇨. 나의 탈출을 도울 수 있게 내가 설명해주겠다뇨.\n우선 이것은 내 체력이다뇨.\n몬스터에게 피격당하면 체력이 깎이고, 체력이 0이되면 탈출에 실패한다뇨.";
        texts[1] = "이 것은 경험치다뇨.\n몬스터를 잡으면 경험치가 차고, 경험치가 끝까지 다 차면 레벨업을 한다뇨.\n레벨업을 하면 체력이 1오르고, 하루가 끝난 후 능력치 카드중 하나를 선택할 수 있다뇨!";
        texts[2] = "그 다음은 날짜와 제한 시간이다뇨.\n제한 시간동안 몬스터들이 나온다뇨. 제한 시간이 지나면 하루가 끝나고 상점으로 넘어간다뇨.\n날짜가 30일이 되면 내가 탈출할 수 있는 조건이 된다뇨.\n또 하루가 끝나면, 몬스터를 잡고 남은 배트 코인들은 1코인으로 들어온다뇨.";
        texts[3] = "이 것은 이파리 나무다뇨.\n이파리 나무를 모아 배를 만들어서 탈출할거다뇨.\n30일이 끝나기 전까지 총 70개의 나무를 모아야한다뇨.\n만약 모으지 못한다면 탈출에 실패한다뇨.";
        texts[4] = "이파리 나무를 파괴하면 음식이 나오는데 먹으면 체력이 회복하고,\n최대 체력일 때 음식을 먹으면 5의 쉴드가 생긴다뇨. 최대 10까지 찬다뇨.\n제한시간 내에 부순 이파리 나무에선 가끔 특별아이템 보급이 나오기도 한다뇨!";
        texts[5] = "상점에서 대쉬 패시브 카드를 구매하면 이 곳에 대쉬 스킬창이 생성된다뇨.\n스킬 버튼을 누르면 캐릭터가 이동 방향으로 빠르게 돌진하게 하는 스킬이다뇨.\n시전 후 잠깐 무적상태가 된다뇨.";
        texts[6] = "마지막으로 이것은 일시 정지 버튼이다뇨.\nEsc키 혹은 이 버튼을 클릭하면 게임이 잠시 멈추며 옵션창과 능력치 창이 나온다뇨.\n게임 정보는 시작화면의 게임 정보에서 다시 확인할 수 있다뇨.\n그럼 힘내서 나와 탈출하자뇨!";
        texts[7] = "오늘은 보스 라운드다뇨!\n10, 20, 30일은 보스 라운드니까 마음의 준비를 하라뇨.\n보스 라운드는 보스 몬스터와 일반 몬스터가 함께 등장한다뇨.";
        texts[8] = "일반 몬스터는 제한 시간동안만 등장한다뇨.\n제한 시간내에 잡은 일반 몬스터는 배트코인과 경험치를 모두 주지만, \n제한 시간이 지나고 잡은 일반 몬스터는 경험치만 준다뇨.";
        texts[9] = "보스 몬스터는 10, 20일에는 한 마리씩,\n30일에는 10, 20일에 나온 보스 두마리가 모두 등장한다뇨.\n보스 몬스터를 잡으면 1레벨이 오르고 많은 배트코인을 준다뇨!";
        texts[10] = "10,20 라운드는 제한 시간이 끝나도 보스몬스터를 잡지 못하면 하루가 끝나지 않는다뇨.\n하지만 30일은 보스를 모두 잡으면 게임이 끝난다뇨.";
        texts[11] = "또 보스 몬스터는 제한시간이 끝나면 광폭화 상태가 된다뇨.\n광폭화가 되면 보스몬스터가 세고 빨라진다뇨!\n그럼 조심해서 잡아보자뇨!";
    }
}
