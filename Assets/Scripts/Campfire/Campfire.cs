using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject interactionUI;
    [SerializeField] GameObject buffIcon;
    [SerializeField] GameObject debuffIcon;
    [SerializeField] GameObject fireImage;

    GameManager gameManager;
    GamesceneManager gamesceneManager;

    bool canInteraction = false;
    Vector3 fireInitScale;

    Dictionary<Buff, int> buffValues = new Dictionary<Buff, int>();
    Buff beforeBuff = Buff.COUNT;

    Dictionary<Buff, int> debuffValues = new Dictionary<Buff, int>();

    bool isWoodRefill = false;
    bool buffInteraction = true;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gamesceneManager = GamesceneManager.Instance;

        fireInitScale = fireImage.transform.localScale;

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

        Buff debuffType = (Buff)Random.Range(0, (int)Buff.COUNT);

        if (!buffValues.ContainsKey(debuffType))
            buffValues.Add(debuffType, 1);

        debuffIcon.GetComponent<CampFireDebuff>().SetDebuff(debuffType, buffValues[debuffType]);
        debuffIcon.SetActive(true);
    }

    public void ToNightScene()
    {
        canInteraction = false;
        isWoodRefill = false;
        interactionUI.SetActive(false);
    }

    public void ToDayScene()
    {
        buffInteraction = true;
    }

    public void OffBuffNDebuff()
    {
        for (int i = 0; i < (int)Buff.COUNT; i++)
        {
            if (!buffValues.ContainsKey((Buff)i))
                buffValues.Add((Buff)i, 1);

            buffValues[(Buff)i] = 1;
            debuffValues[(Buff)i] = 1;
        }

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

        if (!buffValues.ContainsKey(buffType))
            buffValues.Add(buffType, 1);

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

        buffIcon.GetComponent<FishBuff>().SetBuff(buffType, buffValues[buffType]);
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
