using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamingPetMovement : MonoBehaviour
{
    [SerializeField] Transform spawnPos;
    [SerializeField] Transform arrivePos;
    [SerializeField] float moveSpeed;

    void Start()
    {
        transform.position = spawnPos.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,arrivePos.position, Time.deltaTime * moveSpeed);
    }

    private void OnEnable()
    {
        transform.position = spawnPos.position;
    }
}
