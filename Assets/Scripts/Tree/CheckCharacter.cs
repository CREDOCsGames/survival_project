using UnityEngine;

public class CheckCharacter : MonoBehaviour
{
    [SerializeField] GameObject arrow;

    private void Start()
    {
        arrow.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            arrow.SetActive(true);
            transform.parent.GetComponent<LogTree>().canLog = true;
            arrow.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            Vector3 logDir = -(Character.Instance.transform.position - transform.position).normalized;

            Dir(logDir);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            arrow.SetActive(false);
            transform.parent.GetComponent<LogTree>().canLog = false;
        }
    }

    void Dir(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(90, -angle, 0);
    }
}
