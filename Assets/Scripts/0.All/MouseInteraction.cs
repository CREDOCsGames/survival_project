using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseInteraction
{
    void InteractionLeftButtonFuc(GameObject hitObject);
    void InteractionRightButtonFuc(GameObject hitObject);

    void CanInteraction(bool _canInteraction);
    bool ReturnCanInteraction();
    IEnumerator EndInteraction(Animator anim, float waitTime);
}
public interface IMouseHover
{
    void OnHoverEnter();
    void OnHoverStay();
    void OnHoverExit();
}

public class MouseInteraction : MonoBehaviour
{   
    [SerializeField] LayerMask layerMask;

    Character character;

    RaycastHit[] hoverhitBuffer;
    HashSet<IMouseHover> prevHovers = new();
    HashSet<IMouseHover> currHovers = new();

    private void Start()
    {
        character = Character.Instance;

        hoverhitBuffer = new RaycastHit[32];
    }

    private void Update()
    {
        if (!character.isCanControll)
            return;

        HandleHover();

        if (Input.GetMouseButtonUp(0))
        {
            MouseInteractionFuc((gameObject) => { gameObject.GetComponent<IMouseInteraction>().InteractionLeftButtonFuc(gameObject); });
        }

        else if (Input.GetMouseButtonUp(1))
        {
            MouseInteractionFuc((gameObject) => { gameObject.GetComponent<IMouseInteraction>().InteractionRightButtonFuc(gameObject); });
        }
    }

    void MouseInteractionFuc(System.Action<GameObject> interactionFuc)
    {
        if (GamesceneManager.Instance.isNight)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hit = Physics.RaycastAll(ray, 100, layerMask);

        if (hit.Length <= 0)
            return;

        for (int i = 0; i < hit.Length; i++)
        {
            var interactable = hit[i].transform.gameObject.GetComponent<IMouseInteraction>();

            if (interactable != null)
            {
                interactionFuc(hit[i].transform.gameObject);

                return;
            }
        }
    }
    private void HandleHover()
    {
        currHovers.Clear();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int hitCount = Physics.RaycastNonAlloc(ray, hoverhitBuffer, 100f, layerMask);

        // 수집 및 중복 제거
        for (int i = 0; i < hitCount; i++)
        {
            var tr = hoverhitBuffer[i].transform;
            if (!tr) continue;

            var hovers = tr.GetComponentsInParent<IMouseHover>();
            for (int j = 0; j < hovers.Length; j++)
                currHovers.Add(hovers[j]);
        }
        
        // Exit
        foreach (var h in prevHovers)
        {
            if (!currHovers.Contains(h))
                h.OnHoverExit();
        }

        // Enter
        foreach (var h in currHovers)
        {
            if (!prevHovers.Contains(h))
            {
                h.OnHoverEnter();
            }
        }

        // Stay
        foreach (var h in currHovers)
        {
            h.OnHoverStay();
        }

        // prev <- curr 복사
        prevHovers.Clear();
        foreach (var h in currHovers)
            prevHovers.Add(h);
    }
}
