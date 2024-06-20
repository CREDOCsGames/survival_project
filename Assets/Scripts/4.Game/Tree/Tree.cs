using UnityEngine;


public class Tree : MonoBehaviour
{
    [SerializeField] GameObject[] potionPrefab;
    [SerializeField] SpriteRenderer rend;

    int potionsNum;

    GameManager gameManager;

    bool isAttaked = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        potionsNum = gameManager.buffNum;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("myBullet") || other.CompareTag("Sword") || other.CompareTag("Thunder"))
        {
            if (!isAttaked)
            {
                gameManager = GameManager.Instance;

                isAttaked = true;
                GameObject potion = Instantiate(potionPrefab[potionsNum]);
                potion.transform.position = transform.position;
                gameManager.woodCount++;

                float num = Random.Range(0f, 100f);

                if (num < 3 + Mathf.Clamp(gameManager.luck, 0f, 100f) * 0.4f && gameManager.currentGameTime > 0)
                {
                    SoundManager.Instance.PlayES("ItemGet");
                    GameSceneUI.Instance.chestCount++;
                }

                Destroy(gameObject);
            }
        }

        else if (other.CompareTag("Character"))
            rend.color = new Color(1, 1, 1, 0.5f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
            rend.color = new Color(1, 1, 1, 1);
    }
}
