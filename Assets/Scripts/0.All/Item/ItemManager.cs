using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] public DamageUI[] damageUI;
    [SerializeField] public Transform coinStorage;
    [SerializeField] PassiveCardUI passiveCard;

    [HideInInspector] public WeaponInfo[] storedWeapon;
    [HideInInspector] public PassiveInfo[] storedPassive;
    [HideInInspector] public int[] storedPassiveCount;

    [HideInInspector] public int weaponCount;
    int getPassiveCount;

    [HideInInspector] public int equipFullCount;

    [HideInInspector] public Grade[] weaponGrade;

    [HideInInspector] public PassiveInfo[] lockedPassCards;
    [HideInInspector] public WeaponInfo[] lockedWeaCards;
    [HideInInspector] public bool[] cardLocks;
    [HideInInspector] public Grade[] cardGrades;

    [HideInInspector] public int[] passiveCounts;       // 패시브 템 최대갯수

    public Grade[] selectedGrades;

    public int thunderCount;
    public bool[] isThunderCountChange = new bool[6];

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        weaponGrade = new Grade[6];
        passiveCounts = new int[passiveCard.passiveInfo.Length];

        for (int i = 0; i < passiveCounts.Length; i++)
            passiveCounts[i] = passiveCard.passiveInfo[i].MaxCount;

        equipFullCount = 0;
        weaponCount = 0;
        getPassiveCount = 0;
        storedWeapon = new WeaponInfo[6];
        storedPassive = new PassiveInfo[90];
        storedPassiveCount = new int[storedPassive.Length];
        lockedPassCards = new PassiveInfo[4];
        lockedWeaCards = new WeaponInfo[4];
        selectedGrades = new Grade[4];
        cardLocks = new bool[4] { false, false, false, false };
        cardGrades = new Grade[4];
    }

    public void GetWeaponInfo(WeaponInfo weaponInfo)
    {
        if (equipFullCount <= 6)
        {
            for (int i = 0; i < storedWeapon.Length; i++)
            {
                if (storedWeapon[i] == null)
                {
                    weaponCount = i;
                    break;
                }
            }

            storedWeapon[weaponCount] = weaponInfo;
        }
    }

    public void GetPassiveInfo(PassiveInfo passiveInfo)
    {
        storedPassive[getPassiveCount] = passiveInfo;

        if (getPassiveCount == 0)
            storedPassiveCount[getPassiveCount]++;

        else if (getPassiveCount > 0)
            CheckItemEquel();

        getPassiveCount++;
    }

    void CheckItemEquel()
    {
        for (int i = 0; i < getPassiveCount; i++)
        {
            if (storedPassive[getPassiveCount] == storedPassive[i])
            {
                storedPassive[getPassiveCount] = null;
                storedPassiveCount[i]++;
                getPassiveCount--;
                break;
            }

            else
            {
                if (i == getPassiveCount - 1)
                    storedPassiveCount[getPassiveCount]++;
            }
        }
    }
}