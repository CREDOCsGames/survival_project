using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoggingTree : MonoBehaviour
{
    [SerializeField] GameObject buttonUI;
    [SerializeField] TextMeshPro keyText;
    [SerializeField] int treeNum;

    Logging logging;
    bool isContact;
    bool isKeyPush;

    bool isBroekn;

    GameManager gameManager;
    LoggingLilpa lilpa;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        gameManager = GameManager.Instance;
        logging = Logging.Instance;
        lilpa = LoggingLilpa.Instance;

        buttonUI.SetActive(false);

        isContact = false;
        isKeyPush = false;
        isBroekn = false;

        keyText.text = ((KeyCode)PlayerPrefs.GetInt("Key_Dash")).ToString();
    }

    private void Update()
    {
        if (isContact)
        {
            if (!isKeyPush)
            {
                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Key_Dash")))
                {
                    buttonUI.SetActive(false);
                    lilpa.treePos = transform.position;
                    lilpa.treeIndex = treeNum;
                    lilpa.isCanControl = false;
                    lilpa.isAutoMove = true;

                    if (treeNum == 0)
                        logging.loggingTime = 5f;

                    else
                        logging.loggingTime = 3f;

                    logging.currentTime = logging.loggingTime;
                    isKeyPush = true;
                }
            }

            else
            {
                if (logging.isTreeDead && !logging.isLogging)
                {
                    if (logging.isSuccess)
                    {
                        isBroekn = true;
                        anim.SetBool("isBroken", isBroekn);
                    }

                    else
                    {
                        isContact = false;
                        logging.isTreeDead = false;
                        Destroy(gameObject);
                    }
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            buttonUI.SetActive(true);
            isContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            buttonUI.SetActive(false);

            if (!isKeyPush)
                isContact = false;
        }
    }

    public void TreeBroken()
    {
        if (treeNum == 0)
            gameManager.woodCount += 5;

        else
            gameManager.money += 200;

        logging.isSuccess = false;
        logging.isTreeDead = false;
        isContact = false;
        Destroy(gameObject);
    }
}
