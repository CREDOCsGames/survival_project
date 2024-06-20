using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingLilpa : Singleton<LoggingLilpa>
{
    [SerializeField] float speed;
    [SerializeField] Collider ground;
    bool isRun;

    float x;
    float z;

    bool isDownUp = false;
    bool isLeftRight = false;

    [HideInInspector] public bool isLoggingAnimating;
    [HideInInspector] public bool isCanControl;
    [HideInInspector] public Vector3 treePos;
    [HideInInspector] public int treeIndex;
    [HideInInspector] public bool isAutoMove;

    Vector3 dir;

    Animator anim;
    SpriteRenderer rend;
    GameManager gameManager;
    Logging logging;
    LoggingSceneManager sceneManager;

    MoveRewardImage moveRewardImage;

    void Start()
    {
        speed = 5;
        isLoggingAnimating = false;
        isCanControl = true;
        isAutoMove = false;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
        logging = Logging.Instance;
        sceneManager = LoggingSceneManager.Instance;
        moveRewardImage = MoveRewardImage.Instance;

    }

    void Update()
    {
        if (!gameManager.isPause)
        {
            if (isCanControl)
            {
                if (!sceneManager.isEnd)
                    Move();

                else
                    isRun = false;
            }

            else
            {
                if (isAutoMove)
                {
                    if (!logging.isLogging)
                        MoveToTree();

                    else
                        MoveToTreeSide();
                }
            }

            anim.SetFloat("moveSpeed", 1 + (speed * 0.1f));
            anim.SetBool("isRun", isRun);
            anim.SetBool("isLogging", isLoggingAnimating);
            anim.SetBool("isFail", !logging.isSuccess);
        }
    }

    void Move()
    {
        bool xInput = (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Left"))) || (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")));
        bool zInput = (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Up"))) || (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")));

        if (!xInput)
            x = 0;

        if (!zInput)
            z = 0;

        if (zInput)
        {
            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Up")))
            {
                z = 1;

                if (!isDownUp && Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")))
                    z = -1;
            }

            else if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")))
            {
                z = -1;
                isDownUp = true;
            }

            if (Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt("Key_Down")))
                isDownUp = false;
        }

        if (xInput)
        {
            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Left")))
            {
                x = -1;
                rend.flipX = true;

                if (!isLeftRight && Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")))
                {
                    x = 1;
                    rend.flipX = false;
                }
            }

            else if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")))
            {
                x = 1;
                rend.flipX = false;
                isLeftRight = true;
            }

            if (Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt("Key_Right")))
                isLeftRight = false;
        }

        dir = (Vector3.right * x + Vector3.forward * z).normalized;

        transform.position += dir * speed * Time.deltaTime;

        transform.position = ground.bounds.ClosestPoint(transform.position);

        if (dir != Vector3.zero)
            isRun = true;

        else if (dir == Vector3.zero)
            isRun = false;
    }

    void MoveToTree()
    {
        isRun = true;
        transform.position = Vector3.MoveTowards(transform.position, treePos, Time.deltaTime * speed);

        if (transform.position == treePos)
        {
            logging.isLogging = true;
            logging.KeyHit();
            isAutoMove = false;
            isRun = false;
        }
    }

    void MoveToTreeSide()
    {
        isRun = true;
        Vector3 movePos = new Vector3(treePos.x + 1, treePos.y, treePos.z - 0.5f);
        transform.position = Vector3.MoveTowards(transform.position, movePos, Time.deltaTime * speed);

        if (transform.position == movePos)
        {
            isAutoMove = false;
            isRun = false;
            logging.isLogging = false;

            moveRewardImage.ActiveSetting();
            moveRewardImage.gameObject.SetActive(true);
        }
    }
}
