using UnityEngine;

public class CheckCharacter : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject clickUI;

    private void Start()
    {
        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }

        clickUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (arrow != null)
            {
                arrow.SetActive(true);
                arrow.GetComponent<SpriteRenderer>().color = Color.blue;
            }

            clickUI.SetActive(true);
            transform.parent.GetComponent<IMouseInteraction>().CanInteraction(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (arrow != null && other.CompareTag("Character"))
        {
            Vector3 logDir = -(Character.Instance.transform.position - transform.position).normalized;
            ArrowRatateToDir(logDir);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (arrow != null)
            {
                arrow.SetActive(false);
            }

            clickUI.SetActive(false);
            transform.parent.GetComponent<IMouseInteraction>().CanInteraction(false);
        }
    }

    void ArrowRatateToDir(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        if (angle >= 0 && angle < 90)
            angle = 0;

        else if (angle >= 90 && angle < 180)
            angle = 90;

        else if (angle >= -90 && angle < 0)
            angle = -90;

        else if (angle >= -180 && angle < -90)
            angle = -180;

        arrow.transform.rotation = Quaternion.Euler(90, -angle, 0);
    }
}
