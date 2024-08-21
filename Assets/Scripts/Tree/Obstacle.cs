using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageable
{
    [SerializeField] LayerMask collLayer;

    public void Attacked(float damage, GameObject hitObject)
    {
        if(hitObject.GetComponent<ProjectileObjectPool>() != null)
        {
            hitObject.GetComponent<ProjectileObjectPool>().DestroyProjectile();
        }

        else if(hitObject.GetComponent<MonsterBullet>() != null)
        {
            hitObject.GetComponent<MonsterBullet>().DestroyBullet();
        }
    }

    public void RendDamageUI(float damage, Vector3 rendPos, bool canCri, bool isCri)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    { 
        //if ((collLayer | (1 << other.gameObject.layer )) != collLayer)
        if(other.gameObject.layer == LayerMask.NameToLayer("MouseInteraction"))
        {
            Destroy(other.gameObject);
        }
    }
}
