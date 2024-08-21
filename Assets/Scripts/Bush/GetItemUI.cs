using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GetItemUI : MonoBehaviour
{
    [SerializeField] Sprite[] fruitImages;
    [SerializeField] Sprite bulletImage;
    [SerializeField] Image uiImage;
    [SerializeField] float fadeAwaySpeed;

    float imageAlpha = 1;

    private void OnEnable()
    {
        imageAlpha = 1;
        StartCoroutine(FadeAwayUI());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetFruitGetImage(int num)
    {
        uiImage.sprite = fruitImages[num];
    }

    public void SetBulletGetImage()
    {
        uiImage.sprite = bulletImage;
    }

    void AlphaChange()
    {
        Color uiColor = uiImage.color;        // 텍스트의 색상값
        uiColor.a = imageAlpha;              // 색상값의 알파값 변경(직접 변경 불가해서 빼옴)
        uiImage.color = uiColor;           // 변경한 색상을 대입
    }

    IEnumerator FadeAwayUI()
    {
        while (imageAlpha > 0)
        {
            AlphaChange();
            imageAlpha -= Time.deltaTime * fadeAwaySpeed;
            yield return null;
        }

        imageAlpha = 0;
        gameObject.SetActive(false);
    }
}
