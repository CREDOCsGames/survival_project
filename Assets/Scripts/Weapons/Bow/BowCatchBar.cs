using UnityEngine;
using UnityEngine.UI;

public class BowCatchBar : MonoBehaviour
{
    [SerializeField] Slider catchBar;

    bool isMin = true;

    bool isCatch = false;

    private void Start()
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

        if (isMin)
            catchBar.value += Time.deltaTime * 400;

        else
            catchBar.value -= Time.deltaTime * 400;
    }

    void Catch()
    {
        isCatch = catchBar.value >= 80 && catchBar.value <= 120 ? true : false;
    }

    public bool IsCatch()
    {
        return isCatch;
    }
}
