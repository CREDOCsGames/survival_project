using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseInteraction
{
    void InteractionFuc(GameObject hitObject);
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
                hit[0].transform.gameObject.GetComponent<IMouseInteraction>().InteractionFuc(hit[0].transform.gameObject);
            }
        }
    }
}
