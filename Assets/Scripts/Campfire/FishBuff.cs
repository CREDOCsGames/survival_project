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

    public void SetBuff(int _buffType, int _buffValue)
    {
        currentBuffImage.sprite = buffImages[_buffType];
        buffValueText.text = _buffValue.ToString();

        buffType = (Buff)_buffType;
        buffValue = _buffValue;
    }
}
