using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoggingSceneManager : Singleton<LoggingSceneManager>
{
    [SerializeField] Text timeText;
    [SerializeField] Text roundText;
    [SerializeField] Text woodCountText;
    [SerializeField] Text coinText;
    [SerializeField] GameObject nextRoundPanel;
    [SerializeField] GameObject treeParent;
    [SerializeField] GameObject[] treePrefabs;
    [SerializeField] Collider ground;

    GameManager gameManager;

    float time;
    float currentTime;
    [HideInInspector] public bool isEnd;

    private void Start()
    {
        gameManager = GameManager.Instance;

        time = 60f;
        currentTime = time;
        isEnd = false;;
        timeText.text = time.ToString("F2");
        woodCountText.text = gameManager.woodCount.ToString();
        roundText.text = gameManager.round.ToString();
        coinText.text = gameManager.money.ToString();
        nextRoundPanel.SetActive(isEnd);

        TreeInstance();
    }

    private void Update()
    {
        if (treeParent.transform.childCount > 0)
        {
            if (currentTime > 0)
            {
                currentTime = Mathf.Clamp(currentTime - Time.deltaTime, 0f, time);
                timeText.text = currentTime.ToString("F2");
            }

            else if ((currentTime == 0f) && !isEnd)
            {
                isEnd = true;
                treeParent.SetActive(false);
                nextRoundPanel.SetActive(isEnd);
                currentTime = -100f;
            }
        }

        else
        {
            isEnd = true;
            treeParent.SetActive(false);
            nextRoundPanel.SetActive(isEnd);
            currentTime = -100f;
        }


        woodCountText.text = gameManager.woodCount.ToString();
        coinText.text = gameManager.money.ToString();
    }

    List<Vector3> posList;

    void TreeInstance()
    {
        if (posList == null)
        {
            posList = new List<Vector3>();

            for (int i = -19; i <= 21; i += 2)
            {
                for (int j = -11; j <= 11; j += 2)
                {
                    posList.Add(new Vector3(i, 0, j));
                }
            }
        }

        for (int i = 0; i < 30; i++)
        {
            int rand = Random.Range(0, 100);
            GameObject tree;

            if (rand < 95)
                tree = Instantiate(treePrefabs[0]);

            else
                tree = Instantiate(treePrefabs[1]);

            tree.transform.SetParent(treeParent.transform);


            int randPos = Random.Range(0, posList.Count - 1);

            tree.transform.position = ground.bounds.ClosestPoint(posList[randPos]);

            posList.RemoveAt(randPos);
        }
    }

    public void ToGameScene()
    {
        gameManager.round++;
        gameManager.ToNextScene("Game");
    }
}
