using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypingText : MonoBehaviour
{
    [SerializeField] Text typingText;
    [SerializeField] string textMasage;
    [SerializeField] float typingSpeed;

    [HideInInspector] public bool isOver = false;

    private void Start()
    {
        StartCoroutine(Typing(typingText, textMasage, typingSpeed));
    }

    IEnumerator Typing(Text typingText, string message, float speed)
    {
        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);

            yield return CoroutineCaching.WaitForSeconds(speed);
        }

        yield return CoroutineCaching.WaitForSeconds(1);

        isOver = true;
    }
}
