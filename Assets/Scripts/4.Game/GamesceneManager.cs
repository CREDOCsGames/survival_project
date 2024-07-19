using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesceneManager : Singleton<GamesceneManager>
{
    [SerializeField]

    bool isNight = false;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        StartCoroutine(DayRoutine());
    }

    IEnumerator DayRoutine()
    {
        isNight = false;

        yield return new WaitForSeconds(gameManager.gameDayTime);

        StartCoroutine(NightRoutine()); 
    }

    IEnumerator NightRoutine()
    {
        isNight = true;

        yield return new WaitForSeconds(gameManager.gameNightTime);

        gameManager.fishLowGradeCount = 0;
        gameManager.fishHighGradeCount = 0;

        if (gameManager.round <= 30)
            StartCoroutine(DayRoutine());
    }
}
