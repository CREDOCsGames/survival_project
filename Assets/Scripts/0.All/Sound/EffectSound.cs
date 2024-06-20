using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EffectSound : MonoBehaviour
{
    [SerializeField] public AudioSource source;

    private IObjectPool<EffectSound> managedPool;

    public void PlayES(AudioClip clip)
    {
        source.clip = clip;
        source.loop = false;
        source.Play();

        StartCoroutine(CheckPlay());
    }

    IEnumerator CheckPlay()
    {
        while (source.isPlaying)        // 만약 플레잉 중이라면
            yield return null;          // 한프레임 대기

        DestroyPool();
    }

    public void SetManagedPool(IObjectPool<EffectSound> pool)
    {
        managedPool = pool;
    }

    public void DestroyPool()
    {
        managedPool.Release(this);
    }
}
