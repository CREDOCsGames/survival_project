using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile_part : MonoBehaviour
{
    public enum Part { head, tail };
    public Part BodyPart { get { return bodyPart; } }

    [Header("Cache")]
    [SerializeField] Crocodile croco;
    [SerializeField] Part bodyPart;


    private void Start()
    {
        if(croco == null) croco = GetComponentInParent<Crocodile>();
    }


    public void Attacked(float damage, GameObject hitObject)
    {
        croco.OnHit(bodyPart);
    }
}
