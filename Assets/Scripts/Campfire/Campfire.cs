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
    Vector3 fireInitScale;

    Dictionary<Buff, int> buffValues = new Dictionary<Buff, int>();
    Buff beforeBuff = Buff.COUNT;

    Dictionary<Debuff, int> debuffValues = new Dictionary<Debuff, int>();

    bool isWoodRefill = false;
    bool buffInteraction = true;

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
    }

    private void Update()
    {
        ChangeFireImageScale();
    }

    void ChangeFireImageScale()
    {
        if (!isWoodRefill && !gamesceneManager.isNight)
        {
            fireImage.transform.localScale = fireInitScale * (gamesceneManager.currentGameTime / gameManager.gameDayTime);
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

        // 이거 clamp로 처리하기

        character.maxHp = buffValues[Buff.MAXHEALTH] != 0 ? gameManager.maxHp * ((15 * buffValues[Buff.MAXHEALTH]) + (5 * (buffValues[Buff.MAXHEALTH] - 1))) : gameManager.maxHp;

        character.recoverHpRatio = buffValues[Buff.RECOVERY_HEALTH] != 0 ? gameManager.recoverHp * (10 + buffValues[Buff.RECOVERY_HEALTH]) * 0.1f : gameManager.recoverHp;

        character.speed = buffValues[Buff.SPEED] != 0 ? gameManager.speed * (100 + (Mathf.Pow(buffValues[Buff.SPEED], 2) + buffValues[Buff.SPEED] + 8) * 0.5f) * 0.01f : gameManager.speed;
        character.avoid = buffValues[Buff.SPEED] != 0 ? gameManager.avoid + (1 + 2 * buffValues[Buff.SPEED]) * 0.01f : gameManager.avoid;
        gameManager.dashCount = buffValues[Buff.SPEED] == 3 ? 1 : 0;

        gameManager.percentDamage = (100 + 10 * (buffValues[Buff.POWER])) * 0.01f;
        character.defence = gameManager.defence + 5 * buffValues[Buff.POWER];
    }

    public void ToNightScene()
    {
        OnBuff();
        OnDebuff();
        canInteraction = false;
        isWoodRefill = false;
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

        buffInteraction = true;
    }

    public void OffBuffNDebuff()
    {
        for (int i = 0; i < (int)Buff.COUNT; i++)
        {
            buffValues[(Buff)i] = 1;
            debuffValues[(Debuff)i] = 0;
        }

        OnBuff();
        OnDebuff();

        debuffIcon.SetActive(false);
        buffIcon.SetActive(false);
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (!canInteraction || gameManager.woodCount < 10 || isWoodRefill)
            return;

        gameManager.woodCount -= 10;
        fireImage.transform.localScale = fireInitScale;
        isWoodRefill = true;
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        if (!canInteraction || !buffInteraction || (gameManager.fishLowGradeCount <= 0 && gameManager.fishHighGradeCount <= 0))
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
        buffInteraction = false;
        yield return new WaitForSeconds(time);

        if (!gamesceneManager.isNight)
        {
            interactionUI.SetActive(true);
            buffInteraction = true;
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
