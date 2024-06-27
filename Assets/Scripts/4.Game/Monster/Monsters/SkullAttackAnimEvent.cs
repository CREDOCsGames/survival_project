using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class SkullAttackAnimEvent : MonoBehaviour
{
    SkullBat parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<SkullBat>();
    }

    public void ShootAnimEvent()
    {
        parent.Shoot();
    }

    public void EndAttackAnimEvent()
    {
        parent.EndAttack();
    }
}
