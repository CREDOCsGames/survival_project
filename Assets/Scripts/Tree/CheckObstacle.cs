using UnityEngine;

public class CheckObstacle : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            //Debug.DrawRay(transform.position, -(Character.Instance.transform.position - transform.position).normalized * 3.5f, Color.red);

            //arrow.SetActive(true);

            Vector3 logDir = -(Character.Instance.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, logDir, 3.5f, obstacleLayer))
            {
                canLog = false;
                transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
            }

            else
            {
                canLog = true;
                transform.parent.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer.ToString());
        Debug.Log(obstacleLayer.ToString());

        if(other.CompareTag("Obstacle"))
        {
            Debug.Log("ye");
            transform.parent.parent.GetComponent<LogTree>().canLog = false;
            transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
        }

        if(other.gameObject.layer == obstacleLayer)
        {
            Debug.Log("s");
            transform.parent.parent.GetComponent<LogTree>().canLog = false;
            transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == obstacleLayer)
        {
            transform.parent.parent.GetComponent<LogTree>().canLog = true;
            transform.parent.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    /*void Dir(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(90, -angle, 0);
    }*/

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 logDir = -(Character.Instance.transform.position - transform.position).normalized;
        Gizmos.DrawWireCube(transform.position + logDir * 0.57f, logDir.normalized * 3.5f);

    }*/
}
