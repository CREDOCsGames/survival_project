using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject LockImage;

    bool[] characterClear;

    GameManager gameManager;
    SoundManager soundManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;

        Cursor.lockState = CursorLockMode.Confined;
        characterClear = new bool[(int)CHARACTER_NUM.Count];
        characterClear[(int)CHARACTER_NUM.Bagic] = Convert.ToBoolean(PlayerPrefs.GetInt("BagicClear", 0));
        LockImage.SetActive(!characterClear[(int)CHARACTER_NUM.Bagic]);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            TitleScene();
    }

    public void SelectCharacter(int num)
    {
        SoundManager.Instance.PlayES("SelectButton");
        GameObject character = Instantiate(characterPrefab, Vector3.zero, characterPrefab.transform.rotation);
        character.GetComponent<NavMeshAgent>().enabled = false;
        character.GetComponent<Character>().characterNum = num;
        //character.SetActive(false);
        SceneManager.LoadScene("WeaponSelect");
    }

    public void TitleScene()
    {
        if (soundManager.gameObject != null)
            Destroy(soundManager.gameObject);

        if (gameManager.gameObject != null)
            Destroy(gameManager.gameObject);

        SceneManager.LoadScene("StartTitle");
    }
}
