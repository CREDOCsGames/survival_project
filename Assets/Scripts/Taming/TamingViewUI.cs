using UnityEngine;
using UnityEngine.UI;

public class TamingViewUI : MonoBehaviour
{
    [SerializeField] Slider tamingGauge;
    [SerializeField] float gaugeSpeed = 1f;
    [SerializeField] Text catchText;
    [SerializeField] GameObject tamingPet;

    [HideInInspector] public bool isGaugeUp = false;

    public GameObject catchPet;

    private void Start()
    {
        tamingGauge.value = 0;

        catchText.gameObject.SetActive(false);
        Cursor.visible = false;     // esc 누르면 다시 초기화됨.
    }

    private void Update()
    {
        if(tamingGauge.value < 1)
        tamingGauge.value += isGaugeUp ? Time.deltaTime * gaugeSpeed : -Time.deltaTime * gaugeSpeed * 0.5f;

        else
        {
            tamingPet.GetComponent<TamingGamePetMove>().isCatch = false;
            catchText.gameObject.SetActive(true);

            if (catchPet != null)
                StartCoroutine(catchPet.GetComponent<IMouseInteraction>().EndInteraction(null, 3));
        }
    }
}
