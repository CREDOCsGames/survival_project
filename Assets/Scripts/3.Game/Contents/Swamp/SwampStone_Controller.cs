using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwampStone_Controller : Singleton<SwampStone_Controller>
{
    [SerializeField] SwampStone stone;
    [SerializeField] float throwCooldown = 3f;
    Transform throwFrom;

    bool canThrowStone = false;
    Camera cam;
    float lastThrowTime = -Mathf.Infinity;

    public void CanThrowStone(bool canThrowStone)
    {
        this.canThrowStone = canThrowStone;
    }

    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }
    private void Start()
    {
        throwFrom = Character.Instance.transform;
    }
    private void Update()
    {
        if (!canThrowStone) return;

        if (ThrowInput() && CanThrow())
        {
            lastThrowTime = Time.time;

            stone.transform.position = throwFrom.position;

            Vector3 screenPos = Mouse.current.position.ReadValue();
            screenPos.z = Mathf.Abs(cam.transform.position.y - throwFrom.position.y);

            Vector3 targetPos = cam.ScreenToWorldPoint(screenPos);
            targetPos.y = throwFrom.position.y;

            if (!stone.gameObject.activeSelf)
                stone.gameObject.SetActive(true);

            stone.Throw(targetPos);
        }
    }
    private bool ThrowInput()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            return true;
        }
        return false;
    }
    bool CanThrow()
    {
        return Time.time >= lastThrowTime + throwCooldown;
    }
}
