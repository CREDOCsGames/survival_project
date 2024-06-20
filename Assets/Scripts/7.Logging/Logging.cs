using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logging : Singleton<Logging>
{
    [SerializeField] RectTransform arrowParent;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI failText;
    
    [HideInInspector] public bool isLogging;

    int count;
    [HideInInspector] public float loggingTime;
    [HideInInspector] public float currentTime;

    bool isStart;
    bool isArrowClear;
    [HideInInspector] public bool isSuccess;
    [HideInInspector] public bool isTreeDead;

    string arrStr;
    KeyCode arrKey;
    KeyCode checkKey;

    LoggingSceneManager sceneManager;
    LoggingLilpa lilpa;

    bool coroutineEnd = false;

    bool isKeyPush;

    void Start()
    {
        sceneManager = LoggingSceneManager.Instance;
        lilpa = LoggingLilpa.Instance;

        isLogging = false;
        isArrowClear = false;
        isTreeDead = false;
        isSuccess = false;
        isKeyPush = false;
        count = 0;

        timeText.gameObject.SetActive(false);
        failText.gameObject.SetActive(false);

        arrowParent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!sceneManager.isEnd)
        {
            if (isLogging)
            {
                if (!isArrowClear)
                {
                    timeText.text = currentTime.ToString("F2");
                    currentTime -= Time.deltaTime;

                    if (currentTime <= 0)
                        isArrowClear = true;

                    Vector3 arrRot = arrowParent.GetChild(count).localRotation.eulerAngles;

                    switch (arrRot.z)
                    {
                        // up
                        case 0:
                            arrStr = "Key_Up";
                            arrKey = KeyCode.UpArrow;
                            break;

                        // down
                        case 180:
                            arrStr = "Key_Down";
                            arrKey = KeyCode.DownArrow;
                            break;

                        // right
                        case 270:
                            arrStr = "Key_Right";
                            arrKey = KeyCode.RightArrow;
                            break;

                        // left
                        case 90:
                            arrStr = "Key_Left";
                            arrKey = KeyCode.LeftArrow;
                            break;
                    }

                    if (isKeyPush)
                    {
                        if (checkKey == arrKey || checkKey == (KeyCode)PlayerPrefs.GetInt(arrStr))
                        {
                            checkKey = KeyCode.None;
                            arrowParent.GetChild(count).gameObject.SetActive(false);
                            isKeyPush = false;
                        }

                        else
                        {
                            if (checkKey != KeyCode.None && checkKey != (KeyCode)PlayerPrefs.GetInt("Key_Dash"))
                            {
                                isArrowClear = true;
                                arrowParent.GetChild(count).GetComponent<Image>().color = Color.red;
                                isLogging = false;
                                StartCoroutine(ActiveText("실패...", Color.red));
                                isKeyPush = false;
                            }
                        }
                    }

                    if (Input.GetKeyUp(arrKey) || Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt(arrStr)))
                    {
                        isStart = true;
                        count++;
                    }

                    if (count >= arrowParent.childCount)
                    {
                        isArrowClear = true;
                        lilpa.isAutoMove = true;
                        isSuccess = true;
                        isStart = false;

                        StartCoroutine(ActiveText("성공!", Color.cyan));
                    }
                }

                else
                {
                    if (currentTime <= 0)
                    {
                        isStart = false;
                        checkKey = KeyCode.None;
                        isLogging = false;
                        StartCoroutine(ActiveText("시간 초과...", Color.red));
                    }
                }
            }
        }

        else
        {
            if (!coroutineEnd)
            {
                arrowParent.gameObject.SetActive(false);
                isLogging = false;
                StartCoroutine(ActiveText("벌목 종료!", Color.yellow));
                coroutineEnd = true;
            }
        }
    }

    private void OnGUI()
    {
        if (isStart)
        {
            Event keyEvent = Event.current;

            if (!isKeyPush && keyEvent.isKey)
            {
                if (keyEvent.keyCode != (KeyCode)PlayerPrefs.GetInt("Key_Dash"))
                {
                    checkKey = keyEvent.keyCode;
                    isKeyPush = true;
                    isStart = false;
                }

                /*if (keyEvent.keyCode != KeyCode.None && keyEvent.keyCode != (KeyCode)PlayerPrefs.GetInt("Key_Dash"))
                {
                    Debug.Log(keyEvent.keyCode);
                    checkKey = keyEvent.keyCode;
                    isKeyPush = true;
                }*/
            }

            /*if (keyEvent.isKey)
            {
                Debug.Log(keyEvent.keyCode);
                checkKey = keyEvent.keyCode;
                isKeyPush = true;
            }*/
        }
    }

    public void KeyHit()
    {
        lilpa.isLoggingAnimating = true;

        for (int i = 0; i < arrowParent.transform.childCount; i++)
        {
            int rand = Random.Range(0, 3);
            arrowParent.GetChild(i).localRotation = Quaternion.Euler(0, 0, rand * 90);
            arrowParent.GetChild(i).GetComponent<Image>().color = Color.white;

            if (!arrowParent.GetChild(i).gameObject.activeSelf)
                arrowParent.GetChild(i).gameObject.SetActive(true);
        }

        arrowParent.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        isStart = true;
    }

    void Initialize()
    {
        isArrowClear = false;
        count = 0;
        currentTime = loggingTime;
        arrowParent.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
    }

    IEnumerator ActiveText(string str, Color color)
    {
        isTreeDead = true;
        lilpa.isLoggingAnimating = false;
        timeText.gameObject.SetActive(false);
        failText.text = str;
        failText.color = color;
        failText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        failText.gameObject.SetActive(false);
        lilpa.isCanControl = true;
        Initialize();
    }
}
