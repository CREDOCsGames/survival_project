using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingAnim : MonoBehaviour
{
    [SerializeField] AudioClip throwSound;
    [SerializeField] AudioClip catchingSound;

    Animator anim;

    [HideInInspector] public bool isCatch;
    [HideInInspector] public bool CatchSuccess;

    SoundManager soundManager;

    EffectSound currentSfx;

    [SerializeField] Fishing fishing;
    private void Start()
    {
        anim = GetComponent<Animator>();
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

    public void ThrowSoundStart()
    {
        soundManager.PlaySFX(throwSound);
    }

    public void CatchingSoundStart()
    {
       currentSfx = soundManager.PlaySFXAndReturn(catchingSound, true);
    }

    public void CatchingSoundStop()
    {
        if (currentSfx != null)
            soundManager.StopLoopSFX(currentSfx);
    }

    public void IsCatchingStart()
    {
        fishing.isCatchingStart = true;
        StartCoroutine(fishing.CatchingStart());
    }

    public void IsCatchingEnd()
    {
        StartCoroutine(fishing.FishingEnd());
    }
}
