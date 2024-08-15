using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverRotate : MonoBehaviour
{
    float angle;
    Vector3 dir, mouse;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isPause)
            LookMousePosition();
    }

    void LookMousePosition()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.y = 0;

        dir = mouse - transform.position;

        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(90, -angle + 180, 0);

        if (dir.x < 0)
            transform.rotation *= Quaternion.Euler(0, 0, 0);

        else
            transform.rotation *= Quaternion.Euler(180, 0, 0);
    }
}
