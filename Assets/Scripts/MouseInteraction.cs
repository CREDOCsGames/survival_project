using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseInteraction
{
    void InteractionLeftButtonFuc(GameObject hitObject);
    void InteractionRightButtonFuc(GameObject hitObject);

    void CanInteraction(bool canInteraction);
    IEnumerator EndInteraction(Animator anim, float waitTime);
}

public class MouseInteraction : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hit = Physics.RaycastAll(ray, 100, layerMask);

            if (hit.Length > 0)
            {
                if (hit[0].transform.gameObject.GetComponent<IMouseInteraction>() != null)
                    hit[0].transform.gameObject.GetComponent<IMouseInteraction>().InteractionLeftButtonFuc(hit[0].transform.gameObject);
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hit = Physics.RaycastAll(ray, 100, layerMask);

            if (hit.Length > 0)
            {
                if (hit[0].transform.gameObject.GetComponent<IMouseInteraction>() != null)
                    hit[0].transform.gameObject.GetComponent<IMouseInteraction>().InteractionRightButtonFuc(hit[0].transform.gameObject);
            }
        }
    }
}
