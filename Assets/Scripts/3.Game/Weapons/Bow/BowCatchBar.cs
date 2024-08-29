using UnityEngine;
using UnityEngine.UI;

public class BowCatchBar : MonoBehaviour
{
    [SerializeField] Slider catchBar;

    float barSpeed;
    float maxBarSpeed = 600;

    bool isMin = true;

    bool isCatch = false;

    private void OnEnable()
    {
        catchBar.gameObject.SetActive(false);
        catchBar.value = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (GetComponent<ShootArrow>().checkCanFire())
            {
                catchBar.gameObject.SetActive(true);
                MoveBar();
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            Catch();
            catchBar.gameObject.SetActive(false);
            catchBar.value = 0;
        }
    }

    void MoveBar()
    {
        if (catchBar.value == catchBar.minValue)
            isMin = true;

        else if (catchBar.value == catchBar.maxValue)
            isMin = false;

        barSpeed = Mathf.Clamp(catchBar.value <= catchBar.maxValue * 0.5f ? (catchBar.value / (catchBar.maxValue * 0.5f)) * maxBarSpeed : ((catchBar.maxValue - catchBar.value) / (catchBar.maxValue * 0.5f)) * maxBarSpeed, 100, maxBarSpeed);

        catchBar.value += isMin ? Time.deltaTime * barSpeed : -Time.deltaTime * barSpeed;
    }

    void Catch()
    {
        isCatch = catchBar.value >= 77 && catchBar.value <= 123 ? true : false;
    }

    public bool IsCatch()
    {
        return isCatch;
    }
}
