using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTree : MonoBehaviour
{
    [SerializeField] GameObject obstacle;

    [HideInInspector] public bool canLog = false;

    private void Start()
    {
        canLog = false;
    }

    private void OnTriggerEnter(Collider other)
    {
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

        float angle = Mathf.Atan2(logDir.z, logDir.x) * Mathf.Rad2Deg;

        if (angle > 0 && angle <= 90)
            angle = -90;

        else if (angle > 90 && angle <= 180)
            angle = 180;

        else if (angle > -90 && angle <= 0)
            angle = 0;

        else if (angle > -180 && angle <= -90)
            angle = 90;

        return new Vector3(90, 0, -angle);
    }
}
