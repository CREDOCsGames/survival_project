using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingAnim : Singleton<FishingAnim>
{
    Animator anim;

    [HideInInspector] public bool isCatch;
    [HideInInspector] public bool CatchSuccess;
    [HideInInspector] public int isSomeCatch;

    Fishing fishing;

    private void Start()
    {
        anim = GetComponent<Animator>();
        fishing = Fishing.Instance;

        isCatch = false;
    }

    void Update()
    {
        anim.SetBool("isCatch", isCatch);
        anim.SetBool("CatchSuccess", CatchSuccess);
        anim.SetInteger("Catch", isSomeCatch);
    }

    public void IsCatchingStart()
    {
        fishing.isCatchingStart = true;
    }

    public void IsCatchingEnd()
    {
        StartCoroutine(fishing.FishingEnd());
    }
}
