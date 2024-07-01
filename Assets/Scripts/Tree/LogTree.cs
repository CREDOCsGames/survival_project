using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTree : MonoBehaviour
{
    [SerializeField] GameObject obstacle;

    public bool canLog = false;

    private void Start()
    {
        canLog = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (transform.GetChild(1).GetComponent<CheckObstacle>().canLog == false)
        if (canLog == false)
            return;

        if (other.CompareTag("myBullet") || other.CompareTag("Sword") || other.CompareTag("Thunder"))
        {
            GameObject brokenTree = Instantiate(obstacle);

            brokenTree.transform.position = transform.position;
            brokenTree.transform.eulerAngles = LogAngle();

            Destroy(gameObject);
        }
    }

    Vector3 LogAngle()
    {
        Vector3 logDir = (Character.Instance.transform.position - transform.position).normalized;

        return new Vector3(90, 0, Mathf.Atan2(logDir.z, logDir.x) * Mathf.Rad2Deg + 90);
    }
}
