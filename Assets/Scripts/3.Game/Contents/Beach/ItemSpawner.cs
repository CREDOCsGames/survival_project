using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : Singleton<ItemSpawner>
{
    [SerializeField] BoxCollider beachArea;
    [SerializeField] GameObject dropItem;
    [SerializeField] int spawnAmount;
    [SerializeField] bool isCheckOtherItem;
    [SerializeField] LayerMask interactionLayer;

    public IEnumerator SpawnItem()
    {
        foreach(Transform item in transform)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < spawnAmount; ++i)
        {
            Instantiate(dropItem, GetSpawnPosWithCheck(), dropItem.transform.rotation, transform);
            yield return null;
        }
    }

    Vector3 GetSpawnPosWithCheck()
    {
        Vector3 spawnPos = GetRandomSpawnPos();

        if (!isCheckOtherItem)
            return spawnPos;

        while (true)
        {
            if (Physics.OverlapSphere(spawnPos, 2f, interactionLayer).Length <= 0)
            {
                break;
            }

            spawnPos = GetRandomSpawnPos();
        }

        return spawnPos;
    }

    Vector3 GetRandomSpawnPos()
    {
        float groundX = beachArea.bounds.size.x;
        float groundZ = beachArea.bounds.size.z;

        groundX = Random.Range((groundX / 2f) * -1f + beachArea.bounds.center.x, (groundX / 2f) + beachArea.bounds.center.x);
        groundZ = Random.Range((groundZ / 2f) * -1f + beachArea.bounds.center.z, (groundZ / 2f) + beachArea.bounds.center.z);

        return new Vector3(groundX, 0, groundZ);
    }
}
