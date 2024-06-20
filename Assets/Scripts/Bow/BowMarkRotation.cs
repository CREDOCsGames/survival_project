using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowMarkRotation : MonoBehaviour
{
    float angle;
    Vector3 dir, mouse;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.y = 0;

            dir = mouse - transform.position;

            LookMousePosition();
        }
    }

    void LookMousePosition()
    {
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -angle, 0);

        if (dir.x < 0)
            transform.rotation *= Quaternion.Euler(180, 0, 0);

        else
            transform.rotation *= Quaternion.Euler(0, 0, 0);
    }
}
