using System.Collections;
using UnityEngine;

public class GatherFruit : MonoBehaviour, IMouseInteraction
{
    [SerializeField] float defaultGaugeUpValue;

    bool canGather;

    Character character;

    private void Start()
    {
        canGather = false;
        character = Character.Instance;
    }

    public void CanInteraction(bool canInteraction)
    {
        canGather = canInteraction;
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (canGather)
        {
            StartCoroutine(character.MoveToInteractableObject(transform.position, gameObject));
        }
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        RecoveryGaugeUp();
        Destroy(gameObject);

        anim.SetBool("isLogging", false);
        character.isCanControll = true;
        character.ChangeAnimationController(0);
    }

    void RecoveryGaugeUp()
    {
        int rand = Random.Range(0, 100);
        int fruitNum = (rand >= 0 && rand < 80) ? 0 : 1;
        float ratio = (fruitNum == 0) ? 1 : 1.5f;

        character.fruitUI.GetComponent<FruitUI>().SetImage(fruitNum);
        character.fruitUI.gameObject.SetActive(true);

        character.currentRecoveryGauge += defaultGaugeUpValue * ratio;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        
    }
#endif
}
