using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampFireDebuff : MonoBehaviour
{
    [SerializeField] Sprite[] debuffImages;
    [SerializeField] Image currentDEbuffImage;
    [SerializeField] Text debuffValueText;

    public void SetDebuff(Buff _buffType, int _buffValue)
    {
        currentDEbuffImage.sprite = debuffImages[(int)_buffType];
        debuffValueText.text = _buffValue.ToString();
    }
}
