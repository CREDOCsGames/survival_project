using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingAnim : Singleton<FishingAnim>
{
    [SerializeField] AudioClip throwSound;
    [SerializeField] AudioClip catchingSound;

    Animator anim;

    [HideInInspector] public bool isCatch;
    [HideInInspector] public bool CatchSuccess;

    Fishing fishing;
    SoundManager soundManager;

    EffectSound currentSfx;

    private void Start()
    {
        anim = GetComponent<Animator>();
        fishing = Fishing.Instance;
        soundManager = SoundManager.Instance;

        isCatch = false;
    }

    void Update()
    {
        anim.SetBool("isCatch", isCatch);
        anim.SetBool("CatchSuccess", CatchSuccess);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ThrowAnimStart()
    {
        soundManager.PlaySFX(throwSound);
    }

    public void CatchingStartAnimStart()
    {
       currentSfx = soundManager.PlaySFXAndReturn(catchingSound, true);
    }

    public void IsCatchingStart()
    {
        fishing.isCatchingStart = true;
        StartCoroutine(fishing.CatchingStart());
    }

    public void IsCatchingEnd()
    {
        StartCoroutine(fishing.FishingEnd());

        if (currentSfx != null)
            soundManager.StopLoopSFX(currentSfx);
    }
}
