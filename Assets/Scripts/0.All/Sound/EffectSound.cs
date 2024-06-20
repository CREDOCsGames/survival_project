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
        while (source.isPlaying)        // ���� �÷��� ���̶��
            yield return null;          // �������� ���

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
