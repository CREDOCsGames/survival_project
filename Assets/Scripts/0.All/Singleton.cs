using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour                             // T �ڷ����� Monobehaviour�� ����ϰ� �־�� �Ѵ�.
{
    static T instance;
    public static T Instance => instance;

    protected virtual void Awake()                              // ObjectPool�� Awake���� ȣ���ϱ� ���� Protected
    {
        instance = this as T;                           // ��(Singleton)�� T������ ��ȯ�� ����
    }
}
