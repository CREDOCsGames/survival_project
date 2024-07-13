using UnityEngine;
using UnityEngine.UI;

public class TamingViewUI : MonoBehaviour
{
    [SerializeField] Slider tamingGauge;
    [SerializeField] float gaugeSpeed = 1f;

    [HideInInspector] public bool isGaugeUp = false;

    private void Start()
    {
        tamingGauge.value = 0;

        Cursor.visible = false;     // esc 누르면 다시 초기화됨.
    }

    private void Update()
    {
        tamingGauge.value += isGaugeUp ? Time.deltaTime * gaugeSpeed : -Time.deltaTime * gaugeSpeed;
    }
}
