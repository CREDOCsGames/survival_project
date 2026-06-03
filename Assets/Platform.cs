using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour, IMouseHover, IReplaceDash
{
    public float interactDistance = 10f;
    [SerializeField] SpriteRenderer spriteRenderer;
    public bool IsAboard {  get; private set; }

    Transform character;
    Color outlineColor;
    bool enableAboardCheck = false;



    private void Start()
    {
        character = Character.Instance.transform;
        outlineColor = spriteRenderer.material.GetColor("_SolidOutline");
        IsAboard = false;
    }

    private void Update()
    {
        if (!enableAboardCheck) return;

        if (IsAboard && !IsWithinDistance(1f))
        {
            IsAboard = false;
        }
    }
    public void OnHoverEnter()
    {
        RefreshState();
    }

    public void OnHoverStay()
    {
        RefreshState();
    }

    public void OnHoverExit()
    {
        outlineColor.a = 0;
        spriteRenderer.material.SetColor("_SolidOutline", outlineColor);

        Character.Instance.ClearDashReplacer(this);
    }

    void RefreshState()
    {
        bool isClose = IsWithinDistance(interactDistance);

        if (isClose)
        {
            if (outlineColor.a != 1)
            {
                outlineColor.a = 1;
                spriteRenderer.material.SetColor("_SolidOutline", outlineColor);
            }

            Character.Instance.SetDashReplacer(this);
        }
        else
        {
            if (outlineColor.a != 0)
            {
                outlineColor.a = 0;
                spriteRenderer.material.SetColor("_SolidOutline", outlineColor);
            }

            Character.Instance.ClearDashReplacer(this);
        }
    }

    bool IsWithinDistance(float targetdistance)
    {
        float distance = Vector3.Distance(transform.position, character.position);
        return distance <= targetdistance;
    }

    public void Execute()
    {
        Character.Instance.JumpTo(transform.position);
        IsAboard = true;

        StartCoroutine(EnableCheckDelay());
    }
    IEnumerator EnableCheckDelay()
    {
        enableAboardCheck = false;
        yield return CoroutineCaching.WaitForSeconds(1f);
        enableAboardCheck = true;
    }

    private void OnDisable()
    {
        if (Character.Instance != null)
            Character.Instance.ClearDashReplacer(this);
    }

    private void OnDestroy()
    {
        if (Character.Instance != null)
            Character.Instance.ClearDashReplacer(this);
    }
}