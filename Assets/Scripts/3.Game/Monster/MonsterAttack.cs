using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField] AudioClip attackSound;
    
    SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void Attack()
    {
        soundManager.PlaySFX(attackSound);
    }
}
