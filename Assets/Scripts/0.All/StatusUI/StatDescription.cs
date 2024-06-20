using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDescription : MonoBehaviour
{
    [SerializeField] GameObject descripPanel;

    void Start()
    {
        descripPanel.SetActive(false);
    }

    public void ShowStatDescription(int num)
    {
        descripPanel.SetActive(true);
        descripPanel.GetComponent<DescriptionUI>().SetTextInfo(num);
    }

    public void CloseStatDescription()
    {
        descripPanel.SetActive(false);
    }
}
