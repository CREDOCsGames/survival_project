using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowCard : MonoBehaviour
{
    [SerializeField] SelectSceneCard[] cards;

    int[] numArray;

    GameManager gameManager;
    SoundManager soundManager;
    Character character;

    private void Start()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        character = Character.Instance;

        numArray = new int[cards[0].weaponInfos.Length];
        GetRandomNum(cards.Length ,cards[0].weaponInfos.Length);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TitleScene();
    }

    void GetRandomNum(int count, int length)
    {
        for (int i = 0; i < count; i++)
        {
            numArray[i] = Random.Range(0, length);

            for (int j = 0; j < i; j++)
            {
                if (numArray[j] == numArray[i])
                {
                    i--;
                    break;
                }
            }
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].selectedWeapon = cards[i].weaponInfos[i];
            cards[i].selectedWeapon.weaponGrade = Grade.ÀÏ¹Ý;
        }
    }

    public void BackScene()
    {
        SoundManager.Instance.PlayES("SelectButton");
        Destroy(Character.Instance.gameObject);
        SceneManager.LoadScene("CharacterSelect");
    }

    public void TitleScene()
    {

        if (character.gameObject != null)
            Destroy(character.gameObject);

        if (soundManager.gameObject != null)
            Destroy(soundManager.gameObject);

        if (gameManager.gameObject != null)
            Destroy(gameManager.gameObject);

        SceneManager.LoadScene("StartTitle");
    }
}
