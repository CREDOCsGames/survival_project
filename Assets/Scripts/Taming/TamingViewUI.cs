using UnityEngine;
using UnityEngine.UI;

public class TamingViewUI : MonoBehaviour
{
    [SerializeField] Slider tamingGauge;
    [SerializeField] float gaugeSpeed = 1f;
    [SerializeField] Text catchText;
    [SerializeField] Text failText;
    [SerializeField] GameObject tamingPet;

    [HideInInspector] public bool isGaugeUp = false;

    [HideInInspector] public GameObject catchPet;

    private void Start()
    {
        tamingGauge.value = 0.5f;

        tamingPet.SetActive(true);
        catchText.gameObject.SetActive(false);
        failText.gameObject.SetActive(false);

        Cursor.visible = false;     // esc 누르면 다시 초기화됨.
    }

    private void Update()
    {
        if (tamingGauge.value < 1)
        {
            tamingGauge.value += isGaugeUp ? Time.deltaTime * gaugeSpeed : -Time.deltaTime * gaugeSpeed * 0.8f;

            if (tamingGauge.value <= 0)
            {
                tamingPet.SetActive(false);
                failText.gameObject.SetActive(true);

                if (catchPet != null)
                    StartCoroutine(catchPet.GetComponent<IMouseInteraction>().EndInteraction(null, 2));
            }
        }

        else
        {
            tamingPet.GetComponent<TamingGamePetMove>().isCatch = false;
            catchText.gameObject.SetActive(true);

            if (catchPet != null)
                StartCoroutine(catchPet.GetComponent<IMouseInteraction>().EndInteraction(null, 3));
        }
    }
}
