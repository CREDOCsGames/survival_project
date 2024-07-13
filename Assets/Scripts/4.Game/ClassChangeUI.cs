using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassChangeUI : OptionUI
{
    [SerializeField] Image classImage;
    [SerializeField] Text className;

    Character character;
    int characterNum;

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;

        cursorNormal = gameManager.useCursorNormal;
        cursorAttack = gameManager.useCursorAttack;

        panel.SetActive(false);

        if (gameManager.round == 7)
        {
            for (int i = 1; i < 3; i++)
            {
                if (character.CheckEquipWeapon(i))
                {
                    PauseGame();
                    characterNum = i;
                    break;
                }
            }

            classImage.sprite = character.characterInfos[characterNum].CharacterImage;
            className.text = character.characterInfos[characterNum].CharacterName.ToString();
        }

        else if(gameManager.round == 8)
        {
            if(character.characterNum == 2 && gameManager.absorbHp >= 2)
            {
                PauseGame();
                characterNum = 3;
                classImage.sprite = character.characterInfos[characterNum].CharacterImage;
                className.text = character.characterInfos[characterNum].CharacterName.ToString();
            }
        }
    }

    private void Update()
    {
        
    }

    public void SelectChangeYes()
    {
        character.CharacterSetting(characterNum);

        ReturnToGame();
    }

    public void SelectChangeNo()
    {
        //gameManager.percentDamage *= 1.5f;
        ReturnToGame();
    }
}
