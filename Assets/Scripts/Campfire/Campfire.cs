using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Debuff
{
    RECOVERY_HEALTH,
    ATTACK_SPEED,
    SPEED,
    POWER,
    COUNT,
}

public class Campfire : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject interactionUI;
    [SerializeField] GameObject buffIcon;
    [SerializeField] GameObject debuffIcon;
    [SerializeField] GameObject fireImage;

    GameManager gameManager;
    Character character;
    GamesceneManager gamesceneManager;

    bool canInteraction = false;
    bool canCookFish = false;
    Vector3 fireInitScale;

    Dictionary<Buff, int> buffValues = new Dictionary<Buff, int>();
    Buff beforeBuff = Buff.COUNT;

    Dictionary<Debuff, int> debuffValues = new Dictionary<Debuff, int>();

    bool isWoodRefill = false;

    int[] mxHps = { 0, 15, 30, 50 };
    int[] reHps = { 0, 20, 30, 40 };
    int[] speeds = { 0, 5, 7, 10 };
    int[] avoids = { 0, 3, 5, 7 };
    int[] dmgs = { 0, 10, 15, 20 };
    int[] dfs = { 0, 0, 5, 10 };

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Character.Instance;
        gamesceneManager = GamesceneManager.Instance;

        fireInitScale = fireImage.transform.localScale;

        for (int i = 0; i < (int)Buff.COUNT; i++)
        {
            buffValues.Add((Buff)i, 1);
            debuffValues.Add((Debuff)i, 0);
        }

        buffIcon.SetActive(false);
        debuffIcon.SetActive(false);

        fireImage.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        ChangeFireImageScale();
    }

    void ChangeFireImageScale()
    {
        if (isWoodRefill && gamesceneManager.isNight)
        {
            fireImage.transform.localScale = fireInitScale * (gamesceneManager.currentGameTime / gameManager.gameNightTime);
        }
    }

    public void OnDebuff()
    {
        if (isWoodRefill)
            return;

        Debuff debuffType = (Debuff)Random.Range(0, (int)Debuff.COUNT);

        debuffValues[debuffType] = 1;

        character.recoverHpRatio = gameManager.recoverHp * (10 - debuffValues[Debuff.RECOVERY_HEALTH]) * 0.1f;

        character.attackSpeed = gameManager.attackSpeed * (10 + debuffValues[Debuff.ATTACK_SPEED]) * 0.1f;

        character.speed = gameManager.speed * (10 - debuffValues[Debuff.SPEED] * 2) * 0.1f;

        gameManager.percentDamage = (100 - (debuffValues[Debuff.POWER]) * 20) * 0.01f;

        debuffIcon.GetComponent<CampFireDebuff>().SetDebuff(debuffType);
        debuffIcon.SetActive(true);
    }

    void OnBuff()
    {
        for (int i = 0; i < buffValues.Count; i++)
        {
            if (i != (int)beforeBuff)
            {
                buffValues[(Buff)i] = 0;
            }
        }

        character.maxHp = gameManager.maxHp * (100 + mxHps[buffValues[Buff.MAXHEALTH]]) * 0.01f;
        character.currentHp = character.maxHp;

        character.recoverHpRatio = gameManager.recoverHp * (100 + reHps[buffValues[Buff.RECOVERY_HEALTH]]) * 0.01f;

        character.speed = gameManager.speed * (100 + speeds[buffValues[Buff.SPEED]]) * 0.01f;
        character.avoid = gameManager.avoid +  avoids[buffValues[Buff.SPEED]];
        gameManager.dashCount = buffValues[Buff.SPEED] == 3 ? 1 : 0;
        character.dashCount = gameManager.dashCount;

        gameManager.percentDamage = (100 + dmgs[buffValues[Buff.POWER]]) * 0.01f;
        character.defence = gameManager.defence + dfs[buffValues[Buff.POWER]];
    }

    void InitializeBuff()
    {
        character.maxHp = gameManager.maxHp * (100 + mxHps[buffValues[Buff.MAXHEALTH]]) * 0.01f;
        character.currentHp = character.maxHp;

        character.recoverHpRatio = gameManager.recoverHp * (100 + reHps[buffValues[Buff.RECOVERY_HEALTH]]) * 0.01f;

        character.speed = gameManager.speed * (100 + speeds[buffValues[Buff.SPEED]]) * 0.01f;
        character.avoid = gameManager.avoid + avoids[buffValues[Buff.SPEED]];
        gameManager.dashCount = buffValues[Buff.SPEED] == 3 ? 1 : 0;
        character.dashCount = gameManager.dashCount;

        gameManager.percentDamage = (100 + dmgs[buffValues[Buff.POWER]]) * 0.01f;
        character.defence = gameManager.defence + dfs[buffValues[Buff.POWER]];
    }

    public void ToNightScene()
    {
        OnBuff();
        OnDebuff();
        canInteraction = false;
        interactionUI.SetActive(false);
    }

    public void ToDayScene()
    {
        OffBuffNDebuff();
        character.maxHp = gameManager.maxHp;
        character.recoverHpRatio = gameManager.recoverHp;
        character.speed = gameManager.speed;
        character.defence = gameManager.defence;
        character.attackSpeed = gameManager.attackSpeed;
        character.avoid = gameManager .avoid;
        gameManager.dashCount = 0;
        gameManager.percentDamage = 1;
        isWoodRefill = false;

        character.InitailizeDashCool();
    }

    public void OffBuffNDebuff()
    {
        for (int i = 0; i < (int)Buff.COUNT; i++)
        {
            buffValues[(Buff)i] = 1;
            debuffValues[(Debuff)i] = 0;
        }

        InitializeBuff();

        debuffIcon.SetActive(false);
        buffIcon.SetActive(false);
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (!canInteraction)
            return;

        OnFire();

        OnFish();
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        
    }



    void OnFire()
    {
        if (isWoodRefill || gameManager.woodCount < 10)
            return;

        gameManager.woodCount -= 10;
        fireImage.transform.localScale = fireInitScale;
        isWoodRefill = true;

        StartCoroutine(BuffCoolTime(1.5f));
    }

    void OnFish()
    {
        if (!canCookFish || !isWoodRefill || gameManager.fishLowGradeCount <= 0 && gameManager.fishHighGradeCount <= 0)
            return;

        interactionUI.SetActive(false);

        Buff buffType = (Buff)Random.Range(0, (int)Buff.COUNT);

        if (gameManager.fishHighGradeCount > 0)
        {
            if (buffValues[buffType] < 2)
                buffValues[buffType] = 2;

            gameManager.fishHighGradeCount--;
        }

        else
        {
            gameManager.fishLowGradeCount--;
        }

        if (beforeBuff == buffType)
            buffValues[buffType] = Mathf.Clamp(buffValues[buffType] + 1, 1, 3);

        beforeBuff = buffType;

        buffIcon.GetComponent<FishBuffIcon>().SetBuffIcon(buffType, buffValues[buffType]);
        buffIcon.SetActive(true);

        StartCoroutine(BuffCoolTime(1.5f));
    }

    IEnumerator BuffCoolTime(float time)
    {
        canCookFish = false;
        yield return new WaitForSeconds(time);

        if (!gamesceneManager.isNight)
        {
            interactionUI.SetActive(true);
            canCookFish = true;
        }
    }

    public void CanInteraction(bool _canInteraction)
    {
        canInteraction = _canInteraction;
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    public bool ReturnCanInteraction()
    {
        return canInteraction;
    }
}
