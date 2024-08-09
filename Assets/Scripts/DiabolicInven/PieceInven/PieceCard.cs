using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceCard : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Text itemName;
    [SerializeField] Text maxCount;
    [SerializeField] GameObject[] descriptPrefabs;
    

    GameManager gameManager;

    DiabolicItemInfo item;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        UpdateCardUI(item);
    }

    public void UpdateCardUI(DiabolicItemInfo item)
    {
        itemImage.sprite = item.ItemSprite;
        itemName.text = item.ItemName;
        maxCount.text = item.MaxCount.ToString();
        DescriptionInfo(item);
    }

    void DescriptionInfo(DiabolicItemInfo item)
    {
        int max = descriptPrefabs.Length;
        int count = 0;
        Dictionary<Status, int> itemStatus = item.Stat();

        for (int i = 0; i < gameManager.status.Count; i++)
        {
            if (itemStatus[(Status)i] > 0)
            {
                descriptPrefabs[count].transform.GetChild(0).GetComponent<Text>().text = GameManager.statNames[i];
                descriptPrefabs[count].transform.GetChild(2).GetComponent<Text>().text = ($"+{itemStatus[(Status)i]}");

                /*if (gameManager.status[(Status)i] < 0)
                {
                    descriptPrefabs[count].transform.GetChild(2).GetComponent<Text>().color = Color.red;
                    descriptPrefabs[count].transform.GetChild(2).GetComponent<Text>().text = stats[i].ToString();
                }*/

                descriptPrefabs[count].transform.GetChild(3).gameObject.SetActive(false);
                count++;
            }
        }

        for (int i = max - 1; i >= count; i--)
        {
            if (i == count)
            {
                descriptPrefabs[count].transform.GetChild(0).gameObject.SetActive(false);
                descriptPrefabs[count].transform.GetChild(1).gameObject.SetActive(false);
                descriptPrefabs[count].transform.GetChild(2).gameObject.SetActive(false);
                descriptPrefabs[count].transform.GetChild(3).gameObject.SetActive(true);
                descriptPrefabs[count].transform.GetChild(3).GetComponent<Text>().text = item.Description;
            }

            else
                descriptPrefabs[i].gameObject.SetActive(false);
        }
    }

    public void GetRandomItem(DiabolicItemInfo itemInfo)
    {
        item = itemInfo;
    }

    public void OnSelect()
    {
        GamesceneManager.Instance.cardSelecter.SetActive(false);
        ItemManager.Instance.AddItem(item);
    }
}