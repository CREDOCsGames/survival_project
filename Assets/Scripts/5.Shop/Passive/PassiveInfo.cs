using UnityEngine;

[CreateAssetMenu(fileName = "new Item", menuName = "GameData/Item/Passive")]
public class PassiveInfo : ScriptableObject
{
    [SerializeField] Sprite itemSprite;
    [SerializeField] Grade itemGrade;
    [SerializeField] string itemName;
    [SerializeField] int itemPrice;
    [SerializeField] int maxCount;

    [Header("Stat")]
    [SerializeField] float hp;
    [SerializeField] float recoverHp;
    [SerializeField] float absorbHp;
    [SerializeField] float defence;
    [SerializeField] float physicDamage;
    [SerializeField] float magicDamage;
    [SerializeField] float shortDamage;
    [SerializeField] float longDamage;
    [SerializeField] float attackSpeed;
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] float luck;
    [SerializeField] float critical;
    [SerializeField] float percentDamage;
    [SerializeField] float avoid;

    [Header("Item")]
    [SerializeField] float coinRange;
    [SerializeField] float increaseExp;
    [SerializeField] float monsterSpeed;
    [SerializeField] float salePercent;
    [SerializeField] float summonASpd;
    [SerializeField] float summonPDmg;
    [SerializeField] float monsterDef;
    [SerializeField] int dashCount;
    [SerializeField] int buffNum;
    [SerializeField] int exDmg;
    [SerializeField] int isedolCount;
    [SerializeField] bool luckCoin;
    [SerializeField] bool luckDamage;
    [SerializeField] bool luckCritical;
    [SerializeField] bool doubleShot;
    [SerializeField] bool revive;
    [SerializeField] bool ggoGgo;
    [SerializeField] bool ilsoon;
    [SerializeField] bool wakgood;
    [SerializeField] bool ddilpa;
    [SerializeField] bool butterfly;
    [SerializeField] bool subscriptionFee;
    [SerializeField] bool spawnTree;
    [SerializeField] bool dotgu;
    [SerializeField] bool isReflect;
    [SerializeField] bool onePenetrate;
    [SerializeField] bool lowPenetrate;
    [SerializeField] bool penetrate;
    [SerializeField] bool vamAbsorb;
    [TextArea]
    [SerializeField] string description;

    [HideInInspector] public float weight;

    public Sprite ItemSprite => itemSprite;
    public Grade ItemGrade => itemGrade;
    public string ItemName => itemName;
    public int MaxCount => maxCount;
    public float PhysicDamage => physicDamage;
    public float MagicDamage => magicDamage;
    public float ShortDamage => shortDamage;
    public float LongDamage => longDamage;
    public float Range => range;
    public float Defence => defence;
    public float Speed => speed;
    public float Luck => luck;
    public float Hp => hp;
    public float AbsorbHp => absorbHp;
    public float RecoverHp => recoverHp;
    public float AttackSpeed => attackSpeed;
    public int ItemPrice => itemPrice;
    public float Critical => critical;
    public float MonsterSpeed => monsterSpeed;
    public float SalePercent => salePercent;
    public int DashCount => dashCount;
    public int BuffNum => buffNum;
    public int ExDmg => exDmg;
    public int IsedolCount => isedolCount;
    public float CoinRange => coinRange;
    public float IncreaseExp => increaseExp;
    public float SummonASpd => summonASpd;
    public float SummonPDmg => summonPDmg;
    public float MonsterDef => monsterDef;
    public bool LuckCoin => luckCoin;
    public bool LuckDamage => luckDamage;
    public bool LuckCritical => luckCritical;
    public bool DoubleShot => doubleShot;
    public bool Revive => revive;
    public bool GgoGgo => ggoGgo;
    public bool Ilsoon => ilsoon;
    public bool Wakgood => wakgood;
    public bool Ddilpa => ddilpa;
    public bool Butterfly => butterfly;
    public bool SubscriptionFee => subscriptionFee;
    public bool SpawnTree => spawnTree;
    public bool Dotgu => dotgu;
    public bool IsReflect => isReflect;
    public bool OnePenetrate => onePenetrate;
    public bool LowPenetrate => lowPenetrate;
    public bool Penetrate => penetrate;
    public bool Vamabsorb => vamAbsorb;
    public string Description => description;
    public float PercentDamage => percentDamage;
    public float Avoid => avoid;
}
