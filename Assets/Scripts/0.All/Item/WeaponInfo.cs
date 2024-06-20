using UnityEngine;

public enum Grade
{
    ÀÏ¹İ,
    Èñ±Í,
    Àü¼³,
    ½ÅÈ­,
}

[CreateAssetMenu(fileName = "new Weapon", menuName = "GameData/Item/Weapon")]
public class WeaponInfo : ScriptableObject
{
    public enum WEAPON_TYPE
    {
        ÃÑ,
        °Ë,
        ½ºÅÂÇÁ,
    }

    [HideInInspector] public Grade weaponGrade;

    [SerializeField] Sprite itemSprite;
    [SerializeField] string weaponName;
    [SerializeField] WEAPON_TYPE type;
    [SerializeField] float weaponDamage;
    [SerializeField] float magicDamage;
    [SerializeField] float attackDelay;
    [SerializeField] float bulletSpeed;
    [SerializeField] float weaponRange;
    [SerializeField] int weaponPrice;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] AudioClip weaponSound;
    [SerializeField] protected CharacterInfo[] useCharacter;

    public Sprite ItemSprite => itemSprite;
    public string WeaponName => weaponName;
    public WEAPON_TYPE Type => type;
    public float WeaponDamage => weaponDamage;
    public float MagicDamage => magicDamage;
    public float AttackDelay => attackDelay;
    public float BulletSpeed => bulletSpeed;
    public float WeaponRange => weaponRange;
    public int WeaponPrice => weaponPrice;
    public string Description => description;
    public AudioClip WeaponSound => weaponSound;

    public CharacterInfo[] UseCharacter => useCharacter;
}
