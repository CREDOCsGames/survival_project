using UnityEngine;

public class CheckObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            transform.parent.parent.GetComponent<LogTree>().canLog = false;
            transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.parent.parent.GetComponent<LogTree>().canLog = true;
            transform.parent.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
}
