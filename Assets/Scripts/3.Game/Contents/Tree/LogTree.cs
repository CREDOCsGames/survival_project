using System.Collections;
using UnityEngine;

public class LogTree : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject obstacle;
    [SerializeField] Transform[] logPoses;

    bool canLog = false;
    [HideInInspector] int posNum;

    Character character;
    GameManager gameManager;
    GamesceneManager gamesceneManager;

    Vector3 obstacleAngle;

    private void Start()
    {
        canLog = false;
        character = Character.Instance;
        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;
    }

    private void Update()
    {
        if (gamesceneManager.isNight && canLog)
            canLog = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    Vector3 LogAngle()
    {
        Vector3 logDir = (Character.Instance.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(logDir.z, logDir.x) * Mathf.Rad2Deg;

        if (angle >= 0 && angle < 90)
        {
            posNum = 3;
            angle = 180;
        }

        else if (angle >= 90 && angle < 180)
        {
            posNum = 0;
            angle = -90;
        }

        else if (angle >= -90 && angle < 0)
        {
            posNum = 1;
            angle = 90;
        }

        else if (angle >= -180 && angle < -90)
        {
            posNum = 2;
            angle = 0;
        }

        return new Vector3(90, 0, angle);
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (canLog)
        {
            obstacleAngle = LogAngle();
            canLog = false;
            StartCoroutine(character.MoveToInteractableObject(logPoses[posNum].position, gameObject, 3 , 1, posNum));
        }
    }

    void SpawnObstacle()
    {
        GameObject ob = Instantiate(obstacle, transform.position, Quaternion.Euler(obstacleAngle), GamesceneManager.Instance.treeParent);
        ob.GetComponentInChildren<Obstacle>().SetObstacleImage(posNum);

        Destroy(gameObject);
    }

    public void CanInteraction(bool _canInteraction)
    {
        canLog = _canInteraction;
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        if (gameManager.specialStatus[SpecialStatus.DoubleAxe])
            waitTime *= 0.5f;

        yield return CoroutineCaching.WaitForSeconds(waitTime);

        if (gamesceneManager.isNight)
            yield break;

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

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        
    }

    public bool ReturnCanInteraction()
    {
        return canLog;
    }
}
