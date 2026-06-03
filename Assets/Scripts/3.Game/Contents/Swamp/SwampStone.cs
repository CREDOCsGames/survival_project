using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwampStone : MonoBehaviour
{
    public Transform throwFrom;
    public float speed;
    public float lifetime;
    [HideInInspector] public Vector3 targetPos;

    Rigidbody rigid;
    Vector3 dir;
    Coroutine current;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + speed * Time.fixedDeltaTime * dir);
    }

    public void Throw(Vector3 targetPos)
    {
        dir = (targetPos - rigid.position).normalized;

        if (current != null)
        {
            StopCoroutine(current);
            current = null;
        }
        current = StartCoroutine(LiveForSeconds());
    }

    private void OnDisable()
    {
        if (current != null)
        {
            StopCoroutine(current);
            current = null;
        }
    }
    IEnumerator LiveForSeconds()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Crocodile_part>(out var crocodile))
        {
            crocodile.Attacked(0f, null);
            gameObject.SetActive(false);
        }
    }
}
