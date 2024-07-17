using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTree : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject obstacle;
    [SerializeField] Transform[] logPoses;

    bool canLog = false;
    [HideInInspector] int posNum;

    Character character;
    GameManager gameManager;

    Vector3 obstaclePos;

    private void Start()
    {
        canLog = false;
        character = Character.Instance;
        gameManager = GameManager.Instance;
    }

    Vector3 LogAngle()
    {
        Vector3 logDir = (Character.Instance.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(logDir.z, logDir.x) * Mathf.Rad2Deg;

        if (angle > 0 && angle <= 90)
        {
            posNum = 3;
            angle = -90;
        }

        else if (angle > 90 && angle <= 180)
        {
            posNum = 0;
            angle = 180;
        }

        else if (angle > -90 && angle <= 0)
        {
            posNum = 1;
            angle = 0;
        }

        else if (angle > -180 && angle <= -90)
        {
            posNum = 2;
            angle = 90;
        }

        return new Vector3(90, 0, -angle);
    }

    public void InteractionFuc(GameObject hitObject)
    {
        if (canLog)
        {
            obstaclePos = LogAngle();

            StartCoroutine(character.MoveToInteractableObject(logPoses[posNum].position, this.gameObject));
        }
    }

    void SpawnObstacle()
    {
        GameObject brokenTree = Instantiate(obstacle);

        brokenTree.transform.position = transform.position;
        brokenTree.transform.eulerAngles = obstaclePos;

        Destroy(gameObject);
    }

    public void CanInteraction(bool canInteraction)
    {
        canLog = canInteraction;
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        SpawnObstacle();

        int rand = Random.Range(1, 4);

        gameManager.woodCount += rand;
        anim.SetBool("isLogging", false);
        character.isCanControll = true;
        character.ChangeAnimationController(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2);
    }
}
