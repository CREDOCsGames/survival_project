using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FishDifficultyPreset",
    menuName = "Fishing/Fish Difficulty Preset"
)]
public class FishDifficulty : ScriptableObject
{
    [Header("Idle Time (sec)")]
    public float minIdleTime = 1.0f;
    public float maxIdleTime = 1.0f;

    [Header("Move Time (sec)")]
    public float minMoveTime = 1.0f;
    public float maxMoveTime = 1.0f;

    [Header("Move Distance (normalized)")]
    [Range(0.01f, 1f)]public float minMoveDistance = 0.0f;
    [Range(0.01f, 1f)]public float maxMoveDistance = 1.0f;
}
