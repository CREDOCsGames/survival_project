using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Character", menuName = "GameData/Character")]
public class CharacterInfo : ScriptableObject
{
    [SerializeField] RuntimeAnimatorController characterAnim;
    [SerializeField] Sprite characterImage;
    [SerializeField] string characterName;
    [SerializeField] float hpRate;
    [SerializeField] float damageRatio;
    [SerializeField] float characterSpeed;
    [SerializeField] float avoid;
    [SerializeField] float invincibleTime;

    public RuntimeAnimatorController CharacterAnim => characterAnim;
    public Sprite CharacterImage => characterImage;
    public string CharacterName => characterName;
    public float HpRate => hpRate;
    public float DamageRatio => damageRatio;
    public float CharacterSpeed => characterSpeed;
    public float Avoid => avoid;
    public float InvincibleTime => invincibleTime;
}
