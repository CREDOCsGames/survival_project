using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject interactionUI;
    [SerializeField] GameObject buffIcon;
    [SerializeField] GameObject fireImage;

    GameManager gameManager;

    bool canInteraction = false;
    Vector3 fireInitScale;

    Dictionary<Buff, int> buffValues = new Dictionary<Buff, int>();
    int beforeBuff = -1;
    bool isFire = false;
    bool buffInteraction = true;

    private void Start()
    {
        gameManager = GameManager.Instance;
        fireInitScale = fireImage.transform.localScale;

        interactionUI.SetActive(false);
        buffIcon.SetActive(false);
    }

    private void Update()
    {
        if (!isFire)
            fireImage.transform.localScale = fireInitScale * (gameManager.currentGameTime / gameManager.gameDayTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            interactionUI.SetActive(true);
            canInteraction = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            interactionUI.SetActive(false);
            canInteraction = false;
        }
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (!canInteraction || gameManager.woodCount < 10 || isFire)
            return;

        gameManager.woodCount -= 10;
        fireImage.transform.localScale = fireInitScale;
        isFire = true;
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        if (!canInteraction || !buffInteraction || (gameManager.fishLowGradeCount <= 0 && gameManager.fishHighGradeCount <= 0))
            return;

        interactionUI.SetActive(false);

        int buffType = Random.Range(0, (int)Buff.COUNT);

        if (!buffValues.ContainsKey((Buff)buffType))
            buffValues.Add((Buff)buffType, 1);

        if (gameManager.fishHighGradeCount > 0)
        {
            if (buffValues[(Buff)buffType] < 2)
                buffValues[(Buff)buffType] = 2;

            gameManager.fishHighGradeCount--;
        }

        else
        {
            gameManager.fishLowGradeCount--;
        }

        if (beforeBuff == buffType)
            buffValues[(Buff)buffType] = Mathf.Clamp(buffValues[(Buff)buffType] + 1, 1, 3);

        beforeBuff = buffType;

        buffIcon.GetComponent<FishBuff>().SetBuff(buffType, buffValues[(Buff)buffType]);
        buffIcon.SetActive(true);

        StartCoroutine(BuffCoolTime(1.5f));
    }

    IEnumerator BuffCoolTime(float time)
    {
        buffInteraction = false;
        yield return new WaitForSeconds(time);

        interactionUI.SetActive(true);
        buffInteraction = true;
    }

    public void CanInteraction(bool canInteraction)
    {
        
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
