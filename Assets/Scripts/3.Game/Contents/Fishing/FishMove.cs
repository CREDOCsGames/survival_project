using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove
{
    public float FishY => fishY; // 외부(Fishing)에서 읽기만

    enum State { Idle, Move }
    State state = State.Idle;

    float idleTimeMin = 0.2f;
    float idleTimeMax = 0.8f;

    float moveTimeMin = 0.3f;
    float moveTimeMax = 1.2f;

    //normalized value;
    float moveDistMin = 0.1f;
    float moveDistMax = 0.6f;

    public float minY = 0f;
    public float maxY = 1f;

    float fishY = 0.5f;

    float startY;
    float targetY;

    float timer;
    float duration;

    public void SetPreset(FishDifficulty preset)
    {
        idleTimeMin = preset.minIdleTime;
        idleTimeMax = preset.maxIdleTime;
        moveTimeMin = preset.minMoveTime;
        moveTimeMax = preset.maxMoveTime;
        moveDistMin = preset.minMoveDistance;
        moveDistMax = preset.maxMoveDistance;
    }
    public void OnEnable()
    {
        fishY = Mathf.Clamp01(fishY);
        EnterIdle();
    }

    public void Update()
    {
        Tick(Time.deltaTime);
    }

    void Tick(float dt)
    {
        timer += dt;

        switch (state)
        {
            case State.Idle:
                if (timer >= duration)
                    EnterMove();
                break;

            case State.Move:
                float t = Mathf.Clamp01(timer / duration);
                fishY = Mathf.Lerp(startY, targetY, EaseOut(t));

                if (t >= 1f)
                    EnterIdle();
                break;
        }
    }

    void EnterIdle()
    {
        state = State.Idle;
        timer = 0f;
        duration = Random.Range(idleTimeMin, idleTimeMax);
    }

    void EnterMove()
    {
        state = State.Move;
        timer = 0f;
        duration = Random.Range(moveTimeMin, moveTimeMax);

        startY = fishY;

        float dir = Random.value < 0.5f ? -1f : 1f;
        float dist = Random.Range(moveDistMin, moveDistMax);

        targetY = fishY + dir * dist;
        targetY = Mathf.Clamp(targetY, minY, maxY);
    }

    float EaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}