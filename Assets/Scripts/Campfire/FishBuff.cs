using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Buff
{
    SPEED,
    POWER,
    ATTACK_SPEED,
    MAXHEALTH,
    COUNT,
}

public class FishBuff : MonoBehaviour
{
    [SerializeField] Sprite[] buffImages;
    [SerializeField] Image currentBuffImage;
    [SerializeField] Text buffValueText;

    Buff buffType;
    int buffValue;

    public void SetBuff(Buff _buffType, int _buffValue)
    {
        currentBuffImage.sprite = buffImages[(int)_buffType];
        buffValueText.text = _buffValue.ToString();

        buffType = _buffType;
        buffValue = _buffValue;
    }
}
