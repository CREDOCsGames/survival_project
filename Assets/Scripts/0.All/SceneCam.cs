using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCam : MonoBehaviour
{
    GameManager gameManager;
    Character lilpa;
    LoggingLilpa loggingLilpa;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager.currentScene == "Game")
            lilpa = Character.Instance;

        else if (gameManager.currentScene == "Logging")
            loggingLilpa = LoggingLilpa.Instance;
    }

    private void LateUpdate()
    {
        if (lilpa != null && !lilpa.isDead)
        {
            transform.position = new Vector3(Mathf.Clamp(lilpa.transform.position.x, -14f, 17f), transform.position.y, Mathf.Clamp(lilpa.transform.position.z, -49f, -31f));
            //transform.position = Vector3.Lerp(transform.position, new Vector3(character.transform.position.x, transform.position.y, character.transform.position.z), 5*Time.deltaTime);
        }

        else if (loggingLilpa != null)
            transform.position = new Vector3(Mathf.Clamp(loggingLilpa.transform.position.x, -14f, 17f), transform.position.y, Mathf.Clamp(loggingLilpa.transform.position.z, -9f, 9f));
    }

    /*private void Update()
    {
        if (lilpa != null && !lilpa.isDead)
        {
            transform.position = new Vector3(Mathf.Clamp(lilpa.transform.position.x, -14f, 17f), transform.position.y, Mathf.Clamp(lilpa.transform.position.z, -49f, -31f));
            //transform.position = Vector3.Lerp(transform.position, new Vector3(character.transform.position.x, transform.position.y, character.transform.position.z), 5*Time.deltaTime);
        }

        else if(loggingLilpa != null)
            transform.position = new Vector3(Mathf.Clamp(loggingLilpa.transform.position.x, -14f, 17f), transform.position.y, Mathf.Clamp(loggingLilpa.transform.position.z, -9f, 9f));
    }*/
}
