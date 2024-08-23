using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Buff
{
    MAXHEALTH,
    RECOVERY_HEALTH,
    SPEED,
    POWER,
    COUNT,
}

public class FishBuffIcon : MonoBehaviour
{
    [SerializeField] Sprite[] buffImages;
    [SerializeField] Image currentBuffImage;
    [SerializeField] Text buffValueText;

    public void SetBuffIcon(Buff _buffType, int _buffValue)
    {
        currentBuffImage.sprite = buffImages[(int)_buffType];
        buffValueText.text = _buffValue.ToString();
    }
}
